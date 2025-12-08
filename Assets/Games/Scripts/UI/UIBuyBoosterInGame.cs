using DevUlts;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class UIBuyBoosterInGame : PopupUI
    {
        [Header("UI Buy Booster")] [SerializeField]
        private Text txtBoosterPrice;

        [SerializeField] private Text txtNameBooster;
        [SerializeField] private Text txtDescBooster;
        [SerializeField] private Button btnWatchAds;

        [SerializeField] private Button btnBuyBooster;
        [SerializeField] private Image iconBooster;

        private InfoResources infoResources;
        private Tween fadeTween;

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
        }

        public void Initialize(InfoResources info)
        {
            infoResources = info;
            if (infoResources == null)
            {
                Debug.LogError($"InfoResources is null");
                return;
            }

            if (txtNameBooster)
            {
                txtNameBooster.SetText(infoResources.name);
            }

            if (txtDescBooster)
            {
                txtDescBooster.SetText(infoResources.des);
            }

            if (txtBoosterPrice)
            {
                txtBoosterPrice.SetTextMoney(infoResources.price);
            }

            if (iconBooster)
            {
                iconBooster.sprite = infoResources.icon;
            }
        }

        private void OnBuyBoosterClick()
        {
            DataManager.Instance.OnSinkResources(RES_type.GOLD, -infoResources.price, LogEvent.ReasonItem.use,
                "buy_booster", DataManager.Level,
                BuyBoosterSuccess);
        }

        private void OnWatchAdsClick()
        {
            GameManager.Instance.ShowGift($"buy_booster_{infoResources.res_type}", BuyBoosterSuccess);
        }

        private void BuyBoosterSuccess()
        {
            if (infoResources != null)
            {
                DataManager.Instance.OnEarnResources(infoResources.res_type, 1, LogEvent.ReasonItem.exchange,
                    "buy_booster", DataManager.Level);
            }

            Hide();
        }
    }
}