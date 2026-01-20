using System;
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master
{

    public partial class TrainingCardCharaMasterContainer : MasterContainerBase<TrainingCardCharaMasterObject>
    {
        private readonly Dictionary<long, TrainingCardCharaMasterObject[]> practiceCardsByMCharaIdCache = new();
        private readonly Dictionary<long, List<List<TrainingCardCharaMasterObject>>> cardGroupsByCharaId = new();

        long GetDefaultKey(TrainingCardCharaMasterObject masterObject)
        {
            return masterObject.id;
        }

        public long[] FindPracticeCardIds(long charId)
        {
            List<long> result = new List<long>();

            foreach (TrainingCardCharaMasterObject value in values)
            {
                if (value.mCharaId == charId)
                {
                    result.Add(value.mTrainingCardId);
                }
            }

            return result.ToArray();
        }

        public TrainingCardCharaMasterObject[] FindPracticeCard(long charId)
        {
            if (practiceCardsByMCharaIdCache.TryGetValue(charId, out TrainingCardCharaMasterObject[] cached))
            {
                return cached;
            }

            List<TrainingCardCharaMasterObject> result = new List<TrainingCardCharaMasterObject>();

            foreach (TrainingCardCharaMasterObject value in values)
            {
                if (value.mCharaId == charId)
                {
                    result.Add(value);
                }
            }
            TrainingCardCharaMasterObject[] cardCharaMasterArray = result.ToArray();
            practiceCardsByMCharaIdCache[charId] = cardCharaMasterArray;
            return cardCharaMasterArray;
        }

        public long[] FindPracticeCardIds(long charId, long minLv, long maxLv)
        {
            List<long> result = new List<long>();

            foreach (TrainingCardCharaMasterObject value in values)
            {
                if (value.mCharaId == charId && value.level >= minLv && value.level <= maxLv)
                {
                    result.Add(value.mTrainingCardId);
                }
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// 指定したgroupIdに属するカードをすべて取得
        /// levelでソート済み（昇順）
        /// </summary>
        public TrainingCardCharaMasterObject[] FindByGroupIdOrderByLevel(long groupId, long mCharaId)
        {
            List<TrainingCardCharaMasterObject> result = new();

            foreach (TrainingCardCharaMasterObject value in values)
            {
                if (value.groupId == groupId && value.mCharaId == mCharaId)
                {
                    result.Add(value);
                }
            }

            // 選手levelでソート（強化Lv.1 → 2 → 3 の順に並ぶ）
            return result.OrderBy(x => x.level).ToArray();
        }

        /// <summary>
        /// 指定キャラクターIDの練習カードを、強化グループごとにリスト化、レベルでソートして返す
        /// 単独カードは個別リスト、グループカードはgroupIdごとにまとめる
        /// すでにまとめたことがあるものはキャッシュを返す
        /// </summary>
        /// <param name="mCharaId">キャラクターID</param>
        /// <returns>強化グループごとにレベルでソートしてまとめたカードリストのリスト</returns>
        public List<List<TrainingCardCharaMasterObject>> GetGroupedPracticeCardOrderByLevel(long mCharaId)
        {
            // キャッシュに存在する場合はキャッシュを返す
            if (cardGroupsByCharaId.TryGetValue(mCharaId, out List<List<TrainingCardCharaMasterObject>> groupedList))
            {
                return groupedList;
            }

            // 指定キャラIDの全練習カードを取得
            TrainingCardCharaMasterObject[] allCards = FindPracticeCard(mCharaId);
            // グループキーごとにカードリストを保持する辞書
            groupedList = new();
            // groupIdごとのインデックスを保持する辞書(Key: groupId, Value: groupedListのインデックス)
            Dictionary<long, int> groupIndexByGroupId = new();
            // groupedListのインデックス
            int i = 0;

            foreach (TrainingCardCharaMasterObject card in allCards)
            {
                // 単独カード： GroupId が 0 以下のものは個別リストに入れる
                if (card.groupId <= 0)
                {
                    groupedList.Add(new List<TrainingCardCharaMasterObject> { card });
                    i++;
                }
                else
                {
                    // 強化グループに属するカード： GroupId が同じものが同じリストに入る
                    if (!groupIndexByGroupId.TryGetValue(card.groupId, out int index))
                    {
                        groupedList.Add(new List<TrainingCardCharaMasterObject> { card });
                        groupIndexByGroupId.Add(card.groupId, i);
                        i++;
                    }
                    else
                    {
                        groupedList[index].Add(card);
                    }
                }
            }
            
            // 各グループ内をlevelでソート
            for(int t = 0; t < groupedList.Count; t++)
            {
                groupedList[t] = groupedList[t].OrderBy(x => x.level).ToList();
            }
            // キャッシュに保存
            cardGroupsByCharaId[mCharaId] = groupedList;
            return groupedList;
        }
        
       /// <summary>
       /// 指定したキャラレベルに応じた表示用練習カードを取得
       /// </summary>
       /// <param name="mCharaId">キャラのマスターid</param>
       /// <param name="characterLevel">キャラのレベル</param>
       /// <returns>実際に表示する練習メニューカード</returns>
        public List<TrainingCardCharaMasterObject> GetDisplayCardListForCharaLevel(long mCharaId , long characterLevel)
        {
            List<TrainingCardCharaMasterObject> displayCards = new ();
            List<List<TrainingCardCharaMasterObject>> groupedList = GetGroupedPracticeCardOrderByLevel(mCharaId);
            
            // 各グループごとに表示用カードを選定
            foreach (List<TrainingCardCharaMasterObject> cardCharaMasters in groupedList)
            {
                TrainingCardCharaMasterObject targetMaster = null;
                TrainingCardCharaMasterObject minLevelMaster = null;
                
                foreach (TrainingCardCharaMasterObject cardCharaMaster in cardCharaMasters)
                {
                    // 最小レベルのカードを保持
                    if (minLevelMaster == null || cardCharaMaster.level < minLevelMaster.level)
                    {
                        minLevelMaster = cardCharaMaster;
                    }

                    // キャラレベル以下のカードで最もレベルの高いカードを保持
                    if (cardCharaMaster.level <= characterLevel)
                    {
                        if (targetMaster == null || cardCharaMaster.level > targetMaster.level)
                        {
                            targetMaster = cardCharaMaster;
                        }
                    }
                }

                // 解放していない場合は最小レベルのカードをセット
                if (targetMaster == null)
                {
                    targetMaster = minLevelMaster;
                }
                displayCards.Add(targetMaster);
            }

            return displayCards;
        }
    }
}
