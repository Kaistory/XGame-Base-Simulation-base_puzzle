using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] VisualResGeneral visualRes;
    [SerializeField] Text txtNumber;
    RectTransform rectTF;

    public RectTransform RectTF
    {
        get
        {
            if (!rectTF)
            {
                rectTF = GetComponent<RectTransform>();
            }

            return rectTF;
        }
    }

    public void Initialize(DataResource dataResource)
    {
        visualRes.Init(dataResource);
        var res = $"{dataResource.amount}";
        if (RewardReceivedHub.Instance.UnlimitedResourceTypes.Contains(dataResource.resType))
        {
            var hour = dataResource.amount / 3600;
            var min = (dataResource.amount % 3600) / 60;
            res = hour switch
            {
                > 0 when min > 0 => $"{hour}h{min:D2}m",
                > 0 => $"{hour}h",
                _ => $"{min}m"
            };
        }

        res = "+" + res;
        txtNumber.text = res;
    }

    public void Fly()
    {
        RectTransform rectTF = canvasGroup.GetComponent<RectTransform>();
        canvasGroup.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence().SetId(this);
        sequence.Append(canvasGroup.transform.DOScale(1, .3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            // AudioManager.Instance.PlayOneShot(AUDIO_CLIP_NAME.SFX_Claim); //DangVQ
        }));
        sequence.Insert(0f, rectTF.DOAnchorPosY(30, .3f).SetEase(Ease.Linear));
        var text = canvasGroup.GetComponentInChildren<Text>();
        Shadow shadow = null;
        BetterOutline outline = null;
        if (text != null)
        {
            shadow = text.gameObject.GetComponent<Shadow>();
            outline = text.gameObject.GetComponent<BetterOutline>();
        }

        sequence.Insert(.7f, canvasGroup.DOFade(0, .5f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            if (text != null && shadow != null && outline != null)
            {
                var shadowColor = shadow.effectColor;
                shadowColor.a = canvasGroup.alpha / 3;
                var outlineColor = outline.effectColor;
                outlineColor.a = canvasGroup.alpha / 3;
                shadow.effectColor = shadowColor;
                outline.effectColor = outlineColor;
            }
        }));
        sequence.Insert(.5f, rectTF.DOAnchorPosY(100, .5f).SetEase(Ease.InQuad));
        sequence.Play();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}