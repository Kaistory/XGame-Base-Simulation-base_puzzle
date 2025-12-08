using DG.Tweening;
using mygame.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveRewardHub : MonoBehaviour
{
    [SerializeField] private ItemUI itemUIPrefab;
    private readonly List<ItemUI> listItemUI = new List<ItemUI>();
    public Sequence ReceiveReward(Transform claimPosition, float scale = 1, params ItemInfo[] items)
    {
        var sequence = DOTween.Sequence();
        var scr = RectTransformUtility.WorldToScreenPoint(Camera.main, claimPosition.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), scr, Camera.main, out var targetPosition);
        var mul = -1;
        var delay = 0.2f;
        for (int i = 0; i < items.Length; i++)
        {
            var item = GetItemUI();
            item.transform.localScale = Vector3.one * scale;
            item.gameObject.SetActive(false);
            item.GetComponent<RectTransform>().anchoredPosition = targetPosition + new Vector2((Random.Range(i * 10, (i + 1) * 10) + 30) * mul, Random.Range(0, 20));
            Debug.Log(item.GetComponent<RectTransform>().anchoredPosition);
            mul *= -1;
            var time = i * delay;
            var gift = items[i];
            sequence.InsertCallback(time, () =>
            {
                item.gameObject.SetActive(true);
                item.Initialized(gift.Icon, gift.itemAmount);
            });
            sequence.InsertCallback(time + 1f, () =>
            {
                RealseItemUI(item);
            });
        }
        return sequence;

    }
    private ItemUI GetItemUI()
    {
        if (listItemUI.Count == 0)
        {
            return Instantiate(itemUIPrefab, transform);
        }
        else
        {
            var item = listItemUI[0];
            listItemUI.RemoveAt(0);
            return item;
        }
    }
    private void RealseItemUI(ItemUI itemUI)
    {
        itemUI.gameObject.SetActive(false);
        listItemUI.Add(itemUI);
    }

}
