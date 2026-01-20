using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    /// <summary> コンセントレーション演出ベースページ </summary>
    public abstract class TrainingConcentrationEffectBasePage : TrainingPageBase
    {
#if UNITY_EDITOR
        ///<summary>デバッグ用のリセット関数</summary>
        public abstract void ResetEffectId();
#endif
    }
    
    public class TrainingConcentrationEffectPage : TrainingConcentrationEffectBasePage
    {
        private const string AnimationOpen = "Open";
        private const string AnimationEnd = "End";
        
        [SerializeField]
        private Animator zoneEndAnimator = null;
        
        [SerializeField]
        private TrainingConcentrationZoneEnterEffect enterEffect = null;
        [SerializeField]
        private TrainingConcentrationZoneExtraEffect extraEffect = null;
        [SerializeField]
        private TrainingConcentrationZoneUpGradeEffect upGradeEffect = null;
        
        [SerializeField]
        private float skipModeSpeed = 2.0f;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            PlayAnimationAsync().Forget();
        }
        
        private async UniTask PlayAnimationAsync()
        {
            // タッチガード
            SetTouchGurad(true);
            // 再生速度
            float timeScale = Adv.IsSkipOrFastMode ? skipModeSpeed : 1.0f;
            // エフェクトId
            int effectId = MainArguments.GetConcentrationEffectId();
            
            if(MainArguments.Reward != null)
            {
                // ターン延長
                if(MainArguments.Reward.isConcentrationExtended)
                {
                    // ログを追加
                    Adv.AddMessageLog(MainArguments.ActionName, StringValueAssetLoader.Instance["training.concentration.ex"], 0);
                    // スキップ
                    if(IsConcentrationEffectSkip)
                    {
                        SetMessage( StringValueAssetLoader.Instance["training.message.concentration.extra"] );
                    }
                    else
                    {
                        // 表示
                        await extraEffect.PlayEffectAsync(effectId, timeScale, true);
                    }
                }
                // グレードアップ
                else if(MainArguments.Reward.isConcentrationGradeUp)
                {
                    // ログを追加
                    Adv.AddMessageLog(MainArguments.ActionName, StringValueAssetLoader.Instance["training.concentration.up"], 0);
                    // スキップ
                    if(IsConcentrationEffectSkip)
                    {
                        SetMessage( StringValueAssetLoader.Instance["training.message.concentration.upgrade"] );
                    }
                    else
                    {
                        // 表示
                        await upGradeEffect.PlayEffectAsync(effectId, timeScale, true);
                    }
                }
                // 開始
                else if(MainArguments.Reward.mTrainingConcentrationId != 0)
                {
                    // ログを追加
                    Adv.AddMessageLog(MainArguments.ActionName, StringValueAssetLoader.Instance["training.concentration.begin"], 0);
                    // コンセントレーションのエフェクトを出す
                    await MainPageManager.ConcentrationZoneEffectPlayer.PlayEffectAsync(TrainingConcentrationEffectType.Concentration, effectId, timeScale);
                    
                    // スキップ
                    if(IsConcentrationEffectSkip)
                    {
                        SetMessage( StringValueAssetLoader.Instance["training.message.concentration"] );
                    }
                    else
                    {
                        // 表示
                        await enterEffect.PlayEffectAsync(effectId, timeScale, true);
                    }

                }
            }

            // 終了
            if(MainArguments.Pending.isFinishedConcentration)
            {
                // ログを追加
                Adv.AddMessageLog(MainArguments.ActionName, StringValueAssetLoader.Instance["training.concentration.end"], 0);
                // 表示
                zoneEndAnimator.gameObject.SetActive(true);
                // 再生速度
                zoneEndAnimator.speed = timeScale;
                MainPageManager.ConcentrationZoneEffectPlayer.PlayCloseEffect();
                
                // スキップ
                if(IsConcentrationEffectSkip)
                {
                }
                else
                {
                    // アニメーションを待機
                    await AnimatorUtility.WaitStateAsync(zoneEndAnimator, AnimationEnd);
                }
                // 非表示
                zoneEndAnimator.gameObject.SetActive(false);
            }
            
            // 発生コンセントレーションをチェック済み
            MainArguments.OptionFlags |= TrainingMainArguments.Options.ConcentrationEffectEnd;

            // タッチガード
            SetTouchGurad(false);
            // Topへ戻る
            OpenPage(TrainingMainPageType.Top, MainArguments);
        }
#if UNITY_EDITOR
        ///<summary>デバッグ用のリセット関数</summary>
        public override void ResetEffectId()
        {
            enterEffect.ShowEffect(false);
            extraEffect.ShowEffect(false);
            upGradeEffect.ShowEffect(false);
        }
#endif
    }
}
