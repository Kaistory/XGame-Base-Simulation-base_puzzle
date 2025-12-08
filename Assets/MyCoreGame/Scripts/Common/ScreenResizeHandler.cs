using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using master;

public class ScreenResizeHandler : MonoBehaviour
{
    private Vector2 lastScreenSize;

    private void Awake()
    {
        lastScreenSize = new Vector2(Screen.width, Screen.height);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!Mathf.Approximately(lastScreenSize.x, Screen.width) ||
            !Mathf.Approximately(lastScreenSize.y, Screen.height))
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            TigerForge.EventManager.EmitEvent(EventName.OnChangeResolution);
        }
    }
}