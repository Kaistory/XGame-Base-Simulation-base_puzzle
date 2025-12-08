using System;
using System.Collections.Generic;
using System.Linq;
using DevUlts;
using MyBox;
using mygame.sdk;
using Newtonsoft.Json;
using UnityEngine;


[DefaultExecutionOrder(-1000)]
public class DataManager : Singleton<DataManager>
{
    public static DataManager Instance { get; private set; }

    [SerializeField] private DataResourcesSO resourcesSO;

    public ItemInfo[] allGift;

    public List<InfoResources> DataResourcesConfig { get; set; }

    public InfoResources GetInfoResources(RES_type res_Type)
    {
        var listData = DataResourcesConfig ?? resourcesSO.InfoResources.ToList();
        for (int i = 0; i < listData.Count; i++)
        {
            if (listData[i].res_type == res_Type)
            {
                return listData[i];
            }
        }

        return null;
    }

    public static int ConsecutiveLose
    {
        get => PlayerPrefs.GetInt("consecutive_lose", 0);
        set => PlayerPrefs.SetInt("consecutive_lose", value);
    }

    public static int ConsecutivePlay
    {
        get => PlayerPrefs.GetInt("consecutive_play", 0);
        set => PlayerPrefs.SetInt("consecutive_play", value);
    }

    public static int ConsecutiveWin
    {
        get => PlayerPrefs.GetInt("consecutive_win", 0);
        set => PlayerPrefs.SetInt("consecutive_win", value);
    }

    public static int Gold
    {
        get => GameRes.getRes(RES_type.GOLD, 0);
        set => GameRes.AddRes(RES_type.GOLD, value);
    }

    public static int Level => GameRes.GetLevel();

    public static bool IsFirstPlayGame
    {
        get => PlayerPrefs.GetInt("first_play_game", 0) == 1;
        set => PlayerPrefs.SetInt("first_play_game", value ? 1 : 0);
    }

    public static void SetLevel(int level)
    {
        if (level < 1)
        {
            level = 1;
        }

        var totalLevels = LevelRemoteManager.Instance.levelConfig?.GetLevelInfos()?.Length ?? 0;
        totalLevels = totalLevels == 0 ? 250 : totalLevels;
        if (level > totalLevels)
        {
            level = totalLevels;
        }

        GameRes.SetLevel(Level_type.Common, level);
        GameRes.SetLevel(Level_type.Normal, level);
    }

    public int GetIdxLevel(int idLevel)
    {
        return PlayerPrefs.GetInt($"idx_level_{idLevel}", 1);
    }

    public void AddIdxLevel(int idLevel)
    {
        PlayerPrefs.SetInt($"idx_level_{idLevel}", GetIdxLevel(idLevel) + 1);
    }

    #region Settings

    public static bool GetSetting(eSettingElement settingElement)
    {
        switch (settingElement)
        {
            case eSettingElement.Music:
                return PlayerPrefsUtil.IsMusicEnable;
            case eSettingElement.Sound:
                return PlayerPrefsUtil.IsSoundEnable;
            case eSettingElement.Vibration:
                return PlayerPrefs.GetInt(GameHelper.KeyConfigVibrate, 1) == 1;
            default:
                return PlayerPrefsUtil.IsMusicEnable;
        }
    }

    public static void ChangeSetting(eSettingElement settingElement, bool value)
    {
        switch (settingElement)
        {
            case eSettingElement.Music:
                PlayerPrefsUtil.IsMusicEnable = value;

                if (PlayerPrefsUtil.IsMusicEnable)
                {
                    AudioManager.Instance.ResumeMusic();
                    AudioManager.Instance.PlayPlaylist();
                    //AudioManager.Instance.SetMusicVolume(0.2f);
                }
                else
                {
                    AudioManager.Instance.StopMusic(false);
                }

                break;
            case eSettingElement.Sound:
                PlayerPrefsUtil.IsSoundEnable = value;
                break;
            case eSettingElement.Vibration:
                GameHelper.ChangeSettingVibrate(value);
                break;
            default:
                PlayerPrefsUtil.IsMusicEnable = value;
                break;
        }
    }

    #endregion

    #region Resources Get, Earn, Sink

    public static int GetResources(RES_type type, int defaultValue = 0)
    {
        return GameRes.getRes(type, defaultValue);
    }

    public static bool IsEnoughCurrency(RES_type type, int valueCompare)
    {
        return GetResources(type) >= valueCompare;
    }

    public bool OnEarnResources(DataResource[] dataResources, LogEvent.ReasonItem reason, string where, int level,
        Action onSuccess = null, bool isLog = true)
    {
        bool success = false;
        ItemInfo[] itemInfos = new ItemInfo[dataResources.Length];
        for (int i = 0; i < dataResources.Length; i++)
        {
            success = GameRes.AddRes(dataResources[i].resType, dataResources[i].amount, where);
            // LogError($"OnEarnResources Index:{i} --- {dataResources[i].resType.ToString()} || {dataResources[i].amount}");
            itemInfos[i] = new ItemInfo();
            itemInfos[i].CopyFromDataResource(dataResources[i]);
        }

        if (success)
        {
            onSuccess?.Invoke();
            TigerForge.EventManager.EmitEvent(EventName.UpdateResources);
        }

        if (isLog)
        {
            LogEvent.ResourceEarn(where, itemInfos, reason, level);
        }

        return success;
    }

    public bool OnEarnResources(RES_type type, int amount, LogEvent.ReasonItem reason, string where, int level,
        Action onSuccess = null, bool isLog = true)
    {
        DataResource[] dataResources = new DataResource[1] { new DataResource(type, amount) };
        return OnEarnResources(dataResources, reason, where, level, onSuccess, isLog);
    }


    public bool OnSinkResources(DataResource[] dataResources, LogEvent.ReasonItem reason, string where, int level,
        Action onSuccess = null, bool isLog = true)
    {
        bool success = false;
        for (int i = 0; i < dataResources.Length; i++)
        {
            var data = dataResources[i];
            success = GameRes.AddRes(data.resType, data.amount, where);
            // LogError($"OnSinkResources Index:{i} --- {data.resType.ToString()} || {value}");
        }

        ItemInfo[] itemInfos = new ItemInfo[dataResources.Length];
        for (int i = 0; i < dataResources.Length; i++)
        {
            itemInfos[i] = new ItemInfo();
            itemInfos[i].CopyFromDataResource(dataResources[i]);
        }

        if (success)
        {
            onSuccess?.Invoke();
            TigerForge.EventManager.EmitEvent(EventName.UpdateResources);
        }

        if (isLog)
        {
            LogEvent.ResourceSink(where, itemInfos, reason, level);
        }

        return success;
    }

    public bool OnSinkResources(RES_type type, int amount, LogEvent.ReasonItem reason, string where, int level,
        Action onSuccess = null, Action onFail = null, bool isLog = true)
    {
        var enoughCurrency = IsEnoughCurrency(type, Mathf.Abs(amount));
        if (enoughCurrency)
        {
            DataResource[] dataResources = new DataResource[1] { new DataResource(type, amount) };
            bool success = OnSinkResources(dataResources, reason, where, level, onSuccess, isLog);
            IResourceTarget.UpdateAmount(type); // DangVQ fix tạm nếu chưa có hiệu ứng trừ
            return success;
        }

        UIManager.Instance.ShowNotifyNotEnoughResource(type);
        onFail?.Invoke();

        return false;
    }

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Initialize();
    }

    public void Initialize()
    {
        LoadAllData();
    }

    public void LoadAllData()
    {
    }

    public void SetConfig()
    {
        var data = JsonConvert.DeserializeObject<List<DataResourcesSO.InfoResourcesRemote>>(RemoteConfigure
            .CFPriceBuyBoosterString);

        if (data != null)
        {
            DataResourcesConfig = new List<InfoResources>();
            for (int i = 0; i < resourcesSO.InfoResources.Count; i++)
            {
                var item = resourcesSO.InfoResources[i];
                if (item != null)
                {
                    InfoResources dataGet = new InfoResources()
                    {
                        res_type = item.res_type,
                        des = item.des,
                        icon = item.icon,
                        price = item.price,
                        name = item.name,
                    };
                    var remote = data.FirstOrDefault(x => x.res_type == dataGet.res_type);
                    {
                        if (remote != null)
                        {
                            dataGet.price = remote.price;
                            dataGet.lvUnlock = remote.lvUnlock;
                        }
                    }
                    DataResourcesConfig.Add(dataGet);
                }
            }
        }
    }

    public Sprite GetIcon(RES_type rewardType)
    {
        var item = allGift.SingleOrDefault(x => x.itemType == rewardType);
        return item?.Icon;
    }

    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = true;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    private static readonly string LogRegion = $"{nameof(DataManager)}";

    private static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    private static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    private static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion
}