using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ToggleSwitch : MonoBehaviour
{
    [Header("Refs")] [SerializeField] private Button btnInteract; // Nút tương tác
    [SerializeField] private GameObject toggleOn;
    [SerializeField] private GameObject toggleOff;


    private bool isOn = true;

    public Button BtnInteract => btnInteract;
    public bool IsOn => isOn;
    
    /// <summary>
    /// Set trạng thái ban đầu KHÔNG anim (ví dụ khi mở popup hoặc sau khi load save).
    /// </summary>
    public void Initialize(bool initialOn)
    {
        this.DOKill();
        if (btnInteract) btnInteract.interactable = true;
        Toggle(initialOn);
    }


    /// <summary>
    /// Gạt tới trạng thái _isOn (CÓ anim, chặn spam).
    /// </summary>
    public bool Toggle(bool _isOn)
    {
        isOn = _isOn;
        AnimateToggle();
        return _isOn;
    }

    private void AnimateToggle()
    {
        if (toggleOn)
        {
            toggleOn.SetActive(isOn);
        }

        if (toggleOff)
        {
            toggleOff.SetActive(!isOn);
        }
    }
}