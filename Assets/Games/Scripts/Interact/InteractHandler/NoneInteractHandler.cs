using System;
using UnityEngine;

public class NoneInteractHandler : BaseInteractHandler
{
    public NoneInteractHandler(InteractHandler newInteractHandler) : base(newInteractHandler)
    {
    }
    public override void SetInteractObject(BaseInteractableObject interactableObject)
    {
    }
    public override void HandleBeganPhase(Vector3 position)
    {
        Vector3 touchPos = interactHandler.GetScreenToWorldPosition(new Vector3(position.x, position.y));
        BaseInteractableObject interactableObject = null;

        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
        if (hit.collider && hit.collider.CompareTag("Interactable"))
        {
            interactableObject = hit.collider.gameObject.GetComponent<BaseInteractableObject>();
        }

        if (!interactableObject) return;

        switch (interactableObject.TypeOfInteraction)
        {
            case InteractionType.None:
                break;
            case InteractionType.Drag:
                {
                    DragInteractHandler dragInteractHandler = GetInteractHandler<DragInteractHandler>();
                    interactHandler.SetInteractionType(InteractionType.Drag);
                    interactHandler.SetCurrentInteractHandler(dragInteractHandler);
                    dragInteractHandler.SetInteractObject(interactableObject);
                    dragInteractHandler.HandleBeganPhase(position);
                    break;
                }
            case InteractionType.Drop:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void HandleMovedPhase(Vector3 position)
    {
    }

    public override void HandleEndedPhase(Vector3 position)
    {
    }

    public override void Refresh()
    {
    }
}