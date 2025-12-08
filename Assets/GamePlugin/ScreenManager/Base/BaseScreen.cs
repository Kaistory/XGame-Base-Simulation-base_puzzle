using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlugins
{
    public enum ScreenStatus
    {
        Foreground = 0,

        Background,
        Destroy
    }
    [RequireComponent(typeof(Animation))]
    public class BaseScreen : MonoBehaviour
    {
        [HideInInspector]
        public Animation screenanimation;

        public AnimationClip showAnimationClip;

        public AnimationClip hideAnimationClip;
        public bool isCache = false;
        public bool isFixedCache;

        [SerializeField] protected Button backBtn;
        
        private bool isCallShow = false;

        private Transform mTransform;

        private Action hideCompletedCallback;

        private Action showCompletedCallback;
        
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
            Show();
        }

        protected virtual void OnCloseClick()
        {
            Hide();
        }

        public virtual void Refresh()
        {
        }

        private void InitScreen()
        {
            if (mTransform == null)
            {
                screenanimation = GetComponent<Animation>();
                mTransform = base.transform;
                if (screenanimation != null && showAnimationClip != null && hideAnimationClip != null)
                {
                    screenanimation.AddClip(showAnimationClip, showAnimationClip.name);
                    screenanimation.AddClip(hideAnimationClip, hideAnimationClip.name);
                }
                else
                {
                    // BPDebug.LogMessage("Chưa gán Animator hoặc showAnimationClip, hideAnimationClip  cho popup " + GetType().ToString(), error: true);
                }
            }
        }
        
        public virtual void onEnableControl()
        {
            Debug.Log("mysdk: onEnableControl of " + gameObject.name);
        }

        public virtual void onDisableControl()
        {
            Debug.Log("mysdk: onDisableControl of " + gameObject.name);
        }

        public BaseScreen Show(Action showCompletedCallback = null, Action hideCompletedCallback = null)
        {
            if (isCallShow) return this;
            Debug.Log("mysdk: screen show " + gameObject.name);
            InitScreen();
            isCallShow = true;
            gameObject.SetActive(true);
            this.showCompletedCallback = showCompletedCallback;
            this.hideCompletedCallback = hideCompletedCallback;

            if (ScreenManager.Instance.currentScreen != null)
            {
                ScreenManager.Instance.currentScreen.Hide();
                ScreenManager.Instance.currentScreen = null;
            }
            
            ScreenManager.Instance.currentScreen = this;
            ScreenManager.Instance.cacheScreen.Remove(this);
            ChangeSortOrder(mTransform.parent.childCount - 1);

            AnimateShow();
            return this;
        }

        private void GotoForeground()
        {
            Debug.Log("mysdk: screen GotoForeground " + gameObject.name);
            if (screenanimation != null && showAnimationClip != null)
            {
                screenanimation.Play(showAnimationClip.name);
                float animationClipDuration = GetAnimationClipDuration(showAnimationClip);
                Invoke("OnShowFinish", animationClipDuration);
            }
        }

        private void AnimateShow()
        {
            if (screenanimation != null && showAnimationClip != null)
            {
                float animationClipDuration = GetAnimationClipDuration(showAnimationClip);
                Invoke("OnShowFinish", animationClipDuration + .1f);
                screenanimation.Play(showAnimationClip.name);
            }
            else
            {
                OnShowFinish();
            }
        }

        public virtual void OnShowFinish()
        {
            onEnableControl();
            if (showCompletedCallback != null)
            {
                showCompletedCallback();
            }
        }

        public virtual void Hide()
        {
            Debug.Log("mysdk: screen hide " + gameObject.name);
            onDisableControl();
            if (isCallShow || gameObject.activeInHierarchy)
            {
                isCallShow = false;
                AnimateHide();
            }
        }

        private void AnimateHide()
        {
            if (screenanimation != null && hideAnimationClip != null)
            {
                screenanimation.Play(hideAnimationClip.name);
                float animationClipDuration = GetAnimationClipDuration(hideAnimationClip);
                if (Time.timeScale != 0)
                {
                    Invoke("Destroy", animationClipDuration);
                }
                else
                {
                    Destroy();
                }
            }
            else
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            if (this.isCache)
            {
                base.gameObject.SetActive(false);
                if (!ScreenManager.Instance.cacheScreen.Contains(this))
                {
                    ScreenManager.Instance.cacheScreen.Add(this);
                }
                else
                {
                    Debug.Log("mysdk: screen Destroy " + gameObject.name + ", cache follow err");
                }
            }
            else
            {
                DestroyImmediate(base.gameObject);
            }
            ScreenManager.Instance.currentScreen = null;
            hideCompletedCallback?.Invoke();
            ScreenManager.Instance.ResetOrder();
        }
        
        public void ChangeSortOrder(int newSortOrder = -1)
        {
            if (newSortOrder != -1)
            {
                mTransform.SetSiblingIndex(newSortOrder);
            }
        }


        private float GetAnimationClipDuration(AnimationClip clip)
        {
            if (screenanimation != null && clip != null)
            {
                return screenanimation.GetClip(clip.name).length;
            }

            return 0f;
        }
    }
}
