using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using DG.Tweening;
using System;

namespace Pjfb
{
    public class AlphaChangeAnimation
    {
        private float fadeTime = 0.5f;
        /// <summary>フェード時間</summary>
        public  float FadeTime{get{return fadeTime;}}
        
        private float intervalTime = 1.5f;
        /// <summary>インターバル</summary>
        public float IntervalTime{get{return intervalTime;}}
        
        // シーケンス
        private Sequence sequence = null;
        // Index
        private int currentIndex = 0;
        
        // アニメーション対象
        private event Action<int, float, float> animationTargets = null;
        
        private float currentAlpha = 0;
        private float nextAlpha = 0;
        
        
        ~AlphaChangeAnimation()
        {
            Kill();
        }
        
        private void Initialize()
        { 
            if(sequence != null)
            {
                sequence.Kill();
            }
            
            // Index初期化
            currentIndex = 0;
            // アルファ初期化
            currentAlpha = 1.0f;
            nextAlpha = 0;
            
            // シーケンス初期化
            sequence = DOTween.Sequence();
            // アルファ
            sequence.Append( DOTween.To(GetCurrentAlpha, SetCurrentAlpha, 0, fadeTime) ).
                Join( DOTween.To(GetNextAlpha, SetNextAlpha, 1.0f, fadeTime) ).
                OnUpdate(OnUpdate);
            
            // インターバル
            sequence.AppendInterval(intervalTime).AppendCallback(()=> 
            {
                currentIndex++;
            });
            
            sequence.SetLoops(-1);
        }

        private float GetCurrentAlpha()
        {
            return currentAlpha;
        }
        
        private float GetNextAlpha()
        {
            return nextAlpha;
        }
        
        private void SetCurrentAlpha(float alpha)
        {
            currentAlpha = alpha;
        }
        
        private void SetNextAlpha(float alpha)
        {
            nextAlpha = alpha;
        }
        
        private void OnUpdate()
        {
            if(animationTargets != null)
            {
                animationTargets(currentIndex, currentAlpha, nextAlpha);
            }
        }
        

        public void AddAnimationTarget(Action<int, float, float> action)
        {
            animationTargets -= action;
            animationTargets += action;
        }
        
        public void RemoveAnimationTarget(Action<int, float, float> action)
        {
            animationTargets -= action;
        }
        
        public void Kill()
        {
            if(sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }
        
        public void Play()
        {
            Initialize();
            sequence.Play();
        }
    }
}