using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEffectReceiveMutiple : MonoBehaviour
{
    [SerializeField] private UIEffectReceive itemInfo;
    private List<UIEffectReceive> allItems = new List<UIEffectReceive>();

    public void Initialized(ItemInfo[] itemInfos, Vector3 startPoint, Transform target)
    {
        itemInfo.gameObject.SetActive(false);
        var count = Mathf.Max(itemInfos.Length, allItems.Count);
        for (int i = 0; i < count; i++)
        {
            if (i < itemInfos.Length)
            {
                UIEffectReceive item; 
                if (i < allItems.Count)
                {
                    item = allItems[i];
                }
                else
                {
                    item = Instantiate(itemInfo, itemInfo.transform.parent);
                    allItems.Add(item);
                }

                item.Initialized(itemInfos[i], startPoint, target);
                item.gameObject.SetActive(true);
            }
            else
            {
                allItems[i].gameObject.SetActive(false);
            }
        }
    }
    
    // public void Initialized(ItemInfo[] itemInfos, Vector3 startPoint, UITopBar topBar)
    // {
    //     itemInfo.gameObject.SetActive(false);
    //     var count = Mathf.Max(itemInfos.Length, allItems.Count);
    //     for (int i = 0; i < count; i++)
    //     {
    //         if (i < itemInfos.Length)
    //         {
    //             UIEffectReceive item; 
    //             if (i < allItems.Count)
    //             {
    //                 item = allItems[i];
    //             }
    //             else
    //             {
    //                 item = Instantiate(itemInfo, itemInfo.transform.parent);
    //                 allItems.Add(item);
    //             }
    //
    //             var target = topBar.FindTargetReceive(itemInfos[i].itemType);
    //             item.Initialized(itemInfos[i], startPoint, target);
    //             item.gameObject.SetActive(true);
    //         }
    //         else
    //         {
    //             allItems[i].gameObject.SetActive(false);
    //         }
    //     }
    // }

    
    public void Initialized(PackageData.ItemBuyInfo[] itemInfos, Vector3 startPoint, Transform target)
    {
        itemInfo.gameObject.SetActive(false);
        var count = Mathf.Max(itemInfos.Length, allItems.Count);
        for (int i = 0; i < count; i++)
        {
            if (i < itemInfos.Length)
            {
                UIEffectReceive item; 
                if (i < allItems.Count)
                {
                    item = allItems[i];
                }
                else
                {
                    item = Instantiate(itemInfo, itemInfo.transform.parent);
                    allItems.Add(item);
                }

                item.Initialized(itemInfos[i], startPoint, target);
                item.gameObject.SetActive(true);
            }
            else
            {
                allItems[i].gameObject.SetActive(false);
            }
        }
    }
}
