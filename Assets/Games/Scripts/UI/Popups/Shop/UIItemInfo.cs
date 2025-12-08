using System;
using System.Collections;
using System.Collections.Generic;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

public class UIItemInfo : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text amount;
    public GameObject infinitySymbol;

    public ItemInfo dataInfo;

    public void Initialized(Sprite ic, int am)
    {
        icon.sprite = ic;
        amount.text = $"x{am}";
    }

    public void SetUpData(ItemInfo dataInfo)
    {
        this.dataInfo = dataInfo;
    }

    public void Initialized(RES_type resType, Sprite ic, int am)
    {
        icon.sprite = ic;

        amount.text = $"x{am}";

        amount.gameObject.SetActive(false);
    }
}