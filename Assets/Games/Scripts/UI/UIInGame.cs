using System;
using DevUlts;
using DG.Tweening;
using System.Linq;
using mygame.sdk;
using MyGame.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class UIInGame : ScreenUI
    {
        [Header("UIInGame")] [SerializeField] private Button btnSetting;

        [SerializeField] private HardLevelWarning hardLevelWarnning;
        [SerializeField] private GameObject blockClick;
        [SerializeField] private Text txtLevelName;
        [SerializeField] private Text txtAmountCapacity;

        [Header("Boosters")] [SerializeField] private BoosterInGame btnBooster1;
        [SerializeField] private BoosterInGame btnBooster2;
        [SerializeField] private BoosterInGame btnBooster3;

        [Header("Particle System")] [SerializeField]
        private ParticleSystem particleSystemBoost;
        private LevelConfig.LevelInfo levelInfo;
        private int currentLevel = 0;

        protected void OnEnable()
        {
            LevelManager.IsGameReady += SetBlockClick;
            levelInfo = LevelManager.Instance.levelInfo;
            currentLevel = levelInfo.level;

            InitBooster();
            InitUI();
            TigerForge.EventManager.StartListening(EventName.UpdateCapacity,UpdateCapacity);
            TigerForge.EventManager.StartListening(EventName.UseBoostCapacity,UseParticale);
        }

        protected void OnDisable()
        {
            LevelManager.IsGameReady -= SetBlockClick;
            TigerForge.EventManager.StopListening(EventName.UpdateCapacity,UpdateCapacity);
            TigerForge.EventManager.StopListening(EventName.UseBoostCapacity,UseParticale);
        }

        private void InitUI()
        {
            SetBlockClick(false);
            Debug.Log("lvl in uningame: " + currentLevel.ToString());
            if (txtLevelName)
            {
                // txtLevelName.SetText("level_x", StateCapText.FirstCap, FormatText.F_Int, currentLevel);
                txtLevelName.text = "Level " + currentLevel.ToString();
            }
            
            if (txtAmountCapacity)
            {
                txtAmountCapacity.text = "0/4";
            }
        }


        public void SetBlockClick(bool isReadyGame)
        {
            if (blockClick)
            {
                blockClick.SetActive(!isReadyGame);
            }
        }
        void UpdateCapacity()
        {
            int capacity = TrunkManager.Instance.m_capacity;
            int maxCapacity = TrunkManager.Instance.m_maxCapacity;
            txtAmountCapacity.text  = (maxCapacity - capacity).ToString() + "/" + maxCapacity.ToString();
            if (capacity <= 1)
            {
                txtAmountCapacity.DOColor(Color.red, 0.5f)
                    .SetEase(Ease.InOutFlash) 
                    .SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                txtAmountCapacity.DOKill();
                txtAmountCapacity.color =Color.white;
            }
            
        }

        void UseParticale()
        {
            particleSystemBoost.Play();
        }


        protected void Start()
        {
            Init();
        }

        private void InitBooster()
        {
            if (btnBooster1)
            {
                btnBooster1.Initialize(RES_type.BOOSTER_1);
            }

            if (btnBooster2)
            {
                btnBooster2.Initialize(RES_type.BOOSTER_2);
            }

            if (btnBooster3)
            {
                btnBooster3.Initialize(RES_type.BOOSTER_3);
            }
        }

        private void Init()
        {
            if (btnSetting)
            {
                btnSetting.onClick.RemoveAllListeners();
                btnSetting.onClick.AddListener(OnSettingClick);
            }
        }

        public void ShowWarnning()
        {
            hardLevelWarnning.gameObject.SetActive(true);
            hardLevelWarnning.ShowWarnning();
        }

        private void OnSettingClick()
        {
            var uiActive = UIManager.Instance.ShowPopup<UISetting>();
            if (uiActive)
            {
                uiActive.SetupPopup(true);
            }

            GameManager.Instance.ChangeGameState(GameState.PauseGame);
        }
    }
}