using System;
using DevUlts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : PopupUI
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Text _textShow;

    protected void OnEnable()
    {
        DOTween.Kill(this);
    }

    protected void OnDisable()
    {
        DOTween.Kill(this);
    }

    /// <summary>
    /// Gọi hiệu ứng fade màn hình: đen -> chạy action -> hiện lại
    /// </summary>
    /// <param name="onFade">Callback gọi khi màn hình đã fade in xong</param>
    /// <param name="onFinish">Callback gọi khi màn hình đã kết thúc</param>
    /// <param name="text">Nội dung text hiển thị (rỗng thì ẩn)</param>
    /// <param name="timeIn">Thời gian fade in</param>
    /// <param name="timeOut">Thời gian fade out</param>
    /// <param name="delay">Delay giữa fade in và fade out</param>
    public void Fade(Action onFade = null, Action onFinish = null, string text = null, float timeIn = 0.15f,
        float timeOut = 0.25f,
        float delay = 0.05f)
    {
        if (!_fadeImage)
        {
            Debug.LogWarning("FadePopup missing fade image reference.");
            onFade?.Invoke();
            Hide();
            return;
        }

        // Reset trạng thái ban đầu
        _fadeImage.color = new Color(0, 0, 0, 0);
        if (_textShow)
        {
            _textShow.gameObject.SetActive(false);
            _textShow.color = new Color(_textShow.color.r, _textShow.color.g, _textShow.color.b, 0);
        }

        // Fade In
        _fadeImage.DOFade(1, timeIn)
            .SetTarget(this)
            .OnComplete(() =>
            {
                // Hiển thị Text
                if (_textShow && !string.IsNullOrEmpty(text))
                {
                    _textShow.gameObject.SetActive(true);
                    _textShow.SetText(text);
                    _textShow.DOFade(1, 0.2f).SetTarget(this); // fade text vào cùng lúc
                }

                onFade?.Invoke();

                // Fade Out
                Sequence seq = DOTween.Sequence().SetTarget(this);
                seq.AppendInterval(delay);
                seq.Append(_fadeImage.DOFade(0, timeOut));
                if (_textShow && !string.IsNullOrEmpty(text))
                {
                    seq.Join(_textShow.DOFade(0, timeOut)); // text fade out cùng image
                }

                seq.OnComplete(() =>
                {
                    if (_textShow)
                        _textShow.gameObject.SetActive(false);

                    Hide();
                    onFinish?.Invoke();
                });
            });
    }
}