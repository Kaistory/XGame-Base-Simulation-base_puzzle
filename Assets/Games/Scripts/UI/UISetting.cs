using _JigblockPuzzle;
using DevUlts;
using mygame.sdk;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using static LevelConfig;

public class UISetting : PopupUI
{
    [Header("UI Settings")] [SerializeField]
    private RectTransform bgImage;

    [SerializeField] private Button btnRate;

    [SerializeField] private Button btnQuit;
    [SerializeField] private Button btnRestore;

    private bool isPauseUI;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        Init();
    }

    private void Init()
    {
        if (btnRate)
        {
            btnRate.onClick.AddListener(OnRateButtonEvent);
        }

        if (btnQuit)
        {
            btnQuit.onClick.AddListener(OnQuitButtonEvent);
        }

        if (btnRestore)
        {
            btnRestore.onClick.AddListener(InappHelper.Instance.RestorePurchases);
        }
    }

    private void OnQuitButtonEvent()
    {
        isPauseUI = false;
        // LogEvent.LevelExit(DataManager.Level, LevelManager.Instance.TimePlayDuration, DataManager.ConsecutiveLose);
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnQuitGame();
        }
        GameManager.Instance.ChangeGameState(GameState.MainMenu);
        Hide();
    }

    public void SetupPopup(bool isPause)
    {
        isPauseUI = isPause;
        ShowHomeButton(isPauseUI);
    }

    private void ShowHomeButton(bool active)
    {
        bool canShowHome = active && GameManager.Instance.CanShowMainMenu;
        if (btnQuit)
        {
            btnQuit.gameObject.SetActive(canShowHome);
        }

        if (bgImage)
        {
            var sizeY = 700;
            if (btnRestore)
            {
                btnRestore.gameObject.SetActive(false);
            }
#if UNITY_IOS
            sizeY = 700;
            if (btnRestore)
            {
                btnRestore.gameObject.SetActive(true);
            }
#endif
            bgImage.sizeDelta =
                canShowHome ? new Vector2(bgImage.sizeDelta.x, sizeY) : new Vector2(bgImage.sizeDelta.x, sizeY);
        }
    }

    private void OnRateButtonEvent()
    {
        if (SDKManager.Instance)
        {
            SDKManager.Instance.showRate();
        }
    }

    public override void Hide()
    {
        if (isPauseUI)
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.ChangeGameState(GameState.PlayingGame);
            }
        }

        base.Hide();
    }
}