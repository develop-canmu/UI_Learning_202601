using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public abstract class TrainingEffectBase : MonoBehaviour
    {
        private long currentEffectId = -1;
        private Animator currentEffect = null;
     
        /// <summary>リソース</summary>
        protected abstract string ResourceKey{get;}
        /// <summary>初期化ステート</summary>
        protected virtual string InitializeAnimationName{get{return string.Empty;}}
        
        public async UniTask SetEffectAsync(long effectId)
        {
            // 同じエフェクト
            if(currentEffectId == effectId)return;

            // 既に再生中のエフェクトをOff
            if(currentEffect != null)
            {
                GameObject.Destroy(currentEffect.gameObject);
                currentEffect = null;
            }
            
            // 再生なし
            if(effectId < 0)return;
            // 再生したIdを保持
            currentEffectId = effectId;
            
            // エフェクトの読み込み
            await PageResourceLoadUtility.LoadAssetAsync<GameObject>( ResourcePathManager.GetPath(ResourceKey, effectId), (effect)=>
                {
                    currentEffect = GameObject.Instantiate<GameObject>(effect, transform).GetComponent<Animator>();
                    // エフェクトを再生
                    if(currentEffect != null)
                    {
                        currentEffect.gameObject.SetActive(true);
                        if(string.IsNullOrEmpty(InitializeAnimationName) == false)
                        {
                            currentEffect.Play(InitializeAnimationName);
                        }
                    }
                },
                gameObject.GetCancellationTokenOnDestroy()
            );
        }
        
        /// <summary>開くアニメーション</summary>
        public UniTask PlayOpenEffectAsync(long effectId, float speed, bool isWait = false)
        {
            return PlayEffectAsync(effectId, "Open", speed, isWait);
        }
        
        /// <summary>アニメーション</summary>
        public async UniTask PlayEffectAsync(long effectId, string animationName, float speed, bool isWait = false)
        {
            // エフェクトをセット
            await SetEffectAsync(effectId);
            // 開く
            if(currentEffect != null)
            {
                currentEffect.speed = speed;
                currentEffect.gameObject.SetActive(true);
                // 終わるまで待機
                if(isWait)
                {
                    await AnimatorUtility.WaitStateAsync(currentEffect, animationName, gameObject.GetCancellationTokenOnDestroy());
                }
                else
                {
                    currentEffect.Play(animationName);
                }
            }
        }
        
        /// <summary>閉じるアニメーション</summary>
        public void PlayCloseEffect()
        {
            if(currentEffect != null)
            {
                currentEffect.Play("Close");
                currentEffectId = -1;
            }
        }
        
        /// <summary>エフェクトを非表示</summary>
        public void ShowEffect(bool show)
        {
            if(currentEffect != null)
            {
                currentEffect.gameObject.SetActive(show);
            }
        }
    }
}
