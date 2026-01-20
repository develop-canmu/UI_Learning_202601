using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    // レベルアップ効果のView
    public class TrainingEnhanceDeckLevelUpEffectView : TrainingEnhanceDeckBaseView
    {
        [SerializeField] private ScrollDynamic buffScroller;
        
        // 文言表示用テキスト(バフがないときやLv最大時に使う)
        [SerializeField] private TMP_Text annotationText;
        
        //// <summary> Viewの更新 </summary>
        public void UpdateView(long currentLv, long afterLv, TrainingDeckEnhanceListData enhanceListData)
        {
            List<TrainingDeckLevelUpEnhanceScrollDynamicItem.Param> paramList = new List<TrainingDeckLevelUpEnhanceScrollDynamicItem.Param>();

            // 現在のレベル+1~強化後のレベルのバフを取得
            for (long level = currentLv + 1; level <= afterLv; level++) {
                
                List<BuffTargetData> buffList = new List<BuffTargetData>();
              
                TrainingEnhanceDeckBaseView.Param param = new TrainingEnhanceDeckBaseView.Param()
                {
                    growthTargetList = TrainingDeckEnhanceUtility.GetBuffTargetLevelUpData(DeckType.GrowthTarget, level, enhanceListData.GrowthTargetEnhanceList),
                    trainingList = TrainingDeckEnhanceUtility.GetBuffTargetLevelUpData(DeckType.Training, level, enhanceListData.TrainingTargetEnhanceList),
                    equipmentList = TrainingDeckEnhanceUtility.GetBuffTargetLevelUpData(DeckType.SupportEquipment, level, enhanceListData.SupportEquipmentEnhanceList),
                    friendList = TrainingDeckEnhanceUtility.GetBuffTargetLevelUpData(DeckType.SupportFriend, level, enhanceListData.FriendEnhanceList),
                };

                // 表示順に並べる
                SortBuffTargetType(param, buffList);
                
                for (int i = 0; i < buffList.Count; i++)
                {
                    // 一番最初はLevelのラベルを表示する
                    bool isShowLevelLabel = i == 0;
                    paramList.Add(new TrainingDeckLevelUpEnhanceScrollDynamicItem.Param(buffList[i], isShowLevelLabel));
                }
            }

            // 強化できるレベルの最大値を超えているならLv最大テキストを表示する
            bool isLevelMax = currentLv >= TrainingDeckEnhanceUtility.GetMaxEnhanceLevel();

            // バフが何もない時
            bool isEmpty = paramList.Count == 0;

            // レベルが最大時と何もバフがない時は表示する
            annotationText.gameObject.SetActive(isLevelMax || isEmpty);

            // Lv最大時の文言を表示
            if (isLevelMax)
            {
                annotationText.text = StringValueAssetLoader.Instance["training.deckEnhance.EnhanceView.LvMax"];
            }
            // バフが何もない時
            else if (isEmpty)
            {
                annotationText.text = StringValueAssetLoader.Instance["training.deckEnhance.EnhanceView.BuffEmpty"];
            }
            
            buffScroller.SetItems(paramList);
        }
        
        //// <summary> レベルが上がった際の強化されるバフの表示 </summary>
        public void ShowLevelUpEnhanceView(long currentLv, long afterLv, TrainingDeckEnhanceListData enhanceListData)
        {
            List<BuffTargetData> buffList = new List<BuffTargetData>();
            
            TrainingEnhanceDeckBaseView.Param param = new TrainingEnhanceDeckBaseView.Param()
            {
                growthTargetList = TrainingDeckEnhanceUtility.GetBuffLevelUpAcquireData(DeckType.GrowthTarget, currentLv, afterLv, enhanceListData.GrowthTargetEnhanceList),
                trainingList = TrainingDeckEnhanceUtility.GetBuffLevelUpAcquireData(DeckType.Training, currentLv, afterLv, enhanceListData.TrainingTargetEnhanceList),
                equipmentList = TrainingDeckEnhanceUtility.GetBuffLevelUpAcquireData(DeckType.SupportEquipment, currentLv, afterLv, enhanceListData.SupportEquipmentEnhanceList),
                friendList = TrainingDeckEnhanceUtility.GetBuffLevelUpAcquireData(DeckType.SupportFriend, currentLv, afterLv, enhanceListData.FriendEnhanceList),
            };

            // 表示順に並べる
            SortBuffTargetType(param, buffList);
            
            List<TrainingDeckLevelUpEnhanceScrollDynamicItem.Param> paramList = new List<TrainingDeckLevelUpEnhanceScrollDynamicItem.Param>(); 
            // レベルのラベルは表示しない
            bool isShowLevelLabel = false;
            
            foreach (BuffTargetData buffTargetData in buffList)
            {
                paramList.Add(new TrainingDeckLevelUpEnhanceScrollDynamicItem.Param(buffTargetData, isShowLevelLabel));
            }
            
            bool isEmpty = paramList.Count == 0;
            annotationText.gameObject.SetActive(isEmpty);

            // バフが何もない時
            if (isEmpty)
            {
                annotationText.text = StringValueAssetLoader.Instance["training.deckEnhance.levelUpModal.BuffEmpty"];
            }
            
            buffScroller.SetItems(paramList);
        }
    }
}