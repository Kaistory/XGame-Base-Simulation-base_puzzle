using System;
using _JigblockPuzzle;
using UnityEngine;
using UnityEngine.UI;

public class SettingElement : MonoBehaviour
{
    [SerializeField] private eSettingElement settingElement;
    [SerializeField] private ToggleSwitch toggle;
    [SerializeField] private Text text;

    private void Start()
    {
        toggle.BtnInteract.onClick.AddListener(ChangeSetting);
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        toggle.Initialize(DataManager.GetSetting(settingElement));
        Refresh();
    }

    private void ChangeSetting()
    {
        var value = DataManager.GetSetting(settingElement);
        DataManager.ChangeSetting(settingElement, !value);
        Refresh();
    }

    public void Refresh()
    {
        var value = DataManager.GetSetting(settingElement);
        toggle.Toggle(value);
        text.SetText(key: settingElement.ToReadableString(StringCase.LowerCase), defaultValue:settingElement.ToReadableString());
    }
}


public enum eSettingElement
{
    Sound = 0,
    Music = 1,
    Vibration = 2
}