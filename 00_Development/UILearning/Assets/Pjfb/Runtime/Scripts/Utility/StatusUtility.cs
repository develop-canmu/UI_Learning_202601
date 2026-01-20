using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.InGame;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb
{
    
// spd		    スピード（万分率）
// tec		    テクニック（万分率）
// param1(sta)	スタミナ（万分率）
// param2(phy)	フィジカル（万分率）
// param3(sig)	視野（万分率）
// param4(kic)	キック（万分率）
// param5(wis)	賢さ（万分率）

    [System.Serializable]
    public struct StatusParseTemp
    {
        [SerializeField]
        public long hp;
        [SerializeField]
        public long mp;
        [SerializeField]
        public long atk;
        [SerializeField]
        public long def;
        [SerializeField]
        public long spd;
        [SerializeField]
        public long tec;
        [SerializeField]
        public long param1;
        [SerializeField]
        public long param2;
        [SerializeField]
        public long param3;
        [SerializeField]
        public long param4;
        [SerializeField]
        public long param5;
        [SerializeField]
        public long exParam1;
        [SerializeField]
        public long exParam2;
        [SerializeField]
        public long exParam3;
    }
    
    public struct CharacterStatus
    {
        private BigValue stamina;
        /// <summary>スタミナ</summary>
        public BigValue Stamina{get{return stamina;}set{stamina = value;}}
        
        private BigValue speed;
        /// <summary>スピード</summary>
        public BigValue Speed{get{return speed;}set{speed = value;}}
        
        private BigValue technique;
        /// <summary>テクニック</summary>
        public BigValue Technique{get{return technique;}set{technique = value;}}
        
        private BigValue physical;
        /// <summary>フィジカル</summary>
        public BigValue Physical{get{return physical;}set{physical = value;}}
        
        private BigValue kick;
        /// <summary>キック</summary>
        public BigValue Kick{get{return kick;}set{kick = value;}}
        
        private BigValue intelligence;
        /// <summary>賢さ</summary>
        public BigValue Intelligence{get{return intelligence;}set{intelligence = value;}}
        
        private BigValue shootRange;
        /// <summary>シュートレンジ</summary>
        public BigValue ShootRange{get{return shootRange;}set{shootRange = value;}}
        
        
        private BigValue tip;
        /// <summary>チップ</summary>
        public BigValue Tip{get{return tip;}set{tip = value;}}

        public BigValue TotalStatusExceptShootRange()
        {
            BigValue total = BigValue.Zero;
            foreach (CharacterStatusType item in Enum.GetValues(typeof(CharacterStatusType)))
            {
                if (item == CharacterStatusType.ShootRange) continue;

                total += this[item];
            }

            return total;
        }
        
        public BigValue this[CharacterStatusType type]
        {
            get
            {
                switch(type)
                {
                    case CharacterStatusType.Stamina:return stamina;
                    case CharacterStatusType.Speed:return speed;
                    case CharacterStatusType.Technique:return technique;
                    case CharacterStatusType.Physical:return physical;
                    case CharacterStatusType.Intelligence:return intelligence;
                    case CharacterStatusType.Kick:return kick;
                    case CharacterStatusType.ShootRange:return shootRange;
                }
                return BigValue.Zero;
            }
            set
            {
                switch(type)
                {
                    case CharacterStatusType.Stamina:
                        stamina = value;
                        break;
                    case CharacterStatusType.Speed:
                        speed = value;
                        break;
                    case CharacterStatusType.Technique:
                        technique = value;
                        break;
                    case CharacterStatusType.Physical:
                        physical = value;
                        break;
                    case CharacterStatusType.Intelligence:
                        intelligence = value;
                        break;
                    case CharacterStatusType.Kick:
                        kick = value;
                        break;
                    case CharacterStatusType.ShootRange:
                        shootRange = value;
                        break;
                }
            }
        }
        
        public static CharacterStatus operator * (CharacterStatus left, BigValue right)
        {
            CharacterStatus result = new CharacterStatus();

            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                result[type] = BigValue.MulRate(left[type], right);
            }
            
            return result;
        }
        
        public static CharacterStatus operator / (CharacterStatus left, BigValue right)
        {
            CharacterStatus result = new CharacterStatus();

            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                result[type] = BigValue.DivRate(left[type], right);
            }
            
            return result;
        }
        
        public static CharacterStatus operator + (CharacterStatus left, CharacterStatus right)
        {
            CharacterStatus result = new CharacterStatus();

            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                result[type] = left[type] + right[type];
            }
            
            return result;
        }
        
        public static CharacterStatus operator - (CharacterStatus left, CharacterStatus right)
        {
            CharacterStatus result = new CharacterStatus();

            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                result[type] = left[type] - right[type];
            }
            
            return result;
        }
    }
    
    /// <summary>ステータスタイプ</summary>
    public enum CharacterStatusType
    {
        Speed = 1, 
        Technique = 2, 
        Stamina = 3,
        Physical = 4, 
        //FieldOfView = 5, 
        Kick = 6, 
        Intelligence = 7, 
        ShootRange = 9
    }

    
    
    /// <summary>
    /// ステータス計算用クラス
    /// </summary>
    public static class StatusUtility
    {
        private const int ShootRangeShortThreshold = 200;
        private const int ShootRangeMiddleThreshold = 450;
    
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameStamina{get{return StringValueAssetLoader.Instance["character.status.stamina"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameSpeed{get{return StringValueAssetLoader.Instance["character.status.speed"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNamePhysical{get{return StringValueAssetLoader.Instance["character.status.physical"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameTechnique{get{return StringValueAssetLoader.Instance["character.status.technique"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameFoV{get{return StringValueAssetLoader.Instance["character.status.field_of_view"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameIntelligence{get{return StringValueAssetLoader.Instance["character.status.intelligence"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameKick{get{return StringValueAssetLoader.Instance["character.status.kick"];}}
        /// <summary>ステータス名の取得</summary>
        public static string StatusNameShootRange{get{return StringValueAssetLoader.Instance["character.status.shoot_range"];}}
        
        /// <summary>ステータスの取得</summary>
        public static string GetStatusName(CharacterStatusType statusType)
        {
            switch(statusType)
            {
                case CharacterStatusType.Stamina:return StatusNameStamina;
                case CharacterStatusType.Speed:return StatusNameSpeed;
                case CharacterStatusType.Physical:return StatusNamePhysical;
                case CharacterStatusType.Technique:return StatusNameTechnique;
                case CharacterStatusType.Intelligence:return StatusNameIntelligence;
                case CharacterStatusType.Kick:return StatusNameKick;
                case CharacterStatusType.ShootRange:return StatusNameShootRange;
            }
            return string.Empty;
        }
        
        /// <summary>
        /// ステータスの計算
        /// サーバーと計算式を合わせる必要がある
        /// </summary>
        private static BigValue CalcCharacterStatus(BigValue valueA, BigValue valueB, BigValue valueC, long rateB, long rateC)
        {
            return (valueA + valueB + valueC) * (rateB + 100) * (rateC + 100) / 10000;
        }
        

        /// <summary>キャラIdとレベルを指定してステータスを計算</summary>
        public static CharacterStatus CalcCharacterStatus(long characterId, long lv, long liberationLv)
        {
            CharacterStatus result = new CharacterStatus();
            
            // MChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(characterId);
            // MStatusAdditionLevel
            StatusAdditionLevelMasterObject statusAdditionLevelA = MasterManager.Instance.statusAdditionLevelMaster.FindData(mChar.mStatusAdditionIdGrowth, lv);
            // MStatusAdditionLevel
            StatusAdditionLevelMasterObject statusAdditionLevelB = MasterManager.Instance.statusAdditionLevelMaster.FindData(mChar.mStatusAdditionIdLiberation, liberationLv);
            
#if UNITY_EDITOR
            if(statusAdditionLevelA == null || statusAdditionLevelB == null)
            {
                CruFramework.Logger.LogError($"error CalcCharacterStatus : characterId = {characterId} : mStatusAdditionIdGrowth = {mChar.mStatusAdditionIdGrowth}, mStatusAdditionIdLiberation = {mChar.mStatusAdditionIdLiberation}, Lv = {lv}");
                return result;
            }
#endif
            
            // スタミナ
            result.Stamina = CalcCharacterStatus(mChar.param1, statusAdditionLevelA.param1, statusAdditionLevelB.param1, statusAdditionLevelA.param1Rate, statusAdditionLevelB.param1Rate);
            // テクニック
            result.Technique = CalcCharacterStatus(mChar.tec, statusAdditionLevelA.tec, statusAdditionLevelB.tec, statusAdditionLevelA.tecRate, statusAdditionLevelB.tecRate);
            // スピード
            result.Speed = CalcCharacterStatus(mChar.spd, statusAdditionLevelA.spd, statusAdditionLevelB.spd, statusAdditionLevelA.spdRate, statusAdditionLevelB.spdRate);
            // フィジカル
            result.Physical = CalcCharacterStatus(mChar.param2, statusAdditionLevelA.param2, statusAdditionLevelB.param2, statusAdditionLevelA.param2Rate, statusAdditionLevelB.param2Rate);
            // キック
            result.Kick = CalcCharacterStatus(mChar.param4, statusAdditionLevelA.param4, statusAdditionLevelB.param4, statusAdditionLevelA.param4Rate, statusAdditionLevelB.param4Rate);
            // 賢さ
            result.Intelligence = CalcCharacterStatus(mChar.param5, statusAdditionLevelA.param5, statusAdditionLevelB.param5, statusAdditionLevelA.param5Rate, statusAdditionLevelB.param5Rate);
            // シュートレンジ
            result.ShootRange = mChar.exParam1;
            
            return result;
        }
        
        /// <summary>ユーザーキャラIdを指定してステータスを計算</summary>
        public static CharacterStatus CalcUserCharacterStatus(int userCharacterId)
        {
            UserDataChara uChar = UserDataManager.Instance.chara.data[userCharacterId];
            
            return CalcCharacterStatus(uChar.charaId, uChar.level, uChar.newLiberationLevel);
        }
        
        /// <summary>ユーザーキャラIdを指定してステータスを計算</summary>
        public static CharacterStatus ToCharacterStatus(BattleV2Chara battleChara)
        {
            CharacterStatus status = new CharacterStatus();
            status.Speed = new BigValue(battleChara.spd);
            status.Technique = new BigValue(battleChara.tec);
            status.Stamina = new BigValue(battleChara.param1);
            status.Physical = new BigValue(battleChara.param2);
            status.Kick = new BigValue(battleChara.param4);
            status.Intelligence = new BigValue(battleChara.param5);
            status.ShootRange = new BigValue(battleChara.exParam1);
            return status;
        }
        
        public static CharacterStatus ToCharacterStatus(BattleCharacterModel battleChara)
        {
            CharacterStatus status = new CharacterStatus();
            status.Speed = battleChara.baseSpeed;
            status.Technique = battleChara.baseTechnique;
            status.Stamina = battleChara.baseStamina;
            status.Physical = battleChara.basePhysical;
            status.Kick = battleChara.baseKick;
            status.Intelligence = battleChara.baseWise;
            status.ShootRange = battleChara.baseShootRange;
            return status;
        }
        
        public static CharacterStatus ToCharacterStatus(CharaNpcMasterObject npcChara)
        {
            CharacterStatus status = new CharacterStatus();
            status.Speed = npcChara.spd;
            status.Technique = npcChara.tec;
            status.Stamina = npcChara.param1;
            status.Physical = npcChara.param2;
            status.Kick = npcChara.param4;
            status.Intelligence = npcChara.param5;
            // mChara
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(npcChara.mCharaId);
            status.ShootRange = mChara.exParam1;
            return status;
        }
        
        public static CharacterStatus ToCharacterStatus(UserDataCharaVariable uCharaVariable)
        {
            CharacterStatus status = new CharacterStatus();
            status.Speed = new BigValue(uCharaVariable.spd);
            status.Technique = new BigValue(uCharaVariable.tec);
            status.Stamina = new BigValue(uCharaVariable.param1);
            status.Physical = new BigValue(uCharaVariable.param2);
            status.Kick = new BigValue(uCharaVariable.param4);
            status.Intelligence = new BigValue(uCharaVariable.param5);
            // mChara
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(uCharaVariable.charaId);
            status.ShootRange = mChara.exParam1;
            return status;
        }
        
        public static CharacterStatus ToCharacterStatus(CharaVariableRecommendStatus recommendStatus)
        {
            CharacterStatus status = new CharacterStatus();
            status.Speed = new BigValue(recommendStatus.spd);
            status.Technique = new BigValue(recommendStatus.tec);
            status.Stamina = new BigValue(recommendStatus.param1);
            status.Physical = new BigValue(recommendStatus.param2);
            status.Kick = new BigValue(recommendStatus.param4);
            status.Intelligence = new BigValue(recommendStatus.param5);
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(recommendStatus.mCharaId);
            status.ShootRange = mChara.exParam1;
            return status;
        }

        /// <summary>ステータスのランクを取得</summary>
        public static long GetStatusRank(BigValue value)
        {
            return GetRank(CharaRankMasterStatusType.Status, value);
        }
        
        /// <summary>ステータスのランクを取得</summary>
        public static string GetStatusRankName(BigValue value)
        {
            return GetRankName(CharaRankMasterStatusType.Status, value);
        }
        
        /// <summary>パーティのランクを取得</summary>
        public static long GetPartyRank(BigValue value)
        {
            return GetRank(CharaRankMasterStatusType.PartyTotal, value);
        }
        
        /// <summary>パーティのランクを取得</summary>
        public static string GetPartyRankName(BigValue value)
        {
            return GetRankName(CharaRankMasterStatusType.PartyTotal, value);
        }
        
        /// <summary>キャラのランクを取得</summary>
        public static long GetCharacterRank(BigValue value)
        {
            return GetRank(CharaRankMasterStatusType.CharacterTotal, value);
        }
        
        /// <summary>キャラのランクを取得</summary>
        public static long GetCharacterNextRank(BigValue value)
        {
            return GetNextRank(CharaRankMasterStatusType.CharacterTotal, value);
        }
        
        /// <summary>キャラのランクを取得</summary>
        public static BigValue GetCharacterNextRankValue(BigValue value)
        {
            return GetNextRankValue(CharaRankMasterStatusType.CharacterTotal, value);
        }
        
        /// <summary>キャラのランクを取得</summary>
        public static BigValue GetCharacterRankValue(BigValue value)
        {
            return GetRankValue(CharaRankMasterStatusType.CharacterTotal, value);
        }
        
        /// <summary>パーティのランクを取得</summary>
        public static string GetCharacterRankName(BigValue value)
        {
            return GetRankName(CharaRankMasterStatusType.CharacterTotal, value);
        }
        
        
        /// <summary>ステータスのランクを取得</summary>
        public static long GetRank(CharaRankMasterStatusType type, BigValue value)
        {
            CharaRankMasterObject[] ranks = MasterManager.Instance.charaRankMaster.FindDatas(type);
            return GetRank(ranks, value).rankNumber;
        }
        
        /// <summary>ステータスのランクを取得</summary>
        public static long GetNextRank(CharaRankMasterStatusType type, BigValue value)
        {
            CharaRankMasterObject[] ranks = MasterManager.Instance.charaRankMaster.FindDatas(type);
            return GetNextRank(ranks, value).rankNumber;
        }
        
        /// <summary>ステータスのランクを取得</summary>
        public static BigValue GetNextRankValue(CharaRankMasterStatusType type, BigValue value)
        {
            CharaRankMasterObject[] ranks = MasterManager.Instance.charaRankMaster.FindDatas(type);
            return new BigValue(GetNextRank(ranks, value).minValue);
        }
        
        /// <summary>ステータスのランクを取得</summary>
        public static BigValue GetRankValue(CharaRankMasterStatusType type, BigValue value)
        {
            CharaRankMasterObject[] ranks = MasterManager.Instance.charaRankMaster.FindDatas(type);
            return new BigValue(GetRank(ranks, value).minValue);
        }
        
        /// <summary>ステータスのランクを取得</summary>
        public static string GetRankName(CharaRankMasterStatusType type, BigValue value)
        {
            CharaRankMasterObject[] ranks = MasterManager.Instance.charaRankMaster.FindDatas(type);
            return GetRank(ranks, value).name;
        }

        private static CharaRankMasterObject GetRank(CharaRankMasterObject[] ranks, BigValue value)
        {
            for(int i=ranks.Length-1;i>=0;i--)
            {
                CharaRankMasterObject rank = ranks[i];
                if(new BigValue(rank.minValue) <= value)return rank;
            }
            
            // ここまで到達することは無いはずだが一応最低ランクを返しておく
            return ranks[0];
        }
        
        private static CharaRankMasterObject GetNextRank(CharaRankMasterObject[] ranks, BigValue value)
        {
            CharaRankMasterObject result = null;
            
            result = ranks[ranks.Length-1];
            
            for(int i=ranks.Length-1;i>=0;i--)
            {
                CharaRankMasterObject rank = ranks[i];
                if(new BigValue(rank.minValue) <= value)
                {
                    return result;
                }
                result = rank;

            }
            
            if(result == null)
            {
                return ranks[0];
            }
            return result;
        }
        
        public static string GetRankNameByRankNumber(CharaRankMasterStatusType type, long rankNumber)
        {
            return MasterManager.Instance.charaRankMaster.FindDatas(type).FirstOrDefault(x => x.rankNumber == rankNumber)?.name ?? null;
        }


        public static CharacterStatus Parse(string json)
        {
            CharacterStatus result = new CharacterStatus();
            
            if(string.IsNullOrEmpty(json) || json == "0")return result;
            try {
                StatusParseTemp temp = JsonUtility.FromJson<StatusParseTemp>(json);
                result.Tip = new BigValue(temp.hp);
                result.Speed = new BigValue(temp.spd);
                result.Technique = new BigValue(temp.tec);
                result.Stamina = new BigValue(temp.param1);
                result.Physical = new BigValue(temp.param2);
                result.Kick = new BigValue(temp.param4);
                result.Intelligence = new BigValue(temp.param5);
                result.ShootRange = new BigValue(temp.exParam1);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("CharacterStatus Parse error = " + e.Message + " : json = " + json );
                throw e;
            }
            
            return result;
        }
        
        public static CharacterStatus Parse(CombinationStatusCommon combinationStatusCommon)
        {
            CharacterStatus result = new CharacterStatus();
            
            if(combinationStatusCommon == null)return result;
            
            result.Speed = new BigValue(combinationStatusCommon.spd);
            result.Technique = new BigValue(combinationStatusCommon.tec);
            result.Stamina = new BigValue(combinationStatusCommon.param1);
            result.Physical = new BigValue(combinationStatusCommon.param2);
            result.Kick = new BigValue(combinationStatusCommon.param4);
            result.Intelligence = new BigValue(combinationStatusCommon.param5);
            return result;
        }
        
        public static CharacterStatus Parse(CharaVariableTrainerStatusCommon charaVariableTrainerStatusCommon)
        {
            CharacterStatus result = new CharacterStatus();
            
            if(charaVariableTrainerStatusCommon == null)return result;
            
            result.Speed = new BigValue(charaVariableTrainerStatusCommon.spd);
            result.Technique = new BigValue(charaVariableTrainerStatusCommon.tec);
            result.Stamina = new BigValue(charaVariableTrainerStatusCommon.param1);
            result.Physical = new BigValue(charaVariableTrainerStatusCommon.param2);
            result.Kick = new BigValue(charaVariableTrainerStatusCommon.param4);
            result.Intelligence = new BigValue(charaVariableTrainerStatusCommon.param5);
            return result;
        }
        
        public static string GetShootRangeName(BigValue value)
        {
            if (value < ShootRangeShortThreshold)
            {
                return StringValueAssetLoader.Instance["character.status.shoot_range.normal"];
            }
            else if (value < ShootRangeMiddleThreshold)
            {
                return StringValueAssetLoader.Instance["character.status.shoot_range.middle"];
            }

            return StringValueAssetLoader.Instance["character.status.shoot_range.long"];
        }
        
        /// <summary>戦力</summary>
        public static BigValue GetCombatPower(CharacterStatus status, SkillData[] abilityList)
        {
            return GetCombatPower(status.Stamina, status.Speed, status.Physical, status.Technique, status.Kick, status.Intelligence, abilityList);
        }
        
        /// <summary>戦力</summary>
        public static BigValue GetCombatPower(BigValue stamina, BigValue speed, BigValue physical, BigValue technique, BigValue kick, BigValue intelligence, SkillData[] abilityList)
        {
            // 基礎ステータスの合算
            BigValue combatPower = stamina + speed + physical + technique + kick + intelligence;
            // スキル評価点
            for(int i = 0; i < abilityList.Length; i++)
            {
                AbilityMasterObject mAbility = MasterManager.Instance.abilityMaster.FindData(abilityList[i].Id);
                combatPower += mAbility.combatPower;
                combatPower += mAbility.combatPowerAddValue * (abilityList[i].Level-1);
            }
            
            return combatPower;
        }
        
        
        public static CharacterStatusType CharacterTypeToStatusType(CharacterType type)
        {
            switch(type)
            {
                case CharacterType.Stamina:return CharacterStatusType.Stamina;
                case CharacterType.Speed:return CharacterStatusType.Speed;
                case CharacterType.Physical:return CharacterStatusType.Physical;
                case CharacterType.Technique:return CharacterStatusType.Technique;
                case CharacterType.Intelligence:return CharacterStatusType.Intelligence;
                case CharacterType.Kick:return CharacterStatusType.Kick;
            }
            
            return default;
        }
    }
}
