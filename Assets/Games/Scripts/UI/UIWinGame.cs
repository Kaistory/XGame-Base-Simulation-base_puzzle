using System;
using System.Collections.Generic;
using DevUlts;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class UIWinGame : PopupUI
    {
        [Header("UI Win Game")] [SerializeField]
        private ButtonClaimCoin btnClaim;

        [SerializeField] private List<ItemDisplay> itemDisplays;

        [SerializeField] private GameObject blockUI;

        [SerializeField] private ButtonClaimCoin btnClaimAds;

        [SerializeField] private ParticleSystem FX_Cft;

        private LevelConfig.LevelInfo levelInfo;
        private int amountCoinBase;


        private const int valueCoinClaimAds = 3;

        protected void OnEnable()
        {
            AudioManager.Instance.PauseMusic();
            AudioManager.Instance.PlaySFX(AudioName.SFX_WinGame);
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            blockUI.SetActive(false);
            PlayFXCft();
        }

        protected void OnDisable()
        {
            AudioManager.Instance.StopSFX();
            AudioManager.Instance.ResumeMusic();
            DOTween.Kill(this);
        }

        public void Initialize(LevelConfig.LevelInfo info)
        {
            levelInfo = info;
            amountCoinBase = levelInfo.levelRewardValue;
            if (btnClaim)
            {
                btnClaim.Initialize(amountCoinBase, 1);
            }

            if (btnClaimAds)
            {
                btnClaimAds.Initialize(amountCoinBase, valueCoinClaimAds);
            }
        }

        protected void Start()
        {
            if (btnClaim)
            {
                btnClaim.AddListener(OnClaimClick);
            }

            if (btnClaimAds)
            {
                btnClaimAds.AddListener(OnClaimX5Click);
            }
        }


        private void PlayFXCft()
        {
            if (FX_Cft)
            {
                FX_Cft.Play();
            }
        }

        private void OnClaimClick()
        {
            GameManager.Instance.ShowFull("ui_complete_next_level",
                () => { FlyCoin(amountCoinBase, LogEvent.ReasonItem.reward); });
        }

        public void FlyCoin(int amount, LogEvent.ReasonItem reason)
        {
            blockUI.SetActive(true);
            RewardReceivedHub.AddCacheValue(RES_type.GOLD, amount);
            RewardReceivedHub.Instance.CoinFly(amount, () =>
            {
                Hide();
                GameManager.Instance.BackToMain();
            });
            DataManager.Instance.OnEarnResources(RES_type.GOLD, amount, reason,
                "ui_win_game", DataManager.Level);
        }

        private void OnClaimX5Click()
        {
            GameManager.Instance.ShowGift($"claim_x{valueCoinClaimAds}_win",
                () => { FlyCoin(amountCoinBase * valueCoinClaimAds, LogEvent.ReasonItem.watch_ads); });
        }

        private void CheckIncreaseLevel()
        {
            if (DataManager.Level < LevelManager.Instance.levelConfig.GetLevelInfos().Length)
            {
                GameRes.IncreaseLevel();
            }
        }

        public override void Hide()
        {
            DOVirtual.DelayedCall(1, () => { base.Hide(); }).SetId(this);
            CheckIncreaseLevel();
        }
    }
}