//#define ENABLE_LOG_SERVER_TEST

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MyBox;
using mygame.sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using time;
using UnityEngine;

public class IgnoreEmptyStringResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var prop = base.CreateProperty(member, serialization);

        if (prop.PropertyType == typeof(string))
        {
            prop.ShouldSerialize = instance =>
            {
                var value = prop.ValueProvider.GetValue(instance) as string;
                return !string.IsNullOrEmpty(value);
            };
        }

        if (prop.PropertyType == typeof(double))
        {
            prop.ShouldSerialize = instance =>
            {
                var value = prop.ValueProvider.GetValue(instance);
                return !value.Equals(-1.0);
            };
        }

        return prop;
    }
}

public class LogEventCustom : Singleton<LogEventCustom>
{
    private static JsonSerializerSettings jsonSetting = new JsonSerializerSettings
        { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

    private string logCachePref
    {
        get { return PlayerPrefs.GetString("logs_cache", ""); }
        set { PlayerPrefs.SetString("logs_cache", value); }
    }

    private static int _logSequenceID
    {
        get => PlayerPrefs.GetInt("logsequenceid", 1);
        set => PlayerPrefs.SetInt("logsequenceid", value);
    }

    private int logSequenceID
    {
        get
        {
            int id = _logSequenceID;
            _logSequenceID++;
            if (_logSequenceID >= int.MaxValue - 1)
            {
                _logSequenceID = 1;
            }

            return id;
        }
    }

    private bool isDoneSplashLoading = false;


    protected void Awake()
    {
        isDoneSplashLoading = false;
        DontDestroyOnLoad(gameObject);

        SDKManager.CBFinishShowLanAge += (val) => { isDoneSplashLoading = true; };
    }

    //================================Custom Stuffs=====================================


    public static void LogResource(string reason, string position, params DataResource[] items)
    {
        if (items == null || items.Length == 0) return;

        ExtractLogInfo(items, out var resource_type, out var resource_name, out var resource_amount);
        var eventName = items[0].amount < 0 ? "resource_sink" : "resource_earn";

        var logParams = new Dictionary<string, object>
        {
            { "resource_type", resource_type },
            { "resource_name", resource_name },
            { "resource_amount", resource_amount },
            { "reason", reason },
            { "position", position },
        };
        LogEventManager.Instance.LogEvent(eventName, logParams);
    }

    public static void LogResource(string reason, string position, params PackageData.ItemBuyInfo[] items)
    {
        if (items == null || items.Length == 0) return;

        ExtractLogInfo(items, out var resource_type, out var resource_name, out var resource_amount);
        var eventName = items[0].itemAmount < 0 ? "resource_sink" : "resource_earn";

        var logParams = new Dictionary<string, object>
        {
            { "resource_type", resource_type },
            { "resource_name", resource_name },
            { "resource_amount", resource_amount },
            { "reason", reason },
            { "position", position },
        };
        LogEventManager.Instance.LogEvent(eventName, logParams);
    }

    public static void LogResource(string reason, string position, params ItemInfo[] items)
    {
        if (items == null || items.Length == 0) return;

        ExtractLogInfo(items, out var resource_type, out var resource_name, out var resource_amount);
        var eventName = items[0].itemAmount > 0 ? "resource_earn" : "resource_sink";

        var logParams = new Dictionary<string, object>
        {
            { "resource_type", resource_type },
            { "resource_name", resource_name.ToLowerInvariant() },
            { "resource_amount", resource_amount },
            { "reason", reason },
            { "position", position },
        };
        LogEventManager.Instance.LogEvent(eventName, logParams);
    }

    private static void ExtractLogInfo(DataResource[] reward, out string resource_type, out string resource_name,
        out string resource_amount)
    {
        resource_type = string.Join(",", reward.Select(r => $"[{GetResourceType(r.resType)}]"));
        resource_name = string.Join(",", reward.Select(r => $"[{r.resType}]"));
        resource_amount = string.Join(",", reward.Select(r => $"[{Mathf.Abs(r.amount)}]"));
    }

    private static void ExtractLogInfo(ItemInfo[] reward, out string resource_type, out string resource_name,
        out string resource_amount)
    {
        resource_type = string.Join(",", reward.Select(r => $"[{GetResourceType(r.itemType)}]"));
        resource_name = string.Join(",", reward.Select(r => $"[{r.itemType}]"));
        resource_amount = string.Join(",", reward.Select(r => $"[{Mathf.Abs(r.itemAmount)}]"));
    }

    private static void ExtractLogInfo(PackageData.ItemBuyInfo[] reward, out string resource_type,
        out string resource_name,
        out string resource_amount)
    {
        resource_type = string.Join(",", reward.Select(r => $"[{GetResourceType(r.itemType)}]"));
        resource_name = string.Join(",", reward.Select(r => $"[{r.itemType}]"));
        resource_amount = string.Join(",", reward.Select(r => $"[{Mathf.Abs(r.itemAmount)}]"));
    }

    public void LogResourceSink(string reason, string where, int level = 0, params PackageData.ItemBuyInfo[] items)
    {
        LogResource(reason, where, items);
    }

    public void LogResourceSink(string reason, string where, int level = 0, params ItemInfo[] items)
    {
        LogResource(reason, where, items);
    }

    public void LogResourceEarn(string reason, string where, int level = 0, params ItemInfo[] items)
    {
        LogResource(reason, where, items);
    }

    public void LogResourceEarn(string reason, string where, int level = 0, params PackageData.ItemBuyInfo[] items)
    {
        LogResource(reason, where, items);
    }

    public void LogBuyInapp(string pkg, string where, string show_type, string show_action, decimal price,
        int level = 0, params DataResource[] items)
    {
        LogEventManager.Instance.LogEvent("iap_purchase", new Dictionary<string, object>
        {
            { "pack_name", pkg },
            { "price", price },
            { "currency", InappHelper.Instance.CurrencyCode },
            { "position", where },
            { "show_type", show_type },
            { "show_action", show_action },
            { "level_show", level },
        });
    }

    public void LogShowInapp(string where, string show_type, string show_action, string pack_name, int level = 0)
    {
        LogEventManager.Instance.LogEvent("iap_show", new Dictionary<string, object>
        {
            { "pack_name", pack_name },
            { "position", where },
            { "show_type", show_type },
            { "show_action", show_action },
            { "level_show", level },
        });
    }

    public void LogInappClick(string pkg, string show_type, string where, string show_action, int level = 0)
    {
        LogEventManager.Instance.LogEvent("iap_click", new Dictionary<string, object>
        {
            { "pack_name", pkg },
            { "position", where },
            { "show_type", show_type },
            { "show_action", show_action },
            { "level_show", level },
        });
    }

    public void FirstOpen()
    {
        LogEventManager.Instance.LogEvent($"{ELogEventName.first_open}");
    }

    public void LoadingStart(string placement)
    {
        LogEventManager.Instance.LogEvent("loading_start", new()
        {
            { "placement", placement }
        });
    }

    public void LoadingFinish(string placement, long duration, bool success)
    {
        LogEventManager.Instance.LogEvent("loading_finish", new Dictionary<string, object>
        {
            { "placement", placement },
            { "load_time", duration },
            { "is_load", success ? 1 : 0 },
        });
    }

    public void LogLevel(ELogEventName eventName, int level, int playIndex,
        int playTime = 0, int requireTime = 0, int totalMove = 0, int bonusTime = 0,
        int loseIndex = 0, int levelProgress = 0, string result = null, string playType = null)
    {
        // ✅ Khởi tạo dictionary log mặc định
        var log = new Dictionary<string, object>
        {
            { "level", level },
            { "play_index", playIndex },
            { "play_type", playType },
            { "play_time", playTime },
            { "require_time", requireTime },
            { "total_move", totalMove },
            { "bonus_time", bonusTime },
            { "lose_index", loseIndex },
            { "level_progress", levelProgress },
        };

        // ✅ Xử lý thêm dữ liệu tùy loại event
        switch (eventName)
        {
            case ELogEventName.level_end:
                AddIfMissing("result", result);
                break;

            case ELogEventName.level_second_chance:
                AddIfMissing("result", result);
                AddIfMissing("bonus_type", "time");
                break;

            // level_play, hoặc các event khác không cần thêm gì
        }

        // ✅ Gửi log
        LogEventManager.Instance.LogEvent(eventName.ToString(), log);
        return;

        void AddIfMissing(string key, object value)
        {
            log.TryAdd(key, value);
        }
    }


    public void LogDownloadLevel(ELogEventName eventName, string key, string hash, int internetStatus, int status)
    {
        LogEventManager.Instance.LogEvent(eventName.ToString(), new Dictionary<string, object>
        {
            { "key", key },
            { "hash", hash },
            { "internetStatus", internetStatus },
            { "status", status },
        });
    }


    public void LogPlayerAction(string action, int level = 0, params DataResource[] items)
    {
        LogEventManager.Instance.LogEvent("player_action", new Dictionary<string, object>
        {
            { "level", level },
            { "name", action },
        });
    }

    public static LogEvent.ResourceType GetResourceType(RES_type itemType)
    {
        return mapResourceType.GetValueOrDefault(itemType, LogEvent.ResourceType.currency);
    }

    private static Dictionary<RES_type, LogEvent.ResourceType> mapResourceType = new()
    {
        { RES_type.TICKET, LogEvent.ResourceType.item },
        { RES_type.BOOSTER_1, LogEvent.ResourceType.booster },
        { RES_type.BOOSTER_2, LogEvent.ResourceType.booster },
        { RES_type.BOOSTER_3, LogEvent.ResourceType.booster },
        { RES_type.HEART, LogEvent.ResourceType.heart },
    };

    public static string GetConectionType()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return "offline";
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return "mobile_data";
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return "wifi";
        }

        return "unknown";
    }
}

public enum ELogEventName
{
    level_play,
    level_end,
    level_success,
    level_fail,
    level_second_chance,
    level_tutorial,
    level_exit,
    level_retry,
    level_action,
    purchase_action,
    iap_show,
    iap_click,
    iap_purchase,
    resource_earn,
    resource_sink,
    player_action,
    first_open,
    tutorial_step,
    download_bundle,
    level_resume,
}

public class IAPShowType
{
    public const string SHOP = "shop";
    public const string PACK = "pack";
}

public class IAPPackageName
{
    public const string PACK1 = "removeads3day_sale";
    public const string PACK2 = "removeads_sale";
    public const string PACK3 = "removeads_super_sale";
}

public enum ResourceType
{
    Currency,
    Product
}

public static class FloatExtensions
{
    public static string ToInvariantString(this float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}