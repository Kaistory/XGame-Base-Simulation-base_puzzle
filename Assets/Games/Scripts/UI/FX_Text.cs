using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using TigerForge;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class FXSet
{
    [Header("Hiệu ứng hình ảnh (VFX)")] public GameObject vfx;

    [Header("Âm thanh tương ứng (SFX)")] public AudioClip sfx;
}

public class FX_Text : MonoBehaviour
{
    [Header("Danh sách cặp VFX + SFX")]
    [SerializeField]
    private List<FXSet> fxSets = new List<FXSet>();
    [SerializeField] private FXSet luckySound;

    [SerializeField] private AudioSource audioSource;

    private const float volume = 1f;
    private const int LOOP_RANGE = 5; // loop 5 phần tử cuối trong list fxset khi streak lớn hơn số phần tử trong mảng

    private bool isPlayingLuckySound = false;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        foreach (var fx in fxSets)
        {
            if (fx != null && fx.vfx != null)
                fx.vfx.SetActive(false);
        }
    }

    private void Initialize()
    {
        if (audioSource)
        {
            audioSource.volume = volume;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }
    private int index = 0;
    public void ActiveVfx(int streak)
    {
        if (fxSets == null || fxSets.Count == 0 || isPlayingLuckySound)
            return;

        //int count = fxSets.Count;

        //FXSet chosen = fxSets[(count <= LOOP_RANGE)
        //    ? (streak % count)
        //    : (streak < count - LOOP_RANGE)
        //        ? streak
        //        : (count - LOOP_RANGE + ((streak - (count - LOOP_RANGE)) % LOOP_RANGE))];
        FXSet chosen;
        if (index< fxSets.Count && index >= 0)
        {
         chosen = fxSets[index];
        }
        else
        {
            chosen = fxSets[0];
            index = 0;
        }
        index++;

        chosen.vfx.SetActive(true);

        if (audioSource.isPlaying)
            audioSource.Stop();

        if (chosen.sfx)
        {
            audioSource.clip = chosen.sfx;
            audioSource.Play();
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            if (chosen.vfx)
                chosen.vfx.SetActive(false);
        }).SetId(this);
    }

    public void ActiveLuckySound()
    {
        if (luckySound == null) return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        if (luckySound.vfx)
            luckySound.vfx.SetActive(true);

        isPlayingLuckySound = true;

        if (luckySound.sfx)
        {
            audioSource.clip = luckySound.sfx;
            audioSource.Play();
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            if (luckySound.vfx)
            {
                luckySound.vfx.SetActive(false);
            }
            isPlayingLuckySound = false;
        }).SetId(this);
    }
    private void OnEnable()
    {
        EventManager.StartListening(EventName.ActiveLuckySound, ActiveLuckySound);
    }
    private void OnDisable()
    {
        EventManager.StopListening(EventName.ActiveLuckySound, ActiveLuckySound);
        DOTween.Kill(this);
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}