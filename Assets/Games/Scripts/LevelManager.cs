using _JigblockPuzzle;
using DevUlts;
using mygame.sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using MyGame.Data;
using MyGame.Manager;
using MyGame.Tutorial;
using TigerForge;
using time;
using UnityEngine;

public enum LevelType
{
    Easy = 0,
    Hard = 1,
}

public class LevelManager : MonoBehaviour
{
    [Header("Level Manager")] public static LevelManager Instance;
    public static Action OnWin;
    public static Action OnLose;
    public static Action UnLoadLevelAction;
    public static Action<bool> IsGameReady;
    
    public static Action<RES_type, bool, float> CooldownBoosterAction;

    [SerializeField] private Transform levelRoot;
    [SerializeField] private SpriteRenderer background;

    private LevelConfig.LevelInfo currentLevelInfo;
    private int currentLevelID;

    public bool initialized;

    private SaveGameData saveGameData = new SaveGameData();

    public LevelConfig levelConfig => LevelRemoteManager.Instance.levelConfig;
    public LevelConfig.LevelInfo levelInfo => LevelRemoteManager.Instance.levelInfo;

    private int levelTimePlayed;

    public float timeStartLevel
    {
        get => PlayerPrefs.GetFloat("time_start_level");
        private set => PlayerPrefs.SetFloat("time_start_level", value);
    }

    private Level levelCache;
    public Level CurrentLevel { get; private set; }
    private bool isLoad;

    public int TimePlayDuration => (int)((Time.time - timeStartLevel) * 1000);

    private void Awake()
    {
        Instance = this;
        RegisterListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void RegisterListener()
    {
    }

    private void RemoveListener()
    {
    }


    public void LoadLevel(int level, bool isForce = false, string playType = "play_level")
    {
        currentLevelID = level;

        initialized = false;
        LevelRemoteManager.Instance.LoadLevelCache(currentLevelID);
        currentLevelInfo = LevelRemoteManager.Instance.levelInfo;
        UnloadGame();
        bool isAnim = false;
        var uiInGame = UIManager.Instance.GetScreenActive<UIInGame>();

        if ( /*uiInGame == null &&*/ !isForce)
        {
            long startTime = 0;

            var transitionUI = UIManager.Instance.Transition(false, () =>
            {
                startTime = MGTime.Instance.GetUtcTime();
                LogEvent.LoadingStart("level");
                uiInGame = UIManager.Instance.ShowScreen<UIInGame>();
                isAnim = Initialized(currentLevelID);
                Load();
            }, complete: () =>
            {
                LogEvent.LoadingFinish("level", (int)(MGTime.Instance.GetUtcTime() - startTime), true);
                Complete();
            }, null, true);

            transitionUI.FadeIn(onFadeComplete: () =>
            {
                var uiMainMenu = UIManager.Instance.GetScreenActive<MainMenuScreen>();
                if (uiMainMenu)
                {
                    uiMainMenu.Deactive();
                }

                if (uiInGame == null)
                {
                    uiInGame = UIManager.Instance.ShowScreen<UIInGame>();
                }
                else
                {
                    uiInGame.Active();
                }

                LevelRemoteManager.Instance.WaitLoadLevel(transitionUI);
            });
        }
        else
        {
            uiInGame = UIManager.Instance.ShowScreen<UIInGame>();
            isAnim = Initialized(level);
            Load();
        }

        void Load()
        {
            if (levelCache == null)
            {
                levelCache = Resources.Load<Level>($"Level");
            }

            CurrentLevel = Instantiate(levelCache, levelRoot.transform);
            LevelConfig.LevelInfo info = levelInfo;
            if (CurrentLevel != null)
            {
                CurrentLevel.SetLevelInfo(info);
                CurrentLevel.Initialize();
                IsGameReady?.Invoke(false);
            }

            timeStartLevel = Time.time;
            //AudioManager.Instance.PlayPlaylist();
            info.PlayIndexLevel++;
            LogEvent.LogEventParam($"level_{level:0000}_play");
            GameManager.Instance.ChangeGameState(GameState.Game);
            //LogEvent.LevelPlay(DataManager.Level, playType: playType, CurrentLevel.LevelInfo.PlayIndexLevel,
                //CurrentLevel.LevelInfo.levelTime, CurrentLevel.LevelInfo.LoseIndexLevel);
            initialized = true;
        }

        void Complete()
        {
            CurrentLevel.BeginLevel();
        }
    }
    
    public void OnUseBooster(RES_type type)
    {
        if (DataManager.GetResources(type) > 0)
        {
            BoostManger.Instance.SetActiveBoost(type);
        }
        DataManager.Instance.OnSinkResources(type, -1, LogEvent.ReasonItem.use, "use_in_game",
            DataManager.Level);
        CooldownBoosterAction?.Invoke(type, true, 30); // type, use, second
    }

    //private bool Initialized(int level)
    //{
    //    bool isLoad = false;

    //    var lvl = (int)levelInfo.levelID;


    //    if (IsHasSaveGameData(level))
    //    {
    //        lvl = saveGameData.level;
    //    }
    //    else
    //    {
    //        isLoad = true;
    //        TigerForge.EventManager.EmitEvent(EventName.LoadNewLevel);
    //    }

    //    CurrentLevelImage = LevelRemoteManager.Instance.LoadCachedObject();

    //    if (!CurrentLevelImage)
    //    {
    //        Debug.LogError($"Failed to load sprites for level {level}. The result is null or empty.");
    //        return isLoad;
    //    }

    //    isUseFreeze = false;
    //    isUseHintShowOriginalPicture = false;
    //    levelTimeLeft = currentLevelInfo.levelTime;
    //    lastSecondValue = Mathf.CeilToInt(levelTimeLeft);
    //    OnUpdateTimeEachSecond?.Invoke(lastSecondValue);
    //    return isLoad;
    //}
    private bool Initialized(int level)
    {
        bool isLoad = false;

        var lvl = (int)levelInfo.level;
        if (IsHasSaveGameData(level))
        {
            lvl = saveGameData.level;
        }
        else
        {
            isLoad = true;
            TigerForge.EventManager.EmitEvent(EventName.LoadNewLevel);
        }
        
        // var backgroundID = levelInfo?.backgroundID ?? 0;
        // background.sprite = levelConfig.GetBackground(backgroundID, level);
        // background.gameObject.SetActive(false); // Off nếu set màu theo camera
        // GameManager.Instance.camCtrl.SetBackgroundColor(backgroundID);
        
        return isLoad;
    }


    public void UnloadGame()
    {
        UnLoadLevelAction?.Invoke();

        if (CurrentLevel)
        {
            CurrentLevel.gameObject.SetActive(false);
            Destroy(CurrentLevel.gameObject);
        }
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
    }

    public void LoseGame()
    {
        OnLose?.Invoke();
        DataManager.ConsecutiveLose++;
        DataManager.ConsecutiveWin = 0;
        // CurrentLevel.LevelInfo.LoseIndexLevel++;
        // LogEvent.LevelEnd(lv: DataManager.Level, playTime: LevelTimePlayed, playIndex: levelInfo.PlayIndexLevel,
        //     result: isQuitGame ? LogEvent.LevelResult.exit : LogEvent.LevelResult.lose, requireTime: CurrentLevel.LevelInfo.levelTime,
        //     totalMove: CurrentLevel.TotalMoved, bonusTime: TotalTimeBonusBought,
        //     loseIndex: CurrentLevel.LevelInfo.LoseIndexLevel, levelProgress: CurrentLevel.LevelProgress);
        LogEvent.LevelLose(currentLevelID);

        UIManager.Instance.ShowPopup<UILoseGame>();
    }

    public void WinGame()
    {
        if (!GameManager.Instance.IsPlayingGame)
            return;
        OnWin?.Invoke();
        // LogEvent.LevelWin(CurrentLevel.LevelInfo.level, LevelTimePlayed);
        // LogEvent.LogEventParam($"level_{DataManager.Level:0000}_complete");
        // LogEvent.LevelEnd(lv: DataManager.Level, playTime: LevelTimePlayed,
        //     playIndex: CurrentLevel.LevelInfo.PlayIndexLevel, result: LogEvent.LevelResult.win,
        //     requireTime: CurrentLevel.RequireTime, totalMove: CurrentLevel.TotalMoved, bonusTime: TotalTimeBonusBought,
        //     loseIndex: CurrentLevel.LevelInfo.LoseIndexLevel, levelProgress: CurrentLevel.LevelProgress);
        DataManager.ConsecutiveLose = 0;
        DataManager.ConsecutiveWin++;
        GameManager.Instance.ChangeGameState(GameState.Victory);
    }


    public void OnQuitGame()
    {
        GameManager.Instance.ChangeGameState(GameState.MainMenu);
    }


    private void CompleteLevel()
    {
        if (CurrentLevel != null)
        {
            CurrentLevel.CompleteLevel();
        }
    }

    public bool IsHasSaveGameData(int level)
    {
        saveGameData = saveGameData.LoadData();
        var lf = levelConfig.GetLevelInfo(level);
        var lvl = lf.levelID;

        var checkScrewLength = saveGameData != null;
        var checkLevel = saveGameData != null && lvl == saveGameData.level;

        return saveGameData != null && !saveGameData.IsEndGame(level) && checkLevel && checkScrewLength;
    }
}