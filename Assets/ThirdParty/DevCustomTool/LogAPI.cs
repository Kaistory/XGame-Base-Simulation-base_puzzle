#define LOG_API

using UnityEngine;

public static class LogAPI
{
#if LOG_API
    private static readonly bool ENABLE_LOGGING = true;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif
    
    public static void Log(string region, object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{region}] Log: {message}");
        }
    }

    public static void LogError(string region, object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{region}] LogError: {message}");
        }
    }

    public static void LogWarning(string region, object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{region}] LogWarning: {message}");
        }
    }
}