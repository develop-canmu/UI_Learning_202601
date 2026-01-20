using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class AutoTrainingCardUnionInformationListView : TrainingCardUnionInformationListView
    {
        [SerializeField]
        [StringValue]
        private string statusViewTitle = string.Empty;
        
        protected override void SetScrollData(TrainingMainArguments mainArguments)
        {
            List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem> scrollDataList = new List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem>();
            
            // ステータス欄の見出しを追加
            scrollDataList.Add(new TrainingCardUnionInformationHeaderScrollItem.Argument(StringValueAssetLoader.Instance[statusViewTitle]));
            // ステータス情報の追加
            TrainingUnionCardReward cardUnionReward = mainArguments.AutoTrainingResult.unionCardRewardMap.First();
            AutoTrainingCardUnionStatusResultItem.Argument statusArg = new AutoTrainingCardUnionStatusResultItem.Argument(
                // 倍率は万分率なので%表示のために100で割る
                cardUnionReward.effectRate / 100, 
                TrainingUtility.GetStatus(cardUnionReward)
            );
            scrollDataList.Add(statusArg);
            
            List<TrainingCardCharaMasterObject> sortedPracticeCardList = GetCardUnionPracticeCardList(cardUnionReward.trainingCardList, cardUnionReward.baseTrainingData.id);
            // 各カードの情報を取得
            scrollDataList.AddRange(GetCardScrollDataList(
                mainArguments,
                cardUnionReward,
                sortedPracticeCardList,
                PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.DetailLabel));
            
            // アイテムリストの登録
            CardUnionInformationScrollDynamic.SetItems(scrollDataList);
        }
    }
}