using System.Collections.Generic;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Tutorial
{
    public class UiTutorialBoost : PopupUI
    {
        [SerializeField] private List<Image> _images;
        [SerializeField] private List<GameObject> _imageGameObjects;
        [SerializeField] private Text _text;
        private string[] textDescription =
        {
            "Remove from conveyor : You can remove any trunks from conveyor belts.",
            "Add conveyor capacity: +3 trunks ",
            "Remove the outer trunk: the outer trunk will be removed."
        };
        
        public virtual void Initialize(UIManager manager)
        {
            this.uiManager = manager;
            isShowing = false;
            int lvl = LevelManager.Instance.levelInfo.level;
            if (lvl == DataManager.Instance.GetInfoResources(RES_type.BOOSTER_1).lvUnlock)
            {
                _imageGameObjects[0].SetActive(true);
                _text.text = textDescription[0];
            }
            else
            {
                if (lvl == DataManager.Instance.GetInfoResources(RES_type.BOOSTER_2).lvUnlock)
                {
                    _imageGameObjects[1].SetActive(true);
                    _text.text = textDescription[1];
                }
                else
                {
                    if (lvl == DataManager.Instance.GetInfoResources(RES_type.BOOSTER_3).lvUnlock)
                    {
                        _imageGameObjects[2].SetActive(true);
                        _text.text = textDescription[2];
                    }
                }
            }
            ShowIcon();
            if (ButtonClose != null)
                {
                    ButtonClose.onClick.AddListener(OnClickClose);
                } 
        }
        public virtual void OnClickClose()
        {
            Hide();
        }

        void Start()
        {
            Initialize(UIManager.Instance);
        }

        void ShowIcon()
        {
            _images[0].DOFade(0f, 2f);
            _images[1].DOFade(0f, 2f);
            _images[2].DOFade(0f, 2f);
        }
    }
}