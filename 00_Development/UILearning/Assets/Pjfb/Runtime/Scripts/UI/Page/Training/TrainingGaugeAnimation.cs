using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingGaugeAnimation
    {
        public TrainingGaugeAnimation(float duration)
        {
            this.duration = duration;
        }
        
        private float duration = 0;
        /// <summary>時間</summary>
        public float Duration{get{return duration;}}
        
        private float currentValue = 0;
        /// <summary>現在の値</summary>
        public float CurrentValue{get{return currentValue;}}
        
        private float targetValue = 0;
        /// <summary>目標</summary>
        public float TargetValue{get{return targetValue;}}
        
        private float time = 0;
        
        /// <summary>値の設定</summary>
        public void SetValue(float current, float target)
        {
            currentValue = current;
            targetValue = target;
            time = 0;
        }
        
        
        /// <summary>更新</summary>
        public float Update()
        {
            time = Mathf.Min(duration, time + Time.deltaTime);
            currentValue = Mathf.Lerp(currentValue, targetValue, time / duration );
            return currentValue;
        }        
    }
}