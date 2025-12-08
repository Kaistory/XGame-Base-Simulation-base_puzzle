using UnityEngine;

public abstract class BaseDraggableObject : BaseInteractableObject
{
    [field: SerializeField] public bool IsBeingDragged { get; protected set; }
    protected float smoothSpeed = 20f;
    protected float minDistanceThreshold = 0.01f;
    public SpriteRenderer content;
    protected BoxCollider2D col2D;
    protected Vector2 posDifference;
    public virtual void Awake()
    {
        TypeOfInteraction = InteractionType.Drag;
        IsBeingDragged = false;
        gameObject.tag = "Interactable";
    }

    public virtual void BeginDrag()
    {
        IsBeingDragged = true;
        if (col2D != null)
        {
            col2D.enabled = false;
        }
        content.sortingOrder += 100;
    }

    public virtual void EndDrag()
    {
        IsBeingDragged = false;
        ResetDrag();
    }

    public virtual void ResetDrag()
    {
        IsBeingDragged = false;
        if (col2D != null)
        {
            col2D.enabled = true;
        }
        content.sortingOrder -= 100;
    }

    public void SetPositionSmooth(Vector3 targetPosition)
    {
        targetPosition += new Vector3(posDifference.x, posDifference.y, 0);
        if (Vector3.Distance(transform.position, targetPosition) < minDistanceThreshold)
        {
            SetPosInsideScreen(targetPosition);
        }
        else
        {
            SetPosInsideScreen(CustomLerp(transform.position, targetPosition, smoothSpeed, Time.deltaTime));
        }
    }
    private Vector3 CustomLerp(Vector3 start, Vector3 end, float speed, float deltaTime)
    {
        float t = 1f - Mathf.Exp(-speed * deltaTime);
        return Vector3.Lerp(start, end, t);
    }
    private void SetPosInsideScreen(Vector3 worldPosition)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosition);
        Vector3 posOrigin = transform.position;
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        Vector3 posDifference = transform.position - posOrigin;
        MoveDragLink(new Vector3(posDifference.x, posDifference.y, 0));
    }
    public void SetPosDifference(Vector3 posBeginDown)
    {
        posDifference = transform.position - posBeginDown;
    }
    public void AddSortOrder(int orderBonus)
    {
        content.sortingOrder += orderBonus;
    }

    public virtual void MoveDragLink(Vector3 posDifference)
    {

    }
}