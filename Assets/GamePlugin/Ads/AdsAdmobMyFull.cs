using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mygame.sdk
{
    partial class AdsAdmobMy
    {
        private bool isFullRewardCom = false;

        private void initNativeFull()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log("mysdk: ads full nt admobmy init adCfPlacementNtFull=" + advhelper.currConfig.adCfPlacementNtFull);
                bool isFull2 = false;
                if (advhelper.currConfig.adCfPlacementNtFull.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementNtFull.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementFull>(dicPLNtFull, plitem, true);
                        if (!isFull2 && plitem.Contains(PLFull2Default))
                        {
                            isFull2 = true;
                        }
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlNtFull.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementFull>(dicPLNtFull, plitem, false);
                    if (!isFull2 && plitem.Contains(PLFull2Default))
                    {
                        isFull2 = true;
                    }
                }
                if (dicPLNtFull.ContainsKey(PLFullDefault))
                {
                    if (!dicPLNtFull.ContainsKey(PLFull2Default))
                    {
                        AdPlacementFull adplfull2 = new AdPlacementFull();
                        adplfull2.coppyFrom(dicPLNtFull[PLFullDefault]);
                        adplfull2.placement = PLFull2Default;
                        dicPLNtFull.Add(PLFull2Default, adplfull2);
                    }
                    else
                    {
                        if (!isFull2)
                        {
                            AdPlacementFull adplfull2 = dicPLNtFull[PLFull2Default];
                            List<AdECPMItem> tmpl = new List<AdECPMItem>();
                            tmpl.AddRange(adplfull2.adECPM.list);
                            adplfull2.coppyFrom(dicPLNtFull[PLFullDefault]);
                            adplfull2.placement = PLFull2Default;

                            for (int i = 0; i < tmpl.Count; i++)
                            {
                                for (int j = 0; j < adplfull2.adECPM.list.Count; j++)
                                {
                                    if (adplfull2.adECPM.list[j].adsId.CompareTo(tmpl[i].adsId) == 0)
                                    {
                                        adplfull2.adECPM.list[j].coppyFrom(tmpl[i]);
                                        tmpl.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                                }
                            }
                            if (tmpl.Count > 0)
                            {
                                adplfull2.adECPM.list.AddRange(tmpl);
                            }
                            tmpl.Clear();
                        }
                    }
                }

                string memcfntfull = PlayerPrefs.GetString("mem_cf_ntfull_lic", "30,105,70,2,10");
                setCfNtFull(memcfntfull);
                string memcfdayclisk = PlayerPrefs.GetString("mem_cf_nt_dayflic", "2,10,4;3,25,4;4,40,3;5,50,2");
                setCfNtdayClick(memcfdayclisk);
                string memcfntfullfbex = PlayerPrefs.GetString("cf_ntfull_fb_excluse", "8;8;7");
                setCfNtFullFbExcluse(memcfntfullfbex);
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ads full nt admobmy initNativeFull ex=" + ex.ToString());
            }
        }
        private void initNativeIcFull()
        {
            if (adsType != 0)
            {
                return;
            }

            try
            {
                Debug.Log("mysdk: ads full nt admobmy init adCfPlacementNtIcFull=" + advhelper.currConfig.adCfPlacementNtIcFull);
                bool isFull2 = false;
                if (advhelper.currConfig.adCfPlacementNtIcFull.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementNtIcFull.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementFull>(dicPLNtIcFull, plitem, true);
                        if (!isFull2 && plitem.Contains(PLFull2Default))
                        {
                            isFull2 = true;
                        }
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlNtIcFull.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementFull>(dicPLNtIcFull, plitem, false);
                    if (!isFull2 && plitem.Contains(PLFull2Default))
                    {
                        isFull2 = true;
                    }
                }
                if (dicPLNtIcFull.ContainsKey(PLFullDefault))
                {
                    if (!dicPLNtIcFull.ContainsKey(PLFull2Default))
                    {
                        AdPlacementFull adplfull2 = new AdPlacementFull();
                        adplfull2.coppyFrom(dicPLNtIcFull[PLFullDefault]);
                        adplfull2.placement = PLFull2Default;
                        dicPLNtIcFull.Add(PLFull2Default, adplfull2);
                    }
                    else
                    {
                        if (!isFull2)
                        {
                            AdPlacementFull adplfull2 = dicPLNtIcFull[PLFull2Default];
                            List<AdECPMItem> tmpl = new List<AdECPMItem>();
                            tmpl.AddRange(adplfull2.adECPM.list);
                            adplfull2.coppyFrom(dicPLNtIcFull[PLFullDefault]);
                            adplfull2.placement = PLFull2Default;

                            for (int i = 0; i < tmpl.Count; i++)
                            {
                                for (int j = 0; j < adplfull2.adECPM.list.Count; j++)
                                {
                                    if (adplfull2.adECPM.list[j].adsId.CompareTo(tmpl[i].adsId) == 0)
                                    {
                                        adplfull2.adECPM.list[j].coppyFrom(tmpl[i]);
                                        tmpl.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                                }
                            }
                            if (tmpl.Count > 0)
                            {
                                adplfull2.adECPM.list.AddRange(tmpl);
                            }
                            tmpl.Clear();
                        }
                    }
                }

                string memcfntfull = PlayerPrefs.GetString("mem_cf_ntfull_lic", "30,105,70,2,10");
                setCfNtFull(memcfntfull);
                string memcfntfullfbex = PlayerPrefs.GetString("cf_ntfull_fb_excluse", "8;8;7");
                setCfNtFullFbExcluse(memcfntfullfbex);
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ads full nt admobmy initNativeFull ex=" + ex.ToString());
            }
        }
        private void initFull()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log("mysdk: ads full admobmy init adCfPlacementFull=" + advhelper.currConfig.adCfPlacementFull);
                if (advhelper.currConfig.adCfPlacementFull.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementFull.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementFull>(dicPLFull, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlFullAll.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementFull>(dicPLFull, plitem, false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ads full admobmy initFull ex=" + ex.ToString());
            }
        }
        private void initFullRwInter()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log("mysdk: ads full admobmy init adCfPlacementFullRwInter=" + advhelper.currConfig.adCfPlacementFullRwInter);
                if (advhelper.currConfig.adCfPlacementFullRwInter.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementFullRwInter.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementFull>(dicPLFullRwInter, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlFullRwInter.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementFull>(dicPLFullRwInter, plitem, false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ads full admobmy initFullRwInter ex=" + ex.ToString());
            }
        }
        private void initFullRwRw()
        {
            if (adsType != 0)
            {
                return;
            }
            try
            {
                Debug.Log("mysdk: ads full admobmy init adCfPlacementFullRwRw=" + advhelper.currConfig.adCfPlacementFullRwRw);
                if (advhelper.currConfig.adCfPlacementFullRwRw.Length > 0)
                {
                    string[] listpl = advhelper.currConfig.adCfPlacementFullRwRw.Split(new char[] { '#' });
                    foreach (string plitem in listpl)
                    {
                        addAdPlacement<AdPlacementFull>(dicPLFullRwRw, plitem, true);
                    }
                }
                string[] listpldf = AdIdsConfig.AdmobPlFullRwRw.Split(new char[] { '#' });
                foreach (string plitem in listpldf)
                {
                    addAdPlacement<AdPlacementFull>(dicPLFullRwRw, plitem, false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ads full admobmy initFullRwRw ex=" + ex.ToString());
            }
        }

        //full nt
        public override int getNativeFullLoaded(string placement)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlNtFull(placement, true);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} nt admobmy getNativeFullLoaded not pl");
                return 0;
            }
            else
            {
                if (adpl.isloaded)
                {
                    return 1;
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy getNativeFullLoaded={adpl.isloaded}");
                }
            }
#endif
            return 0;
        }
        protected override void tryLoadNativeFull(AdPlacementFull adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            string idLoad = "";
            adpl.isAdHigh = false;
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            idLoad = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }

            int tryload = adpl.countLoad;
            if (tryload >= toTryLoad)
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} nt admobmy tryLoadNativeFull over try");
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }

                return;
            }
            if (idLoad != null && idLoad.Contains("ca-app-pub"))
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} nt admobmy tryLoadNativeFull id={idLoad} idxCurrEcpmFull={adpl.adECPM.idxCurrEcpm} isFullHigh={adpl.isAdHigh} plid={adpl.idPl}");
                AdsHelper.onAdLoad(adpl.loadPl, "native_full", idLoad, "admob");
                adpl.isLoading = true;
                adpl.isloaded = false;
                AdsAdmobMyBridge.Instance.loadNativeFull(adpl.placement, idLoad, (int)advhelper.bnOrien);
            }
            else
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} nt admobmy tryLoadNativeFull id not correct");
                adpl.isLoading = false;
                adpl.isloaded = false;
            }
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads full {adpl.placement} nt admobmy tryLoadNativeFull not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadNativeFull(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlNtFull(placement, false);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} nt admobmy loadNativeFull not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
                if (!adpl.isloaded && !adpl.isLoading && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy loadNativeFull type={adsType}");
                    adpl.cbLoad = cb;
                    nativefullisnew = false;
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadNativeFull(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy loadNativeFull isloading={adpl.isLoading} or isloaded={adpl.isloaded} showing={adpl.getShowing()}");
                }
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showNativeFull(string placement, float timeDelay, int timeNtDl, bool isHideBtClose, bool isShow2, int timeClose, bool isAutoCloseWhenClick, AdCallBack cb)
        {
            isFullNt2 = isShow2;
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlNtFull(placement, true);
            if (adpl != null)
            {
                //adpl.cbShow = null;
                int ss = getNativeFullLoaded(adpl.placement);
                if (ss > 0 && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy showNativeFull call show");
                    adpl.countLoad = 0;
                    adpl.cbShow = cb;
                    adpl.setSetPlacementShow(placement);
                    AdsHelper.onAdShowStart(placement, "native_full", "admob", "");
                    bool iss = AdsAdmobMyBridge.Instance.showNativeFull(adpl.placement, !isHideBtClose, timeClose, timeNtDl, isAutoCloseWhenClick);
                    adpl.setShowing(iss);
                    return iss;
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy showNativeFull not load or showing={adpl.getShowing()}");
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy showNativeFull not pl");
            }
#endif
            return false;
        }
        public static void reCountNtFull()
        {
#if UNITY_IOS || UNITY_IPHONE
            AdsAdmobMyiOSBridge.reCountCurrShow();
#endif
        }

        //full nt Ic
        public override int getNativeIcFullLoaded(string placement)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlNtIcFull(placement, true);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} nt admobmy getNativeIcFullLoaded not pl");
                return 0;
            }
            else
            {
                if (adpl.isloaded)
                {
                    return 1;
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy getNativeIcFullLoaded={adpl.isloaded}");
                }
            }
#endif
            return 0;
        }
        protected override void tryLoadNativeIcFull(AdPlacementFull adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            string idLoad = "";
            adpl.isAdHigh = false;
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            idLoad = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }

            int tryload = adpl.countLoad;
            if (tryload >= toTryLoad)
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} nt admobmy tryLoadNativeIcFull over try");
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }

                return;
            }
            if (idLoad != null && idLoad.Contains("ca-app-pub"))
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} nt admobmy tryLoadNativeIcFull id={idLoad} idxCurrEcpmFull={adpl.adECPM.idxCurrEcpm} isFullHigh={adpl.isAdHigh} plid={adpl.idPl}");
                AdsHelper.onAdLoad(adpl.loadPl, "native_full", idLoad, "admob");
                adpl.isLoading = true;
                adpl.isloaded = false;
                AdsAdmobMyBridge.Instance.loadNativeIcFull(adpl.placement, idLoad, (int)advhelper.bnOrien);
            }
            else
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} nt admobmy tryLoadNativeIcFull id not correct");
                adpl.isLoading = false;
                adpl.isloaded = false;
            }
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads full {adpl.placement} nt admobmy tryLoadNativeIcFull not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadNativeIcFull(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY

#if UNITY_IOS || UNITY_IPHONE
            SdkUtil.logd($"ads full {placement} nt admobmy loadNativeIcFull not in ios");
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
            return;
#endif

            AdPlacementFull adpl = getPlNtIcFull(placement, false);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} nt admobmy loadNativeIcFull not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
                if (!adpl.isloaded && !adpl.isLoading && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy loadNativeIcFull type={adsType}");
                    adpl.cbLoad = cb;
                    nativefullisnew = false;
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadNativeIcFull(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy loadNativeIcFull isloading={adpl.isLoading} or isloaded={adpl.isloaded} showing={adpl.getShowing()}");
                }
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showNativeIcFull(string placement, float timeDelay, int timeNtDl, bool isHideBtClose, bool isShow2, int timeClose, bool isAutoCloseWhenClick, AdCallBack cb)
        {
            isFullNt2 = isShow2;
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlNtIcFull(placement, true);
            if (adpl != null)
            {
                //adpl.cbShow = null;
                int ss = getNativeIcFullLoaded(adpl.placement);
                if (ss > 0 && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy showNativeIcFull call show");
                    adpl.countLoad = 0;
                    adpl.cbShow = cb;
                    adpl.setSetPlacementShow(placement);
                    AdsHelper.onAdShowStart(placement, "native_full", "admob", "");
                    bool iss = AdsAdmobMyBridge.Instance.showNativeIcFull(adpl.placement, !isHideBtClose, timeClose, timeNtDl, isAutoCloseWhenClick);
                    adpl.setShowing(iss);
                    return iss;
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} nt admobmy showNativeIcFull not load or showing={adpl.getShowing()}");
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy showNativeIcFull not pl");
            }
#endif
            return false;
        }

        //full
        public override void clearCurrFull(string placement)
        {
            if (getFullLoaded(placement) == 1)
            {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
                AdsAdmobMyBridge.Instance.clearCurrFull(placement);
                AdPlacementFull adpl = getPlFull(placement, true);
                if (adpl != null)
                {
                    adpl.isloaded = false;
                    adpl.isAdHigh = false;
                }
#endif
            }
        }
        public override int getFullLoaded(string placement)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFull(placement, true);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} admobmy getFullLoaded not pl");
                return 0;
            }
            else
            {
                //SdkUtil.logd($"ads full {placement}-{adpl.placement} admobmy getFullLoaded={adpl.isloaded}");
                if (adpl.isloaded)
                {
                    if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm && adpl.isAdHigh)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} admobmy getFullLoaded={adpl.isloaded}");
                }
            }
#endif
            return 0;
        }
        protected override void tryLoadFull(AdPlacementFull adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            string idLoad = "";
            adpl.isAdHigh = false;
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            idLoad = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }

            int tryload = adpl.countLoad;
            if (tryload >= toTryLoad)
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} admobmy tryLoadFull over try");
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }

                return;
            }
            if (idLoad != null && idLoad.Contains("ca-app-pub"))
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} admobmy tryLoadFull id={idLoad} idxCurrEcpmFull={adpl.adECPM.idxCurrEcpm} isFullHigh={adpl.isAdHigh}");
                AdsHelper.onAdLoad(adpl.loadPl, "interstitial", idLoad, "admob");
                adpl.isLoading = true;
                adpl.isloaded = false;
                AdsAdmobMyBridge.Instance.loadFull(adpl.placement, idLoad);
            }
            else
            {
                SdkUtil.logd($"ads full {adpl.loadPl}-{adpl.placement} admobmy tryLoadFull id not correct");
                adpl.isLoading = false;
                adpl.isloaded = false;
            }
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads full {adpl.placement} admobmy tryLoadFull not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadFull(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFull(placement, false);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full {placement} admobmy loadFull not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
                if (!adpl.isloaded && !adpl.isLoading && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} admobmy loadFull");
                    adpl.cbLoad = cb;
                    fullisnew = false;
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadFull(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads full {placement}-{adpl.placement} admobmy loadFull isloading={adpl.isLoading} or isloaded={adpl.isloaded} showing={adpl.getShowing()}");
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
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFull(placement, true);
            if (adpl != null)
            {
                //adpl.cbShow = null;
                int ss = getFullLoaded(adpl.placement);
                if (ss > 0 && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full {placement} admobmy showFull timeDelay={timeDelay}");
                    adpl.countLoad = 0;
                    adpl.cbShow = cb;
                    adpl.setSetPlacementShow(placement);
                    if (timeDelay > 0)
                    {
                        adpl.setShowing(true);
                        AdsProcessCB.Instance().Enqueue(() =>
                        {
                            AdsHelper.onAdShowStart(placement, "interstitial", "admob", "");
                            bool iss = AdsAdmobMyBridge.Instance.showFull(adpl.placement);
                            if (!iss)
                            {
                                adpl.setShowing(false);
                                if (cb != null)
                                {
                                    cb(AD_State.AD_SHOW_FAIL);
                                }
                            }
                        }, timeDelay);
                        return true;
                    }
                    else
                    {
                        AdsHelper.onAdShowStart(placement, "interstitial", "admob", "");
                        bool iss = AdsAdmobMyBridge.Instance.showFull(adpl.placement);
                        adpl.setShowing(iss);
                        return iss;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads full {placement} admobmy showFull not load or showing={adpl.getShowing()}");
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} admobmy showFull not pl");
            }
#endif
            return false;
        }

        //full Rw Inter
        protected override void tryLoadFullRwInter(AdPlacementFull adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            string idLoad = "";
            adpl.isAdHigh = false;
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            idLoad = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }

            int tryload = adpl.countLoad;
            if (tryload >= toTryLoad)
            {
                SdkUtil.logd($"ads full rwinter {adpl.loadPl}-{adpl.placement} admobmy tryLoadFullRwInter over try");
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }

                return;
            }
            if (idLoad != null && idLoad.Contains("ca-app-pub"))
            {
                SdkUtil.logd($"ads full rwinter {adpl.loadPl}-{adpl.placement} admobmy tryLoadFullRwInter id={idLoad} idxCurrEcpmFull={adpl.adECPM.idxCurrEcpm} isFullHigh={adpl.isAdHigh}");
                AdsHelper.onAdLoad(adpl.loadPl, "rewarded_interstitial", idLoad, "admob");
                adpl.isLoading = true;
                adpl.isloaded = false;
                AdsAdmobMyBridge.Instance.loadFullRwInter(adpl.placement, idLoad);
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {adpl.loadPl}-{adpl.placement} admobmy tryLoadFullRwInter id not correct");
                adpl.isLoading = false;
                adpl.isloaded = false;
            }
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads full rwinter {adpl.placement} admobmy tryLoadFullRwInter not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadFullRwInter(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFullRwInter(placement, false);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy loadFullRwInter not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
                if (!adpl.isloaded && !adpl.isLoading && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full rwinter {placement}-{adpl.placement} admobmy loadFullRwInter");
                    adpl.cbLoad = cb;
                    fullRwInterisnew = false;
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadFullRwInter(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads full rwinter {placement}-{adpl.placement} admobmy loadFullRwInter isloading={adpl.isLoading} or isloaded={adpl.isloaded} showing={adpl.getShowing()}");
                }
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showFullRwInter(string placement, float timeDelay, bool isShow2, AdCallBack cb)
        {
            isFull2 = isShow2;
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFullRwInter(placement, true);
            if (adpl != null)
            {
                //adpl.cbShow = null;
                int ss = getFullRwInterLoaded(adpl.placement);
                if (ss > 0 && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full rwinter {placement} admobmy showFullRwInter timeDelay={timeDelay}");
                    adpl.countLoad = 0;
                    adpl.cbShow = cb;
                    adpl.setSetPlacementShow(placement);
                    if (timeDelay > 0)
                    {
                        adpl.setShowing(true);
                        AdsProcessCB.Instance().Enqueue(() =>
                        {
                            AdsHelper.onAdShowStart(placement, "rewarded_interstitial", "admob", "");
                            bool iss = AdsAdmobMyBridge.Instance.showFullRwInter(adpl.placement);
                            if (!iss)
                            {
                                adpl.setShowing(false);
                                if (cb != null)
                                {
                                    cb(AD_State.AD_SHOW_FAIL);
                                }
                            }
                        }, timeDelay);
                        return true;
                    }
                    else
                    {
                        AdsHelper.onAdShowStart(placement, "rewarded_interstitial", "admob", "");
                        bool iss = AdsAdmobMyBridge.Instance.showFullRwInter(adpl.placement);
                        adpl.setShowing(iss);
                        return iss;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads full rwinter {placement} admobmy showFullRwInter not load or showing={adpl.getShowing()}");
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy showFullRwInter not pl");
            }
#endif
            return false;
        }

        //full Rw Inter
        protected override void tryLoadFullRwRw(AdPlacementFull adpl)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            string idLoad = "";
            adpl.isAdHigh = false;
            if (adpl.adECPM.idxCurrEcpm >= adpl.adECPM.list.Count)
            {
                adpl.adECPM.idxCurrEcpm = 0;
            }
            idLoad = adpl.adECPM.list[adpl.adECPM.idxCurrEcpm].adsId;
            if (adpl.adECPM.idxHighPriority >= adpl.adECPM.idxCurrEcpm)
            {
                adpl.isAdHigh = true;
            }

            int tryload = adpl.countLoad;
            if (tryload >= toTryLoad)
            {
                SdkUtil.logd($"ads full rwrw {adpl.loadPl}-{adpl.placement} admobmy tryLoadFullRwRw over try");
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }

                return;
            }
            if (idLoad != null && idLoad.Contains("ca-app-pub"))
            {
                SdkUtil.logd($"ads full rwrw {adpl.loadPl}-{adpl.placement} admobmy tryLoadFullRwRw id={idLoad} idxCurrEcpmFull={adpl.adECPM.idxCurrEcpm} isFullHigh={adpl.isAdHigh}");
                AdsHelper.onAdLoad(adpl.loadPl, "rewarded", idLoad, "admob");
                adpl.isLoading = true;
                adpl.isloaded = false;
                AdsAdmobMyBridge.Instance.loadFullRwRw(adpl.placement, idLoad);
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {adpl.loadPl}-{adpl.placement} admobmy tryLoadFullRwRw id not correct");
                adpl.isLoading = false;
                adpl.isloaded = false;
            }
#else
            if (adpl != null && adpl.cbLoad != null)
            {
                SdkUtil.logd($"ads full rwrw {adpl.placement} admobmy tryLoadFullRwRw not enable");
                var tmpcb = adpl.cbLoad;
                adpl.cbLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadFullRwRw(string placement, AdCallBack cb)
        {
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFullRwRw(placement, false);
            if (adpl == null)
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy loadFullRwRw not placement");
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            else
            {
                if (!adpl.isloaded && !adpl.isLoading && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full rwrw {placement}-{adpl.placement} admobmy loadFullRwRw");
                    adpl.cbLoad = cb;
                    fullRwRwisnew = false;
                    adpl.countLoad = 0;
                    adpl.adECPM.idxCurrEcpm = 0;
                    adpl.setSetPlacementLoad(placement);
                    tryLoadFullRwRw(adpl);
                }
                else
                {
                    SdkUtil.logd($"ads full rwrw {placement}-{adpl.placement} admobmy loadFullRwRw isloading={adpl.isLoading} or isloaded={adpl.isloaded} showing={adpl.getShowing()}");
                }
            }
#else
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override bool showFullRwRw(string placement, float timeDelay, bool isShow2, AdCallBack cb)
        {
            isFull2 = isShow2;
#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
            AdPlacementFull adpl = getPlFullRwRw(placement, true);
            if (adpl != null)
            {
                //adpl.cbShow = null;
                int ss = getFullRwRwLoaded(adpl.placement);
                if (ss > 0 && !adpl.getShowing())
                {
                    SdkUtil.logd($"ads full rwrw {placement} admobmy showFullRwRw timeDelay={timeDelay}");
                    adpl.countLoad = 0;
                    adpl.cbShow = cb;
                    adpl.setSetPlacementShow(placement);
                    if (timeDelay > 0)
                    {
                        adpl.setShowing(true);
                        AdsProcessCB.Instance().Enqueue(() =>
                        {
                            AdsHelper.onAdShowStart(placement, "rewarded", "admob", "");
                            bool iss = AdsAdmobMyBridge.Instance.showFullRwRw(adpl.placement);
                            if (!iss)
                            {
                                adpl.setShowing(false);
                                if (cb != null)
                                {
                                    cb(AD_State.AD_SHOW_FAIL);
                                }
                            }
                        }, timeDelay);
                        return true;
                    }
                    else
                    {
                        AdsHelper.onAdShowStart(placement, "rewarded", "admob", "");
                        bool iss = AdsAdmobMyBridge.Instance.showFullRwRw(adpl.placement);
                        adpl.setShowing(iss);
                        return iss;
                    }
                }
                else
                {
                    SdkUtil.logd($"ads full rwrw {placement} admobmy showFullRwRw not load or showing={adpl.getShowing()}");
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy showFullRwRw not pl");
            }
#endif
            return false;
        }

#if ENABLE_ADS_ADMOB && USE_ADSMOB_MY
        #region Full Native AD EVENTS
        private void OnNativeFullLoadedEvent(string placement, string adsId, string adnet)
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
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                SdkUtil.logd($"ads full {adpl.loadPl}-{placement} nt admobmy OnNativeFullLoadedEvent plid={adpl.idPl}");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_full", adsId, "admob", adsource, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                nativefullisnew = false;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy OnNativeFullLoadedEvent not pl");
            }
        }
        private void OnNativeFullFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                SdkUtil.logd($"ads full {adpl.loadPl}-{placement} nt admobmy onload fail=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_full", adsId, "admob", "", false);
                adpl.isLoading = false;
                adpl.isloaded = false;
                if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                {
                    if (nativefullisnew)
                    {
                        nativefullisnew = false;
                        adpl.adECPM.idxCurrEcpm = 0;
                    }
                    else
                    {
                        adpl.adECPM.idxCurrEcpm++;
                    }
                    tryLoadNativeFull(adpl);
                }
                else
                {
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        adpl.countLoad++;
                        tryLoadNativeFull(adpl);
                    }, 1.0f);
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy onload fail=" + err);
            }
        }
        private void OnNativeFullDisplayedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} nt admobmy onshow");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy onshow not pl");
            }
        }
        private void OnNativeFullImpresstionEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full {placement} nt admobmy OnNativeFullImpresstionEvent");
        }
        private void onNativeFullFailedToShow(string placement, string adsId, string adnet, string err)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} nt admobmy onNativeFullFailedToShow=" + err);
                adpl.isLoading = false;
                adpl.isloaded = false;
                spl = adpl.showPl;
                adpl.setShowing(false);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy not pl onNativeFullFailedToShow=" + err);
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "native_full", "admob", adsource, adsId, false, err);
            onFullClose(placement);
        }
        private void OnNativeFullClickEvent(string placement, string adsId, string adnet)
        {
            if (!isFullNt2)
            {
                FIRhelper.logEvent("show_ads_full_nt_click");
            }
            else
            {
                FIRhelper.logEvent("show_ads_full2_nt_click");
            }
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "native_full", "admob", adsource, adsId);
        }
        private void OnNativeFullDismissedEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} nt admobmy onclose");
                adpl.isLoading = false;
                adpl.isloaded = false;
                spl = adpl.showPl;
                adpl.setShowing(false);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }

                adpl.countLoad = 0;
                adpl.cbShow = null;
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy onclose not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "native_full", "admob", adsource, adsId, true, "");
            onFullClose(placement);
        }
        private void OnNativeFullFinishShowEvent(string placement, string adsId, string err)
        {
            advhelper.onCloseFullGift(true);
        }
        private void OnNativeFullAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            long originva = valueMicros;
            AdsHelper.Instance.setEcpmNtFull4Fb(originva / 1000);
            if (!isFullNt2)
            {
                FIRhelper.logEvent("show_ads_total_imp");
                FIRhelper.logEvent("show_ads_full_imp");
                FIRhelper.logEvent("show_ads_full_imp_0_nt");
            }
            else
            {
                valueMicros = (long)(valueMicros * FIRhelper.perPostAdsNtFull2);
                Debug.Log($"mysdk: ads full nt {placement} admobmy onpaid v={valueMicros} perpost={FIRhelper.perPostAdsNtFull2}");
            }
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtFull[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(9);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            if (!isFullNt2 && AdsBase.PLFullSplash.CompareTo(spl) == 0)
            {
                valueMicros = (long)(valueMicros * FIRhelper.perPostAdsNtSplash);
                Debug.Log($"mysdk: ads full nt {spl} admobmy onpaid v={valueMicros} perpost={FIRhelper.perPostAdsNtSplash}");
            }
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, originva, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, originva);
        }
        #endregion
        #region Full Native Icon AD EVENTS
        private void OnNativeIcFullLoadedEvent(string placement, string adsId, string adnet)
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
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                SdkUtil.logd($"ads full {adpl.loadPl}-{placement} nt admobmy OnNativeIcFullLoadedEvent plid={adpl.idPl}");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_full", adsId, "admob", adsource, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                nativeicfullisnew = false;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy OnNativeIcFullLoadedEvent not pl");
            }
        }
        private void OnNativeIcFullFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                SdkUtil.logd($"ads full {adpl.loadPl}-{placement} nt admobmy OnNativeIcFullFailedEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "native_full", adsId, "admob", "", false);
                adpl.isLoading = false;
                adpl.isloaded = false;
                if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                {
                    if (nativeicfullisnew)
                    {
                        nativeicfullisnew = false;
                        adpl.adECPM.idxCurrEcpm = 0;
                    }
                    else
                    {
                        adpl.adECPM.idxCurrEcpm++;
                    }
                    tryLoadNativeIcFull(adpl);
                }
                else
                {
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        adpl.countLoad++;
                        tryLoadNativeIcFull(adpl);
                    }, 1.0f);
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy OnNativeIcFullFailedEvent=" + err);
            }
        }
        private void OnNativeIcFullDisplayedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} nt admobmy OnNativeIcFullDisplayedEvent");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy OnNativeIcFullDisplayedEvent not pl");
            }
        }
        private void OnNativeIcFullImpresstionEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full {placement} nt admobmy OnNativeIcFullImpresstionEvent");
        }
        private void onNativeIcFullFailedToShow(string placement, string adsId, string adnet, string err)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} nt admobmy onNativeIcFullFailedToShow=" + err);
                adpl.isLoading = false;
                adpl.isloaded = false;
                spl = adpl.showPl;
                adpl.setShowing(false);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy not pl onNativeIcFullFailedToShow=" + err);
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "native_full", "admob", adsource, adsId, false, err);
            onFullClose(placement);
        }
        private void OnNativeIcFullClickEvent(string placement, string adsId, string adnet)
        {
            if (!isFullNt2)
            {
                FIRhelper.logEvent("show_ads_full_nt_click");
            }
            else
            {
                FIRhelper.logEvent("show_ads_full2_nt_click");
            }
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "native_full", "admob", adsource, adsId);
        }
        private void OnNativeIcFullDismissedEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} nt admobmy OnNativeIcFullDismissedEvent");
                adpl.isLoading = false;
                adpl.isloaded = false;
                spl = adpl.showPl;
                adpl.setShowing(false);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }

                adpl.countLoad = 0;
                adpl.cbShow = null;
            }
            else
            {
                SdkUtil.logd($"ads full {placement} nt admobmy OnNativeIcFullDismissedEvent not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "native_full", "admob", adsource, adsId, true, "");
            onFullClose(placement);
        }
        private void OnNativeIcFullFinishShowEvent(string placement, string adsId, string err)
        {
            advhelper.onCloseFullGift(true);
        }
        private void OnNativeIcFullAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            long originva = valueMicros;
            AdsHelper.Instance.setEcpmNtFull4Fb(originva / 1000);
            if (!isFullNt2)
            {
                FIRhelper.logEvent("show_ads_total_imp");
                FIRhelper.logEvent("show_ads_full_imp");
                FIRhelper.logEvent("show_ads_full_imp_0_nt");
            }
            else
            {
                valueMicros = (long)(valueMicros * FIRhelper.perPostAdsNtFull2);
                Debug.Log($"mysdk: ads full ntic {placement} admobmy onpaid v={valueMicros} perpost={FIRhelper.perPostAdsNtFull2}");
            }
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLNtIcFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLNtIcFull[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(9);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            if (!isFullNt2 && AdsBase.PLFullSplash.CompareTo(spl) == 0)
            {
                valueMicros = (long)(valueMicros * FIRhelper.perPostAdsNtSplash);
                Debug.Log($"mysdk: ads full ntic {spl} admobmy onpaid v={valueMicros} perpost={FIRhelper.perPostAdsNtSplash}");
            }
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, originva, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, originva);
        }
        #endregion
        #region Full AD EVENTS
        private void OnInterstitialLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                SdkUtil.logd($"ads full {adpl.loadPl}-{placement} admobmy OnInterstitialLoadedEvent");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "interstitial", adsId, "admob", adsource, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                fullisnew = false;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} admobmy OnInterstitialLoadedEvent not pl");
            }
        }
        private void OnInterstitialFailedEvent(string placement, string adsId, string err)
        {
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                SdkUtil.logd($"ads full {adpl.loadPl}-{placement} admobmy OnInterstitialFailedEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "interstitial", adsId, "admob", "", false);
                adpl.isLoading = false;
                adpl.isloaded = false;
                if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                {
                    if (fullisnew)
                    {
                        fullisnew = false;
                        adpl.adECPM.idxCurrEcpm = 0;
                    }
                    else
                    {
                        adpl.adECPM.idxCurrEcpm++;
                    }
                    tryLoadFull(adpl);
                }
                else
                {
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        adpl.countLoad++;
                        tryLoadFull(adpl);
                    }, 1.0f);
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} admobmy not pl OnInterstitialFailedEvent=" + err);
            }
        }
        private void OnInterstitialDisplayedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} admobmy OnInterstitialDisplayedEvent");
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} admobmy OnInterstitialDisplayedEvent not pl");
            }
        }
        private void OnInterstitialImpresstionEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full {placement} admobmy OnInterstitialImpresstionEvent");
        }
        private void OnInterstitialClickEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "interstitial", "admob", adsource, adsId);
        }
        private void onInterstitialFailedToShow(string placement, string adsId, string adnet, string err)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} admobmy onInterstitialFailedToShow=" + err);
                adpl.isloaded = false;
                adpl.isLoading = false;
                spl = adpl.showPl;
                adpl.setShowing(false);
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full {placement} admobmy onInterstitialFailedToShow not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "interstitial", "admob", adsource, adsId, false, err);
            onFullClose(placement);
        }
        private void OnInterstitialDismissedEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                SdkUtil.logd($"ads full {adpl.showPl}-{placement} admobmy OnInterstitialDismissedEvent");
                adpl.isloaded = false;
                adpl.isLoading = false;
                adpl.setShowing(false);
                spl = adpl.showPl;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }
                adpl.cbShow = null;
                adpl.countLoad = 0;
            }
            else
            {
                SdkUtil.logd($"ads full {placement} admobmy OnInterstitialDismissedEvent not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "interstitial", "admob", adsource, adsId, true, "");
            onFullClose(placement);
        }
        private void OnInterstitialFinishShowEvent(string placement, string adsId, string err)
        {
            advhelper.onCloseFullGift(true);
        }
        private void OnInterstitialAdPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            if (!isFull2)
            {
                FIRhelper.logEvent("show_ads_total_imp");
                FIRhelper.logEvent("show_ads_full_imp");
                FIRhelper.logEvent("show_ads_full_imp_0");
            }
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFull.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFull[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(4);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
        #region Full REWARDED Inter AD EVENTS
        private void OnInterRwInterLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                SdkUtil.logd($"ads full rwinter {adpl.loadPl}-{placement} admobmy OnInterRwInterLoadedEvent");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "rewarded_interstitial", adsId, "admob", adsource, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                fullRwInterisnew = false;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy OnInterRwInterLoadedEvent not pl");
            }
        }
        private void OnInterRwInterLoadFailEvent(string placement, string adsId, string err)
        {
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                SdkUtil.logd($"ads full rwinter {adpl.loadPl}-{placement} admobmy OnInterRwInterLoadFailEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "rewarded_interstitial", adsId, "admob", "", false);
                adpl.isLoading = false;
                adpl.isloaded = false;
                if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                {
                    if (fullRwInterisnew)
                    {
                        fullRwInterisnew = false;
                        adpl.adECPM.idxCurrEcpm = 0;
                    }
                    else
                    {
                        adpl.adECPM.idxCurrEcpm++;
                    }
                    tryLoadFullRwInter(adpl);
                }
                else
                {
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        adpl.countLoad++;
                        tryLoadFullRwInter(adpl);
                    }, 1.0f);
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy not pl OnInterRwInterLoadFailEvent=" + err);
            }
        }
        private void OnInterRwInterFailedToShowEvent(string placement, string adsId, string adnet, string err)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                SdkUtil.logd($"ads full rwinter {adpl.showPl}-{placement} admobmy OnInterRwInterFailedToShowEvent=" + err);
                adpl.isloaded = false;
                adpl.isLoading = false;
                adpl.setShowing(false);
                spl = adpl.showPl;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy OnInterRwInterFailedToShowEvent dic not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "rewarded_interstitial", "admob", adsource, adsId, false, err);
            onFullClose(placement);
        }
        private void OnInterRwInterShowedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                SdkUtil.logd($"ads full rwinter {adpl.showPl}-{placement} admobmy OnInterRwInterShowedEvent");
                adpl.countLoad = 0;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy OnInterRwInterShowedEvent not pl");
            }
        }
        private void OnInterRwInterImpresstionEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full rwinter {placement} admobmy OnInterRwInterImpresstionEvent");
        }
        private void OnInterRwInterClickEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "rewarded_interstitial", "admob", adsource, adsId);
        }
        private void OnInterRwInterRewardEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full rwinter admobmy OnInterRwInterRewardEvent");
            isFullRewardCom = true;
        }
        private void OnInterRwInterDismissedEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                SdkUtil.logd($"ads full rwinter {adpl.showPl}-{placement} admobmy OnInterRwInterDismissedEvent");
                adpl.isloaded = false;
                adpl.isLoading = false;
                adpl.setShowing(false);
                spl = adpl.showPl;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    if (isFullRewardCom)
                    {
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                    }
                    else
                    {
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                    }
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }
                adpl.cbShow = null;
                adpl.countLoad = 0;
            }
            else
            {
                SdkUtil.logd($"ads full rwinter {placement} admobmy OnInterRwInterDismissedEvent not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "rewarded_interstitial", "admob", adsource, adsId, true, "");
            onFullClose(placement);
            isFullRewardCom = false;
        }
        private void OnInterRwInterFinishShowEvent(string placement, string adsId, string err)
        {
            advhelper.onCloseFullGift(true);
        }
        private void OnInterRwInterPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            FIRhelper.logEvent("show_ads_total_imp");
            FIRhelper.logEvent("show_ads_full_rwin_imp_0");
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwInter.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwInter[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(11);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
        #region Full REWARDED VIDEO AD EVENTS
        private void OnInterRwRwLoadedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                SdkUtil.logd($"ads full rwrw {adpl.loadPl}-{placement} admobmy OnInterRwRwLoadedEvent");
                string adsource = FIRhelper.getAdsourceAdmob(adnet);
                AdsHelper.onAdLoadResult(adpl.loadPl, "rewarded", adsId, "admob", adsource, true);
                adpl.countLoad = 0;
                adpl.isLoading = false;
                adpl.isloaded = true;
                fullRwRwisnew = false;
                if (adpl.cbLoad != null)
                {
                    var tmpcb = adpl.cbLoad;
                    adpl.cbLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy OnInterRwRwLoadedEvent not pl");
            }
        }
        private void OnInterRwRwLoadFailEvent(string placement, string adsId, string err)
        {
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                SdkUtil.logd($"ads full rwrw {adpl.loadPl}-{placement} admobmy OnInterRwRwLoadFailEvent=" + err);
                AdsHelper.onAdLoadResult(adpl.loadPl, "rewarded", adsId, "admob", "", false);
                adpl.isLoading = false;
                adpl.isloaded = false;
                if (adpl.adECPM.idxCurrEcpm < (adpl.adECPM.list.Count - 1))
                {
                    if (fullRwRwisnew)
                    {
                        fullRwRwisnew = false;
                        adpl.adECPM.idxCurrEcpm = 0;
                    }
                    else
                    {
                        adpl.adECPM.idxCurrEcpm++;
                    }
                    tryLoadFullRwRw(adpl);
                }
                else
                {
                    AdsProcessCB.Instance().Enqueue(() =>
                    {
                        adpl.countLoad++;
                        tryLoadFullRwRw(adpl);
                    }, 1.0f);
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy not pl OnInterRwRwLoadFailEvent=" + err);
            }
        }
        private void OnInterRwRwFailedToShowEvent(string placement, string adsId, string adnet, string err)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                SdkUtil.logd($"ads full rwrw {adpl.showPl}-{placement} admobmy OnInterRwRwFailedToShowEvent=" + err);
                adpl.isloaded = false;
                adpl.isLoading = false;
                adpl.setShowing(false);
                spl = adpl.showPl;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy OnInterRwRwFailedToShowEvent dic not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "rewarded", "admob", adsource, adsId, false, err);
            onFullClose(placement);
        }
        private void OnInterRwRwShowedEvent(string placement, string adsId, string adnet)
        {
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                SdkUtil.logd($"ads full rwrw {adpl.showPl}-{placement} admobmy OnInterRwRwShowedEvent");
                adpl.countLoad = 0;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW); });
                }
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy OnInterRwRwShowedEvent not pl");
            }
        }
        private void OnInterRwRwImpresstionEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full rwrw {placement} admobmy OnInterRwRwImpresstionEvent");
        }
        private void OnInterRwRwClickEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdClick(spl, "rewarded", "admob", adsource, adsId);
        }
        private void OnInterRwRwRewardEvent(string placement, string adsId, string adnet)
        {
            SdkUtil.logd($"ads full rwrw admobmy OnInterRwRwRewardEvent");
            isFullRewardCom = true;
        }
        private void OnInterRwRwDismissedEvent(string placement, string adsId, string adnet)
        {
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                SdkUtil.logd($"ads full rwrw {adpl.showPl}-{placement} admobmy OnInterRwRwDismissedEvent");
                adpl.isloaded = false;
                adpl.isLoading = false;
                adpl.setShowing(false);
                spl = adpl.showPl;
                if (adpl.cbShow != null)
                {
                    AdCallBack tmpcb = adpl.cbShow;
                    if (isFullRewardCom)
                    {
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                    }
                    else
                    {
                        AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                    }
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
                }
                adpl.cbShow = null;
                adpl.countLoad = 0;
            }
            else
            {
                SdkUtil.logd($"ads full rwrw {placement} admobmy OnInterRwRwDismissedEvent not pl");
            }
            string adsource = FIRhelper.getAdsourceAdmob(adnet);
            AdsHelper.onAdShowEnd(spl, "rewarded", "admob", adsource, adsId, true, "");
            onFullClose(placement);
            isFullRewardCom = false;
        }
        private void OnInterRwRwFinishShowEvent(string placement, string adsId, string err)
        {
            advhelper.onCloseFullGift(true);
        }
        private void OnInterRwRwPaidEvent(string placement, string adsId, string adNet, int precisionType, string currencyCode, long valueMicros)
        {
            FIRhelper.logEvent("show_ads_total_imp");
            FIRhelper.logEvent("show_ads_full_rwrw_imp_0");
            string spl = SDKManager.Instance.currPlacement;
            if (dicPLFullRwRw.ContainsKey(placement))
            {
                AdPlacementFull adpl = dicPLFullRwRw[placement];
                spl = adpl.showPl;
            }
            if (spl == null || spl.Length <= 1)
            {
                spl = placement;
            }
            string adformat = FIRhelper.getAdformatAdmob(5);
            string adsource = FIRhelper.getAdsourceAdmob(adNet);
            FIRhelper.logEventAdsPaidAdmob(spl, adformat, adsource, adsId, valueMicros, valueMicros, currencyCode);
            float realValue = ((float)valueMicros) / 1000000000.0f;
            AdsHelper.onAdImpresstion(spl, adsId, adformat, "admob", adsource, realValue, valueMicros);
        }
        #endregion
#endif
    }
}