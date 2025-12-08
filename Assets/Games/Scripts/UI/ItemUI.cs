using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image image;
    public Text text;
    public void Initialized(Sprite sprite, int amount)
    {
        image.sprite = sprite;
        text.text = $"+{amount}";
    }
}
