using System;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle.Inapp
{
    public class InappGoldAndBoosterPack : InappPanelBase
    {
        [Header("Inapp Starter Pack Item")] [SerializeField]
        private Text txtEyeValue;

        [SerializeField] private Text txtHintValue;
        [SerializeField] private Text txtTimeValue;
        
        [Header("Pack Name")] [SerializeField] private Text txtPackName;

#if UNITY_EDITOR
        public void OnValidate()
        {
            ValidatePackName();
        }

        private void ValidatePackName()
        {
            if (txtPackName != null)
            {
                gameObject.name = txtPackName.text;
            }
        }
#endif

        private int eyeValue;
        private int hintValue;
        private int timeValue;

        public override void SetDisplay()
        {
            base.SetDisplay();
            eyeValue = InappHelper.Instance.getItemRcv(skuId, "eye");
            hintValue = InappHelper.Instance.getItemRcv(skuId, "hint");
            timeValue = InappHelper.Instance.getItemRcv(skuId, "time");

            if (txtEyeValue != null)
            {
                if (eyeValue != 0)
                {
                    txtEyeValue.text = $"x{eyeValue}";
                }
            }

            if (txtHintValue != null)
            {
                if (hintValue != 0)
                {
                    txtHintValue.text = $"x{hintValue}";
                }
            }

            if (txtTimeValue != null)
            {
                if (timeValue != 0)
                {
                    txtTimeValue.text = $"x{timeValue}";
                }
            }

            DataResource dataAdd = new DataResource(RES_type.BOOSTER_1, timeValue);
            dataResources.Add(dataAdd);
            dataAdd = new DataResource(RES_type.BOOSTER_2, hintValue);
            dataResources.Add(dataAdd);
            dataAdd = new DataResource(RES_type.BOOSTER_3, eyeValue);
            dataResources.Add(dataAdd);
        }
    }
}