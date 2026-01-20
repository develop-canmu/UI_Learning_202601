using System.Collections.Generic;
using System.Linq;
using Pjfb.Networking.App.Request;

namespace Pjfb.Master {

    public partial class TrainingCardComboElementMasterContainer : MasterContainerBase<TrainingCardComboElementMasterObject> {
        long GetDefaultKey(TrainingCardComboElementMasterObject masterObject){
            return masterObject.id;
        }

        //// <summary> カードコンボのコンボ数を返す </summary>
        public long GetComboValue(long comboId)
        {
            return values.Count(x => x.mTrainingCardComboId == comboId);
        }
        
        //// <summary> 指定されたコンボのカードIdリストを返す </summary>
        public long[] GetComboCardIdList(long comboId)
        {
            return values.Where(x => x.mTrainingCardComboId == comboId).Select(x => x.mTrainingCardId).ToArray();
        }
        
        //// <summary> 指定されたコンボのカードIdリストを返す(選択カード、カード種別に基づいてソート) </summary>
        public long[] GetComboCardSortIdList(long comboId, long selectCardId)
        {
            return values.Where(x => x.mTrainingCardComboId == comboId).Select(x => x.mTrainingCardId)
                // 選択したカードは一番前
                .OrderByDescending(x => x == selectCardId)
                // 練習種別順でソート
                .ThenBy(x => MasterManager.Instance.trainingCardMaster.FindData(x).practiceType).ToArray();
        }

        //// <summary> 該当のカードが使われるカードコンボを取得 </summary>
        public long[] GetComboIdListByCardId(long cardId)
        {
            return values.Where(x => x.mTrainingCardId == cardId).Select(x => x.mTrainingCardComboId).ToArray();
        }
        
        //// <summary> 指定のカード内で発生できるコンボIdを取得 </summary>
        public long[] GetComboIdListByCardId(long[] cardIdList)
        {
            // 発生するコンボ格納用(重複はのぞく)
            HashSet<long> result = new HashSet<long>();

            foreach (long cardId in cardIdList)
            {
                // そのカードが含まれるコンボリスト
                long[] comboIdList = GetComboIdListByCardId(cardId);
                
                // コンボが発生できるかそれぞれ判定する
                foreach (long comboId in comboIdList)
                {
                    // そのコンボに含まれるカードリスト
                    long[] comboConditionIdArray = GetComboCardIdList(comboId);
                    // コンボ条件の全てのカードが含まれているか
                    if (comboConditionIdArray.All(id => cardIdList.Contains(id)))
                    {
                        result.Add(comboId);
                    }
                }
            }
            
            return result.ToArray();
        }
    }
}
