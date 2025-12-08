//#define ENABLE_ADS_IRON
//#define use_old_api
#define use_load_all

using System;
using UnityEngine;
using System.Collections;

#if ENABLE_ADS_IRON && !use_ver_840
#if !use_old_api
using Unity.Services.LevelPlay;
using System.Collections.Generic;
#endif
#endif

namespace mygame.sdk
{
    public partial class  AdsIron : AdsBase
    {
        #if !use_ver_840

#if ENABLE_ADS_IRON
#if !use_old_api
        LevelPlayBannerAd bannerAd;
        LevelPlayInterstitialAd interstitialAd;
        LevelPlayRewardedAd rewardedAd;
#endif
#endif
        private static bool isInitAds = false;
        int posBnCurr = -1;
        private bool isAdsInited = false;
        private static bool isCallInit = false;
        private bool isAllowInit = false;
        private string plWaitFull = "aabc";
        private string plWaitGift = "aabc";
        private string plFullShow = "";
        private string plGiftShow = "";

        public override void InitAds()
        {
#if ENABLE_ADS_IRON
            isEnable = true;
            if (!isInitAds)
            {
                isInitAds = true;
                isAdsInited = false;
                isCallInit = false;
                isAllowInit = true;
                Debug.Log($"mysdk: ads iron init ads");
                IronSource.Agent.setMetaData("do_not_sell", "false");

                int memage = PlayerPrefs.GetInt("mem_age_child", 14);
                if (memage >= 13)
                {
                    IronSource.Agent.setMetaData("is_child_directed", "false");
                }
                else if (memage < 13 && memage > 5)
                {
                    IronSource.Agent.setMetaData("is_child_directed", "true");
                    IronSource.Agent.setMetaData("is_deviceid_optout", "true");
                    IronSource.Agent.setConsent(false);
                }

                IronSource.Agent.shouldTrackNetworkState(true);
                int memss = PlayerPrefs.GetInt("mem_ss_consent_ir", -1);
                if (memss != -1)
                {
                    if (memss == 1)
                    {
                        IronSource.Agent.setConsent(true);
                    }
                    else
                    {
                        IronSource.Agent.setConsent(false);
                    }
                    checkInit();
                }
                else
                {
                    int showss = PlayerPrefs.GetInt("mem_show_CMP", 0);
                    if (showss == 1)
                    {
                        checkInit();
                    }
                    else
                    {
                        StartCoroutine(WaitInit());
                    }
                }

                //banner
                dicPLBanner.Clear();
                AdPlacementBanner plbn = new AdPlacementBanner();
                dicPLBanner.Add(PLBnDefault, plbn);
                plbn.placement = PLBnDefault;
                plbn.adECPM.idxHighPriority = -1;
                plbn.adECPM.listFromDstring(bannerId);
                //full
                dicPLFull.Clear();
                AdPlacementFull plfull = new AdPlacementFull();
                dicPLFull.Add(PLFullDefault, plfull);
                plfull.placement = PLFullDefault;
                plfull.adECPM.idxHighPriority = -1;
                plfull.adECPM.listFromDstring(fullId);
                //gift
                dicPLGift.Clear();
                AdPlacementFull plgift = new AdPlacementFull();
                dicPLGift.Add(PLGiftDefault, plgift);
                plgift.placement = PLGiftDefault;
                plgift.adECPM.idxHighPriority = -1;
                plgift.adECPM.listFromDstring(giftId);
            }
#endif
        }

        IEnumerator WaitInit()
        {
            yield return new WaitForSeconds(30);
            checkInit();
        }

        private void checkInit()
        {
            Debug.Log($"mysdk: ads iron checkInit isCallInit=" + isCallInit);
            if (!isCallInit)
            {
                isCallInit = true;
#if ENABLE_ADS_IRON
                Debug.Log($"mysdk: ads iron init ironsdk");
                addEvent();
#if use_old_api
                IronSource.Agent.init(appId);
#else
                IronSource.Agent.validateIntegration();
                IronSource.Agent.setManualLoadRewardedVideo(true);
                com.unity3d.mediation.LevelPlayAdFormat[] legacyAdFormats = new[] { com.unity3d.mediation.LevelPlayAdFormat.BANNER, com.unity3d.mediation.LevelPlayAdFormat.INTERSTITIAL, com.unity3d.mediation.LevelPlayAdFormat.REWARDED };
                LevelPlay.Init(appId, SystemInfo.deviceUniqueIdentifier, legacyAdFormats);
#endif

#endif
            }
        }

        public override void AdsAwake()
        {
#if ENABLE_ADS_IRON
            GameAdsHelperBridge.CBRequestGDPR += onShowCmp;
            isEnable = true;
#endif
        }

        public void onShowCmp(int state, string des)
        {
#if ENABLE_ADS_IRON
            if (state == 0)
            {
                Debug.Log($"mysdk: ads iron onshow cmp");
                if (des != null && des.CompareTo("0") == 0)
                {
                    if (isAllowInit)
                    {
                        checkInit();
                    }
                    else
                    {
                        StartCoroutine(Wait4AllowInit());
                    }
                }
            }
            else if (state == 1)
            {
                if (des != null && des.Length > 5)
                {
                    PlayerPrefs.SetInt("mem_ss_consent_ir", 1);
                    IronSource.Agent.setConsent(true);
                }
                else
                {
                    PlayerPrefs.SetInt("mem_ss_consent_ir", 0);
                    IronSource.Agent.setConsent(false);
                }
                if (isAllowInit)
                {
                    checkInit();
                }
                else
                {
                    StartCoroutine(Wait4AllowInit());
                }
            }
#endif
        }

        IEnumerator Wait4AllowInit()
        {
            yield return new WaitForSeconds(0.5f);
            checkInit();
        }

        public override string getname()
        {
            return "iron";
        }

        private void Start()
        {

        }

        private void addEvent()
        {
#if ENABLE_ADS_IRON

#if use_old_api
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            IronSourceEvents.onImpressionDataReadyEvent += irOnImpressionDataReadyEvent;

            IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerAdScreenDismissedEvent;

            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialAdClosedEvent;

            IronSourceRewardedVideoEvents.onAdAvailableEvent += ReWardedVideoOnAdAvailableEvent;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
#else
            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += (error => Debug.Log("Initialization error: " + error));

            IronSourceEvents.onImpressionDataReadyEvent += irOnImpressionDataReadyEvent;
#endif

#endif
        }
#if ENABLE_ADS_IRON
#if use_old_api
        private void SdkInitializationCompletedEvent()
        {
            Debug.Log($"mysdk: ads iron SdkInitializationCompletedEvent");
            isAdsInited = true;
            AdsProcessCB.Instance().Enqueue(() =>
            {
                if ("aabc".CompareTo(plWaitFull) != 0)
                {
                    loadFull(plWaitFull, null);
                }
                if ("aabc".CompareTo(plWaitGift) != 0)
                {
                    loadGift(plWaitGift, null);
                }
                advhelper.onAdsInitSuc();
            }, 0.1f);
        }
#else
        private void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
        {
            Debug.Log($"mysdk: ads iron SdkInitializationCompletedEvent");
            isAdsInited = true;
            AdsProcessCB.Instance().Enqueue(() =>
            {
                if ("aabc".CompareTo(plWaitFull) != 0)
                {
                    loadFull(plWaitFull, null);
                }
                if ("aabc".CompareTo(plWaitGift) != 0)
                {
                    loadGift(plWaitGift, null);
                }
                advhelper.onAdsInitSuc();
            }, 0.1f);
        }
#endif
#endif

        void OnApplicationPause(bool isPaused)
        {
#if ENABLE_ADS_IRON
            IronSource.Agent.onApplicationPause(isPaused);
#endif
        }

        protected override void tryLoadBanner(AdPlacementBanner adpl)
        {
#if ENABLE_ADS_IRON
            if (adpl != null)
            {
                if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
                {
                    adpl.adECPM.idxCurrEcpm = 0;
                }
                adpl.isLoading = true;
                adpl.isloaded = false;
                adpl.isRealShow = false;
#if use_old_api
                SdkUtil.logd($"ads bn {adpl.getPlacement} iron bn tryLoadBanner");
                IronSourceBannerPosition adposbn;
                if (adpl.posBanner == 0)
                {
                    adposbn = IronSourceBannerPosition.TOP;
                }
                else if (adpl.posBanner == 1)
                {
                    adposbn = IronSourceBannerPosition.BOTTOM;
                }
                else
                {
                    adposbn = IronSourceBannerPosition.BOTTOM;
                }
                IronSourceBannerSize ironSourceBannerSize = IronSourceBannerSize.BANNER;
                if (bnWidth <= -2)
                {
                    float Height;
                    if (AppConfig.isBannerIpad && SdkUtil.isiPad())
                    {
                        Height = 100;
                    }
                    else
                    {
                        Height = 60;
                    }
                    float Width = IronSource.Agent.getDeviceScreenWidth();
                    ISContainerParams isContainerParams = new ISContainerParams { Width = Width, Height = Height };
                    ironSourceBannerSize.setBannerContainerParams(isContainerParams);
                    ironSourceBannerSize.SetAdaptive(true);
                }
                else if (bnWidth < 10)
                {
                    if (AppConfig.isBannerIpad && SdkUtil.isiPad())
                    {
                        ironSourceBannerSize = new IronSourceBannerSize(728, 90);
                    }
                }
                else
                {
                    float Height;
                    if (AppConfig.isBannerIpad && SdkUtil.isiPad())
                    {
                        Height = 90;
                    }
                    else
                    {
                        Height = 50;
                    }
                    ISContainerParams isContainerParams = new ISContainerParams { Width = bnWidth, Height = Height };
                    ironSourceBannerSize.setBannerContainerParams(isContainerParams);
                }
                AdsHelper.onAdLoad(adpl.getPlacement, "banner", appId, "iron");
                IronSource.Agent.loadBanner(ironSourceBannerSize, adposbn);
#else           
                string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn tryLoadBanner id={idload}");
                com.unity3d.mediation.LevelPlayBannerPosition pos;
                if (adpl.posBanner == 0)
                {
                    pos = com.unity3d.mediation.LevelPlayBannerPosition.TopCenter;
                }
                else
                {
                    pos = com.unity3d.mediation.LevelPlayBannerPosition.BottomCenter;
                }
                com.unity3d.mediation.LevelPlayAdSize bns = com.unity3d.mediation.LevelPlayAdSize.BANNER;
                if (bnWidth <= -2)
                {
                    bns = com.unity3d.mediation.LevelPlayAdSize.CreateAdaptiveAdSize();
                }
                bannerAd = new LevelPlayBannerAd(idload, bns, pos, adpl.loadPl, false, false);
                bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
                bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
                bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
                bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
                bannerAd.OnAdClicked += BannerOnAdClickedEvent;
                bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
                bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
                bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
                bannerAd.LoadAd();
                AdsHelper.onAdLoad(adpl.loadPl, "banner", idload, "iron");
#endif
            }
            else
            {
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn tryLoadBanner not pl");
            }
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn tryLoadBanner not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }

        IEnumerator waitLoadBannerWhenDestroy(string placement, AdCallBack cb)
        {
            AdPlacementBanner adpl = getPlBanner(placement, 0);
            if (adpl != null)
            {
                adpl.cbLoad = cb;
                if (!adpl.isLoading)
                {
                    adpl.isLoading = true;
                    yield return new WaitForSeconds(0.25f);
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadBanner(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl} iron bn waitLoadBannerWhenDestroy isloading");
                }
            }
            else
            {
                yield return null;
            }
        }
        public override void loadBanner(string placement, AdCallBack cb)
        {
            if (!isAdsInited)
            {
                SdkUtil.logd($"ads bn {placement} iron loadBanner not init");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            AdPlacementBanner adpl = getPlBanner(placement, 0);
            if (adpl != null)
            {
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn loadBanner isloading={adpl.isLoading}");
                adpl.cbLoad = cb;
                if (!adpl.isLoading && !adpl.isloaded)
                {
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadBanner(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl} iron bn loadBanner isloading={adpl.isLoading} or isloaded={adpl.isloaded}");
                    if (adpl.isloaded)
                    {
                        if (cb != null)
                        {
                            cb(AD_State.AD_LOAD_OK);
                        }
                    }
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn loadBanner not pl");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
            }
        }
        public override bool showBanner(string placement, int pos, int width, int maxH, AdCallBack cb, float dxCenter, bool highP = false)
        {
            if (!isAdsInited)
            {
                SdkUtil.logd($"ads bn {placement} iron showBanner not init");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
            AdPlacementBanner adpl = getPlBanner(placement, 0);
#if ENABLE_ADS_IRON
            if (adpl != null)
            {
                SdkUtil.logd($"ads bn {adpl.showPl} iron bn showBanner isloading={adpl.isLoading}");
                bool isDestroyBanner = false;
                if (posBnCurr != -1 && posBnCurr != pos)
                {
                    isDestroyBanner = true;
                    destroyBanner();
                }
                adpl.isShow = true;
                adpl.posBanner = pos;
                adpl.setSetPlacementShow(placement);
                bnWidth = width;
#if use_old_api
                if (adpl.isloaded)
#else
                if (adpl.isloaded && bannerAd != null)
#endif

                {
                    if (!adpl.isRealShow)
                    {
                        adpl.isRealShow = true;
#if use_old_api
                        IronSource.Agent.displayBanner();
#else
                        bannerAd.ShowAd();
#endif
                    }
                    advhelper.hideOtherBanner(adsType);
                    return true;
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.showPl} iron bn showBanner not show and load isloading={adpl.isLoading}");
                    posBnCurr = pos;
                    if (isDestroyBanner)
                    {
                        StartCoroutine(waitLoadBannerWhenDestroy(placement, cb));
                    }
                    else
                    {
                        loadBanner(placement, cb);
                    }
                    return false;
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {adpl.showPl} iron bn tryLoadBanner not pl");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
#else
            if (cb != null)
            {
                SdkUtil.logd($"ads bn {placement} iron bn tryLoadBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
#endif
        }
        public override void hideBanner()
        {
#if ENABLE_ADS_IRON
            SdkUtil.logd($"ads bn iron bn hideBanner");
            foreach (var adi in dicPLBanner)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
            }
#if use_old_api
            IronSource.Agent.hideBanner();
#else
            if (bannerAd != null)
            {
                bannerAd.HideAd();
            }
#endif
#endif
        }
        public override void destroyBanner()
        {
#if ENABLE_ADS_IRON
            SdkUtil.logd($"ads bn iron bn destroyBanner");
            foreach (var adi in dicPLBanner)
            {
                adi.Value.isLoading = false;
                adi.Value.isloaded = false;
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
            }
            posBnCurr = -1;
#if use_old_api
            IronSource.Agent.destroyBanner();
#else
            if (bannerAd != null)
            {
                bannerAd.DestroyAd();
                bannerAd = null;
            }
#endif
#endif
        }
        //Native

        //
        public override void clearCurrFull(string placement)
        {
#if ENABLE_ADS_IRON
            if (getFullLoaded(placement) == 1)
            {
                AdPlacementFull adpl = getPlFull(placement, true);
                if (adpl != null)
                {
                    adpl.isloaded = false;
                }
            }
#endif
        }
        public override int getFullLoaded(string placement)
        {
#if ENABLE_ADS_IRON
            AdPlacementFull adpl = getPlFull(placement, true);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {adpl.showPl} iron getFullLoaded not pl");
                return 0;
            }
            else
            {
#if use_load_all
                for (int i = 0; i < adpl.adECPM.list.Count; i++)
                {
                    if (adpl.adECPM.list[i].isLoaded)
                    {
                        fullAdNetwork = adpl.adECPM.list[i].adnetname;
                        return 1;
                    }
                }
#else

#if use_old_api
                if (adpl.isloaded)
#else
                if (adpl.isloaded && interstitialAd != null)
#endif
                {
                    return 1;
                }
#endif
            }
#endif
            return 0;
        }
        protected override void tryLoadFull(AdPlacementFull adpl)
        {
#if ENABLE_ADS_IRON

#if use_load_all
            adpl.isLoading = true;
            adpl.isloaded = false;
            adpl.countLoad = 0;
            for (int i = 0; i < adpl.adECPM.list.Count; i++)
            {
                if (!adpl.adECPM.list[i].isLoading && !adpl.adECPM.list[i].isLoaded)
                {
                    string idload = adpl.adECPM.list[i].adsId;
                    SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} iron tryLoadFull load all id={idload}");
                    adpl.adECPM.list[i].isLoading = true;
                    adpl.countLoad++;
                    interstitialAd = new LevelPlayInterstitialAd(idload);
                    adpl.adECPM.list[i].adObject = interstitialAd;

                    interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
                    interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
                    interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
                    interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
                    interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
                    interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
                    interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;

                    //interstitialAd.OnAdLoaded += (adInfo) => { InterstitialOnAdLoadedEvent(idload, adInfo); };
                    //interstitialAd.OnAdLoadFailed += (error) => { InterstitialOnAdLoadFailedEvent(idload, error); };
                    //interstitialAd.OnAdDisplayed += (adInfo) => { InterstitialOnAdDisplayedEvent(idload, adInfo); };
                    //interstitialAd.OnAdDisplayFailed += (infoError) => { InterstitialOnAdDisplayFailedEvent(idload, infoError); };
                    //interstitialAd.OnAdClicked += (adInfo) => { InterstitialOnAdClickedEvent(idload, adInfo); };
                    //interstitialAd.OnAdClosed += (adInfo) => { InterstitialOnAdClosedEvent(idload, adInfo); };
                    //interstitialAd.OnAdInfoChanged += (adInfo) => { InterstitialOnAdInfoChangedEvent(idload, adInfo); };

                    interstitialAd.LoadAd();
                    AdsHelper.onAdLoad(adpl.loadPl, "interstitial", idload, "iron");
                }
                else
                {
                    if (adpl.adECPM.list[i].isLoading)
                    {
                        adpl.countLoad++;
                    }
                    if (adpl.adECPM.list[i].isLoaded)
                    {
                        adpl.isloaded = true;
                    }
                    SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} iron tryLoadFull id={adpl.adECPM.list[i].adsId} loading={adpl.adECPM.list[i].isLoading} loaded={adpl.adECPM.list[i].isLoaded}");
                }
            }
            if (adpl.countLoad == 0)
            {
                adpl.isLoading = false;
            }
#else
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            adpl.isLoading = true;
            adpl.isloaded = false;
#if use_old_api
            SdkUtil.logd($"ads full {adpl.getPlacement}-{adpl.placement} iron tryLoadFull");
            AdsHelper.onAdLoad(adpl.getPlacement, "interstitial", appId, "iron");
            IronSource.Agent.loadInterstitial();
#else
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            SdkUtil.logd($"ads full {adpl.getPlacement} iron tryLoadFull id={idload}");
            interstitialAd = new LevelPlayInterstitialAd(idload);
            interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
            interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
            interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
            interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
            interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
            interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
            interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
            interstitialAd.LoadAd();
            AdsHelper.onAdLoad(adpl.getPlacement, "interstitial", idload, "iron");
#endif
#endif

#else
            if (adpl != null && adpl.cbLoad != null)
            {
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadFull(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_IRON
            if (!isAdsInited)
            {
                SdkUtil.logd($"ads full {placement} iron loadFull not init");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            AdPlacementFull adpl = getPlFull(placement, false);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} iron loadFull not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
#if use_load_all
                if (true) //(!adpl.isLoading)
#else
                if (!adpl.isloaded && !adpl.isLoading)
#endif
                {
                    SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} iron loadFull type=" + adsType);
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.cbLoad = cb;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadFull(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} iron loadFull isloading={adpl.isLoading} or isloaded={adpl.isloaded}");
                }
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showFull(string placement, float timeDelay, bool isShow2, AdCallBack cb)
        {
            isFull2 = isShow2;
#if ENABLE_ADS_IRON
            if (!isAdsInited)
            {
                SdkUtil.logd($"ads full {placement} iron showFull not init");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
            AdPlacementFull adpl = getPlFull(placement, true);
            if (adpl != null)
            {
                adpl.cbShow = null;
                int ss = getFullLoaded(adpl.placement);
                if (ss > 0)
                {
                    adpl.countLoad = 0;
                    adpl.cbShow = cb;
                    adpl.setSetPlacementShow(placement);
                    string idShow = "";
                    string netShow = "";
#if use_load_all
                    for (int i = 0; i < adpl.adECPM.list.Count; i++)
                    {
                        if (adpl.adECPM.list[i].isLoaded)
                        {
                            idShow = adpl.adECPM.list[i].adsId;
                            netShow = adpl.adECPM.list[i].adnetname;
                            fullAdNetwork = adpl.adECPM.list[i].adnetname;
                            interstitialAd = (LevelPlayInterstitialAd)adpl.adECPM.list[i].adObject;
                            break;
                        }
                    }
#else
                    idShow = fullIdLoaded;
                    netShow = fullAdNetwork;
#endif
                    if (timeDelay > 0)
                    {
                        SdkUtil.logd($"ads full {adpl.showPl}-{adpl.placement} iron showFull show net={netShow} timeDelay={timeDelay}");
                        AdsProcessCB.Instance().Enqueue(() =>
                        {
                            plFullShow = placement;
#if use_old_api
                            AdsHelper.onAdShowStart(placement, "interstitial", "iron", fullIdLoaded);
                            IronSource.Agent.showInterstitial(placement);
#else
                            AdsHelper.onAdShowStart(placement, "interstitial", "iron", interstitialAd.AdUnitId);
                            interstitialAd.ShowAd(placement);
#endif
                        }, timeDelay);
                        return true;
                    }
                    else
                    {
                        SdkUtil.logd($"ads full {adpl.showPl}-{adpl.placement} iron showFull show net={netShow}");
                        plFullShow = placement;
#if use_old_api
                        AdsHelper.onAdShowStart(placement, "interstitial", "iron", fullIdLoaded);
                        IronSource.Agent.showInterstitial(placement);
#else
                        AdsHelper.onAdShowStart(placement, "interstitial", "iron", interstitialAd.AdUnitId);
                        interstitialAd.ShowAd(placement);
#endif
                        return true;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads full {adpl.showPl}-{adpl.placement} iron showFull show not loaded");
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} iron showFull not pl");
            }
#endif
            return false;
        }

        //------------------------------------------------
        public override void clearCurrGift(string placement)
        {
#if ENABLE_ADS_IRON
            if (getGiftLoaded(placement) == 1)
            {
                AdPlacementFull adpl = getPlGift(placement);
                if (adpl != null)
                {
                    adpl.isloaded = false;
                }
            }
#endif
        }
        public override int getGiftLoaded(string placement)
        {
#if ENABLE_ADS_IRON
            AdPlacementFull adpl = getPlGift(placement);
            if (adpl == null)
            {
                SdkUtil.logd($"ads gift {adpl.showPl} iron getGiftLoaded not pl");
                return 0;
            }
            else
            {
#if use_load_all
                for (int i = 0; i < adpl.adECPM.list.Count; i++)
                {
                    if (adpl.adECPM.list[i].isLoaded)
                    {
                        giftAdNetwork = adpl.adECPM.list[i].adnetname;
                        return 1;
                    }
                }
#else

#if use_old_api
                if (adpl.isloaded && IronSource.Agent.isRewardedVideoAvailable())
#else
                if (adpl.isloaded && rewardedAd != null && rewardedAd.IsAdReady())
#endif
                {
                    return 1;
                }
#endif
            }

#endif
            return 0;
        }
        protected override void tryloadGift(AdPlacementFull adpl)
        {
#if ENABLE_ADS_IRON

#if use_load_all
            adpl.isLoading = true;
            adpl.isloaded = false;
            adpl.countLoad = 0;
            for (int i = 0; i < adpl.adECPM.list.Count; i++)
            {
                if (!adpl.adECPM.list[i].isLoading && !adpl.adECPM.list[i].isLoaded)
                {
                    string idload = adpl.adECPM.list[i].adsId;
                    SdkUtil.logd($"ads gift {adpl.loadPl} iron tryloadGift gift={idload}");
                    adpl.adECPM.list[i].isLoading = true;
                    adpl.countLoad++;
                    var rewardedAd = new LevelPlayRewardedAd(idload);
                    adpl.adECPM.list[i].adObject = rewardedAd;

                    rewardedAd.OnAdLoaded += OnAdLoaded;
                    rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
                    rewardedAd.OnAdDisplayed += OnAdDisplayed;
                    rewardedAd.OnAdDisplayFailed += OnAdDisplayFailed;
                    rewardedAd.OnAdRewarded += OnAdRewarded;
                    rewardedAd.OnAdClosed += OnAdClosed;
                    // Optional
                    rewardedAd.OnAdClicked += OnAdClicked;
                    rewardedAd.OnAdInfoChanged += OnAdInfoChanged;

                    //rewardedAd.OnAdLoaded += (adinfo) => { OnAdLoaded(idload, adinfo); };
                    //rewardedAd.OnAdLoadFailed += (adError) => {  OnAdLoadFailed(idload, adError); };
                    //rewardedAd.OnAdDisplayed += (adinfo) => { OnAdDisplayed(idload, adinfo); };
                    //rewardedAd.OnAdDisplayFailed += (adinfoError) => { OnAdDisplayFailed(idload, adinfoError); };
                    //rewardedAd.OnAdRewarded += (adinfo, adReward) => { OnAdRewarded(idload, adinfo, adReward); };
                    //rewardedAd.OnAdClosed += (adinfo) => { OnAdClosed(idload, adinfo); };
                    //// Optional
                    //rewardedAd.OnAdClicked += (adinfo) => { OnAdClicked(idload, adinfo); };
                    //rewardedAd.OnAdInfoChanged += (adinfo) => { OnAdInfoChanged(idload, adinfo); };

                    rewardedAd.LoadAd();
                    AdsHelper.onAdLoad(adpl.loadPl, "rewarded", idload, "iron");
                }
                else
                {
                    if (adpl.adECPM.list[i].isLoading)
                    {
                        adpl.countLoad++;
                    }
                    if (adpl.adECPM.list[i].isLoaded)
                    {
                        adpl.isloaded = true;
                    }
                    SdkUtil.logd($"ads gift {adpl.loadPl}-{adpl.placement} iron tryloadGift id={adpl.adECPM.list[i].adsId} loading={adpl.adECPM.list[i].isLoading} loaded={adpl.adECPM.list[i].isLoaded}");
                }
            }
            if (adpl.countLoad == 0)
            {
                adpl.isLoading = false;
            }
#else
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            adpl.isLoading = true;
            adpl.isloaded = false;
#if use_old_api
            SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron tryloadGift");
            adpl.isLoading = true;
            //IronSource.Agent.loadRewardedVideo();
            StartCoroutine(waitGiftReady(adpl));
#else
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            SdkUtil.logd($"ads gift {adpl.getPlacement} iron tryloadGift gift={idload}");
            rewardedAd = new LevelPlayRewardedAd(idload);
            rewardedAd.OnAdLoaded += OnAdLoaded;
            rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
            rewardedAd.OnAdDisplayed += OnAdDisplayed;
            rewardedAd.OnAdDisplayFailed += OnAdDisplayFailed;
            rewardedAd.OnAdRewarded += OnAdRewarded;
            rewardedAd.OnAdClosed += OnAdClosed;
            // Optional
            rewardedAd.OnAdClicked += OnAdClicked;
            rewardedAd.OnAdInfoChanged += OnAdInfoChanged;
            rewardedAd.LoadAd();
            AdsHelper.onAdLoad(adpl.getPlacement, "rewarded", idload, "iron");
#endif

#endif

#else
            if (adpl != null && adpl.cbLoad != null)
            {
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        IEnumerator waitGiftReady(AdPlacementFull adpl)
        {
#if ENABLE_ADS_IRON
            if (adpl != null)
            {
                int count = 0;
                while (!adpl.isloaded && adpl.isLoading && count < 40)
                {
                    yield return new WaitForSeconds(0.5f);
                    count++;
                }
                adpl.isLoading = false;
                if (adpl.cbLoad != null)
                {
                    if (adpl.isloaded)
                    {
                        SdkUtil.logd($"ads gift {adpl.loadPl}-{adpl.placement} iron waitGiftReady ok");
                        var tmpcb = adpl.cbLoad;
                        adpl.cbLoad = null;
                        tmpcb(AD_State.AD_LOAD_OK);
                    }
                    else
                    {
                        SdkUtil.logd($"ads gift {adpl.loadPl}-{adpl.placement} iron waitGiftReady fail");
                        var tmpcb = adpl.cbLoad;
                        adpl.cbLoad = null;
                        tmpcb(AD_State.AD_LOAD_FAIL);
                    }
                }
                else
                {
                    SdkUtil.logd($"ads gift {adpl.loadPl}-{adpl.placement} iron waitGiftReady cb null");
                }
            }
            else
            {
                yield return null;
            }
#else
            yield return null;
#endif
        }
        public override void loadGift(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_IRON
            if (!isAdsInited)
            {
                SdkUtil.logd($"ads full {placement} iron loadFull not init");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            AdPlacementFull adpl = getPlGift(placement);
            if (adpl == null)
            {
                SdkUtil.logd($"ads gift {adpl.loadPl} iron loadGift not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
#if use_load_all
                if (true)//(!adpl.isLoading)
#else
                if (!adpl.isloaded && !adpl.isLoading)
#endif
                {
                    SdkUtil.logd($"ads gift {adpl.loadPl}-{adpl.placement} iron loadGift type=" + adsType);
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.cbLoad = cb;
                    adpl.setSetPlacementLoad(placement);
                    tryloadGift(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads gift {adpl.loadPl}-{adpl.placement} iron loadGift isloading={adpl.isLoading} or isloaded={adpl.isloaded}");
                }
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showGift(string placement, float timeDelay, AdCallBack cb)
        {
#if ENABLE_ADS_IRON
            AdPlacementFull adpl = getPlGift(placement);
            if (adpl != null)
            {
                if (getGiftLoaded(placement) > 0)
                {
                    adpl.cbShow = cb;
                    adpl.isloaded = false;
                    adpl.setSetPlacementShow(placement);
                    adpl.isAddCondition = true;
                    string idShow = "";
                    string netShow = "";
#if use_load_all
                    for (int i = 0; i < adpl.adECPM.list.Count; i++)
                    {
                        if (adpl.adECPM.list[i].isLoaded)
                        {
                            idShow = adpl.adECPM.list[i].adsId;
                            giftAdNetwork = adpl.adECPM.list[i].adnetname;
                            netShow = adpl.adECPM.list[i].adnetname;
                            rewardedAd = (LevelPlayRewardedAd)adpl.adECPM.list[i].adObject;
                            break;
                        }
                    }
#else
                    idShow = giftIdLoaded;
                    netShow = giftAdNetwork;
#endif
                    if (timeDelay > 0)
                    {
                        SdkUtil.logd($"ads gift {adpl.showPl}-{adpl.placement} iron showGift show net={netShow} timeDelay={timeDelay}");
                        AdsProcessCB.Instance().Enqueue(() =>
                        {
                            plGiftShow = placement;
#if use_old_api
                            AdsHelper.onAdShowStart(placement, "rewarded", "iron", giftIdLoaded);
                            adpl.isloaded = false;
                            IronSource.Agent.showRewardedVideo(placement);
#else
                            AdsHelper.onAdShowStart(placement, "rewarded", "iron", rewardedAd.AdUnitId);
                            rewardedAd.ShowAd(placement);
#endif
                        }, timeDelay);
                        return true;
                    }
                    else
                    {
                        SdkUtil.logd($"ads gift {adpl.showPl}-{adpl.placement} iron showGift show net={netShow}");
                        plGiftShow = placement;
#if use_old_api
                        AdsHelper.onAdShowStart(placement, "rewarded", "iron", giftIdLoaded);
                        adpl.isloaded = false;
                        IronSource.Agent.showRewardedVideo(placement);
#else
                        AdsHelper.onAdShowStart(placement, "rewarded", "iron", rewardedAd.AdUnitId);
                        rewardedAd.ShowAd(placement);
#endif
                        return true;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads gift {placement}-{adpl.placement} iron showGift not load");
                }
            }
            else
            {

                SdkUtil.logd($"ads gift {placement} iron showGift not pl");
            }
#endif
            return false;
        }
        //-------------------------------------------------------------------------------
#if ENABLE_ADS_IRON
        private void irOnImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                string adFormat = "";
                string adPl = "";
#if use_old_api
                adFormat = impressionData.adUnit;
#else
                adFormat = impressionData.adFormat;
#endif
                if (adFormat.Contains("banner"))
                {
                    SdkUtil.logd($"ads bn iron onpaid adNetwork={adFormat}-{impressionData.mediationAdUnitId}-{impressionData.revenue}");
                    FIRhelper.logEvent("show_ads_bn");
                    FIRhelper.logEvent("show_ads_bn_nm_3");
                    if (impressionData.revenue != null && impressionData.revenue.HasValue)
                    {
                        float realValue = (float)impressionData.revenue.Value;
                        string adsource = FIRhelper.getAdsourceIron(impressionData.adNetwork);
                        AdsHelper.onAdImpresstion(SDKManager.Instance.currPlacement, impressionData.mediationAdUnitId, "banner", "iron", adsource, realValue);
                    }
                    adFormat = "banner";
                    adPl = SDKManager.Instance.currPlacement;
                }
                else if (adFormat.Contains("interstitial"))
                {
                    if (!isFull2)
                    {
                        SdkUtil.logd($"ads full iron onpaid not isFull2 adNetwork={adFormat}-{impressionData.mediationAdUnitId}-{impressionData.revenue}");
                        FIRhelper.logEvent("show_ads_total_imp");
                        FIRhelper.logEvent("show_ads_full_imp");
                        FIRhelper.logEvent("show_ads_full_imp_3");
                    }
                    else
                    {
                        SdkUtil.logd($"ads full iron onpaid isFull2 adNetwork={adFormat}-{impressionData.mediationAdUnitId}-{impressionData.revenue}");
                    }
                    if (dicPLFull.ContainsKey(PLFullDefault))
                    {
                        if (impressionData.revenue != null && impressionData.revenue.HasValue)
                        {
                            AdPlacementFull adpl = dicPLFull[PLFullDefault];
                            float realValue = (float)impressionData.revenue.Value;
                            string adsource = FIRhelper.getAdsourceIron(impressionData.adNetwork);
                            AdsHelper.onAdImpresstion(adpl.showPl, impressionData.mediationAdUnitId, "interstitial", "iron", adsource, realValue);
                        }
                    }
                    adFormat = "interstitial";
                    adPl = plFullShow;
                }
                else if (adFormat.Contains("rewarded"))
                {
                    SdkUtil.logd($"ads gift iron onpaid adNetwork={adFormat}-{impressionData.mediationAdUnitId}-{impressionData.revenue}");
                    FIRhelper.logEvent("show_ads_total_imp");
                    FIRhelper.logEvent("show_ads_reward_imp");
                    FIRhelper.logEvent("show_ads_reward_imp_3");
                    if (dicPLGift.ContainsKey(PLGiftDefault))
                    {
                        if (impressionData.revenue != null && impressionData.revenue.HasValue)
                        {
                            AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                            float realValue = (float)impressionData.revenue.Value;
                            string adsource = FIRhelper.getAdsourceIron(impressionData.adNetwork);
                            AdsHelper.onAdImpresstion(adpl.showPl, impressionData.mediationAdUnitId, "rewarded", "iron", adsource, realValue);
                        }
                    }
                    adPl = plGiftShow;
                    adFormat = "rewarded";
                }
                else
                {
                    SdkUtil.logd($"ads {adFormat} iron onpaid adNetwork={adFormat}-{impressionData.mediationAdUnitId}-{impressionData.revenue}");
                }
                adFormat = adFormat.ToLower();
#if use_old_api
                SdkUtil.logd($"ads iron imp adFormat={adFormat} va={impressionData.revenue} pl={impressionData.placement} net={impressionData.adNetwork}");
                FIRhelper.logEventAdsPaidIron(adPl, adFormat, impressionData.adNetwork, impressionData.mediationAdUnitId, (double)impressionData.revenue, impressionData.country);
#else
                SdkUtil.logd($"ads iron imp adFormat={adFormat} va={impressionData.revenue} pl={impressionData.placement} net={impressionData.adNetwork} mid={impressionData.mediationAdUnitId} mnane={impressionData.mediationAdUnitName}");
                FIRhelper.logEventAdsPaidIron(adPl, adFormat, impressionData.adNetwork, impressionData.mediationAdUnitId, (double)impressionData.revenue, impressionData.country);
#endif
            }
        }

#if !use_old_api

        #region BANNER AD EVENTS
        void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
        {
            LevelPlayAdSize size = adInfo.AdSize;
            int width = size.Width;
            int height = size.Height;
            if (dicPLBanner.ContainsKey(PLBnDefault))
            {
                AdPlacementBanner adpl = dicPLBanner[PLBnDefault];
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn BannerOnAdLoadedEvent adNetwork={adInfo.AdNetwork}-{adInfo.AdFormat}-{adInfo.AdUnitId}");
                string adsource = FIRhelper.getAdsourceIron(adInfo.AdNetwork);
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner", adInfo.AdUnitId, "iron", adsource, true);
                adpl.isloaded = true;
                adpl.isLoading = false;
                adpl.countLoad = 0;
                if (adpl.isShow)
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl} iron bn BannerAdLoadedEvent show");
                    if (!adpl.isRealShow && advhelper.isShowBanner)
                    {
                        adpl.isRealShow = true;
                        bannerAd.ShowAd();
                    }
                    if (advhelper.bnCurrShow == adsType)
                    {
                        SdkUtil.logd($"ads bn {adpl.loadPl} iron bn BannerAdLoadedEvent hide other");
                        advhelper.hideOtherBanner(adsType);
                    }
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl} iron bn BannerAdLoadedEvent hide");
                    adpl.isRealShow = false;
                    bannerAd.HideAd();
                }

                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
                if (advhelper != null)
                {
                    advhelper.onBannerLoadOk(adsType);
                }
            }
            else
            {
                SdkUtil.logd($"ads bn iron bn BannerAdLoadedEvent not pl adNetwork={adInfo.AdNetwork}-{adInfo.AdFormat}-{adInfo.AdUnitId}");
            }
        }
        void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
        {
            if (dicPLBanner.ContainsKey(PLBnDefault))
            {
                AdPlacementBanner adpl = dicPLBanner[PLBnDefault];
                SdkUtil.logd($"ads bn {adpl.loadPl} iron bn BannerOnAdLoadFailedEvent AdUnitId={ironSourceError.AdUnitId}-{ironSourceError.ErrorCode}-{ironSourceError.ErrorMessage}");
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner", ironSourceError.AdUnitId, "iron", "", false);
                if (adpl.isLoading)
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl} iron bn BannerAdLoadFailedEvent isloading");
                    adpl.adECPM.idxCurrEcpm++;
                    if (adpl.adECPM.idxCurrEcpm < adpl.adECPM.list.Count)
                    {
                        tryLoadBanner(adpl);
                    }
                    else
                    {
                        adpl.isLoading = false;
                        AdsProcessCB.Instance().Enqueue(() =>
                        {
                            if (adpl.cbLoad != null)
                            {
                                var tmpcb = adpl.cbLoad;
                                adpl.cbLoad = null;
                                tmpcb(AD_State.AD_LOAD_FAIL);
                            }
                            if (advhelper != null)
                            {
                                advhelper.onBannerLoadFail(adsType);
                            }
                        });
                    }
                }
            }
            else
            {
                SdkUtil.logd($"ads bn iron bn BannerAdLoadFailedEvent not pl");
            }
        }
        void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iro bn BannerAdClickedEvent={adInfo.AdUnitId} net={adInfo.AdNetwork}");
            SDKManager.Instance.onClickAd();
            string adsource = FIRhelper.getAdsourceIron(adInfo.AdNetwork);
            AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "banner", "iron", adsource, adInfo.AdUnitId);
        }
        void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iron bn BannerOnAdDisplayedEvent={adInfo.AdUnitId} net={adInfo.AdNetwork}");
            if (advhelper.bnCurrShow == adsType)
            {
                SdkUtil.logd($"ads bn iron bn BannerOnAdDisplayedEvent hide other");
                advhelper.hideOtherBanner(adsType);
            }
        }
        void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
        {
            SdkUtil.logd($"ads bn iron bn BannerOnAdDisplayFailedEvent");
        }
        void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iron bn BannerOnAdCollapsedEvent={adInfo.AdUnitId}");
        }
        void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iron bn BannerOnAdLeftApplicationEvent={adInfo.AdUnitId}");
        }
        void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iron bn BannerOnAdExpandedEvent={adInfo.AdUnitId}");
        }
        #endregion

        #region INTERSTITIAL AD EVENTS
        void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.loadPl} iron HandleInterstitialAdDidLoad adNetwork={adInfo.AdNetwork}-{adInfo.AdFormat}-{AdUnitId}-{adInfo.Revenue}");
                string adsource = FIRhelper.getAdsourceIron(adInfo.AdNetwork);
                AdsHelper.onAdLoadResult(adpl.loadPl, "interstitial", AdUnitId, "iron", adsource, true);
                fullAdNetwork = adInfo.AdNetwork;
#if use_load_all
                adpl.countLoad--;
                adpl.isloaded = true;
                adpl.setStateAd4Id(AdUnitId, false, true, fullAdNetwork, adInfo.Revenue);
                if (adpl.isLoading)
                {
                    adpl.isLoading = false;
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        if (adpl.cbLoad != null)
                        {
                            var tmpcb = adpl.cbLoad;
                            adpl.cbLoad = null;

                            SdkUtil.logd($"ads full {adpl.loadPl} iron HandleInterstitialAdDidLoad=" + AdUnitId + " -> cb ok");
                            tmpcb(AD_State.AD_LOAD_OK);
                        }
                    });
                }
#else
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
#endif
            }
            else
            {
                SdkUtil.logd($"ads full iron HandleInterstitialAdDidLoad not pl");
            }
        }
        void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
        {
            string AdUnitId = error.AdUnitId;
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.loadPl} iron InterstitialAdShowFailedEvent err={AdUnitId}-{error}");
                AdsHelper.onAdLoadResult(adpl.loadPl, "interstitial", AdUnitId, "iron", "", false);
#if use_load_all
                adpl.countLoad--;
                adpl.setStateAd4Id(AdUnitId, false, false, "", null);
                adpl.setObjectAd4Id(AdUnitId, null);
                if (adpl.isLoading)
                {
                    adpl.isLoading = false;
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        if (adpl.cbLoad != null)
                        {
                            var tmpcb = adpl.cbLoad;
                            adpl.cbLoad = null;

                            SdkUtil.logd($"ads full {adpl.loadPl} iron InterstitialAdShowFailedEvent {AdUnitId} -> {adpl.isloaded}");
                            if (adpl.isloaded)
                            {
                                tmpcb(AD_State.AD_LOAD_OK);
                            }
                            else
                            {
                                tmpcb(AD_State.AD_LOAD_FAIL);
                            }
                        }
                    });
                }
#else
                adpl.isLoading = false;
                adpl.isloaded = false;
                adpl.adECPM.idxCurrEcpm++;
                if (adpl.adECPM.idxCurrEcpm < adpl.adECPM.list.Count)
                {
                    tryLoadFull(adpl);
                }
                else
                {
                    if (adpl.cbLoad != null)
                    {
                        var tmpcb = adpl.cbLoad;
                        adpl.cbLoad = null;
                        tmpcb(AD_State.AD_LOAD_FAIL);
                    }
                }
#endif
            }
            else
            {
                SdkUtil.logd($"ads full iron HandleInterstitialAdDidFailWithError not pl");
            }
        }
        void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.showPl} iron InterstitialOnAdDisplayedEvent={AdUnitId}-{adInfo.AdNetwork}");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full iron InterstitialOnAdDisplayedEvent not pl={AdUnitId}-{adInfo.AdNetwork}");
            }
        }
        void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
        {
            string AdUnitId = infoError.LevelPlayError.AdUnitId;
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.showPl} iron InterstitialAdShowFailedEvent err=" + infoError.ToString());
                AdsHelper.onAdShowEnd(adpl.showPl, "interstitial", "iron", infoError.DisplayLevelPlayAdInfo.adNetwork, AdUnitId, false, infoError.ToString());
#if !use_load_all
                adpl.isloaded = false;
                adpl.isLoading = false;
#endif
                adpl.setStateAd4Id(AdUnitId, false, false, "", null);
                advhelper.onCloseFullGift(true);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full iron InterstitialAdShowFailedEvent not pl err=" + infoError.ToString());
            }
            onFullClose(PLFullDefault);
        }
        void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            SdkUtil.logd($"ads full iron InterstitialOnAdClickedEvent={AdUnitId}-{adInfo.AdNetwork}");
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                string adsource = FIRhelper.getAdsourceIron(adInfo.AdNetwork);
                AdsHelper.onAdClick(adpl.showPl, "interstitial", "iron", adsource, AdUnitId);
            }
            else
            {
                AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "interstitial", "iron", adInfo.AdNetwork, AdUnitId);
            }
        }
        void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.showPl} iron InterstitialOnAdClosedEvent={AdUnitId}-{adInfo.AdNetwork}");
                AdsHelper.onAdShowEnd(adpl.showPl, "interstitial", "iron", adInfo.AdNetwork, AdUnitId, true, "");
#if !use_load_all
                adpl.isloaded = false;
                adpl.isLoading = false;
#endif
                adpl.setStateAd4Id(AdUnitId, false, false, "", null);
                advhelper.onCloseFullGift(true);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }
                adpl.countLoad = 0;
            }
            else
            {
                SdkUtil.logd($"ads full iron InterstitialOnAdClosedEvent not pl={AdUnitId}-{adInfo.AdNetwork}");
            }
            onFullClose(PLFullDefault);
        }
        void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            SdkUtil.logd($"ads full iron InterstitialOnAdInfoChangedEvent={AdUnitId}-{adInfo.AdNetwork}");
        }
        #endregion

        #region REWARDED VIDEO AD EVENTS
        void OnAdLoaded(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.loadPl} iron OnAdLoaded adNetwork={adInfo.AdNetwork}-{AdUnitId}-{adInfo.Revenue}");
                string adsource = FIRhelper.getAdsourceIron(adInfo.AdNetwork);
                AdsHelper.onAdLoadResult(adpl.loadPl, "rewarded", AdUnitId, "iron", adsource, true);
#if use_load_all
                adpl.countLoad--;
                adpl.isloaded = true;
                adpl.setStateAd4Id(AdUnitId, false, true, adInfo.AdNetwork, adInfo.Revenue);
                if (adpl.isLoading)
                {
                    adpl.isLoading = false;
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        if (adpl.cbLoad != null)
                        {
                            var tmpcb = adpl.cbLoad;
                            adpl.cbLoad = null;
                            SdkUtil.logd($"ads gift {adpl.loadPl} iron OnAdLoaded={AdUnitId} -> cb ok");
                            tmpcb(AD_State.AD_LOAD_OK);
                        }
                    });
                }
#else
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                giftAdNetwork = adInfo.AdNetwork;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
#endif
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdLoaded not pl adNetwork={adInfo.AdNetwork}-{AdUnitId}");
            }
        }
        void OnAdLoadFailed(LevelPlayAdError adError)
        {
            string AdUnitId = adError.AdUnitId;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.loadPl} iron OnAdLoadFailed err={adError}");
                AdsHelper.onAdLoadResult(adpl.loadPl, "rewarded", AdUnitId, "iron", "", false);
#if use_load_all
                adpl.countLoad--;
                adpl.setStateAd4Id(AdUnitId, false, false, "", null);
                adpl.setObjectAd4Id(AdUnitId, null);
                if (adpl.isLoading)
                {
                    adpl.isLoading = false;
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        if (adpl.cbLoad != null)
                        {
                            var tmpcb = adpl.cbLoad;
                            adpl.cbLoad = null;

                            SdkUtil.logd($"ads gift {adpl.loadPl} iron OnAdLoadFailed {AdUnitId} -> {adpl.isloaded}");
                            if (adpl.isloaded)
                            {
                                tmpcb(AD_State.AD_LOAD_OK);
                            }
                            else
                            {
                                tmpcb(AD_State.AD_LOAD_FAIL);
                            }
                        }
                    });
                }
#else
                adpl.isLoading = false;
                adpl.isloaded = false;
                adpl.adECPM.idxCurrEcpm++;
                if (adpl.adECPM.idxCurrEcpm < adpl.adECPM.list.Count)
                {
                    tryloadGift(adpl);
                }
                else
                {
                    if (adpl.cbLoad != null)
                    {
                        var tmpcb = adpl.cbLoad;
                        adpl.cbLoad = null;
                        tmpcb(AD_State.AD_LOAD_FAIL);
                    }
                }
#endif
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdLoadFailed not pl err={adError}");
            }
        }
        void OnAdDisplayed(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.showPl} iron OnAdDisplayed={adInfo.AdNetwork}-{AdUnitId}");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdDisplayed not pl={adInfo.AdNetwork}-{AdUnitId}");
            }
        }
        void OnAdDisplayFailed(LevelPlayAdDisplayInfoError adInfoError)
        {
            string AdUnitId = adInfoError.LevelPlayError.AdUnitId;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.showPl} iron OnAdDisplayFailed={adInfoError}");
                AdsHelper.onAdShowEnd(adpl.showPl, "rewarded", "iron", adInfoError.DisplayLevelPlayAdInfo.adNetwork, AdUnitId, false, adInfoError.ToString());
                advhelper.onCloseFullGift(false);
                adpl.setStateAd4Id(AdUnitId, false, false, "", null);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    SdkUtil.logd($"ads gift {adpl.showPl} iron _cbAD fail");
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
#if !use_load_all
                adpl.isloaded = false;
#endif
                adpl.isAddCondition = false;
                adpl.cbShow = null;
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdDisplayFailed not pl err={adInfoError}");
            }
            onGiftClose(PLGiftDefault);
        }
        void OnAdClicked(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            SdkUtil.logd($"ads gift iron OnAdClicked={adInfo.AdNetwork}-{AdUnitId}");
            string adsource = FIRhelper.getAdsourceIron(adInfo.AdNetwork);
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                spl = adpl.showPl;
            }
            AdsHelper.onAdClick(spl, "rewarded", "iron", adsource, AdUnitId);
        }
        void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                if (!adpl.isAddCondition)
                {
                    SdkUtil.logd($"ads gift iron RewardedVideoAdRewardedEvent was rcv onclose and will call close {adInfo.AdNetwork}-{AdUnitId}");
                    if (adpl.cbShow != null)
                    {
                        AdCallBack tmpcb = adpl.cbShow;
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                        isRewardCom = false;
                        adpl.cbShow = null;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads gift iron RewardedVideoAdRewardedEvent not rcv onclose {adInfo.AdNetwork}-{AdUnitId}");
                    adpl.isAddCondition = false;
                    isRewardCom = true;
                }
            }
            else
            {
                SdkUtil.logd($"ads gift iron RewardedVideoAdRewardedEvent not pl");
                isRewardCom = true;
            }
        }
        void OnAdClosed(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                AdsHelper.onAdShowEnd(adpl.showPl, "rewarded", "iron", adInfo.AdNetwork, AdUnitId, true, "");
                advhelper.onCloseFullGift(false);
                adpl.setStateAd4Id(AdUnitId, false, false, "", null);
                if (!adpl.isAddCondition)
                {
                    if (adpl.cbShow != null)
                    {
                        AdCallBack tmpcb = adpl.cbShow;
                        if (isRewardCom)
                        {
                            AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                        }
                        else
                        {
                            AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                        }
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                        adpl.cbShow = null;
                    }
                    else
                    {
                        SdkUtil.logd($"ads gift {adpl.showPl} iron HandleRewardBasedVideoClosed isRewardCom={isRewardCom} not cb {adInfo.AdNetwork}-{AdUnitId}");
                    }
                }
                else
                {
                    adpl.isAddCondition = false;
                    SdkUtil.logd($"ads gift {adpl.showPl} iron HandleRewardBasedVideoClosed isRewardCom={isRewardCom} not rcv reward {adInfo.AdNetwork}-{AdUnitId}");
                }
#if !use_load_all
                adpl.isloaded = false;
#endif
            }
            else
            {
                SdkUtil.logd($"ads gift iron HandleRewardBasedVideoClosed not pl");
            }
            onGiftClose(PLGiftDefault);
            isRewardCom = false;
        }
        void OnAdInfoChanged(LevelPlayAdInfo adInfo)
        {
            string AdUnitId = adInfo.AdUnitId;
            SdkUtil.logd($"ads gift iron OnAdInfoChanged={adInfo.AdNetwork}-{AdUnitId}");
        }
        #endregion
        //------
#else
        #region Banner Events
                private void BannerAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLBanner.ContainsKey(PLBnDefault))
            {
                AdPlacementBanner adpl = dicPLBanner[PLBnDefault];
                AdsHelper.onAdLoadResult(adpl.getPlacement, "banner", appId, "iron", adInfo.adNetwork, true);
                adpl.isloaded = true;
                adpl.isLoading = false;
                adpl.countLoad = 0;
                if (adpl.isShow)
                {
                    SdkUtil.logd($"ads bn {adpl.getPlacement}-{adpl.placement} iron bn BannerAdLoadedEvent adNetwork={adInfo.adNetwork} show");
                    if (!adpl.isRealShow && advhelper.isShowBanner)
                    {
                        adpl.isRealShow = true;
                        advhelper.hideOtherBanner(3);
                        IronSource.Agent.displayBanner();
                    }
                    if (advhelper.bnCurrShow == adsType)
                    {
                        SdkUtil.logd($"ads bn {adpl.getPlacement}-{adpl.placement} iron bn BannerAdLoadedEvent hide other");
                        advhelper.hideOtherBanner(adsType);
                    }
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.getPlacement}-{adpl.placement} iron bn BannerAdLoadedEvent hide");
                    adpl.isRealShow = false;
                    IronSource.Agent.hideBanner();
                }

                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
                if (advhelper != null)
                {
                    advhelper.onBannerLoadOk(adsType);
                }
            }
            else
            {
                SdkUtil.logd($"ads bn iron bn BannerAdLoadedEvent not pl adNetwork={adInfo.adNetwork}");
            }
        }

        private void BannerAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            if (dicPLBanner.ContainsKey(PLBnDefault))
            {
                AdPlacementBanner adpl = dicPLBanner[PLBnDefault];
                AdsHelper.onAdLoadResult(adpl.getPlacement, "banner", appId, "iron", "", false);
                if (adpl.isLoading)
                {
                    SdkUtil.logd($"ads bn {adpl.getPlacement}-{adpl.placement} iron bn BannerAdLoadFailedEvent isloading");
                    adpl.isloaded = false;
                    adpl.isLoading = false;
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        if (adpl.cbLoad != null)
                        {
                            var tmpcb = adpl.cbLoad;
                            adpl.cbLoad = null;
                            tmpcb(AD_State.AD_LOAD_FAIL);
                        }
                        if (advhelper != null)
                        {
                            advhelper.onBannerLoadFail(adsType);
                        }
                    });
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.getPlacement}-{adpl.placement} iron bn BannerAdLoadFailedEvent not loading");
                }
            }
            else
            {
                SdkUtil.logd($"ads bn iron bn BannerAdLoadFailedEvent not pl");
            }
        }

        private void BannerAdClickedEvent(IronSourceAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iro bn BannerAdClickedEvent");
            SDKManager.Instance.onClickAd();
            AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "banner", "iron", appId);
        }

        private void BannerAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iron bn BannerAdScreenPresentedEvent net={adInfo.adNetwork}");
            if (advhelper.bnCurrShow == adsType)
            {
                SdkUtil.logd($"ads bn iron bn BannerAdScreenPresentedEvent hide other");
                advhelper.hideOtherBanner(adsType);
            }
        }

        private void BannerAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
            SdkUtil.logd($"ads bn iron bn BannerAdScreenDismissedEvent net={adInfo.adNetwork}");
        }
        #endregion

        #region Interstitial Events
        private void InterstitialAdReadyEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.getPlacement}-{adpl.placement} iron HandleInterstitialAdDidLoad adNetwork={adInfo.adNetwork}");
                AdsHelper.onAdLoadResult(adpl.getPlacement, "interstitial", appId, "iron", adInfo.adNetwork, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                fullAdNetwork = adInfo.adNetwork;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads full iron HandleInterstitialAdDidLoad not pl");
            }
        }
        private void InterstitialAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.getPlacement}-{adpl.placement} iron HandleInterstitialAdDidFailWithError");
                AdsHelper.onAdLoadResult(adpl.getPlacement, "interstitial", appId, "iron", "", false);
                adpl.isLoading = false;
                adpl.isloaded = false;
                if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                {
                    adpl.adECPM.idxCurrEcpm++;
                    tryLoadFull(adpl);
                }
                else
                {
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        dicPLFull[PLFullDefault].countLoad++;
                        tryLoadFull(dicPLFull[PLFullDefault]);
                    }, 1.0f);
                }
            }
            else
            {
                SdkUtil.logd($"ads full iron HandleInterstitialAdDidFailWithError not pl");
            }
        }
        private void InterstitialAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            SdkUtil.logd($"ads full iron InterstitialAdShowSucceededEvent");
        }
        private void InterstitialAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.getPlacement}-{adpl.placement} iron InterstitialAdShowFailedEvent err=" + ironSourceError.ToString());
                adpl.isloaded = false;
                adpl.isLoading = false;
                if (adpl.cbShow != null)
                {
                    advhelper.onCloseFullGift(true);
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full iron InterstitialAdShowFailedEvent not pl err=" + ironSourceError.ToString());
            }
            onFullClose(PLFullDefault);
        }
        private void InterstitialAdClickedEvent(IronSourceAdInfo adInfo)
        {
            SdkUtil.logd($"ads full iron InterstitialAdClickedEvent");
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                AdsHelper.onAdClick(adpl.getPlacement, "interstitial", "iron", appId);
            }
            else
            {
                AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "interstitial", "iron", appId);
            }
        }
        private void InterstitialAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.getPlacement}-{adpl.placement} iron InterstitialAdOpenedEvent");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
        }
        private void InterstitialAdClosedEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                SdkUtil.logd($"ads full {adpl.getPlacement}-{adpl.placement} iron InterstitialAdClosedEvent");
                adpl.isloaded = false;
                adpl.isLoading = false;
                if (adpl.cbShow != null)
                {
                    advhelper.onCloseFullGift(true);
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }
                adpl.cbShow = null;
                adpl.countLoad = 0;
            }
            else
            {
                SdkUtil.logd($"ads full iron HandleInterstitialClosed not pl");
            }
            onFullClose(PLFullDefault);
        }
        #endregion

        #region REWARDED VIDEO AD EVENTS
        private void ReWardedVideoOnAdAvailableEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron OnAdLoaded adNetwork={adInfo.adNetwork}");
                AdsHelper.onAdLoadResult(adpl.getPlacement, "rewarded", appId, "iron", adInfo.adNetwork, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                giftAdNetwork = adInfo.adNetwork;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdLoaded not pl adNetwork={adInfo.adNetwork}");
            }
        }
        private void RewardedVideoOnAdUnavailable()
        {
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                AdsHelper.onAdLoadResult(adpl.getPlacement, "rewarded", appId, "iron", "", false);
                if (adpl.isLoading)
                {
                    SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron OnAdLoadFailed loading");
                    adpl.isLoading = false;
                    adpl.isloaded = false;
                }
                else
                {
                    SdkUtil.logd($"ads gift {adpl.getPlacement} iron OnAdLoadFailed not loading");
                }
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdLoadFailed not pl");
            }
        }
        private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            SdkUtil.logd($"ads gift iron RewardedVideoOnAdClickedEvent");
            if (dicPLFull.ContainsKey(PLFullDefault))
            {
                AdPlacementFull adpl = dicPLFull[PLFullDefault];
                AdsHelper.onAdClick(adpl.getPlacement, "rewarded", "iron", appId);
            }
            else
            {
                AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "rewarded", "iron", appId);
            }
        }
        private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                if (!adpl.isAddCondition)
                {
                    SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron RewardedVideoAdRewardedEvent was rcv onclose and will call close");
                    if (adpl.cbShow != null)
                    {
                        AdCallBack tmpcb = adpl.cbShow;
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                        isRewardCom = false;
                        adpl.cbShow = null;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron RewardedVideoAdRewardedEvent not rcv onclose");
                    adpl.isAddCondition = false;
                    isRewardCom = true;
                }
            }
            else
            {
                SdkUtil.logd($"ads gift iron RewardedVideoAdRewardedEvent not pl");
                isRewardCom = true;
            }
        }
        private void RewardedVideoAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
        {
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron OnAdDisplayFailed");
                advhelper.onCloseFullGift(false);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    SdkUtil.logd($"ads gift {adpl.getPlacement} iron _cbAD fail");
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
                adpl.isAddCondition = false;
                adpl.countLoad = 0;
                adpl.cbShow = null;
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdDisplayFailed not pl");
            }
            onGiftClose(PLGiftDefault);
        }
        private void RewardedVideoAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron OnAdDisplayed={adInfo.adNetwork}");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads gift iron OnAdDisplayed not pl={adInfo.adNetwork}");
            }
        }
        private void RewardedVideoAdClosedEvent(IronSourceAdInfo adInfo)
        {
            if (dicPLGift.ContainsKey(PLGiftDefault))
            {
                AdPlacementFull adpl = dicPLGift[PLGiftDefault];
                advhelper.onCloseFullGift(false);
                if (adpl.cbShow != null)
                {
                    if (!adpl.isAddCondition)
                    {
                        AdCallBack tmpcb = adpl.cbShow;
                        if (isRewardCom)
                        {
                            AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                        }
                        else
                        {
                            AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                        }
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                        adpl.cbShow = null;
                    }
                    else
                    {
                        adpl.isAddCondition = false;
                        SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron HandleRewardBasedVideoClosed isRewardCom={isRewardCom} not rcv reward");
                    }
                }
                else
                {
                    SdkUtil.logd($"ads gift {adpl.getPlacement}-{adpl.placement} iron HandleRewardBasedVideoClosed isRewardCom={isRewardCom} not cb");
                }
                adpl.countLoad = 0;
            }
            else
            {
                SdkUtil.logd($"ads gift iron HandleRewardBasedVideoClosed not pl");
            }
            onGiftClose(PLGiftDefault);
            isRewardCom = false;
        }
        #endregion
#endif

#endif

#endif //!use_ver_840
    }
}