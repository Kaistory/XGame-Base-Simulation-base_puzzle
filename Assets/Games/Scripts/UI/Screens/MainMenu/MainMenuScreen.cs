using System;
using GamePlugins;
using mygame.sdk;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Crystal;

public class MainMenuScreen : ScreenUI
{
    [System.Serializable]
    public class Group
    {
        public MainMenuPanel panel;
        public ButtonMainMenu button;
    }

    public Group[] groups;
    public int startPanel = 2;
    private int CurrentPanel = -1;
    
    public override void Initialize(UIManager uiManager)
    {
        base.Initialize(uiManager);
        for (int i = 0; i < groups.Length; i++)
        {
            var idx = i;
            groups[i].button.SetEventClick(() => ActivePanel(idx));
            groups[i].panel.Initialize(this);
        }
    }

    public override void Active()
    {
        base.Active();

        uiManager.ResetResolution();
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i].panel.Deactive();
            groups[i].button.OnDeselectButton();
            //groups[i].button.SetNoti(groups[i].panel.CheckNoti());
        }
        if (SdkUtil.isFold())
        {
            UIManager.Instance.SetCanvasScale(1);

        }
        else if (SdkUtil.isiPad())
        {
            UIManager.Instance.SetCanvasScale(1);

        }
        else
        {
            UIManager.Instance.SetCanvasScale(0);

        }
        ActivePanel(startPanel);
        // AudioManager.Instance.StopMusic();
        // AudioManager.Instance.PlayBGMusicMain(); //DangVQ
    }

    public void ActivePanel(int index)
    {
        if (index == CurrentPanel) return;
        if (CurrentPanel >= 0)
        {
            groups[CurrentPanel].panel.Deactive();
            groups[CurrentPanel].button.OnDeselectButton();
        }

        CurrentPanel = index;
        groups[CurrentPanel].panel.Active();
        groups[CurrentPanel].button.OnSelectButton();
    }

    public void ResetCurrentPanel()
    {
        CurrentPanel = -1;
    }

    // private void OnEnable()
    // {
    //     Debug.Log($"Main: {1}");
    // }
    //
    // private void OnDisable()
    // {
    //     Debug.Log($"Main: {0}");
    // }
    
}