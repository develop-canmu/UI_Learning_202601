using System.Collections.Generic;
using System.Linq;
using CruFramework;
using MagicOnion;
using Pjfb.Master;

namespace Pjfb
{
    public struct CharaAbilityInfo
    {
        // m_ability.id
        private long skillId;
        public long SkillId => skillId;

        // m_CharaId
        private long mCharaId;
        public long MCharaId => mCharaId;
        
        // スキルレベル
        private long skillLevel;
        public long SkillLevel => skillLevel;

        // スキルが解放されるキャラのレベル
        private long openCharaLevel;
        public long OpenCharaLevel => openCharaLevel;

        // スキルが解放される限界突破レベル
        private long openLiberationLevel;
        public long OpenLiberationLevel => openLiberationLevel;

        // スキルが解放されているか
        private bool isAbilityUnLock;
        public bool IsAbilityUnLock => isAbilityUnLock;
        
        private BattleConst.AbilityType abilityType;
        public BattleConst.AbilityType AbilityType => abilityType;
        
        public CharaAbilityInfo(SkillCharaMasterObject skillMaster, BattleConst.AbilityType abilityType, bool isAbilityUnLock)
        {
            this.skillId = skillMaster.mSkillId;
            this.mCharaId = skillMaster.mCharaId;
            this.skillLevel = skillMaster.skillLevel;
            openCharaLevel = skillMaster.openLevel;
            openLiberationLevel = skillMaster.openLiberationLevel;
            this.isAbilityUnLock = isAbilityUnLock;
            this.abilityType = abilityType;
        }

        public static bool Builder(out CharaAbilityInfo charaAbilityInfo , GuildBattleAbilityData guildBattleAbilityData ,BattleConst.AbilityType abilityType)
        {
            charaAbilityInfo = default;
            SkillCharaMasterObject skillCharaMasterObject = MasterManager.Instance.skillCharaMaster.values.FirstOrDefault(v => 
                v.mCharaId == guildBattleAbilityData.MCharaId && 
                v.mSkillId == guildBattleAbilityData.AbilityId && 
                v.skillLevel == guildBattleAbilityData.AbilityLevel);
            // スキルマスターが見つからない場合はエラーログを出力
            if (skillCharaMasterObject == null)
            {
                CruFramework.Logger.LogError($"SkillCharaMasterObject not found for MCharaId: {guildBattleAbilityData.MCharaId}, AbilityId: {guildBattleAbilityData.AbilityId}, AbilityLevel: {guildBattleAbilityData.AbilityLevel}");
                return false;
            }
            charaAbilityInfo = new CharaAbilityInfo(
                skillCharaMasterObject,
                abilityType,
                true
            );
            return true;
        }

        /// <summary> アビリティマスターの取得 </summary>
        public AbilityMasterObject GetAbilityMaster()
        {
           return MasterManager.Instance.abilityMaster.FindData(skillId);
        }
    }
    
    /// <summary> キャラクターに紐づいている能力 </summary>
    public static class CharaAbilityUtility
    {
        /// <summary> キャラクタースキルをすべて取得(修得済みスキルは最大レベルで、未修得のものはすべて含んで返す) </summary>
        public static List<CharaAbilityInfo> GetAbilityListAll(BattleConst.AbilityType abilityType, long charaId, long charaLevel, long liberationLevel)
        {
            List<CharaAbilityInfo> result = new List<CharaAbilityInfo>();

            // 修得済みスキル
            List<CharaAbilityInfo> acquireSkillList = GetAbilityAcquireList(abilityType, charaId, charaLevel, liberationLevel);
            
            // 修得済みスキルを追加
            result.AddRange(acquireSkillList);

            // 修得済みグループレベル
            long groupLevel = GetAbilityAcquireLevelGroup(charaId, charaLevel);
            
            // 未修得のスキルを追加していく
            foreach (SkillCharaMasterObject skillMaster in MasterManager.Instance.skillCharaMaster.values)
            {
                // キャラIdが違う
                if (skillMaster.mCharaId != charaId) continue;
                // アビリティタイプが違う
                if(MasterManager.Instance.abilityMaster.FindData(skillMaster.mSkillId).AbilityType != abilityType) continue;
                // 解放条件を満たしているデータはすでに追加済みなので無視する
                if (skillMaster.openLevel <= groupLevel && skillMaster.openLiberationLevel <= liberationLevel) continue;
                // すでに同じスキルレベルのデータが追加済みなら無視
                if (result.Any(x => x.SkillId == skillMaster.mSkillId && x.SkillLevel == skillMaster.skillLevel)) continue;
                
                // 該当スキルのスキルレベルが解放されるマスタを取得(最も解放レベルが低いもの)
                SkillCharaMasterObject liberationMaster = GetAbilityLiberationMaster(abilityType, charaId, skillMaster.mSkillId, skillMaster.skillLevel);
                if (liberationMaster == null)
                {
                    continue;
                }
                
                // 未修得のデータを追加
                result.Add(new CharaAbilityInfo(liberationMaster, abilityType, false));
            }
            
            // 修得済みのものを優先
            return result.OrderByDescending(x => x.IsAbilityUnLock)
                // スキルId順
                .ThenBy(x => x.SkillId)
                // スキルレベル順
                .ThenBy(x => x.SkillLevel).ToList();
        }
        
        /// <summary> 現在のレベルで修得済みのスキルリスト </summary>
        public static List<CharaAbilityInfo> GetAbilityAcquireList(BattleConst.AbilityType abilityType, long charaId, long charaLevel, long liberationLevel)
        {
            List<CharaAbilityInfo> result = new List<CharaAbilityInfo>();

            // すでにチェックしたスキルIdリスト
            List<long> checkedSkillIdList = new List<long>();
            
            // 修得済みになるグループレベル
            long groupLevel = GetAbilityAcquireLevelGroup(charaId, charaLevel);
            
            foreach (SkillCharaMasterObject skillMaster in MasterManager.Instance.skillCharaMaster.values)
            {
                // キャラIdが違う
                if (skillMaster.mCharaId != charaId) continue;
                // アビリティタイプが違う
                if(MasterManager.Instance.abilityMaster.FindData(skillMaster.mSkillId).AbilityType != abilityType) continue;
                // 対象グループレベル以外
                if (skillMaster.openLevel != groupLevel) continue;
                // 潜在解放レベルがスキルの解放レベルに達していない
                if (skillMaster.openLiberationLevel > liberationLevel) continue;
                
                // チェック済みスキルId
                if(checkedSkillIdList.Contains(skillMaster.mSkillId))continue;
                
                // チェック済みIdに追加
                checkedSkillIdList.Add(skillMaster.mSkillId);
                
                result.Add(new CharaAbilityInfo(skillMaster, abilityType, true));
            }

            return result;
        }

        /// <summary> 現在のスキル修得のレベル帯(サーバー側の実装がグループ化されたレベルで見ているので合わせる) </summary>
        public static long GetAbilityAcquireLevelGroup(long mCharaId, long charaLevel)
        {
            long result = -1;
            
            foreach (SkillCharaMasterObject skillMaster in MasterManager.Instance.skillCharaMaster.values)
            {
                // キャラIdが違う
                if (skillMaster.mCharaId != mCharaId) continue;
                // スキル解放レベルを超えてない
                if(skillMaster.openLevel > charaLevel) continue;

                // 修得可能な最大レベルに更新する
                if (skillMaster.openLevel > result)
                {
                    result = skillMaster.openLevel;
                }
            }

            // データが見つからない場合はエラー出す
            if (result < 0)
            {
                CruFramework.Logger.LogError($"Not Find charaId:{mCharaId}, charaLevel:{charaLevel} in SkillCharaMaster");
            }
            
            return result;
        }

        /// <summary> 新規アビリティとレベルアップしたアビリティを取得 </summary>
        public static List<CharaAbilityInfo> GetNewAbilityOrLevelUpAbility(BattleConst.AbilityType abilityType, long charaId, long charaLevel, long liberationLevel, long afterCharaLevel, long afterLiberationLevel)
        {
            // 結果格納用
            List<CharaAbilityInfo> result = new List<CharaAbilityInfo>();
            
            // 現在のレベルで取得しているスキル
            List<CharaAbilityInfo> currentLevelSkillList = GetAbilityAcquireList(abilityType, charaId, charaLevel, liberationLevel);
            // 強化後のレベルで取得しているスキル
            List<CharaAbilityInfo> afterLevelSkillList = GetAbilityAcquireList(abilityType, charaId, afterCharaLevel, afterLiberationLevel);

            // 現在のレベルスキルのSkillIdごとのリスト
            Dictionary<long, CharaAbilityInfo> currentLevelSkillDictionary = new Dictionary<long, CharaAbilityInfo>();

            // Dictionary作成
            foreach (CharaAbilityInfo info in currentLevelSkillList)
            {
                currentLevelSkillDictionary.Add(info.SkillId, info);
            }


            foreach (CharaAbilityInfo afterSkillInfo in afterLevelSkillList)
            {
                // 同じSkillIdがあるならスキルレベルを比較する
                if (currentLevelSkillDictionary.TryGetValue(afterSkillInfo.SkillId, out CharaAbilityInfo currentInfo))
                {
                    // 現在のスキルレベル以下なら追加はしない
                    if (afterSkillInfo.SkillLevel <= currentInfo.SkillLevel)
                    {
                        continue;
                    }
                }
                
                // 新規スキル、スキルレベルアップなら追加
                result.Add(afterSkillInfo);
            }

            return result;
        }

        /// <summary> スキルのレベルアップ毎のデータ一覧を返す </summary>
        public static List<CharaAbilityInfo> GetLevelUpSkillList(BattleConst.AbilityType abilityType, long mCharaId, long skillId, long charaLevel, long liberationLevel)
        {
            List<CharaAbilityInfo> result = new List<CharaAbilityInfo>();
            
            foreach (SkillCharaMasterObject skillMaster in MasterManager.Instance.skillCharaMaster.values)
            {
                // キャラIdが違う
                if (skillMaster.mCharaId != mCharaId) continue;
                // アビリティタイプが違う
                if(MasterManager.Instance.abilityMaster.FindData(skillMaster.mSkillId).AbilityType != abilityType) continue;
                // スキルIdが違う
                if(skillMaster.mSkillId != skillId) continue;
                // 同じスキルのレベルデータがすでに追加済み
                if (result.Any(x => x.SkillLevel == skillMaster.skillLevel)) continue;
                
                // 該当のスキルレベルが解放されるマスタを取得
                SkillCharaMasterObject liberationMaster = GetAbilityLiberationMaster(abilityType, mCharaId, skillId, skillMaster.skillLevel);
                if (liberationMaster == null)
                {
                    continue;
                }
                
                // スキルがアンロック済みか
                bool isUnlockSkill = charaLevel >= liberationMaster.openLevel && liberationLevel >= liberationMaster.openLiberationLevel;
               
                result.Add(new CharaAbilityInfo(liberationMaster, abilityType, isUnlockSkill));
            }

            return result;
        }

        /// <summary> 該当のスキルが解放される際の対象マスタ(最も低レベルで発生する解放条件のマスタを取得する) </summary>
        public static SkillCharaMasterObject GetAbilityLiberationMaster(BattleConst.AbilityType abilityType, long mCharaId, long skillId, long skillLevel)
        {
            SkillCharaMasterObject result = null;
            
            foreach (SkillCharaMasterObject skillMaster in MasterManager.Instance.skillCharaMaster.values)
            {
                // キャラIdが違う
                if (skillMaster.mCharaId != mCharaId) continue;
                // スキルIdが違う
                if(skillMaster.mSkillId != skillId) continue;
                // アビリティタイプが違う
                if(MasterManager.Instance.abilityMaster.FindData(skillMaster.mSkillId).AbilityType != abilityType) continue;
                // スキルレベルが違う
                if(skillMaster.skillLevel != skillLevel) continue;
                // 保持しているデータのキャラレベルよりも大きいものはとばす
                if(result != null && skillMaster.openLevel > result.openLevel) continue;
                // 保持しているデータの解放レベルよりも大きいものはとばす
                if(result != null && skillMaster.openLiberationLevel > result.openLiberationLevel) continue;

                result = skillMaster;
            }

            return result;
        }
    }
}