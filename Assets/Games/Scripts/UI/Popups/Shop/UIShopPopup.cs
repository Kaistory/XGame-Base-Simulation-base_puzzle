using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIShopPopup : PopupUI
{
    [Header("Shop Panel")] [SerializeField]
    private List<InappPanelBase> inappPanelsItems;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform[] holderPack;
    [SerializeField] private RectTransform contentPanel;

    public List<InappPanelBase> InappPanelsItems => inappPanelsItems;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        Init();
        RefreshLayout();
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
    }

    public void LogIAPShow(LogEvent.IAP_ShowType showType, LogEvent.IAP_ShowPosition showPosition,
        LogEvent.IAP_ShowAction showAction,
        bool isLog = false)
    {
        for (int i = 0; i < inappPanelsItems.Count; i++)
        {
            var item = inappPanelsItems[i];
            if (item != null)
            {
                item.IAPShowAction(showType, showPosition, showAction, isLog);
            }
        }
    }

    public void RefreshLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel);
    }

    private void OnEnable()
    {
        RefreshLayout();
    }

    private void Init()
    {
        for (int i = 0; i < inappPanelsItems.Count; i++)
        {
            var item = inappPanelsItems[i];
            if (item != null)
            {
                item.Initialize();
            }
        }
    }

    public void ScrollTo(float value)
    {
        foreach (var layoutGroup in GetComponentsInChildren<VerticalLayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.content.localPosition = new Vector3(scrollRect.content.localPosition.x, value, 0);
    }

    public void ScrollToTarget()
    {
        ScrollTo(0);
    }

    public void SetUpMainShop()
    {
        ButtonClose.gameObject.SetActive(false);
    }


    public void ScrollToHolder(int index)
    {
        Canvas.ForceUpdateCanvases();
        RectTransform target = holderPack[index].parent as RectTransform;
        Bounds contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.content, target);
        Bounds viewportBounds =
            RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, scrollRect.content);
        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float itemCenterY = contentBounds.center.y;
        float scrollOffset = itemCenterY - viewportHeight / 2f;
        float normalizedPos = 1f - (scrollOffset / (contentHeight - viewportHeight));
        normalizedPos = Mathf.Clamp01(normalizedPos);
        scrollRect.verticalNormalizedPosition = normalizedPos;
    }

#if UNITY_EDITOR
    [ContextMenu("Get All References")]
    public void GetAllReferences()
    {
        inappPanelsItems = new List<InappPanelBase>();
        inappPanelsItems = GetComponentsInChildren<InappPanelBase>().ToList();
        holderPack = new Transform[inappPanelsItems.Count];
        for (int i = 0; i < inappPanelsItems.Count; i++)
        {
            holderPack[i] = inappPanelsItems[i].transform;
        }
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}