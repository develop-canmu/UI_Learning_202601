using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingStatusValuesView : MonoBehaviour
    {
        /// <summary>アニメーション名</summary>
        private static readonly string JumpingAnimation = "Jumping";
        /// <summary>アニメーション名</summary>
        private static readonly string StatusUpAnimation = "OpenStatusUp";
        /// <summary>アニメーション名</summary>
        private static readonly string RankUpAnimation = "Rankup";
        
        /// <summary>カウントアップのアニメーション時間</summary>
        private const float AnimationDuration = 0.5f;
        
        [SerializeField]
        private TrainingStatusValueView staminaValue = null;
        [SerializeField]
        private TrainingStatusValueView speedValue = null;
        [SerializeField]
        private TrainingStatusValueView physicalValue = null;
        [SerializeField]
        private TrainingStatusValueView techniqueValue = null;
        [SerializeField]
        private TrainingStatusValueView intelligenceValue = null;
        [SerializeField]
        private TrainingStatusValueView kickValue = null;

        // 現在のステータス
        private CharacterStatus currentStatus = new CharacterStatus();
        // アニメーション用のステータス
        private CharacterStatus animationStatus = new CharacterStatus();
        // アニメーション時間
        private float animationTime = 0;
        
        private CancellationTokenSource tokenSource = null;
      
        // 待機用タスク
        private Func<UniTask> waitTask = default;
        // 閉じる際のイベント
        private Action onClose = null;
        
        public TrainingStatusValueView GetStatusValue(CharacterStatusType type)
        {
            switch(type)
            {
                case CharacterStatusType.Stamina:return staminaValue;
                case CharacterStatusType.Speed:return speedValue;
                case CharacterStatusType.Physical:return physicalValue;
                case CharacterStatusType.Technique:return techniqueValue;
                case CharacterStatusType.Intelligence:return intelligenceValue;
                case CharacterStatusType.Kick:return kickValue;
            }
            return null;
        }
        
        public Animator GetRankUpAnimator(CharacterStatusType type)
        {
            return GetStatusValue(type)?.RankUpAnimator;
        }
        
        public Animator GetStatusUpAnimator(CharacterStatusType type)
        {
            return GetStatusValue(type)?.ValueUpAnimator;
        }
        
        public TrainingStatusUpView GetStatusUpView(CharacterStatusType type)
        {
            return GetStatusValue(type)?.AnimationUpValueView;
        }
        
        public void SetStatus(CharacterStatus status)
        {
            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                TrainingStatusValueView view = GetStatusValue(type);
                if(view == null)continue;
                view.SetStatus(status[type]);
            }
            
            // 現在のステータス保持
            currentStatus = status;
        }
        
        public void SetStatusWithoutRank(CharacterStatus status)
        {
            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                TrainingStatusValueView view = GetStatusValue(type);
                if(view == null)continue;
                view.SetStatusWithoutRank(status[type]);
            }
        }

        public void SetStatusUpValues(CharacterStatus status)
        {
            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                TrainingStatusValueView view = GetStatusValue(type);
                if(view == null)continue;
                view.UpValueView.SetValue(status[type]);
                view.UpValueView.gameObject.SetActive( status[type] > 0 );
            }
        }
        
        public void PlayStatusUpAnimation(TrainingMainArguments arguments, AppAdvManager adv, TrainingConcentrationEffectBasePlayer concentrationEffect, RectTransform characterEffectRoot, Action onComplete = null)
        {
            PlayStatusUpAnimationAsync(arguments, adv, concentrationEffect, characterEffectRoot, onComplete).Forget();
        }
        
        public async UniTask PlayStatusUpAnimationAsync(TrainingMainArguments arguments, AppAdvManager adv, TrainingConcentrationEffectBasePlayer concentrationEffect, RectTransform characterEffectRoot, Action onComplete = null)
        {
            CharacterStatus status = arguments.Status;
            animationStatus = status;
            // 再生前に初期化しとく
            Cancel();
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            // 通常の待機タスクをセット
            waitTask = async () => { await UniTask.Delay((int)(TrainingUtility.Config.StatusUpAnimationSkipDuration * 1000.0f), cancellationToken: token); };
            
            // Flow時ステータス獲得演出再生
            if (adv.AppAutoMode != AppAdvAutoMode.Skip4 && arguments.IsFlow())
            {
                await PlayFlowZoneStatusUpEffectAsync(arguments, concentrationEffect, characterEffectRoot, token);
            }
             
            // アニメーションの再生
            animationTime = AnimationDuration;
            // ランクアップ
            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                // アニメーター取得
                Animator animator = GetRankUpAnimator(type);
                // ない場合
                if(animator == null)continue;
                // ステータス変動なし
                if(currentStatus[type] == status[type])continue;
                // 現在のランク
                long currentRank = StatusUtility.GetRank(CharaRankMasterStatusType.Status, currentStatus[type]);
                // ステータス上昇後のランク
                long afterRank = StatusUtility.GetRank(CharaRankMasterStatusType.Status, status[type]);
                    
                // 上昇値
                GetStatusUpView(type).SetValue((status[type] - currentStatus[type]));
                    
                animator.SetTrigger(JumpingAnimation);
                // ランクが変わった場合はアニメーション再生
                animator.SetBool(RankUpAnimation, currentRank != afterRank);
            }
                
            // ステータスアップ
            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                Animator animator = GetStatusUpAnimator(type);
                if(animator == null)continue;
                if(currentStatus[type] >= status[type])continue;
                animator.SetTrigger(StatusUpAnimation);
            }
            
            if(onComplete != null)
            {
                // 待機タスクの完了を待つ
                await waitTask.Invoke();
                onComplete();
                waitTask = null;
            }
        }

        /// <summary> Flowゾーン中のステータスUpエフェクト </summary>
        private async UniTask PlayFlowZoneStatusUpEffectAsync(TrainingMainArguments arguments, TrainingConcentrationEffectBasePlayer concentrationEffect, RectTransform characterEffectRoot, CancellationToken token)
        {
            // エフェクト読み込み
            TrainingFlowZoneStatusUpEffect effect = await PageResourceLoadUtility.resourcesLoader.LoadAssetAsync<TrainingFlowZoneStatusUpEffect>(ResourcePathManager.GetPath("FlowZoneStatusGetEffect"), token);
            effect = Instantiate(effect, characterEffectRoot);

            // いったんエフェクトを止める
            concentrationEffect.PlayCloseEffect();
            onClose += () =>
            {
                if (effect != null)
                {
                    Destroy(effect.gameObject);
                }
                
                // 止めたエフェクトを再生
                concentrationEffect.PlayEffect(TrainingConcentrationEffectType.Flow, arguments.GetConcentrationEffectId(), 1.0f);
            };
            
            // エフェクト再生
            await effect.PlayAnimationAsync(token);
            // 待機タスクをFlow演出終了待ちに変更
            waitTask = async () => await effect.WaitAnimationEndAsync(token);
        }

        /// <summary> アニメーション再生中なら中断し表示を切る </summary>
        public void Close()
        {
            // 初期化
            animationTime = 0;
            Cancel();
            gameObject.SetActive(false);
            onClose?.Invoke();
            onClose = null;
        }

        /// <summary> キャンセル </summary>
        private void Cancel()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }
            
            waitTask = null;
        }
        
        private void Update()
        {
            // アニメーション中
            if(animationTime <= 0)return;
            // アニメーション時間
            animationTime -= Time.deltaTime;
            if(animationTime < 0)animationTime = 0;
            
            // 表示するアニメーションステータスの計算
            CharacterStatus s = currentStatus + (animationStatus - currentStatus) * (BigValue.RateValue * (1.0f - animationTime / AnimationDuration));
            
            // ステータス表示
            SetStatusWithoutRank(s);
        }

        private void OnDestroy()
        {
            onClose?.Invoke();
            onClose = null;
        }
    }
}
