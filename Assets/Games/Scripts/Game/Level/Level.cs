using _JigblockPuzzle;
using DG.Tweening;
using mygame.sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using MyGame.Manager;
using MyGame.Tutorial;
using UnityEngine;

public class Level : MonoBehaviour
{
    private LevelConfig.LevelInfo levelInfo;
    public LevelConfig.LevelInfo LevelInfo => levelInfo;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            CompleteLevel();
        }
    }
#endif

    public void Initialize()
    {
        Conveyor.Instance.Initialize();
        TrunkManager.Instance.Initialize();
        CutterMachineManager.Instance.Initialize();
        AudioManager.Instance.PlayMusic(AudioName.Music_BackGround[levelInfo.level % 3 + 1]);
        BgMapManager.Instance.Initialize();
    }

    public void BeginLevel()
    {
        LevelManager.IsGameReady?.Invoke(true);
        //AudioManager.Instance.StopMusic();
        //AudioManager.Instance.PlayMusic("music_bg");
        if (levelInfo.level == 1)
        {
            UIManager.Instance.ShowPopup<UiTutorialLv1>();
        } else if (levelInfo.level == DataManager.Instance.GetInfoResources(RES_type.BOOSTER_1).lvUnlock ||
                   levelInfo.level == DataManager.Instance.GetInfoResources(RES_type.BOOSTER_2).lvUnlock ||
                   levelInfo.level == DataManager.Instance.GetInfoResources(RES_type.BOOSTER_3).lvUnlock)
        {
            UIManager.Instance.ShowPopup<UiTutorialBoost>();
        }
    }

    public void SetLevelInfo(LevelConfig.LevelInfo info)
    {
        
        levelInfo = info;
    }
    

    public void CompleteLevel()
    {
        LevelManager.Instance.WinGame();
        GameHelper.Instance.Vibrate(Type_vibreate.Vib_Light);
        ShowPopupWin();
    }

    public void ShowPopupWin()
    {
        DOVirtual.DelayedCall(0.5f, CallShowUIWin).SetId(this);
    }

    private void CallShowUIWin()
    {
        var uiActive = UIManager.Instance.ShowPopup<UIWinGame>();
        if (uiActive)
        {
            uiActive.Initialize(levelInfo);
        }
    }
    
    public void OnDisable()
    {
        DOTween.Kill(this);
    }
}