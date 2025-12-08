using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Quản lý thời gian người chơi mở app và ghi log tổng thời gian chơi.
/// - Chỉ tính khi app đang mở (foreground)
/// - Tự động cộng dồn tổng thời gian chơi (totalTime)
/// - Tự động ghi log engagement (ví dụ 300s, 600s, 900s...)
/// - Lưu dữ liệu theo batch để giảm tần suất PlayerPrefs.Save()
/// </summary>
public class TimeLogger : MonoBehaviour
{
    // ====================== Singleton ======================
    private static TimeLogger _instance;
    public static TimeLogger Instance => _instance;

    // ====================== Key lưu dữ liệu ======================
    private const string K_TOTAL = "total_second_play_game"; // Tổng số giây đã chơi (tất cả session)
    private const string K_INTERVAL = "cf_time_log_engagement"; // Khoảng thời gian log engagement (mặc định 300s)

    // ====================== Tuỳ chọn ======================
    [Header("Save mỗi N giây (batch)")] [SerializeField]
    private int saveBatchSeconds = 10; // Lưu mỗi N giây tích luỹ

    // ====================== Biến nội bộ ======================
    private int _unsaved; // Đếm giây chưa lưu
    private int totalTime; // Tổng số giây đã chơi (cache)
    private float timeInSection = 0f; // Tổng thời gian app chạy kể từ khi khởi tạo class (reset khi Awake)
    private float _accumSeconds = 0f; // Tích luỹ deltaTime để quy đổi ra giây nguyên

    private static int CFTimeLogEngagementDefault = 300; // Mặc định log mỗi 300s

    private int _nextLogThreshold; // Mốc log engagement tiếp theo
    private Coroutine _tickRoutine; // Coroutine đếm thời gian
    private bool _tickRunning; // Đã start TickRoutine chưa

    // ====================== Sự kiện (Event) ======================
    public static event Action<int> OnEngagementLogged; // Gửi event mỗi khi log engagement (trả về tổng giây)

    // ====================== Thuộc tính public ======================
    /// <summary>Tổng giây đã chơi (được cập nhật liên tục trong cache)</summary>
    public int TotalTime => totalTime;

    /// <summary>Tổng thời gian app chạy (tính bằng giây thực, không reset trừ khi Awake)</summary>
    public float TimeInSection => timeInSection;

    // ====================== API static cho bên ngoài ======================

    /// <summary>
    /// Đọc tổng thời gian chơi đã lưu.  
    /// Ưu tiên đọc cache nếu có Instance, fallback về PlayerPrefs.
    /// </summary>
    public static int TotalSecondPlayGame
    {
        get
        {
            if (Instance) return Instance.totalTime;
            return PlayerPrefs.GetInt(K_TOTAL, 0);
        }
        private set => PlayerPrefs.SetInt(K_TOTAL, value);
    }

    /// <summary>
    /// Khoảng thời gian giữa các lần log engagement (mặc định 300s).  
    /// Có thể chỉnh runtime, giá trị được lưu vào PlayerPrefs.
    /// </summary>
    public static int CFTimeLogEngagement
    {
        get => PlayerPrefs.GetInt(K_INTERVAL, CFTimeLogEngagementDefault);
        set
        {
            if (value <= 0) value = CFTimeLogEngagementDefault;
            PlayerPrefs.SetInt(K_INTERVAL, value);
            PlayerPrefs.Save();
            if (Instance != null) Instance.RecalculateNextThreshold();
        }
    }

    /// <summary>
    /// Cộng thủ công thêm số giây chơi (nếu cần, ví dụ từ data khác).  
    /// Có thể gọi mà không cần Instance tồn tại.
    /// </summary>
    public static void AddPlaySeconds(int seconds)
    {
        if (seconds <= 0) return;
        if (!Instance)
        {
            int cur = PlayerPrefs.GetInt(K_TOTAL, 0) + seconds;
            PlayerPrefs.SetInt(K_TOTAL, cur);
            PlayerPrefs.Save();
            return;
        }

        Instance.InternalAddSeconds(seconds);
    }

    /// <summary>Đặt lại khoảng cách log engagement (đổi giá trị runtime).</summary>
    public void SetEngagementInterval(int newInterval) => CFTimeLogEngagement = newInterval;

    // ====================== Unity Lifecycle ======================

    private void Awake()
    {
        // Đảm bảo singleton (chỉ có 1 instance tồn tại)
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Tải dữ liệu cache
        totalTime = PlayerPrefs.GetInt(K_TOTAL, 0);
        timeInSection = 0f; // Reset khi khởi tạo app (section mới)
        _accumSeconds = 0f;
        _unsaved = 0;
        _tickRunning = false;

        // Tính mốc log tiếp theo (vd: 300, 600, 900…)
        RecalculateNextThreshold();
    }

    private void OnEnable() => StartTickIfNeeded();

    private void OnDisable()
    {
        StopTickIfNeeded();
        FlushSavesIfNeeded();
    }

    /// <summary>
    /// Khi app bị pause (vào nền) → dừng tick và lưu lại dữ liệu chưa save.
    /// Khi app resume → tiếp tục tick.
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopTickIfNeeded();
            FlushSavesIfNeeded();
        }
        else
        {
            RecalculateNextThreshold();
            StartTickIfNeeded();
        }
    }

    /// <summary>
    /// Khi mất/gain focus (tương tự Pause/Resume)
    /// </summary>
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            RecalculateNextThreshold();
            StartTickIfNeeded();
        }
        else
        {
            StopTickIfNeeded();
            FlushSavesIfNeeded();
        }
    }

    /// <summary>
    /// Khi app thoát → flush dữ liệu vào PlayerPrefs để tránh mất.
    /// </summary>
    private void OnApplicationQuit() => FlushSavesIfNeeded(true);

    // ====================== Tick logic (chạy mỗi frame) ======================

    /// <summary>Bắt đầu TickRoutine nếu chưa chạy.</summary>
    private void StartTickIfNeeded()
    {
        if (_tickRunning) return;
        _tickRunning = true;
        if (_tickRoutine == null) _tickRoutine = StartCoroutine(TickRoutine());
    }

    /// <summary>Dừng TickRoutine nếu đang chạy.</summary>
    private void StopTickIfNeeded()
    {
        if (!_tickRunning) return;
        _tickRunning = false;
        if (_tickRoutine != null) StopCoroutine(_tickRoutine);
        _tickRoutine = null;
    }

    /// <summary>
    /// Coroutine chính, cập nhật mỗi frame:
    /// - Cộng dồn thời gian thực (unscaledDeltaTime)
    /// - Cộng giây nguyên vào totalTime mỗi khi đủ 1 giây
    /// - Batch save & log engagement
    /// </summary>
    private IEnumerator TickRoutine()
    {
        while (true)
        {
            float dt = Time.unscaledDeltaTime;

            // Cộng dồn tổng thời gian app chạy
            timeInSection += dt;

            // Tích luỹ delta để quy đổi sang giây nguyên
            _accumSeconds += dt;
            if (_accumSeconds >= 1f)
            {
                int gained = Mathf.FloorToInt(_accumSeconds);
                _accumSeconds -= gained;
                InternalAddSeconds(gained);
            }

            yield return null;
        }
    }

    /// <summary>
    /// Thêm số giây vào totalTime (được gọi mỗi khi TickRoutine đủ 1s).
    /// Gồm:
    /// - Cộng vào cache
    /// - Batch save PlayerPrefs
    /// - Kiểm tra và log mốc engagement
    /// </summary>
    private void InternalAddSeconds(int seconds)
    {
        if (seconds <= 0) return;

        totalTime += seconds;
        _unsaved += seconds;

        // Batch save: chỉ lưu mỗi N giây
        int batch = Mathf.Max(1, saveBatchSeconds);
        if (_unsaved >= batch)
        {
            TotalSecondPlayGame = totalTime;
            PlayerPrefs.Save();
            _unsaved = 0;
        }

        // Log engagement (300s, 600s…)
        while (totalTime >= _nextLogThreshold)
        {
            LogEngagement(totalTime);
            _nextLogThreshold += Mathf.Max(1, CFTimeLogEngagement);
        }
    }

    /// <summary>
    /// Tính lại mốc engagement kế tiếp dựa vào tổng thời gian hiện tại.
    /// Ví dụ: total=450, interval=300 → mốc kế tiếp = 600.
    /// </summary>
    private void RecalculateNextThreshold()
    {
        int interval = Mathf.Max(1, CFTimeLogEngagement);
        int total = Mathf.Max(0, totalTime);

        int k = Mathf.CeilToInt(total / (float)interval);
        _nextLogThreshold = k * interval;
        if (_nextLogThreshold <= total) _nextLogThreshold += interval;

        Log($"RecalculateNextThreshold -> total={total}, interval={interval}, next={_nextLogThreshold}");
    }

    /// <summary>
    /// Lưu dữ liệu ra PlayerPrefs nếu có giây chưa lưu.
    /// Thường được gọi khi app pause, quit, hoặc force save.
    /// </summary>
    private void FlushSavesIfNeeded(bool force = false)
    {
        if (force || _unsaved > 0)
        {
            TotalSecondPlayGame = totalTime;
            PlayerPrefs.Save();
            _unsaved = 0;
        }
    }

    /// <summary>
    /// Gửi log engagement lên Firebase (hoặc hệ thống log khác).
    /// </summary>
    private void LogEngagement(int totalSeconds)
    {
        Log($"Engagement log at {totalSeconds} seconds");

        LogEvent.LogEventParam("total_time_play_{0}", totalSeconds);

        OnEngagementLogged?.Invoke(totalSeconds);
    }

    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = false;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    private static readonly string LogRegion = $"{nameof(TimeLogger)}";

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