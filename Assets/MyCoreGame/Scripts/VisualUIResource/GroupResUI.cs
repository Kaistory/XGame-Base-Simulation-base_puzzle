using DG.Tweening;
using GamePlugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GroupResUI : MonoBehaviour
{

    private IList<DataResource> items;
    [SerializeField] private GameObject holder;
    [SerializeField] private RectTransform anchorPreview;
    [SerializeField] private RectTransform arrow;
    [Tooltip("Require GridLayoutGroup")]
    [SerializeField] private ListResUI listResUI;
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
    }

    public void Initialized(IList<DataResource> iListItem)
    {
        items = iListItem;
        gameObject.SetActive(true);
        ClickPreview();
    }

    private void ClickPreview()
    {
        if (rectView == null)
        {
            rectView = UIManager.Instance.Canvas.GetComponent<RectTransform>();
        }

        if (anchorPreview == null) anchorPreview = GetComponent<RectTransform>();
        listResUI.Init(items);
        var rectHolder = holder.GetComponent<RectTransform>();
        var canvas = UIManager.Instance.Canvas;
        var grid = listResUI.GetComponent<GridLayoutGroup>();
        var listResRect = listResUI.GetComponent<RectTransform>();
        var sizeDel = items.Count * grid.cellSize.x + grid.padding.left + grid.padding.right + (items.Count - 1) * grid.spacing.x;
        var scrPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, anchorPreview.position);
        var scrView = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectView.position);
        scrPoint += offsetPreview;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectHolder.parent.GetComponent<RectTransform>(), scrPoint, canvas.worldCamera, out var p0);
        rectHolder.anchoredPosition = p0;
        if (scrPoint.x + sizeDel / 2 - rectView.rect.xMax - scrView.x > 0)
        {
            var p = new Vector2(scrView.x + rectView.rect.xMax - sizeDel / 2, scrPoint.y);
            if (p.x > Screen.width)
            {
                p.x = Screen.width;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectHolder, p, canvas.worldCamera, out var p1);
            listResRect.anchoredPosition = p1;
        }
        else if (-(scrPoint.x - sizeDel / 2 - rectView.rect.xMin) > 0)
        {
            var p = new Vector2(scrView.x + rectView.rect.xMin + sizeDel / 2, scrPoint.y);
            if (p.x < 0)
            {
                p.x = 0;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectHolder, p, canvas.worldCamera, out var p1);
            listResRect.anchoredPosition = p1;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectHolder, scrPoint, canvas.worldCamera, out var p1);
            listResRect.anchoredPosition = p1;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectHolder, scrPoint, canvas.worldCamera, out var p2);
        arrow.anchoredPosition = p2 + offsetArrow;
        if (twScale != null)
        {
            twScale.Kill();
        }
        holder.transform.localScale = Vector3.zero;
        holder.SetActive(true);
        twScale = holder.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetId(this);
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
