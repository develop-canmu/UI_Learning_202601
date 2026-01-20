#if CRUFRAMEWORK_URP_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CruFramework.Rendering.PostProcessing
{
    [ExecuteAlways]
    [RequireComponent(typeof(Volume))]
    public class InvertColorAnimationController : MonoBehaviour
    {
        public bool active = false;
        [Range(0, 1)]
        public float weight = 0;

        private InvertColor invertColor = null;
        private Volume volume = null;

        private void Awake()
        {
            volume = GetComponent<Volume>();
        }

        private void Update()
        {
            if (volume == null) return;

            if (invertColor == null)
            {
                if (volume.profile.TryGet(typeof(InvertColor), out InvertColor value))
                {
                    invertColor = value;
                }
            }

            if (invertColor == null) return;

            invertColor.active = active;
            invertColor.Weight.value = weight;
        }
    }
}

#endif