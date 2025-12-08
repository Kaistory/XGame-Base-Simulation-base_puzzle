using System;
using UnityEngine;

[Serializable]
public abstract class BaseInteractHandler
{
    protected readonly InteractHandler interactHandler;

    protected BaseInteractHandler(InteractHandler newInteractHandler)
    {
        interactHandler = newInteractHandler;
    }

    public abstract void SetInteractObject(BaseInteractableObject interactableObject);

    public abstract void HandleBeganPhase(Vector3 position);

    public abstract void HandleMovedPhase(Vector3 position);

    public abstract void HandleEndedPhase(Vector3 position);

    public abstract void Refresh();

    protected virtual void Cancel()
    {
        interactHandler.ResetToNoneInteractHandler();
    }

    protected T GetInteractHandler<T>() where T : BaseInteractHandler
    {
        return interactHandler.FindInteractHandler<T>();
    }

    public virtual void Restore()
    {
    }
}