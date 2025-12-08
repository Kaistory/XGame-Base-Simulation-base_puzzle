using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    public Vector2 startScale = Vector2.one;
    public Vector2 endScale = new Vector2(0.95f, 0.95f);
    [SerializeField] Transform targetTF;
    [SerializeField] protected UnityEvent eventOnPointDown;
    [SerializeField] protected UnityEvent eventOnPointUp;
    [SerializeField] private bool isActiveSound = true;
    [SerializeField] private bool ignoreVib = false;
 
    private bool isPointerDown = false;
    private bool isPointerInside = false;

    private void Awake()
    {
        if (targetTF == null)
        {
            targetTF = transform;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        targetTF.DOScale(endScale, 0.1f).SetEase(Ease.OutQuad).SetUpdate(true).SetId(this);
        eventOnPointDown?.Invoke();
        isPointerInside = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPointerDown) return;
        isPointerDown = false;
        targetTF.DOScale(startScale, 0.1f).SetEase(Ease.OutQuad).SetUpdate(true).SetId(this);
        eventOnPointUp?.Invoke();
        if (eventData.dragging)
        {
            return;
        }

        if (!isPointerInside)
        {
            return;
        }

        if (isActiveSound)
        {
            AudioManager.Instance.PlaySFX(AudioName.BUTTON_CLICK, 0.3f);
        }

        if (!ignoreVib)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVibrate();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
    }

    private void OnDisable()
    {
        targetTF.localScale = startScale;
        this.DOKill();
        isPointerDown = false;
        isPointerInside = false;
    }
}