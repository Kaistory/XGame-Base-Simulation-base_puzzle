using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainMenuPanel : MonoBehaviour
{
    protected ScreenUI mainScreenUI;
    public virtual void Initialize(ScreenUI screenUI)
    {
        mainScreenUI = screenUI;
    }
    public virtual void Active()
    {
        gameObject.SetActive(true);
    }
    public virtual void Deactive()
    {
        gameObject.SetActive(false);
    }
}