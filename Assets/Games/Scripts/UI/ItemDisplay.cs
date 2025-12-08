using System;
using System.Collections;
using System.Collections.Generic;
using Crystal;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IResourceTarget
{
    [SerializeField] private Text text;
    [SerializeField] private RES_type resType;
    [SerializeField] private bool isSetupEnable = true;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private bool isOnMain;
    private int oldNum;
    public static Action<ItemDisplay> OnClick;

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }

        IResourceTarget.OnItemChange += SetText;
        if (isSetupEnable)
        {
            int endValue = GameRes.getRes(resType) - RewardReceivedHub.GetCacheValue(resType);

            SetText(endValue);
        }

        RewardReceivedHub.RegisterTarget(this);
    }

    private void OnButtonClick()
    {
        OnClick?.Invoke(this);
    }

    private void OnDisable()
    {
        IResourceTarget.OnItemChange -= SetText;
        RewardReceivedHub.RemoveTarget(this);
    }

    private void OnDestroy()
    {
        IResourceTarget.OnItemChange -= SetText;
        DOTween.Kill(this);
    }

    Tween tweenSetText;

    private void SetText(RES_type res, float duration = 1)
    {
        if (resType != RES_type.NONE && res != resType) return;
        if (tweenSetText != null)
        {
            tweenSetText.Kill();
        }

        int endValue = GameRes.getRes(resType) - RewardReceivedHub.GetCacheValue(resType);

        int num = oldNum;
        tweenSetText = DOTween.To(() => num, x => num = x, endValue, duration).OnUpdate(() => { SetText(num); });
    }

    public void SetText(int endValue)
    {
        oldNum = endValue;
        if (text == null) return;
        text.text = SdkUtil.convertMoneyToString(endValue);
    }

    public List<RES_type> GetResourceTypes()
    {
        return new List<RES_type>() { resType };
    }

    public Transform GetTransform()
    {
        return icon.transform;
    }

    private Tween tweenScale = null;

    public void UpdateVisual()
    {
        if (tweenScale != null)
        {
            tweenScale.Kill();
        }

        tweenScale = transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.05f).SetId(this).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.03f).SetId(this);
        });
    }
}