using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeTextGroupRes : MonoBehaviour
{
    [SerializeField] Text txtMove;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    public void SetNotice(string msg,string text="",string param ="")
    {
        if (msg.Length > 0)
        {
            txtMove.SetText(msg,mygame.sdk.StateCapText.None,mygame.sdk.FormatText.F_String,param);
        }
        else
        {
            txtMove.SetValue(text);
        }
        if(msg.Length == 0)
        {
            txtMove.gameObject.SetActive(false);
            gridLayoutGroup.padding.top = 20;
        }
        else
        {
            txtMove.gameObject.SetActive(true);
            gridLayoutGroup.padding.top = (int)txtMove.rectTransform.rect.height;
        }
    }
}
