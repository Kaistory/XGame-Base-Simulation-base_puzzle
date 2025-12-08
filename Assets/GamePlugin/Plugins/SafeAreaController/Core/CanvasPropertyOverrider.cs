using System;
using mygame.sdk;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CanvasPropertyOverrider : MonoBehaviour
{
    /// <summary>
    ///  1  = áp SafeArea ở TOP (chừa phần tai thỏ trên)
    /// -1  = áp SafeArea ở BOTTOM (chừa phần gesture/bottom)
    ///  0  = không tác động SafeArea (giữ full)
    /// </summary>
    public int safeAreaCanvas = 2;

    public bool isFlipBanner = false; // true = banner TOP, false = BOTTOM
    public bool isBannerCanvas = true; // panel này có chừa chỗ cho banner?

    private RectTransform _rt;

    protected void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    protected void OnEnable()
    {
        TigerForge.EventManager.StartListening(EventName.OnPurchaseInapp, UpdateCanvasProperty);
        TigerForge.EventManager.StartListening(EventName.OnRemoveAdsAction, UpdateCanvasProperty);
    }

    protected void OnDisable()
    {
        TigerForge.EventManager.StopListening(EventName.OnPurchaseInapp, UpdateCanvasProperty);
        TigerForge.EventManager.StopListening(EventName.OnRemoveAdsAction, UpdateCanvasProperty);
    }

    private void Start()
    {
        UpdateCanvasProperty();
    }

#if UNITY_EDITOR
    private void Update()
    {
        // Cho Unity Simulator: device thay đổi → cập nhật tức thời
        if (!Application.isPlaying) UpdateCanvasProperty();
    }
#endif

    public void UpdateCanvasProperty()
    {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!_rt) return;

        // Nếu safeAreaCanvas = 0 => ép full, bỏ qua banner
        if (safeAreaCanvas == 0)
        {
            SetFullRect(_rt);
            return;
        }

        // Ngược lại, áp SafeArea trước rồi mới chừa banner (nếu cần)
        ApplySafeArea(_rt, safeAreaCanvas);
        if (isBannerCanvas)
        {
            ApplyBannerPadding(_rt, isFlipBanner);
        }
    }


    private void SetFullRect(RectTransform rect)
    {
        if (!rect)
        {
            return;
        }

        // Luôn reset trước để đảm bảo full
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }


    /// <summary>
    /// Áp safe area theo hướng. Yêu cầu panel stretch full (anchorMin=0, anchorMax=1).
    /// </summary>
    private void ApplySafeArea(RectTransform t, int mode)
    {
        // Luôn reset trước để đảm bảo full
        SetFullRect(t);

        // Nếu không áp safe area → giữ full
        if (mode == 0) return;

        Rect sa = Screen.safeArea;
        Vector2 screen = new Vector2(Screen.width, Screen.height);

        float left = sa.xMin / screen.x;
        float right = 1f - (sa.xMax / screen.x);
        float bottom = sa.yMin / screen.y;
        float top = 1f - (sa.yMax / screen.y);

        Vector2 amin = t.anchorMin;
        Vector2 amax = t.anchorMax;

        if (mode == 1) amax.y = 1f - top;
        else if (mode == -1) amin.y = bottom;
        else if (mode == 2)
        {
            amin.y = bottom;
            amax.y = 1f - top;
            amin.x = left;
            amax.x = 1f - right;
        }

        t.anchorMin = amin;
        t.anchorMax = amax;
        t.offsetMin = Vector2.zero;
        t.offsetMax = Vector2.zero;
    }


    /// <summary>
    /// Chừa khoảng trống cho banner theo pixel height của SDK.
    /// isFlipBanner: true=TOP, false=BOTTOM
    /// </summary>
    private void ApplyBannerPadding(RectTransform t, bool topBanner)
    {
        // Nếu đang ở chế độ full (safeAreaCanvas = 0) => không chừa banner, trả full
        if (safeAreaCanvas == 0)
        {
            var amin = t.anchorMin;
            var amax = t.anchorMax;
            amin.y = 0f;
            amax.y = 1f;
            t.anchorMin = amin;
            t.anchorMax = amax;
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
            return;
        }

        bool showBanner = !AdsHelper.isRemoveAds(0) && RemoteConfigure.CFEnableBanner;
        if (!showBanner)
        {
            // trả về full
            var amin = t.anchorMin;
            var amax = t.anchorMax;
            amin.y = 0f;
            amax.y = 1f;
            t.anchorMin = amin;
            t.anchorMax = amax;
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
            Log($"ApplyBannerPadding remove ads or disable banner");
            return;
        }

        float bannerPx = Mathf.Max(0, SdkUtil.GetSizeBanner());
        float norm = bannerPx / Mathf.Max(1f, (float)Screen.height);

        var _amin = t.anchorMin;
        var _amax = t.anchorMax;
        if (topBanner) _amax.y = Mathf.Clamp01(1f - norm);
        else _amin.y = Mathf.Clamp01(norm);

        t.anchorMin = _amin;
        t.anchorMax = _amax;
        t.offsetMin = Vector2.zero;
        t.offsetMax = Vector2.zero;
        Log($"ApplyBannerPadding showBanner with norm: {norm}");
    }


    public void DisableBannerArea()
    {
        if (_rt == null) _rt = GetComponent<RectTransform>();
        _rt.anchorMin = new Vector2(0, 0);
        _rt.anchorMax = new Vector2(1, 1);
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;
    }


    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = false;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    private static string LogRegion = $"{nameof(CanvasPropertyOverrider)}";

    public static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    public static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    public static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion
}