#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    [Serializable]
    [VolumeComponentMenu("CruFramework/PostProcessing/FastBlur")]
    public sealed class FastBlur : VolumeComponent, IPostProcessComponent
    {
        public ClampedIntParameter Strength = new ClampedIntParameter(0, 0, 16);
        public ClampedIntParameter Downsample = new ClampedIntParameter(0, 0, 4);

        public bool IsActive()
        {
            return Strength.value > 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

#endif