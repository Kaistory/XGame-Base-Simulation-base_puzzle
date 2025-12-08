using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class ButtonLevelPlay : MonoBehaviour
    {
        [SerializeField] private Button btnPlay;
        [SerializeField] private Text txtLevel;
        [SerializeField] private ButtonScaler btnScaler;
        [SerializeField] private Transform holder;

        private bool isInteractable = true;

        private Tween pulseTween; // Giữ tham chiếu tween để dễ stop
        
        
        
        public bool IsInteractable
        {
            get => isInteractable;

            private set
            {
                isInteractable = value;
                {
                    SetInteractable(value);
                }
            }
        }

        public void RefreshUI()
        {
            int lv = DataManager.Level;

            if (LevelRemoteManager.Instance.levelConfig.GetLevelInfos().Length <= lv)
            {
                txtLevel.SetText("infinity_mode");
            }
            else
            {
                txtLevel.SetText($"level_x", StateCapText.FirstCapOnly, FormatText.F_Int, formatObj: lv,
                    defaultValue: $"Level {lv}");
            }
        }

        public void Initialize()
        {
            if (btnPlay)
            {
                btnPlay.onClick.RemoveAllListeners();
            }

            IsInteractable = true;
        }

        public void AddListener(UnityAction listener)
        {
            if (btnPlay)
            {
                btnPlay.onClick.AddListener(listener);
            }
        }

        private void SetInteractable(bool active)
        {
            if (btnPlay)
            {
                btnPlay.interactable = active;
            }

            if (btnScaler)
            {
                btnScaler.enabled = active;
            }
        }
    }
}