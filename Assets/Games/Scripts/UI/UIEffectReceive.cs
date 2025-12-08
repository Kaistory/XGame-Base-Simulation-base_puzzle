using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

public class UIEffectReceive : MonoBehaviour
{
    private Vector3 targetPosition;
    private Transform target;
    [SerializeField] private float moveDuration = .75f;
    //[SerializeField] private float spreadDistance = 5f;
    [SerializeField] private float spreadDuration = 0.35f;
    [SerializeField] private float returnDuration = .75f;
    [SerializeField] private float scaleFactor = 1.5f;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private GameObject icon;

    [SerializeField] private UIItemInfo itemInfo;
    private List<UIItemInfo> allItems = new List<UIItemInfo>();

    public void Initialized(ItemInfo itemInf, Vector3 startPoint, Transform tar)
    {
        target = tar;
        targetPosition = tar.position;
        itemInfo.gameObject.SetActive(false);
        var length = Mathf.Clamp(itemInf.itemAmount / 5, 1, 15);
        var count = Mathf.Max(length, allItems.Count);
        for (int i = 0; i < count; i++)
        {
            if (i < length)
            {
                UIItemInfo item; 
                if (i < allItems.Count)
                {
                    item = allItems[i];
                }
                else
                {
                    item = Instantiate(itemInfo, itemInfo.transform.parent);
                    allItems.Add(item);
                }
                
                item.Initialized(itemInf.itemType, itemInf.Icon, itemInf.itemAmount);
                item.transform.position = startPoint;
                item.transform.localScale = Vector3.one;
                item.GetComponentInChildren<TrailRenderer>().Clear();
                item.gameObject.SetActive(true);
                CollectItem(item.transform);
            }
            else
            {
                allItems[i].gameObject.SetActive(false);
            }
        }
    }
    
    private void CollectItem(Transform trans)
    {
        trans.DOKill();
        trans.GetComponent<CanvasGroup>().DOKill();
        var rd = Random.Range(1f, 1.25f);
        trans.GetComponent<CanvasGroup>().alpha = 1;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        Vector3 spreadPosition = trans.position + randomDirection * Random.Range(1f, 3f);
        trans.DOScale(scaleFactor * .75f, spreadDuration * rd);
        trans.DOMove(spreadPosition, spreadDuration * rd).SetEase(Ease.OutCubic).OnComplete(() => ReturnToTarget(trans));
    }
    private void ReturnToTarget(Transform trans)
    {
        var rd = Random.Range(0.5f, 1f);
        trans.DOScale(scaleFactor, returnDuration * rd);
        trans.DOMove(targetPosition, returnDuration * rd).SetEase(Ease.InCubic).OnComplete(() => OnItemCollected(trans));
    }

    private void OnItemCollected(Transform trans)
    {
        if (target != null)
        {
            target.GetComponentInChildren<ParticleSystem>().Play();
            GameHelper.Instance.Vibrate(Type_vibreate.Vib_Heavy);
        }
        trans.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
