using System;
using System.Collections;
using System.Collections.Generic;
using MyJson;
using UnityEngine;
using System.Linq;
using _JigblockPuzzle;
using Newtonsoft.Json;
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
using Firebase.RemoteConfig;
#endif

namespace mygame.sdk
{
    public class FIRParserOtherConfig
    {
        public static Action OnReceiveDataDone;

        public static void parserInGameConfig() //
        {
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_enable_banner");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFEnableBanner = (int)v.LongValue == 1;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_show_ads_break");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFShowAdsBreak = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_time_wait_banner_second");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CfTimeWaitBanner = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_show_shop_when_not_enough_resource");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFShowShopWhenNotEnoughResource = (int)v.LongValue == 1;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_count_level_show_main_menu");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFCountLevelShowMainMenu = (int)v.LongValue;
            } 

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_time_buy_bonus");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFTimeBuyBonus = (int)v.LongValue;
            } 

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_time_freeze_time");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFTimeFreezeTime = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_level_config");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFLevelConfig = v.StringValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_try_download_level_count");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFTryDownloadLevelCount = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_use_local_if_no_network");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFUseLocalIfDisableNetwork = (int)v.LongValue == 1;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_preload_level_count");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFPreloadLevelCount = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_version_catalog");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFVersionCatalog = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_clear_old_level_cache");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFClearOldLevelCache = (int)v.LongValue == 1;
            }
            
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_max_download_label");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                LevelRemoteManager.CFMaxDownloadLabel = (int)v.LongValue;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_enable_cheat_game");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFEnableCheatGame = (int)v.LongValue == 1;
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_price_buy_booster");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                RemoteConfigure.CFPriceBuyBoosterString = v.StringValue;
            }

            OnReceiveDataDone?.Invoke();
#endif
            DataManager.Instance.SetConfig();
        }
    }
}