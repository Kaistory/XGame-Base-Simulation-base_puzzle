using UnityEngine;

public abstract class BaseDroppableObject : BaseInteractableObject
{
    public virtual void Awake()
    {
        TypeOfInteraction = InteractionType.Drop;
        gameObject.tag = "Interactable";
    }

    public virtual void DropIn(BaseDraggableObject baseDraggableObject)
    {

    }
}