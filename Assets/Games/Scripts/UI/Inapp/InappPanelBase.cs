using System.Collections.Generic;
using _JigblockPuzzle;
using DG.Tweening;
using mygame.sdk;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;


public abstract class InappPanelBase : MonoBehaviour
{
    [Header("Inapp Panel Base")] public string skuId;
    [SerializeField] protected Button buyBtn;
    [SerializeField] protected Text priceText;
    [SerializeField] protected Text coinText;

    protected string remove_ads = "remove_ads_pack";
    protected List<DataResource> dataResources = new List<DataResource>();

    protected LogEvent.IAP_ShowPosition showPosition;
    protected LogEvent.IAP_ShowType showType;
    protected LogEvent.IAP_ShowAction showAction;

    protected int goldValueGetCache;

    public bool IsFirstBuyPack
    {
        get => PlayerPrefs.GetInt($"is_first_buy_pack_{skuId}", 0) == 1;
        private set => PlayerPrefs.SetInt($"is_first_buy_pack_{skuId}", value ? 1 : 0);
    }

    public virtual void Initialize()
    {
        if (buyBtn != null)
        {
            buyBtn.onClick.AddListener(OnClickBuy);
        }
    }

    protected virtual void OnEnable()
    {
        SetDisplay();
        TigerForge.EventManager.StartListening(EventName.OnRemoveAdsAction, SetDisplay);
        TigerForge.EventManager.StartListening(EventName.OnPurchaseInapp, SetDisplay);
    }

    protected void OnDisable()
    {
        TigerForge.EventManager.StopListening(EventName.OnRemoveAdsAction, SetDisplay);
        TigerForge.EventManager.StopListening(EventName.OnPurchaseInapp, SetDisplay);
    }

    public virtual void SetDisplay()
    {
        dataResources = new List<DataResource>();
        // if (skuId == remove_ads && priceText)
        // {
        //     priceText.text = InappHelper.Instance.getPrice(skuId);
        // }

        if (priceText)
        {
            priceText.text = InappHelper.Instance.getPrice(skuId);
        }

        goldValueGetCache = InappHelper.Instance.getMoneyRcv(skuId, "gold");

        if (coinText)
        {
            if (goldValueGetCache != 0)
            {
                coinText.text = goldValueGetCache.ToString();
            }
        }

        DataResource dataAdd = new DataResource(RES_type.GOLD, goldValueGetCache);
        dataResources.Add(dataAdd);

        CheckDisablePackage();
    }

    protected virtual void CheckDisablePackage()
    {
        if (skuId == remove_ads && AdsHelper.isRemoveAds(0))
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void OnClickBuy()
    {
#if UNITY_EDITOR && !ENABLE_INAPP
        AddGift();
#endif
        if (skuId == remove_ads)
        {
            InappHelper.Instance.BuyPackage(skuId, "RemoveAds", (status) =>
            {
                if (status != null && status.status == 1)
                {
                    CheckFirstBuyPack();
                    TigerForge.EventManager.EmitEvent(EventName.OnRemoveAdsAction);
                    TigerForge.EventManager.EmitEvent(EventName.OnPurchaseInapp);
                    LogEvent.IAPBuy(showType, showPosition, showAction, skuId);
                }
            });
        }
        else
        {
            InappHelper.Instance.BuyPackage(skuId, "Shop", (status) =>
            {
                if (status != null && status.status == 1)
                {
                    LogEvent.IAPBuy(showType, showPosition, showAction, skuId);
                    CheckFirstBuyPack();
                    AddGift();
                    EventManager.EmitEvent(EventName.OnPurchaseInapp);
                }
            });
        }

        LogEvent.IAPClick(showType, showPosition, showAction, skuId);
    }

    private void CheckFirstBuyPack()
    {
        if (!IsFirstBuyPack)
        {
            IsFirstBuyPack = true;
        }
    }

    public virtual void AddGift()
    {
        //Add resouce
        // GameRes.AddRes(RES_type.GEM, InappHelper.Instance.getMoneyRcv(skuId, "gem"), "gem_inapp");
        // GameRes.AddRes(RES_type.ENERGY, InappHelper.Instance.getMoneyRcv(skuId, "energy"), "energy_inapp");
        // GameRes.AddRes(RES_type.COIN, InappHelper.Instance.getMoneyRcv(skuId, "money"), "coin_inapp");

        DataManager.Instance.OnEarnResources(dataResources.ToArray(), LogEvent.ReasonItem.purchase, "Buy_Inapp_Shop",
            DataManager.Level);

        RewardReceivedHub.AddCacheValue(RES_type.GOLD, goldValueGetCache);
        RewardReceivedHub.Instance.CoinFly(goldValueGetCache);

        //Add booster
        //RemoveAds
        // AdsHelper.setRemoveAds(1);
    }

    public void IAPShowAction(LogEvent.IAP_ShowType showType, LogEvent.IAP_ShowPosition showPosition,
        LogEvent.IAP_ShowAction showAction, bool isLog = false)
    {
        this.showType = showType;
        this.showAction = showAction;
        this.showPosition = showPosition;
        if (isLog)
        {
            LogEvent.IAPShow(showType, showPosition, showAction, skuId);
        }
    }
}