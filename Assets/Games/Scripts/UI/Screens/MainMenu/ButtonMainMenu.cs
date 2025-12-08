using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class ButtonMainMenu : MonoBehaviour
{
    [SerializeField] protected RectTransform IconImage;
    [SerializeField] protected Image btnImg;
    [SerializeField] protected GameObject textObj;
    [SerializeField] Sprite[] btnSprites;
    [SerializeField] float[] position;
    private Button btn;
    private Action clickAction;
    public RectTransform Icon => IconImage;

    private float scaleSize = 1.2f;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Select);
        OnDeselectButton();
    }

    private void Select()
    {
        AudioManager.Instance.PlayOneShot(AudioName.BUTTON_CLICK);
        AudioManager.Instance.PlayVibrate();
        clickAction?.Invoke();
    }

    public void OnSelectButton()
    {
        this.DOKill();
        btnImg.sprite = btnSprites[1];
        //btnImg.rectTransform.sizeDelta = new Vector2(btnImg.rectTransform.sizeDelta.x, 230);
        IconImage.transform.localScale = Vector3.one;
        IconImage.GetComponent<RectTransform>().DOAnchorPosY(position[1], 0.2f).SetId(this);
        IconImage.transform.DOScale(scaleSize, 0.2f).SetEase(Ease.OutBack).SetId(this);
        textObj.SetActive(true);
        //SetNoti(false);
    }

    public void OnDeselectButton()
    {
        this.DOKill();
        btnImg.sprite = btnSprites[0];
        //btnImg.rectTransform.sizeDelta = new Vector2(btnImg.rectTransform.sizeDelta.x, 190);

        IconImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, position[0]);
        IconImage.transform.localScale = Vector3.one;
        textObj.SetActive(false);
    }

    public void SetEventClick(Action action)
    {
        clickAction = action;
    }

    protected virtual void OnDisable()
    {
        this.DOKill();
    }
}