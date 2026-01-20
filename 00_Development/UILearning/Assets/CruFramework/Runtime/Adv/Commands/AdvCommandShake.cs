using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public abstract class AdvCommandShake : IAdvCommand
    {
        [SerializeField]
        [AdvDocument("揺らす時間。")]
        protected float duration = 0.5f;
       
        [SerializeField]
        [AdvDocument("揺らす強さ。")]
        protected float stlength = 10.0f;
        
        [SerializeField]
        [AdvDocument("揺らす速さ。")]
        protected int vibrato = 10;
        
        [SerializeField]
        [AdvDocument("アニメーションが終わるまで待つ。")]
        private bool isWait = true;
        
        /// <summary>揺らす対象</summary>
        protected abstract Transform GetTargetTransform(AdvManager manager);
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            Transform t = GetTargetTransform(manager);
            t.DOShakePosition(duration, stlength, vibrato).onUpdate += ()=>
            {
                Vector3 p = t.localPosition;
                p.z = 0;
                t.localPosition = p;
            };
            
            if(isWait)
            {
                manager.SetWaitTime(duration);
            }
        }
    }
}