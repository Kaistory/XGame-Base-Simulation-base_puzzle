using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MyBox;
using mygame.sdk;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSfxSource;

    [Header("Default Settings")]
    [Range(0f, 1f)]
    public float defaultMusicVolume = 0.8f;

    [Range(0f, 1f)] public float defaultSfxVolume = 1f;
    [Range(0f, 1f)] public float defaultLoopSfxVolume = 1f;
    private float fadeDuration = 0.2f;

    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();
    private Coroutine fadeCoroutine;

    // Track current playing
    private string currentMusicName;
    private string currentSfxName;
    private string currentLoopSfxName;

    // Playlist
    private List<string> musicPlaylist = new List<string>();
    private int currentTrackIndex = 0;
    private bool isAutoPlayNext = false;
    private Coroutine playlistChecker;

    // Link trực tiếp với PlayerPrefsUtil
    public bool IsMusicEnabled
    {
        get => mygame.sdk.PlayerPrefsUtil.IsMusicEnable;
        set => mygame.sdk.PlayerPrefsUtil.IsMusicEnable = value;
    }

    public bool IsSfxEnabled
    {
        get => mygame.sdk.PlayerPrefsUtil.IsSoundEnable;
        set => mygame.sdk.PlayerPrefsUtil.IsSoundEnable = value;
    }

    private AudioConfiguration audioConfiguration;
    private float musicPlayTime;
    private bool musicIsPlaying;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioConfiguration = AudioSettings.GetConfiguration();
    }

    public void ResetAudio()
    {
        AudioSettings.Reset(audioConfiguration);
        if (musicIsPlaying)
        {
            musicSource.time = musicPlayTime;
            musicSource.Play();
        }
    }

    public void SetCacheAudio()
    {
        musicPlayTime = musicSource.time;
        musicIsPlaying = musicSource.isPlaying;
        // musicSource.Pause();
    }

    public void Initialize()
    {
        // Ensure AudioSources exist
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }

        if (loopSfxSource == null)
        {
            loopSfxSource = gameObject.AddComponent<AudioSource>();
            loopSfxSource.loop = true;
        }

        // Clear cache & playlist
        clipCache.Clear();
        musicPlaylist.Clear();

        // Apply volumes based on saved settings
        musicSource.volume = IsMusicEnabled ? defaultMusicVolume : 0f;
        sfxSource.volume = IsSfxEnabled ? defaultSfxVolume : 1f;
        loopSfxSource.volume = IsSfxEnabled ? defaultSfxVolume : 0f;


        musicSource.loop = true;
        loopSfxSource.loop = true;
        if (musicSource && musicSource.clip)
        {
            musicSource.Play();
        }
        else
        {
            // Optional: Add default BG music clips
            for (int i = 0; i < 4; i++)
            {
                AddToPlaylist($"BG{i + 1}");
            }

            // Auto play playlist
            PlayPlaylist();
        }

        // Example: lower volume for intro
        SetMusicVolume(0.5f);
    }


    #region PLAYLIST

    public void AddToPlaylist(string clipName)
    {
        bool hasClip = GetClip(clipName, "Music") != null;
        if (!musicPlaylist.Contains(clipName) && hasClip)
            musicPlaylist.Add(clipName);
    }

    public void PlayPlaylist(bool autoPlayNext = true)
    {
        if (musicPlaylist.Count == 0)
        {
            if (musicSource)
                musicSource.Play();
            return;
        }

        isAutoPlayNext = autoPlayNext;
        currentTrackIndex = 0;
        Log($"PlayPlaylist");
        PlayMusic(musicPlaylist[currentTrackIndex]);

        // if (playlistChecker == null)
        //     playlistChecker = StartCoroutine(CheckMusicCompletion());
    }

    public void PlayNextTrack()
    {
        if (musicPlaylist.Count == 0) return;

        bool fade = musicPlaylist.Count > 1;
        currentTrackIndex++;
        if (currentTrackIndex >= musicPlaylist.Count)
            currentTrackIndex = 0;
        Log($"PlayNextTrack");
        PlayMusic(musicPlaylist[currentTrackIndex], fade);
    }

    private IEnumerator CheckMusicCompletion()
    {
        while (true)
        {
            if (IsMusicEnabled &&
                isAutoPlayNext &&
                musicSource.clip != null &&
                !musicSource.isPlaying &&
                musicPlaylist.Count > 0)
            {
                PlayNextTrack();
            }

            yield return null;
        }
    }

    #endregion

    #region MUSIC

    public void PlayMusic(string clipName, bool fade = true, float fadeTime = -1f)
    {
        if (!IsMusicEnabled) return;

        AudioClip clip = GetClip(clipName, "Music");
        if (clip == null) return;

        currentMusicName = clipName;
        if (fadeTime < 0) fadeTime = fadeDuration;

        if (fade && musicSource.isPlaying)
        {
            FadeOutIn(musicSource, clip, fadeTime, defaultMusicVolume);
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = defaultMusicVolume;
            musicSource.Play();
        }

        Log($"Play Music --  clipName: {clipName} || fade: {fade}");
    }

    public void StopMusic(bool fade = true, float fadeTime = -1f)
    {
        if (fadeTime < 0)
        {
            fadeTime = fadeDuration;
        }

        if (fade)
        {
            FadeOut(musicSource, fadeTime, () => musicSource.Stop());
        }
        else
        {
            musicSource.Stop();
        }

        Log($"Stop Music --  fade: {fade}");
        currentMusicName = null;
    }

    public void PauseMusic()
    {
        if (IsMusicEnabled)
        {
            musicSource.Pause();
            Log($"Pause Music");
        }
    }

    public void ResumeMusic()
    {
        if (IsMusicEnabled)
        {

            musicSource.UnPause();
            if (!musicSource.isPlaying)
                musicSource.Play();
            Log($"Resume Music");
        }
    }

    #endregion

    #region SFX

    public void PlaySFX(string clipName, float volumeScale = 1f)
    {
        if (!IsSfxEnabled) return;

        AudioClip clip = GetClip(clipName, "SFX");
        if (clip)
        {
            currentSfxName = clipName;
            sfxSource.PlayOneShot(clip, defaultSfxVolume * volumeScale);
            Log($"Play SFX ---- clipName: {clipName}");
        }
    }

    public void PlayOneShot(string clipName, float volumeScale = 1f)
    {
        if (!IsSfxEnabled) return;

        AudioClip clip = GetClip(clipName, "SFX");
        if (clip)
        {
            currentSfxName = clipName;
            sfxSource.PlayOneShot(clip, defaultSfxVolume * volumeScale);
            Log($"Play SFX ---- clipName: {clipName}");
        }
    }

    public void PlayLoopSFX(string clipName, float volumeScale = 1f)
    {
        if (!IsSfxEnabled) return;

        AudioClip clip = GetClip(clipName, "SFX");
        if (clip != null)
        {
            if (loopSfxSource.clip != clip)
            {
                loopSfxSource.clip = clip;
                SetVolumeLoopSFX(volumeScale);
                loopSfxSource.loop = true;
            }

            loopSfxSource.Play();
            currentLoopSfxName = clipName;
            currentSfxName = clipName;

            Log($"Play PlayLoopSFX ---- clipName: {clipName}");
        }
    }

    public void PauseLoopSFX()
    {
        if (IsSfxEnabled)
        {
            loopSfxSource.Pause();
            Log($"Pause Loop SFX");
        }
    }

    public void ResumeLoopSFX()
    {
        if (IsSfxEnabled)
        {
            loopSfxSource.UnPause();
            Log($"Resume Loop SFX");
        }
    }

    public void SetVolumeLoopSFX(float volumeScale = 1f)
    {
        loopSfxSource.volume = Mathf.Clamp01(defaultSfxVolume * volumeScale);
        Log($"Set Volume Loop SFX ---- volumeScale: {loopSfxSource.volume}");
    }

    public void StopLoopSFX(string clipName, bool fade = false, float fadeTime = 0.15f)
    {
        if (string.IsNullOrEmpty(clipName)) return;
        if (loopSfxSource == null) return;
        if (!loopSfxSource.isPlaying) return;

        if (currentLoopSfxName != clipName &&
            !(loopSfxSource.clip != null && loopSfxSource.clip.name == clipName))
            return;

        Action stopNow = () =>
        {
            loopSfxSource.Stop();
            loopSfxSource.clip = null;
            currentLoopSfxName = null;
            if (currentSfxName == clipName) currentSfxName = null;
        };

        if (fade)
        {
            FadeOut(loopSfxSource, fadeTime, stopNow);
        }
        else
        {
            stopNow();
        }

        Log($"Stop Loop SFX ---- clipName: {clipName} || fade: {fade}");
    }

    public void StopLoopSFX()
    {
        if (loopSfxSource == null)
        {
            return;
        }

        loopSfxSource.Stop();
        loopSfxSource.clip = null;
        currentLoopSfxName = null;
        currentSfxName = null;
        Log($"Stop Loop SFX");
    }

    public void StopSFX()
    {
        sfxSource.Stop();
        currentSfxName = null;
        Log($"Stop SFX");
    }

    #endregion

    #region Volume

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
        defaultMusicVolume = musicSource.volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
        defaultSfxVolume = sfxSource.volume;
    }

    public void SetLoopSfxVolume(float volume)
    {
        loopSfxSource.volume = Mathf.Clamp01(volume);
        defaultLoopSfxVolume = loopSfxSource.volume;
    }

    #endregion

    #region Fade

    private void FadeOut(AudioSource source, float duration, Action onComplete)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(source, source.volume, 0f, duration, onComplete));
    }

    private void FadeOutIn(AudioSource source, AudioClip newClip, float duration, float targetVolume)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutInRoutine(source, newClip, duration, targetVolume));
    }

    private IEnumerator FadeRoutine(AudioSource source, float from, float to, float duration, Action onComplete)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        source.volume = to;
        onComplete?.Invoke();
    }

    private IEnumerator FadeOutInRoutine(AudioSource source, AudioClip newClip, float duration, float targetVolume)
    {
        yield return FadeRoutine(source, source.volume, 0f, duration, null);
        source.clip = newClip;
        source.Play();
        yield return FadeRoutine(source, 0f, targetVolume, duration, null);
    }

    #endregion

    #region Clip Loading

    private AudioClip GetClip(string nameClip, string folder)
    {
        if (clipCache.TryGetValue(nameClip, out var cached))
            return cached;

        AudioClip clip = Resources.Load<AudioClip>($"Audio/{folder}/{nameClip}");
        if (clip)
        {
            clipCache[nameClip] = clip;
        }
        else
        {
            LogWarning($"Audio clip '{nameClip}' not found in Resources/Audio/{folder}");
        }

        return clip;
    }

    #endregion

    #region Vibrate

    public void PlayVibrate()
    {
        GameHelper.Instance.Vibrate(Type_vibreate.Vib_Medium);
    }

    #endregion

    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = false;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    private static readonly string LogRegion = $"{nameof(AudioManager)}";

    private static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    private static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    private static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion
}

// public partial class AudioName
// {
//     public const string AUDIO_ENERGY = "Energy1";
//     public const string AUDIO_DOORBELL = "DoorBellRing";
//     public const string AUDIO_DYNAMITE_EXPLOSION = "DynamiteExplosion";
//     public const string AUDIO_DETECTOR = "DetectorBeep";
//     public const string BUTTON_CLICK = "click";
//     public const string AUDIO_GETSHOVEL = "GetShovel";
//     public const string AUDIO_KNOCKDOOR = "KnockDoor";
//     public const string AUDIO_SNORE = "Snore";
//     public const string AUDIO_OPENDOOR = "OpenDoorSFX";
//     public const string AUDIO_CLOSEDOOR = "CloseDoorSFX";
//     public const string AUDIO_WALKINROOM = "WalkInRoomSFX";
//     public const string AUDIO_WALKONDIRT = "WalkOnDirt";
//     public const string AUDIO_FLUSHTOILET = "FlushToiletSFX";
//     public const string AUDIO_BELLLUNCH = "BellLunch";
//     public const string AUDIO_SCOOPSOIL = "ScoopSoilSFX";
//     public const string AUDIO_LUNCHRING = "LunchRing";
//     public const string AUDIO_POLICEWALK = "PoliceWalk";
//     public const string AUDIO_FLYPAPER = "FlyPaper";
//     public const string AUDIO_GAIN_TOILET_PAPER = "GainToiletPapers";
//     public const string AUDIO_CARPETSFX = "CarpetSFX";
//     public const string AUDIO_BELL_WARNING = "BellWarning";
//     public const string AUDIO_FILL_ITEM_ON_TABLE = "FillItemOnTable";
//     public const string AUDIO_FILL_ITEM_POLICE = "FillItemPolice";
//     public const string AUDIO_LOW_ENERGY = "LowEnergy";
//     public const string AUDIO_FULL_ITEM_INVENTORY = "FullItemInventory";
//     public const string AUDIO_EAT_SOUP = "EatingSoup";
//     public const string AUDIO_MINUS_MONEY = "MinusMoney";
//     public const string AUDIO_PUNCH = "PunchSFX";
//     public const string AUDIO_JUMP = "JumpSFX";
//     public const string AUDIO_FLY_MONEY_UI = "IconUIfly";
//     public const string AUDIO_FALL_CAN = "CanFall1";
//     public const string AUDIO_DRINK = "Drinking2";
//     public const string AUDIO_MEETBOSS = "MeetBoss";
//     public const string AUDIO_OPENPOPUP = "OpenPopup";
//     public const string AUDIO_SCATTERSOIL = "ScatterSoil";
//     public const string AUDIO_COIN_SPEND = "CoinSpend";
//     public const string AUDIO_UNLOCK = "Unlock";
//
//     //MiniGame Arm Wrestling
//     public const string AUDIO_AW_PlayerLongArgh = "m_PlayerLongArgh";
//     public const string AUDIO_AW_Beep = "m_Beep";
//     public const string AUDIO_AW_Ready = "m_Ready";
//     public const string AUDIO_AW_Fight = "m_Fight";
//     public const string AUDIO_AW_TableSlam = "m_TableSlam";
//     public const string AUDIO_AW_KO = "m_KO";
//     public const string AUDIO_AW_PlayerArgh = "m_PlayerArgh";
//     public const string AUDIO_AW_PlayerMhmm = "m_PlayerMhmm";
//     public const string AUDIO_AW_SayNo = "m_SayNo";
//
//
//     //MiniGame FlipCard
//     public const string AUDIO_F_CardCorrectChoose = "F_CardCorrectChoose";
//     public const string AUDIO_F_CardLoseGame = "F_CardLoseGame";
//     public const string AUDIO_F_CardWrongChoose = "F_CardWrongChoose";
//     public const string AUDIO_F_ClickSound = "F_ClickSound";
//     public const string AUDIO_F_FlipCard = "F_FlipCard";
//
//     //MiniGame Wrestling (đẩy tạ)
//     public const string AUDIO_W_PopSound = "Popsound";
//     public const string AUDIO_W_SuccessSound = "SuccessSound";
//     public const string AUDIO_W_Lose = "LoseGame";
//     public const string AUDIO_W_DRUM = "drum";
//
//     //MiniGame Move Block
//     public const string HIT_BLOCK = "BlockHits";
//
//     //Minigame Red Zone Escape
//     public const string AUDIO_RZE_ONOFF = "RZE_OnOffCamera";
//     public const string AUDIO_RZE_PICKUP_KEY = "UnlockNewItem";
//
//     //Chest Open
//     public const string AUDIO_CHEST_OPEN = "ChestOpen2";
//
// }