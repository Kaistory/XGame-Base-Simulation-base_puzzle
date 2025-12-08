using mygame.sdk;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class ButtonClaimCoin : MonoBehaviour
    {
        [Header("Button Claim Coin")] [SerializeField]
        private Button btnClick;

        [SerializeField] private Text txtValue;
        [SerializeField] private Text txtDescription;

        private int valueX;
        private int amountCoinBase;

        public void AddListener(UnityAction callback)
        {
            btnClick.onClick.RemoveAllListeners();
            btnClick.onClick.AddListener(callback);
        }

        public void Initialize(int amountBase, int amountX = 1)
        {
            valueX = amountX;
            amountCoinBase = amountBase;

            if (txtDescription)
            {
                if (amountX > 1)
                {
                    txtDescription.SetText("claim_x", StateCapText.FirstCapOnly, FormatText.F_Int, formatObj: valueX,
                        defaultValue: $"Claim x{valueX}");
                }
                else
                {
                    txtDescription.SetText("claim", defaultValue: "Claim");
                }
            }

            if (txtValue)
            {
                var valueCoin = amountCoinBase * valueX;
                txtValue.text = valueCoin.ToString();
            }
        }
    }
}