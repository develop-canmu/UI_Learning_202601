using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb
{
    public static class RarityUtility
    {
        public static long GetRarityId(long mCharaId, long liberationLevel)
        {
            var mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            if (mChara == null) return 0;
            
            var mCharaRarityChangeMasterList = MasterManager.Instance.charaRarityChangeMaster.values.Where(data =>
                data.mCharaRarityChangeCategoryId == mChara.mCharaRarityChangeCategoryId);
            var rarityId = mChara.mRarityId;
            foreach (var charaRarityChangeMaster in mCharaRarityChangeMasterList)
            {
                if (liberationLevel < charaRarityChangeMaster.liberationLevel) return rarityId;
                rarityId = charaRarityChangeMaster.mRarityId;
            }

            return rarityId;
        }
        
        public static long GetRarity(long mCharId, long liberationLevel)
        {
            long rarityId = GetRarityId(mCharId, liberationLevel);
            return MasterManager.Instance.rarityMaster.FindData(rarityId).value;
        }

        public static long GetBaseRarity(long mCharaId)
        {
            var mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            if (mChara == null) return 0;
            return MasterManager.Instance.rarityMaster.FindData(mChara.mRarityId).value;
        }

        public static long GetMinRarityMCharaId(long parentMCharaId)
        {
            var rarityTable = MasterManager.Instance.rarityMaster;
            return MasterManager.Instance.charaMaster.values.Where(x => x.parentMCharaId == parentMCharaId)
                .Aggregate((min, next) =>
                    rarityTable.FindData(min.mRarityId).value < rarityTable.FindData(next.mRarityId).value ? min : next).id;
        }

        // <summary> キャラの最大レアリティ </summary>
         public static long GetMaxRarity(long mCharaId)
         {
             var mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
             // キャラの解放後のレアリティの変化マスタ
             IEnumerable<CharaRarityChangeMasterObject> mCharaRarityChangeMasterList = MasterManager.Instance.charaRarityChangeMaster.values.Where(data => data.mCharaRarityChangeCategoryId == mChara.mCharaRarityChangeCategoryId);

             // 解放レベル
             long liberationLevel = 0;
             // 解放レベル最大時のレアリティId
             long liberationMaxRarityId = 0;
             
             // 解放レベルが最も高いマスタを取得する
             foreach (CharaRarityChangeMasterObject master in mCharaRarityChangeMasterList)
             {
                 if (master.liberationLevel > liberationLevel)
                 {
                     // 解放レベルとレアリティIdを更新
                     liberationLevel = master.liberationLevel;
                     liberationMaxRarityId = master.mRarityId;
                 }
             }

             // 最大解放時のレアリティを返す
             return MasterManager.Instance.rarityMaster.FindData(liberationMaxRarityId).value;
         }
    }
}