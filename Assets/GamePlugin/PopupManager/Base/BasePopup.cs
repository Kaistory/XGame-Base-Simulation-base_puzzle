using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlugins
{
    [RequireComponent(typeof(Animation))]
    public class BasePopup : MonoBehaviour
    {
        [HideInInspector]
        public Animation animation;

        public AnimationClip showAnimationClip;

        public AnimationClip hideAnimationClip;
        
        public bool isCache;
        
        public bool isFixedCache;
        public bool isDisableAutoHide;
        
        public bool usingDefaultTransparent = true; 
        
        [SerializeField] protected Button backBtn;
        

        protected bool isShowed;

        private Transform mTransform;

        protected Action hideCompletedCallback;

        protected Action showCompletedCallback;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (showAnimationClip != null || hideAnimationClip != null)
            {
                animation = GetComponent<Animation>();
                if (animation == null) animation = gameObject.AddComponent<Animation>();
                if (showAnimationClip != null) animation.AddClip(showAnimationClip, showAnimationClip.name);
                if (hideAnimationClip != null) animation.AddClip(hideAnimationClip, hideAnimationClip.name);
                animation.playAutomatically = false;
                if (GetComponent<CanvasGroup>() == null) gameObject.AddComponent<CanvasGroup>();
            }
        }
#endif
        
        protected virtual void Awake()
        {
        }
        
        private void Start()
        {
            SetupUI();
        }

        protected virtual void SetupUI()
        {
            if (backBtn != null)
            {
                backBtn.onClick.AddListener(OnCloseClick);
            }
        }
        
        public virtual void Refresh()
        {
        }
        
        protected void InitScreen()
        {
            animation = GetComponent<Animation>();
            mTransform = transform;
            if (animation != null && showAnimationClip != null && hideAnimationClip != null)
            {
                animation.AddClip(showAnimationClip, showAnimationClip.name);
                animation.AddClip(hideAnimationClip, hideAnimationClip.name);
            }
        }

        public BasePopup Show(Action showCompletedCallback = null, Action hideCompletedCallback = null, bool overlay = false, bool isRenderCamera = false)
        {
            Debug.Log("mysdk: popup Show " + gameObject.name);
            InitScreen();
            if (isRenderCamera) PopupManager.ActiveCamera();
            else PopupManager.DisableCamera();
            // if (isShowed)
            // {
            //     Reshow();
            //     UICamera.uiChange?.Invoke();
            //     return this;
            // }
            this.showCompletedCallback = showCompletedCallback;
            this.hideCompletedCallback = hideCompletedCallback;
            float waitTime = 0f;
            isShowed = true;
            if (!overlay && PopupManager.refStacks.Count > 0)
            {
                ForceHideAllCurrent(ref waitTime);
            }
            PopupManager.Instance.cachePopup.Remove(this);
            ChangeSortOrder(mTransform.parent.childCount - 1);
            if (!PopupManager.refStacks.Contains(this))
            {
                PopupManager.refStacks.Push(this);
            }
            else
            {
                MoveElementToTopStack();
            }
            if (waitTime != 0f)
            {
                Invoke("AnimateShow", waitTime);
            }
            else
            {
                AnimateShow();
            }
            return this;
        }

        
        private void Reshow()
        {
            Debug.Log("mysdk: popup Reshow " + gameObject.name);
            ChangeSortOrder(mTransform.parent.childCount - 1);
            if (animation != null && showAnimationClip != null)
            {
                animation.Play(showAnimationClip.name);
                float animationClipDuration = GetAnimationClipDuration(showAnimationClip);
                Invoke("OnShowFinish", animationClipDuration);
            }
            if (usingDefaultTransparent)
            {
                PopupManager.Instance.ChangeTransparentOrder(mTransform, usingDefaultTransparent);
            }
        }

        private void AnimateShow()
        {
            gameObject.SetActive(true);
            if (animation != null && showAnimationClip != null && Time.timeScale > 0)
            {
                // float animationClipDuration = GetAnimationClipDuration(showAnimationClip);
                // Invoke("OnShowFinish", animationClipDuration + .1f);
                animation.Play(showAnimationClip.name);
            }
            // else
            // {
            // }
            if (usingDefaultTransparent)
            {
                PopupManager.Instance.ChangeTransparentOrder(mTransform, usingDefaultTransparent);
            }
            OnShowFinish();
            PopupManager.Instance.RefreshNativeAds();
        }

        public virtual void OnShowFinish()
        {
            showCompletedCallback?.Invoke();
        }

        public virtual void Hide(bool isRenderCamera)
        {
            if(this == null || gameObject == null) return;
            Debug.Log("mysdk: popup hide " + gameObject.name);
            if (isRenderCamera) PopupManager.ActiveCamera();
            else PopupManager.DisableCamera();
            if (isShowed)
            {
                isShowed = false;
                AnimateHide();
            }
        }
        
        public virtual void Hide()
        {
            Hide(false);
        }
        protected string soundClosePopup = "SFX_UI_Button_Click_Close";
        protected virtual void OnCloseClick()
        {
            if (string.IsNullOrEmpty(soundClosePopup))
            {
                soundClosePopup = "SFX_UI_Button_Click_Close";
            }
            // AudioManager.Instance.PlayOneShot(soundClosePopup);
            Hide();
        }

        private void AnimateHide()
        {
            if (animation != null && hideAnimationClip != null)
            {
                animation.Play(hideAnimationClip.name);
                float animationClipDuration = GetAnimationClipDuration(hideAnimationClip);
                if (Time.timeScale != 0) Invoke("Destroy", animationClipDuration);
                else Destroy();
            }
            else
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            Debug.Log("mysdk: popup Destroy " + gameObject.name);

            var reversedStack = new Stack<BasePopup>();

            while (PopupManager.refStacks.Count > 0)
            {
                var topItem = PopupManager.refStacks.Pop();
                if (this != topItem)
                {
                    reversedStack.Push(topItem);
                }
            }

            while (reversedStack.Count > 0)
            {
                PopupManager.refStacks.Push(reversedStack.Pop());
            }
            
            if (isCache)
            {
                gameObject.SetActive(false);
                if (!PopupManager.Instance.cachePopup.Contains(this))
                {
                    PopupManager.Instance.cachePopup.Add(this);
                }
                else
                {
                    Debug.Log("mysdk: popup Destroy " + gameObject.name + ", cache follow err");
                }
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    DestroyImmediate(gameObject);
                }
            }

            PopupManager.Instance.ResetOrder();
            
            if (usingDefaultTransparent)
            {
                PopupManager.Instance.ChangeTransparentOrder(mTransform, false);
            }
            hideCompletedCallback?.Invoke();
            PopupManager.Instance.RefreshNativeAds();
        }

        public void ChangeSortOrder(int newSortOrder = -1)
        {
            if (newSortOrder != -1)
            {
                mTransform.SetSiblingIndex(newSortOrder);
            }
        }

        private void ForceHideAllCurrent(ref float waitTime)
        {
            Debug.Log("mysdk: popup ForceHideAllCurrent " + gameObject.name);

            for (int i = 0; i < PopupManager.refStacks.Count; i++)
            {
                if (PopupManager.refStacks.Peek().isDisableAutoHide) continue;
                BasePopup basePopup = PopupManager.refStacks.Pop();
                waitTime += basePopup.GetAnimationClipDuration(basePopup.hideAnimationClip);
                basePopup.Hide();
            }
        }

        private float GetAnimationClipDuration(AnimationClip clip)
        {
            if (animation != null && clip != null)
            {
                return animation.GetClip(clip.name).length;
            }

            return 0f;
        }

        private void MoveElementToTopStack()
        { 
            var reversedStack = new Stack<BasePopup>();
            while (PopupManager.refStacks.Count > 0)
            {
                var topItem = PopupManager.refStacks.Pop();
                if (this != topItem)
                {
                    reversedStack.Push(topItem);
                }
            }

            while (reversedStack.Count > 0)
            {
                PopupManager.refStacks.Push(reversedStack.Pop());
            }
            reversedStack.Push(this);
        }
    }
}
