using System;
using System.Collections.Generic;
using System.Linq;
using MyGame.Data;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Data/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    #region Consts & Prefs

    public const int CountLevelRandom = 10;

    // Cache random cho level thường
    public static string CacheLevelIndex
    {
        get => PlayerPrefs.GetString("s_cache_level_index", "");
        set => PlayerPrefs.SetString("s_cache_level_index", value);
    }

    public static string LastCacheLevelIndex
    {
        get => PlayerPrefs.GetString("last_cache_level_index", "");
        set => PlayerPrefs.SetString("last_cache_level_index", value);
    }

    #endregion

    #region Data models

    [System.Serializable]
    public class LevelConfigData
    {
        public int version;
        public LevelInfo[] levelInfos;
        public string levelMonetizations;
    }

    [System.Serializable]
    public enum PuzzleGridType
    {
        Grid2x2 = 1,
        Grid3x3 = 2,
        Grid4x4 = 3,
        Grid5x5 = 4,
        Grid6x6 = 5,
        Grid7x7 = 6,
        Grid8x8 = 7
    }
    
    [System.Serializable]
    public class TrunkData
    {
        // Vị trí XYZ
        [JsonProperty("posT")] public Vector3 position;
        
        // 3 lớp màu (Lưu ID của màu, ví dụ: 0=Red, 1=Blue...)
        // Dùng mảng cố định hoặc List. Ở đây mình dùng mảng int[3]
        [JsonProperty("cols")] public AllColor[] colorLayers = new AllColor[3]; 

        // Các tính năng trạng thái
        [JsonProperty("hslck")] public bool hasLock;       // Khóa
        [JsonProperty("chn")] public bool isChained;      // Xích (Chain)
        [JsonProperty("ice")] public bool isFrozen;       // Băng (Ice)
        [JsonProperty("block")] public bool isBlock;       // Băng (Block)
        
        // Block số lớp hiển thị (Ví dụ: chỉ hiện 1 lớp, 2 lớp hay cả 3)
        [JsonProperty("vlc")] public int visibleLayerCount; 
        [JsonProperty("ant")] public int amountUpTrunk; 
    }

    [System.Serializable]
    public class CutterMachineData
    {
        [JsonProperty("posCM")] public Vector3 position;
        [JsonProperty("rotateCM")] public int rot;
    }

    [Serializable]
    public class LevelInfo
    {
#if UNITY_EDITOR
        [JsonIgnore, HideInInspector] public string name;
#endif
        [JsonProperty("lv")] public ushort level; // số thứ tự hiển thị
        [JsonProperty("id")] public ushort levelID; // id file Level_{levelID}.png
        //[JsonProperty("gt")] public PuzzleGridType gridType;
        //[JsonProperty("t")] public int levelTime;
        //[JsonProperty("bg")] public int backgroundID;
        [JsonProperty("map")] public int mapID;
        [JsonProperty("lt")] public LevelType levelType;
        [JsonProperty("r")] public ushort levelRewardValue;

        //Trunk
        [JsonProperty("trunks")] 
        public TrunkData[] trunks;
        //Cutter Machine
        [JsonProperty("cutterMachine")] 
        public CutterMachineData[] cutterMachine;
        
        //spawn color cutter
        [JsonProperty("spawnCutter")] public List<AllColor> allColorsSpawnCutter;
        
        //Glass 
        
        // Cache trạng thái hợp lệ
        [JsonIgnore] public bool? iconValid;
        [JsonIgnore] public bool? iconValidLocal;
        [JsonIgnore] public bool? isValid;
        [JsonIgnore] public bool? isValidLocal;

        [JsonIgnore]
        public int PlayIndexLevel
        {
            get => PlayerPrefs.GetInt($"play_index_level_{level}", 0);
            set => PlayerPrefs.SetInt($"play_index_level_{level}", value);
        }

        [JsonIgnore]
        public int LoseIndexLevel
        {
            get => PlayerPrefs.GetInt($"lose_index_level_{level}", 0);
            set => PlayerPrefs.SetInt($"lose_index_level_{level}", value);
        }

        public bool IsValid()
        {
            return true;
        }

        public bool IsIconValid()
        {
            return true
                ? iconValidLocal == true
                : iconValid == true;
        }

        public void ClearIsValidCache()
        {
            isValid = null;
            isValidLocal = null;
            iconValid = null;
            iconValidLocal = null;
        }
        
    }

    #endregion

    #region Serialized fields

    [Header("Level Normal")] public LevelInfo[] levelInfos;

    #endregion

    #region Runtime caches / indices

    // runtime override
    public LevelInfo[] levelInfoConfig { get; private set; }

    // tránh random trùng
    private readonly List<ushort> _randomIgnore = new();

    // index nhanh Normal
    private Dictionary<int, LevelInfo> _byLevel;
    private Dictionary<int, LevelInfo> _byLevelId;

    #endregion

    #region Source helpers

    private LevelInfo[] SourceNormal => (levelInfoConfig != null && levelInfoConfig.Length > 0)
        ? levelInfoConfig
        : levelInfos;

    private void EnsureIndexesNormal()
    {
        var data = SourceNormal;
        if (data == null) return;

        if (_byLevel == null || _byLevel.Count != data.Length)
        {
            _byLevel = new(data.Length);
            _byLevelId = new(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                var lf = data[i];
                if (lf == null) continue;
                _byLevel[lf.level] = lf;
                _byLevelId[lf.levelID] = lf;
            }
        }
    }

    #endregion

    #region Public APIs (Normal)

#if UNITY_EDITOR
    public void OnValidate()
    {
        ValidateData();
    }

    public void ValidateData()
    {
        if (levelInfos != null)
        {
            for (int i = 0; i < levelInfos.Length; i++)
            {
                if (levelInfos[i] != null)
                {
                    levelInfos[i].name =
                        $"Level {levelInfos[i].level} - {levelInfos[i].levelType.ToString().ToUpper()}";
                }
            }
        }
    }
#endif

    public void SetLevelInfos(LevelInfo[] data)
    {
        levelInfoConfig = data;
        _byLevel = null;
        _byLevelId = null;
    }

    public LevelInfo[] GetLevelInfos() => SourceNormal;

    public void UpdateValidStatus()
    {
        var nor = GetLevelInfos();
        if (nor != null)
        {
            foreach (var i in nor)
            {
                i?.ClearIsValidCache();
            }
        }
    }

    public LevelInfo GetLevelInfo(int level, bool isRandom = false)
    {
        EnsureIndexesNormal();

        LevelInfo lf = null;
        _byLevel?.TryGetValue(level, out lf);

        if (!isRandom) return lf;

        if (lf == null || !lf.IsValid() || !lf.IsIconValid())
        {
            DebugLogHelper.LogWarning($"[LevelConfig] Not exists or invalid level {level} → using random fallback.");
            return RandomLevel(SourceNormal, level);
        }

        return lf;
    }

    public LevelInfo GetLevelInfo(int level, out bool isLocal)
    {
        EnsureIndexesNormal();

        LevelInfo lf = null;
        _byLevel?.TryGetValue(level, out lf);

        if (lf != null)
        {
            isLocal = lf.isValidLocal == true;
            return lf;
        }

        if (lf == null || !lf.IsValid() || !lf.IsIconValid())
        {
            DebugLogHelper.LogWarning($"[LevelConfig] Not exists or invalid level {level} → using local fallback.");
            var lvRandom = RandomLevel(SourceNormal, level);
            lvRandom?.IsValid();
            isLocal = lvRandom != null && lvRandom.isValidLocal == true;
            return lvRandom;
        }

        isLocal = lf.isValidLocal == true;
        return lf;
    }

    #endregion

    #region Random pipeline

    // Helper get/set cache
    private void GetRandomCache(out string cache, out string last)
    {
        cache = CacheLevelIndex;
        last = LastCacheLevelIndex;
    }

    private void SetRandomCache(string cache, string last)
    {
        LastCacheLevelIndex = last;
        CacheLevelIndex = cache;
    }

    private LevelInfo RandomLevel(LevelInfo[] data, int level)
    {
        if (data == null || data.Length == 0) return null;

        var ignoreList = _randomIgnore;
        GetRandomCache(out var cache, out var last);

        if (string.IsNullOrEmpty(cache))
        {
            var easy = data.Where(x => x != null && x.levelType == LevelType.Easy).ToArray();
            var hard = data.Where(x => x != null && x.levelType == LevelType.Hard).ToArray();

            var selected = new List<LevelInfo>(CountLevelRandom);

            for (int i = 0; i < 2 && hard.Length > 0; i++)
                selected.Add(hard[UnityEngine.Random.Range(0, hard.Length)]);

            int safety = 0;
            while (selected.Count < CountLevelRandom && easy.Length > 0 && safety++ < 1000)
            {
                var e = easy[UnityEngine.Random.Range(0, easy.Length)];
                if (!selected.Contains(e)) selected.Add(e);
            }

            var pool = data.Where(x => x != null).ToArray();
            safety = 0;
            while (selected.Count < CountLevelRandom && safety++ < 2000)
            {
                var any = pool[UnityEngine.Random.Range(0, pool.Length)];
                if (!selected.Contains(any)) selected.Add(any);
            }

            selected = selected.OrderBy(_ => UnityEngine.Random.value).ToList();

            for (int i = 1; i < selected.Count; i++)
            {
                var a = selected[i - 1];
                var b = selected[i];
                if (a.levelType >= LevelType.Hard && b.levelType >= LevelType.Hard)
                {
                    int j = selected.FindIndex(i + 1, x => x.levelType == LevelType.Easy);
                    if (j != -1) (selected[i], selected[j]) = (selected[j], selected[i]);
                }
            }

            var output = selected.Select(x => x.level).ToList();
            ignoreList.AddRange(output);

            var newLast = cache;
            var newCache = string.Join(",", output);
            SetRandomCache(newCache, newLast);

            var c = ignoreList.Count - (CountLevelRandom + 5);
            if (c > 0) ignoreList.RemoveRange(0, c);
        }

        return GetRandomLevelCache(data, level);
    }

    public LevelInfo GetRandomLevelCache(LevelInfo[] data, int level)
    {
        if (data == null || data.Length == 0) return null;
        if (string.IsNullOrEmpty(CacheLevelIndex)) return null;

        var lst = CacheLevelIndex.Split(',');
        if (lst.Length == 0) return null;

        var idx = (Mathf.Max(1, level) - 1) % CountLevelRandom;
        if (idx < 0 || idx >= lst.Length) idx = 0;

        if (!int.TryParse(string.IsNullOrEmpty(lst[idx]) ? lst[0] : lst[idx], out var lv)) return null;

        EnsureIndexesNormal();
        if (_byLevel != null && _byLevel.TryGetValue(lv, out var found)) return found;

        for (int i = 0; i < data.Length; i++)
            if (data[i] != null && data[i].level == lv)
                return data[i];

        return null;
    }

    #endregion

    #region Background

    [Serializable]
    public class BackgroundInfo
    {
        public byte groupID;
        public Sprite[] background;
    }

    public BackgroundInfo[] backgroundInfo;

    public Sprite GetBackground(int id, int rand)
    {
        var info = backgroundInfo.SingleOrDefault(x => x.groupID == id) ?? backgroundInfo[0];
        return info.background[new System.Random(rand).Next(0, info.background.Length)];
    }

    #endregion
}