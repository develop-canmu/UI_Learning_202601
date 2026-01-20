using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Pjfb.Gacha
{
    public class GachaResultRushEffect : MonoBehaviour
    {
        // フレーム色変更用構造体
        [Serializable]
        private struct RushEffectColorSetting
        {
            public long effectId;
            public Color color;
            public ParticleSystem.MinMaxGradient particleStartColor;
            public ParticleSystem.MinMaxGradient particleColorOverLifeTime;
            public EffectType effectType;
        }
        
        // チャンス演出の種類
        private enum EffectType
        {
            Normal,
            Rare,
            HardLight,
        }

        // アニメーションの段階
        private enum RushAnimationPhase
        {
            OpenRushActivate_1 = 1,
            OpenRushActivate_2 = 2,
        }

        private readonly int effectInterval = 10;
        private float skipCountTimer = 0.0f;
        
        // stateのトリガー名
        private readonly string RushAnimationNormalTriggerName = "OpenRushActivateNormal_{0}";
        private readonly string RushAnimationRareTriggerName = "OpenRushActivateRare_{0}";
        private readonly string RushAnimationHardLightTriggerName = "OpenRushActivateHardLight_{0}";
        private readonly string RushAnimationIdleTriggerName = "Idle";
        private readonly string RushAnimationCloseTriggerName = "Close";
        private readonly string RushAnimationRushEndTriggerName = "OpenRushEnd";
        
        [SerializeField]
        GachaRushImageWebTexture _imageTexture = null;
        [SerializeField]
        GachaRushLogoWebTexture _logoTexture = null;
        [SerializeField]
        GachaRushBGWebTexture _bgTexture = null;
        [SerializeField]
        private Image frameImage = null;
        [SerializeField]
        GachaRushFinishedImageWebTexture _finishedImageTexture = null;
        [SerializeField]
        GachaRushFinishedBGWebTexture _finishedBgTexture = null;

        [SerializeField]
        Animator _rushAnimator = null;
        
        [SerializeField]
        private ParticleSystem[] fxParticles = null;
        
        [Header("終了演出の色")]
        [SerializeField]
        private Color finishRushEffectColor;
        [Header("エフェクトの色設定")]
        [SerializeField]
        private RushEffectColorSetting[] rushEffectColorSettings;

        List<GachaRushData> rushList = null;
        GachaCategoryData gachaCategoryData = null;
        bool waitClick = false;

        public async UniTask Init( GachaCategoryData gachaCategoryData, List<GachaRushData> rushList ){
            this.rushList = rushList;
            this.gachaCategoryData = gachaCategoryData;
            if( rushList != null && rushList.Count > 0 ) {
                foreach( var rushData in rushList ){
                    var id = rushData.imageNumber;
                    if( rushData.isFinished == 0 ) {
                        await _imageTexture.SetTextureAsync(id);
                        await _logoTexture.SetTextureAsync(id);
                        await _bgTexture.SetTextureAsync(id);
                    } else {
                        await _finishedImageTexture.SetTextureAsync(id);
                        await _finishedBgTexture.SetTextureAsync(id);
                    }
                }
            }
            _bgTexture.gameObject.SetActive(false);
            _imageTexture.gameObject.SetActive(false);
            _logoTexture.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            if (skipCountTimer <= 0.0f) return;
            
            // タイマー減算
            skipCountTimer -= Time.deltaTime;
            if (skipCountTimer <= 0.0f)
            {
                waitClick = true;
            }
        }

        public void OnClick()
        {
            if (waitClick)
            {
                waitClick = false;

                // ステートの最後にスキップ
                AnimatorStateInfo currentState = _rushAnimator.GetCurrentAnimatorStateInfo(0);
                _rushAnimator.Play(currentState.shortNameHash, 0, 1.0f);
            }
        }

        public bool IsPlayEffect( ){
            return rushList != null && rushList.Count > 0;
        }
        public async UniTask PlayEffect( CancellationToken token ){
            if( rushList == null || rushList.Count <= 0 ) {
                return;
            }

            this.gameObject.SetActive(true);

            
            foreach( var rushData in rushList ){
                
                var imageNumber = rushData.imageNumber;
                var effectNumber = rushData.effectNumber;

                if( rushData.isFinished > 0 ) {
                    await PlayFinishedRushEffect(imageNumber, token);
                } else {
                    await PlayRushEffect(imageNumber, effectNumber,token);
                }
                await UniTask.Delay(effectInterval, cancellationToken:token);
            }
            this.gameObject.SetActive(false);
        }


        private async UniTask PlayRushEffect( long imageNumber, long effectNumber, CancellationToken token )
        {
            // 変数リセット
            ResetSkipVariable();
            
            // 各チャンスアニメーション設定
            _imageTexture.SetTexture(imageNumber);
            _logoTexture.SetTexture(imageNumber);
            string stateName = null;
            string triggerName = null;
            
            // 演出種類の識別
            bool isStateSet = false;
            foreach (var setting in rushEffectColorSettings)
            {
                if (setting.effectId == effectNumber)
                {
                    // state設定
                    switch (setting.effectType)
                    {
                        case EffectType.Normal:
                            stateName = RushAnimationNormalTriggerName;
                            break;
                        case EffectType.Rare:
                            stateName = RushAnimationRareTriggerName;
                            break;
                        case EffectType.HardLight:
                            stateName = RushAnimationHardLightTriggerName;
                            break;
                        default:
                            stateName = string.Empty;
                            CruFramework.Logger.LogError($"EffectType:{setting.effectType} が存在しません");
                            break;
                    }
                    triggerName = string.Format(stateName, (int)RushAnimationPhase.OpenRushActivate_1);
                    
                    // 演出によってフレームの色を変更
                    frameImage.color = setting.color;
                    
                    // パーティクルの色
                    // アクティブの制御はアニメーションに任せるので全てのパーティクルに適用
                    for(int i = 0; i < fxParticles.Length; i++)
                    {
                        ParticleSystem.MainModule main = fxParticles[i].main;
                        main.startColor = setting.particleStartColor;
                        
                        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = fxParticles[i].colorOverLifetime;
                        colorOverLifetime.color = setting.particleColorOverLifeTime;
                    }
                    
                    isStateSet = true;
                    break;
                }
            }
            if (!isStateSet)
            {
                CruFramework.Logger.LogError($"effectNumber:{effectNumber} の演出が存在しません");
            }
            
            // チャンス開始ステート
            ResetSkipVariable();
            _rushAnimator.SetTrigger(triggerName);
            await AnimatorUtility.WaitStateFinishAsync(_rushAnimator, triggerName, token);
            
            // ガチャ固有画像ステート
            ResetSkipVariable();
            triggerName = string.Format(stateName, (int)RushAnimationPhase.OpenRushActivate_2);
            _rushAnimator.SetTrigger(triggerName);
            await AnimatorUtility.WaitStateFinishAsync(_rushAnimator, triggerName, token);
            
            // NEXT画面のクリック待機（Idleステート）
            waitClick = true;
            skipCountTimer = 0.0f;
            _rushAnimator.SetTrigger(RushAnimationIdleTriggerName);
            await UniTask.WaitWhile(()=>waitClick, cancellationToken:token);
            
            // Closeステート
            _rushAnimator.SetTrigger(RushAnimationCloseTriggerName);
            await AnimatorUtility.WaitStateFinishAsync(_rushAnimator, RushAnimationCloseTriggerName, token);
        }

        private async UniTask PlayFinishedRushEffect( long imageNumber, CancellationToken token )
        {
            // 変数リセット
            ResetSkipVariable();
            
            _finishedImageTexture.SetTexture(imageNumber);
            _finishedBgTexture.SetTexture(imageNumber);
            
            // 終了時はフレームの色を固定
            frameImage.color = finishRushEffectColor;
            
            // チャンス終了アニメーション
            _rushAnimator.SetTrigger(RushAnimationRushEndTriggerName);
            await AnimatorUtility.WaitStateFinishAsync(_rushAnimator, RushAnimationRushEndTriggerName, token);
        }

        private void ResetSkipVariable()
        {
            skipCountTimer = ConfigManager.Instance.gachaEffectTimeUntilSkippable;
            
            if (skipCountTimer > 0.0f)
            {
                waitClick = false;
            }
            else
            {
                // 設定値が0の場合は、waitClickをtrueで維持
                waitClick = true;
            }
        }
    }
}
