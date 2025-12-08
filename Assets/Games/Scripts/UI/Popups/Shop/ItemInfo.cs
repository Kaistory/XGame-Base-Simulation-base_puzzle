using System;
using mygame.sdk;
using UnityEngine;

[Serializable]
public class ItemInfo
{
    public RES_type itemType;
    [SerializeField] private Sprite itemIcon;
    public int itemAmount;

    public string itemName;

    public Sprite Icon
    {
        get
        {
            if (itemIcon == null)
            {
                itemIcon = DataManager.Instance.GetIcon(itemType);
            }

            return itemIcon;
        }
    }


    public ItemInfo(Sprite ic, RES_type rwType, int am, string name = "")
    {
        itemIcon = ic;
        itemType = rwType;
        itemAmount = am;
        itemName = name;
    }

    public ItemInfo(ItemInfo itemInf)
    {
        itemIcon = itemInf.itemIcon;
        itemType = itemInf.itemType;
        itemAmount = itemInf.itemAmount;
        itemName = itemInf.itemName;
    }

    public ItemInfo()
    {
    }

    public DataResource ToDataResource()
    {
        DataResource dataResource = new DataResource();
        dataResource.amount = itemAmount;
        dataResource.resType = itemType;
        dataResource.icon = itemIcon;
        return dataResource;
    }

    public ItemInfo CopyFromDataResource(DataResource dataResource)
    {
        this.itemAmount = dataResource.amount;
        itemType = dataResource.resType;
        itemIcon = dataResource.icon;
        return this;
    }
}