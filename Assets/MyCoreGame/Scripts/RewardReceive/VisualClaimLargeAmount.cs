using mygame.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualClaimLargeAmount : VisualClaimBase
{
    [SerializeField] List<RES_type> listRestype;

    public override bool CanVisual(RES_type rES_Type)
    {
        return listRestype.Contains(rES_Type);
    }
    public override void Init(DataResource dataResource, Sprite bgDf = null, Sprite iconDf = null, byte star = 0)
    {
        base.Init(dataResource, bgDf, iconDf, star);
        if (dataResource.amount <= 1)
        {
            txtAmount.gameObject.SetActive(false);
        }
        else
        {
            txtAmount.gameObject.SetActive(true);
            txtAmount.SetValue($"{prefix}{dataResource.amount}");
        }
    }
}
