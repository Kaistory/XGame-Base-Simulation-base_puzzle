using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractHandler : master.Singleton<InteractHandler>
{
    [SerializeField] protected InteractionType interactionType;
    [SerializeField] protected bool isActivated;
    public static event Action OnMouseDown;
    public static event Action OnMouseUp;
    protected Camera mainCamera;
    protected Dictionary<Type, BaseInteractHandler> interactHandlers;
    protected BaseInteractHandler currentInteractHandler;
    protected bool isMouseDown;
    protected bool isHolding, isReleasing;
    private bool checkUI;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = GameManager.Instance.camCtrl.MainCam;
        interactHandlers = new Dictionary<Type, BaseInteractHandler>();

        NoneInteractHandler noneInteractHandler = new NoneInteractHandler(this);
        DragInteractHandler dragInteractHandler = new DragInteractHandler(this);
        DropInteractHandler dropInteractHandler = new DropInteractHandler(this);

        interactHandlers.Add(typeof(NoneInteractHandler), noneInteractHandler);
        interactHandlers.Add(typeof(DragInteractHandler), dragInteractHandler);
        interactHandlers.Add(typeof(DropInteractHandler), dropInteractHandler);

        ResetToNoneInteractHandler();
    }
    private bool isDown;
    protected virtual void Update()
    {
        if (isActivated == false) return;
        bool isPointerOverUI = checkUI ? IsPointerOverUIObject() : false;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !isPointerOverUI)
        {
            isDown = true;
            OnPointerDown(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && !isPointerOverUI)
        {
            if (currentInteractHandler != null)
            {
                currentInteractHandler.HandleMovedPhase(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0) && isDown)
        {
            isDown = false;
            OnPointerUp(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !isPointerOverUI)
            {
                isDown = true;
                OnPointerDown(touch.position);
            }
            if (isDown)
            {
                currentInteractHandler?.HandleMovedPhase(touch.position);
            }
            if (touch.phase == TouchPhase.Ended && isDown)
            {
                isDown = false;
                OnPointerUp(touch.position);
            }
        }
#endif
    }

    protected virtual void Refresh()
    {
        interactHandlers[typeof(NoneInteractHandler)].Refresh();
        interactHandlers[typeof(DragInteractHandler)].Refresh();
        interactHandlers[typeof(DropInteractHandler)].Refresh();
    }
    public bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    public virtual void Activate(bool checkUI = true)
    {
        isActivated = true;
        this.checkUI = checkUI;
        Refresh();
        ResetToNoneInteractHandler();
    }

    public virtual void Deactivate()
    {
        currentInteractHandler.Restore();
        Refresh();
        isActivated = false;
    }

    public virtual void RestoreCurrentInteract()
    {
        currentInteractHandler?.Restore();
    }

    public Vector3 GetScreenToWorldPosition(Vector3 position)
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, 0f));
    }

    public bool GetActive()
    {
        return isActivated;
    }

    #region InteractHandlers

    public virtual void ResetToNoneInteractHandler()
    {
        currentInteractHandler = interactHandlers[typeof(NoneInteractHandler)];
        SetInteractionType(InteractionType.None);
    }

    public T FindInteractHandler<T>() where T : BaseInteractHandler
    {
        return (T)interactHandlers[typeof(T)];
    }

    public void SetCurrentInteractHandler(BaseInteractHandler baseInteractHandler)
    {
        currentInteractHandler = baseInteractHandler;
    }

    public void SetInteractionType(InteractionType newInteractionType)
    {
        interactionType = newInteractionType;
    }

    public virtual void OnForceRestore()
    {
    }

    public void OnPointerDown(Vector3 position)
    {
        if (!isActivated)
        {
            return;
        }
        isMouseDown = true;
        if (currentInteractHandler != null)
        {
            currentInteractHandler.HandleBeganPhase(position);
        }
        OnMouseDown?.Invoke();
    }

    public void OnPointerUp(Vector3 position)
    {
        if (!isActivated)
        {
            return;
        }
        isMouseDown = false;
        if (currentInteractHandler != null)
        {
            currentInteractHandler.HandleEndedPhase(position);
        }
        OnMouseUp?.Invoke();
    }
    public void BeginDrag(BaseInteractableObject baseInteractableObject = null)
    {
        isDown = true;
        DragInteractHandler dragInteractHandler = FindInteractHandler<DragInteractHandler>();
        SetInteractionType(InteractionType.Drag);
        SetCurrentInteractHandler(dragInteractHandler);
        dragInteractHandler.SetInteractObject(baseInteractableObject);
        dragInteractHandler.HandleBeganPhase(Input.mousePosition);
    }
    public void EnDrag(BaseInteractableObject baseInteractableObject = null)
    {
        isDown = false;
        DragInteractHandler dragInteractHandler = FindInteractHandler<DragInteractHandler>();
        SetCurrentInteractHandler(dragInteractHandler);
        SetInteractionType(InteractionType.Drag);
        dragInteractHandler.SetInteractObject(baseInteractableObject);
        dragInteractHandler.HandleEndedPhase(Input.mousePosition);
    }
    public InteractionType GetInteractionType()
    {
        return interactionType;
    }
    public void ForceSetInteract(BaseInteractableObject item)
    {
        currentInteractHandler.SetInteractObject(item);
    }
    #endregion
}