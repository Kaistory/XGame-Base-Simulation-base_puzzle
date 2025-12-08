using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CoinFly : ItemFlyBase
{
    [SerializeField] Image spriteRendererCoin;
    [SerializeField] SortingGroup sortingGroup;
    public override void Initialize()
    {
        base.Initialize();
        SetAlpha(0);
        transform.localScale = Vector3.zero;
    }
    public override Tween AnimAppear()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(FadeSprites(spriteRendererCoin, 1, .35f));
        sequence.Join(transform.DOScale(1, .35f));
        return sequence;
    }
    public void SetAlpha(float alpha)
    {

        Color color = spriteRendererCoin.color;
        color.a = alpha;
        spriteRendererCoin.color = color;

    }
    public Tween FadeSprites(Image spriteRenderer,float targetAlpha, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.DOFade(targetAlpha, duration).SetEase(Ease.Linear));

        return sequence;

    }
    public override Tween MoveTo(Vector3 destination, float time)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(base.MoveTo(destination, time).SetEase(Ease.InOutQuart));
        return sequence;
    }
    public override void SetUpLayer(int sortingOrder)
    {
        sortingGroup.sortingOrder = sortingOrder;
        base.SetUpLayer(sortingOrder);
    }

}
