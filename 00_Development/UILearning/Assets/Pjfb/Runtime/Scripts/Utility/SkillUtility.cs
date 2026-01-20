using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniJSON;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using WrapperIntList = Pjfb.Master.WrapperIntList;

namespace Pjfb
{
    
    [System.Serializable]
    public struct SkillData
    {
        [SerializeField]
        private long id;
        /// <summary>Id</summary>
        public long Id{get{return id;}}
                
        [SerializeField]
        private long level;
        /// <summary>Level</summary>
        public long Level{get{return level;}}
            
        public SkillData(long id, long level)
        {
            this.id = id;
            this.level = level;
        }
    }
    
    public class EventSkillData
    {
        private long eventId = 0;
        /// <summary>EventId</summary>
        public long EventId{get{return eventId;}}
        
        private List<SkillData> skillList = new List<SkillData>();
        /// <summary>スキルリスト</summary>
        public IReadOnlyList<SkillData> SkillList{get{return skillList;}}
        
        public EventSkillData(long eventId, List<SkillData> skillList)
        {
            this.eventId = eventId;
            this.skillList = skillList;
        }
    }
    
    public static class SkillUtility
    {
               /// <summary>キャラクタの獲得できるスキルを取得</summary>
        public static List<EventSkillData> GetCharacterEventSkillList(long mCharId, long trainingScenarioId)
        {
            
            List<EventSkillData> result = new List<EventSkillData>();
            
            // スキルIdを取得            
            List<TrainingEventMasterObject> eventList = GetCharacterEventList(mCharId, trainingScenarioId);
            // イベントに紐づくスキルを取得
            foreach(TrainingEventMasterObject e in eventList)
            {
                List<SkillData> abilityList = GetSkillList(e);
                result.Add( new EventSkillData(e.id, abilityList) );
            }
            
            return result;
        }
        

        /// <summary>キャラクタの獲得できるスキルを取得</summary>
        public static List<EventSkillData> GetSupportCharacterEventSkillList(long mCharId, long trainingScenarioId)
        {            
            List<EventSkillData> result = new List<EventSkillData>();
            
            // スキルIdを取得            
            List<TrainingEventMasterObject> eventList = GetSupportCharacterEventList(mCharId, trainingScenarioId);
            // イベントに紐づくスキルを取得
            foreach(TrainingEventMasterObject e in eventList)
            {
                List<SkillData> abilityList = GetSkillList(e);
                result.Add( new EventSkillData(e.id, abilityList) );
            }
            
            return result;
        }
        
        /// <summary>イベントを取得</summary>
        public static List<EventSkillData> GetUnitEventList(long mUnitId, long trainingScenarioId)
        {
            List<EventSkillData> result = new List<EventSkillData>();
            // スキルIdを取得            
            List<TrainingEventMasterObject> eventList = new List<TrainingEventMasterObject>();
            // 該当するイベント
            foreach(TrainingEventMasterObject e in MasterManager.Instance.trainingEventMaster.values)
            {
                if(e.mTrainingUnitId == mUnitId && (e.trainingMCharaId == 0 || trainingScenarioId < 0 || e.trainingMCharaId == trainingScenarioId) )
                {
                    List<SkillData> abilityList = GetSkillList(e);
                    result.Add( new EventSkillData(e.id, abilityList) );
                }
            }

            return result;
        }
        
        /// <summary>イベントを取得</summary>
        private static List<TrainingEventMasterObject> GetSupportCharacterEventList(long mCharId, long trainingScenarioId)
        {
            // スキルIdを取得            
            List<TrainingEventMasterObject> eventList = new List<TrainingEventMasterObject>();
            // 該当するイベント
            foreach(TrainingEventMasterObject e in MasterManager.Instance.trainingEventMaster.values)
            {
                if(e.mTrainingScenarioId == 0 || trainingScenarioId < 0 || e.mTrainingScenarioId == trainingScenarioId)
                {
                    if(e.trainingMCharaId == 0 && e.supportMCharaId == mCharId)
                    {
                        eventList.Add(e);
                    }
                }
            }
            
            return eventList;
        }
        
        /// <summary>イベントに紐づくスキルを取得</summary>
        public static List<SkillData> GetSkillList(TrainingEventMasterObject e)
        {
            List<SkillData> abilityList = new List<SkillData>();
                
            foreach(WrapperIntList list in e.choicePrizeJson)
            {
                TrainingEventRewardMasterObject mReward = MasterManager.Instance.trainingEventRewardMaster.FindData(list.l[1]);

                List<object> abilityIdList = (List<object>)MiniJSON.Json.Deserialize(mReward.getAbilityJson);
                foreach(object abilityId in abilityIdList)
                {
                    Dictionary<string, object> dic = (Dictionary<string, object>)abilityId;
                        
                    long id = (long)System.Convert.ChangeType(dic["id"], typeof(long));
                    long level = (long)System.Convert.ChangeType(dic["level"], typeof(long));
                    SkillData data = new SkillData(id, level);
                    abilityList.Add(data);
                }
            }
            
            return abilityList;
        }
        
        /// <summary>イベントを取得</summary>
        private static List<TrainingEventMasterObject> GetCharacterEventList(long mCharId, long trainingScenarioId)
        {
            // スキルIdを取得            
            List<TrainingEventMasterObject> eventList = new List<TrainingEventMasterObject>();
            // 該当するイベント
            foreach(TrainingEventMasterObject e in MasterManager.Instance.trainingEventMaster.values)
            {
                // シナリオIdのチェック
                if(e.mTrainingScenarioId == 0 || trainingScenarioId < 0 || e.mTrainingScenarioId == trainingScenarioId)
                {
                    if(e.supportMCharaId == 0 && e.trainingMCharaId == mCharId)
                    {
                        eventList.Add(e);
                    }
                }
            }
            
            return eventList;
        }
        
        /// <summary> スキル種類によって異なる文字色を取得する </summary>
        public static string GetSkillTextColorKey(AbilityMasterObject.AbilityCategory abilityCategory)
        {
            // 文字色変更
            string colorKey;
            switch (abilityCategory)
            {
                // 通常スキル
                case AbilityMasterObject.AbilityCategory.Normal:
                    colorKey = "default";
                    break;
                // FLOWスキル
                case AbilityMasterObject.AbilityCategory.Flow:
                    colorKey = "white";
                    break;
                    
                default:
                    CruFramework.Logger.LogError($"AbilityCategory:{abilityCategory} は未実装です");
                    colorKey = "default";
                    break;
            }

            return colorKey;
        }
    }
    
    /*

    public enum PracticeSkillType
    {
        Unique                    = 0,
        FirstParamAdd             = 1,
        PracticeParamAdd          = 2,
        PracticeParamEnhanceRate  = 3,
        BattleParamEnhanceRate    = 4,
        ConditionDiscountPractice = 5,
        ConditionDiscountFusion   = 6,
        RarePracticeEnhanceRate   = 7,
        SkillLevelUp              = 8,
        PracticeLevelEnhance      = 9,
        PopRateUp                 = 10,
        SpecialRateUp             = 11,
        ConditionRecoverUp        = 12,
        EventStatusEnhanceRate    = 13,
        PracticeExpEnhanceRate    = 14,
        PracticeTipEnhanceRate    = 15,
        BattleTipEnhanceRate      = 16,
        EventTipEnhanceRate       = 17,
        ConditionEffectGradeUpMapOnType = 18,
        PracticeParamAddBonusMap = 19,
        PracticeParamEnhanceMapOnType = 20,
        RarePracticeEnhanceRateMapOnType = 21,
        PopRateEnhanceMapOnType = 22,
        FirstReward = 23,
        ConditionEffectGradeUpRate = 24,
        PracticeParamRateMap = 25,
        ConditionDiscountRate = 26,
    }

    public enum PracticeSkillIconType
    {
        //▼固有スキル系
        //・固有スキル
        //▼ステアップ系
        //・初期〇〇アップ			
        //・練習時〇〇アップ(練習時)			
        //・イベントステータスアップ	
        //・トレーニング固定ボーナス
        //・練習時特定ステータス獲得量%アップ
        //▼練習ボーナス系
        //・練習ボーナス
        //▼練習試合ボーナス系
        //・練習試合ボーナス	
        //▼コンディション消費緩和系
        //・練習コンディション消費軽減			
        //・合成コンディション消費軽減	
        //▼レア練習カード出現率アップ系
        //・レア練習アップ
        //・特定スペシャルトレーニング出現率アップ
        //▼スキルレベルアップ系
        //・スキルレベルアップ
        //・練習時発生ボーナス%上昇
        //▼練習レベルボーナス系
        //・練習レベルボーナス	
        //▼レクチャー系
        //・レクチャー率	
        //・特定スペシャルレクチャー確率アップ
        //▼コンディション回復系
        //・コンディション回復			
        //・休息時「食堂」発生率上昇
        //・休息時「就寝」発生率上昇
        //▼スペシャルレクチャーボーナス系
        //・スペシャルレクチャーボーナス	

        Unique                    = 0,
        StatusUp                  = 1,
        PracticeParamEnhanceRate  = 2,
        BattleParamEnhanceRate    = 3,
        ConditionDiscount         = 4,
        RarePracticeEnhanceRate   = 5,
        SkillLevelUp              = 6,
        PracticeLevelEnhance      = 7,
        PopRateUp                 = 8,
        ConditionRecoverUp        = 9,
        SpecialRateUp             = 10,
    }
    
    public enum PracticeType
    {
        None = -1,
        MuscleTrainingMachine = 0,
        FullPowerShuttleRun = 1,
        DribbleTraining = 2,
        PairBlmShootPractice = 3,
        ImageTraining = 4,
    }
    
    public enum PracticeSkillCategoryType
    {
        None = 0,
        Main = 1,
        Sub = 2,
    }
    
    public class PracticeSkillData
    {
        
        private float value = 0;
        /// <summary>効果値</summary>
        public  float Value{get{return value;}set{this.value = value;}}
        
        private string unit = string.Empty;
        /// <summary>単位</summary>
        public  string Unit{get{return unit;}}
        
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        private string description = string.Empty;
        /// <summary>説明</summary>
        public string Description{get{return description;}}
        
        private long mCharId = 0;
        /// <summary>mCharId</summary>
        public long MCharId{get{return mCharId;}}
        
        private long characterLv = 0;
        /// <summary>mCharId</summary>
        public long CharacterLv{get{return characterLv;}}
        
        private long liberationLv = 0;
        /// <summary>Lv</summary>
        public long LiberationLv{get{return liberationLv;}}
        
        private long masterId = 0;
        /// <summary>CharaTrainingStatus のマスタId</summary>
        public long MasterId{get{return masterId;}}
        
        private PracticeSkillType skillType = PracticeSkillType.Unique;
        /// <summary>スキルの種類</summary>
        public PracticeSkillType SkillType{get{return skillType;}}
        
        private List<PracticeSkillType> uniqueSkillTypes = new List<PracticeSkillType>();
        /// <summary>ユニークスキルの場合の内包するタイプ</summary>
        public IReadOnlyList<PracticeSkillType> UniqueSkillTypes{get{return uniqueSkillTypes;}}
        
        private List<CharacterStatusType> uniqueSkillStatusTypes = new List<CharacterStatusType>();
        /// <summary>ユニークスキルの場合の内包するタイプ</summary>
        public IReadOnlyList<CharacterStatusType> UniqueSkillStatusTypes{get{return uniqueSkillStatusTypes;}}

        private PracticeSkillIconType skillIconType = PracticeSkillIconType.Unique;
        /// <summary>スキルの種類</summary>
        public PracticeSkillIconType SkillIconType{get{return skillIconType;}}
        
        private CharacterStatusType statusType = CharacterStatusType.Speed;
        /// <summary>スキルの種類</summary>
        public CharacterStatusType StatusType{get{return statusType;}}
        
        private  bool isEnableTraining = false;
        /// <summary>トレーニングで有効</summary>
        public bool IsEnableTraining{get{return isEnableTraining;}}
        
        private PracticeSkillCategoryType category = PracticeSkillCategoryType.None;
        /// <summary>カテゴリ</summary>
        public PracticeSkillCategoryType Category{get{return category;}set{category = value;}}
        
        private PracticeType practiceType = PracticeType.None;
        /// <summary>練習タイプ</summary>
        public PracticeType PracticeType{get{return practiceType;}}
        
        private long[] statusIdList = null;
        /// <summary>サポート器具のサブスキル</summary>
        public long[] StatusIdList{get{return statusIdList;}set{statusIdList = value;}}

        public bool ContainsStatusType(CharacterStatusType type)
        {
            if(statusType == type)return true;
            return uniqueSkillStatusTypes.Contains(type);
        }
        
        public bool ContainsPracticeParamAdd(CharacterStatus status)
        {
            if(skillType == PracticeSkillType.PracticeParamAdd)
            {
                if(status[statusType] > 0)return true;
            }

            foreach(CharacterStatusType type in uniqueSkillStatusTypes)
            {
                if(status[type] > 0)return true;
            }
            
            return false;
        }
        
        public bool ContainsPracticeBonusParamAdd(CharacterStatus status)
        {
            if(skillType == PracticeSkillType.PracticeParamAddBonusMap)
            {
                if(status[statusType] > 0)return true;
            }
            
            return false;
        }
        
        public bool ContainsSkillType(PracticeSkillType type)
        {
            if(skillType == type)return true;
            
            return uniqueSkillTypes.Contains(type);
        }
        
        /// <summary>表示名を取得 名前 + 効果値</summary>
        public string ToName()
        {
            if(skillType == PracticeSkillType.Unique)return name;
            return $"{name} {ToValueName()}";
        }
        
        /// <summary>効果値の文字列を取得 効果値 + 単位(%) </summary>
        public string ToValueName()
        {
            const float digit = 1000;
            var formattedValue = Mathf.Round(value * digit) / digit;
            return $"{formattedValue}{unit}";
        }

        private void Initialize(string name, float value, long mCharId, long characterLv, long liberationLv, string unit, string description, long masterId, PracticeSkillType skillType, PracticeSkillIconType skillIconType, CharacterStatusType statusType, PracticeType practiceType)
        {
            this.name = name;
            this.value = value;
            this.mCharId = mCharId;
            this.characterLv = characterLv;
            this.liberationLv = liberationLv;
            this.unit = unit;
            this.description = description;
            this.masterId = masterId;
            this.skillType = skillType;
            this.skillIconType = skillIconType;
            this.statusType = statusType;
            this.practiceType = practiceType;
        }
        
        
        
        public PracticeSkillData(string name, float value, long mCharId, long characterLv, long liberationLv, string unit, string description, long masterId, PracticeSkillType skillType, PracticeSkillIconType skillIconType, CharacterStatusType statusType)
        {
            Initialize(name, value, mCharId, characterLv, liberationLv, unit, description, masterId, skillType, skillIconType, statusType, PracticeType.None);

            isEnableTraining = SkillUtility.IsEnableTraining(skillType);
        }
        
        public PracticeSkillData(string name, float value, long mCharId, long characterLv, long liberationLv, string unit, string description, long masterId, PracticeSkillType skillType, PracticeSkillIconType skillIconType, CharacterStatusType statusType, PracticeType practiceType)
        {
            Initialize(name, value, mCharId, characterLv, liberationLv, unit, description, masterId, skillType, skillIconType, statusType, practiceType);

            isEnableTraining = SkillUtility.IsEnableTraining(skillType);
        }
        
        public PracticeSkillData(string name, float value, long mCharId, long characterLv, long liberationLv, string unit, string description, long masterId, PracticeSkillType skillType, PracticeSkillIconType skillIconType, CharacterStatusType statusType, List<CharacterStatusType> uniqueSkillStatusTypes, List<PracticeSkillType> uniqueSkillTypes, bool enableTraining)
        {
            Initialize(name, value, mCharId, characterLv, liberationLv, unit, description, masterId, skillType, skillIconType, statusType, PracticeType.None);
            isEnableTraining = enableTraining;
            this.uniqueSkillTypes.AddRange(uniqueSkillTypes);
            this.uniqueSkillStatusTypes.AddRange(uniqueSkillStatusTypes);
        }
    }


    
    /// <summary>
    /// スキル取得等
    /// </summary>
    public static class SkillUtility
    {
        
        public static string PercentString
        {
            get
            {
                return StringValueAssetLoader.Instance["common.percent"];
            }
        }
        
        /// <summary>％に変換</summary>
        public static float ToPercent(float value, float p = 10000.0f)
        {
            return value / p * 100.0f;
        }
        
        public static string GetPracticeTypeString(PracticeType practiceType)
        {
            switch (practiceType)
            {
                case PracticeType.MuscleTrainingMachine:
                    return StringValueAssetLoader.Instance["practice_skill.practice_type.muscle_training_machine"];
                case PracticeType.FullPowerShuttleRun:
                    return StringValueAssetLoader.Instance["practice_skill.practice_type.full_power_shuttle_run"];
                case PracticeType.DribbleTraining:
                    return StringValueAssetLoader.Instance["practice_skill.practice_type.dribble_training"];
                case PracticeType.PairBlmShootPractice:
                    return StringValueAssetLoader.Instance["practice_skill.practice_type.pair_blm_shoot_practice"];
                case PracticeType.ImageTraining:
                    return StringValueAssetLoader.Instance["practice_skill.practice_type.image_training"];
                default:
                    throw new ArgumentOutOfRangeException(nameof(practiceType), practiceType, null);
            }
        }

        

        

        


 

        public static bool IsEnableTraining(PracticeSkillType skillType)
        {
            return skillType == PracticeSkillType.PracticeParamAddBonusMap;
        }

        /// <summary>練習能力の取得</summary>
        private static void GetPracticeSkillData(CharacterStatus status, bool isPercent, long mCharId, long characterLv, long liberationLv, string nameFormat, string descriptionFormat, long masterId, PracticeSkillType skillType, PracticeSkillIconType skillIconType, List<PracticeSkillData> result)
        {
            // 各ステータスをチェック
            foreach(CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                // 値を取得
                long value = status[type];
                // 上昇値なし
                if(value <= 0)continue;
                
                // データ追加
                string name = string.Format(nameFormat, StatusUtility.GetStatusName(type));
                string description = string.Format(descriptionFormat, StatusUtility.GetStatusName(type));
                // リストに追加
                result.Add(new PracticeSkillData(name, isPercent ? ToPercent(value) : value, mCharId, characterLv, liberationLv, isPercent ? PercentString : string.Empty, description, masterId, skillType, skillIconType, type));
            }
        }

        /// <summary>練習能力の取得</summary>
        private static void GetPracticeSkillData(CharaTrainingStatusMasterObject mStatus, long mCharId, long characterLv,
            long liberationLv, List<PracticeSkillData> result)
        {
            if (mStatus == null) return;
            // 初期ステータス
            CharacterStatus status = StatusUtility.Parse(mStatus.firstParamAddMap);
            GetPracticeSkillData(status, false, mCharId, characterLv, liberationLv,
                StringValueAssetLoader.Instance["practice_skill.first_param_up.name"],
                StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], mStatus.id,
                PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, result);

            // 練習時ステータス
            status = StatusUtility.Parse(mStatus.practiceParamAddMap);
            GetPracticeSkillData(status, true, mCharId, characterLv, liberationLv,
                StringValueAssetLoader.Instance["practice_skill.practice_param_up.name"],
                StringValueAssetLoader.Instance["practice_skill.practice_param_up.description"], mStatus.id,
                PracticeSkillType.PracticeParamAdd, PracticeSkillIconType.StatusUp, result);

            // 練習時チップ獲得アップ
            if (status.Tip > 0)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.tip_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.tip_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(status.Tip), mCharId, characterLv, liberationLv,
                    PercentString, description, mStatus.id, PracticeSkillType.PracticeTipEnhanceRate,
                    PracticeSkillIconType.StatusUp, CharacterStatusType.Speed));
            }

            // 練習時ステータス
            status = StatusUtility.Parse(mStatus.battleParamEnhanceMap);
            if (status.Tip > 0)
            {
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.practice_game_tip_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.practice_game_tip_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(status.Tip), mCharId, characterLv, liberationLv,
                    PercentString, description, mStatus.id, PracticeSkillType.BattleTipEnhanceRate,
                    PracticeSkillIconType.StatusUp, CharacterStatusType.Speed));
            }

            // 練習時ステータス
            status = StatusUtility.Parse(mStatus.eventStatusEnhanceMap);
            if (status.Tip > 0)
            {
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.event_tip_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.event_tip_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(status.Tip), mCharId, characterLv, liberationLv,
                    PercentString, description, mStatus.id, PracticeSkillType.EventTipEnhanceRate,
                    PracticeSkillIconType.StatusUp, CharacterStatusType.Speed));
            }

            // ステータス
            if (mStatus.practiceParamEnhanceRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.practiceParamEnhanceRate), mCharId,
                    characterLv, liberationLv, PercentString, description, mStatus.id,
                    PracticeSkillType.PracticeParamEnhanceRate, PracticeSkillIconType.PracticeParamEnhanceRate,
                    CharacterStatusType.Speed));
            }

            // ステータス
            if (mStatus.battleParamEnhanceRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.battleParamEnhanceRate), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id, PracticeSkillType.BattleParamEnhanceRate,
                    PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // コンディション
            if (mStatus.conditionDiscountPractice > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_practice.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_practice.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, mStatus.conditionDiscountPractice, mCharId, characterLv,
                    liberationLv, string.Empty, description, mStatus.id,
                    PracticeSkillType.ConditionDiscountPractice, PracticeSkillIconType.ConditionDiscount,
                    CharacterStatusType.Speed));
            }

            // 合成コンディション
            if (mStatus.conditionDiscountFusion > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_dusion.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_dusion.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, mStatus.conditionDiscountFusion, mCharId, characterLv,
                    liberationLv, string.Empty, description, mStatus.id, PracticeSkillType.ConditionDiscountFusion,
                    PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            // レア出現率
            if (mStatus.rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.rarePracticeEnhanceRate), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id,
                    PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate,
                    CharacterStatusType.Speed));
            }

            // スキルレベル
            if (mStatus.skillLevelUp > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.skill_levelup.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.skill_levelup.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, mStatus.skillLevelUp, mCharId, characterLv, liberationLv,
                    string.Empty, description, mStatus.id, PracticeSkillType.SkillLevelUp,
                    PracticeSkillIconType.SkillLevelUp, CharacterStatusType.Speed));
            }

            // スキルレベル
            if (mStatus.practiceLevelEnhance > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.practice_level_enhance.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.practice_level_enhance.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, mStatus.practiceLevelEnhance, mCharId, characterLv, liberationLv,
                    string.Empty, description, mStatus.id, PracticeSkillType.PracticeLevelEnhance,
                    PracticeSkillIconType.PracticeLevelEnhance, CharacterStatusType.Speed));
            }

            // レクチャー出現率
            if (mStatus.popRateUp > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_up.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.pop_rate_up.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.popRateUp), mCharId, characterLv, liberationLv,
                    PercentString, description, mStatus.id, PracticeSkillType.PopRateUp,
                    PracticeSkillIconType.PopRateUp, CharacterStatusType.Speed));
            }

            // レクチャー出現率
            if (mStatus.specialRateUp > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.special_rate_up.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.special_rate_up.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.specialRateUp), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id, PracticeSkillType.SpecialRateUp,
                    PracticeSkillIconType.SpecialRateUp, CharacterStatusType.Speed));
            }


            // コンディション回復
            if (mStatus.conditionRecoverUp > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_recover_up.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.condition_recover_up.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, mStatus.conditionRecoverUp, mCharId, characterLv, liberationLv,
                    string.Empty, description, mStatus.id, PracticeSkillType.ConditionRecoverUp,
                    PracticeSkillIconType.ConditionRecoverUp, CharacterStatusType.Speed));
            }

            // イベントステータス
            if (mStatus.eventStatusEnhanceRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.event_status_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.event_status_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.eventStatusEnhanceRate), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id, PracticeSkillType.EventStatusEnhanceRate,
                    PracticeSkillIconType.StatusUp, CharacterStatusType.Speed));
            }

            // 練習メニュー経験値獲得
            if (mStatus.practiceExpEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.exp_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.exp_enhance_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.practiceExpEnhanceRate), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id, PracticeSkillType.PracticeExpEnhanceRate,
                    PracticeSkillIconType.StatusUp, CharacterStatusType.Speed));
            }

            // 練習時発生ボーナス%上昇
            CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType =
                ConditionEffectGradeUpMapOnTypeParse(mStatus.conditionEffectGradeUpMapOnType);
            GetPracticeSkillData(conditionEffectGradeUpMapOnType, mCharId, characterLv, liberationLv, mStatus.id, result);

            // トレーニング固定ボーナス
            status = StatusUtility.Parse(mStatus.practiceParamAddBonusMap);
            GetPracticeSkillData(status, false, mCharId, characterLv, liberationLv,
                StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"],
                StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"],
                mStatus.id, PracticeSkillType.PracticeParamAddBonusMap, PracticeSkillIconType.StatusUp, result);

            // 練習時特定ステータス獲得量%アップ
            CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType = PracticeParamEnhanceMapOnTypeParse(mStatus.practiceParamEnhanceMapOnType);

            GetPracticeSkillData(practiceParamEnhanceMapOnType, mCharId, characterLv, liberationLv, mStatus.id, result);

            // 特定スペシャルトレーニング出現率アップ
            CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType = RarePracticeEnhanceRateMapOnTypeParse(mStatus.rarePracticeEnhanceRateMapOnType);
            GetPracticeSkillData(rarePracticeEnhanceRateMapOnType, mCharId, characterLv, liberationLv, mStatus.id, result);



            // 特定スペシャルレクチャー確率アップ
            CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType = PopRateEnhanceMapOnTypeParse(mStatus.popRateEnhanceMapOnType);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(popRateEnhanceMapOnType, mCharId, characterLv, liberationLv, mStatus.id, result);


            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            long[] firstRewardIdList = FirstRewardIdListParse(mStatus.firstMTrainingEventRewardIdList);
            GetPracticeSkillData(firstRewardIdList, mCharId, characterLv, liberationLv, mStatus.id, result);

            // エクストラボーナス
            if (mStatus.conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.conditionEffectGradeUpRate), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id,
                    PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate,
                    CharacterStatusType.Speed));
            }

            // 常時特定ステータスアップ
            status = StatusUtility.Parse(mStatus.practiceParamRateMap);
            GetPracticeSkillData(status, true, mCharId, characterLv, liberationLv,
                StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"],
                StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], mStatus.id,
                PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, result);

            // コンディション割合軽減
            if (mStatus.conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.conditionDiscountRate), mCharId, characterLv,
                    liberationLv, PercentString, description, mStatus.id,
                    PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount,
                    CharacterStatusType.Speed));
            }
        }

        /// <summary>練習能力の取得</summary>
        private static void GetPracticeSkillData(CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType, long mCharId, long characterLv, long liberationLv, long masterId, List<PracticeSkillData> result)
        {
            if(conditionEffectGradeUpMapOnType == null) return;
            
            //conditionEffectGradeUpMapOnTypeの配列をもとに練習能力を設定
            foreach (var conditionEffectGradeUpMap in conditionEffectGradeUpMapOnType)
            {
                // データ追加
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_map_on_type.name"],
                        GetPracticeTypeString((PracticeType)conditionEffectGradeUpMap.practiceType),
                        conditionEffectGradeUpMap.grade);
                string description = string.Format(
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_map_on_type.description"],
                    GetPracticeTypeString((PracticeType)conditionEffectGradeUpMap.practiceType),
                    conditionEffectGradeUpMap.grade);
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(conditionEffectGradeUpMap.rate), mCharId, characterLv,
                    liberationLv, PercentString, description, masterId, PracticeSkillType.ConditionEffectGradeUpMapOnType,
                    PracticeSkillIconType.SkillLevelUp, CharacterStatusType.Speed, (PracticeType)conditionEffectGradeUpMap.practiceType));
            }
        }
        
        private static void GetPracticeSkillData(CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType, long mCharId, long characterLv, long liberationLv, long masterId, List<PracticeSkillData> result)
        {
            if(practiceParamEnhanceMapOnType == null) return;
            
            //practiceParamEnhanceMapOnTypeの配列をもとに練習能力を設定
            foreach (var practiceParamEnhanceMap in practiceParamEnhanceMapOnType)
            {
                CharacterStatus status = StatusUtility.Parse(practiceParamEnhanceMap.rateMap);
                // 各ステータスをチェック
                foreach (CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
                {
                    // 値を取得
                    long value = status[type];
                    // 上昇値なし
                    if(value <= 0)continue;
                    
                    // データ追加
                    string name =
                        string.Format(
                            StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.name"],
                            GetPracticeTypeString((PracticeType)practiceParamEnhanceMap.practiceType),
                            StatusUtility.GetStatusName(type));
                    string description = string.Format(
                        StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.description"],
                        GetPracticeTypeString((PracticeType)practiceParamEnhanceMap.practiceType),
                        StatusUtility.GetStatusName(type));
                    // リストに追加
                    result.Add(new PracticeSkillData(name, ToPercent(value), mCharId, characterLv,
                        liberationLv, PercentString, description, masterId, PracticeSkillType.PracticeParamEnhanceMapOnType,
                        PracticeSkillIconType.StatusUp, type, (PracticeType)practiceParamEnhanceMap.practiceType));
                }
                
            }
        }
        
        private static void GetPracticeSkillData(CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType, long mCharId, long characterLv, long liberationLv, long masterId, List<PracticeSkillData> result)
        {
            if(rarePracticeEnhanceRateMapOnType == null) return;
            
            //rarePracticeEnhanceRateMapOnTypeの配列をもとに練習能力を設定
            foreach (var rarePracticeEnhanceRateMap in rarePracticeEnhanceRateMapOnType)
            {
                // データ追加
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.name"],
                        GetPracticeTypeString((PracticeType)rarePracticeEnhanceRateMap.practiceType));
                string description = string.Format(
                    StringValueAssetLoader.Instance
                        ["practice_skill.rare_practice_enhance_rate_map_on_type.description"],
                    GetPracticeTypeString((PracticeType)rarePracticeEnhanceRateMap.practiceType));
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRateMap.rate), mCharId, characterLv,
                    liberationLv, PercentString, description, masterId, PracticeSkillType.RarePracticeEnhanceRateMapOnType,
                    PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed, PracticeType.None));
            }
        }
        
        private static void GetPracticeSkillData(CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType, long mCharId, long characterLv, long liberationLv, long masterId, List<PracticeSkillData> result)
        {
            if(popRateEnhanceMapOnType == null) return;

            //popRateEnhanceMapOnTypeの配列をもとに練習能力を設定
            foreach (var popRateEnhanceMap in popRateEnhanceMapOnType)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.name"],
                        GetPracticeTypeString((PracticeType)popRateEnhanceMap.practiceType));
                string description =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.description"],
                        GetPracticeTypeString((PracticeType)popRateEnhanceMap.practiceType));
                // リストに追加
                result.Add(new PracticeSkillData(name, ToPercent(popRateEnhanceMap.rate), mCharId, characterLv,
                    liberationLv, PercentString, description, masterId, PracticeSkillType.PopRateEnhanceMapOnType,
                    PracticeSkillIconType.PopRateUp, CharacterStatusType.Speed, PracticeType.None));
            }
        }
        
        private static void GetPracticeSkillData(long[] firstRewardIdList, long mCharId, long characterLv, long liberationLv, long masterId, List<PracticeSkillData> result)
        {
            if(firstRewardIdList == null) return;

            //popRateEnhanceMapOnTypeの配列をもとに練習能力を設定
            foreach (var firstRewardId in firstRewardIdList)
            {
                var mTrainingEventReward = MasterManager.Instance.trainingEventRewardMaster.FindData(firstRewardId);
                if (mTrainingEventReward == null)
                {
                    CruFramework.Logger.LogError($"mTrainingEventRewardが取得できせんでした。　mTrainingEventRewardId : {firstRewardId}");
                    continue;
                }
                // データ追加
                string name = mTrainingEventReward.name;
                string description = string.Empty;
                // 「食堂」の文字列が含まれているか
                if (name.Contains(StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria"]))
                {
                    description = StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria.description"];
                }
                // 「就寝」の文字列が含まれているか
                if (name.Contains(StringValueAssetLoader.Instance["practice_skill.first_reward.sleeping"]))
                {
                    description = StringValueAssetLoader.Instance["practice_skill.first_reward.sleeping.description"];
                }
                // リストに追加
                // サポート器具はliberationLvがないため-1を渡しておく
                result.Add(new PracticeSkillData(name, ToPercent(mTrainingEventReward.displayNumber), mCharId, characterLv, liberationLv, PercentString, description,
                    masterId, PracticeSkillType.FirstReward,
                    PracticeSkillIconType.ConditionRecoverUp, CharacterStatusType.Speed));
            }
        }

        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetTotalPracticeSkillList(long[] mComboBuffIds)
        {
            List<PracticeSkillData> skillList = new List<PracticeSkillData>();

            // 初期ステータス
            CharacterStatus status = new CharacterStatus();

            int practiceTypeEnumCount = Enum.GetValues(typeof(PracticeType)).Length;
            
            long battleParamEnhanceRate = 0;
            long rarePracticeEnhanceRate = 0;
            Dictionary<PracticeType, Dictionary<long, long>> conditionEffectGradeUpMapOnTypeValues =
                new Dictionary<PracticeType, Dictionary<long, long>>();
            Dictionary<CharacterStatusType, long> practiceParamAddBonusMapValues = new Dictionary<CharacterStatusType, long>();
            Dictionary<PracticeType, Dictionary<CharacterStatusType, long>> practiceParamEnhanceMapOnTypeValues = new Dictionary<PracticeType, Dictionary<CharacterStatusType, long>>();
            long[] rarePracticeEnhanceRateMapOnTypeRates = new long[Enum.GetValues(typeof(PracticeType)).Length];
            long[] popRateEnhanceMapOnTypeRates = new long[Enum.GetValues(typeof(PracticeType)).Length];
            Dictionary<string, long> firstTrainingEventRewardId = new Dictionary<string, long>();
            long conditionEffectGradeUpRate = 0;
            Dictionary<CharacterStatusType, long> practiceParamRateMapValues = new Dictionary<CharacterStatusType, long>();
            long conditionDiscountRate = 0;

            foreach (long id in mComboBuffIds)
            {
                CharaTrainingComboBuffStatusMasterObject mComboBuff = MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(id);

                status += StatusUtility.Parse(mComboBuff.firstParamAddMap);
                battleParamEnhanceRate += mComboBuff.battleParamEnhanceRate;
                rarePracticeEnhanceRate += mComboBuff.rarePracticeEnhanceRate;
                CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnTypes = ConditionEffectGradeUpMapOnTypeParse(mComboBuff.conditionEffectGradeUpMapOnType);
                if (conditionEffectGradeUpMapOnTypes is not null)
                {
                    foreach (var conditionEffectGradeUpMapOnType in conditionEffectGradeUpMapOnTypes)
                    {
                        if (conditionEffectGradeUpMapOnType.practiceType == (long)PracticeType.None) continue;
                        // PracticeTypeが含まれていなければなければ作る
                        if (!conditionEffectGradeUpMapOnTypeValues.ContainsKey((PracticeType)conditionEffectGradeUpMapOnType.practiceType))
                        {
                            conditionEffectGradeUpMapOnTypeValues.Add((PracticeType)conditionEffectGradeUpMapOnType.practiceType, new Dictionary<long, long>());
                        }
                        // gradeがなければ作る
                        if (conditionEffectGradeUpMapOnTypeValues[(PracticeType)conditionEffectGradeUpMapOnType.practiceType].ContainsKey(conditionEffectGradeUpMapOnType.grade))
                        {
                            conditionEffectGradeUpMapOnTypeValues[(PracticeType)conditionEffectGradeUpMapOnType.practiceType][
                                conditionEffectGradeUpMapOnType.grade] += conditionEffectGradeUpMapOnType.rate;
                        }
                        else
                        {
                            conditionEffectGradeUpMapOnTypeValues[(PracticeType)conditionEffectGradeUpMapOnType.practiceType].Add(conditionEffectGradeUpMapOnType.grade, conditionEffectGradeUpMapOnType.rate);
                        }
                    }
                }
                CharacterStatus practiceParamAddBonusMap = StatusUtility.Parse(mComboBuff.practiceParamAddBonusMap);
                practiceParamAddBonusMapValues = GetTotalPracticeSkillList(practiceParamAddBonusMap, practiceParamAddBonusMapValues);
                CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnTypes = PracticeParamEnhanceMapOnTypeParse(mComboBuff.practiceParamEnhanceMapOnType);
                if (practiceParamEnhanceMapOnTypes is not null)
                {
                    foreach (var practiceParamEnhanceMapOnType in practiceParamEnhanceMapOnTypes)
                    {
                        if (!practiceParamEnhanceMapOnTypeValues.ContainsKey((PracticeType)practiceParamEnhanceMapOnType.practiceType))
                        {
                            practiceParamEnhanceMapOnTypeValues.Add((PracticeType)practiceParamEnhanceMapOnType.practiceType, new Dictionary<CharacterStatusType, long>());
                        }
                        practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType] = GetTotalPracticeSkillList(StatusUtility.Parse(practiceParamEnhanceMapOnType.rateMap), practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType]);
                    }
                }
                CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnTypes = RarePracticeEnhanceRateMapOnTypeParse(mComboBuff.rarePracticeEnhanceRateMapOnType);
                if (rarePracticeEnhanceRateMapOnTypes is not null)
                {
                    foreach (var rarePracticeEnhanceRateMapOnType in rarePracticeEnhanceRateMapOnTypes)
                    {
                        if (rarePracticeEnhanceRateMapOnType.practiceType == (long)PracticeType.None) continue;
                        rarePracticeEnhanceRateMapOnTypeRates[rarePracticeEnhanceRateMapOnType.practiceType] += rarePracticeEnhanceRateMapOnType.rate;
                    }
                }
                CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnTypes = PopRateEnhanceMapOnTypeParse(mComboBuff.popRateEnhanceMapOnType);
                if (popRateEnhanceMapOnTypes is not null)
                {
                    foreach (var popRateEnhanceMapOnType in popRateEnhanceMapOnTypes)
                    {
                        if (popRateEnhanceMapOnType.practiceType == (long)PracticeType.None) continue;
                        popRateEnhanceMapOnTypeRates[popRateEnhanceMapOnType.practiceType] += popRateEnhanceMapOnType.rate;
                    }
                }
                long[] firstTrainingEventRewardIdLists = FirstRewardIdListParse(mComboBuff.firstMTrainingEventRewardIdList);
                if (firstTrainingEventRewardIdLists is not null)
                {
                    foreach (var firstMTrainingEventRewardIdList in firstTrainingEventRewardIdLists)
                    {
                        var firstMTrainingEventRewardId = MasterManager.Instance.trainingEventRewardMaster.FindData(firstMTrainingEventRewardIdList);
                        string name = firstMTrainingEventRewardId.name;
                        // 「食堂」か「就寝」の文字列が含まれているか
                        if (name.Contains(StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria"]) || name.Contains(StringValueAssetLoader.Instance["practice_skill.first_reward.sleeping"]))
                        {
                            if (!firstTrainingEventRewardId.ContainsKey(name))
                            {
                                firstTrainingEventRewardId.Add(name, firstMTrainingEventRewardId.displayNumber);
                            }
                            else
                            {
                                firstTrainingEventRewardId[name] += firstMTrainingEventRewardId.displayNumber;
                            }
                        }
                    }
                }
                conditionEffectGradeUpRate += mComboBuff.conditionEffectGradeUpRate;
                practiceParamRateMapValues = GetTotalPracticeSkillList(StatusUtility.Parse(mComboBuff.practiceParamRateMap), practiceParamRateMapValues);
                conditionDiscountRate += mComboBuff.conditionDiscountRate;
            }

            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.first_param_up.name"], StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], -1, PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, skillList);


            // ステータス
            if (battleParamEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(battleParamEnhanceRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.BattleParamEnhanceRate, PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // レア出現率
            if (rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 練習時発生ボーナス%上昇
            foreach (var conditionEffectGradeUpMapOnType in conditionEffectGradeUpMapOnTypeValues)
            {
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_map_on_type.name"],
                        GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key));
                foreach (var conditionEffectGradeUpMapOnTypeGradeAndRate in conditionEffectGradeUpMapOnType.Value)
                {
                    string description =
                        string.Format(
                            StringValueAssetLoader.Instance[
                                "practice_skill.condition_effect_grade_up_map_on_type.description"],
                            GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key), conditionEffectGradeUpMapOnTypeGradeAndRate.Key);
                    skillList.Add(new PracticeSkillData(name, ToPercent(conditionEffectGradeUpMapOnTypeGradeAndRate.Value), -1, -1, -1, PercentString,
                        description, -1, PracticeSkillType.ConditionEffectGradeUpMapOnType,
                        PracticeSkillIconType.SkillLevelUp, CharacterStatusType.Speed,
                        conditionEffectGradeUpMapOnType.Key));
                }
            }

            // トレーニング固定ボーナス
            foreach (var practiceParamAddBonusMapValue in practiceParamAddBonusMapValues)
            {
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"],
                        StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                string description =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"],
                        StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, practiceParamAddBonusMapValue.Value, -1, -1, -1,
                    string.Empty, description, -1, PracticeSkillType.PracticeParamAddBonusMap,
                    PracticeSkillIconType.StatusUp, practiceParamAddBonusMapValue.Key));
            }

            // 練習時特定ステータス獲得量%アップ
            foreach (var practiceParamEnhanceMapOnTypeValue in practiceParamEnhanceMapOnTypeValues)
            {
                foreach (var practiceParamEnhanceMapOnTypeStatusAndValue in practiceParamEnhanceMapOnTypeValue.Value)
                {
                    // データ追加
                    string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.name"], GetPracticeTypeString(practiceParamEnhanceMapOnTypeValue.Key), StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)practiceParamEnhanceMapOnTypeValue.Key), StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    // リストに追加
                    skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamEnhanceMapOnTypeStatusAndValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamEnhanceMapOnType, PracticeSkillIconType.StatusUp, practiceParamEnhanceMapOnTypeStatusAndValue.Key, (PracticeType)practiceParamEnhanceMapOnTypeValue.Key));
                }
            }

            // 特定スペシャルトレーニング出現率アップ
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (rarePracticeEnhanceRateMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRateMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.RarePracticeEnhanceRateMapOnType, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed, PracticeType.None));
            }

            // 特定スペシャルレクチャー確率アップ
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (popRateEnhanceMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(popRateEnhanceMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PopRateEnhanceMapOnType, PracticeSkillIconType.PopRateUp, CharacterStatusType.Speed, PracticeType.None));
            }

            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            // リストに追加
            if (firstTrainingEventRewardId is not null)
            {
                foreach (var firstTrainingEventRewardIdDisplayNumber in firstTrainingEventRewardId)
                {
                    skillList.Add(new PracticeSkillData(firstTrainingEventRewardIdDisplayNumber.Key, ToPercent(firstTrainingEventRewardIdDisplayNumber.Value), -1, -1, -1, PercentString, StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria.description"], -1, PracticeSkillType.FirstReward, PracticeSkillIconType.ConditionRecoverUp, CharacterStatusType.Speed));
                }
            }

            // エクストラボーナス
            if (conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(conditionEffectGradeUpRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 常時特定ステータスアップ
            foreach (var practiceParamRateMapValue in practiceParamRateMapValues)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamRateMapValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, practiceParamRateMapValue.Key));
            }

            // コンディション割合軽減
            if (conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(conditionDiscountRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            return skillList;
        }

        /// <summary>練習能力の合計値</summary>
        public static Dictionary<CharacterStatusType, long> GetTotalPracticeSkillList(CharacterStatus status, Dictionary<CharacterStatusType, long> values)
        {
            foreach (CharacterStatusType type in System.Enum.GetValues(typeof(CharacterStatusType)))
            {
                // 値を取得
                long value = status[type];
                // 上昇値なし
                if (value > 0)
                {
                    if (values.ContainsKey(type))
                    {
                        values[type] += value;
                    }
                    else
                    {
                        values.Add(type, value);
                    }
                }
            }
            return values;
        }

        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetPracticeSkillList(CharaTrainingComboBuffStatusMasterObject mComboBuffStatus)
        {

            var skillList = new List<PracticeSkillData>();
            if (mComboBuffStatus == null) return skillList;
            // 初期ステータス
            CharacterStatus status = StatusUtility.Parse(mComboBuffStatus.firstParamAddMap);
            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.first_param_up.name"], StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], mComboBuffStatus.id, PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, skillList);


            // ステータス
            if (mComboBuffStatus.battleParamEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mComboBuffStatus.battleParamEnhanceRate), -1, -1, -1, PercentString, description, mComboBuffStatus.id, PracticeSkillType.BattleParamEnhanceRate, PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // レア出現率
            if (mComboBuffStatus.rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mComboBuffStatus.rarePracticeEnhanceRate), -1, -1, -1, PercentString, description, mComboBuffStatus.id, PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 練習時発生ボーナス%上昇
            CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType =
                ConditionEffectGradeUpMapOnTypeParse(mComboBuffStatus.conditionEffectGradeUpMapOnType);
            GetPracticeSkillData(conditionEffectGradeUpMapOnType, -1, -1, -1, mComboBuffStatus.id, skillList);

            // トレーニング固定ボーナス
            status = StatusUtility.Parse(mComboBuffStatus.practiceParamAddBonusMap);
            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"], StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"], mComboBuffStatus.id, PracticeSkillType.PracticeParamAddBonusMap, PracticeSkillIconType.StatusUp, skillList);

            // 練習時特定ステータス獲得量%アップ
            CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType = PracticeParamEnhanceMapOnTypeParse(mComboBuffStatus.practiceParamEnhanceMapOnType);

            GetPracticeSkillData(practiceParamEnhanceMapOnType, -1, -1, -1, mComboBuffStatus.id, skillList);

            // 特定スペシャルトレーニング出現率アップ
            CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType = RarePracticeEnhanceRateMapOnTypeParse(mComboBuffStatus.rarePracticeEnhanceRateMapOnType);
            GetPracticeSkillData(rarePracticeEnhanceRateMapOnType, -1, -1, -1, mComboBuffStatus.id, skillList);

            // 特定スペシャルレクチャー確率アップ
            CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType = PopRateEnhanceMapOnTypeParse(mComboBuffStatus.popRateEnhanceMapOnType);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(popRateEnhanceMapOnType, -1, -1, -1, mComboBuffStatus.id, skillList);


            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            long[] firstRewardIdList = FirstRewardIdListParse(mComboBuffStatus.firstMTrainingEventRewardIdList);
            GetPracticeSkillData(firstRewardIdList, -1, -1, -1, mComboBuffStatus.id, skillList);

            // エクストラボーナス
            if (mComboBuffStatus.conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mComboBuffStatus.conditionEffectGradeUpRate), -1, -1, -1, PercentString, description, mComboBuffStatus.id, PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 常時特定ステータスアップ
            status = StatusUtility.Parse(mComboBuffStatus.practiceParamRateMap);
            GetPracticeSkillData(status, true, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"], StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], mComboBuffStatus.id, PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, skillList);

            // コンディション割合軽減
            if (mComboBuffStatus.conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mComboBuffStatus.conditionDiscountRate), -1, -1, -1, PercentString, description, mComboBuffStatus.id, PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            return skillList;
        }

        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetTotalPracticeSkillList(
            CombinationStatusTrainingBase statusTrainingBase)
        {
            List<PracticeSkillData> skillList = new List<PracticeSkillData>();
            CharacterStatus status = StatusUtility.Parse(statusTrainingBase.firstParamAddMap);
            long battleParamEnhanceRate = statusTrainingBase.battleParamEnhanceRate;
            long rarePracticeEnhanceRate = statusTrainingBase.rarePracticeEnhanceRate;

            int practiceTypeEnumCount = Enum.GetValues(typeof(PracticeType)).Length;
            Dictionary<PracticeType, Dictionary<long, long>> conditionEffectGradeUpMapOnTypeValues =
                new Dictionary<PracticeType, Dictionary<long, long>>();
            Dictionary<CharacterStatusType, long> practiceParamAddBonusMapValues =
                new Dictionary<CharacterStatusType, long>();
            Dictionary<PracticeType, Dictionary<CharacterStatusType, long>> practiceParamEnhanceMapOnTypeValues =
                new Dictionary<PracticeType, Dictionary<CharacterStatusType, long>>();
            long[] rarePracticeEnhanceRateMapOnTypeRates = new long[Enum.GetValues(typeof(PracticeType)).Length];
            long[] popRateEnhanceMapOnTypeRates = new long[Enum.GetValues(typeof(PracticeType)).Length];
            Dictionary<string, long> firstTrainingEventRewardId = new Dictionary<string, long>();
            Dictionary<CharacterStatusType, long> practiceParamRateMapValues =
                new Dictionary<CharacterStatusType, long>();

            GetPracticeSkillData(status, false, -1, -1, -1,
                StringValueAssetLoader.Instance["practice_skill.first_param_up.name"],
                StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], -1,
                PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, skillList);

            // ステータス
            if (battleParamEnhanceRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(battleParamEnhanceRate), -1, -1, -1, PercentString,
                    description, -1, PracticeSkillType.BattleParamEnhanceRate,
                    PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // レア出現率
            if (rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRate), -1, -1, -1, PercentString,
                    description, -1, PracticeSkillType.RarePracticeEnhanceRate,
                    PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 練習時発生ボーナス%上昇
            foreach (var conditionEffectGradeUpMapOnType in statusTrainingBase.conditionEffectGradeUpMapOnType)
            {
                if (conditionEffectGradeUpMapOnType.practiceType == (long)PracticeType.None) continue;
                // PracticeTypeが含まれていなければなければ作る
                if (!conditionEffectGradeUpMapOnTypeValues.ContainsKey(
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType))
                {
                    conditionEffectGradeUpMapOnTypeValues.Add(
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType, new Dictionary<long, long>());
                }

                // gradeがなければ作る
                if (conditionEffectGradeUpMapOnTypeValues[(PracticeType)conditionEffectGradeUpMapOnType.practiceType]
                    .ContainsKey(conditionEffectGradeUpMapOnType.grade))
                {
                    conditionEffectGradeUpMapOnTypeValues[(PracticeType)conditionEffectGradeUpMapOnType.practiceType][
                        conditionEffectGradeUpMapOnType.grade] += conditionEffectGradeUpMapOnType.rate;
                }
                else
                {
                    conditionEffectGradeUpMapOnTypeValues[(PracticeType)conditionEffectGradeUpMapOnType.practiceType]
                        .Add(conditionEffectGradeUpMapOnType.grade, conditionEffectGradeUpMapOnType.rate);
                }
            }

            foreach (var conditionEffectGradeUpMapOnType in conditionEffectGradeUpMapOnTypeValues)
            {
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_map_on_type.name"],
                        GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key));
                foreach (var conditionEffectGradeUpMapOnTypeGradeAndRate in conditionEffectGradeUpMapOnType.Value)
                {
                    string description =
                        string.Format(
                            StringValueAssetLoader.Instance[
                                "practice_skill.condition_effect_grade_up_map_on_type.description"],
                            GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key),
                            conditionEffectGradeUpMapOnTypeGradeAndRate.Key);
                    skillList.Add(new PracticeSkillData(name,
                        ToPercent(conditionEffectGradeUpMapOnTypeGradeAndRate.Value), -1, -1, -1, PercentString,
                        description, -1, PracticeSkillType.ConditionEffectGradeUpMapOnType,
                        PracticeSkillIconType.SkillLevelUp, CharacterStatusType.Speed,
                        conditionEffectGradeUpMapOnType.Key));
                }
            }

            // トレーニング固定ボーナス
            CharacterStatus practiceParamAddBonusMap = StatusUtility.Parse(statusTrainingBase.practiceParamAddBonusMap);
            practiceParamAddBonusMapValues =
                GetTotalPracticeSkillList(practiceParamAddBonusMap, practiceParamAddBonusMapValues);
            foreach (var practiceParamAddBonusMapValue in practiceParamAddBonusMapValues)
            {
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"],
                        StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                string description =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"],
                        StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, practiceParamAddBonusMapValue.Value, -1, -1, -1,
                    string.Empty, description, -1, PracticeSkillType.PracticeParamAddBonusMap,
                    PracticeSkillIconType.StatusUp, practiceParamAddBonusMapValue.Key));
            }

            // 練習時特定ステータス獲得量%アップ
            foreach (var practiceParamEnhanceMapOnType in statusTrainingBase.practiceParamEnhanceMapOnType)
            {
                if (!practiceParamEnhanceMapOnTypeValues.ContainsKey(
                        (PracticeType)practiceParamEnhanceMapOnType.practiceType))
                {
                    practiceParamEnhanceMapOnTypeValues.Add((PracticeType)practiceParamEnhanceMapOnType.practiceType,
                        new Dictionary<CharacterStatusType, long>());
                }

                practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType] =
                    GetTotalPracticeSkillList(StatusUtility.Parse(practiceParamEnhanceMapOnType.rateMap),
                        practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType]);
            }

            foreach (var practiceParamEnhanceMapOnTypeValue in practiceParamEnhanceMapOnTypeValues)
            {
                foreach (var practiceParamEnhanceMapOnTypeStatusAndValue in practiceParamEnhanceMapOnTypeValue.Value)
                {
                    // データ追加
                    string name = string.Format(
                        StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.name"],
                        GetPracticeTypeString(practiceParamEnhanceMapOnTypeValue.Key),
                        StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    string description = string.Format(
                        StringValueAssetLoader.Instance
                            ["practice_skill.practice_param_enhance_map_on_type.description"],
                        GetPracticeTypeString((PracticeType)practiceParamEnhanceMapOnTypeValue.Key),
                        StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    // リストに追加
                    skillList.Add(new PracticeSkillData(name,
                        ToPercent(practiceParamEnhanceMapOnTypeStatusAndValue.Value), -1, -1, -1, PercentString,
                        description, -1, PracticeSkillType.PracticeParamEnhanceMapOnType,
                        PracticeSkillIconType.StatusUp, practiceParamEnhanceMapOnTypeStatusAndValue.Key,
                        (PracticeType)practiceParamEnhanceMapOnTypeValue.Key));
                }
            }

            // 特定スペシャルトレーニング出現率アップ
            foreach (var rarePracticeEnhanceRateMapOnType in statusTrainingBase.rarePracticeEnhanceRateMapOnType)
            {
                if (rarePracticeEnhanceRateMapOnType.practiceType == (long)PracticeType.None) continue;
                rarePracticeEnhanceRateMapOnTypeRates[rarePracticeEnhanceRateMapOnType.practiceType] +=
                    rarePracticeEnhanceRateMapOnType.rate;
            }

            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (rarePracticeEnhanceRateMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.name"],
                        GetPracticeTypeString((PracticeType)type));
                string description =
                    string.Format(
                        StringValueAssetLoader.Instance[
                            "practice_skill.rare_practice_enhance_rate_map_on_type.description"],
                        GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRateMapOnTypeRates[type]), -1,
                    -1, -1, PercentString, description, -1, PracticeSkillType.RarePracticeEnhanceRateMapOnType,
                    PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed, PracticeType.None));
            }

            // 特定スペシャルレクチャー確率アップ
            foreach (var popRateEnhanceMapOnType in statusTrainingBase.popRateEnhanceMapOnType)
            {
                if (popRateEnhanceMapOnType.practiceType == (long)PracticeType.None) continue;
                popRateEnhanceMapOnTypeRates[popRateEnhanceMapOnType.practiceType] += popRateEnhanceMapOnType.rate;
            }
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (popRateEnhanceMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(popRateEnhanceMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PopRateEnhanceMapOnType, PracticeSkillIconType.PopRateUp, CharacterStatusType.Speed, PracticeType.None));
            }

            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            foreach (var firstMTrainingEventRewardIdList in statusTrainingBase.firstMTrainingEventRewardIdList)
            {
                var firstMTrainingEventRewardId =
                    MasterManager.Instance.trainingEventRewardMaster.FindData(firstMTrainingEventRewardIdList);
                string name = firstMTrainingEventRewardId.name;
                // 「食堂」か「就寝」の文字列が含まれているか
                if (name.Contains(StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria"]) ||
                    name.Contains(StringValueAssetLoader.Instance["practice_skill.first_reward.sleeping"]))
                {
                    if (!firstTrainingEventRewardId.ContainsKey(name))
                    {
                        firstTrainingEventRewardId.Add(name, firstMTrainingEventRewardId.displayNumber);
                    }
                    else
                    {
                        firstTrainingEventRewardId[name] += firstMTrainingEventRewardId.displayNumber;
                    }
                }
            }
            // リストに追加
            foreach (var firstTrainingEventRewardIdDisplayNumber in firstTrainingEventRewardId)
            {
                skillList.Add(new PracticeSkillData(firstTrainingEventRewardIdDisplayNumber.Key,
                    ToPercent(firstTrainingEventRewardIdDisplayNumber.Value), -1, -1, -1, PercentString,
                    StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria.description"], -1,
                    PracticeSkillType.FirstReward, PracticeSkillIconType.ConditionRecoverUp,
                    CharacterStatusType.Speed));
            }

            // エクストラボーナス
            if (statusTrainingBase.conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(statusTrainingBase.conditionEffectGradeUpRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }
            
            // 常時特定ステータスアップ
            practiceParamRateMapValues = GetTotalPracticeSkillList(StatusUtility.Parse(statusTrainingBase.practiceParamRateMap), practiceParamRateMapValues);
            foreach (var practiceParamRateMapValue in practiceParamRateMapValues)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamRateMapValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, practiceParamRateMapValue.Key));
            }

            // コンディション割合軽減
            if (statusTrainingBase.conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(statusTrainingBase.conditionDiscountRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            return skillList;
        }

        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetPracticeSkillList(CombinationTrainingStatusMasterObject mCombinationTrainingStatus)
        {

            var skillList = new List<PracticeSkillData>();
            if (mCombinationTrainingStatus == null) return null;
            // 初期ステータス
            CharacterStatus status = StatusUtility.Parse(mCombinationTrainingStatus.firstParamAddMap);
            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.first_param_up.name"], StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], mCombinationTrainingStatus.id, PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, skillList);

            // ステータス
            if (mCombinationTrainingStatus.battleParamEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mCombinationTrainingStatus.battleParamEnhanceRate), -1, -1, -1, PercentString, description, mCombinationTrainingStatus.id, PracticeSkillType.BattleParamEnhanceRate, PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // レア出現率
            if (mCombinationTrainingStatus.rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mCombinationTrainingStatus.rarePracticeEnhanceRate), -1, -1, -1, PercentString, description, mCombinationTrainingStatus.id, PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 練習時発生ボーナス%上昇
            CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType =
                ConditionEffectGradeUpMapOnTypeParse(mCombinationTrainingStatus.conditionEffectGradeUpMapOnType);
            GetPracticeSkillData(conditionEffectGradeUpMapOnType, -1, -1, -1, mCombinationTrainingStatus.id, skillList);

            // トレーニング固定ボーナス
            status = StatusUtility.Parse(mCombinationTrainingStatus.practiceParamAddBonusMap);
            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"], StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"], mCombinationTrainingStatus.id, PracticeSkillType.PracticeParamAddBonusMap, PracticeSkillIconType.StatusUp, skillList);

            // 練習時特定ステータス獲得量%アップ
            CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType = PracticeParamEnhanceMapOnTypeParse(mCombinationTrainingStatus.practiceParamEnhanceMapOnType);

            GetPracticeSkillData(practiceParamEnhanceMapOnType, -1, -1, -1, mCombinationTrainingStatus.id, skillList);

            // 特定スペシャルトレーニング出現率アップ
            CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType = RarePracticeEnhanceRateMapOnTypeParse(mCombinationTrainingStatus.rarePracticeEnhanceRateMapOnType);
            GetPracticeSkillData(rarePracticeEnhanceRateMapOnType, -1, -1, -1, mCombinationTrainingStatus.id, skillList);

            // 特定スペシャルレクチャー確率アップ
            CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType = PopRateEnhanceMapOnTypeParse(mCombinationTrainingStatus.popRateEnhanceMapOnType);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(popRateEnhanceMapOnType, -1, -1, -1, mCombinationTrainingStatus.id, skillList);


            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            long[] firstRewardIdList = FirstRewardIdListParse(mCombinationTrainingStatus.firstMTrainingEventRewardIdList);
            GetPracticeSkillData(firstRewardIdList, -1, -1, -1, mCombinationTrainingStatus.id, skillList);

            // エクストラボーナス
            if (mCombinationTrainingStatus.conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mCombinationTrainingStatus.conditionEffectGradeUpRate), -1, -1, -1, PercentString, description, mCombinationTrainingStatus.id, PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 常時特定ステータスアップ
            status = StatusUtility.Parse(mCombinationTrainingStatus.practiceParamRateMap);
            GetPracticeSkillData(status, true, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"], StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], mCombinationTrainingStatus.id, PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, skillList);

            // コンディション割合軽減
            if (mCombinationTrainingStatus.conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(mCombinationTrainingStatus.conditionDiscountRate), -1, -1, -1, PercentString, description, mCombinationTrainingStatus.id, PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            return skillList;
        }
        
        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetCombinationCollectionTotalPracticeSkillList(long[] mCombinationTrainingStatusIds)
        {
            List<PracticeSkillData> skillList = new List<PracticeSkillData>();

            // 初期ステータス
            CharacterStatus status = new CharacterStatus();
            
            int practiceTypeEnumCount = Enum.GetValues(typeof(PracticeType)).Length;

            long battleParamEnhanceRate = 0;
            long rarePracticeEnhanceRate = 0;
            Dictionary<PracticeType, Dictionary<long, long>> conditionEffectGradeUpMapOnTypeValues =
                new Dictionary<PracticeType, Dictionary<long, long>>();
            Dictionary<CharacterStatusType, long> practiceParamAddBonusMapValues = new Dictionary<CharacterStatusType, long>();
            Dictionary<PracticeType, Dictionary<CharacterStatusType, long>> practiceParamEnhanceMapOnTypeValues = new Dictionary<PracticeType, Dictionary<CharacterStatusType, long>>();
            long[] rarePracticeEnhanceRateMapOnTypeRates = new long[practiceTypeEnumCount];
            long[] popRateEnhanceMapOnTypeRates = new long[practiceTypeEnumCount];
            Dictionary<string, long> firstMTrainingEventRewardId = new Dictionary<string, long>();
            long conditionEffectGradeUpRate = 0;
            Dictionary<CharacterStatusType, long> practiceParamRateMapValues = new Dictionary<CharacterStatusType, long>();
            long conditionDiscountRate = 0;
            

            foreach (long id in mCombinationTrainingStatusIds)
            {
                CombinationTrainingStatusMasterObject mCombiantionTrainingStatus = MasterManager.Instance.combinationTrainingStatusMaster.FindData(id);

                status += StatusUtility.Parse(mCombiantionTrainingStatus.firstParamAddMap);
                battleParamEnhanceRate += mCombiantionTrainingStatus.battleParamEnhanceRate;
                rarePracticeEnhanceRate += mCombiantionTrainingStatus.rarePracticeEnhanceRate;
                CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnTypes = ConditionEffectGradeUpMapOnTypeParse(mCombiantionTrainingStatus.conditionEffectGradeUpMapOnType);
                if (conditionEffectGradeUpMapOnTypes is not null)
                {
                    foreach (var conditionEffectGradeUpMapOnType in conditionEffectGradeUpMapOnTypes)
                    {
                        if (conditionEffectGradeUpMapOnType.practiceType == (long)PracticeType.None) continue;
                        // PracticeTypeが含まれていなければなければ作る
                        if (!conditionEffectGradeUpMapOnTypeValues.ContainsKey(
                                (PracticeType)conditionEffectGradeUpMapOnType.practiceType))
                        {
                            conditionEffectGradeUpMapOnTypeValues.Add(
                                (PracticeType)conditionEffectGradeUpMapOnType.practiceType,
                                new Dictionary<long, long>());
                        }

                        // gradeがなければ作る
                        if (conditionEffectGradeUpMapOnTypeValues[
                                (PracticeType)conditionEffectGradeUpMapOnType.practiceType]
                            .ContainsKey(conditionEffectGradeUpMapOnType.grade))
                        {
                            conditionEffectGradeUpMapOnTypeValues[
                                (PracticeType)conditionEffectGradeUpMapOnType.practiceType][
                                conditionEffectGradeUpMapOnType.grade] += conditionEffectGradeUpMapOnType.rate;
                        }
                        else
                        {
                            conditionEffectGradeUpMapOnTypeValues[
                                (PracticeType)conditionEffectGradeUpMapOnType.practiceType].Add(
                                conditionEffectGradeUpMapOnType.grade, conditionEffectGradeUpMapOnType.rate);
                        }
                    }
                }

                CharacterStatus practiceParamAddBonusMap = StatusUtility.Parse(mCombiantionTrainingStatus.practiceParamAddBonusMap);
                practiceParamAddBonusMapValues = GetTotalPracticeSkillList(practiceParamAddBonusMap, practiceParamAddBonusMapValues);
                CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnTypes = PracticeParamEnhanceMapOnTypeParse(mCombiantionTrainingStatus.practiceParamEnhanceMapOnType);
                if (practiceParamEnhanceMapOnTypes is not null)
                {
                    foreach (var practiceParamEnhanceMapOnType in practiceParamEnhanceMapOnTypes)
                    {
                        if (!practiceParamEnhanceMapOnTypeValues.ContainsKey((PracticeType)practiceParamEnhanceMapOnType.practiceType))
                        {
                            practiceParamEnhanceMapOnTypeValues.Add((PracticeType)practiceParamEnhanceMapOnType.practiceType, new Dictionary<CharacterStatusType, long>());
                        }
                        practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType] = GetTotalPracticeSkillList(StatusUtility.Parse(practiceParamEnhanceMapOnType.rateMap), practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType]);
                    }
                }

                CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnTypes = RarePracticeEnhanceRateMapOnTypeParse(mCombiantionTrainingStatus.rarePracticeEnhanceRateMapOnType);
                if (rarePracticeEnhanceRateMapOnTypes is not null)
                {
                    foreach (var rarePracticeEnhanceRateMapOnType in rarePracticeEnhanceRateMapOnTypes)
                    {
                        if (rarePracticeEnhanceRateMapOnType.practiceType == (long)PracticeType.None) continue;
                        rarePracticeEnhanceRateMapOnTypeRates[rarePracticeEnhanceRateMapOnType.practiceType] +=
                            rarePracticeEnhanceRateMapOnType.rate;
                    }
                }

                CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnTypes = PopRateEnhanceMapOnTypeParse(mCombiantionTrainingStatus.popRateEnhanceMapOnType);
                if (popRateEnhanceMapOnTypes is not null)
                {
                    foreach (var popRateEnhanceMapOnType in popRateEnhanceMapOnTypes)
                    {
                        if (popRateEnhanceMapOnType.practiceType == (long)PracticeType.None) continue;
                        popRateEnhanceMapOnTypeRates[popRateEnhanceMapOnType.practiceType] +=
                            popRateEnhanceMapOnType.rate;
                    }
                }

                long[] firstMTrainingEventRewardIdLists = FirstRewardIdListParse(mCombiantionTrainingStatus.firstMTrainingEventRewardIdList);
                if (firstMTrainingEventRewardIdLists is not null)
                {
                    foreach (var firstMTrainingEventRewardIdList in firstMTrainingEventRewardIdLists)
                    {
                        var firstTrainingEventRewardId =
                            MasterManager.Instance.trainingEventRewardMaster.FindData(firstMTrainingEventRewardIdList);
                        string name = firstTrainingEventRewardId.name;
                        if (!firstMTrainingEventRewardId.ContainsKey(name))
                        {
                            firstMTrainingEventRewardId.Add(name, firstTrainingEventRewardId.displayNumber);
                        }
                        else
                        {
                            firstMTrainingEventRewardId[name] += firstTrainingEventRewardId.displayNumber;
                        }
                    }
                }

                conditionEffectGradeUpRate += mCombiantionTrainingStatus.conditionEffectGradeUpRate;
                practiceParamRateMapValues = GetTotalPracticeSkillList(StatusUtility.Parse(mCombiantionTrainingStatus.practiceParamRateMap), practiceParamRateMapValues);
                conditionDiscountRate += mCombiantionTrainingStatus.conditionDiscountRate;
            }

            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.first_param_up.name"], StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], -1, PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, skillList);


            // ステータス
            if (battleParamEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(battleParamEnhanceRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.BattleParamEnhanceRate, PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // レア出現率
            if(rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }
            
            // 練習時発生ボーナス%上昇
            foreach (var conditionEffectGradeUpMapOnType in conditionEffectGradeUpMapOnTypeValues)
            {
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_map_on_type.name"],
                        GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key));
                foreach (var conditionEffectGradeUpMapOnTypeGradeAndRate in conditionEffectGradeUpMapOnType.Value)
                {
                    string description =
                        string.Format(
                            StringValueAssetLoader.Instance[
                                "practice_skill.condition_effect_grade_up_map_on_type.description"],
                            GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key), conditionEffectGradeUpMapOnTypeGradeAndRate.Key);
                    skillList.Add(new PracticeSkillData(name, ToPercent(conditionEffectGradeUpMapOnTypeGradeAndRate.Value), -1, -1, -1, PercentString,
                        description, -1, PracticeSkillType.ConditionEffectGradeUpMapOnType,
                        PracticeSkillIconType.SkillLevelUp, CharacterStatusType.Speed,
                        conditionEffectGradeUpMapOnType.Key));
                }
            }

            // トレーニング固定ボーナス
            foreach (var practiceParamAddBonusMapValue in practiceParamAddBonusMapValues)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"], StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"], StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, practiceParamAddBonusMapValues[practiceParamAddBonusMapValue.Key], -1, -1, -1, string.Empty, description, -1, PracticeSkillType.PracticeParamAddBonusMap, PracticeSkillIconType.StatusUp, practiceParamAddBonusMapValue.Key));
            }

            // 練習時特定ステータス獲得量%アップ
            foreach (var practiceParamEnhanceMapOnTypeValue in practiceParamEnhanceMapOnTypeValues)
            {
                foreach (var practiceParamEnhanceMapOnTypeStatusAndValue in practiceParamEnhanceMapOnTypeValue.Value)
                {
                    // データ追加
                    string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.name"], GetPracticeTypeString(practiceParamEnhanceMapOnTypeValue.Key), StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)practiceParamEnhanceMapOnTypeValue.Key), StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    // リストに追加
                    skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamEnhanceMapOnTypeStatusAndValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamEnhanceMapOnType, PracticeSkillIconType.StatusUp, practiceParamEnhanceMapOnTypeStatusAndValue.Key, (PracticeType)practiceParamEnhanceMapOnTypeValue.Key));
                }
            }

            // 特定スペシャルトレーニング出現率アップ
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (rarePracticeEnhanceRateMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRateMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.RarePracticeEnhanceRateMapOnType, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed, PracticeType.None));
            }

            // 特定スペシャルレクチャー確率アップ
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (popRateEnhanceMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(popRateEnhanceMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PopRateEnhanceMapOnType, PracticeSkillIconType.PopRateUp, CharacterStatusType.Speed, PracticeType.None));
            }

            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            // リストに追加
            foreach (var firstTrainingEventRewardIdDisplayNumber in firstMTrainingEventRewardId)
            {
                skillList.Add(new PracticeSkillData(firstTrainingEventRewardIdDisplayNumber.Key, ToPercent(firstTrainingEventRewardIdDisplayNumber.Value), -1, -1, -1, PercentString, StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria.description"], -1, PracticeSkillType.FirstReward, PracticeSkillIconType.ConditionRecoverUp, CharacterStatusType.Speed));
            }

            // エクストラボーナス
            if (conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(conditionEffectGradeUpRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 常時特定ステータスアップ
            foreach(var practiceParamRateMapValue in practiceParamRateMapValues)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamRateMapValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, practiceParamRateMapValue.Key));
            }

            // コンディション割合軽減
            if (conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(conditionDiscountRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            return skillList;
        }

        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetPracticeSkillList(CombinationStatusTrainingBase combinationStatusTrainingBase)
        {
            var skillList = new List<PracticeSkillData>();
            if (combinationStatusTrainingBase == null) return null;

            int practiceTypeEnumCount = Enum.GetValues(typeof(PracticeType)).Length;
            Dictionary<PracticeType, Dictionary<long, long>> conditionEffectGradeUpMapOnTypeValues = new Dictionary<PracticeType, Dictionary<long, long>>();
            Dictionary<CharacterStatusType, long> practiceParamAddBonusMapValues = new Dictionary<CharacterStatusType, long>();
            Dictionary<PracticeType, Dictionary<CharacterStatusType, long>> practiceParamEnhanceMapOnTypeValues = new Dictionary<PracticeType, Dictionary<CharacterStatusType, long>>();
            long[] rarePracticeEnhanceRateMapOnTypeRates = new long[Enum.GetValues(typeof(PracticeType)).Length];
            long[] popRateEnhanceMapOnTypeRates = new long[Enum.GetValues(typeof(PracticeType)).Length];
            Dictionary<string, long> firstMTrainingEventRewardId = new Dictionary<string, long>();
            long conditionEffectGradeUpRate = 0;
            Dictionary<CharacterStatusType, long> practiceParamRateMapValues = new Dictionary<CharacterStatusType, long>();
            
            // 初期ステータス
            CharacterStatus status = StatusUtility.Parse(combinationStatusTrainingBase.firstParamAddMap);
            // Apiからいただいた補正データでマスターIdがないためmasterIdは0を渡す
            GetPracticeSkillData(status, false, -1, -1, -1, StringValueAssetLoader.Instance["practice_skill.first_param_up.name"], StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], 0, PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, skillList);

            // ステータス
            if (combinationStatusTrainingBase.battleParamEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                // Apiからいただいた補正データでマスターIdがないためmasterIdは0を渡す
                skillList.Add(new PracticeSkillData(name, ToPercent(combinationStatusTrainingBase.battleParamEnhanceRate), -1, -1, -1, PercentString, description, 0, PracticeSkillType.BattleParamEnhanceRate, PracticeSkillIconType.BattleParamEnhanceRate, CharacterStatusType.Speed));
            }

            // レア出現率
            if (combinationStatusTrainingBase.rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                // Apiからいただいた補正データでマスターIdがないためmasterIdは0を渡す
                skillList.Add(new PracticeSkillData(name, ToPercent(combinationStatusTrainingBase.rarePracticeEnhanceRate), -1, -1, -1, PercentString, description, 0, PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 練習時発生ボーナス%上昇
            foreach (var conditionEffectGradeUpMapOnType in combinationStatusTrainingBase.conditionEffectGradeUpMapOnType)
            {
                if (conditionEffectGradeUpMapOnType.practiceType == (long)PracticeType.None) continue;
                // PracticeTypeが含まれていなければなければ作る
                if (!conditionEffectGradeUpMapOnTypeValues.ContainsKey(
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType))
                {
                    conditionEffectGradeUpMapOnTypeValues.Add(
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType,
                        new Dictionary<long, long>());
                }

                // gradeがなければ作る
                if (conditionEffectGradeUpMapOnTypeValues[
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType]
                    .ContainsKey(conditionEffectGradeUpMapOnType.grade))
                {
                    conditionEffectGradeUpMapOnTypeValues[
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType][
                        conditionEffectGradeUpMapOnType.grade] += conditionEffectGradeUpMapOnType.rate;
                }
                else
                {
                    conditionEffectGradeUpMapOnTypeValues[
                        (PracticeType)conditionEffectGradeUpMapOnType.practiceType].Add(
                        conditionEffectGradeUpMapOnType.grade, conditionEffectGradeUpMapOnType.rate);
                }
            }
            foreach (var conditionEffectGradeUpMapOnType in conditionEffectGradeUpMapOnTypeValues)
            {
                string name =
                    string.Format(
                        StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_map_on_type.name"],
                        GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key));
                foreach (var conditionEffectGradeUpMapOnTypeGradeAndRate in conditionEffectGradeUpMapOnType.Value)
                {
                    string description =
                        string.Format(
                            StringValueAssetLoader.Instance[
                                "practice_skill.condition_effect_grade_up_map_on_type.description"],
                            GetPracticeTypeString(conditionEffectGradeUpMapOnType.Key), conditionEffectGradeUpMapOnTypeGradeAndRate.Key);
                    skillList.Add(new PracticeSkillData(name, ToPercent(conditionEffectGradeUpMapOnTypeGradeAndRate.Value), -1, -1, -1, PercentString,
                        description, -1, PracticeSkillType.ConditionEffectGradeUpMapOnType,
                        PracticeSkillIconType.SkillLevelUp, CharacterStatusType.Speed,
                        conditionEffectGradeUpMapOnType.Key));
                }
            }
            
            // トレーニング固定ボーナス
            CharacterStatus practiceParamAddBonusMap =  StatusUtility.Parse(combinationStatusTrainingBase.practiceParamAddBonusMap);
            practiceParamAddBonusMapValues = GetTotalPracticeSkillList(practiceParamAddBonusMap, practiceParamAddBonusMapValues);
            foreach (var practiceParamAddBonusMapValue in practiceParamAddBonusMapValues)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"], StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"], StatusUtility.GetStatusName(practiceParamAddBonusMapValue.Key));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, practiceParamAddBonusMapValues[practiceParamAddBonusMapValue.Key], -1, -1, -1, string.Empty, description, -1, PracticeSkillType.PracticeParamAddBonusMap, PracticeSkillIconType.StatusUp, practiceParamAddBonusMapValue.Key));
            }
            
            // 練習時特定ステータス獲得量%アップ
            foreach (var practiceParamEnhanceMapOnType in combinationStatusTrainingBase.practiceParamEnhanceMapOnType)
            {
                if (!practiceParamEnhanceMapOnTypeValues.ContainsKey((PracticeType)practiceParamEnhanceMapOnType.practiceType))
                {
                    practiceParamEnhanceMapOnTypeValues.Add((PracticeType)practiceParamEnhanceMapOnType.practiceType, new Dictionary<CharacterStatusType, long>());
                }
                practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType] = GetTotalPracticeSkillList(StatusUtility.Parse(practiceParamEnhanceMapOnType.rateMap), practiceParamEnhanceMapOnTypeValues[(PracticeType)practiceParamEnhanceMapOnType.practiceType]);
            }
            foreach (var practiceParamEnhanceMapOnTypeValue in practiceParamEnhanceMapOnTypeValues)
            {
                foreach (var practiceParamEnhanceMapOnTypeStatusAndValue in practiceParamEnhanceMapOnTypeValue.Value)
                {
                    // データ追加
                    string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.name"], GetPracticeTypeString(practiceParamEnhanceMapOnTypeValue.Key), StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)practiceParamEnhanceMapOnTypeValue.Key), StatusUtility.GetStatusName(practiceParamEnhanceMapOnTypeStatusAndValue.Key));
                    // リストに追加
                    skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamEnhanceMapOnTypeStatusAndValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamEnhanceMapOnType, PracticeSkillIconType.StatusUp, practiceParamEnhanceMapOnTypeStatusAndValue.Key, (PracticeType)practiceParamEnhanceMapOnTypeValue.Key));
                }
            }
            
            // 特定スペシャルトレーニング出現率アップ
            foreach (var rarePracticeEnhanceRateMapOnType in combinationStatusTrainingBase.rarePracticeEnhanceRateMapOnType)
            {
                if (rarePracticeEnhanceRateMapOnType.practiceType == (long)PracticeType.None) continue;
                rarePracticeEnhanceRateMapOnTypeRates[rarePracticeEnhanceRateMapOnType.practiceType] +=
                    rarePracticeEnhanceRateMapOnType.rate;

            }
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (rarePracticeEnhanceRateMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(rarePracticeEnhanceRateMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.RarePracticeEnhanceRateMapOnType, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed, PracticeType.None));
            }
            
            // 特定スペシャルレクチャー確率アップ
            foreach (var popRateEnhanceMapOnType in combinationStatusTrainingBase.popRateEnhanceMapOnType)
            {
                if (popRateEnhanceMapOnType.practiceType == (long)PracticeType.None) continue;
                popRateEnhanceMapOnTypeRates[popRateEnhanceMapOnType.practiceType] +=
                    popRateEnhanceMapOnType.rate;
            }
            for (int type = 0; type < practiceTypeEnumCount; type++)
            {
                if (popRateEnhanceMapOnTypeRates[type] == 0) continue;
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.name"], GetPracticeTypeString((PracticeType)type));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.pop_rate_enhance_map_on_type.description"], GetPracticeTypeString((PracticeType)type));
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(popRateEnhanceMapOnTypeRates[type]), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PopRateEnhanceMapOnType, PracticeSkillIconType.PopRateUp, CharacterStatusType.Speed, PracticeType.None));
            }

            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            foreach (var firstMTrainingEventRewardIdList in combinationStatusTrainingBase.firstMTrainingEventRewardIdList)
            {
                var firstTrainingEventRewardId =
                    MasterManager.Instance.trainingEventRewardMaster.FindData(firstMTrainingEventRewardIdList);
                string name = firstTrainingEventRewardId.name;
                if (!firstMTrainingEventRewardId.ContainsKey(name))
                {
                    firstMTrainingEventRewardId.Add(name, firstTrainingEventRewardId.displayNumber);
                }
                else
                {
                    firstMTrainingEventRewardId[name] += firstTrainingEventRewardId.displayNumber;
                }
            }
            foreach (var firstTrainingEventRewardIdDisplayNumber in firstMTrainingEventRewardId)
            {
                skillList.Add(new PracticeSkillData(firstTrainingEventRewardIdDisplayNumber.Key, ToPercent(firstTrainingEventRewardIdDisplayNumber.Value), -1, -1, -1, PercentString, StringValueAssetLoader.Instance["practice_skill.first_reward.cafeteria.description"], -1, PracticeSkillType.FirstReward, PracticeSkillIconType.ConditionRecoverUp, CharacterStatusType.Speed));
            }
            
            // エクストラボーナス
            if (combinationStatusTrainingBase.conditionEffectGradeUpRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_effect_grade_up_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(combinationStatusTrainingBase.conditionEffectGradeUpRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionEffectGradeUpRate, PracticeSkillIconType.RarePracticeEnhanceRate, CharacterStatusType.Speed));
            }

            // 常時特定ステータスアップ
            practiceParamRateMapValues = GetTotalPracticeSkillList(StatusUtility.Parse(combinationStatusTrainingBase.practiceParamRateMap), practiceParamRateMapValues);
            foreach(var practiceParamRateMapValue in practiceParamRateMapValues)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.name"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                string description = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_param_rate_map.description"], StatusUtility.GetStatusName(practiceParamRateMapValue.Key));
                skillList.Add(new PracticeSkillData(name, ToPercent(practiceParamRateMapValue.Value), -1, -1, -1, PercentString, description, -1, PracticeSkillType.PracticeParamRateMap, PracticeSkillIconType.StatusUp, practiceParamRateMapValue.Key));
            }
            
            // コンディション割合軽減
            if (combinationStatusTrainingBase.conditionDiscountRate > 0)
            {
                // データ追加
                string name =
                    string.Format(StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.name"]);
                string description =
                    StringValueAssetLoader.Instance["practice_skill.condition_discount_rate.description"];
                // リストに追加
                skillList.Add(new PracticeSkillData(name, ToPercent(combinationStatusTrainingBase.conditionDiscountRate), -1, -1, -1, PercentString, description, -1, PracticeSkillType.ConditionDiscountRate, PracticeSkillIconType.ConditionDiscount, CharacterStatusType.Speed));
            }

            return skillList;
        }



        private static void GetUniqueSkillData(CharaTrainingStatusMasterObject mStatus, long mCharId, long lv, long liberationLv, List<PracticeSkillData> result)
        {
            // ユニークスキルがある場合はスキル説明をくっつける
            if (mStatus != null)
            {
                // ユニークスキル
                List<PracticeSkillData> uniqueSkillList = new List<PracticeSkillData>();
                // スキルリストを取得
                GetPracticeSkillData(mStatus, mCharId, lv, liberationLv, uniqueSkillList);
                // 説明文
                StringBuilder sb = new StringBuilder();
                bool isEnableTraining = false;
                List<PracticeSkillType> subTypes = new List<PracticeSkillType>();
                List<CharacterStatusType> statueTypes = new List<CharacterStatusType>();
                // 各説明文をつなげる
                foreach(PracticeSkillData uniqueSkill in uniqueSkillList)
                {
                    if(sb.Length > 0)sb.Append("<br>");
                    sb.Append($"{uniqueSkill.Name} {uniqueSkill.ToValueName()}");
                    // トレーニングで有効
                    isEnableTraining |= IsEnableTraining(uniqueSkill.SkillType);
                    subTypes.Add(uniqueSkill.SkillType);
                    if(uniqueSkill.SkillType == PracticeSkillType.PracticeParamAdd)
                    {
                        statueTypes.Add(uniqueSkill.StatusType);
                    }
                }
                // 結果に追加
                result.Add( new PracticeSkillData(mStatus.uniqueName, 0, mCharId, lv, liberationLv, string.Empty, sb.ToString(), mStatus.id, PracticeSkillType.Unique, PracticeSkillIconType.Unique, CharacterStatusType.Speed, statueTypes, subTypes, isEnableTraining) );
            }
        }
        
        
        /// <summary>練習能力の取得</summary>
        public static List<PracticeSkillData> GetPracticeSkillList(long mCharId, long lv, long liberationLv, long trainingScenarioId, bool isIncludeZero = true)
        {
            if(trainingScenarioId < 0)
            {
                trainingScenarioId = 0;
            }
            
            List<PracticeSkillData> result = new List<PracticeSkillData>();
                        
            // 通常スキル
            CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharId, lv, trainingScenarioId, false);
            GetPracticeSkillData(mStatus, mCharId, lv, liberationLv, result);
            // Id0を追加
            if(trainingScenarioId != 0 && isIncludeZero)
            {
                mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharId, lv, 0, false);
                GetPracticeSkillData(mStatus, mCharId, lv, liberationLv, result);
            }
            
            // ユニークスキル
            mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharId, lv, trainingScenarioId, true);
            GetUniqueSkillData(mStatus, mCharId, lv, liberationLv, result);
            
            // Id0を追加
            if(trainingScenarioId != 0 && isIncludeZero)
            {
                mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharId, lv, 0, true);
                GetUniqueSkillData(mStatus, mCharId, lv, liberationLv, result);
            }

            // ソート
            result = result.OrderBy(v=>v.MasterId).ThenBy(v=>v.SkillType).ToList();
            
            return result;            
        }
        
        /// <summary>練習能力の取得</summary>
        public static Dictionary<long, List<PracticeSkillData>> GetAllPracticeSkillDictionary(long mCharId, long liberationLv)
        {
            var result = new Dictionary<long, List<PracticeSkillData>>();
            var mStatusList = MasterManager.Instance.charaTrainingStatusMaster.values.Where(x => x.mCharaId == mCharId);
            foreach (var mStatus in mStatusList)
            {
                if(result.ContainsKey(mStatus.level)) continue;
                result.Add(mStatus.level, GetPracticeSkillList(mCharId, mStatus.level, liberationLv, -1));
            }
            return result;            
        }
        
        public static List<PracticeSkillData> GetSupportEquipmentMainPracticeSkillList(long mCharaId, long lv)
        {
            return GetSupportEquipmentMainPracticeSkillList(mCharaId, lv, null);
        }

        public static List<PracticeSkillData> GetSupportEquipmentMainPracticeSkillList(long mCharaId, long lv, long[] statusIdList)
        {
            var result = new List<PracticeSkillData>();
            
            // 通常スキル
            var mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaId, lv, false);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(mStatus, mCharaId, lv, -1, result);
            
            // ユニークスキル
            mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaId, lv, true);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetUniqueSkillData(mStatus, mCharaId, lv, -1, result);
            
            foreach(PracticeSkillData skillData in result)
            {
                skillData.Category = PracticeSkillCategoryType.Main;
                skillData.StatusIdList = statusIdList;
            }
            
            return result;
        }
        
        public static List<PracticeSkillData> GetSupportEquipmentSubPracticeSkillList(long uEquipmentId, long mCharaId, long characterLv, long statusId)
        {
            return GetSupportEquipmentSubPracticeSkillList(uEquipmentId, mCharaId, characterLv, statusId, null);
        }
        
        public static List<PracticeSkillData> GetSupportEquipmentSubPracticeSkillList(long uEquipmentId, long mCharaId, long characterLv, long statusId, long[] statusIdList)
        {
            CharaTrainerLotteryStatusMasterObject mStatus = MasterManager.Instance.charaTrainerLotteryStatusMaster.FindData(statusId);
            
            var result = new List<PracticeSkillData>();

            // 初期ステータス
            CharacterStatus status = StatusUtility.Parse(mStatus.firstParamAddMap);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(status, false, mCharaId, characterLv, -1,
                StringValueAssetLoader.Instance["practice_skill.first_param_up.name"],
                StringValueAssetLoader.Instance["practice_skill.first_param_up.description"], uEquipmentId,
                PracticeSkillType.FirstParamAdd, PracticeSkillIconType.StatusUp, result);

            // 練習時ステータス
            status = StatusUtility.Parse(mStatus.battleParamEnhanceMap);
            if(status.Tip > 0)
            {
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.practice_game_tip_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.practice_game_tip_enhance_rate.description"];
                // リストに追加
                // サポート器具はliberationLvがないため-1を渡しておく
                result.Add(new PracticeSkillData(name, ToPercent(status.Tip), mCharaId, characterLv, -1, PercentString,
                    description, uEquipmentId, PracticeSkillType.BattleTipEnhanceRate,
                    PracticeSkillIconType.StatusUp, CharacterStatusType.Speed));
            }
            
            // ステータス
            if(mStatus.battleParamEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.battle_param_enhance_rate.description"];
                // リストに追加
                // サポート器具はliberationLvがないため-1を渡しておく
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.battleParamEnhanceRate),
                    mCharaId, characterLv, -1, PercentString, description, uEquipmentId,
                    PracticeSkillType.BattleParamEnhanceRate, PracticeSkillIconType.BattleParamEnhanceRate,
                    CharacterStatusType.Speed));
            }
            
            // レア出現率
            if(mStatus.rarePracticeEnhanceRate > 0)
            {
                // データ追加
                string name = string.Format(StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.name"]);
                string description = StringValueAssetLoader.Instance["practice_skill.rare_practice_enhance_rate.description"];
                // リストに追加
                // サポート器具はliberationLvがないため-1を渡しておく
                result.Add(new PracticeSkillData(name, ToPercent(mStatus.rarePracticeEnhanceRate),
                    mCharaId, characterLv, -1, PercentString, description, uEquipmentId,
                    PracticeSkillType.RarePracticeEnhanceRate, PracticeSkillIconType.RarePracticeEnhanceRate,
                    CharacterStatusType.Speed));
            }
            
            // 練習時発生ボーナス%上昇
            // サポート器具はliberationLvがないため-1を渡しておく
            CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType = ConditionEffectGradeUpMapOnTypeParse(mStatus.conditionEffectGradeUpMapOnType);
            GetPracticeSkillData(conditionEffectGradeUpMapOnType, mCharaId, characterLv, -1, mStatus.id, result);
            
            // トレーニング固定ボーナス
            // サポート器具はliberationLvがないため-1を渡しておく
            status = StatusUtility.Parse(mStatus.practiceParamAddBonusMap);
            GetPracticeSkillData(status, false, mCharaId, characterLv, -1,
                StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.name"],
                StringValueAssetLoader.Instance["practice_skill.practice_param_add_bonus_map.description"],
                mStatus.id, PracticeSkillType.PracticeParamAddBonusMap, PracticeSkillIconType.StatusUp, result);

            // 練習時特定ステータス獲得量%アップ
            CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType = PracticeParamEnhanceMapOnTypeParse(mStatus.practiceParamEnhanceMapOnType);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(practiceParamEnhanceMapOnType, mCharaId, characterLv, -1, mStatus.id, result);

            // 特定スペシャルトレーニング出現率アップ
            CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType = RarePracticeEnhanceRateMapOnTypeParse(mStatus.rarePracticeEnhanceRateMapOnType);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(rarePracticeEnhanceRateMapOnType, mCharaId, characterLv, -1, mStatus.id, result);


            // 特定スペシャルレクチャー確率アップ
            CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType = PopRateEnhanceMapOnTypeParse(mStatus.popRateEnhanceMapOnType);
            // サポート器具はliberationLvがないため-1を渡しておく
            GetPracticeSkillData(popRateEnhanceMapOnType, mCharaId, characterLv, -1, mStatus.id, result);


            // 休息時「食堂」発生率上昇
            // 休息時「就寝」発生率上昇
            long[] firstRewardIdList = FirstRewardIdListParse(mStatus.firstMTrainingEventRewardIdList);
            GetPracticeSkillData(firstRewardIdList, mCharaId, characterLv, -1, mStatus.id, result);

            foreach(PracticeSkillData skillData in result)
            {
                skillData.Category = PracticeSkillCategoryType.Sub;
                skillData.StatusIdList = statusIdList;
            }
            
            return result;
        }
        
        public static List<PracticeSkillData> GetSupportEquipmentSubPracticeSkillList(UserDataSupportEquipment userDataSupportEquipment)
        {
            var result = new List<PracticeSkillData>();

            if (userDataSupportEquipment == null) return result;
            
            long mCharaId = userDataSupportEquipment.charaId;
            long characterLv = userDataSupportEquipment.level;

            foreach(long statusId in userDataSupportEquipment.lotteryProcessJson.statusList)
            {
                result.AddRange( GetSupportEquipmentSubPracticeSkillList( userDataSupportEquipment.id, mCharaId, characterLv, statusId) );
            }
            
            return result;
        }

        public static List<PracticeSkillData> GetSupportEquipmentSubPracticeSkillList(long[] statusIdList, long uEquipmentId, long mCharaId, long characterLv)
        {
            var result = new List<PracticeSkillData>();

            if (statusIdList == null) return result;
            
            foreach(long statusId in statusIdList)
            {
                result.AddRange(GetSupportEquipmentSubPracticeSkillList(uEquipmentId, mCharaId, characterLv, statusId));
            }

            return result;
        }
        
        private static CharaVariableTrainerConditionEffectGradeUpOnType[] ConditionEffectGradeUpMapOnTypeParse(string json)
        {
            if(string.IsNullOrEmpty(json))return null;

            CharaVariableTrainerConditionEffectGradeUpOnType[] result =
                JsonHelper.FromJson<CharaVariableTrainerConditionEffectGradeUpOnType>(json);
            
            return result;
        }

        private static CharaVariableTrainerPracticeParamEnhanceOnType[] PracticeParamEnhanceMapOnTypeParse(string json)
        {
            if(string.IsNullOrEmpty(json))return null;

            CharaVariableTrainerPracticeParamEnhanceOnType[] result =
                JsonHelper.FromJson<CharaVariableTrainerPracticeParamEnhanceOnType>(json);
            
            return result;
        }
        
        private static CharaVariableTrainerRarePracticeEnhanceOnType[] RarePracticeEnhanceRateMapOnTypeParse(string json)
        {
            if(string.IsNullOrEmpty(json))return null;

            CharaVariableTrainerRarePracticeEnhanceOnType[] result =
                JsonHelper.FromJson<CharaVariableTrainerRarePracticeEnhanceOnType>(json);

            return result;
        }
        
        private static CharaVariableTrainerPopRateEnhanceOnType[] PopRateEnhanceMapOnTypeParse(string json)
        {
            if(string.IsNullOrEmpty(json))return null;

            CharaVariableTrainerPopRateEnhanceOnType[] result =
                JsonHelper.FromJson<CharaVariableTrainerPopRateEnhanceOnType>(json);

            return result;
        }

        private static long[] FirstRewardIdListParse(string json)
        {
            if(string.IsNullOrEmpty(json))return null;

            long[] result = JsonHelper.FromJson<long>(json);

            return result;
        }
    }
    */
}