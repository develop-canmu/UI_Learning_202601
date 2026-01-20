using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using System.Linq;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryStatusView : MonoBehaviour
    {
        
        [SerializeField]
        private GameObject cardUnionInfoButton = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView viewPrefab = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView specialLectureView = null;
        [SerializeField]
        private AutoTrainingSummaryStatusItemView restView = null;
        [SerializeField]
        private AutoTrainingSummaryStatusItemView performaceLvView = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView revenueMatchView = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView inspirationBoostLvView = null;
        
        [SerializeField]
        private RectTransform conditionRoot = null;
        [SerializeField]
        private RectTransform concentrationRoot = null;
        
        [SerializeField]
        private GameObject inspirationButtonRoot = null;
        
        [SerializeField]
        private AutoTrainingSummaryBoostPointView boostPointView = null;
        
        // FLOW関係の表示
        [SerializeField]
        private AutoTrainingSummaryFlowView flowView = null;
        
        // カードコンボ発生回数
        [SerializeField] 
        private AutoTrainingSummaryCardComboItemView cardComboView = null;
        
        private TrainingMainArguments trainingMainArguments = null;
        
        private TrainingAutoResultStatus resultStatus = null;
        
        // 生成したView
        private List<AutoTrainingSummaryStatusItemView> createdViews = new List<AutoTrainingSummaryStatusItemView>();

        private AutoTrainingSummaryStatusItemView CreateView(string name, long value, RectTransform parent)
        {
            AutoTrainingSummaryStatusItemView view = GameObject.Instantiate<AutoTrainingSummaryStatusItemView>(viewPrefab, parent);
            view.SetName(name);
            view.SetValue(value);
            createdViews.Add(view);
            return view;
        }

        public void SetData(TrainingMainArguments mainArguments, TrainingAutoResultStatus result)
        {
            trainingMainArguments = mainArguments;
            resultStatus = result;
            
            // 生成したViewを削除
            foreach(AutoTrainingSummaryStatusItemView view in createdViews)
            {
                GameObject.Destroy(view.gameObject);
            }
            createdViews.Clear();
            
            // スペシャルレクチャー回数
            specialLectureView.SetValue(result.rareTrainingCount);
            // 休憩回数
            restView.SetValue(result.restCount);
            
            // コンディション
            if(result.tierList.Length > 0)
            {
                conditionRoot.gameObject.SetActive(true);
                foreach(ResultTier condition in result.tierList)
                {
                    CreateView( string.Format(StringValueAssetLoader.Instance["auto_training.summary.condition"], condition.name), condition.count, conditionRoot);
                }
            }
            else
            {
                conditionRoot.gameObject.SetActive(false);
            }
            
            // パフォーマンスが有効の場合
            if(TrainingUtility.IsEnablePerformace( mainArguments.Pending.mTrainingScenarioId ) )
            {
                // パフォーマンスLv
                if(mainArguments.Pending.overallProgress != null && mainArguments.Pending.overallProgress.currentLevel > 0)
                {
                    performaceLvView.gameObject.SetActive(true);
                    performaceLvView.SetValue(mainArguments.Pending.overallProgress.currentLevel);
                }
                
                // レベニューマッチ
                revenueMatchView.gameObject.SetActive(true);
                revenueMatchView.SetValue(result.intentionalCount);
            }
            else
            {
                performaceLvView.gameObject.SetActive(false);
                revenueMatchView.gameObject.SetActive(false);
            }
            
            // 獲得したインスピがある場合
            if(result.inspireList.Length > 0)
            {
                // インスピ詳細ボタン
                inspirationButtonRoot.SetActive( true );
                // インスピブースト
                long inspirationBoostLv = mainArguments.GetInspirationLv(out _).level;
                inspirationBoostLvView.gameObject.SetActive(true);
                inspirationBoostLvView.SetValue( inspirationBoostLv );
            }
            else
            {
                inspirationButtonRoot.SetActive(false);
                inspirationBoostLvView.gameObject.SetActive(false);
            }
            
            // コンセントレーション
            if(result.concentrationList.Length > 0)
            {
                concentrationRoot.gameObject.SetActive(true);
                
                // ソート
                IEnumerable<ResultIdCount> sortedList = result.concentrationList.
                                                            // EffectNumber順
                                                            OrderBy(v=>MasterManager.Instance.trainingConcentrationMaster.FindData(v.id).effectNumber).
                                                            // Id順
                                                            ThenBy(v=>v.id);
                
                Dictionary<long, long> concentrationCountList = new Dictionary<long, long>();
                // EffectNumberごとにまとめてカウントする
                foreach(ResultIdCount c in sortedList)
                {
                    TrainingConcentrationMasterObject mConcentration = MasterManager.Instance.trainingConcentrationMaster.FindData(c.id);
                    // EffectNumberごとに分ける
                    if(concentrationCountList.ContainsKey(mConcentration.effectNumber) == false)
                    {
                        concentrationCountList.Add(mConcentration.effectNumber, c.count);
                    }
                    else
                    {
                        concentrationCountList[mConcentration.effectNumber] += c.count;
                    }
                }
                
                // EffectNumberごとに表示
                foreach(KeyValuePair<long, long> concentrationCount in concentrationCountList)
                {
                    // effectNumberから名前を探す
                    foreach(TrainingConcentrationEffectMasterObject mEffect in MasterManager.Instance.trainingConcentrationEffectMaster.values )
                    {
                        if(mEffect.mTrainingConcentrationEffectNumber == concentrationCount.Key)
                        {
                            CreateView(mEffect.text, concentrationCount.Value, concentrationRoot);
                            break;
                        }
                    }
                }
            }
            else
            {
                concentrationRoot.gameObject.SetActive(false);
            }

            if (TrainingUtility.IsEnableBoostPoint(mainArguments.Pending.mTrainingScenarioId))
            {
                boostPointView.gameObject.SetActive(true);
                boostPointView.SetData(mainArguments, resultStatus);
            }
            else
            {
                boostPointView.gameObject.SetActive(false);
            }

            cardComboView.gameObject.SetActive(true);
            // 発生したカードコンボに応じて表示を切り替え
            cardComboView.SetComboData(result.cardComboList);
            
            // カードユニオン情報ボタンの表示
            cardUnionInfoButton.SetActive(mainArguments.AutoTrainingResult.unionCardRewardMap != null && mainArguments.AutoTrainingResult.unionCardRewardMap.Length > 0);
            
            // FLOW関係の表示をセット
            if (TrainingUtility.IsEnableType(TrainingScenarioType.Flow, mainArguments.Pending.mTrainingScenarioId))
            {
                flowView.gameObject.SetActive(true);
                flowView.SetData(result);
            }
            else
            {
                flowView.gameObject.SetActive(false);
            }
        }
  
        /// <summary>UGUI</summary>
        public void OnPracticeMenuCardButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoTrainingSelectedPracticeCardDetail, resultStatus);
        }

        /// <summary>UGUI</summary>
        public void OnGetSkillButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoTrainingGetSkillList, resultStatus);
        }
        
        /// <summary>UGUI</summary>
        public void OnInspirationButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoTrainingInspirationList, resultStatus);
        }

        public void OnClickCardUnionInformationButton()
        {
            AutoTrainingCardUnionInformationModalWindow.ModalData modalData = new AutoTrainingCardUnionInformationModalWindow.ModalData(trainingMainArguments);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoTrainingCardUnionInformation, modalData);
        }
    }
}