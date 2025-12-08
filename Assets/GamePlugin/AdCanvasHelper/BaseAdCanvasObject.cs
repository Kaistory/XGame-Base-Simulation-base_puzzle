using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace mygame.sdk
{

    public enum AdCanvasSize
    {
        Size6x5 = 0,
        Size32x5,
        Size4x3Video,
        Size6x5Video,
        Size300x600,
        Size320x480,
        Size960x540,
        Size970x250
    }

    public abstract class BaseAdCanvasObject : MonoBehaviour
    {
        public BaseAdCanvas adCanvasHelper { get; private set; }
        public AdCanvasSize adType;
        public string placement;
        public bool isHideBillboard = false;
        public bool isHideDefault = false;
        public Vector2 scaleAdSize = Vector2.one;
        public Vector3 rotationAd = Vector3.zero;

        [HideInInspector] public Vector3 pos;
        [HideInInspector] public Vector3 forward;
        [HideInInspector] public Transform target;
        [HideInInspector] public int stateLoockat = 0;
        [HideInInspector] public float defaultYBillBoard = -10000;

        public virtual void initAds(BaseAdCanvas adhelper, string placement, Texture2D ttdf)
        {
            adCanvasHelper = adhelper;
            this.placement = placement;
        }

        public virtual void setPlacement(string placement)
        {
            this.placement = placement;
        }

        public virtual void onchangeCamera(Camera cam) { }
        public virtual void initClick(bool isClick) { }

        public abstract void onUpdate();
        public abstract bool isLoaded();
        public abstract void enableMesh();
        public abstract void setFollowY(bool isfl, float yTaget = -10000);
        public abstract void hideBillboards(bool isHide);
        public abstract void hideDefault(bool isHide);
        public abstract void setScaleAd(Vector2 scale);
        public abstract void setRotation(Vector3 rot);
    }

    [Serializable]
    public class BaseAdInfo
    {
        public Renderer mesh;
        public Renderer defaultMesh;
#if ENABLE_Adverty
        [HideInInspector] public Adverty5.AdPlacements.AdPlacement targetAdPlacement = null;
#endif
        [HideInInspector] public bool isLoadDefault = false;
        [HideInInspector] public bool isAdLoaded;
        [HideInInspector] public Vector3 defaultScaleFace = Vector2.zero;
    }
}
