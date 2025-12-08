using System.Collections.Generic;
using mygame.sdk;
using UnityEngine;

public class LogEvent
{
    #region Log Resources

    public static void ResourceEarn(string where, ItemInfo[] reward, ReasonItem reason, int level)
    {
        LogEventCustom.Instance.LogResourceEarn(reason.ToString(), where?.ToLower(), level, reward);
    }

    public static void ResourceEarn(string where, PackageData.ItemBuyInfo[] reward, ReasonItem reason, int level)
    {
        LogEventCustom.Instance.LogResourceEarn(reason.ToString(), where?.ToLower(), level, reward);
    }

    public static void ResourceSink(string where, ItemInfo[] reward, ReasonItem reason, int level)
    {
        LogEventCustom.Instance.LogResourceSink(reason.ToString(), where?.ToLower(), level, reward);
    }

    public static void ResourceSink(string where, PackageData.ItemBuyInfo[] reward, ReasonItem reason, int level)
    {
        LogEventCustom.Instance.LogResourceSink(reason.ToString(), where?.ToLower(), level, reward);
    }

    #endregion

    // public static void FIRLevelResume(int lv, int timePlay)
    // {
    //     string eventName = $"level_resume_{lv}";
    //     Dictionary<string, object> dicParams = new Dictionary<string, object>();
    //     dicParams["lv"] = lv;
    //     dicParams["timePlay"] = timePlay;
    //     FIRhelper.logEvent(eventName, dicParams);
    // }

    // public static void FIRLevelExit(int lv, int timePlay)
    // {
    //     string eventName = $"level_exit_{lv}";
    //     Dictionary<string, object> dicParams = new Dictionary<string, object>();
    //     dicParams["lv"] = lv;
    //     dicParams["timePlay"] = timePlay;
    //     FIRhelper.logEvent(eventName, dicParams);
    // }
    //
    public static void LevelWin(int lv, int timePlay)
    {
        string eventName = $"level_win";
        Dictionary<string, object> dicParams = new Dictionary<string, object>();
        dicParams["lv"] = lv;
        dicParams["timePlay"] = timePlay;
        FIRhelper.logEvent(eventName, dicParams);
    }

    public static void LevelLose(int lv)
    {
        string eventName = $"level_lose_{lv}";
        Dictionary<string, object> dicParams = new Dictionary<string, object>();
        dicParams["lv"] = lv;
        FIRhelper.logEvent(eventName, dicParams);
    }

    public static void LogEventParam(string evt, object param = null)
    {
        string eventName = SafeFormat(evt, param);
        FIRhelper.logEvent(eventName);
    }

    private static string SafeFormat(string evt, object param)
    {
        if (string.IsNullOrEmpty(evt))
            return "invalid_event";

        if (param == null)
            return evt;

        try
        {
            return string.Format(evt, param);
        }
        catch (System.FormatException)
        {
            Debug.LogWarning($"[LogEvent] Format mismatch in event: {evt}, param: {param}");
            return evt + "_" + param;
        }
    }

    #region LOG LEVEL

    public static void DownloadBundle(string key, string hash, int internetStatus, int status)
    {
        LogEventCustom.Instance.LogDownloadLevel(ELogEventName.download_bundle, key, hash, internetStatus, status);
    }

    public static void LevelPlay(int lv, string playType, int playIndex, int requireTime, int loseIndex)
    {
        LogEventCustom.Instance.LogLevel(eventName: ELogEventName.level_play, level: lv, playIndex: playIndex,
            playTime: 0, requireTime: requireTime, totalMove: 0, bonusTime: 0, loseIndex: loseIndex,
            levelProgress: 0, result: "",
            playType: playType?.ToLower());
    }

    public static void LevelEnd(int lv, int playTime, int playIndex, LevelResult result, int requireTime, int totalMove,
        int bonusTime, int loseIndex, int levelProgress, string playType = null)
    {
        LogEventCustom.Instance.LogLevel(eventName: ELogEventName.level_end, level: lv, playIndex: playIndex,
            playTime: playTime, requireTime: requireTime, totalMove: totalMove, bonusTime: bonusTime,
            loseIndex: loseIndex, levelProgress: levelProgress, result: result.ToString(),
            playType: playType?.ToLower());
    }

    public static void LevelSecondChance(int lv, int playTime, int playIndex, int requireTime, int totalMove,
        int bonusTime, int loseIndex, int levelProgress, string playType = null)
    {
        LogEventCustom.Instance.LogLevel(eventName: ELogEventName.level_second_chance, level: lv, playIndex: playIndex,
            playTime: playTime, requireTime: requireTime, totalMove: totalMove, bonusTime: bonusTime,
            loseIndex: loseIndex, levelProgress: levelProgress, result: "", playType: playType?.ToLower());
    }

    public static void LevelTutorial(int lv, int playTime, int playIndex, int requireTime, int totalMove, int bonusTime,
        int loseIndex, int levelProgress, string playType = null)
    {
        LogEventCustom.Instance.LogLevel(eventName: ELogEventName.level_tutorial, level: lv, playIndex: playIndex,
            playTime: playTime, requireTime: requireTime, totalMove: totalMove, bonusTime: bonusTime,
            loseIndex: loseIndex, levelProgress: levelProgress, result: "", playType: playType?.ToLower());
    }

    // public static void LevelExit(int lv, int playTime, int playIndex, LevelResult result = LevelResult.exit,
    //     string playType = null)
    // {
    //     LogEventCustom.Instance.LogLevel(ELogEventName.level_exit, lv, playIndex, playTime, result.ToString(), playType);
    // }
    //
    // public static void LevelResume(int lv, int playTime, int playIndex, LevelResult result = LevelResult.resume,
    //     string playType = null)
    // {
    //     LogEventCustom.Instance.LogLevel(ELogEventName.level_resume, lv, playIndex, playTime, result.ToString(),
    //         playType);
    // }

    #endregion

    #region LOG IAP

    public static void IAPShow(IAP_ShowType showType, IAP_ShowPosition showPosition, IAP_ShowAction showAction,
        string packName)
    {
        LogEventCustom.Instance.LogShowInapp(where: showPosition.ToString(), show_type: showType.ToString(),
            show_action: showAction.ToString(),
            packName, level: DataManager.Level);
    }

    public static void IAPClick(IAP_ShowType showType, IAP_ShowPosition showPosition, IAP_ShowAction showAction,
        string packName)
    {
        LogEventCustom.Instance.LogInappClick(packName, showType.ToString(), showPosition.ToString(),
            showAction.ToString(), DataManager.Level);
    }

    public static void IAPBuy(IAP_ShowType showType, IAP_ShowPosition showPosition, IAP_ShowAction showAction,
        string packName)
    {
        LogEventCustom.Instance.LogBuyInapp(packName, showType.ToString(), showPosition.ToString(),
            showAction.ToString(), InappHelper.Instance.getDecimalPrice(packName), DataManager.Level);
    }

    #endregion

    public static void PlayerAction(string action)
    {
        LogEventCustom.Instance.LogPlayerAction(action, DataManager.Level);
    }

    public static void FirstOpen()
    {
        LogEventCustom.Instance.FirstOpen();
    }

    public static void LoadingStart(string placement)
    {
        LogEventCustom.Instance.LoadingStart(placement);
    }

    public static void LoadingFinish(string placement, long duration, bool success)
    {
        LogEventCustom.Instance.LoadingFinish(placement, duration, success);
    }

    public enum IAP_ShowType
    {
        shop,
        pack
    }

    public enum IAP_ShowAction
    {
        click_button,
        auto_show,
    }

    public enum IAP_ShowPosition
    {
        shop_popup,
        home_shop,
        pack,
        home_popup,
    }

    public enum ReasonItem
    {
        use,
        purchase,
        exchange,
        reward,
        watch_ads,
    }

    public enum ResourceType
    {
        currency,
        item,
        booster,
        heart,
        unlimited,
    }

    public enum LevelResult
    {
        win,
        lose,
        resume,
        retry,
        exit,
        time_out
    }

    public enum EventPlacement
    {
        home,
        home_icon,
        home_popup,
        end_level_icon,
    }

    public enum EventCloseReason
    {
        complete,
        out_of_time,
        lose,
        close,
    }
}