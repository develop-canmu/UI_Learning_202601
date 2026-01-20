using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Runtime.Scripts.Utility
{
    public static class DeviceUtility
    {
        private const int LOW_SPEC_MEMORY = 4096; // 単位はMB
        
        public static bool IsLowSpecDevice()
        {
            var isLowSpec = false;
#if UNITY_EDITOR
            isLowSpec = false;
#elif UNITY_IOS
            var generation = UnityEngine.iOS.Device.generation;
            if (generation >= UnityEngine.iOS.DeviceGeneration.iPhone &&
                generation <= UnityEngine.iOS.DeviceGeneration.iPhone7Plus)
            {
                isLowSpec = true;    
            }
#elif UNITY_ANDROID
            isLowSpec = SystemInfo.systemMemorySize < LOW_SPEC_MEMORY;
#endif
            return isLowSpec;
        }
    }
}