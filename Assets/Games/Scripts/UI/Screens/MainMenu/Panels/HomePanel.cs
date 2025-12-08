using System;
using _JigblockPuzzle;
using mygame.sdk;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HomePanel : MainMenuPanel
{
    //[Header("Shop Panel")] [SerializeField]
    //private List<InappPanelBase> inappPanelsItems;

    [Header("Home Panel")] [SerializeField]
    private Transform BGTransform;

    [SerializeField] private ButtonLevelPlay btnLevel;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnRemoveAds;

    private void Awake()
    {
        if (SdkUtil.isFold() || SdkUtil.isiPad())
        {
            SetBGScale(1.4f);
        }
        else
        {
            SetBGScale(1f);
        }

        Initialize();
    }

    private void SetBGScale(float scale = 1f)
    {
        if (BGTransform)
        {
            BGTransform.localScale = Vector3.one * scale;
        }
    }

    private void OnEnable()
    {
        TigerForge.EventManager.StartListening(EventName.OnRemoveAdsAction, CheckRemoveAds);
    }

    private void OnDisable()
    {
        TigerForge.EventManager.StopListening(EventName.OnRemoveAdsAction, CheckRemoveAds);
    }

    public override void Active()
    {
        base.Active();
        RefreshUI();
        if (AdsHelper.isRemoveAds(0))
        {
            btnRemoveAds.gameObject.SetActive(false);
        }
    }

    private void Initialize()
    {
        if (btnLevel)
        {
            btnLevel.Initialize();
            btnLevel.AddListener(OnLoadLevel);
        }

        if (btnSetting)
        {
            btnSetting.onClick.AddListener(OnSettingClick);
        }

        if (btnRemoveAds)
        {
            btnRemoveAds.onClick.AddListener(OnRemoveAdsClick);
        }
    }

    private void OnSettingClick()
    {
        var uiActive = UIManager.Instance.ShowPopup<UISetting>();
        if (uiActive)
        {
            uiActive.SetupPopup(false);
        }
    }

    private void OnRemoveAdsClick()
    {
        var ui = UIManager.Instance.ShowPopup<UIBuyRemoveAds>();
        if (ui)
        {
            ui.LogIAPShow(LogEvent.IAP_ShowType.pack, LogEvent.IAP_ShowPosition.home_popup,
                LogEvent.IAP_ShowAction.click_button, true);
        }
    }

    private void CheckRemoveAds()
    {
        if (btnRemoveAds && !AdsHelper.isRemoveAds(0))
        {
            bool isRemoveAds = AdsHelper.isRemoveAds(0);
            btnRemoveAds.gameObject.SetActive(!isRemoveAds);
        }
    }

    private void OnLoadLevel()
    {
        if (!HasConnection())
        {
            return;
        }

        var lv = DataManager.Level;
        if (LevelRemoteManager.Instance.levelConfig.GetLevelInfos().Length < lv)
        {
            return;
        }

        if (lv == LevelRemoteManager.Instance.levelConfig.GetLevelInfos().Length)
        {
            lv = LevelRemoteManager.Instance.GetRandomLevel();
        }

        LevelManager.Instance.LoadLevel(DataManager.Level, playType: "home_play");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void RefreshUI()
    {
        CheckRemoveAds();
        int lv = DataManager.Level;

        var levelInfo = LevelRemoteManager.Instance.levelConfig.GetLevelInfos();

        btnLevel.RefreshUI();
    }


    private bool HasConnection()
    {
        string title = UIExtension.GetTextValue("notification");
        string msg = UIExtension.GetTextValue("please_check_internet_connection");
        return mygame.sdk.SDKManager.Instance.checkConnection(title, msg);
    }
}