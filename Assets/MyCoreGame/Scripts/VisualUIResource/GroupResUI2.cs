using DG.Tweening;
using GamePlugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GroupResUI2 : MonoBehaviour
{

    private IList<DataResource> items;
    [SerializeField] private GameObject holder;
    [SerializeField] private RectTransform anchorPreview;
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
        transform.position = anchorPreview.position;
        GetComponent<RectTransform>().anchoredPosition += offsetPreview;
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
            rectView = PopupManager.Instance.parent.GetComponent<RectTransform>();
        }

        if (anchorPreview == null) anchorPreview = GetComponent<RectTransform>();
        listResUI.Init(items);
        
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
