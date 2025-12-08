using DG.Tweening;
using GamePlugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GroupResUI3 : MonoBehaviour
{

    private IList<DataResource> items1;
    private IList<DataResource> items2;
    private IList<DataResource> items3;
    [SerializeField] private GameObject holder;
    [SerializeField] private RectTransform holderContent;
    [SerializeField] private RectTransform anchorPreview;
    [Tooltip("Require GridLayoutGroup")]
    [SerializeField] private ListResUI listResUI1;
    [SerializeField] private ListResUI listResUI2;
    [SerializeField] private ListResUI listResUI3;
    [SerializeField] private RectTransform rectView;
    [SerializeField] private Vector2 offsetPreview;
    [SerializeField] private Vector2 offsetArrow;
    private Tween twScale;
    private void Start()
    {
        if (rectView == null)
        {
            rectView = PopupManager.Instance.parent.GetComponent<RectTransform>();
        }
    }
    public void SetOffset(Vector2 offset)
    {
        offsetPreview = offset;
    }
    public void SetAnchorPreview(RectTransform anchorPreview)
    {
        this.anchorPreview = anchorPreview;
        transform.position = anchorPreview.position;
    }

    public void Initialized(IList<DataResource> iListItem1, IList<DataResource> iListItem2, IList<DataResource> iListItem3)
    {
        items1 = iListItem1;
        items2 = iListItem2;
        items3 = iListItem3;
        gameObject.SetActive(true);
        ClickPreview();
    }

    private void ClickPreview()
    {
        if (rectView == null)
        {
            rectView = PopupManager.Instance.parent.GetComponent<RectTransform>();
        }

        if (anchorPreview == null) anchorPreview = GetComponent<RectTransform>();
        listResUI1.Init(items1);
        listResUI2.Init(items2);
        listResUI3.Init(items3);

        if (twScale != null)
        {
            twScale.Kill();
        }
        holder.transform.localScale = Vector3.zero;
        holder.SetActive(true);
        twScale = holder.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetId(this);
        StartCoroutine(IEFixPos());
        IEnumerator IEFixPos()
        {
            yield return new WaitForEndOfFrame();
            var canvas = UIManager.Instance.Canvas;
            Camera cam = canvas.worldCamera;
            Vector2 vector2 = holderContent.anchoredPosition;
            vector2.x = 0;
            holderContent.anchoredPosition = vector2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectView, cam.WorldToScreenPoint(holderContent.transform.position), canvas.worldCamera, out var localPoint);
            float over1 = localPoint.x + holderContent.rect.width / 2 - rectView.rect.width / 2 +10;
            float over2 = localPoint.x - holderContent.rect.width / 2 + rectView.rect.width / 2 -10;

            if (over1 > 0)
            {
                vector2 = holderContent.anchoredPosition;
                vector2.x -= over1;
                holderContent.anchoredPosition = vector2;
            }
            if (over2 < 0)
            {
                vector2 = holderContent.anchoredPosition;
                vector2.x += over2;
                holderContent.anchoredPosition = vector2;
            }
        }
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && holder.activeSelf)
        {
            holder.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        this.DOKill();
    }
}
