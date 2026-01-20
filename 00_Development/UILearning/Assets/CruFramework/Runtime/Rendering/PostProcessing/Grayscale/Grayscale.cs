#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    [Serializable]
    [VolumeComponentMenu("CruFramework/PostProcessing/Grayscale")]
    public sealed class Grayscale : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Weight = new ClampedFloatParameter(0, 0, 1);

        public bool IsActive()
        {
            return Weight.value > 0;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

#endif