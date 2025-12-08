using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ArabicSupport;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

#if Use_TMPro
using TMPro;
#endif

public static class UIExtension
{
    #region Interactable

    public static void SetInteractable(this Button bt, bool interactable)
    {
        var graphics = bt.GetComponentsInChildren<MaskableGraphic>();
        bt.interactable = interactable;
        Color targetColor = interactable ? bt.colors.normalColor : bt.colors.disabledColor;
        foreach (var g in graphics)
            g.color = targetColor;
    }

    #endregion

    #region Collection Utilities

    public static int SumRange(this IList<int> collection, int min, int max)
    {
        int total = 0;
        for (int i = min; i <= max && i < collection.Count; i++)
            total += collection[i];
        return total;
    }

    public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        int index = 0;
        foreach (var item in collection)
        {
            if (predicate(item)) return index;
            index++;
        }

        return -1;
    }

    #endregion

    #region Localization

// Đếm placeholder {0},{1}...
    private static int CountPlaceholders(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        var m = Regex.Matches(s, @"\{(\d+)(?:[^}]*)\}");
        int max = -1;
        foreach (Match mm in m)
            if (int.TryParse(mm.Groups[1].Value, out var idx) && idx > max)
                max = idx;
        return max + 1;
    }

    // ===== Helpers =====
    private static string FallbackDefault(string key, string defaultValue)
    {
        // Luôn trả về chuỗi hợp lệ
        if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;
        return string.IsNullOrEmpty(key) ? string.Empty : key;
    }

    private static string SafeConvertRTL(string s)
    {
        // ConvertRTL có thể null → phòng thủ
        return ConvertRTL(s ?? string.Empty);
    }

    private static object[] ToArgs(object maybeArgs)
    {
        if (maybeArgs == null) return null;
        if (maybeArgs is object[] arr) return arr;
        return new object[] { maybeArgs };
    }

    private static bool TryGetLocalizedString(string key, StateCapText cap, FormatText format,
        object argsOrArray, bool useParams, string defaultValue, out string result)
    {
#if use_mutil_lang
        try
        {
            if (useParams)
            {
                // Ép kiểu bắt buộc để chọn đúng overload object[]
                object[] args = argsOrArray as object[]
                                ?? (argsOrArray == null ? Array.Empty<object>() : new object[] { argsOrArray });

                string raw;
                try
                {
                    // Thử để engine format như bình thường
                    raw = MutilLanguage.getStringWithKey(key, cap, format, args, defaultValue);
                }
                catch (System.FormatException)
                {
                    // Nếu dịch sai placeholder → lấy template thô rồi tự format an toàn
                    string tmpl;
                    try
                    {
                        tmpl = MutilLanguage.getStringWithKey(key, cap, FormatText.None, null, defaultValue);
                    }
                    catch
                    {
                        result = FallbackDefault(key, defaultValue);
                        return true;
                    }

                    if (string.IsNullOrEmpty(tmpl) || string.Equals(tmpl, key, StringComparison.Ordinal))
                    {
                        result = FallbackDefault(key, defaultValue);
                        return true;
                    }

                    raw = SafeFormat(tmpl, args);
                    result = SafeConvertRTL(raw);
                    return true;
                }

                // Nếu engine trả về key/rỗng → coi như miss và fallback
                if (string.IsNullOrEmpty(raw) || string.Equals(raw, key, StringComparison.Ordinal))
                {
                    result = FallbackDefault(key, defaultValue);
                    return true;
                }

                // Nếu engine trả về template chưa format (không ném exception) → tự format thêm lần nữa (an toàn)
                if (CountPlaceholders(raw) > 0)
                    raw = SafeFormat(raw, args);

                result = SafeConvertRTL(raw);
                return true;
            }
            else
            {
                // Overload object đơn
                var raw = MutilLanguage.getStringWithKey(key, cap, format, argsOrArray, defaultValue);

                if (string.IsNullOrEmpty(raw) || string.Equals(raw, key, StringComparison.Ordinal))
                {
                    result = FallbackDefault(key, defaultValue);
                    return true;
                }

                result = SafeConvertRTL(raw);
                return true;
            }
        }
        catch (Exception)
        {
            result = FallbackDefault(key, defaultValue);
            return false;
        }
#else
    result = FallbackDefault(key, defaultValue);
    return true;
#endif
    }


// ===== TextMutil attach + resize an toàn =====
    private static void EnsureTextMutilAndResize(Text text, float? sizeRate)
    {
#if use_mutil_lang
        if (!text) return;
        var txMulti = text.GetComponent<TextMutil>() ?? text.gameObject.AddComponent<TextMutil>();
        txMulti.Initialized(false, false);
        if (sizeRate.HasValue)
        {
            // Clamp sizeRate để tránh lỗi cấu hình
            var r = Mathf.Clamp(sizeRate.Value, 0.01f, 10f);
            txMulti.Resize(r);
        }
#endif
    }

    private static void InternalSetText(Text text, string key, float? sizeRate, StateCapText cap, FormatText format,
        object formatObj, bool useParams, string defaultValue)
    {
        if (!text) return;

        // 1) Lấy template RAW (không format, không RTL)
        string template = GetTextValue(key, cap, FormatText.None, defaultValue);

        // 2) Chuẩn hoá tham số
        object[] args = NormalizeArgs(formatObj, useParams);

        // 3) Format an toàn
        string result = SafeFormat(template, args);

        // 4) Fix RTL SAU KHI format
        //   - nếu bạn đã có ConvertRTL(s) thì gọi ở đây
        //   - hoặc dùng RTLTextProcessor.FixRTLText(result) như ta bàn
        result = SafeConvertRTL(result);

#if use_mutil_lang
        var txMulti = text.GetComponent<TextMutil>() ?? text.gameObject.AddComponent<TextMutil>();
        txMulti.Initialized(false, false);
        if (sizeRate.HasValue) txMulti.Resize(Mathf.Clamp(sizeRate.Value, 0.01f, 10f));
#endif

        text.text = result;
    }

    private static object[] NormalizeArgs(object formatObj, bool useParams)
    {
        if (!useParams)
        {
            // Nhánh "object format đơn" → để null hoặc bọc 1 phần tử
            return formatObj == null ? Array.Empty<object>() : new[] { formatObj };
        }

        if (formatObj == null) return Array.Empty<object>();

        // Nếu đã là object[] thì lấy thẳng
        if (formatObj is object[] arr)
        {
            // Gỡ bọc kép: object[] { object[] {…} } → object[] {…}
            if (arr.Length == 1 && arr[0] is object[] inner) return inner;
            return arr;
        }

        // Nếu là mảng kiểu cụ thể (int[], float[]...) → cast sang object[]
        if (formatObj is System.Collections.IEnumerable e && !(formatObj is string))
            return e.Cast<object>().ToArray();

        // Trường hợp đơn lẻ
        return new[] { formatObj };
    }

    private static string SafeFormat(string fmt, object[] args)
    {
        if (string.IsNullOrEmpty(fmt)) return fmt ?? "";
        if (args == null || args.Length == 0) return fmt;

        int need = Regex.Matches(fmt, @"\{\d+\}").Count;
        if (need <= 0) return fmt;

        // Cắt bớt nếu dư tham số
        if (args.Length > need)
            args = args.Take(need).ToArray();

        try
        {
            return string.Format(CultureInfo.InvariantCulture, fmt, args);
        }
        catch
        {
            return fmt;
        } // fallback nếu placeholder lỗi
    }

    // ====== Lấy string sẵn để dùng ở nơi khác ======
    private static string GetTextValue(string key, float? sizeRate, StateCapText cap, FormatText format,
        object formatObj, bool useParams = false, string defaultValue = "")
    {
        if (string.IsNullOrEmpty(key))
            return FallbackDefault(key, defaultValue);

        // sizeRate không cần ở đây, giữ tham số cho đồng bộ chữ ký.
        TryGetLocalizedString(key, cap, format, useParams ? formatObj as object[] : formatObj,
            useParams, defaultValue, out var result);
        return result;
    }

    // 1) Không resize, object format đơn
    public static void SetText(this Text text, string key, StateCapText cap = StateCapText.None,
        FormatText format = FormatText.None, object formatObj = null, string defaultValue = "")
        => InternalSetText(text, key, null, cap, format, formatObj, useParams: false, defaultValue);

    // 2) Có resize, object format đơn
    public static void SetText(this Text text, string key, float sizeRate, StateCapText cap = StateCapText.None,
        FormatText format = FormatText.None, object formatObj = null, string defaultValue = "")
        => InternalSetText(text, key, sizeRate, cap, format, formatObj, useParams: false, defaultValue);

    // 3) Không resize, hỗ trợ params (CHUẨN – defaultValue trước params)
    public static void SetText(this Text text, string key, StateCapText cap, FormatText format,
        string defaultValue = "", params object[] formatObj)
        => InternalSetText(text, key, null, cap, format, formatObj, useParams: true, defaultValue);

    // 4) Có resize, hỗ trợ params (CHUẨN – defaultValue trước params)
    public static void SetText(this Text text, string key, float sizeRate, StateCapText cap = StateCapText.None,
        FormatText format = FormatText.None, string defaultValue = "", params object[] formatObj)
        => InternalSetText(text, key, sizeRate, cap, format, formatObj, useParams: true, defaultValue);

    // ===== GetTextValue: bản gọi nhanh (mirror SetText) =====

    // 1) Không resize, hỗ trợ params
    public static string GetTextValue(string key, StateCapText cap, FormatText format,
        string defaultValue = "", params object[] formatObj)
        => InternalGetTextValue(key, null, cap, format, formatObj, true, defaultValue);

    // 2) Có resize, hỗ trợ params
    public static string GetTextValue(string key, float sizeRate, StateCapText cap, FormatText format,
        string defaultValue = "", params object[] formatObj)
        => InternalGetTextValue(key, sizeRate, cap, format, formatObj, true, defaultValue);

    // 3) Các overload object đơn (không params)
    public static string GetTextValue(string key, StateCapText cap = StateCapText.None,
        FormatText format = FormatText.None, string defaultValue = "")
        => InternalGetTextValue(key, null, cap, format, null, false, defaultValue);

    public static string GetTextValue(string key, StateCapText cap,
        FormatText format, object formatObj, string defaultValue = "")
        => InternalGetTextValue(key, null, cap, format, formatObj, false, defaultValue);


    // ===== Internal =====

    private static string InternalGetTextValue(string key, float? sizeRate, StateCapText cap, FormatText format,
        object formatObjOrParams, bool useParams, string defaultValue)
    {
#if use_mutil_lang
        try
        {
            object argsToPass = formatObjOrParams;
            if (useParams)
            {
                if (!(formatObjOrParams is object[]))
                    argsToPass = formatObjOrParams == null ? Array.Empty<object>() : new object[] { formatObjOrParams };
            }

            // LẤY RAW theo yêu cầu format (cho template dùng FormatText.None)
            string result = MutilLanguage.getStringWithKey(key, cap, format, argsToPass, defaultValue);
            return result; // ⟵ KHÔNG ConvertRTL ở đây
        }
        catch (Exception ex)
        {
            LogError($"Localization Error: {ex}\nkey={key}");
            return defaultValue ?? key;
        }
#else
    return defaultValue ?? key;
#endif
    }
    
    public static void SetValue(this Text text, string value)
    {
        text.text = value;
        try
        {
            var txMulti = text.GetComponent<TextMutil>();
            if (txMulti == null) txMulti = text.gameObject.AddComponent<TextMutil>();
            txMulti.Initialized(false, false);
            //txMulti.setNewPosition(OffsetNewFont);
            
            text.text = ConvertRTL(value);
        }
        catch (Exception ex)
        {
            Debug.Log("mysdk: ex=" + ex.ToString());
        }
    }

    #endregion
    
    #region Time

    private static long GetUtcTime() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    private static TimeSpan GetTimeSpan(long startTime, long duration)
    {
        long elapsed = GetUtcTime() - startTime;
        return new TimeSpan(Math.Max(0, (duration - elapsed) * TimeSpan.TicksPerSecond));
    }

    public static void SetTextTime(this Text text, int seconds)
    {
        int h = seconds / 3600, m = (seconds % 3600) / 60, s = seconds % 60;
        text.text = h > 0 ? $"{h:D2}:{m:D2}:{s:D2}" : $"{m:D2}:{s:D2}";
    }

#if Use_TMPro
    public static void SetTMPTextTime(this TMP_Text tmp, int seconds)
    {
        int h = seconds / 3600, m = (seconds % 3600) / 60, s = seconds % 60;
        tmp.text = h > 0 ? $"{h:D2}:{m:D2}:{s:D2}" : $"{m:D2}:{s:D2}";
    }
#endif

    #endregion

    #region Number & Price

    public static void SetTextMoney(this Text text, long money)
    {
        text.text = SdkUtil.convertMoneyToString(money);
    }

    #endregion

    #region RTL

    public static string ConvertRTL(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return RTLTextProcessor.IsRTL(text) ? RTLTextProcessor.FixRTLText(text) : text;
    }

    #endregion

    #region Tween

    public static Tween DOFadeAllShadow(this Text text, float alpha, float duration, float timeDelay = 0, Ease type = Ease.OutQuad, object obj_ID = null, Action onComplete = null)
    {
        Shadow[] betterOutlines = text.GetComponents<Shadow>();
        var shadowAlpha = betterOutlines.Select(t => t.effectColor).ToList();
        Tween tween = text.DOFade(alpha, duration)
            .OnUpdate(() =>
            {
                if (betterOutlines != null && betterOutlines.Length > 0)
                {
                    for (var index = 0; index < betterOutlines.Length; index++)
                    {
                        var shadowColor = betterOutlines[index].effectColor;
                        var value = (text.color.a) * 0.45f;
                        shadowColor.a = Mathf.Clamp(value, 0, 1);
                        betterOutlines[index].effectColor = shadowColor;
                    }
                }
            })
            .SetDelay(timeDelay)
            .SetId(obj_ID)
            .SetEase(type)
            .OnComplete(() =>
            {
                for (var index = 0; index < betterOutlines.Length; index++)
                {
                    betterOutlines[index].effectColor = shadowAlpha[index];
                }
                onComplete?.Invoke();
            });
        return tween;

        float ColorToFloat(Color color)
        {
            return (color.r + color.g + color.b) / 3;
        }
    }


    #endregion

    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = false;
#else
        private static readonly bool ENABLE_LOGGING = false;
#endif

    private static string LogRegion = $"{nameof(UIExtension)}";

    private static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    private static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    private static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion
}