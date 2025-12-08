using System;
using UnityEngine;

public static class DebugLogHelper
{
#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = true;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    #region Log API

    private static string LogRegion = $"DangVQ";

    public static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    public static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    public static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion

    #region Log API T

    public static void Log<T>(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{typeof(T).Name}] Log: {message}");
        }
    }

    public static void LogError<T>(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{typeof(T).Name}] LogError: {message}");
        }
    }

    public static void LogWarning<T>(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{typeof(T).Name}] LogWarning: {message}");
        }
    }

    #endregion
}