using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb.Training
{
    public class TrainingHeader : MonoBehaviour
    {
        [SerializeField]
        private GameObject actionPage = null;
        [SerializeField]
        private GameObject advPage = null;
    
        [SerializeField]
        private TrainingEventView eventView = null;
        [SerializeField]
        private TrainingConditionView conditionView = null;
        [SerializeField]
        private TrainingTurnView turnView = null;
        [SerializeField]
        private TrainingHeaderTargetView targetView = null;
        
        [SerializeField]
        private TrainingConcentrationLabelEffectPlayer concentrationLabelEffectPlayer = null;
        /// <summary>コンセントレーションのラベルエフェクト</summary>
        public TrainingConcentrationLabelEffectPlayer ConcentrationLabelEffectPlayer{get{return concentrationLabelEffectPlayer;}}
        
        [SerializeField]
        private TrainingHeaderFlowLevelView flowLevelView = null;
        /// <summary> Flowレベルの表示 </summary>
        public TrainingHeaderFlowLevelView FlowLevelView => flowLevelView;
        
        [SerializeField]
        private TrainingPerformanceView performaceView = null;
        /// <summary>パフォーマンス</summary>
        public TrainingPerformanceView PerformaceView{get{return performaceView;}}
        
        [SerializeField]
        private TrainingHeaderInspirationView inspirationView = null;
        
        [SerializeField]
        private TrainingHeaderBoostButtonView boostView = null;
        /// <summary>ブースト情報表示</summary>
        public TrainingHeaderBoostButtonView BoostView{get{return boostView;}}

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void SetActiveAllPage()
        {
            advPage.SetActive(true);
            actionPage.SetActive(true);
        }
        
        public void SetActiveActionPage()
        {
            advPage.SetActive(false);
            actionPage.SetActive(true);
        }
        
        public void SetActiveAdvPage()
        {
            advPage.SetActive(true);
            actionPage.SetActive(false);
        }
        
        public void ShowCoentrationLabelEffect(bool show)
        {
            concentrationLabelEffectPlayer.ShowEffect(show);
        }
        
        public void ShowEventView(bool show)
        {
            eventView.gameObject.SetActive(show);
        }
        
        public void ShowEventView(string eventName)
        {
            // イベント表示
            eventView.SetEvent(0, string.Empty, eventName);
            eventView.gameObject.SetActive(true);
        }
        
        public void SetView(TrainingMainArguments mainArguments)
        {
            UpdateCondition(mainArguments.Pending);
            
            // mEvent
            if(mainArguments.TrainingEvent.mTrainingEventId > 0)
            {
                TrainingEventMasterObject mEvent = MasterManager.Instance.trainingEventMaster.FindData( mainArguments.TrainingEvent.mTrainingEventId );
                // イベント表示
                eventView.SetEvent(mEvent.supportMCharaId, string.Empty, mEvent.name);
            }
            
            // 目標
            targetView.SetTarget(mainArguments);
            
            // パフォーマンス
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(mainArguments.Pending.mTrainingScenarioId);
            if(TrainingUtility.IsEnablePerformace(mScenario.id))
            {                
                performaceView.gameObject.SetActive(true);
                performaceView.SetTipCount(mainArguments.CurrentTipCount);
                performaceView.Set(mainArguments.Pending.overallProgress, 0, true);
            }
            else
            {
                performaceView.gameObject.SetActive(false);
                // 中でキャッシュしてたりするので一応読んでおく
                // サーバーから正しいデータが来れば不要ではある。。。
                performaceView.Set(mainArguments.Pending.overallProgress, 0, true);
            }

            // 非表示
            inspirationView.gameObject.SetActive(false);
            flowLevelView.gameObject.SetActive(false);
            
            switch (mScenario.ScenarioType)
            {
                // インスピレーション表示
                case TrainingScenarioType.Concentration:
                {
                    inspirationView.gameObject.SetActive(true);
                    inspirationView.SetView(mainArguments);
                    concentrationLabelEffectPlayer.ShowEffect(true);
                    break;
                }
                // Flowポイント表示
                case TrainingScenarioType.Flow:
                {
                    flowLevelView.gameObject.SetActive(true);
                    flowLevelView.UpdateView(mainArguments);
                    concentrationLabelEffectPlayer.ShowEffect(true);
                    break;
                }
                default:
                {
                    concentrationLabelEffectPlayer.ShowEffect(false);
                    break;
                }
            }
        }

        public void UpdateTurn(TrainingMainArguments mainArguments)
        {
            var mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(mainArguments.Pending.mTrainingScenarioId);
            if (mainArguments.CurrentTarget == null)
            {
                turnView.SetTurn(Mathf.Clamp((int)(mScenario.trainingTurn - mainArguments.Pending.turn), 0, (int)mScenario.trainingTurn), mScenario.trainingTurn);
                return;
            }

            if (mainArguments.IsNormalTurnCurrent())
            {
                if (mainArguments.HasExtraTurnCurrentTarget())
                {
                    turnView.SkipToEndAddExtraTurnRightEffectAsync(mainArguments.CurrentTargetRestTurn, mainArguments.CurrentTargetTurn, mainArguments.Pending.turnAddEffectType).Forget();
                }
                else
                {
                    turnView.SetTurn(mainArguments.CurrentTargetRestTurn, mainArguments.CurrentTargetTurn);
                }
            }
            else if (mainArguments.IsFirstExtraTurnCurrent())
            {
                turnView.SkipToEndEnterExtraTurnEffectAsync(mainArguments.CurrentTarget.restFirstAddedTurnCount, mainArguments.CurrentTarget.firstAddedTurn, mainArguments.CurrentTarget.addedTurn, mainArguments.Pending.turnAddEffectType).Forget();
            }
            else if (mainArguments.IsContinueExtraTurnCurrent())
            {
                turnView.SkipToEndContinueExtraTurnEffectAsync().Forget();
            }
        }

        public void UpdateCondition(TrainingPending pending)
        {
            // コンディション
            conditionView.SetCondition(pending.mTrainingScenarioId, pending.condition, pending.conditionType);
        }
        
        public void PlayNormalTurnEffect(long current, long max)
        {
            turnView.SetTurn(current, max);
        }
        
        public void PlayHasExtraTurnRightEffect(long current, long max, long effectType)
        {
            turnView.SetTurnHasExtraTurnRight(current, max, effectType);
        }
        
        public void PlayFirstExtraTurnEffect(long current, long max, long addedTurn, long effectType)
        {
            turnView.SkipToEndEnterExtraTurnEffectAsync(current, max, addedTurn, effectType).Forget();
        }

        public UniTask PlayAddExtraTurnRightEffectAsync(long effectType) => turnView.PlayAddExtraTurnRightEffectAsync(effectType);
        public UniTask PlayStartLotteryExtraTurnEffectAsync() => turnView.PlayStartLotteryEffectAsync();
        public UniTask PlayEnterExtraTurnEffectAsync(long extraTurn, long turnAddEffectType) => turnView.PlayEnterExtraTurnEffectAsync(extraTurn, turnAddEffectType);
        public UniTask PlayContinueExtraTurnEffectAsync() => turnView.PlayContinueExtraTurnEffectAsync();
    }
}
