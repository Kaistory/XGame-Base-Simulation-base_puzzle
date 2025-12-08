using mygame.sdk;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ResGeneralUI : ResUIBase
{
    public VisualResUIBase visual;
    public Text txtValue;

    public string xValue = "";
    public string endValue = "";
    public DataResource data { get; private set; }

    public override Text TextValue => txtValue;

    public override Image Icon => visual.Icon;

    private Action _cb;
    protected override void SetUp()
    {
    }
    public override void Init(DataResource data, Action OnClickRes)
    {
        this.data = data;
        _cb = OnClickRes;
        VisualTextAmount(data.amount);
        visual.Init(data, data.bg, data.icon);
    }
    private void OnClickRes()
    {
        _cb?.Invoke();
    }
    public override bool CanVisual(DataResource data)
    {
        return true;
    }

    public override void VisualTextAmount(int value)
    {
        if (txtValue != null)
        {
            if (value <= 0)
            {
                txtValue.text = "";
            }
            else
            {
                txtValue.text = $"{xValue}{value.ToString("N0")}{endValue}";
            }
        }
    }

    public override DataResource GetData()
    {
        return data;
    }


}
