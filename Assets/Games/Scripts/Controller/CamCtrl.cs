using mygame.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    [SerializeField] private Camera mainCam;

    [SerializeField] private List<Color> colorInGame;
    public float sizeDefault { get; private set; }
    public Camera MainCam => mainCam;
    private Vector3 mainCamPosDefault = new Vector3(0, -0.25f, -10);
    private Vector3 mainCamPosIpad = new Vector3(0, -0.85f, -10);

    private Vector3 currentMainCamPos;

    public void Initialize()
    {
    }

    public void SetBackgroundColor(int backgroundID)
    {
        var id = Mathf.Clamp(backgroundID, 0, colorInGame.Count - 1);
        mainCam.backgroundColor = colorInGame[id];
    }

    public void ResizeCamera(SpriteRenderer target)
    {
        float spriteWidth = target.bounds.size.x;

        float padding = spriteWidth * 0.05f;
        float desiredWorldWidth = spriteWidth + padding * 2f;
        var pos = mainCamPosDefault;

        float requiredOrthoSize = desiredWorldWidth / (2f * mainCam.aspect);
        if (SdkUtil.isiPad())
        {
            requiredOrthoSize += 2.5f;
            pos = mainCamPosIpad;
        }

        mainCam.orthographicSize = requiredOrthoSize;
        sizeDefault = mainCam.orthographicSize;
        currentMainCamPos = pos;
        mainCam.transform.localPosition = pos;
    }
}