using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Pjfb
{
    [ExecuteAlways]
    [RequireComponent(typeof(Volume))]
    public class ColorAdjustmentsAnimationController : MonoBehaviour
    {
        [Range(-100f, 100f)]
        public float saturation = 0;
        
        private ColorAdjustments colorAdjustments = null;
        private Volume volume = null;
        
        private void Awake()
        {
            volume = GetComponent<Volume>();
        }

        private void Update()
        {
            if(volume == null) return;
            
            if(colorAdjustments == null)
            {
                if(volume.profile.TryGet(typeof(ColorAdjustments), out ColorAdjustments value))
                {
                    colorAdjustments = value;
                }
            }
            
            if(colorAdjustments == null) return;
            
            colorAdjustments.saturation.value = saturation;
        }
    }
}