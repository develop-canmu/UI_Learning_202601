using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;


namespace Pjfb.Training
{
    public abstract class TrainingConcentrationEffectBase : MonoBehaviour
    { 
        public abstract void PlayEffect(int effectId, float timeScale);
        public abstract UniTask PlayEffectAsync(int effectId, float timeScale, bool isWait = false, CancellationToken token = default);
        public abstract void SetEffect(int effectId);
        public abstract void PlayCloseEffect();
        public abstract void ShowEffect(bool show);

        /// <summary> 演出タイプ </summary>
        public abstract TrainingConcentrationEffectType EffectType { get; }

        /// <summary> 再生中か？ </summary>
        public abstract bool IsPlaying { get; }
    }

    /// <summary> 演出タイプ </summary>
    public enum TrainingConcentrationEffectType
    {
        // Cゾーン
        Concentration,
        // Flowゾーン
        Flow,
    }
    
    public abstract class TrainingConcentrationEffectBase<TConfig> : TrainingConcentrationEffectBase where TConfig : ScriptableObject
    {     
        [SerializeField]
        private Animator animator = null;
        
        // 同一エフェクトの再生を許可するか
        [SerializeField]
        private bool allowReplaySameEffect = false;
        
        // 読み込んだエフェクト
        private int loadedEffectId = -1;
        
        private TConfig loadedConfig = null;
        /// <summary>読み込んだ設定ファイル</summary>
        public TConfig LoadedConfig{get{return loadedConfig;}}
        
        /// <summary>再生中</summary>
        public override bool IsPlaying{get{return loadedEffectId >= 0;}}
        
        /// <summary>設定ファイルの読み込みパス</summary>
        protected abstract string ConfigResourceKey{get;}
        
        /// <summary>設定ファイルの登録</summary>
        protected abstract void SetConfig(TConfig config);
        
        /// <summary>設定ファイル読み込み前</summary>
        protected virtual void OnPreLoadConfig(){}

        /// <summary> 演出タイプ </summary>
        public override TrainingConcentrationEffectType EffectType => TrainingConcentrationEffectType.Concentration;
        
        /// <summary>エフェクトの再生</summary>
        public override void PlayEffect(int effectId, float timeScale)
        {
            PlayEffectAsync(effectId, timeScale, token: destroyCancellationToken).Forget();
        }
        
        /// <summary>エフェクトの再生</summary>
        public override UniTask PlayEffectAsync(int effectId, float timeScale, bool isWait = false, CancellationToken token = default)
        {
            return PlayEffectAsync(effectId, "Open", timeScale, isWait, token);
        }
        
        /// <summary>エフェクトの再生</summary>
        public async UniTask PlayEffectAsync(int effectId, string animationName, float timeScale, bool isWait = false, CancellationToken token = default)
        {
            // 指定なし
            if(effectId < 0)return;
            // 同一エフェクトか？
            bool isSameEffect = loadedEffectId == effectId;
            // 同じエフェクトは再生しない
            if(isSameEffect && allowReplaySameEffect == false)return;
            loadedEffectId = effectId;
            // 再生速度
            animator.speed = timeScale;
            // エフェクトの設置
            if (isSameEffect == false)
            {
                await SetEffectAsync(effectId, token);
            }

            // アクティブか
            gameObject.SetActive(true);
            // アニメーション再生
            await PlayAnimationAsync(animationName, isWait, token);
        }
        
        private async UniTask PlayAnimationAsync(string animationName, bool isWait, CancellationToken token)
        {
            // アニメーション再生
            if(isWait)
            {
                await AnimatorUtility.WaitStateAsync(animator, animationName, token);
            }
            else
            {
                animator.Play(animationName);
            }
        }
        
        /// <summary>エフェクトの設定</summary>
        public override void SetEffect(int effectId)
        {
            SetEffectAsync(effectId, destroyCancellationToken).Forget();
        }
        
        /// <summary>エフェクトの設定</summary>
        public async UniTask SetEffectAsync(int effectId, CancellationToken token = default)
        {
            // 設定を初期化
            loadedConfig = null;
            
            // 読み込み前の通知
            OnPreLoadConfig();
            
            // エフェクトなし
            if(effectId < 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            // 設定ファイルのの読み込み
            await PageResourceLoadUtility.LoadAssetAsync<TConfig>( ResourcePathManager.GetPath(ConfigResourceKey, effectId), (config)=>
                {
                    loadedConfig = config;
                    // 設定
                    SetConfig(config);
                },
                token);
        }
        
        public override void PlayCloseEffect()
        {
            PlayCloseEffectAsync(destroyCancellationToken).Forget();
        }
        
        public async UniTask PlayCloseEffectAsync(CancellationToken token)
        {
            await PlayAnimationAsync("Close", true, token);
            loadedEffectId = -1;
            gameObject.SetActive(false);
        }
        
        /// <summary>エフェクトを非表示</summary>
        public override void ShowEffect(bool show)
        {
            gameObject.SetActive(show);
            // 非表示にした場合は読み込んだIdを初期化
            if(show == false)
            {
                loadedEffectId = -1;
            }
        }
    }
}