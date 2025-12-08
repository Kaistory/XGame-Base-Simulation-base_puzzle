using System;
using DevUlts;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class UIBuyBonusTime : PopupUI
    {
        [Header("UI Buy Bonus Time")] [SerializeField]
        private Text txtBoosterPrice;

        [SerializeField] private Text txtNameBooster;
        [SerializeField] private Text txtDescBooster;

        [SerializeField] private Button btnWatchAds;

        [SerializeField] private Button btnBuyBooster;
        [SerializeField] private Button btnExit;

        private int price = 200;

        private Action onHideEvent;
        private Action onSuccessEvent;

        protected void Start()
        {
            if (btnWatchAds)
            {
                btnWatchAds.onClick.AddListener(OnWatchAdsClick);
            }

            if (btnBuyBooster)
            {
                btnBuyBooster.onClick.AddListener(OnBuyBoosterClick);
            }

            if (btnExit)
            {
                btnExit.onClick.AddListener(OnExitClick);
            }
        }

        private void OnExitClick()
        {
            onHideEvent?.Invoke();
            Hide();
        }

        public void Initialize(Action onSuccess, Action onHide, int priceBuy = 200)
        {
            price = priceBuy;
            onSuccessEvent = onSuccess;
            onHideEvent = onHide;

            if (txtBoosterPrice)
            {
                txtBoosterPrice.SetTextMoney(price);
            }

            if (txtNameBooster)
            {
                txtNameBooster.SetText($"buy_bonus_time_name", defaultValue: $"Bonus Time");
            }

            if (txtDescBooster)
            {
                txtDescBooster.SetText($"buy_bonus_time_name", defaultValue: $"Bonus Time");
            }
        }

        private void OnBuyBoosterClick()
        {
            DataManager.Instance.OnSinkResources(RES_type.GOLD, -price, LogEvent.ReasonItem.use, "buy_booster_in_game",
                DataManager.Level,
                BuyBoosterSuccess);
        }

        private void OnWatchAdsClick()
        {
            GameManager.Instance.ShowGift($"buy_bonus_time", BuyBoosterSuccess);
        }

        private void BuyBoosterSuccess()
        {
            onSuccessEvent?.Invoke();
            Hide();
        }
    }
}