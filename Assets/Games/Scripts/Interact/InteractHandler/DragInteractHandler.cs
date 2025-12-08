using UnityEngine;

public class DragInteractHandler : BaseInteractHandler
{
    private BaseDraggableObject draggedObject;
    private Vector3 startPosScreen;
    private float startTime;
    private bool isDragging;
    private bool hasBegan;

    private const float dragThreshold = 5f;
    private const float tapMaxDuration = 0.25f;

    public DragInteractHandler(InteractHandler newInteractHandler) : base(newInteractHandler)
    {
        draggedObject = null;
    }

    public override void Refresh()
    {
        draggedObject = null;
        isDragging = false;
        hasBegan = false;
    }

    protected override void Cancel()
    {
        draggedObject = null;
        isDragging = false;
        hasBegan = false;
        base.Cancel();
    }

    public override void SetInteractObject(BaseInteractableObject interactableObject)
    {
        if (interactableObject == null) return;
        draggedObject = interactableObject.GetComponent<BaseDraggableObject>();
    }

    public override void HandleBeganPhase(Vector3 position)
    {
        hasBegan = true;
        startTime = Time.time;
        startPosScreen = position;
        isDragging = false;

        if (draggedObject)
        {
            if (draggedObject is IBeginTouch beginTouchable)
                beginTouchable.OnBeginTouch();

            draggedObject.BeginDrag();
            Vector3 touchPosition = interactHandler.GetScreenToWorldPosition(position);
            draggedObject.SetPosDifference(touchPosition);
        }
    }

    public override void HandleMovedPhase(Vector3 position)
    {
        if (!hasBegan || draggedObject == null)
            return;

        if (!isDragging && Vector2.Distance(position, startPosScreen) > dragThreshold)
        {
            isDragging = true;
            if (draggedObject is IBeginDrag beginDraggable)
                beginDraggable.OnBeginDrag();
        }

        if (isDragging)
        {
            Vector3 touchPosition = interactHandler.GetScreenToWorldPosition(position);
            draggedObject.SetPositionSmooth(touchPosition);
        }
    }

    public override void HandleEndedPhase(Vector3 position)
    {
        if (!hasBegan || draggedObject == null)
        {
            Cancel();
            return;
        }

        float heldTime = Time.time - startTime;
        float moveDist = Vector2.Distance(position, startPosScreen);

        if (draggedObject is IEndTouch endTouchable)
            endTouchable.OnEndTouch();

        if (!isDragging && heldTime <= tapMaxDuration && moveDist <= dragThreshold)
        {
            if (draggedObject is ITappable tappable)
            {
                tappable.OnTap();
            }
        }

        if (draggedObject.IsBeingDragged)
            draggedObject.EndDrag();
        else
            draggedObject.ResetDrag();

        Cancel();
    }

    public override void Restore()
    {
        if (draggedObject)
        {
            draggedObject.ResetDrag();
        }
        Cancel();
    }
}
public interface ITappable
{
    void OnTap();
}
public interface IBeginTouch
{
    void OnBeginTouch();
}
public interface IBeginDrag
{
    void OnBeginDrag();
}
public interface IEndTouch
{
    void OnEndTouch();
}
