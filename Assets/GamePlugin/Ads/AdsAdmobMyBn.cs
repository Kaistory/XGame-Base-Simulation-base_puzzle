using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mygame.sdk
{
    partial class AdsAdmobMy
    {
        private void initBannerNm()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log($"mysdk: ads bn admobmy adCfPlacementBanner=" + advhelper.currConfig.adCfPlacementBanner);
                if (advhelper.currConfig.adCfPlacementBanner.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementBanner.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementBanner>(dicPLBanner, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlBanner.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementBanner>(dicPLBanner, plitem, false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"mysdk: ads bn admobmy initBanner ex=" + ex.ToString());
            }
        }
        private void initBnNt()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log($"mysdk: ads bnnt admobmy adCfPlacementBnNt=" + advhelper.currConfig.adCfPlacementBnNt);
                if (advhelper.currConfig.adCfPlacementBnNt.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementBnNt.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementNative>(dicPLBnNt, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlBnNt.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementNative>(dicPLBnNt, plitem, false);
                }
                int flagShowmedia = PlayerPrefs.GetInt("cf_bnnt_showmedia", 0);
                setTypeBnnt(flagShowmedia);
            }
            catch (Exception ex)
            {
                Debug.Log($"mysdk: ads bnnt admobmy initBnNt ex=" + ex.ToString());
            }
        }
        private void initNativeCl()
        {
            if (adsType == 0)
            {
                try
                {
                    Debug.Log("mysdk: ads ntcl admobmy init stepFloorECPMNativeCl=" + advhelper.currConfig.adCfPlacementNattiveCl);
                    if (advhelper.currConfig.adCfPlacementNative.Length > 0)
                    {
                        string[] listpl = advhelper.currConfig.adCfPlacementNattiveCl.Split(new char[] { '#' });
                        foreach (string plitem in listpl)
                        {
                            addAdPlacement<AdPlacementNative>(dicPLNativeCl, plitem, true);
                        }
                    }
                    string[] listpldf = AdIdsConfig.AdmobPlBnNativeCl.Split(new char[] { '#' });
                    foreach (string plitem in listpldf)
                    {
                        addAdPlacement<AdPlacementNative>(dicPLNativeCl, plitem, false);
                    }
                    Debug.Log("mysdk: ads ntcl admobmy init dicPLNativeCl=" + dicPLNativeCl.Count);
                }
                catch (Exception ex)
                {
                    Debug.Log($"mysdk: ads ntcl admobmy initNativeCl ex=" + ex.ToString());
                }
                string memcfntcl = PlayerPrefs.GetString("mem_cf_ntcl_flic", "20,85,2,50");
                setCfNtCl(memcfntcl);
            }
        }
        private void initBannerCl()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log($"mysdk: ads bncl admobmy adCfPlacementCollapse=" + advhelper.currConfig.adCfPlacementCollapse);
                if (advhelper.currConfig.adCfPlacementCollapse.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementCollapse.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementBanner>(dicPLCl, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlCl.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementBanner>(dicPLCl, plitem, false);
                }
                string memcfntclfbex = PlayerPrefs.GetString("cf_ntcl_fb_excluse", "6;5;0;2");
                setCfNtClFbExcluse(memcfntclfbex);
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ads bncl admobmy initBanner collapse ex=" + ex.ToString());
            }
        }
        private void initBannerRect()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log($"mysdk: ads rect admobmy adCfPlacementRect=" + advhelper.currConfig.adCfPlacementRect);
                if (advhelper.currConfig.adCfPlacementRect.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementRect.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementBanner>(dicPLRect, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlRect.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementBanner>(dicPLRect, plitem, false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"mysdk: ads rect admobmy initBanner Rect ex=" + ex.ToString());
            }
        }

        // bn nm
        protected override void tryLoadBanner(AdPlacementBanner adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            adpl.isAdHigh = false;
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }
            SdkUtil.logd($"ads bn {adpl.loadPl}-{adpl.placement} admobmy tryLoadBanner = " + idload + ", idxCurrEcpm=" + adpl.adECPM.idxCurrEcpm);
            AdsHelper.onAdLoad(adpl.loadPl, "banner", idload, "admob");
            adpl.isLoading = true;
            adpl.isloaded = false;
            AdsAdmobMyBridge.Instance.loadBanner(adpl.placement, idload);
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads bn {adpl.loadPl} admobmy tryLoadBanner not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadBanner(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementBanner adpl = getPlBanner(placement, 0);
            if (adpl != null)
            {
                adpl.cbLoad = cb;
                if (!adpl.isLoading && !adpl.isloaded)
                {
                    SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy loadBanner");
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadBanner(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy loadBanner isLoading={adpl.isLoading} isloaded={adpl.isloaded}");
                }
            }
            else
            {

                SdkUtil.logd($"ads bn {placement} admobmy loadBanner not pl");
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showBanner(string placement, int pos, int width, int maxH, AdCallBack cb, float dxCenter, bool highP = false)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementBanner adpl = getPlBanner(placement, 0);
            if (adpl == null)
            {
                if (cb != null)
                {
                    SdkUtil.logd($"ads bn {placement} admobmy showBanner not pl");
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
            SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy showBanner pos=" + pos + ", dxCenter=" + dxCenter + ", idxEcpm=" + adpl.adECPM.idxCurrEcpm + ", countecpm=" + adpl.adECPM.list.Count + ", highP=" + highP);
            isShowHighPriorityBanner = highP;
            adpl.isShow = true;
            adpl.posBanner = pos;
            adpl.setSetPlacementShow(placement);
            bnWidth = width;
            bnDxCenter = dxCenter;
            if (!adpl.isLoading)
            {
                bnnmisnew = false;
                int idxshow = -10;
                long tcurr = SdkUtil.CurrentTimeMilis() / 1000;
                adpl.adECPM.idxCurrEcpm = 0;
                for (int j = 0; j < adpl.adECPM.list.Count; j++)
                {
                    AdECPMItem bnec = adpl.adECPM.list[j];
                    if (bnec.isLoaded)
                    {
                        string idload = bnec.adsId;
                        bool ishasnext = false;
                        if ((tcurr - bnec.timeShow) >= advhelper.currConfig.timeReloadBanner)
                        {
                            ishasnext = true;
                        }
                        SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy showBanner show pre loaded adsid=" + bnec.adsId + ", idx=" + j + ", dxCenter=" + dxCenter);
                        AdsAdmobMyBridge.Instance.showBanner(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                        advhelper.hideOtherBanner(adsType);
                        if (ishasnext)
                        {
                            StartCoroutine(waitLoadNextBanner(adpl));
                        }
                        idxshow = j;
                        break;
                    }
                }

                if (idxshow != -10)
                {
                    if (cb != null)
                    {
                        cb(AD_State.AD_SHOW);
                    }
                    return true;
                }
                else
                {
                    AdsAdmobMyBridge.Instance.showBanner(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                    loadBanner(placement, cb);
                    return false;
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy showBanner isprocess show dxCenter=" + dxCenter);
                adpl.cbLoad = cb;
                bool _iss = false;
                for (int j = 0; j < adpl.adECPM.list.Count; j++)
                {
                    AdECPMItem bnec = adpl.adECPM.list[j];
                    if (bnec.isLoaded)
                    {
                        _iss = true;
                        string idload = bnec.adsId;
                        AdsAdmobMyBridge.Instance.showBanner(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                        advhelper.hideOtherBanner(adsType);
                        break;
                    }
                }
                if (!_iss)
                {
                    AdsAdmobMyBridge.Instance.showBanner(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                }
                return false;
            }
#else
            if (cb != null)
            {
                SdkUtil.logd($"ads bn {placement} admobmy showBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
#endif
        }
        IEnumerator waitLoadNextBanner(AdPlacementBanner adpl)
        {
            adpl.isLoading = true;
            tShowBannerNm = -1;
            yield return new WaitForSeconds(0.1f);
            adpl.countLoad = 0;
            adpl.adECPM.idxCurrEcpm = 0;
            tryLoadBanner(adpl);
        }
        public override void hideBanner()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads bn admobmy hideBanner");
            foreach (var adi in dicPLBanner)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
            }
            AdsAdmobMyBridge.Instance.hideBanner();
#endif
        }
        public override void destroyBanner()
        {
            SdkUtil.logd($"ads bn admobmy destroyBanner");
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            foreach (var adi in dicPLBanner)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
                adi.Value.isloaded = false;
            }
            AdsAdmobMyBridge.Instance.hideBanner();
#endif
        }
        //bn cl
        protected override void tryLoadCollapseBanner(AdPlacementBanner adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads bncl {adpl.loadPl}-{adpl.placement} admobmy tryLoadCollapseBanner");
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow = 0;
            adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].time4Count = 0;
            adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].countTimeShow = 0;
            adpl.isLoading = true;
            adpl.isloaded = false;
            AdsHelper.onAdLoad(adpl.loadPl, "banner_collapse", idload, "admob");
            AdsAdmobMyBridge.Instance.loadBannerCl(adpl.placement, idload);
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads bncl {adsType} admobmy tryLoadCollapseBanner not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadCollapseBanner(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementBanner adpl = getPlBanner(placement, 1);
            if (adpl != null)
            {
                adpl.cbLoad = cb;
                if (!adpl.isLoading)
                {
                    SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy loadCollapseBanner");
                    adpl.countLoad = 0;
                    tShowBannerCl = -1;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadCollapseBanner(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy loadCollapseBanner isProcessShow");
                }
            }
            else
            {
                SdkUtil.logd($"ads bncl {placement} admobmy loadCollapseBanner not pl");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
            }
#else
            if (cb != null)
            {
                SdkUtil.logd($"ads bncl {placement} admobmy loadCollapseBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showCollapseBanner(string placement, int pos, int width, int maxH, float dxCenter, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementBanner adpl = getPlBanner(placement, 1);
            if (adpl != null)
            {
                adpl.isShow = true;
                adpl.posBanner = pos;
                adpl.setSetPlacementShow(placement);
                bnClWidth = width;
                bnClDxCenter = dxCenter;
                flagChangecl2Nm = -1;
                var cf = advhelper.getCfAdsPlacement(placement, -1);
                if (cf != null)
                {
                    flagChangecl2Nm = cf.flagShow;
                }
                SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy showCollapseBanner pos={pos} flagChangecl2Nm={flagChangecl2Nm}");
                if (!adpl.isLoading)
                {
                    bnclisnew = false;
                    int idxsh = -10;
                    long tcurr = SdkUtil.CurrentTimeMilis() / 1000;
                    adpl.adECPM.idxCurrEcpm = 0;
                    for (int j = 0; j < adpl.adECPM.list.Count; j++)
                    {
                        AdECPMItem bnec = adpl.adECPM.list[j];
                        if (bnec.isLoaded)
                        {
                            string idload = bnec.adsId;
                            bool ishasnext = false;
                            if ((tcurr - bnec.timeShow) >= advhelper.currConfig.timeReloadBanner)
                            {
                                ishasnext = true;
                            }
                            SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy showCollapseBanner show pre loaded adsid=" + bnec.adsId + ", idx=" + j + ", dxCenter=" + dxCenter);
                            isShowingCollapse = true;
                            AdsAdmobMyBridge.Instance.showBannerCl(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                            if (ishasnext)
                            {
                                StartCoroutine(waitLoadNextBannerCl(adpl));
                            }
                            idxsh = j;
                            break;
                        }
                    }

                    if (idxsh != -10)
                    {
                        if (cb != null)
                        {
                            cb(AD_State.AD_SHOW);
                        }
                        if (tChangeCl2Nm > 0)
                        {
                            tChangeCl2Nm = 0;
                        }
                        return true;
                    }
                    else
                    {
                        SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy show with empty id 1");
                        AdsAdmobMyBridge.Instance.showBannerCl(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                        loadCollapseBanner(placement, cb);
                        return false;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy isprocess show dxCenter=" + dxCenter);
                    adpl.cbLoad = cb;
                    bool _iss = false;
                    for (int ii = 0; ii < adpl.adECPM.list.Count; ii++)
                    {
                        AdECPMItem bnec = adpl.adECPM.list[ii];
                        if (bnec.isLoaded)
                        {
                            _iss = true;
                            string idload = bnec.adsId;
                            isShowingCollapse = true;
                            if (tChangeCl2Nm > 0)
                            {
                                tChangeCl2Nm = 0;
                            }
                            AdsAdmobMyBridge.Instance.showBannerCl(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                            break;
                        }
                    }
                    if (!_iss)
                    {
                        SdkUtil.logd($"ads bncl {placement}-{adpl.placement} admobmy show with empty id 2");
                        AdsAdmobMyBridge.Instance.showBannerCl(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter);
                    }
                    return false;
                }
            }
            else
            {
                SdkUtil.logd($"ads bncl {placement} admobmy showCollapseBanner not pl");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
#else
            if (cb != null)
            {
                SdkUtil.logd($"ads bncl {placement} admobmy showCollapseBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
#endif
        }
        IEnumerator waitLoadNextBannerCl(AdPlacementBanner adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            adpl.isLoading = true;
            tShowBannerCl = -1;
            yield return new WaitForSeconds(0.1f);
            adpl.adECPM.idxCurrEcpm = 0;
            tryLoadCollapseBanner(adpl);
#else
            yield return null;
#endif
        }
        public override void hideCollapseBanner()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads bncl admobmy hideCollapseBanner call hide");
            foreach (var adi in dicPLCl)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
            }
            isShowingCollapse = false;
            AdsAdmobMyBridge.Instance.hideBannerCl();
#endif
        }
        public override void destroyCollapseBanner()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads bncl admobmy destroyCollapseBanner");
            foreach (var adi in dicPLCl)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
                adi.Value.isloaded = false;
            }
            isShowingCollapse = false;
            AdsAdmobMyBridge.Instance.destroyBannerCl();
#endif
        }
        //bn rect
        protected override void tryLoadRectBanner(AdPlacementBanner adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads rect {adpl.loadPl}-{adpl.placement} admobmy tryLoadRectBanner");
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow = 0;
            adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].time4Count = 0;
            adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].countTimeShow = 0;
            adpl.isLoading = true;
            adpl.isloaded = false;
            AdsHelper.onAdLoad(adpl.loadPl, "banner_rect", idload, "admob");
            AdsAdmobMyBridge.Instance.loadBannerRect(adpl.placement, idload);
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads rect admobmy tryLoadRectBanner not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadRectBanner(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads rect {placement} admobmy loadRectBanner");
            AdPlacementBanner adpl = getPlBanner(placement, 2);
            if (adpl != null)
            {
                adpl.cbLoad = cb;
                if (!adpl.isLoading)
                {
                    adpl.countLoad = 0;
                    tShowBannerRect = -1;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadRectBanner(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads rect {placement}-{adpl.placement} admobmy loadRectBanner isProcessShow");
                }
            }
            else
            {
                SdkUtil.logd($"ads rect {placement} admobmy loadRectBanner not pl");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
            }
#else
            SdkUtil.logd($"ads rect {placement} admobmy loadRectBanner not enable");
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showRectBanner(string placement, int pos, float width, int maxH, float dxCenter, float dyVertical, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads rect {placement} admobmy showRectBanner");
            AdPlacementBanner adpl = getPlBanner(placement, 2);
            if (adpl != null)
            {
                adpl.isShow = true;
                adpl.posBanner = pos;
                adpl.setSetPlacementShow(placement);
                bnRectWidth = width;
                bnRectDxCenter = dxCenter;
                bnRectDyVertical = dyVertical;
                if (!adpl.isLoading)
                {
                    bnrectisnew = false;
                    int idxsh = -10;
                    long tcurr = SdkUtil.CurrentTimeMilis() / 1000;
                    for (int j = 0; j < adpl.adECPM.list.Count; j++)
                    {
                        AdECPMItem bnec = adpl.adECPM.list[j];
                        if (bnec.isLoaded)
                        {
                            string idload = bnec.adsId;
                            bool ishasnext = false;
                            if ((tcurr - bnec.timeShow) >= advhelper.currConfig.timeReloadBanner)
                            {
                                ishasnext = true;
                            }
                            SdkUtil.logd($"ads rect {placement}-{adpl.placement} admobmy showRectBanner show pre loaded adsid=" + bnec.adsId + ", idx=" + j + ", dxCenter=" + dxCenter);
                            AdsAdmobMyBridge.Instance.showBannerRect(adpl.placement, adpl.posBanner, width, maxH, dxCenter, dyVertical);
                            if (ishasnext)
                            {
                                StartCoroutine(waitLoadNextBannerRect(adpl));
                            }
                            idxsh = j;
                            break;
                        }
                    }

                    if (idxsh != -10)
                    {
                        if (cb != null)
                        {
                            cb(AD_State.AD_SHOW);
                        }
                        return true;
                    }
                    else
                    {
                        AdsAdmobMyBridge.Instance.showBannerRect(adpl.placement, adpl.posBanner, width, maxH, dxCenter, dyVertical);
                        loadRectBanner(placement, cb);
                        return false;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads rect {placement}-{adpl.placement} admobmy showRectBanner isprocess show dxCenter=" + dxCenter);
                    adpl.cbLoad = cb;
                    bool _iss = false;
                    for (int j = 0; j < adpl.adECPM.list.Count; j++)
                    {
                        AdECPMItem bnec = adpl.adECPM.list[j];
                        if (bnec.isLoaded)
                        {
                            _iss = true;
                            string idload = bnec.adsId;
                            AdsAdmobMyBridge.Instance.showBannerRect(adpl.placement, adpl.posBanner, width, maxH, dxCenter, dyVertical);
                            break;
                        }
                    }
                    if (!_iss)
                    {
                        AdsAdmobMyBridge.Instance.showBannerRect(adpl.placement, adpl.posBanner, width, maxH, dxCenter, dyVertical);
                    }
                    return false;
                }
            }
            else
            {
                SdkUtil.logd($"ads rect {placement} admobmy showRectBanner not pl");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
#else
            if (cb != null)
            {
                SdkUtil.logd($"ads rect {placement} admobmy showCollapseBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
#endif
        }
        IEnumerator waitLoadNextBannerRect(AdPlacementBanner adpl)
        {
            adpl.isLoading = true;
            tShowBannerRect = -1;
            yield return new WaitForSeconds(0.1f);
            adpl.adECPM.idxCurrEcpm = 0;
            tryLoadRectBanner(adpl);
        }
        public override void hideRectBanner()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads rect admobmy hideRectBanner");
            foreach (var adi in dicPLRect)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
            }
            AdsAdmobMyBridge.Instance.hideBannerRect();
#endif
        }
        public override void destroyRectBanner()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads rect admobmy destroyRectBanner");
            foreach (var adi in dicPLRect)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
                adi.Value.isloaded = false;
            }
            AdsAdmobMyBridge.Instance.hideBannerRect();
#endif
        }
        // bnnt
        protected override void tryLoadBnNt(AdPlacementNative adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }
            SdkUtil.logd($"ads bnnt {adpl.loadPl}-{adpl.placement} admobmy tryLoadBnNt = " + idload + ", idxCurrEcpm=" + adpl.adECPM.idxCurrEcpm);
            AdsHelper.onAdLoad(adpl.loadPl, "native_banner", idload, "admob");
            adpl.isLoading = true;
            adpl.isloaded = false;
            AdsAdmobMyBridge.Instance.loadBnNt(adpl.placement, idload);
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads bnnt {adpl.loadPl} admobmy tryLoadBnNt not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadBnNt(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementNative adpl = getPlBnNt(placement);
            if (adpl != null)
            {
                adpl.cbLoad = cb;
                if (!adpl.isLoading && !adpl.isloaded)
                {
                    SdkUtil.logd($"ads bnnt {placement}-{adpl.placement} admobmy loadBnNt");
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadBnNt(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads bnnt {placement}-{adpl.placement} admobmy loadBnNt isLoading={adpl.isLoading} isloaded={adpl.isloaded}");
                }
            }
            else
            {

                SdkUtil.logd($"ads bnnt {placement} admobmy loadBnNt not pl");
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showBnNt(string placement, int pos, int width, int maxH, AdCallBack cb, float dxCenter)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementNative adpl = getPlBnNt(placement);
            if (adpl == null)
            {
                if (cb != null)
                {
                    SdkUtil.logd($"ads bnnt {placement} admobmy showBnNt not pl");
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
            SdkUtil.logd($"ads bnnt {placement}-{adpl.placement} admobmy showBnNt pos=" + pos + ", dxCenter=" + dxCenter + ", idxEcpm=" + adpl.adECPM.idxCurrEcpm + ", countecpm=" + adpl.adECPM.list.Count);
            adpl.isShow = true;
            adpl.posBanner = pos;
            adpl.setSetPlacementShow(placement);
            bnWidth = width;
            bnDxCenter = dxCenter;
            int trefresh = PlayerPrefs.GetInt("cf_bnnt_refresh", 20);
            if (!adpl.isLoading)
            {
                bnnmisnew = false;
                int idxshow = -10;
                adpl.adECPM.idxCurrEcpm = 0;
                for (int j = 0; j < adpl.adECPM.list.Count; j++)
                {
                    AdECPMItem bnec = adpl.adECPM.list[j];
                    if (bnec.isLoaded)
                    {
                        string idload = bnec.adsId;
                        SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy showBnNt show pre loaded adsid=" + bnec.adsId + ", idx=" + j + ", dxCenter=" + dxCenter);
                        AdsAdmobMyBridge.Instance.showBnNt(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter, trefresh);
                        advhelper.hideOtherBanner(20);
                        idxshow = j;
                        break;
                    }
                }

                if (idxshow != -10)
                {
                    if (cb != null)
                    {
                        cb(AD_State.AD_SHOW);
                    }
                    return true;
                }
                else
                {
                    AdsAdmobMyBridge.Instance.showBnNt(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter, trefresh);
                    loadBnNt(placement, cb);
                    return false;
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement}-{adpl.placement} admobmy showBnNt isprocess show dxCenter=" + dxCenter);
                adpl.cbLoad = cb;
                bool _iss = false;
                for (int j = 0; j < adpl.adECPM.list.Count; j++)
                {
                    AdECPMItem bnec = adpl.adECPM.list[j];
                    if (bnec.isLoaded)
                    {
                        _iss = true;
                        string idload = bnec.adsId;
                        AdsAdmobMyBridge.Instance.showBnNt(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter, trefresh);
                        advhelper.hideOtherBanner(20);
                        break;
                    }
                }
                if (!_iss)
                {
                    AdsAdmobMyBridge.Instance.showBnNt(adpl.placement, adpl.posBanner, width, maxH, (int)advhelper.bnOrien, dxCenter, trefresh);
                }
                return false;
            }
#else
            if (cb != null)
            {
                SdkUtil.logd($"ads bn {placement} admobmy showBnNt not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
#endif
        }
        public override void hideBnNt()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads bnnt admobmy hideBnNt");
            foreach (var adi in dicPLBanner)
            {
                adi.Value.isShow = false;
                adi.Value.isRealShow = false;
            }
            AdsAdmobMyBridge.Instance.hideBnNt();
#endif
        }
        public override void destroyBnNt()
        {
            hideBnNt();
        }
        //bn nt cl
        public override void loadNtCl(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementNative adpl = getPlNtCl(placement);
            if (adpl != null)
            {
                if (!adpl.isLoading && !adpl.isloaded)
                {
                    SdkUtil.logd($"ads bnclnt {placement}-{adpl.placement} admobmy ntcl loadNativeCl");
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadNtCl(adpl, adpl.cbLoad);
                }
                else
                {
                    SdkUtil.logd($"ads bnclnt {placement}-{adpl.placement} admobmy ntcl loadNativeCl loading={adpl.isLoading}, loaded={adpl.isloaded}");
                }
            }
            else
            {
                SdkUtil.logd($"ads bnclnt {placement} admobmy ntcl loadNativeCl not pl");
            }
#endif
        }
        protected override void tryLoadNtCl(AdPlacementNative adpl, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            string idload = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            SdkUtil.logd($"ads bnclnt {adpl.loadPl}-{adpl.placement} admobmy ntcl tryLoadNtCl = " + idload + ", idxCurrEcpm=" + adpl.adECPM.idxCurrEcpm);
            AdsHelper.onAdLoad(adpl.loadPl, "native_collapse", idload, "admob");
            adpl.isLoading = true;
            adpl.isloaded = false;
            AdsAdmobMyBridge.Instance.loadNativeCl(adpl.placement, idload, (int)advhelper.bnOrien);
#endif
        }
        public override bool showNtCl(string placement, int pos, int width, float dxCenter, bool isHideBtClose, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementNative adpl = getPlNtCl(placement);
            if (adpl == null)
            {
                if (cb != null)
                {
                    SdkUtil.logd($"ads bnclnt {placement} admobmy ntcl showNtCl not pl");
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return false;
            }
            else
            {
                bool re = false;
                adpl.isShow = true;
                adpl.setSetPlacementShow(placement);
                if (adpl.isloaded)
                {
                    SdkUtil.logd($"ads bnclnt {adpl.placement} admobmy ntcl showNtCl loaded and show");
                    adpl.isloaded = false;
                    re = AdsAdmobMyBridge.Instance.showNativeCl(adpl.placement, pos, width, dxCenter, isHideBtClose, advhelper.isNtclCloseWhenClick > 0);
                }
                else if (adpl.hasLoaded)
                {
                    SdkUtil.logd($"ads bnclnt {adpl.placement} admobmy ntcl showNtCl has loaded, load new and show pre");
                    re = AdsAdmobMyBridge.Instance.showNativeCl(adpl.placement, pos, width, dxCenter, isHideBtClose, advhelper.isNtclCloseWhenClick > 0);
                }
                else
                {
                    SdkUtil.logd($"ads bnclnt {adpl.placement} admobmy ntcl showNtCl not loaded");
                }
                return re;
            }
#endif
            return false;
        }
        public override void hideNtCl()
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            SdkUtil.logd($"ads bnclnt admobmy ntcl hideNtCl");
            foreach (var adi in dicPLNativeCl)
            {
                adi.Value.isShow = false;
            }
            AdsAdmobMyBridge.Instance.hideNativeCl();
#endif
        }

#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
        #region BANNER AD EVENTS
        public void OnBannerAdLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLBanner.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLBanner[placement];
                SdkUtil.logd($"ads bn {placement} admobmy OnBannerAdLoadedEvent");
                if (adpl.isLoading)
                {
                    bnnmisnew = false;
                    adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].isLoaded = true;
                    if (adpl.isShow)
                    {
                        tShowBannerNm = 0;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow = SdkUtil.CurrentTimeMilis() / 1000;
                    }
                }
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner", adsId, "admob", adsource, true);
                adpl.isloaded = true;
                adpl.isLoading = false;
                adpl.countLoad = 0;

                if (adpl.isShow)
                {
                    if (advhelper.bnCurrShow == adsType)
                    {
                        SdkUtil.logd($"ads bn {placement} admobmy OnBannerAdLoadedEvent hide other");
                        advhelper.hideOtherBanner(adsType);
                    }
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
                SdkUtil.logd($"ads bn {placement} admobmy OnBannerAdLoadedEvent not pl");
            }
        }
        private void OnBannerAdLoadFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLBanner.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLBanner[placement];
                SdkUtil.logd($"ads bn {adpl.loadPl}-{placement} admobmy OnBannerAdLoadFailedEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner", adsId, "admob", "", false);
                if (adpl.isLoading)
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl}-{placement} admobmy OnBannerAdLoadFailedEvent isloading=true");
                    if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                    {
                        if (bnnmisnew)
                        {
                            bnnmisnew = false;
                            adpl.adECPM.idxCurrEcpm = 0;
                        }
                        else
                        {
                            adpl.adECPM.idxCurrEcpm++;
                        }
                        if (isShowHighPriorityBanner)
                        {
                            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
                            {
                                tryLoadBanner(adpl);
                            }
                            else
                            {
                                adpl.adECPM.idxCurrEcpm = 0;
                                adpl.isLoading = false;
                                AdsProcessCB.Instance().Enqueue(() =>
                                {
                                    if (adpl.cbLoad != null)
                                    {
                                        var tmpcb = adpl.cbLoad;
                                        adpl.cbLoad = null;
                                        tmpcb(AD_State.AD_LOAD_FAIL);
                                    }
                                });
                            }
                        }
                        else
                        {
                            tryLoadBanner(adpl);
                        }
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
                SdkUtil.logd($"ads bn {placement} admobmy not pl OnBannerAdLoadFailedEvent=" + err);
            }
        }
        private void OnBannerClickEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads bn {placement} admobmy OnBannerClick");
            SDKManager.Instance.onClickAd();
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "banner", "admob", adsource, adsId);
        }
        private void OnBannerImpression(string placement, string adsId, string adnet)
        {
            if (dicPLBanner.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLBanner[placement];
                SdkUtil.logd($"ads bn {adpl.showPl}-{placement} admobmy OnBannerImpression");
                if (bnnmisnew && !adpl.isCheckNewIds)
                {
                    adpl.isCheckNewIds = true;
                    adpl.isloaded = false;
                }
                if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm && AdsHelper.Instance.statusLogicIron > 0 && isBannerHigh)
                {
                    SdkUtil.logd($"ads bn {adpl.showPl}-{placement} admobmy OnBannerImpression {placement} hideOtherBanner");
                    advhelper.hideOtherBanner(adsType);
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy OnBannerImpression not pl");
            }
        }
        private void OnBannerAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            FIRhelper.logEvent("show_ads_bn");
            FIRhelper.logEvent("show_ads_bn_nm_0");
            string spl = SDKManager.Instance.currPlacement;
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(0);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);

            if (dicPLBanner.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLBanner[placement];
            }
        }
        #endregion
        #region BANNER Collapse AD EVENTS
        public void OnBannerClAdLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLCl.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLCl[placement];
                SdkUtil.logd($"ads bncl {adpl.loadPl}-{placement} admobmy OnBannerClAdLoadedEvent");
                statusShowCl = 1;
                if (adpl.isLoading)
                {
                    bnclisnew = false;
                    adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].isLoaded = true;
                    if (adpl.isShow)
                    {
                        tShowBannerCl = 0;
                        if (tChangeCl2Nm < 0)
                        {
                            tChangeCl2Nm = 0;
                        }
                        isShowingCollapse = true;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow = SdkUtil.CurrentTimeMilis() / 1000;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].time4Count = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].countTimeShow = 0;
                    }
                }
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner_collapse", adsId, "admob", adsource, true);
                adpl.isloaded = true;
                adpl.isLoading = false;
                adpl.countLoad = 0;

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
        }
        private void OnBannerClAdLoadFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLCl.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLCl[placement];
                SdkUtil.logd($"ads bncl {adpl.loadPl}-{placement} admobmy OnBannerClAdLoadFailedEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner_collapse", adsId, "admob", "", false);
                if (adpl.isLoading)
                {
                    if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                    {
                        if (bnclisnew)
                        {
                            bnclisnew = false;
                            adpl.adECPM.idxCurrEcpm = 0;
                        }
                        else
                        {
                            adpl.adECPM.idxCurrEcpm++;
                        }
                        tryLoadCollapseBanner(adpl);
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
                            if (!isShowingCollapse)
                            {
                                SdkUtil.logd($"ads bncl {adpl.loadPl}-{placement} admobmy OnBannerClAdLoadFailedEvent show bn when cl load fail isShowingCollapse = false isshow={adpl.isShow}");
                                advhelper.hideBannerCollapse();
                                if (adpl.isShow)
                                {
                                    advhelper.showBanner(adpl.loadPl, (AD_BANNER_POS)adpl.posBanner, advhelper.bnOrien, 0, bnClWidth, advhelper.bnMaxH, bnClDxCenter);
                                }
                            }
                            else
                            {
                                SdkUtil.logd($"ads bncl {adpl.loadPl}-{placement} admobmy OnBannerClAdLoadFailedEvent show bn when cl load fail isShowingCollapse = true");
                            }
                        });
                    }
                }
            }
            else
            {
                SdkUtil.logd($"ads bncl {placement} admobmy not pl OnBannerClAdLoadFailedEvent=" + err);
            }
        }
        private void OnBannerClImpression(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads bncl {placement} admobmy OnBannerClImpression");
            if (dicPLCl.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLCl[placement];
                if (bnclisnew && !adpl.isCheckNewIds)
                {
                    adpl.isCheckNewIds = true;
                    adpl.isloaded = false;
                }
            }
        }
        private void OnBannerClOpen(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads bncl {placement} admobmy OnBannerClOpen");
            advhelper.isBannerClExpand = true;
            if (statusShowCl == 1)
            {
                StatusClViewShow++;
                if (advhelper.currConfig.typeAutoReloadBannerCl == 0)
                {
                    tShowBannerCl = -1;
                }
                FIRhelper.logEvent("show_ads_banner_cletr");
                SdkUtil.logd($"ads bncl {placement} admobmy OnBannerClOpen isClViewShow");
            }
        }
        private void OnBannerClClickEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads bncl {placement} admobmy OnBannerClClick");
            SDKManager.Instance.onClickAd();
            statusShowCl = 2;
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLCl.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLCl[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "banner_collapse", "admob", adsource, adsId);
        }
        private void OnBannerClClose(string placement, string adsId, string adnet)
        {
            if (dicPLCl.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLCl[placement];
                SdkUtil.logd($"ads bncl {adpl.showPl}-{placement} admobmy OnBannerClClose");
                advhelper.isBannerClExpand = false;
                StatusClViewShow--;
                if (StatusClViewShow < 0)
                {
                    StatusClViewShow = 0;
                }
                if (flagChangecl2Nm == 2)
                {
                    SdkUtil.logd($"ads bncl {adpl.showPl}-{placement} admobmy OnBannerClClose change collapse to banner");
                    tChangeCl2Nm = -1;
                    tShowBannerCl = -1;
                    advhelper.hideBannerCollapse();
                    if (adpl.posBanner == 0)
                    {
                        advhelper.showBanner(adpl.showPl, AD_BANNER_POS.TOP, advhelper.bnOrien, 0, bnClWidth, advhelper.bnMaxH, bnClDxCenter);
                    }
                    else
                    {
                        advhelper.showBanner(adpl.showPl, AD_BANNER_POS.BOTTOM, advhelper.bnOrien, 0, bnClWidth, advhelper.bnMaxH, bnClDxCenter);
                    }
                }
                else if (flagChangecl2Nm == 1)
                {
                    SdkUtil.logd($"ads bncl {adpl.showPl}-{placement} admobmy OnBannerClClose typerl=" + advhelper.currConfig.typeAutoReloadBannerCl + ", StatusClViewShow=" + StatusClViewShow);
                    if (advhelper.currConfig.typeAutoReloadBannerCl == 0 && StatusClViewShow <= 0)
                    {
                        tShowBannerCl = 0;
                    }
                }
            }
            else
            {
                SdkUtil.logd($"ads bncl {placement} admobmy OnBannerClClose not pl");
            }
        }
        private void OnBannerClAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            FIRhelper.logEvent("show_ads_bn");
            FIRhelper.logEvent("show_ads_bn_cl_0");
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLCl.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLCl[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(1);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
        #region BANNER Rect AD EVENTS
        public void OnBannerRectAdLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLRect.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLRect[placement];
                SdkUtil.logd($"ads rect {adpl.loadPl}-{placement} admobmy OnBannerRectAdLoadedEvent");
                if (adpl.isLoading)
                {
                    bnrectisnew = false;
                    adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].isLoaded = true;
                    if (adpl.isShow)
                    {
                        tShowBannerRect = 0;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow = SdkUtil.CurrentTimeMilis() / 1000;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].time4Count = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].countTimeShow = 0;
                    }
                }
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner_rect", adsId, "admob", adsource, true);
                adpl.isloaded = true;
                adpl.isLoading = false;
                adpl.countLoad = 0;

                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads rect {placement} admobmy OnBannerRectAdLoadedEvent not pl");
            }
        }
        private void OnBannerRectAdLoadFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLRect.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLRect[placement];
                SdkUtil.logd($"ads rect {adpl.loadPl}-{placement} admobmy OnBannerRectAdLoadFailedEvent err={err}");
                AdsHelper.onAdLoadResult(adpl.loadPl, "banner_rect", adsId, "admob", "", false);
                if (adpl.isLoading)
                {
                    if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                    {
                        if (bnrectisnew)
                        {
                            bnrectisnew = false;
                            adpl.adECPM.idxCurrEcpm = 0;
                        }
                        else
                        {
                            adpl.adECPM.idxCurrEcpm++;
                        }
                        tryLoadRectBanner(adpl);
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
                        });
                    }
                }
            }
            else
            {
                SdkUtil.logd($"ads rect {placement} admobmy OnBannerRectAdLoadFailedEvent not pl err={err}");
            }
        }
        private void OnBannerRectImpression(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads rect {placement} admobmy OnBannerRectImpression");
            if (dicPLRect.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLRect[placement];
                if (bnrectisnew && !adpl.isCheckNewIds)
                {
                    adpl.isCheckNewIds = true;
                    adpl.isloaded = false;
                }
            }
        }
        private void OnBannerRectClickEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads rect {placement} admobmy OnBannerRectClick");
            SDKManager.Instance.onClickAd();
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLRect.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLRect[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "banner_rect", "admob", adsource, adsId);
        }
        private void OnBannerRectAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            FIRhelper.logEvent("show_ads_bn");
            FIRhelper.logEvent("show_ads_bn_rect_0");
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLRect.ContainsKey(placement))
            {
                AdPlacementBanner adpl = dicPLRect[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(2);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
        #region Banner native
        public void OnBnNtAdLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLBnNt.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLBnNt[placement];
                SdkUtil.logd($"ads bnnt {placement} admobmy OnBnNtAdLoadedEvent");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_banner", adsId, "admob", adsource, true);
                if (adpl.isLoading)
                {
                    bnnmisnew = false;
                    adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].isLoaded = true;
                    if (adpl.isShow)
                    {
                        tShowBannerNm = 0;
                        adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].timeShow = SdkUtil.CurrentTimeMilis() / 1000;
                    }
                }
                adpl.isloaded = true;
                adpl.isLoading = false;
                adpl.countLoad = 0;

                if (adpl.isShow)
                {
                    if (advhelper.bnCurrShow == 20)
                    {
                        SdkUtil.logd($"ads bnnt {placement} admobmy OnBnNtAdLoadedEvent hide other");
                        advhelper.hideOtherBanner(20);
                    }
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

                SdkUtil.logd($"ads bnnt {placement} admobmy OnBnNtAdLoadedEvent not pl");
            }
        }
        private void OnBnNtAdLoadFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLBnNt.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLBnNt[placement];
                SdkUtil.logd($"ads bnnt {adpl.loadPl}-{placement} admobmy OnBnNtAdLoadFailedEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_banner", adsId, "admob", "", false);
                if (adpl.isLoading)
                {
                    SdkUtil.logd($"ads bnnt {adpl.loadPl}-{placement} admobmy OnBnNtAdLoadFailedEvent isloading=true");
                    if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                    {
                        if (bnnmisnew)
                        {
                            bnnmisnew = false;
                            adpl.adECPM.idxCurrEcpm = 0;
                        }
                        else
                        {
                            adpl.adECPM.idxCurrEcpm++;
                        }
                        if (isShowHighPriorityBanner)
                        {
                            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
                            {
                                tryLoadBnNt(adpl);
                            }
                            else
                            {
                                adpl.adECPM.idxCurrEcpm = 0;
                                adpl.isLoading = false;
                                AdsProcessCB.Instance().Enqueue(() =>
                                {
                                    if (adpl.cbLoad != null)
                                    {
                                        var tmpcb = adpl.cbLoad;
                                        adpl.cbLoad = null;
                                        tmpcb(AD_State.AD_LOAD_FAIL);
                                    }
                                });
                            }
                        }
                        else
                        {
                            tryLoadBnNt(adpl);
                        }
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
                SdkUtil.logd($"ads bnnt {placement} admobmy not pl OnBnNtAdLoadFailedEvent=" + err);
            }
        }
        private void OnBnNtClickEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads bnnt {placement} admobmy OnBnNtClick");
            SDKManager.Instance.onClickAd();
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(SDKManager.Instance.currPlacement, "native_banner", "admob", adsource, adsId);
        }
        private void OnBnNtImpression(string placement, string adsId, string adnet)
        {
            if (dicPLBnNt.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLBnNt[placement];
                SdkUtil.logd($"ads bnnt {adpl.showPl}-{placement} admobmy OnBnNtImpression");
                if (bnntisnew && !adpl.isCheckNewIds)
                {
                    adpl.isCheckNewIds = true;
                    adpl.isloaded = false;
                }
                if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm && AdsHelper.Instance.statusLogicIron > 0 && isBannerHigh)
                {
                    SdkUtil.logd($"ads bnnt {adpl.showPl}-{placement} admobmy OnBnNtImpression {placement} hideOtherBanner");
                    advhelper.hideOtherBanner(20);
                }
            }
            else
            {
                SdkUtil.logd($"ads bnnt {placement} admobmy OnBnNtImpression not pl");
            }
        }
        private void OnBnNtAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            SdkUtil.logd($"ads bnnt {placement} admobmy bnnt OnBnNtAdPaidEvent va={valueMicros}");
            FIRhelper.logEvent("show_ads_nt");
            FIRhelper.logEvent("show_ads_nt_bn");
            string spl = SDKManager.Instance.currPlacement;
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(10);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
        #region Banner Native Collapse AD EVENTS
        private void OnNativeClLoadedEvent(string placement, string adsId, string adnet)
        {
            if (adnet != null && adnet.EndsWith("@@@"))
            {
                adnet = adnet.Replace("@@@", "");
                advhelper.isQcThu = true;
                if (advhelper.islogttttttt == 0)
                {
                    advhelper.islogttttttt = 1;
                    PlayerPrefs.SetInt("mem_is_log_ttt", advhelper.islogttttttt);
                    FIRhelper.logEvent("ads_test");
                    string dv = $"atn{GameHelper.Instance.AdsIdentify}";
                    dv = dv.Replace("-", "");
                    FIRhelper.logEvent(dv);
                }
            }
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                SdkUtil.logd($"ads bn {adpl.loadPl}-{placement} admobmy ntcl OnNativeClLoadedEvent");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_collapse", adsId, "admob", adsource, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                adpl.hasLoaded = true;
                bnntclisnew = false;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy ntcl OnNativeClLoadedEvent not pl");
            }
        }
        private void OnNativeClFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_collapse", adsId, "admob", "", false);
                if (adpl.isLoading)
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl}-{placement} admobmy ntcl onload loading fail=" + err);
                    if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                    {
                        if (bnntclisnew)
                        {
                            bnntclisnew = false;
                            adpl.adECPM.idxCurrEcpm = 0;
                        }
                        else
                        {
                            adpl.adECPM.idxCurrEcpm++;
                        }
                        tryLoadNtCl(adpl, adpl.cbLoad);
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
                        });
                    }
                }
                else
                {
                    SdkUtil.logd($"ads bn {adpl.loadPl}-{placement} admobmy ntcl onload not loading fail=" + err);
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy ntcl onload not pl fail=" + err);
            }
        }
        private void OnNativeClDisplayedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                SdkUtil.logd($"ads bn {adpl.showPl}-{placement} admobmy ntcl onshow flag={advhelper.ntclCountShowing}");
                if (adpl.flagCountShow)
                {
                    adpl.flagCountShow = false;
                    advhelper.ntclCountShowing++;
                }
                if (advhelper.isShowBanner)
                {
                    advhelper.statusShowBannerAfterCloseNtCl = 1;
                    advhelper.hideBanner(0, true);
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy ntcl onshow not pl");
            }
            advhelper.onBnClCb(placement, AD_State.AD_SHOW);
        }
        private void OnNativeClImpresstionEvent(string placement, string adsId, string adnet)
        {
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                SdkUtil.logd($"ads bn {adpl.showPl}-{placement} admobmy ntcl OnNativeClImpresstionEvent");
                adpl.isloaded = false;
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy ntcl OnNativeClImpresstionEvent not pl");
            }
        }
        private void OnNativeClFailedToShow(string placement, string adsId, string adnet, string err)
        {
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                SdkUtil.logd($"ads bn {adpl.showPl}-{placement} admobmy ntcl onshowfail=" + err);
                adpl.isLoading = false;
                adpl.isloaded = false;
                //if (adpl.cbShow != null)
                //{
                //    AdCallBack tmpcb = adpl.cbShow;
                //    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                //}
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy ntcl not pl onshowfail=" + err);
            }
            advhelper.onBnClCb(placement, AD_State.AD_SHOW_FAIL);
        }
        private void OnNativeClClickEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads bn {placement} admobmy ntcl OnNativeClClickEvent");
            SDKManager.Instance.onClickAd();
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "native_collapse", "admob", adsource, adsId);
        }
        private void OnNativeClDismissedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                SdkUtil.logd($"ads bn {adpl.showPl}-{placement} admobmy ntcl onclose flag={advhelper.statusShowBannerAfterCloseNtCl}");
                advhelper.ntclCountShowing--;
                adpl.flagCountShow = true;
                if (advhelper.statusShowBannerAfterCloseNtCl == 1)
                {
                    if (advhelper.ntclCountShowing <= 0)
                    {
                        advhelper.ntclCountShowing = 0;
                        advhelper.statusShowBannerAfterCloseNtCl = 0;
                        advhelper.prebannerwhenCloseNtCl();
                        advhelper.showBanner(advhelper.memPlacementBn, advhelper.bnPos, advhelper.bnOrien, 0, bnClWidth, advhelper.bnMaxH, bnClDxCenter);
                    }
                }
            }
            else
            {
                SdkUtil.logd($"ads bn {placement} admobmy ntcl onclose not pl");
            }
            advhelper.onBnClCb(placement, AD_State.AD_CLOSE);
        }
        private void OnNativeClAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            SdkUtil.logd($"ads bn {placement} admobmy ntcl OnNativeClAdPaidEvent va={valueMicros}");
            FIRhelper.logEvent("show_ads_nt");
            FIRhelper.logEvent("show_ads_nt_cl");
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNativeCl.ContainsKey(placement))
            {
                AdPlacementNative adpl = dicPLNativeCl[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(6);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
#endif
    }
}