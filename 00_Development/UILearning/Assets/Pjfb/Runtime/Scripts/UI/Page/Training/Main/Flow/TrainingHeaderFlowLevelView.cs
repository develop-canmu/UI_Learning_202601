using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pjfb.Adv;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    /// <summary> Flowレベルのゲージ表示View </summary>
    public class TrainingHeaderFlowLevelView : MonoBehaviour
    {
        /// <summary> Flowレベルのゲージ表示用データ </summary>
        private struct FlowLevelGaugeData
        {
            public long Level;
            public float GaugeValue;
            
            public FlowLevelGaugeData(long level, float gaugeValue)
            {
                Level = level;
                GaugeValue = gaugeValue;
            }
        }
        
        private static readonly string LevelUpAnimationKey = "LevelUpEffect";
        private static readonly string CloseLevelUpAnimationKey = "CloseLevelUpEffect";
        
        [SerializeField]
        private Animator animator = null;

        // 現在のレベルテキスト
        [SerializeField]
        private TMP_Text currentLevelText = null;
        
        // 増加後のレベルルートオブジェクト
        [SerializeField]
        private GameObject afterLevelRoot = null;
        
        // 増加後のレベルテキスト
        [SerializeField]
        private TMP_Text afterLevel = null;
        
        // 現在のポイントゲージ
        [SerializeField]
        private Slider currentPointGauge = null;
        
        // 増加後のポイントゲージ
        [SerializeField]
        private Slider afterPointGauge = null;
        
        // 最大レベル時のポイントゲージ
        [SerializeField]
        private Slider maxPointGauge = null;
        
        // 加算ポイントルートオブジェクト
        [SerializeField]
        private GameObject addPointRoot = null;
        
        // 加算ポイントテキスト
        [SerializeField]
        private TMP_Text addPointText = null;

        // レベルアップ表示オブジェクト
        [SerializeField]
        private GameObject levelUpObject = null;

        // ポイント取得通知オブジェクト
        [SerializeField]
        private TrainingPointNotification pointNotification = null;
        
        // ゲージアニメーションの再生時間
        [SerializeField]
        private float gaugeAnimationDuration = 0.5f;
        
        // ソート済みFlowレベルテーブルマスターリスト
        private TrainingConcentrationMasterObject[] flowSortedLevelTableList = null;
        // 最大レベル
        private long maxLevel = 0;
        
        // 初期化済みフラグ
        private bool isInitialized = false;
        
        // アニメーション再生中フラグ
        private bool playingAnimation = false;
        
        private CancellationTokenSource tokenSource = null;
        
        /// <summary> Flowレベルテーブル初期化 </summary>
        private void InitializeLevelTable(TrainingMainArguments arguments)
        {
            long scenarioId = arguments.Pending.mTrainingScenarioId;
            long mCharaId = arguments.TrainingCharacter.MCharId;
            // Flowゾーン対象範囲のコンセントレーションIDリストを取得
            long[] concentrationIdList = MasterManager.Instance.trainingConcentrationLotteryMaster.GetTrainingCharacterConcentrationIdList(scenarioId, mCharaId);
            maxLevel = 0;
            
            List<TrainingConcentrationMasterObject> flowConcentrationMasterList = new List<TrainingConcentrationMasterObject>();

            foreach (var id in concentrationIdList)
            {
                TrainingConcentrationMasterObject master = MasterManager.Instance.trainingConcentrationMaster.FindData(id);
                // Flowタイプ以外はスキップ
                if (master.GetConcentrationType() != TrainingConcentrationMasterObject.TrainingConcentrationType.Flow)
                {
                    continue;
                }

                // 最大レベルの更新
                if (maxLevel < master.level)
                {
                    maxLevel = master.level;
                }

                flowConcentrationMasterList.Add(master);
            }

            flowSortedLevelTableList = flowConcentrationMasterList.OrderBy(x => x.level).ToArray();
            isInitialized = true;
        }

        /// <summary> 現在のレベルとポイントゲージ量をセット(初期化データが無いならデータを取得する) </summary>
        public void UpdateView(TrainingMainArguments arguments)
        {
            // レベルテーブル初期化
            if (isInitialized == false)
            {
                InitializeLevelTable(arguments);
            }
     
            // 強化後のレベル表示はオフに
            ShowAfterLevel(false);
            // 今のレベルゲージデータをセット
            SetCurrentLevelView(arguments.Pending.concentrationExp);
        }
        
        private void SetCurrentLevelView(long exp)
        {
            // 今のレベルゲージデータを取得
            FlowLevelGaugeData data = GetLevelGauge(exp);
            SetCurrentLevelView(data.Level, data.GaugeValue);
        }
        
        private void SetCurrentLevelView(long level, float gaugeValue)
        {
            // いったん表示オフ
            currentPointGauge.gameObject.SetActive(false);
            maxPointGauge.gameObject.SetActive(false);
            
            // 最大レベル時
            if (level == maxLevel)
            {
                maxPointGauge.gameObject.SetActive(true);
                maxPointGauge.value = 1.0f;
                currentLevelText.text = string.Format(StringValueAssetLoader.Instance["training.flow_Lv_max"], level);
            }
            else
            {
                currentPointGauge.gameObject.SetActive(true);
                currentPointGauge.value = gaugeValue;
                currentLevelText.text = string.Format(StringValueAssetLoader.Instance["training.flow_Lv"], level);
            }
        }

        /// <summary> 増加後のレベルとポイントゲージ量をセット </summary>
        public void SetAfterLevelView(long currentExp, long rewardExp)
        {
            // 未初期化なら処理しない
            if (isInitialized == false)
            {
                return;
            }
            
            // 現在のゲージデータ
            FlowLevelGaugeData currentGaugeData = GetLevelGauge(currentExp);
            // 増加後のゲージデータ
            FlowLevelGaugeData afterGaugeData = GetLevelGauge(currentExp + rewardExp);

            // レベルアップするか？
            bool isLevelUp = afterGaugeData.Level > currentGaugeData.Level;
            
            // レベルアップ時は増加後のレベル表示をオンに
            afterLevelRoot.SetActive(isLevelUp);
            
            // それぞれのゲージをいったんオフに
            afterPointGauge.gameObject.SetActive(false);
            maxPointGauge.gameObject.SetActive(false);

            // 現在レベルが最大
            if (currentGaugeData.Level == maxLevel)
            {
                maxPointGauge.gameObject.SetActive(true);
                maxPointGauge.value = 1.0f;
                return;
            }
            // 増加後最大レベル時
            else if (afterGaugeData.Level == maxLevel)
            {
                afterLevel.text = string.Format(StringValueAssetLoader.Instance["training.flow_after_Lv_max"], afterGaugeData.Level);
                maxPointGauge.gameObject.SetActive(true);
                maxPointGauge.value = afterGaugeData.GaugeValue;
            }
            else
            {
                afterLevel.text = afterGaugeData.Level.ToString();
                afterPointGauge.gameObject.SetActive(true);
                afterPointGauge.value = isLevelUp ? 1.0f : afterGaugeData.GaugeValue;
            }

            // 加算ポイント表示
            addPointRoot.SetActive(true);
            addPointText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], rewardExp);
        }

        /// <summary> 増加レベルの表示制御 </summary>
        public void ShowAfterLevel(bool show)
        {
            // 未初期化なら処理しない
            if (isInitialized == false)
            {
                return;
            }
            
            afterLevelRoot.SetActive(show);
            afterPointGauge.gameObject.SetActive(false);
            addPointRoot.SetActive(show);
        }
        
        /// <summary> ポイント取得時のゲージ増加アニメーション再生 </summary>
        public void PlayGetPointAnimation(AppAdvManager adv, long currentPoint, long addFlowPoint, long convertTurnPoint, long convertTurnValue, long convertConditionPoint, Action onComplete = null)
        {
            PlayGetPointAnimationAsync(adv, currentPoint, addFlowPoint, convertTurnPoint, convertTurnValue, convertConditionPoint, onComplete).Forget();
        }
        
        /// <summary> ポイント取得時のゲージ増加アニメーション再生 </summary>
        public async UniTask PlayGetPointAnimationAsync(AppAdvManager adv, long currentPoint,  long addFlowPoint, long convertTurnPoint,  long convertTurnValue, long convertConditionPoint, Action onComplete = null)
        {
            // 未初期化なら処理しない
            if (isInitialized == false)
            {
                return;
            }
            
            Cancel();
            
            // 合計加算ポイント
            long totalAddPoint = addFlowPoint + convertTurnPoint + convertConditionPoint;
            // 取得後のポイント
            long afterPoint = currentPoint + addFlowPoint + convertTurnPoint + convertConditionPoint;
            
            FlowLevelGaugeData current = GetLevelGauge(currentPoint);
            FlowLevelGaugeData after = GetLevelGauge(afterPoint);
            
            // 現在のレベルゲージをセット
            SetCurrentLevelView(current.Level, current.GaugeValue);
            // すでに最大レベルなら処理しない
            if (current.Level == maxLevel)
            {
                // 最大レベル時のポイントスキップログ
                adv.AddMessageLog(string.Empty, StringValueAssetLoader.Instance["training.log.flow_point.skipped_max_level"], 0);
                return;
            }
            
            // 再生中に
            playingAnimation = true;
            tokenSource = new CancellationTokenSource();
            
            addPointRoot.SetActive(true);
            addPointText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], totalAddPoint);
            // ポイント獲得ログ
            adv.AddMessageLog(string.Empty, string.Format(StringValueAssetLoader.Instance["training.log.flow_point.get_point"], totalAddPoint), 0);
                
            bool isLevelUp = after.Level > current.Level;
            
            // レベルアップが発生
            afterLevelRoot.SetActive(isLevelUp);
            if (isLevelUp)
            {
                // レベルアップログ
                adv.AddMessageLog(StringValueAssetLoader.Instance["training.log.flow_point.level_up.title"], string.Format(StringValueAssetLoader.Instance["training.log.flow_point.level_up"], current.Level, after.Level), 0);
                
                // 最大レベル時
                if (after.Level == maxLevel)
                {
                    afterLevel.text = string.Format(StringValueAssetLoader.Instance["training.flow_after_Lv_max"], after.Level);
                    
                    // 最大レベル時ログ
                    adv.AddMessageLog(string.Empty, StringValueAssetLoader.Instance["training.log.flow_point.max_level"], 0);
                }
                else
                {
                    afterLevel.text = after.Level.ToString();
                }
            }
            
            long currentLevel = current.Level;

            // 変換されたターン延長ポイントを通知に追加
            if (convertTurnPoint > 0)
            {
                pointNotification.AddMessage(string.Format(StringValueAssetLoader.Instance["training.flow_point.convert_turn"], convertTurnValue), convertTurnPoint);
                // ポイント変換ログ
                adv.AddMessageLog(StringValueAssetLoader.Instance["training.log.point_convert"], string.Format(StringValueAssetLoader.Instance["training.log.flow_point.convert_turn_point"], convertTurnValue, convertTurnPoint), 0);
            }

            // 変換されたコンディションポイントを通知に追加
            if (convertConditionPoint > 0)
            {
                pointNotification.AddMessage(StringValueAssetLoader.Instance["training.flow_point.convert_condition"], convertConditionPoint);
                // ポイント変換ログ
                adv.AddMessageLog(StringValueAssetLoader.Instance["training.log.point_convert"], string.Format(StringValueAssetLoader.Instance["training.log.flow_point.convert_condition_point"], convertConditionPoint), 0);
            }
            
            try
            {
                // ゲージ増加演出
                while (currentLevel <= after.Level && currentLevel < maxLevel)
                {
                    // レベルアップが発生するか？
                    isLevelUp = after.Level > currentLevel;
                    // 増加後のゲージ量(レベルアップ後のレベル以下ならゲージは最大)
                    float endGaugeValue = isLevelUp ? 1.0f : after.GaugeValue;
                    // アニメーション時間計算
                    float animationTime = gaugeAnimationDuration * (endGaugeValue - current.GaugeValue);

                    // ゲージアニメーションの再生
                    await currentPointGauge.DOValue(endGaugeValue, animationTime).ToUniTask(cancellationToken: tokenSource.Token);

                    if (isLevelUp)
                    {
                        await PlayLevelUpAnimation(tokenSource.Token);
                        currentLevel++;
                        // レベルアップ後のゲージは0にリセット
                        currentPointGauge.value = 0f;
                    }
                    // これ以上レベルアップしないならループ終了
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                // 演出終了後
                SetCurrentLevelView(after.Level, after.GaugeValue);
                ShowAfterLevel(false);
                levelUpObject.SetActive(false);
                // レベルアップ用のエフェクトもオフにしとく
                animator.SkipToEnd(CloseLevelUpAnimationKey);
                playingAnimation = false;
                onComplete?.Invoke();
            }
        }

        /// <summary> アニメーション再生中なら止める </summary>
        public void StopAnimation()
        {
            if (playingAnimation == false)
            {
                return;
            }

            Cancel();
        }
        
        private async UniTask PlayLevelUpAnimation(CancellationToken token)
        {
            animator.SetTrigger(LevelUpAnimationKey);
            // レベルアップ表示オブジェクトをアクティブに
            levelUpObject.SetActive(true);
            
            // アニメーション終了まで待機
            await AnimatorUtility.WaitStateFinishAsync(animator, CloseLevelUpAnimationKey, token);
        }

        /// <summary> 今のポイントからのレベルゲージデータを取得 </summary>
        private FlowLevelGaugeData GetLevelGauge(long exp)
        {
            long currentLevel = 0;
            float currentGauge = 0f;
            
            for (int i = 0; i < flowSortedLevelTableList.Length; i++)
            {
                // 最大レベル時
                if (i == flowSortedLevelTableList.Length - 1)
                {
                    currentLevel = flowSortedLevelTableList[i].level;
                    currentGauge = 1.0f;
                    break;
                }
                
                // 今見るレベル帯のマスタ
                var target = flowSortedLevelTableList[i];
                // 次のレベル帯のマスタ
                var next = flowSortedLevelTableList[i + 1];
                
                // そのレベル帯に必要な経験値が足りているか見る
                if(exp >= target.exp && exp < next.exp)
                {
                    currentLevel = target.level;
                    currentGauge = (exp - target.exp) / (float)(next.exp - target.exp);
                    break;
                }
            }
            
            return new FlowLevelGaugeData(currentLevel, currentGauge);
        }

        private void Cancel()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }
        }

        private void OnDestroy()
        {
            Cancel();
        }
    }
}