using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using static LevelConfig;

public class LevelRemoteManager : Singleton<LevelRemoteManager>
{
    public LevelConfig levelConfig
    {
        get
        {
#if UNITY_ANDROID
            return levelConfigAndroid;
#endif
            return levelConfigIOS;
        }
    }

    [SerializeField] private LevelConfig levelConfigAndroid;
    [SerializeField] private LevelConfig levelConfigIOS;

    public LevelInfo levelInfo { get; private set; }


    private void Awake()
    {
        InitializeSingleton();
    }

    public void LoadLevelCache(int level, bool forceRelease = true)
    {
        levelInfo = levelConfig.GetLevelInfo(level, false);
        if (levelInfo != null)
        {
            levelInfo.IsValid();
            if (!levelInfo.IsValid())
            {
                if (!string.IsNullOrEmpty(CacheLevelIndex))
                {
                    LastCacheLevelIndex = CacheLevelIndex;
                    CacheLevelIndex = "";
                }
            }
        }

        levelInfo = levelConfig.GetLevelInfo(level, out _);
    }

    public List<int> GetAvailableLevels()
    {
        var levels = new List<int>();

        for (int i = 0; i < levelConfig.levelInfos.Length; i++)
        {
            var lv = levelConfig.levelInfos[i];
            levels.Add(lv.level);
        }

        return levels;
    }

    public void WaitLoadLevel(TransitionUI transitionUI)
    {
        transitionUI.FadeOut();
    }

    public int GetRandomLevel()
    {
        var levels = GetAvailableLevels();

        int randomIndex = UnityEngine.Random.Range(0, levels.Count);
        int randomLevel = levels[randomIndex];
        return randomLevel;
    }
}