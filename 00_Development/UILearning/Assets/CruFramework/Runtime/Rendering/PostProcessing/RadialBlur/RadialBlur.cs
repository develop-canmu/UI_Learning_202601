#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    [Serializable]
    [VolumeComponentMenu("CruFramework/PostProcessing/RadialBlur")]
    public sealed class RadialBlur : VolumeComponent, IPostProcessComponent
    {
        public Vector2Parameter Center = new Vector2Parameter(Vector2.zero);
        public ClampedIntParameter Strength = new ClampedIntParameter(0, 0, 32);
        public ClampedIntParameter SampleCount = new ClampedIntParameter(1, 1, 16);

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