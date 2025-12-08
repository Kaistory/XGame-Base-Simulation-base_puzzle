using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using mygame.sdk;

public class UIManager : master.Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject blockerUI;
    [SerializeField] private Image fadeImage;
    [SerializeField] private RectTransform screenHolder;
    [SerializeField] private RectTransform popupHolder;
    [SerializeField] private NotifyPanel notifyPanel;
    [SerializeField] private TransitionUI transitionUI;
    [SerializeField] private Camera uiCamera;

    private List<PopupUI> listPopupCached = new List<PopupUI>();
    private List<PopupUI> listPopupExist = new List<PopupUI>();
    private List<ScreenUI> listScreenCached = new List<ScreenUI>();
    private List<ScreenUI> listScreenExist = new List<ScreenUI>();

    public List<PopupUI> ListPopupCached => listPopupCached;
    public ScreenUI CurrentScreen { get; private set; }
    public PopupUI CurrentPopup { get; private set; }

    public List<PopupUI> ListPopupExist => listPopupExist;

    public Canvas Canvas => canvas;

    public Camera UICamera => uiCamera;

    protected override void Awake()
    {
        base.Awake();
        blockerUI.SetActive(false);
        ResetResolution();
        /*  for (int i = 0; i < listScreenCached.Count; i++)
          {
              listScreenCached[i].Initialize(this);
          }
          for (int i = 0; i < listPopupCached.Count; i++)
          {
              listPopupCached[i].Initialize(this);
          }*/
        /*listPopupExist = new List<PopupUI>(listPopupCached);
        listScreenExist = new List<ScreenUI>(listScreenCached);*/
        PopupUI.OnDestroyPopup += OnPopupDestroyed;
        ScreenUI.OnDestroyScreen += OnDestroyScreen;
        transitionUI.gameObject.SetActive(false);
    }

    public void BlockUI(bool active)
    {
        blockerUI.SetActive(active);
    }

    public void ResetResolution()
    {
        if (SdkUtil.isFold())
        {
            canvasScaler.matchWidthOrHeight = 1f;
            canvasScaler.referenceResolution = new Vector2(1080, 2200);
        }
        else if (SdkUtil.isiPad())
        {
            canvasScaler.matchWidthOrHeight = 1f;
            canvasScaler.referenceResolution = new Vector2(1080, 2200);
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0f;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }
    }

    public bool ShowBreakAds(int typeShowOnPlaying)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable && !AdsHelper.isRemoveAds(0))
        {
            AudioManager.Instance.SetCacheAudio();
            var ss = AdsHelper.Instance.showFull("break_ads", DataManager.Level, -1, 0, 0, true, false, true,
                state =>
                {
                    if (state == AD_State.AD_REWARD_OK || state == AD_State.AD_CLOSE || state == AD_State.AD_CLOSE2 ||
                        state == AD_State.AD_SHOW_MISS_CB || state == AD_State.AD_SHOW_FAIL ||
                        state == AD_State.AD_SHOW_FAIL2)
                    {
                        AudioManager.Instance.ResetAudio();
                    }
                });
            return true;
        }

        return false;
    }

    public void SetCanvasScale(float val)
    {
        canvasScaler.matchWidthOrHeight = val;
        Canvas.ForceUpdateCanvases();
    }

    public void SetResolution(int width, int height)
    {
        canvasScaler.referenceResolution = new Vector2(width, height);
        Canvas.ForceUpdateCanvases();
    }

    public void NotifyContent(string content, string key = "", float number = 0)
    {
        notifyPanel.ShowNotify(content, key, number);
    }

    public void NotifyContent2(string content, string key = "", string objFormat = "")
    {
        notifyPanel.ShowNotify(content, key, objFormat);
    }

    public void ShowNotifyNotEnoughResource(RES_type type)
    {
        string key = type.ToReadableString(StringCase.LowerCase);
        var formatObj = UIExtension.GetTextValue(key);
        var defaultValue = $"You don't have enough {key}.";

        // Đã có string hoàn chỉnh => dùng Raw để không qua lookup lần nữa
        NotifyContent2(defaultValue, "not_enough_resource_x", formatObj);

        if (RemoteConfigure.CFShowShopWhenNotEnoughResource)
        {
            GameManager.Instance.ShowShopUI();
        }
    }

    public TransitionUI Transition(bool autoFade, TweenCallback onload, TweenCallback complete,
        TweenCallback doneTransition = null, bool canPlayFadeSound = false)
    {
        transitionUI.Transition(onload, complete, doneTransition,
            canPlayFadeSound);
        if (autoFade) transitionUI.Fade();
        return transitionUI;
    }

    public void OnPopupDestroyed(PopupUI obj)
    {
        if (listPopupCached.Contains(obj))
        {
            listPopupCached.Remove(obj);
        }

        listPopupExist.Remove(obj);
        Destroy(obj.gameObject);
    }

    public void OnDestroyScreen(ScreenUI screen)
    {
        if (listScreenCached.Contains(screen))
        {
            listScreenCached.Remove(screen);
        }

        if (listScreenExist.Contains(screen))
        {
            listScreenExist.Remove(screen);
        }

        Destroy(screen.gameObject);
    }

    public T ShowScreen<T>() where T : ScreenUI
    {
        if (CurrentScreen)
        {
            CurrentScreen.Deactive();
        }

        for (int i = 0; i < listScreenCached.Count; i++)
        {
            if (listScreenCached[i] is T)
            {
                CurrentScreen = listScreenCached[i];
                listScreenCached[i].Active();
                listScreenCached[i].transform.SetAsLastSibling();
                SetFirstScreen();
                return listScreenCached[i].GetComponent<T>();
            }
        }

        T screen = CreateScreen<T>();
        CurrentScreen = screen;
        screen.Active();
        screen.transform.SetAsLastSibling();
        SetFirstScreen();
        return screen;
    }

    private T CreateScreen<T>() where T : ScreenUI
    {
        string screenName = typeof(T).Name;
        T screen = Instantiate(Resources.Load<T>("UI/Screens/" + screenName), screenHolder);
        listScreenExist.Add(screen);
        if (screen.isCache)
        {
            listScreenCached.Add(screen);
        }

        screen.Initialize(this);
        return screen;
    }

    public T GetScreen<T>() where T : ScreenUI
    {
        T screen = default;
        for (int i = 0; i < listScreenExist.Count; i++)
        {
            if (listScreenExist[i] is T)
            {
                screen = listScreenExist[i].GetComponent<T>();
                return screen;
            }
        }

        screen = CreateScreen<T>();
        return screen;
    }

    public T GetScreenActive<T>() where T : ScreenUI
    {
        T screen = default;
        for (int i = 0; i < listScreenExist.Count; i++)
        {
            if (listScreenExist[i] is T)
            {
                screen = listScreenExist[i].GetComponent<T>();
                return screen;
            }
        }

        return screen;
    }

    public T ShowPopup<T>(System.Action onClose = null, bool isForceNew = false) where T : PopupUI
    {
        for (int i = 0; i < listPopupCached.Count; i++)
        {
            if (listPopupCached[i] is T)
            {
                listPopupCached[i].Show(onClose);
                listPopupCached[i].transform.SetAsLastSibling();
                CurrentPopup = listPopupCached[i];
                SetFirstScreen();
                return listPopupCached[i].GetComponent<T>();
            }
        }

        if (!isForceNew)
        {
            for (int i = 0; i < listPopupExist.Count; i++)
            {
                if (listPopupExist[i] is T)
                {
                    listPopupExist[i].Show(onClose);
                    listPopupExist[i].transform.SetAsLastSibling();
                    CurrentPopup = listPopupExist[i];
                    SetFirstScreen();
                    return listPopupExist[i].GetComponent<T>();
                }
            }
        }

        T popup = CreatePopup<T>();
        popup.Show(onClose);
        popup.transform.SetAsLastSibling();
        CurrentPopup = popup;
        SetFirstScreen();
        return popup;
    }

    private T CreatePopup<T>() where T : PopupUI
    {
        string popupName = typeof(T).Name;
        T popup = Instantiate(Resources.Load<T>("UI/Popups/" + popupName), popupHolder);
        listPopupExist.Add(popup);
        if (popup.isCache)
        {
            listPopupCached.Add(popup);
        }

        popup.Setup();
        return popup;
    }

    public T GetPopup<T>() where T : PopupUI
    {
        T popup = default;
        for (int i = 0; i < listPopupCached.Count; i++)
        {
            if (listPopupCached[i] is T)
            {
                popup = listPopupCached[i].GetComponent<T>();
                return popup;
            }
        }

        for (int i = 0; i < listPopupExist.Count; i++)
        {
            if (listPopupExist[i] is T)
            {
                popup = listPopupExist[i].GetComponent<T>();
                return popup;
            }
        }

        popup = CreatePopup<T>();
        return popup;
    }

    public void HideAllPopup()
    {
        for (int i = 0; i < listPopupCached.Count; i++)
        {
            if (listPopupCached[i])
            {
                listPopupCached[i].Hide();
            }
        }

        for (int i = 0; i < listPopupExist.Count; i++)
        {
            if (listPopupExist[i])
            {
                listPopupExist[i].Hide();
            }
        }
    }

    public T GetPopupActive<T>() where T : PopupUI
    {
        T popup = default;
        for (int i = 0; i < listPopupCached.Count; i++)
        {
            if (listPopupCached[i] is T)
            {
                popup = listPopupCached[i].GetComponent<T>();
                return popup;
            }
        }

        for (int i = 0; i < listPopupExist.Count; i++)
        {
            if (listPopupExist[i] is T)
            {
                popup = listPopupExist[i].GetComponent<T>();
                return popup;
            }
        }

        return popup;
    }

    public bool HasPopupShowing()
    {
        foreach (var item in listPopupExist)
        {
            if (item.isShowing) return true;
        }

        return false;
    }

    public PopupUI GetPopupCached<T>() where T : PopupUI
    {
        T popup = default;
        for (int i = 0; i < listPopupCached.Count; i++)
        {
            if (listPopupCached[i] is T)
            {
                popup = listPopupCached[i].GetComponent<T>();
                return popup;
            }
        }

        return popup;
    }

    public void AddPopupCache(PopupUI popup)
    {
        listPopupCached.Add(popup);
    }

    public void ClearPopupCache(PopupUI popup)
    {
        listPopupExist.Remove(popup);
        listPopupCached.Remove(popup);
    }

    public void SetFirstScreen()
    {
        if (!SDKManager.Instance) return;
        var str = "";
        if (CurrentPopup && CurrentPopup.gameObject.activeInHierarchy)
        {
            str = CurrentPopup.gameObject.name;
        }
        else if (CurrentScreen)
        {
            str = CurrentScreen.gameObject.name;
        }

        if (string.IsNullOrEmpty(str))
        {
            SDKManager.Instance.currPlacement = "default";
            return;
        }

        var f = PascalToSnake(str);
        f = f.Replace("(Clone)", "").Replace("(clone)", "");
        SDKManager.Instance.currPlacement = f;
        //Debug.Log($"aaaaaaaaaaaaa={f}");
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    public static string PascalToSnake(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        var result = Regex.Replace(input, @"([A-Z]+)([A-Z][a-z])", "$1_$2");
        result = Regex.Replace(result, @"([a-z0-9])([A-Z])", "$1_$2");
        result = Regex.Replace(result, @"([A-Z])([a-z])_([A-Z])", "$1$2$3");
        return result.ToLower();
    }

    // ========= ADS RESOLUTION =========
    public void SetScreenAfterAds()
    {
    }

    private bool IsCurrentResolutionMatch(int width, int height, int tolerance = 2)
    {
        return Mathf.Abs(Screen.width - width) <= tolerance && Mathf.Abs(Screen.height - height) <= tolerance;
    }
}

public static class CanvasPositioningExtensions
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width, screenPosition.y / Screen.height, 0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}