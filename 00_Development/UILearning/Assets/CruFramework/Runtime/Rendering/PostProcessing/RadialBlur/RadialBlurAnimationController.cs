#if CRUFRAMEWORK_URP_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    [ExecuteAlways]
    [RequireComponent(typeof(Volume))]
    public class RadialBlurAnimationController : MonoBehaviour
    {

        public bool active = false;
        public Vector2Int center = Vector2Int.zero;
        [Range(0, 32)]
        public int strength = 1;
        [Range(1, 16)]
        public int sampleCount = 1;

        private RadialBlur radialBlur = null;
        private Volume volume = null;

        private void Awake()
        {
            volume = GetComponent<Volume>();
        }

        private void Update()
        {
            if (volume == null) return;

            if (radialBlur == null)
            {
                if (volume.profile.TryGet(typeof(RadialBlur), out RadialBlur value))
                {
                    radialBlur = value;
                }
            }

            if (radialBlur == null) return;

            radialBlur.active = active;
            radialBlur.Center.value = center;
            radialBlur.Strength.value = strength;
            radialBlur.SampleCount.value = sampleCount;
        }
    }
}

#endif