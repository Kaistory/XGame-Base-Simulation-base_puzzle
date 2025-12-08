using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame.sdk;

public class GameAdsHelperBridge : MonoBehaviour
{
    public static GameAdsHelperBridge Instance;

    public static event Action<int, string> CBRequestGDPR = null;
    private static bool isFirst = true;
    private static long tShowAppOpenAd = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.name = "GameAdsHelperBridge";           //Change the GameObject name to IronSourceEvents.
        }
        else
        {
            //if (this != Instance) Destroy(gameObject);
        }
    }

    public void showOpenAd(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            _showOpenAds(description);
        });
    }

    public void onAppOpenAdLoad(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            SdkUtil.logd("ads openad onOpenAdLoad=" + description);
            string[] sarr = description.Split(new char[] { ';' });
            if (sarr != null && sarr.Length >= 1)
            {
                AdsHelper.onAdLoad(SDKManager.Instance.currPlacement, "openad", sarr[0], "admob");
            }
            else
            {
                AdsHelper.onAdLoad(SDKManager.Instance.currPlacement, "openad", "", "admob");
            }
        });
    }
    public void onAppOpenAdLoadResult(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            SdkUtil.logd("ads open onLoadOpenAd=" + description);
            string[] sarr = description.Split(new char[] { ';' });
            if (sarr != null && sarr.Length >= 3)
            {
                string adsource = FIRhelper.getAdsourceAdmob(sarr[2]);
                AdsHelper.onAdLoadResult(SDKManager.Instance.currPlacement, "openad", sarr[1], "admob", adsource, "1".CompareTo(sarr[0]) == 0);
            }
        });
    }
    public void onAppOpenAdPaidEvent(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            string[] sarr = description.Split(new char[] { ';' });
            if (sarr != null && sarr.Length >= 5)
            {
                string adopenid = sarr[0];
                string adnet = sarr[1];
                int precision = 0;
                int.TryParse(sarr[2], out precision);
                long lva = 0;
                long.TryParse(sarr[4], out lva);
                string pl = "openad_default";
                if (SDKManager.Instance != null)
                {
                    pl = SDKManager.Instance.currPlacement;
                }
                string adformat = FIRhelper.getAdformatAdmob(7);
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                //FIRhelper.logEvent("show_ads_total");
                //FIRhelper.logEvent("show_ads_full");
                FIRhelper.logEvent("show_ads_full_open");
                FIRhelper.logEvent("show_ads_full_resume");

                //FIRhelper.logEvent("show_ads_total_imp");
                //FIRhelper.logEvent("show_ads_full_imp");
                FIRhelper.logEvent("show_ads_full_imp_0_oad");
                FIRhelper.logEventAdsPaidAdmob(pl, adformat, adsource, adopenid, lva, lva, sarr[3]);
                AdsHelper.onAdImpresstion(pl, adopenid, adformat, "admob", adsource, (float)((float)lva / 1000000000.0f), lva);
            }
        });
    }
    public void onAppOpenAdImpression(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
        });
    }
    public void onAppOpenAdClicked(string description)
    {
#if UNITY_ANDROID
        AdsProcessCB.Instance().Enqueue(() =>
        {
#endif
            SdkUtil.logd("ads open onClickedOpenNative=" + description);
            string[] sarr = description.Split(new char[] { ';' });
            string adUnitId = "";
            string adsource = "";
            if (sarr != null && sarr.Length >= 2)
            {
                adUnitId = sarr[0];
                adsource = FIRhelper.getAdsourceAdmob(sarr[1]);
            }
            AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "openad", "admob", adsource, adUnitId);
#if UNITY_ANDROID
        });
#endif
    }
    public void onAppOpenAdShowFailed(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            SDKManager.Instance.closeWaitShowFull();
            SDKManager.Instance.flagTimeScale = 0;
            Time.timeScale = 1;
            string[] sarr = description.Split(new char[] { ';' });
            if (sarr != null && sarr.Length >= 3)
            {
                string adsource = FIRhelper.getAdsourceAdmob(sarr[1]);
                AdsHelper.onAdShowEnd(SDKManager.Instance.currPlacement, "openad", "admob", adsource, sarr[0], false, sarr[2]);
            }
        });
    }
    public void onAppOpenAdDismiss(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            SDKManager.Instance.closeWaitShowFull();
            SDKManager.Instance.flagTimeScale = 0;
            Time.timeScale = 1;
            string[] sarr = description.Split(new char[] { ';' });
            if (sarr != null && sarr.Length >= 2)
            {
                string adsource = FIRhelper.getAdsourceAdmob(sarr[1]);
                AdsHelper.onAdShowEnd(SDKManager.Instance.currPlacement, "openad", "admob", adsource, sarr[0], true, "");
            }
        });
    }
    private void _showOpenAds(string description)
    {
        SdkUtil.logd("ads open call _showOpenAds=" + description);
        bool isAdsShowing = false;
        bool isAllowShow = true;
        if (AdsHelper.Instance != null)
        {
            if (AdsHelper.Instance.count4AdShowing > 0)
            {
                isAdsShowing = true;
            }
            long tcurr = SdkUtil.CurrentTimeMilis();
            if ((tcurr - tShowAppOpenAd) < AdsHelper.Instance.currConfig.OpenAdDelTimeOpen)
            {
                SdkUtil.logd("ads open _showOpenAds not met delta time show");
                isAllowShow = false;
            }
            if (isAllowShow)
            {
                isAllowShow = AdsHelper.Instance.isShowOpenAds(false) > 0;
            }
        }
        else
        {
            isAllowShow = false;
        }
        if (GameHelper.Instance != null && isAllowShow)
        {
            isAllowShow = GameHelper.Instance.isAlowShowAppOpenAd;
            if (!GameHelper.Instance.isAlowShowAppOpenAd)
            {
                GameHelper.Instance.isAlowShowAppOpenAd = true;
            }
        }
        SdkUtil.logd($"ads open _showOpenAds isAdsShowing={isAdsShowing} isAllowShow={isAllowShow}");
        string appOpenAdId = "";
#if UNITY_ANDROID
        appOpenAdId = AdsHelper.Instance.OpenAdIdAndroid;
#elif UNITY_IOS || UNITY_IPHONE
        appOpenAdId = AdsHelper.Instance.OpenAdIdiOS;
#endif
        if (isAllowShow && !isAdsShowing)
        {
            bool isshow = false;
            if (AdsHelper.Instance.currConfig.OpenAdShowtype == 0 || AdsHelper.Instance.currConfig.OpenAdShowtype == 2)
            {
                isshow = GameHelper.Instance.showAppOpenAd();
                if (!isshow)
                {
                    if (AdsHelper.Instance.currConfig.OpenAdShowtype == 2)
                    {
                        isshow = AdsHelper.Instance.showFull(AdsBase.PLFullResume, GameRes.LevelCommon(), 0, 0, 0, false, false, false, (satead) =>
                        {
                            if (satead == AD_State.AD_CLOSE || satead == AD_State.AD_SHOW_FAIL || satead == AD_State.AD_CLOSE2 || satead == AD_State.AD_SHOW_FAIL2)
                            {
                                SDKManager.Instance.closeWaitShowFull();
                                SDKManager.Instance.flagTimeScale = 0;
                                Time.timeScale = 1;
                                if (SDKManager.Instance.CBPauseGame != null)
                                {
                                    SDKManager.Instance.CBPauseGame.Invoke(false);
                                }
                                GameHelper.Instance.loadAppOpenAd();
                            }
                        }, false, false);
                    }
                }
                else
                {
                    AdsHelper.onAdShowStart(AdsBase.PLFullResume, "openad", "admob", appOpenAdId);
                }
            }
            else
            {
                isshow = AdsHelper.Instance.showFull(AdsBase.PLFullResume, GameRes.LevelCommon(), 0, 0, 0, false, false, false, (satead) =>
                {
                    if (satead == AD_State.AD_CLOSE || satead == AD_State.AD_SHOW_FAIL || satead == AD_State.AD_CLOSE2 || satead == AD_State.AD_SHOW_FAIL2)
                    {
                        SDKManager.Instance.closeWaitShowFull();
                        SDKManager.Instance.flagTimeScale = 0;
                        Time.timeScale = 1;
                        if (SDKManager.Instance.CBPauseGame != null)
                        {
                            SDKManager.Instance.CBPauseGame.Invoke(false);
                        }
                    }
                }, false, false);
                if (!isshow && AdsHelper.Instance.currConfig.OpenAdShowtype == 3)
                {
                    isshow = GameHelper.Instance.showAppOpenAd();
                    if (isshow)
                    {
                        AdsHelper.onAdShowStart(AdsBase.PLFullResume, "openad", "admob", appOpenAdId);
                    }
                    else
                    {
                        GameHelper.Instance.loadAppOpenAd();
                    }
                }
            }
            if (isshow)
            {
                SDKManager.Instance.flagTimeScale = 1;
                Time.timeScale = 0;
                if (SDKManager.Instance.CBPauseGame != null)
                {
                    SDKManager.Instance.CBPauseGame.Invoke(true);
                }
                SDKManager.Instance.showWait4ShowFull();
                AdsProcessCB.Instance().Enqueue(()=>
                {
                    SDKManager.Instance.closeWaitShowFull();
                }, 5);
            }
            isFirst = false;
        }
    }

    public void requestIDFACallBack(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge requestIDFACallBack=" + description);
        if (description != null && description.Contains("3"))
        {
            PlayerPrefs.SetInt("user_allow_track_ads", 1);
            GameHelper.Instance.getAdsIdentify();
        }
        else if (description != null && !description.Contains("3"))
        {
            PlayerPrefs.SetInt("user_allow_track_ads", -1);
        }
        AdsHelper.Instance.initAds();
        AppsFlyerHelperScript.Instance.checkStartAF();
    }

    public void sendFlagCheckMaxAdsErr(string des)
    {
#if UNITY_ANDROID
        mygame.plugin.Android.GameHelperAndroid.sendFlagCheckMaxAdsErr();
#endif
    }

    public void gameIsBecomeActive(string description)
    {
        if (AdsHelper.Instance != null)
        {
            Debug.Log("mysdk: ads GameAdsHelperBridge gameIsBecomeActive=" + AdsHelper.Instance.statuschekAdsFullCloseErr);
            if (AdsHelper.Instance.statuschekAdsFullCloseErr == 2)
            {
                Invoke("waitCheckAdsCloseErr", 2);
            }
            if (AdsHelper.Instance.statuschekAdsGiftErr == 2)
            {
                Invoke("waitCheckAdsCloseErr", 2);
            }
        }
        else
        {
            Debug.Log("mysdk: ads GameAdsHelperBridge gameIsBecomeActive AdsHelper null");
        }
    }

    private void waitCheckAdsCloseErr()
    {
        Debug.Log("mysdk: ads GameAdsHelperBridge waitCheckAdsCloseErr");
        AdsHelper.Instance.waitStatusAdsCloseErr();
    }

    public void gameIsResignActive(string description)
    {
        Debug.Log("GameAdsHelperBridge gameIsResignActive");
        if (AdsHelper.Instance != null)
        {

        }
    }

    private void onshowCmp()
    {
        PlayerPrefs.SetInt("mem_show_CMP", 1);
    }

    public static void unityOnNotShowCMP()
    {
        Debug.Log("mysdk: GameAdsHelperBridge unityOnNotShowCMP");
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(0, "0");
        }
    }

    public void AndroidCBOnShowCMP(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge AndroidCBOnShowCMP");
        onshowCmp();
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(0, description);
        }
    }

    public void AndroidCBCMP(string description)
    {
        if (description != null && description.Length > 0)
        {
            byte[] bytes = Encoding.Default.GetBytes(description);
            description = Encoding.UTF8.GetString(bytes);
        }
        Debug.Log("mysdk: GameAdsHelperBridge AndroidCBCMP=" + description);
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(1, description);
        }
    }

    public void iOSCBOnShowCMP(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge iOSCBOnShowCMP");
        onshowCmp();
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(0, description);
        }
    }

    public void iOSCBCMP(string description)
    {
        if (description != null && description.Length > 0)
        {
            byte[] bytes = Encoding.Default.GetBytes(description);
            description = Encoding.UTF8.GetString(bytes);
        }
        Debug.Log("mysdk: GameAdsHelperBridge iOSCBCMP=" + description);
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(1, description);
        }
    }

    public void AndroidCBPrCheck(string des)
    {
        if (des != null && des.Length > 0)
        {
            AdsProcessCB.Instance().Enqueue(() =>
            {
                Debug.Log("mysdk: GameAdsHelperBridge AndroidCBPrCheck=" + des);
                PlayerPrefsBase.Instance().setInt("mem_kt_jvpirakt", 1);
                FIRhelper.logEvent($"game_invalid2");
                int rsesss = PlayerPrefsBase.Instance().getInt("mem_procinva_gema", 3);
                if (rsesss != 1 && rsesss != 2 && rsesss != 3 && rsesss != 101 && rsesss != 102 && rsesss != 103 && rsesss != 1985)
                {
                    rsesss = 103;
                }
                if (rsesss == 102 || rsesss == 103)
                {
                    SDKManager.Instance.showNotAllowGame();
                    FIRhelper.logEvent($"game_invalid2_notallow");
                }
            });
        }
    }
}