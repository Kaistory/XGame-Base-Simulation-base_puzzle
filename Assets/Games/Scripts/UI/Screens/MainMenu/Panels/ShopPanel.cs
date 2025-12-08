using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ShopPanel : MainMenuPanel
{
    private UIShopPopup uiShopPopup;

    public UIShopPopup UIShopPopup => uiShopPopup;

    public override void Active()
    {
        base.Active();
        uiShopPopup = UIManager.Instance.ShowPopup<UIShopPopup>(null);
        uiShopPopup.transform.SetParent(transform, false);
        uiShopPopup.SetUpMainShop();
        uiShopPopup.LogIAPShow(LogEvent.IAP_ShowType.shop, LogEvent.IAP_ShowPosition.shop_popup,
            LogEvent.IAP_ShowAction.click_button, true);
        DOVirtual.DelayedCall(0.1f, () => { uiShopPopup.RefreshLayout(); });
    }

    public override void Deactive()
    {
        if (uiShopPopup == null) return;
        uiShopPopup.Hide();
        base.Deactive();
    }
}