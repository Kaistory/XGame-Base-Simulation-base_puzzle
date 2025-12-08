using UnityEngine;

public static class RemoteConfigure
{
    #region Ads Remote Config

    public static bool CFEnableBanner
    {
        get => PlayerPrefs.GetInt("cf_enable_banner", 1) == 1;
        set => PlayerPrefs.SetInt("cf_enable_banner", value ? 1 : 0);
    }

    public static int CfTimeWaitBanner
    {
        get => PlayerPrefs.GetInt("cf_time_wait_banner_second", 10); // Second
        set => PlayerPrefs.SetInt("cf_time_wait_banner_second", value);
    }

    public static int CFShowAdsBreak
    {
        get => PlayerPrefs.GetInt("cf_show_ads_break", 180); // Second
        set => PlayerPrefs.SetInt("cf_show_ads_break", value);
    }

    #endregion

    #region Others Remote Config

    public static bool CFShowShopWhenNotEnoughResource
    {
        get => PlayerPrefs.GetInt("cf_show_shop_when_not_enough_resource", 1) == 1;
        set => PlayerPrefs.SetInt("cf_show_shop_when_not_enough_resource", value ? 1 : 0);
    }

    public static int CFCountLevelShowMainMenu
    {
        get => PlayerPrefs.GetInt("cf_count_level_show_main_menu", 5);
        set => PlayerPrefs.SetInt("cf_count_level_show_main_menu", value);
    }
    
    public static string CFPriceBuyBoosterString
    {
        get => PlayerPrefs.GetString("cf_price_buy_booster", "");
        set => PlayerPrefs.SetString("cf_price_buy_booster", value);
    }
    
    #endregion

    #region Helper Remote Config

    public static bool CFEnableCheatGame
    {
        get => PlayerPrefs.GetInt("cf_enable_cheat_game", 1) == 1;
        set => PlayerPrefs.SetInt("cf_enable_cheat_game", value ? 1 : 0);
    }

    #endregion
}