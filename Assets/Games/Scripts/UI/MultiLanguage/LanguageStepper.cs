using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageStepper : MonoBehaviour
{
    [Header("UI")] [SerializeField] private Button btnLeft;
    [SerializeField] private Button btnRight;
    [SerializeField] private Text label; // dùng TMP thì đổi sang TextMeshProUGUI

    [Header("Behavior")] [Tooltip("Bật: vòng lặp đầu/cuối. Tắt: không vòng, khoá nút ở mép.")]
    public bool wrapAround = true;

    [Header("Options (code → label)")] public List<LangOption> options = new List<LangOption>
    {
        new LangOption(MutilLanguage.langDefault, "English"),
        new LangOption("es", "Español"),
        new LangOption("pt", "Português"),
        new LangOption("fr", "Français"),
        new LangOption("de", "Deutsch"),
        new LangOption("ru", "Русский"),
        new LangOption("ar", "العربية"),
        new LangOption("hi", "हिन्दी"),
        new LangOption("ja", "日本語"),
        new LangOption("ko", "한국어"),
        new LangOption("vi", "Tiếng Việt"),
        new LangOption("tr", "Türkçe"),
        new LangOption("th", "ไทย"),
        new LangOption("id", "Bahasa Indonesia"),
    };

    private int _currentIndex = -1;
    private const string PrefKey = "mem_set_lang";

    private string SavedCode
    {
        get => PlayerPrefs.GetString(PrefKey, MutilLanguage.langDefault);
        set
        {
            PlayerPrefs.SetString(PrefKey, value);
            MutilLanguage.Instance()?.setLang(value);
            TigerForge.EventManager.EmitEvent(EventName.ChangeLanguage);
        }
    }

    private void Awake()
    {
        if (btnLeft) btnLeft.onClick.AddListener(OnLeft);
        if (btnRight) btnRight.onClick.AddListener(OnRight);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (options == null || options.Count == 0)
        {
            UpdateButtonsState();
            return;
        }

        var idx = IndexOfCode(SavedCode);
        if (idx < 0) idx = IndexOfCode(MutilLanguage.langDefault);
        if (idx < 0) idx = 0;

        SetIndex(idx, applyLanguage: false);
        ApplyLanguageByIndex(idx);
    }

    private int IndexOfCode(string code)
    {
        for (int i = 0; i < options.Count; i++)
            if (string.Equals(options[i].code, code, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    private void OnLeft()
    {
        if (options.Count == 0) return;

        int next;
        if (wrapAround)
            next = (_currentIndex - 1 + options.Count) % options.Count;
        else
            next = Mathf.Max(0, _currentIndex - 1);

        SetIndex(next, applyLanguage: true);
    }

    private void OnRight()
    {
        if (options.Count == 0) return;

        int next;
        if (wrapAround)
            next = (_currentIndex + 1) % options.Count;
        else
            next = Mathf.Min(options.Count - 1, _currentIndex + 1);

        SetIndex(next, applyLanguage: true);
    }

    private void SetIndex(int newIndex, bool applyLanguage)
    {
        _currentIndex = Mathf.Clamp(newIndex, 0, Mathf.Max(0, options.Count - 1));
        UpdateLabel(options.Count > 0 ? options[_currentIndex].label : "-");
        UpdateButtonsState();
        if (applyLanguage) ApplyLanguageByIndex(_currentIndex);
    }

    private void UpdateLabel(string text)
    {
        if (label) label.text = text; // TMP: label.SetText(text);
    }

    private void UpdateButtonsState()
    {
        if (!btnLeft || !btnRight) return;

        if (options == null || options.Count <= 1)
        {
            // 0–1 lựa chọn: ẩn luôn
            btnLeft.gameObject.SetActive(false);
            btnRight.gameObject.SetActive(false);
            return;
        }

        if (wrapAround)
        {
            // vòng lặp: luôn hiện
            btnLeft.gameObject.SetActive(true);
            btnRight.gameObject.SetActive(true);
        }
        else
        {
            // không vòng: ẩn ở mép
            btnLeft.gameObject.SetActive(_currentIndex > 0);
            btnRight.gameObject.SetActive(_currentIndex < options.Count - 1);
        }
    }


    private void ApplyLanguageByIndex(int index)
    {
        if (index < 0 || index >= options.Count) return;
        var code = options[index].code;
        SavedCode = code;
        Debug.Log($"LanguageStepper ApplyLanguage idx={index} code={code}");
    }

    [Serializable]
    public struct LangOption
    {
        public string code;
        public string label;

        public LangOption(string code, string label)
        {
            this.code = code;
            this.label = label;
        }
    }
}