using System;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingTargetResult : MonoBehaviour
    {
    
        private enum State
        {
            Open, List, Close
        }
    
        /// <summary>目標達成アニメーション</summary>
        private static readonly string OpenTargetAchievedAnimation = "OpenTargetAchieved";
        /// <summary>目標失敗アニメーション</summary>
        private static readonly string OpenTargetUnachievedAnimation = "OpenTargetUnachieved";
        
        /// <summary>リスト表示アニメーション</summary>
        private static readonly string OpenTargetListAnimation = "OpenTargetList";
        
        /// <summary>閉じるアニメーション</summary>
        private static readonly string CloseAnimation = "Close";
    
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private TrainingTargetView targetView = null;
        [SerializeField]
        private TMP_Text progressText = null;
        
        [SerializeField]
        private TrainingTargetView targetViewPrefab = null;
        [SerializeField]
        private GameObject targetListRoot = null;
        
        [SerializeField]
        private UIButton closeButton = null;
        [SerializeField]
        private UIButton nextButton = null;
        
        [SerializeField]
        private float skipModeSpeed = 2.0f;
        
        private Action onEndAnimation = null;
        
        private State state = State.Open;
        
        private TrainingTargetView[] targetViews = new TrainingTargetView[0];
        
        private bool isAuto = false;
        
        /// <summary>目標の登録</summary>
        private void SetTargetDatas(TrainingGoal[] targets, TrainingGoal currentTarget)
        {
            // 目標表示
            if(currentTarget != null)
            {
                targetView.SetTarget(currentTarget);
            }
            
            
            // 足りない場合は生成
            if(targetViews.Length < targets.Length)
            {
                int length = targetViews.Length;
                Array.Resize(ref targetViews, targets.Length);
                for(int i=length;i<targetViews.Length;i++)
                {
                    targetViews[i] = Instantiate<TrainingTargetView>(targetViewPrefab, targetListRoot.transform);
                }
            }
            
            // ビューの更新
            for(int i=0;i<targetViews.Length;i++)
            {
                if(i >= targets.Length)
                {
                    targetViews[i].gameObject.SetActive(false);
                }
                else
                {
                    targetViews[i].gameObject.SetActive(true);
                    targetViews[i].SetTarget(targets[i]);
                }
            }
            
            int count = 0;
            
            for(int i=0;i<targets.Length;i++)
            {
                if(targets[i].state == (long)TrainingUtility.TargetState.Completed)
                {
                    count++;
                }
            }
            
            // 進捗テキスト更新
            progressText.text = string.Format(StringValueAssetLoader.Instance["training.target.progress"], count, targets.Length);

        }
        
        private async UniTask SetAnimatorTrigger(string trigger, Action onCompleted)
        {
            // 1フレーム待たないとAnimatorのSpeedが反映されないので待つ
            await UniTask.DelayFrame(1);
            await AnimatorUtility.WaitStateAsync(animator, trigger);
            // 完了通知
            if(onCompleted != null)
            {
                onCompleted();
            }
        }
        
        /// <summary>目標達成アニメーション</summary>
        public void PlayOpenTargetAnimation(TrainingGoal[] targets, TrainingGoal currentTarget, string animName, bool autoSkip, Action onEnd)
        {
            // 自動モード
            isAuto = autoSkip;
            // ステート初期化
            state = State.Open;
            // 終了コールバック
            onEndAnimation = onEnd;
            // アニメーション速度
            animator.speed = isAuto ? skipModeSpeed : 1.0f;
            // ボタン表示
            closeButton.gameObject.SetActive( isAuto == false );
            nextButton.gameObject.SetActive( isAuto == false );
            
            // 目標データの設定
            SetTargetDatas(targets, currentTarget);
            gameObject.SetActive(true);
            SetAnimatorTrigger(animName, isAuto ? OnNextButton : null).Forget();
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoChoiceAsync().Forget();
#endif
        }
        
        /// <summary>目標達成アニメーション</summary>
        public void PlayOpenTargetAchieved(TrainingGoal[] targets, TrainingGoal currentTarget, bool autoSkip, Action onEnd)
        {
            PlayOpenTargetAnimation(targets, currentTarget, OpenTargetAchievedAnimation, autoSkip, onEnd);
        }
        
        /// <summary>目標失敗アニメーション</summary>
        public void PlayOpenTargetUnachieved(TrainingGoal[] targets, TrainingGoal currentTarget, bool autoSkip, Action onEnd)
        {
            PlayOpenTargetAnimation(targets, currentTarget, OpenTargetUnachievedAnimation, autoSkip, onEnd);
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private async UniTask DebugAutoChoiceAsync()
        {
            if (!TrainingChoiceDebugMenu.EnabledAutoChoiceTarget) return;

            await UniTask.DelayFrame(1);
            await OnNextButtonAsync();
            OnCloseButton();
        }
#endif
        

        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            // リスト表示アニメーション
            OnNextButtonAsync().Forget();
            
        }
        
        public async UniTask OnNextButtonAsync()
        {
            if(state != State.Open)return;
            state = State.List;
            await AnimatorUtility.WaitStateAsync(animator, OpenTargetListAnimation);
            
            // 自動モードの場合はそのまま閉じる
            if(isAuto)
            {
                await OnCloseButtonAsync();
            }
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnCloseButton()
        {
            OnCloseButtonAsync().Forget();
        }
        
        private async UniTask OnCloseButtonAsync()
        {
            if(state != State.List)return;
            state = State.Close;
            
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            if(onEndAnimation != null)
            {
                onEndAnimation();
            }
        }
    }
}
