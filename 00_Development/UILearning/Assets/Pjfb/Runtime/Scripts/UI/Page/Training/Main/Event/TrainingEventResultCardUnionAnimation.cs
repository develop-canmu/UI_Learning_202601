using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb
{
    /// <summary> カードユニオン演出 </summary>
    public class TrainingEventResultCardUnionAnimation : MonoBehaviour
    {
        /// <summary> 演出再生モード </summary>
        private enum EffectMode
        {
            // 通常再生
            Normal,
            // 自動再生
            Auto,
            // スキップ
            Skip,
        }

        /// <summary> アニメーションのステート待機タイプ </summary>
        private enum AnimationWaitType
        {
            // ステート変更まで待機
            StateChange,
            // ステート終了まで待機
            StateFinish,
        }
        
        /// <summary> カード表示アニメーション </summary>
        private static readonly string OpenTrainingCardKey = "OpenMainTrainingCard";

        /// <summary> カード表示アニメーション </summary>
        private static readonly string OpenTrainingSubCardKey = "OpenSubTrainingCard";

        /// <summary> カードの表示後の待機ステートキー </summary>
        private static readonly string IdleKey = "Idle";
        
        /// <summary> 対象カード数が一定数以上の場合のアニメーションキー </summary>
        private static readonly string OpenTrainingSubOverKey = "OpenTrainingCardUnionOver";
        
        /// <summary> カードユニオン演出 </summary>
        private static readonly string OpenTrainingCardUnionKey = "OpenTrainingCardUnion";

        /// <summary> トータルボーナス演出 </summary>
        private static readonly string OpenTrainingTotalBonusKey = "OpenCardUnionTotalBonus";

        [SerializeField]
        private Animator animator = null;

        // メインカード
        [SerializeField]
        private TrainingCardUnionPracticeCard mainCard = null;
        
        // サブカード
        [SerializeField]
        private TrainingCardUnionPracticeCard[] subCardList = null;

        // サブカードが一定数以上の場合の表示用カードリスト
        [SerializeField]
        private TrainingCardUnionPracticeCard[] unionOverCardList = null;
        
        // FlowLvBonus
        [SerializeField]
        private TMP_Text flowLvBonus = null;

        // スタミナ
        [SerializeField]
        private TMP_Text stamina = null;
        
        // スピード
        [SerializeField]
        private TMP_Text speed = null;
        
        // フィジカル
        [SerializeField]
        private TMP_Text physical = null;
        
        // テクニック
        [SerializeField]
        private TMP_Text technique = null;
        
        // 賢さ
        [SerializeField]
        private TMP_Text intelligence = null;
        
        // キック
        [SerializeField]
        private TMP_Text kick = null;
        
        // 獲得ステータス用のテキスト設定データ
        [SerializeField]
        private OmissionTextSetter rewardOmissionTextSetter = null;
        
        // NextButton
        [SerializeField]
        private UIButton nextButton = null;

        // 自動送り時の待機秒数
        [SerializeField]
        private float autoNextSeconds = 1.0f;
        
        private TrainingMainArguments arguments;
        // キャンセルトークンソース
        private CancellationTokenSource cancelTokenSource = null;

        // 演出再生モード
        private EffectMode effectMode = EffectMode.Normal;

        // 演出再生中か？
        private bool isPlayingEffect = true;
        
        // モーダル表示中か？
        private bool isOpenModal = false;
        
        // 演出完了済みか？
        private bool isComplete = false;
        
        // 演出終了後の処理
        private Action onComplete = null; 
        
        /// <summary> 演出再生 </summary>
        public void PlayAnimation(TrainingMainArguments mainArguments, Adv.AppAdvAutoMode autoMode, Action onComplete)
        {
            PlayAnimationAsync(mainArguments, autoMode, onComplete).Forget();
        }

        private async UniTask PlayAnimationAsync(TrainingMainArguments mainArguments, Adv.AppAdvAutoMode autoMode, Action onComplete)
        {
            gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            this.arguments = mainArguments;
            
            // 演出再生モードを決定
            if (autoMode == AppAdvAutoMode.Skip4)
            {
                effectMode = EffectMode.Skip;
            }
            else if (autoMode == AppAdvAutoMode.Fast || autoMode == AppAdvAutoMode.Skip3)
            {
                effectMode = EffectMode.Auto;
            }
            else
            {
                effectMode = EffectMode.Normal;
            }
            
            cancelTokenSource = new CancellationTokenSource();
            isPlayingEffect = true;
            isOpenModal = false;
            isComplete = false;
            this.onComplete = onComplete;

            TrainingUnionCardReward unionReward = mainArguments.Reward.concentrationUnionCard;
            // 獲得ステータス
            CharacterStatus rewardStatus = TrainingUtility.GetStatus(unionReward);
            var omissionData = rewardOmissionTextSetter.GetOmissionData();
            
            // FlowLvBonus表示
            flowLvBonus.text = string.Format(StringValueAssetLoader.Instance["common.plus_percent"] , unionReward.effectRate / 100);
            // 獲得ステータス表示
            stamina.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardStatus.Stamina.ToDisplayString(omissionData));
            speed.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardStatus.Speed.ToDisplayString(omissionData));
            physical.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardStatus.Physical.ToDisplayString(omissionData));
            technique.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardStatus.Technique.ToDisplayString(omissionData));
            intelligence.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardStatus.Intelligence.ToDisplayString(omissionData));
            kick.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardStatus.Kick.ToDisplayString(omissionData));

            // Id降順で並びかえ
            TrainingTrainingCardData[] sortedSubCardDataList = unionReward.trainingCardList.OrderByDescending(x => x.id).ToArray();
            
            List<UniTask> taskList = new List<UniTask>();
            // メインカード
            long inspireEffectId = GetInspireEffectId(mainArguments, unionReward.baseTrainingData.id);
            taskList.Add(mainCard.SetCardAsync(unionReward.baseTrainingData.id, inspireEffectId));
            
            // サブカード
            // カードユニオン対象カードがサブカード数を超えているか
            bool isSubCardOver = sortedSubCardDataList.Length > subCardList.Length;
          
            for (int i = 0; i < subCardList.Length; i++)
            {
                inspireEffectId = GetInspireEffectId(mainArguments, sortedSubCardDataList[i].id);
                taskList.Add(subCardList[i].SetCardAsync(sortedSubCardDataList[i].id, inspireEffectId));
            }

            for (int i = 0; i < unionOverCardList.Length; i++)
            {
                // 表示するデータのIndex
                int rewardIndex = i + subCardList.Length;
                // 表示するデータがあるなら表示
                if(sortedSubCardDataList.Length > rewardIndex)
                {
                    unionOverCardList[i].gameObject.SetActive(true);
                    inspireEffectId = GetInspireEffectId(mainArguments, sortedSubCardDataList[rewardIndex].id);
                    taskList.Add(unionOverCardList[i].SetCardAsync(sortedSubCardDataList[rewardIndex].id, inspireEffectId));
                }
                else
                {
                    unionOverCardList[i].gameObject.SetActive(false);
                }
            }
        
            await UniTask.WhenAll(taskList);
            
            // メインカード表示演出
            await PlayAnimationState(AnimationWaitType.StateFinish, OpenTrainingCardKey, isWaitTap: false);
            // カードユニオン対象カード表示演出
            await PlayAnimationState(AnimationWaitType.StateChange, OpenTrainingSubCardKey, IdleKey);
            // カードユニオン演出
            await PlayAnimationState(AnimationWaitType.StateFinish, isSubCardOver ? OpenTrainingSubOverKey : OpenTrainingCardUnionKey, isWaitTap: false);
            // メインカードエフェクト停止
            mainCard.StopCardEffect();
            // トータルボーナス演出
            await PlayAnimationState(AnimationWaitType.StateFinish, OpenTrainingTotalBonusKey, string.Empty, false, false);
            
            // 終了後の表示。。
            isPlayingEffect = false;
            // 演出完了後はスキップボタンを切る
            nextButton.gameObject.SetActive(false);

            await WaitCloseEffect();
        }
        

        /// <summary> アニメーション再生共通処理 </summary>
        private async UniTask PlayAnimationState(AnimationWaitType waitType, string animationKey, string waitStateKey = "", bool checkSkip = true, bool isWaitNextEffect = true, bool isWaitTap = true)
        {
            // 演出スキップ時
            if (checkSkip && effectMode == EffectMode.Skip)
            {
                return;
            }
            
            // 指定がない場合トリガーのステートで待機
            if (string.IsNullOrEmpty(waitStateKey))
            {
                waitStateKey = animationKey;
            }
         
            try
            {
                animator.SetTrigger(animationKey);
                // ステート待機タイプ
                switch (waitType)
                {
                    // ステート変更まで待機
                    case AnimationWaitType.StateChange:
                    {
                        await AnimatorUtility.WaitStateChangeAsync(animator, waitStateKey, cancelTokenSource.Token);
                        break;
                    }
                    // ステート終了まで待機
                    case AnimationWaitType.StateFinish:
                    {
                        await AnimatorUtility.WaitStateFinishAsync(animator, waitStateKey, cancelTokenSource.Token);
                        break;
                    }
                }
            }
            // キャンセル時はStateの最後までとばす
            catch (OperationCanceledException)
            {
                animator.SkipToEnd(animationKey);
            }

            if (isWaitNextEffect)
            {
                // 次のエフェクト再生まで待機
                await WaitNextEffect(isWaitTap);
            }
        }

        /// <summary> 演出を次に進ませるボタンが押されるまで待機 </summary>
        private async UniTask WaitNextEffect(bool isWaitTap = true)
        {
            // 通常再生時はタップ待機
            if (effectMode == EffectMode.Normal && isWaitTap)
            {
                // キャンセルされるまで待機(キャンセル時の例外は出ない)
               await UniTask.WaitUntilCanceled(cancelTokenSource.Token);
            }
            // それ以外は自動送りの為、一定秒数待機
            else
            {
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(autoNextSeconds), cancellationToken: cancelTokenSource.Token);
                }
                // キャンセル時
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async UniTask WaitCloseEffect()
        {
            // 通常時は自動送りないのでボタン側に処理任せる
            if (effectMode == EffectMode.Normal)
            {
                return;
            }
            
            // 次の処理まで待機
            await WaitNextEffect();
            // 終了時の処理を呼ぶ 
            OnClickChallenge();
        }

        /// <summary> 指定カードのインスピレーション演出Idを取得 </summary>
        private long GetInspireEffectId(TrainingMainArguments mainArguments, long mTrainingCardId)
        {
            long maxGrade = -1;
            long effectNumber = -1;
            
            foreach (TrainingInspire inspire in mainArguments.Pending.inspireList)
            {
                // 対象カードのインスピレーション
                if (inspire.mTrainingCardId == mTrainingCardId)
                {
                    var master = MasterManager.Instance.trainingCardInspireMaster.FindData(inspire.id);
                    
                    // 最もグレードの高いインスピレーションの演出Idに更新
                    if (master.grade > maxGrade)
                    {
                        maxGrade = master.grade;
                        effectNumber = master.effectNumber;
                    }
                }
            }

            return effectNumber;
        }

        /// <summary> カードユニオン情報モーダルを開く </summary>
        public void OnClickCardUnionDetail()
        {
            OnClickCardUnionDetailAsync().Forget();
        }

        private async UniTask OnClickCardUnionDetailAsync()
        {
            // すでに完了済み,演出再生中なら開かない
            if (isComplete || isPlayingEffect)
            {
                return;
            }
            
            // モーダルを開いていることにする
            isOpenModal = true;
            Cancel();
            TrainingCardUnionInformationModal.Arguments args = new TrainingCardUnionInformationModal.Arguments(arguments);
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.CardUnionInformation, args);
            await modal.WaitCloseAsync();
            isOpenModal = false;
        }
        
        
        /// <summary> NextButtonクリック時 </summary>
        public void OnClickNextButton()
        {
            // キャンセルする
            Cancel();
            cancelTokenSource = new CancellationTokenSource();
        }
        
        /// <summary> カードユニオン演出終了後のChallengeボタン </summary>
        public void OnClickChallenge()
        {
            // 完了済み,モーダル表示中,演出再生中ならリターン
            if (isComplete || isOpenModal || isPlayingEffect)
            {
                return;
            }
            
            // 終了前にトークン破棄しとく
            Cancel();
            gameObject.SetActive(false);
            // 演出を閉じる
            onComplete();
            isComplete = true;
        }

        /// <summary> キャンセル </summary>
        private void Cancel()
        {
            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
            }
        }
    }
}