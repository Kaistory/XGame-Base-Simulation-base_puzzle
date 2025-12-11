using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Tutorial
{
    public class UiTutorialLv1 : PopupUI
    {
        [SerializeField] private Text _text; 
        public string content = "Click to feed trunk onto the conveyor belt.";
        public virtual void Initialize(UIManager manager)
        {
            
            this.uiManager = manager;
            isShowing = false;
            _text.DOText(content, 3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (ButtonClose != null)
                {
                    ButtonClose.onClick.AddListener(OnClickClose);
                } 
            });
        }
        public virtual void OnClickClose()
        {
            Hide();
        }

        void Start()
        {
            Initialize(UIManager.Instance);
        }
    }
}