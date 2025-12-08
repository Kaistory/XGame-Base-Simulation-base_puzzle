using System;
using DevUlts;
using mygame.sdk;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class UILoseGame : PopupUI
    {
        [Header("UI Elements")] [SerializeField]
        private Text textLevel;

        [SerializeField] private Button btnRetry;
        [SerializeField] private Button btnHome;

        private bool canShowMainMenu;

        protected void OnEnable()
        {
            AudioManager.Instance.PauseMusic();
        }

        protected void OnDisable()
        {
            AudioManager.Instance.StopSFX();
            AudioManager.Instance.ResumeMusic();
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            canShowMainMenu = GameManager.Instance.CanShowMainMenu;
            if (btnHome)
            {
                btnHome.gameObject.SetActive(canShowMainMenu);
            }

            RefreshUI();
        }


        protected void Start()
        {
            if (btnRetry)
            {
                btnRetry.onClick.AddListener(OnRetryClick);
            }

            if (btnHome)
            {
                btnHome.onClick.AddListener(OnHomeClick);
            }
        }

        private void OnRetryClick()
        {
            // GameManager.Instance.ShowFull("retry_level");
            LevelManager.Instance.LoadLevel(DataManager.Level, playType: "restart_lose");
            Hide();
        }

        private void OnHomeClick()
        {
            // GameManager.Instance.ShowFull("back_to_home");
            GameManager.Instance.ChangeGameState(GameState.MainMenu);
            Hide();
        }

        public void RefreshUI()
        {
            int lv = GameRes.GetLevel();
            if (LevelRemoteManager.Instance.levelConfig.GetLevelInfos().Length <= lv)
            {
                textLevel.SetText("infinity_mode");
            }
            else
            {
                textLevel.SetText($"level_x", StateCapText.FirstCapOnly, FormatText.F_Int, formatObj: lv,
                    defaultValue: $"Level {lv}");
            }
        }
    }
}