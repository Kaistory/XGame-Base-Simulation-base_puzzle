using System;
using System.Collections.Generic;
using System.Linq;
using mygame.sdk;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlugins
{
    public class ScreenManager : MonoBehaviour
    {
        public Canvas canvas;
        public RectTransform parent;
        public BaseScreen currentScreen;
        public CanvasPropertyOverrider canvasPropertyOverrider;

        public BaseScreen[] prefabs;

        public List<BaseScreen> cacheScreen = new List<BaseScreen>();
        private List<BaseScreen> ExistScreen = new List<BaseScreen>();

        private int defaultSortingOrder;

        private Dictionary<Type, string> dictionary = new Dictionary<Type, string>()
        {

        };

        private static ScreenManager mInstance;

        public static ScreenManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = LoadResource<ScreenManager>("Screen/ScreenManager");
                }
                return mInstance;
            }
        }

        private void Awake()
        {
            mInstance = this;
            defaultSortingOrder = canvas.sortingOrder;
            getExixtScreen();
        }

        public void SetCanvasScale(float val)
        {
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = val;
            Canvas.ForceUpdateCanvases();
        }

        public void SetResolution(int width, int height)
        {
            canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(width, height);
            Canvas.ForceUpdateCanvases();
        }
        
        public void RefreshBannerArea()
        {
            canvasPropertyOverrider.UpdateCanvasProperty();
        }

        public void DisableBannerArea()
        {
            canvasPropertyOverrider.DisableBannerArea();
        }

        public static T ShowScreen<T>(Action showCompletedCallback = null, Action hideCompletedCallback = null) where T : BaseScreen
        {
            return (T)Instance.CheckInstanceScreenPrebab<T>().Show(showCompletedCallback, hideCompletedCallback);
        }

        private void getExixtScreen()
        {
            ExistScreen.Clear();
            int idxstartEnable = -1;
            bool isacmorethanone = false;
            for (int i = 0; i < parent.childCount; i++)
            {
                BaseScreen bs = parent.GetChild(i).GetComponent<BaseScreen>();
                if (bs == null) continue;
                if (bs.gameObject.activeInHierarchy)
                {
                    if (idxstartEnable == -1)
                    {
                        idxstartEnable = i;
                    }
                    else
                    {
                        isacmorethanone = true;
                    }
                }
                if (bs != null)
                {
                    ExistScreen.Add(bs);
                }
            }
            if (isacmorethanone)
            {
                for (int i = (idxstartEnable + 1); i < parent.childCount; i++)
                {
                    parent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public T CheckExistScreen<T>() where T : BaseScreen
        {
            BaseScreen gameObject;
            for (int i = 0; i < ExistScreen.Count; i++)
            {
                if (IsOfType<T>(ExistScreen[i]))
                {
                    gameObject = ExistScreen[i];
                    ExistScreen.RemoveAt(i);
                    return gameObject as T;
                }
            }
            return null;
        }
        
        public static bool HasScreen<T>() where T : BaseScreen
        {
            return SingletonScreen<T>.Instance != null;
        }

        
        public void Release()
        {
            for (var i = cacheScreen.Count - 1; i >= 0; i--)
            {
                var cache = cacheScreen[i];
                if(cache.isFixedCache) continue;
                cacheScreen.Remove(cache);
                Destroy(cache.gameObject);
            }
        }
        
        public T CheckInstanceScreenPrebab<T>() where T : BaseScreen
        {
            GameObject gameObject = null;
            if (SingletonScreen<T>.Instance != null) return SingletonScreen<T>.Instance;
            for (int k = 0; k < cacheScreen.Count; ++k)
            {
                if (IsOfType<T>(cacheScreen[k]))
                {
                    return cacheScreen[k].gameObject.GetComponent<T>();
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

        public bool Contains<T>()
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                if (IsOfType<T>(prefabs[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsOfType<T>(object value)
        {
            return value is T;
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
    }
}
