using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class TweenUtils
{
    /// <summary>
    /// Scale nẩy như punch, nhưng cho phép set scale cuối.
    /// </summary>
    /// <param name="t">Transform</param>
    /// <param name="punchAmount">Độ phình lên thêm (ví dụ Vector3.one * 0.1f)</param>
    /// <param name="duration">Thời gian tổng punch (ví dụ 0.3f)</param>
    /// <param name="finalScale">Scale cuối cùng mong muốn</param>
    /// <returns>Sequence tween</returns>
    public static Sequence DOPunchScaleTo(this Transform t, Vector3 punchAmount, float duration, Vector3 finalScale)
    {
        var originalScale = t.localScale;
        var punchScale = originalScale + punchAmount;

        var seq = DOTween.Sequence();
        seq.Append(t.DOScale(punchScale, duration * 0.5f).SetEase(Ease.OutQuad));
        seq.Append(t.DOScale(finalScale, duration * 0.5f).SetEase(Ease.OutQuad));
        return seq;
    }
}

