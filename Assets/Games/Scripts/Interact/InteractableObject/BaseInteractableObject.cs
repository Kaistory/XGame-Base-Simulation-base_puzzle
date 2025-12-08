using UnityEngine;

public abstract class BaseInteractableObject : MonoBehaviour
{
    public InteractionType TypeOfInteraction { get; protected set; }

    public bool isComplete { protected set; get; }

    public virtual void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}