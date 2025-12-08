using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemReceiveFly : MonoBehaviour
{
    [SerializeField] ItemFlyBase itemObjPrefab;
    [SerializeField] ParticleSystem fx;
    [SerializeField] SortingGroup fxSorting;
    private List<ItemFlyBase> listIcon;
    private RectTransform rectTransform;

    RectTransform RectTF
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            return rectTransform;
        }
    }

    private void Awake()
    {
        itemObjPrefab.gameObject.SetActive(false);
    }

    private List<ItemFlyBase> CreateList(int amount)
    {
        List<ItemFlyBase> list = new List<ItemFlyBase>();
        for (int i = 0; i < amount; i++)
        {
            var item = GetItem();
            item.gameObject.SetActive(true);
            list.Add(item);
        }

        return list;
    }

    private ItemFlyBase GetItem()
    {
        if (listIcon == null)
        {
            listIcon = new List<ItemFlyBase>();
        }

        ItemFlyBase item = null;
        for (int i = 0; i < listIcon.Count; i++)
        {
            if (!listIcon[i].gameObject.activeInHierarchy)
            {
                item = listIcon[i];
                break;
            }
        }

        if (!item)
        {
            item = Instantiate(itemObjPrefab, transform);
            listIcon.Add(item);
        }

        return item;
    }

    public Tween Animate(Vector2 scrStartPoint, RectTransform target, int amount, Action<int, int> complete)
    {
        var list = CreateList(amount);
        var cacheList = new List<ItemFlyBase>(list);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTF, scrStartPoint,
            RewardReceivedHub.Instance.Canvas.worldCamera, out Vector2 localPoint);
        float baseRadius = 60f;
        float angleStep = 360f / amount;
        var canvas = target.GetComponentInParent<Canvas>();
        int sortingOrder = 200;
        if (canvas != null)
        {
            sortingOrder = canvas.sortingOrder + 20;
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i].Initialize();
            list[i].SetUpLayer(sortingOrder);

            float angle = i * angleStep + Random.Range(-15f, 15f);
            float radius = baseRadius + Random.Range(-20f, 20f);

            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            list[i].RectTF.anchoredPosition = localPoint + offset;
        }

        list.Shuffle();
        Sequence sequence = DOTween.Sequence().SetId(this);
        Vector3 targetPosition = target.position;
        for (int i = 0; i < list.Count; i++)
        {
            int index = i;

            sequence.Insert(i * 0.1f, list[index].AnimAppear());
            sequence.Insert(i * 0.1f + 0.35f, list[index]
                .MoveTo(targetPosition,
                    Mathf.Min(Vector3.Distance(list[0].transform.position, targetPosition) / 15f, 1.1f))
                .OnComplete(() =>
                {
                    list[index].gameObject.SetActive(false);
                    cacheList.Remove(list[index]);
                    fx.transform.position = targetPosition;
                    fxSorting.sortingOrder = list[index].layerSort;
                    fx.Play();

                    mygame.sdk.GameHelper.Instance.Vibrate(mygame.sdk.Type_vibreate.Vib_Medium);
                    complete?.Invoke(index, list.Count);
                }).OnStart(() =>
                {
                    DOVirtual.DelayedCall(0.4f,
                            () => { AudioManager.Instance.PlayOneShot(AudioName.SFX_Item_Jump_End_1, 1); })
                        .SetId(list[index]);
                }));
        }

        sequence.OnUpdate(() =>
        {
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                sequence.Kill();
            }
        });
        sequence.OnKill(() =>
        {
            for (int i = 0; i < cacheList.Count; i++)
            {
                if (cacheList[i].gameObject.activeInHierarchy)
                {
                    cacheList[i].gameObject.SetActive(false);
                }
            }
        });
        return sequence;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}