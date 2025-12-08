using UnityEngine;

public class DropInteractHandler : BaseInteractHandler
{
    public DropInteractHandler(InteractHandler newInteractHandler) : base(newInteractHandler)
    {
    }

    public override void SetInteractObject(BaseInteractableObject interactableObject)
    {
    }

    public override void HandleBeganPhase(Vector3 position)
    {
        Cancel();
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

    protected override void Cancel()
    {
        base.Cancel();
    }
}