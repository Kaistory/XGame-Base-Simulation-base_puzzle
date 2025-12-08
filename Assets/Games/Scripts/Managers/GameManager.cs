using System;
using System.Collections;
using _JigblockPuzzle;
using Crystal;
using DevUlts;
using DG.Tweening;
using mygame.sdk;
using time;
using UnityEngine;

public enum GameState
{
    None = 0,
    MainMenu = 1,
    Game = 2,
    PlayingGame = 3,
    PauseGame = 4,
    GameOver = 5,
    Victory = 6,
}

public enum GameMode
{
    Level
}

public class GameManager : master.Singleton<GameManager>
{
    [Header("Game Manager")] public CamCtrl camCtrl;
    public LevelManager levelManager;

    private GameState gameState = GameState.None;

    public GameState GameState
    {
        get => gameState;
        private set => gameState = value;
    }

    public bool IsPlayingGame => gameState == GameState.PlayingGame;

    public bool CanShowMainMenu => DataManager.Level >= RemoteConfigure.CFCountLevelShowMainMenu;

    public LevelConfig levelConfig => LevelRemoteManager.Instance.levelConfig;
    public LevelConfig.LevelInfo levelInfo => LevelRemoteManager.Instance.levelInfo;

    private float startTime;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60; // 60

        SDKManager.CBFinishShowLanAge += CBFinishShowLanAge;

        ItemDisplay.OnClick -= OnClickItemDisplay;
        ItemDisplay.OnClick += OnClickItemDisplay;
        startTime = MGTime.Instance.GetUtcTime();
    }

    private void CBFinishShowLanAge(int flag)
    {
        Initialize();
        if (CanShowMainMenu)
        {
            GameState = GameState.MainMenu;
            UIManager.Instance.ShowScreen<MainMenuScreen>();
        }
        else
        {
            LevelManager.Instance.LoadLevel(DataManager.Level, playType: "app_open");
        }
        //
        // LogEvent.LoadingFinish("app_open", (int)(MGTime.Instance.GetUtcTime() - startTime), true);
        // AdsHelper.Instance.loadFull4ThisTurn("app_open", false, DataManager.Level, 0);
        // AdsHelper.Instance.loadGift4ThisTurn("app_open", DataManager.Level, null);
    }

    private void Start()
    {
        Application.targetFrameRate = 60; // 60
    }

    private void OnDestroy()
    {
        ItemDisplay.OnClick -= OnClickItemDisplay;
    }

    public void ChangeGameState(GameState newGameState, Action onChangeSuccess = null)
    {
        if (GameState == newGameState)
        {
            return;
        }

        GameState = newGameState;

        TigerForge.EventManager.EmitEvent(EventName.OnChangeGameState);
        switch (GameState)
        {
            case GameState.MainMenu:
                if (CanShowMainMenu)
                {
                    UIManager.Instance.Transition(true, Load, Complete);

                    void Load()
                    {
                        LevelManager.Instance.UnloadGame();
                        startTime = MGTime.Instance.GetUtcTime();
                        LogEvent.LoadingStart("home");
                        HideUISetting();
                        HideUIInGame();
                        var mainMenu = UIManager.Instance.ShowScreen<MainMenuScreen>();
                        if (mainMenu)
                        {
                            mainMenu.ResetCurrentPanel();
                            mainMenu.ActivePanel(1);
                        }
                    }

                    void Complete()
                    {
                        LogEvent.LoadingFinish("home", (int)(MGTime.Instance.GetUtcTime() - startTime), true);
                    }
                }
                else
                {
                    LevelManager.Instance.LoadLevel(DataManager.Level, playType: "home_play");
                }

                HideBanner();
                // ShowBanner("main_menu");
                break;
            case GameState.Game:
                HideUIMainMenu();
                ShowBanner("game_play");
                break;
        }

        onChangeSuccess?.Invoke();
    }

    public void ShowUIRemoveAds()
    {
        if (DataManager.Level > 25 && DataManager.Level % 5 == 2)
        {
            var ui = UIManager.Instance.ShowPopup<UIBuyRemoveAds>();
            if (ui)
            {
                ui.LogIAPShow(LogEvent.IAP_ShowType.pack, LogEvent.IAP_ShowPosition.home_popup,
                    LogEvent.IAP_ShowAction.auto_show, true);
            }
        }
    }

    private void Initialize()
    {
        if (!DataManager.IsFirstPlayGame)
        {
            LogEvent.FirstOpen();
            DataResource[] dataResources = new DataResource[3]
            {
                new DataResource(RES_type.BOOSTER_1, 1),
                new DataResource(RES_type.BOOSTER_2, 1), new DataResource(RES_type.BOOSTER_3, 1)
            };
            DataManager.Instance.OnEarnResources(dataResources, LogEvent.ReasonItem.reward, "reward_first_game",
                DataManager.Level);
            DataManager.IsFirstPlayGame = true;
        }

        AudioManager.Instance.Initialize();
        ResetTimeShowAdsBreak();
        AdsHelper.Instance.setTimeShowFull();
    }

    private void HideUIInGame()
    {
        var uiActive = UIManager.Instance.GetScreenActive<UIInGame>();
        if (uiActive)
        {
            uiActive.Deactive();
        }
    }

    private void HideUISetting()
    {
        var uiActive = UIManager.Instance.GetPopupActive<UISetting>();
        if (uiActive)
        {
            uiActive.Hide();
        }
    }

    private void HideUIMainMenu()
    {
        var uiActive = UIManager.Instance.GetScreenActive<MainMenuScreen>();
        if (uiActive)
        {
            uiActive.Deactive();
        }
    }

    public void BackToMain()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void OnClickItemDisplay(ItemDisplay itemDisplay)
    {
        ShowShopUI();
    }

    public void ShowShopUI()
    {
        if (UIManager.Instance.GetScreenActive<MainMenuScreen>() != null &&
            UIManager.Instance.GetScreen<MainMenuScreen>().gameObject.activeInHierarchy)
        {
            UIManager.Instance.GetScreen<MainMenuScreen>().ActivePanel(0);
            var shop = UIManager.Instance.GetPopup<UIShopPopup>();
            shop.GetComponent<SafeAreaPanel>().enabled = false;
            shop.ScrollToTarget();
            shop.LogIAPShow(LogEvent.IAP_ShowType.shop, LogEvent.IAP_ShowPosition.home_shop,
                LogEvent.IAP_ShowAction.click_button, true);
        }
        else
        {
            var shop = UIManager.Instance.GetPopup<UIShopPopup>();
            shop.GetComponent<SafeAreaPanel>().enabled = true;
            shop.ScrollToTarget();
            shop.LogIAPShow(LogEvent.IAP_ShowType.shop, LogEvent.IAP_ShowPosition.shop_popup,
                LogEvent.IAP_ShowAction.click_button, true);
        }
    }


    #region Ads

    private float CurrentTimeShowAdsBreak;

    private void OnInitCheckAds()
    {
    }

    private void ResetTimeShowAdsBreak()
    {
        CurrentTimeShowAdsBreak = RemoteConfigure.CFShowAdsBreak;
    }

    private bool CheckShowAdsBreak()
    {
        CurrentTimeShowAdsBreak -= Time.deltaTime;
        return CurrentTimeShowAdsBreak <= 0f;
    }

    public void ShowAdsBreak()
    {
        if (CheckShowAdsBreak())
        {
            ShowFullBreakAds();
        }
    }

    public bool ShowFullBreakAds()
    {
        ResetTimeShowAdsBreak();
        AudioManager.Instance.SetCacheAudio();
        string placement = "break_ads";
        LogEvent.LogEventParam("show_ads_full_{0}", placement);
        return AdsHelper.Instance.showFull(placement, DataManager.Level, DataManager.ConsecutiveLose, 0, 1,
            true,
            false, cb: state =>
            {
                if (state == AD_State.AD_CLOSE || state == AD_State.AD_CLOSE2 || state == AD_State.AD_SHOW_FAIL ||
                    state == AD_State.AD_SHOW_FAIL2)
                {
                    ResetAfterShowFull();
                    Time.timeScale = 1;
                    AudioManager.Instance.ResetAudio();
                }
            });
    }

    public void ShowFull(string placement, Action cbCloseOrFail = null)
    {
        if (AdsHelper.Instance)
        {
            ResetTimeShowAdsBreak();
            AudioManager.Instance.SetCacheAudio();
            Time.timeScale = 0;
            LogEvent.LogEventParam("show_ads_full_{0}", placement);
            var b = AdsHelper.Instance.showFull(placement: placement, level: DataManager.Level,
                countLose: DataManager.ConsecutiveLose,
                typeShowImg: 0, typeShowOnPlaying: 0, isWaitAd: false, isHideBtClose: false, ischecknumover: true,
                cb: (state) =>
                {
                    if (state == AD_State.AD_CLOSE || state == AD_State.AD_CLOSE2 ||
                        state == AD_State.AD_SHOW_FAIL ||
                        state == AD_State.AD_SHOW_FAIL2)
                    {
                        Time.timeScale = 1;
                        ResetAfterShowFull();
                        cbCloseOrFail?.Invoke();
                        AudioManager.Instance.ResetAudio();
                    }
                });
            if (!b)
            {
                Time.timeScale = 1;
                cbCloseOrFail?.Invoke();
            }
        }
        else
        {
            cbCloseOrFail?.Invoke();
        }
    }

    public void ShowBanner(string placement = "gameplay")
    {
        if (AdsHelper.Instance && RemoteConfigure.CFEnableBanner && IsAvailableShowBanner())
        {
            AdsHelper.Instance.showBanner(placement, AD_BANNER_POS.BOTTOM, App_Open_ad_Orien.Orien_Landscape, 0, 320,
                50);
            TigerForge.EventManager.EmitEvent(EventName.RefreshBannerArea);
        }
    }

    public void HideBanner()
    {
        if (AdsHelper.Instance)
        {
            AdsHelper.Instance.hideBanner(0);
            TigerForge.EventManager.EmitEvent(EventName.RefreshBannerArea);
        }
    }

    public void ShowGift(string placement, Action cbSuccess, Action cbCloseOrFail = null)
    {
        LogEvent.LogEventParam("watch_ads_gift_{0}", placement);

        if (AdsHelper.Instance)
        {
            AudioManager.Instance.SetCacheAudio();
            Time.timeScale = 0;
            var state = AdsHelper.Instance.showGift(placement: placement, lv: DataManager.Level,
                isShowWait: false,
                cb: (state) =>
                {
                    Debug.Log($"Gift done with state: {state} - {Time.timeScale}");
                    Time.timeScale = 1;
                    if (state == AD_State.AD_REWARD_OK)
                    {
                        cbSuccess?.Invoke();
                    }

                    if (state == AD_State.AD_CLOSE || state == AD_State.AD_CLOSE2 ||
                        state == AD_State.AD_SHOW_FAIL ||
                        state == AD_State.AD_SHOW_FAIL2)
                    {
                        cbCloseOrFail?.Invoke();
                        UIManager.Instance.SetScreenAfterAds();
                        AudioManager.Instance.ResetAudio();
                        DOVirtual.DelayedCall(0.5f,
                                () => { AdsHelper.Instance.loadGift4ThisTurn(placement, GameRes.LevelCommon(), null); })
                            .SetId(this);
                    }
                });

            if (state != 0)
            {
                Time.timeScale = 1;
                cbCloseOrFail?.Invoke();
                ShowNotificationNoAdsAvailable();
            }
        }
        else
        {
            cbCloseOrFail?.Invoke();
            ShowNotificationNoAdsAvailable();
        }
    }

    private void ShowNotificationNoAdsAvailable()
    {
        UIManager.Instance.NotifyContent(content: "No Ads Available!", "no_ads_available");
    }

    // check time for show banner
    private bool IsAvailableShowBanner()
    {
        var timeWaitBanner = RemoteConfigure.CfTimeWaitBanner;
        return LogEventManager.FirstTimeJoinGame > 0 && TimeLogger.Instance.TotalTime >= timeWaitBanner;
    }

    private void ResetAfterShowFull()
    {
        UIManager.Instance.SetScreenAfterAds();
    }

    #endregion

    #region Helper Methods

    public static void Delay(float duration, Action callBack)
    {
        Instance.StartCoroutine(IEDelay());

        IEnumerator IEDelay()
        {
            float timer = duration;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            callBack?.Invoke();
        }
    }

    public void DelayCall(float duration, Action callBack)
    {
        StartCoroutine(IEDelay());

        IEnumerator IEDelay()
        {
            float timer = duration;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            callBack?.Invoke();
        }
    }

    #endregion
}