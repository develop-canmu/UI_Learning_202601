using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingSelectedPracticeCardDetailModal : ModalWindow
    {
        private class AutoPracticeCardSummary
        {
            public long TrainingCardId { private set; get; }
            public long CharaId { private set; get; }
            public long CardCharaId { private set; get; }
            public long Count { set; get; }
            public long EnhanceLevel { private set; get; }

            public AutoPracticeCardSummary(long trainingCardId, long charaId, long cardCharaId, long count, long enhanceLevel)
            {
                TrainingCardId = trainingCardId;
                CharaId = charaId;
                CardCharaId = cardCharaId;
                Count = count;
                EnhanceLevel = enhanceLevel;
            }
        }
        
        [SerializeField]
        private TMP_Text[] trainingTypeCountText = null;

        [SerializeField]
        private ScrollGrid scrollGrid = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            TrainingAutoResultStatus result = (TrainingAutoResultStatus)args;
            long[] typeCount = new long[trainingTypeCountText.Length];

            Dictionary<long, AutoPracticeCardSummary> cardCount = new ();

            // 各練習カードの回数
            foreach (ResultCard r in result.cardList)
            {
                // マスタ
                TrainingCardMasterObject mTrainingCard = MasterManager.Instance.trainingCardMaster.FindData(r.mTrainingCardId);
                // タイプカウント
                int type = (int)mTrainingCard.practiceType;
                typeCount[type] += r.count;
                
                
                if (cardCount.TryGetValue(r.mTrainingCardId, out AutoPracticeCardSummary entry))
                {
                    // 既に存在する場合はカウントのみ加算
                    entry.Count += r.count;
                }
                else
                {
                    long enhanceLevel = r.displayEnhanceLevel;

                    cardCount.Add(r.mTrainingCardId, new AutoPracticeCardSummary(
                        r.mTrainingCardId,
                        r.mCharaId,
                        r.mTrainingCardCharaId,
                        r.count,
                        enhanceLevel
                    ));
                }
            }

            // 種類ごとのカウント表示
            for (int i = 0; i < typeCount.Length; i++)
            {
                trainingTypeCountText[i].text = string.Format(StringValueAssetLoader.Instance["auto_training.summary.count"], typeCount[i]);
            }

            // スクロール用配列作成
            AutoTrainingSummaryPracticeCardItem.Arguments[] cardArray = new AutoTrainingSummaryPracticeCardItem.Arguments[cardCount.Count];

            int index = 0;
            foreach (AutoPracticeCardSummary entry in cardCount.Values)
            {
                // 配列に追加
                cardArray[index] = new AutoTrainingSummaryPracticeCardItem.Arguments(
                    entry.CharaId,
                    entry.TrainingCardId,
                    entry.Count,
                    entry.EnhanceLevel,
                    entry.CardCharaId);
                // インデックス更新
                index++;
            }
            
            // ソート
            cardArray = cardArray
                // グレード
                .OrderByDescending(item => MasterManager.Instance.trainingCardMaster.FindData(item.CardId).grade)
                // タイプ
                .ThenBy(item => MasterManager.Instance.trainingCardMaster.FindData(item.CardId).practiceType)
                // 回数
                .ThenByDescending(item => item.Count)
                // カードId
                .ThenByDescending(item => item.CardId)
                .ToArray();
            
            // スクロールに登録
            scrollGrid.SetItems(cardArray);
            
            return base.OnPreOpen(args, token);
        }
    }
}