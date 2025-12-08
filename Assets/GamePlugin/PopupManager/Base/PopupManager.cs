using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlugins
{
    public class PopupManager : MonoBehaviour
    {
        public Canvas canvas;
        public Transform parent;
        public GameObject transparent;
        public NotifyPanel floatNotify;
        public GameObject breakAds;
        public Image fade;

        public bool usingDefaultTransparent = true;

        public BasePopup[] prefabs;

        private Transform mTransparentTrans;

        public List<BasePopup> cachePopup = new List<BasePopup>();
        public static Stack<BasePopup> refStacks = new Stack<BasePopup>();

        private int defaultSortingOrder;

        private static PopupManager mInstance;

        Dictionary<Type, string> dictionary = new Dictionary<Type, string>()
        {
        };


        public static PopupManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = LoadResource<PopupManager>("Popup/PopupManager");
                }

                return mInstance;
            }
        }

        private void Awake()
        {
            mInstance = this;
            //if (SdkUtil.isFold())
            //{
            //    canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
            //    canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 2200);
            //}
            //else

            if (SdkUtil.isiPad())
            {
                canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
                canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(2200, 1080);
            }
            else
            {
                canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
                canvas.GetComponent<CanvasScaler>().referenceResolution =
                    new Vector2(Screen.width / (float)Screen.height > 1.85f ? 1920 : 2200, 1080);
            }

            mTransparentTrans = transparent.transform;
            defaultSortingOrder = canvas.sortingOrder;
            // DontDestroyOnLoad(gameObject);
        }

        public static T ShowPopup<T>(Action showCompletedCallback = null, Action hideCompletedCallback = null,
            bool overlay = false, bool isRenderCamera = false) where T : BasePopup
        {
            AdAudioHelper.hide();
            return (T)Instance.CheckInstancePopupPrefabs<T>()
                .Show(showCompletedCallback, hideCompletedCallback, overlay, isRenderCamera);
        }

        public static bool HasPopup<T>()
        {
            GameObject gameObject = null;

            return refStacks.OfType<T>().Any();
        }

        public static bool HasPopup()
        {
            return refStacks.Count > 0 || Instance.breakAds.activeSelf;
        }

        public T CheckInstancePopupPrefabs<T>() where T : BasePopup
        {
            GameObject gameObject = null;
            if (SingletonPopup<T>.Instance != null) return SingletonPopup<T>.Instance;
            foreach (var basePopup in refStacks)
            {
                if (basePopup is T)
                {
                    return basePopup.GetComponent<T>();
                }
            }

            for (int k = 0; k < cachePopup.Count; ++k)
            {
                if (IsOfType<T>(cachePopup[k]))
                {
                    return cachePopup[k].gameObject.GetComponent<T>();
                }
            }


            for (int i = 0; i < prefabs.Length; i++)
            {
                if (IsOfType<T>(prefabs[i]))
                {
                    gameObject = Instantiate(prefabs[i].gameObject, parent);
                    break;
                }
            }

            if (gameObject == null && dictionary.TryGetValue(typeof(T), out var value))
            {
                var obj = Resources.Load<GameObject>(value);
                gameObject = Instantiate(obj, parent);
            }

            return gameObject.GetComponent<T>();
        }

        public BasePopup GetCurrentPopup()
        {
            if (refStacks.Count > 0)
            {
                return refStacks.Peek();
            }

            return null;
        }

        private bool IsOfType<T>(object value)
        {
            return value is T;
        }

        public void SetCameraDepth(int depth)
        {
            if (canvas.worldCamera != null) canvas.worldCamera.depth = depth;
        }

        public void ChangeTransparentOrder(Transform topPopupTransform, bool active)
        {
            if (active)
            {
                mTransparentTrans.SetSiblingIndex(topPopupTransform.GetSiblingIndex() - 1);
                transparent.SetActive(usingDefaultTransparent);
                // if (parent.childCount > 2)
                // {
                //     mTransparentTrans.SetSiblingIndex(topPopupTransform.GetSiblingIndex() - 2);
                // }
            }
            else if (refStacks.Count > 0)
            {
                bool hasTransparent = false;
                for (int i = 0; i < refStacks.Count; i++)
                {
                    var re = refStacks.ElementAt(i);
                    if (re.usingDefaultTransparent)
                    {
                        hasTransparent = true;
                        var sib = re.transform.GetSiblingIndex();
                        if (mTransparentTrans.GetSiblingIndex() < sib)
                        {
                            mTransparentTrans.SetSiblingIndex(Mathf.Max(sib - 1, 0));
                        }
                        else
                        {
                            mTransparentTrans.SetSiblingIndex(Mathf.Max(sib, 0));
                        }

                        break;
                    }
                }

                transparent.SetActive(hasTransparent);
            }
            else
            {
                transparent.SetActive(false);
            }
        }

        public void OnFade(Action cb)
        {
            fade.color = Color.clear;
            fade.DOFade(1, .15f).OnComplete(() =>
            {
                cb?.Invoke();
                fade.DOFade(0, .25f).SetDelay(.05f);
            });
        }

        public void RefreshNativeAds()
        {
            //foreach (var bx in AdsNativeWrapper.allNavtive)
            //{
            //    if (bx != null)
            //    {
            //        var bp = bx.GetComponentInParent<BasePopup>();
            //        if (bp != null)
            //        {
            //            var siblingIndex = bp.transform.GetSiblingIndex();
            //            var p = refStacks.FirstOrDefault(x => x != null && x.gameObject.activeSelf && x.transform.GetSiblingIndex() > siblingIndex);
            //            bx.adNativeObject.setEnableClick(siblingIndex >= mTransparentTrans.GetSiblingIndex() && p == null);
            //        }
            //        else
            //        {
            //            bx.adNativeObject.setEnableClick(!HasPopup());
            //        }
            //    }
            //}
            //Debug.Log("nysdk: RefreshNaitveAds");
        }

        public static bool SequenceHidePopup()
        {
            if (refStacks.Count > 0)
            {
                refStacks.Pop().Hide();
            }
            else
            {
                Instance.transparent.SetActive(false);
            }

            // Instance.canvas.worldCamera = null;
            return refStacks.Count > 0;
        }

        public static T LoadResource<T>(string name)
        {
            GameObject gameObject = (GameObject)Instantiate(Resources.Load(name));
            gameObject.name = $"[{name}]";
            DontDestroyOnLoad(gameObject);
            return gameObject.GetComponent<T>();
        }

        public void SetSortingOrder(int order)
        {
            canvas.sortingOrder = order;
        }

        public void ResetOrder()
        {
            canvas.sortingOrder = defaultSortingOrder;
        }

        public void Release()
        {
            for (var i = cachePopup.Count - 1; i >= 0; i--)
            {
                var cache = cachePopup[i];
                if (cache.isFixedCache) continue;
                cachePopup.Remove(cache);
                Destroy(cache.gameObject);
            }
        }

        // private void Update()
        // {
        // if (Input.GetKey(KeyCode.End))
        // {
        //     Debug.LogError("okelalal");
        //     ShowPopup<UILikeFanpage>();

        // }
        // }

        public static void ActiveCamera()
        {
            // Instance.camera.gameObject.SetActive(true);
            // Instance.canvas.worldCamera = Instance.camera;
        }

        public static void DisableCamera()
        {
            // Instance.camera.gameObject.SetActive(false);
            // Instance.canvas.worldCamera = null;
        }

        public bool ShowBreakAds(int typeShowOnPlaying)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable && !AdsHelper.isRemoveAds(0))
            {
                breakAds.SetActive(true);
                // var skeletonGraphics = breakAds.GetComponentInChildren<SkeletonGraphic>();
                // skeletonGraphics.Initialize(false);
                // skeletonGraphics.AnimationState.SetAnimation(0, "animation", false);

                DOVirtual.DelayedCall(1.4f,
                    () =>
                    {
                        AdsHelper.Instance.showFull("break_ads", DataManager.Level, -1, 0, typeShowOnPlaying, false,
                            false);
                    });
                DOVirtual.DelayedCall(2.5f, () => breakAds.SetActive(false));
                return true;
            }

            return false;
        }
    }
}