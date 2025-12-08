using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HardLevelWarning : MonoBehaviour
{
    [SerializeField] private Image backGround;
    [SerializeField] private Image sign;

    private Tween tween;
    public void ShowWarnning()
    {
        Quaternion originalRot = sign.rectTransform.rotation;
        Color c = backGround.color;
        c.a = 0f;
        backGround.color = c;

        tween = DOTween.Sequence()
            .AppendInterval(0.75f)
            .Append(
                sign.rectTransform
                    .DOLocalRotate(new Vector3(0, -90f, 0), 0.5f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.Linear)
            )
            .Join(
                sign.rectTransform
                    .DOScale(1.25f, 0.25f)
                    .SetEase(Ease.Linear)
                    .SetLoops(2, LoopType.Yoyo)
            )
            .Join(
                backGround.DOFade(0.66f, 0.5f)
                    .SetEase(Ease.Linear)
            )
            //.Append(
            //    sign.rectTransform
            //        .DOLocalRotate(new Vector3(0, -45f, 0), 0.25f, RotateMode.LocalAxisAdd)
            //        .SetEase(Ease.Linear)
            //)
            //.Join(
            //    sign.rectTransform
            //        .DOScale(1f, 0.25f)
            //        .SetEase(Ease.Linear)
            //)
            .AppendInterval(1f)
            .Append(
                sign.rectTransform
                    .DOLocalRotate(new Vector3(0, -90f, 0), 0.25f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.Linear)
            )
            .Join(
                backGround.DOFade(0f, 0.25f)
                    .SetEase(Ease.Linear)
            )
            .OnComplete(() =>
            {
                tween.Kill();
                sign.rectTransform.rotation = originalRot;
                sign.rectTransform.localScale = Vector3.one;

                Color c = backGround.color;
                c.a = 0f;
                backGround.color = c;

                gameObject.SetActive(false);
            });
    }
}
