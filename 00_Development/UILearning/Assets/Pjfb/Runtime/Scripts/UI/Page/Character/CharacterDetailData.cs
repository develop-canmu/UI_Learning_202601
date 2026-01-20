using System.Linq;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class CharacterDetailData
    {
        private long mCharaId = 0;
        public long MCharaId { get { return mCharaId; } }

        private long lv = 0;
        public long Lv { get { return lv; } }

        private long liberationLevel = 0;
        public long LiberationLevel { get { return liberationLevel; } }
        
        private long uCharaId = -1;
        public  long UCharId{get{return uCharaId;}}
        
        private bool fromPrizeJson = false;
        /// <summary>prizeJson経由のデータ</summary>
        public bool FromPrizeJson { get { return fromPrizeJson; } }

        public CharaMasterObject MChara => MasterManager.Instance.charaMaster.FindData(mCharaId);

        public CharacterDetailData(UserDataChara uChara)
        {
            this.uCharaId = uChara.id;
            this.mCharaId = uChara.charaId;
            this.lv = uChara.level;
            this.liberationLevel = uChara.newLiberationLevel;
        }

        public CharacterDetailData(CharaMasterObject mChara)
        {
            this.mCharaId = mChara.id;
            var mStatusAdditionLevelDictionary = MasterManager.Instance.statusAdditionLevelMaster.values.ToDictionaryOfList(aData => aData.mStatusAdditionId);
            this.liberationLevel = mStatusAdditionLevelDictionary.TryGetValue(mChara.mStatusAdditionIdLiberation, out var liberationDataList) && liberationDataList.Any() ?
                liberationDataList.Max(aData => aData.level) : 1;
            this.lv = mStatusAdditionLevelDictionary.TryGetValue(mChara.mStatusAdditionIdGrowth, out var growthDataList) && growthDataList.Any() ?
                growthDataList.Max(aData => aData.level) : 1;
        }
        
        public CharacterDetailData(long mCharaId, long lv, long liberationLevel, bool fromPrizeJson = false)
        {
            this.mCharaId = mCharaId;
            this.lv = lv;
            this.liberationLevel = liberationLevel;
            this.fromPrizeJson = fromPrizeJson;
        }
        
        public CharacterDetailData(long uCharId, long mCharaId, long lv, long liberationLevel)
        {
            this.uCharaId = uCharId;
            this.mCharaId = mCharaId;
            this.lv = lv;
            this.liberationLevel = liberationLevel;
        }

        public long GetMaxGrowthLevel(long liberationLv)
        {
            //1. m_chara.maxLevel を取得
            //2. u_chara.level と m_chara.mStatusAdditionIdGrowth をもとに m_status_addition_level.maxLevelGrowth を取得
            //3. u_chara.newLiberationLevel と m_chara.mStatusAdditionIdLiberation をもとに m_status_addition_level.maxLevelGrowth を取得
            //4. 1～3の合計値が強化の最大レベルになる

            var mStatusAdditionalLevelGrowth = MasterManager.Instance
                .statusAdditionLevelMaster.values.Where(data =>
                    data.mStatusAdditionId == MChara.mStatusAdditionIdGrowth).ToArray()
                .FirstOrDefault(x => x.level == Lv);
            var mStatusAdditionalLevelLiberation = MasterManager.Instance
                .statusAdditionLevelMaster.values.Where(data =>
                    data.mStatusAdditionId == MChara.mStatusAdditionIdLiberation).ToArray()
                .FirstOrDefault(x => x.level == liberationLv);

            if (mStatusAdditionalLevelGrowth is null || mStatusAdditionalLevelLiberation is null)
            {
                CruFramework.Logger.LogError(
                    $"MStatusAdditionalLevelGrowth か　mStatusAdditionalLevelLiberationのデータがありません  MCharaId : {mCharaId}");
                return 0;
            }

            return MChara.maxLevel + mStatusAdditionalLevelGrowth.maxLevelGrowth + mStatusAdditionalLevelLiberation.maxLevelGrowth;
        }

        public long GetMaxGrowthLevel()
        {
            return GetMaxGrowthLevel(liberationLevel);
        }
    }
}