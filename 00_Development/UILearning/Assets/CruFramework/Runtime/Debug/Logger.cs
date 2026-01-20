using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CruFramework
{
    public class Logger
    {
        [Conditional("CRUFRAMEWORK_DEBUG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("CRUFRAMEWORK_DEBUG")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("CRUFRAMEWORK_DEBUG")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
        
        [Conditional("CRUFRAMEWORK_DEBUG")]
        public static void LogError(object message, Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }
    }
}
