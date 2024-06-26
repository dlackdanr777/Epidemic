using UnityEngine;

public static class DebugLog
{
    public static void Log(object message){ Debug.Log(message); }

    public static void LogError(object message) { Debug.LogError(message); }
}
