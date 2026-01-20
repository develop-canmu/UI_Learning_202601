using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using System.Linq;
using System.Text;
using Pjfb.Networking.App.Request;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    
    public enum PracticeSkillMasterType
    {
        CharaTrainingStatus,
        CharaTrainingStatusSub,
        CombinationTrainingStatus,
        CharaTrainingComboBuffStatus,
        CharaTrainingComboBuffTotalStatus,
        CharaTrainerLotteryStatus,
        CombinationStatusTrainingBase,
        TrainingCardInspiration,
        TrainingPointStatusEffect,
        TrainingPointStatusEffectChara,
        TotalTrainingStatusSkill
    }
    
    public struct PracticeSkillInfo
    {
        private long trainingStatusTypeDetailId;
        /// <summary>mTrainingStatusTypeDetailのId</summary>
        public long TrainingStatusTypeDetailId{get{return trainingStatusTypeDetailId;}}
        
        private long masterId;
        /// <summary>マスタのId</summary>
        public long MasterId{get{return masterId;}}
        
        private BigValue value;
        /// <summary>効果値</summary>
        public BigValue Value{get{return value;}}
        
        private PracticeSkillMasterType masterType;
        /// <summary>マスタの種類</summary>
        public PracticeSkillMasterType MasterType{get{return masterType;}}
        
        private PracticeSkillInfo[] subSkills;
        /// <summary>ユニーク等の複合スキルの場合</summary>
        public IReadOnlyList<PracticeSkillInfo> SubSkills{get{return subSkills;}}
        
        private static BigValue valueRate = new BigValue(100);
        
#if UNITY_EDITOR
        
        private bool ErrorCheck(long trainingStatusTypeDetailId)
        {
            TrainingStatusTypeDetailMasterObject mDetail = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId);
            if(mDetail == null)
            {
                Debug.LogError("Not found TrainingStatusTypeDetailMaster : " + trainingStatusTypeDetailId);
                return true;
            }
            return false;
        }
        
#endif
        
        /// <summary>アイコンを取得</summary>
        public long GetIconId()
        {
            switch(masterType)
            {
                case PracticeSkillMasterType.CharaTrainingStatus:
                {
                    // マスタ取得
                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(masterId);
                    // ユニークの場合は固定で返す
                    if(mStatus.isUnique)return PracticeSkillUtility.UniqueSkillIconId;
                    // その他
#if UNITY_EDITOR
                    if(ErrorCheck(trainingStatusTypeDetailId))return 0;
#endif
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).iconId;
                }
                    
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                {
                    TrainingPointStatusEffectCharaMasterObject mEffectChara = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(masterId);
                    return mEffectChara.imageId;
                }
                
                default:
                {
#if UNITY_EDITOR
                    if(ErrorCheck(trainingStatusTypeDetailId))return 0;
#endif
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).iconId;
                }
            }
        }
        
        /// <summary>表示名を取得 名前 + 効果値</summary>
        public string GetNameWithValue(StringUtility.OmissionTextData omissionTextData = null)        
        {
        
            switch(masterType)
            {
                // ユニークの場合は効果値がいらない
                case PracticeSkillMasterType.CharaTrainingStatus:
                {
                    // マスタ取得
                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(masterId);
                    // ユニークの場合
                    if(mStatus.isUnique)return mStatus.uniqueName;
                    
                    break;
                }
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                {
                    return MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(masterId).name;
                }
            }
            
            return $"{GetName()} {ToValueName(omissionTextData)}";
        }

        /// <summary>名前を取得</summary>
        public string GetName()
        {
            switch(masterType)
            {
                case PracticeSkillMasterType.CharaTrainingStatus:
                {
                    // マスタ取得
                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(masterId);
                    // ユニークの場合
                    if(mStatus.isUnique)return mStatus.uniqueName;
                    // その他
#if UNITY_EDITOR
                    if(ErrorCheck(trainingStatusTypeDetailId))return trainingStatusTypeDetailId.ToString();
#endif
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).name;
                }
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                {
                    TrainingPointStatusEffectCharaMasterObject mStatus = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(masterId);
                    return mStatus.name;
                }
                case PracticeSkillMasterType.TotalTrainingStatusSkill:
                {
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).aggregatedEffectName;
                }
                default:
                {
#if UNITY_EDITOR
                    if(ErrorCheck(trainingStatusTypeDetailId))return trainingStatusTypeDetailId.ToString();
#endif
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).name;
                }
            }
        }
        
        /// <summary>レベルを取得</summary>
        public long GetLevel()
        {
            switch(masterType)
            {
                case PracticeSkillMasterType.CharaTrainingStatus:
                {
                    // マスタ取得
                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(masterId);
                    return mStatus.level;
                }
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                {
                    TrainingPointStatusEffectCharaMasterObject mStatus = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(masterId);
                    return mStatus.level;
                }
            }
            return -1;
        }
        
        
        /// <summary>ユニークスキル？</summary>
        public bool IsUnique()
        {
            switch(masterType)
            {
                case PracticeSkillMasterType.CharaTrainingStatus:
                {
                    // マスタ取得
                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(masterId);
                    return mStatus.isUnique;
                }
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary> 効果値表示をするか？ </summary>
        public bool ShowValue()
        {
            // ユニークスキルは非表示
            if (IsUnique())
            {
                return false;
                
            }

            TrainingStatusTypeDetailMasterObject detailMaster = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId);
            if (detailMaster != null)
            {
                return detailMaster.valueType != PracticeSkillUtility.ValueNone;   
            }

            return false;
        }
        
        /// <summary>説明を取得</summary>
        public string GetDescription(StringUtility.OmissionTextData omissionTextData = null)
        {
            switch(masterType)
            {
                case PracticeSkillMasterType.CharaTrainingStatus:
                {
                    // マスタ取得
                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(masterId);
                    // ユニークの場合
                    if(mStatus.isUnique)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach(PracticeSkillInfo skill in subSkills)
                        {
                            sb.AppendLine( $"{skill.GetName()} {skill.ToValueName(omissionTextData)}" );
                        }
                        return sb.ToString();
                    }
                    
#if UNITY_EDITOR
                    if(ErrorCheck(trainingStatusTypeDetailId))return "Description : " + trainingStatusTypeDetailId.ToString();
#endif
                    
                    // その他
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).description;
                }

                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                {
                    long effectGroup = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(masterId).mTrainingPointStatusEffectGroup;
                    // mTrainingPointStatusEffectGroupが同じMasterを参照する
                    IEnumerable<TrainingPointStatusEffectMasterObject> trainingPointStatusEffects = MasterManager.Instance.trainingPointStatusEffectMaster.values.Where(x => x.mTrainingPointStatusEffectGroup == effectGroup);
                    StringBuilder description = new StringBuilder();
                    // 効果の名前を取得
                    foreach (var effectMasterObject in trainingPointStatusEffects)
                    {
                        description.AppendLine(effectMasterObject.name);
                    }
                    return description.ToString();
                }
                
                default:
                {
                    
#if UNITY_EDITOR
                    if(ErrorCheck(trainingStatusTypeDetailId))return "Description : " + trainingStatusTypeDetailId.ToString();
#endif
                    
                    return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).description;
                }
            }
        }
        
        /// <summary>効果値の文字列を取得 効果値 + 単位(%) </summary>
        public string ToValueName(StringUtility.OmissionTextData omissionTextData = null)
        {
            if(MasterManager.Instance.trainingStatusTypeDetailMaster.Contains(trainingStatusTypeDetailId))
            {
                TrainingStatusTypeDetailMasterObject mStatus =　MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId);
                // %表示
                if(mStatus != null)
                {
                    switch(mStatus.valueType)
                    {
                        case PracticeSkillUtility.ValueTypePercent:
                        {
                            float v = (float)BigValue.RatioCalculation(value, valueRate);
                            return $"{v}{StringValueAssetLoader.Instance["common.percent"]}";
                        }
                        
                        case PracticeSkillUtility.ValueTypeMul:
                        {
                            BigValue v = value / 10000;
                            return $"{StringValueAssetLoader.Instance["common.mul"]}{v}";
                        }
                        
                        case PracticeSkillUtility.ValueTypeValue:
                        {
                            return omissionTextData == null ? value.ToDisplayCommaString() : value.ToDisplayString(omissionTextData);
                        }
                        
                        // 効果値表示なし
                        case PracticeSkillUtility.ValueNone:
                        default:
                        {
                            return string.Empty;
                        }
                    }
                }
            }
            
            return omissionTextData == null ? value.ToDisplayCommaString() : value.ToDisplayString(omissionTextData);
        }
        
        public long GetMCharaId()
        {
            switch (MasterType)
            {
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                    return MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(MasterId).mCharaId;
                default:
                    return MasterManager.Instance.charaTrainingStatusMaster.FindData(MasterId).mCharaId;
            }
        }

        public PracticeSkillInfo(long trainingStatusTypeDetailType, BigValue value, PracticeSkillMasterType masterType, long masterId, PracticeSkillInfo[] subSkills)
        {
            // 効果値
            this.value = value;
            // マスタのId
            this.masterId = masterId;
            // マスタの種類
            this.masterType = masterType;
            // サブ
            this.subSkills = subSkills;
            // Id初期化
            trainingStatusTypeDetailId = 0;

            switch (masterType)
            {
                case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                    trainingStatusTypeDetailId = PracticeSkillUtility.TrainingPointStatusEffectCharaTypeDetailId;
                    break;
                default:
                    // ユニークはチェックしない
                    if (trainingStatusTypeDetailType != PracticeSkillUtility.UniqueTrainingStatusTypeDetailType)
                    {
                        // trainingStatusTypeDetailIdを探す
                        foreach (var m in MasterManager.Instance.trainingStatusTypeDetailMaster.values)
                        {
                            if (m.type == trainingStatusTypeDetailType)
                            {
                                trainingStatusTypeDetailId = m.id;
                                break;
                            }
                        }

                        // マスタにデータがない
                        if (trainingStatusTypeDetailId == 0)
                        {
                            Logger.LogError($"Not found TrainingStatusTypeDetailMaster Type {trainingStatusTypeDetailType}");
                        }
                    }
                    break;
            }

        }
    }

    /// <summary> 練習能力の効果値範囲情報 </summary>
    public struct PracticeSkillValueRangeInfo
    {
        // 最小値
        private PracticeSkillInfo minValueSkill;
        public PracticeSkillInfo MinValueSkill => minValueSkill;
        
        // 最大値
        private PracticeSkillInfo maxValueSkill;
        public PracticeSkillInfo MaxValueSkill => maxValueSkill;
     
        // 基本のスキル情報(同スキル想定なので最小値のデータを渡す)
        public PracticeSkillInfo SkillInfo => minValueSkill;
        
        public PracticeSkillValueRangeInfo(PracticeSkillInfo minValueSkill, PracticeSkillInfo maxValueSkill)
        {
            this.minValueSkill = minValueSkill;
            this.maxValueSkill = maxValueSkill;
        }

        /// <summary> 効果量の表示 </summary>
        public string ToValueName(StringUtility.OmissionTextData omissionTextData = null)
        {
            // 効果値が同じなら通常表記
            if (minValueSkill.Value == MaxValueSkill.Value)
            {
                return SkillInfo.ToValueName(omissionTextData);
            }
            
            // 効果量の範囲表記
            return string.Format(StringValueAssetLoader.Instance["common.value_range"], MinValueSkill.ToValueName(omissionTextData), MaxValueSkill.ToValueName(omissionTextData));
        }
    }
    
    /// <summary>
    /// TODO
    /// 新しい構造の練習能力
    /// SkillUtilityは削除する
    /// </summary>
    
    public static class PracticeSkillUtility
    {
    
        /// <summary>
        /// ユニークスキルはアイコン固定
        /// </summary>
        public const long UniqueSkillIconId = 1;
        
        /// <summary>
        /// ユニークのId
        /// </summary>
        public const long UniqueTrainingStatusTypeDetailType = -1;
        
        /// <summary>
        /// TrainingPointStatusEffectCharaのTypeDetailId
        /// ユニーク側は初期値から変更しない実装になっているため、-1とかでも問題ないが分かりにくくなるので大げさな値を格納
        /// </summary>
        public const long TrainingPointStatusEffectCharaTypeDetailId = -100;
        
        /// <summary>
        /// ValueType
        /// </summary>
        public const long ValueTypeValue = 1;
        public const long ValueTypePercent = 2;
        public const long ValueTypeMul = 3;
        // 効果値表示なし
        public const long ValueNone = 1000;
        
        /// <summary>％に変換</summary>
        public static float ToPercent(float value, float p = 10000.0f)
        {
            return value / p * 100.0f;
        }
        
        /// <summary>
        /// mCharaTrainingStatusのレコード一つ分の練習能力を取得
        /// </summary>
        public static void GetCharacterPracticeSkill(long mCharaTrainingStatusId, List<PracticeSkillInfo> result)
        {
            // マスタ取得
            CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaTrainingStatusId);
            
            // ユニークスキルの場合
            if(mStatus.isUnique)
            {
                // サブスキル
                List<PracticeSkillInfo> subSkills = new List<PracticeSkillInfo>();
                // Idと効果値を取得
                for(int i=0;i<mStatus.typeList.Length;i++)
                {
                    subSkills.Add( new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CharaTrainingStatusSub, mCharaTrainingStatusId, new PracticeSkillInfo[0]) );
                }
                // ユニークスキル
                PracticeSkillInfo uniqueSkill = new PracticeSkillInfo(UniqueTrainingStatusTypeDetailType, BigValue.Zero, PracticeSkillMasterType.CharaTrainingStatus, mCharaTrainingStatusId, subSkills.ToArray());
                // 結果にユニークスキルを入れる
                result.Add(uniqueSkill);
            }
            else
            {
                // Idと効果値を取得
                for(int i=0;i<mStatus.typeList.Length;i++)
                {
                    result.Add( new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CharaTrainingStatus, mCharaTrainingStatusId, new PracticeSkillInfo[0]) );
                }
            }
        }
        
        /// <summary>キャラの練習能力取得</summary>
        public static List<PracticeSkillInfo> GetCharacterPracticeSkill(long mCharaId, long level)
        {
            return GetCharacterPracticeSkill(mCharaId, level, 0, true);
        }
        
        /// <summary>キャラの練習能力取得</summary>
        public static List<PracticeSkillInfo> GetCharacterPracticeSkill(long mCharaId)
        {
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            
            // すべてのマスタをチェック
            foreach(CharaTrainingStatusMasterObject value in MasterManager.Instance.charaTrainingStatusMaster.values)
            {
                // キャラId
                if(value.mCharaId != mCharaId)continue;
                // スキル取得
                GetCharacterPracticeSkill(value.id, result);
            }
            
            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            // スペシャルブーストの取得
            foreach(TrainingPointStatusEffectCharaMasterObject mCharaEffect in MasterManager.Instance.trainingPointStatusEffectCharaMaster.values)
            {
                // キャラId
                if(mCharaEffect.mCharaId != mCharaId)continue;
                
                // スペシャルブーストを取得
                PracticeSkillInfo specialBoost = GetPracticeSkillInfoFromTrainingPointStatusEffectChara(mCharaEffect);
                result.Add(specialBoost);
            }
            
            return result;
        }
        
        /// <summary>キャラの練習能力取得</summary>
        public static List<PracticeSkillInfo> GetCharacterPracticeSkill(long mCharaId, long level, long trainingScenarioId, bool isIncludeTrainingScenarioZeroId = true)
        {
            if(trainingScenarioId < 0)
            {
                trainingScenarioId = 0;
            }
            
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            
            // 通常スキル
            CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaId, level, trainingScenarioId, false);
            if(mStatus != null)
            {
                GetCharacterPracticeSkill(mStatus.id, result);
            }
            // Id0を追加
            if(trainingScenarioId != 0 && isIncludeTrainingScenarioZeroId)
            {
                mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaId, level, 0, false);
                if(mStatus != null)
                {
                    GetCharacterPracticeSkill(mStatus.id, result);
                }
            }
            
            // ユニークスキル
            mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaId, level, trainingScenarioId, true);
            if(mStatus != null)
            {
                GetCharacterPracticeSkill(mStatus.id, result);
            }
            
            // Id0を追加
            if(trainingScenarioId != 0 && isIncludeTrainingScenarioZeroId)
            {
                mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(mCharaId, level, 0, true);
                if(mStatus != null)
                {
                    GetCharacterPracticeSkill(mStatus.id, result);
                }
            }

            // シナリオの指定がない,シナリオに紐づかないスキルの表示を行う場合のみ追加する(スペシャルブーストは特攻効果にはないので)
            if (trainingScenarioId == 0 || isIncludeTrainingScenarioZeroId)
            {
                // スペシャルブーストを取得
                TrainingPointStatusEffectCharaMasterObject mCharaEffect = MasterManager.Instance.trainingPointStatusEffectCharaMaster.GetDataByCharaData(mCharaId, level);
                if (mCharaEffect != null)
                {
                    PracticeSkillInfo specialBoost = GetPracticeSkillInfoFromTrainingPointStatusEffectChara(mCharaEffect);
                    result.Add(specialBoost);
                }
            }

            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            return result;
        }
        
        /// <summary>修得済みのスキルと未修得のスキルを結合し返す</summary>
        public static List<PracticeSkillInfo> GetCharacterPracticeSkillAcquireAndUnAcquire(long mCharaId, long level)
        {
            // 一旦すべて取得
            List<PracticeSkillInfo> skillList = GetCharacterPracticeSkill(mCharaId);
            // 結果よう
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            
            //　今のLevelで修得しているスキル
            var acquireSkills = PracticeSkillUtility.GetCharacterPracticeSkill(mCharaId, level);
            // 修得スキルはresultに追加
            result.AddRange(acquireSkills);
            
            // TrainingStatusTypeDetail
            HashSet<long> trainingStatusTypeDetaiId = new HashSet<long>();
            // 習得済みのスキルIdを追加
            acquireSkills.ForEach(x => trainingStatusTypeDetaiId.Add(x.TrainingStatusTypeDetailId));
            // ユニーク
            // ユニークスキルは各キャラ１つずつ＆TrainingStatusTypeDetailIdを持っていない
            bool isCheckedUnique = acquireSkills.Any(x => x.MasterType == PracticeSkillMasterType.CharaTrainingStatus && MasterManager.Instance.charaTrainingStatusMaster.FindData(x.MasterId).isUnique);
            // スペシャルブーストを既に持っているか
            bool hasTrainingPointStatusEffectChara = acquireSkills.Any(x => x.MasterType == PracticeSkillMasterType.TrainingPointStatusEffectChara);
            // レベル順にソート
            IOrderedEnumerable<PracticeSkillInfo> orderedList = skillList.OrderBy(v=> v.GetLevel());

            // 表示順
            Dictionary<long, long> displayOrder = new Dictionary<long, long>();
            long uniqueDisplayOrder = 0;
            long trainingPointStatusEffectCharaDisplayOrder = 0;

            foreach(PracticeSkillInfo skill in orderedList )
            {
                switch (skill.MasterType)
                {
                    case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                        // すでに表示順セット済み
                        if (trainingPointStatusEffectCharaDisplayOrder > 0)
                        {
                            continue;
                        }
                        TrainingPointStatusEffectCharaMasterObject mEffectChara = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(skill.MasterId);
                        // 最初に解放されるレベル帯でセット
                        trainingPointStatusEffectCharaDisplayOrder = mEffectChara.level;
                        // 既に追加済みなら追加しない
                        if(hasTrainingPointStatusEffectChara) continue;
                        hasTrainingPointStatusEffectChara = true;
                        break;
                    default:
                        // マスタ
                        CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(skill.MasterId);
                
                        // 特定のシナリオのみ
                        if(mStatus.mTrainingScenarioId != 0)continue;
                
                        // ユニークの表示順
                        if(mStatus.isUnique)
                        {
                            uniqueDisplayOrder = mStatus.level;
                        }
                
                        // 表示順ソート用
                        if(displayOrder.ContainsKey(skill.TrainingStatusTypeDetailId) == false)
                        {
                            displayOrder.Add(skill.TrainingStatusTypeDetailId, mStatus.level);
                        }
                
                        // 既に習得済み
                        if(mStatus.level < level)continue;

                        // ユニークスキル
                        if(mStatus.isUnique)
                        {
                            // 既にチェック済み
                            if(isCheckedUnique)continue;
                            // チェック済みに
                            isCheckedUnique = true;
                        }
                        else
                        {
                            // 既にチェック済み
                            if(trainingStatusTypeDetaiId.Contains(skill.TrainingStatusTypeDetailId))continue;
                            // チェック済みに
                            trainingStatusTypeDetaiId.Add(skill.TrainingStatusTypeDetailId);
                        }
                        break;
                }

                result.Add(skill);
            }
            
            // 表示順にソート
            return result 
                // 　習得してるもの優先
                .OrderByDescending(v=>
                    {
                        long mLevel = 0;
                        switch (v.MasterType)
                        {
                            case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                                mLevel = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(v.MasterId).level;
                                break;
                            default:
                                mLevel = MasterManager.Instance.charaTrainingStatusMaster.FindData(v.MasterId).level;
                                break;
                        }
                        return mLevel <= level;
                    })
                // 表示順
                .ThenBy(v=>
                    {
                        switch (v.MasterType)
                        {
                            case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                                return trainingPointStatusEffectCharaDisplayOrder;
                            default:
                                if (v.MasterType == PracticeSkillMasterType.CharaTrainingStatus)
                                {
                                    CharaTrainingStatusMasterObject mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(v.MasterId);
                                    if(mStatus.isUnique)
                                    {
                                        return uniqueDisplayOrder;
                                    }
                                }
                                return displayOrder[v.TrainingStatusTypeDetailId];
                        }
                    })
                // 参照マスタType順
                .ThenBy(v => v.MasterType)
                // マスタのId
                .ThenBy(v=>v.MasterId)
                .ToList();
        }
        
        /// <summary>
        /// mCombiationTrainingStatusのレコード一つ分の練習能力を取得
        /// </summary>
        private static void GetCombinationPracticeSkill(long mCombinationId, List<PracticeSkillInfo> result)
        {
            CombinationTrainingStatusMasterObject mStatus = MasterManager.Instance.combinationTrainingStatusMaster.FindData(mCombinationId);
            
            // Idと効果値を取得
            for(int i=0;i<mStatus.typeList.Length;i++)
            {
                result.Add( new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CombinationTrainingStatus, mCombinationId, null) );
            }
        }
        
        /// <summary>コンビネーションの練習能力取得</summary>
        public static List<PracticeSkillInfo> GetCombinationPracticeSkill(long mCombinationId)
        {
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            // スキル取得
            GetCombinationPracticeSkill(mCombinationId, result);            
            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            return result;
        }

        /// <summary> コンビネーション(コレクションスキル)の練習能力取得 </summary>
        public static List<PracticeSkillInfo> GetCombinationTotalPracticeSkill(long[] mCombinationId)
        {
            // 結果格納用
            List<PracticeSkillInfo> statusList = new List<PracticeSkillInfo>();
            // 一旦すべてのスキルを取得
            foreach(long id in mCombinationId)
            {
                // スキル取得
                GetCombinationPracticeSkill(id, statusList);
            }
      
            // タイプごとに集計
            Dictionary<long, BigValue> totalValues =  GetTotalValue(statusList);;

            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            // 結果を生成
            foreach(KeyValuePair<long, BigValue> value in totalValues)
            {
                TrainingStatusTypeDetailMasterObject mDetail = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(value.Key);
                result.Add( new PracticeSkillInfo(mDetail.type, value.Value, PracticeSkillMasterType.CombinationTrainingStatus, 0, null) );
            }

            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            return result;
        }
        
        /// <summary>
        /// mCharaTrainingComboBuffStatusのレコード一つ分の練習能力取得
        /// </summary>
        private static void GetComboBuffPracticeSkill(long mCharaTrainingComboBuffStatusId, List<PracticeSkillInfo> result)
        {
            CharaTrainingComboBuffStatusMasterObject mStatus = MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(mCharaTrainingComboBuffStatusId);
            
            // Idと効果値を取得
            for(int i=0;i<mStatus.typeList.Length;i++)
            {
                result.Add( new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CharaTrainingComboBuffStatus, mCharaTrainingComboBuffStatusId, null) );
            }
        }
        
        /// <summary>コンボバフの練習能力取得</summary>
        public static List<PracticeSkillInfo> GetComboBuffPracticeSkill(long mCharaTrainingComboBuffStatusId)
        {
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            // スキル取得
            GetComboBuffPracticeSkill(mCharaTrainingComboBuffStatusId, result);            
            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            return result;
        }
        
        /// <summary>コンボバフの練習能力取得</summary>
        public static List<PracticeSkillInfo> GetComboBuffTotalPracticeSkill(long[] mCharaTrainingComboBuffStatusIds)
        {
            // 結果格納用
            List<PracticeSkillInfo> statusList = new List<PracticeSkillInfo>();
            // 一旦すべてのスキルを取得
            foreach(long id in mCharaTrainingComboBuffStatusIds)
            {
                // スキル取得
                GetComboBuffPracticeSkill(id, statusList);  
            }
            
            // タイプごとに集計
            Dictionary<long, BigValue> totalValues =  GetTotalValue(statusList);

            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            // 結果を生成
            foreach(KeyValuePair<long, BigValue> value in totalValues)
            {
                TrainingStatusTypeDetailMasterObject mDetail = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(value.Key);
                result.Add( new PracticeSkillInfo(mDetail.type, value.Value, PracticeSkillMasterType.CharaTrainingComboBuffTotalStatus, 0, null) );
            }

            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            return result;
        }

        /// <summary> スキルidごとの効果量の合計を計算 </summary>
        private static Dictionary<long, BigValue> GetTotalValue(List<PracticeSkillInfo> statusList)
        {
            Dictionary<long, BigValue> totalValues = new Dictionary<long, BigValue>();
            
            foreach(PracticeSkillInfo status in statusList)
            {
                if(totalValues.TryGetValue(status.TrainingStatusTypeDetailId, out BigValue value) == false)
                {
                    totalValues.Add(status.TrainingStatusTypeDetailId, BigValue.Zero);
                }
                // 合計値
                totalValues[status.TrainingStatusTypeDetailId] += status.Value;
            }

            return totalValues;
        }
        
        /// <summary>
        /// mCharaTrainerLotteryStatusのレコード一つ分の練習能力取得
        /// </summary>
        private static void GetCharaTrainerLotteryStatusPracticeSkill(long mCharaTrainerLotteryStatusId, List<PracticeSkillInfo> result)
        {
            CharaTrainerLotteryStatusMasterObject mStatus = MasterManager.Instance.charaTrainerLotteryStatusMaster.FindData(mCharaTrainerLotteryStatusId);
            
            // Idと効果値を取得
            for(int i=0;i<mStatus.typeList.Length;i++)
            {
                result.Add( new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CharaTrainerLotteryStatus, mCharaTrainerLotteryStatusId, null) );
            }
        }
        
        /// <summary>サポート器具の練習能力取得</summary>
        public static List<PracticeSkillInfo> GetCharaTrainerLotteryStatusPracticeSkill(long mCharaTrainerLotteryStatusId)
        {
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            // スキル取得
            GetCharaTrainerLotteryStatusPracticeSkill(mCharaTrainerLotteryStatusId, result);            
            // ソート
            result = result.OrderBy(v=>v.TrainingStatusTypeDetailId).ToList();
            
            return result;
        }
        
        /// <summary>サポート器具の練習能力取得</summary>
        public static List<PracticeSkillInfo> GetCharaTrainerLotteryStatusPracticeSkill(long[] mCharaTrainerLotteryStatusIds)
        {
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();

            //　ガチャのピックアップ欄ではサブスキルに何も入らないのでnullの場合はそのまま返す
            if (mCharaTrainerLotteryStatusIds == null || mCharaTrainerLotteryStatusIds.Length == 0)
            {
                return result;
            }
            
            // すべてのIdを取得
            foreach(long id in mCharaTrainerLotteryStatusIds)
            {
                // スキル取得
                GetCharaTrainerLotteryStatusPracticeSkill(id, result);   
            }
            
           
            
            return result;
        }
        
        /// <summary>
        /// CombinationStatusTrainingBaseから練習能力を取得
        /// </summary>
        public static List<PracticeSkillInfo> GetCombinationStatusTrainingPracticeSkill(CombinationStatusTrainingBase  combinationStatusTrainingBase)
        {
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
        
            // Idと効果値を取得
            for(int i=0;i<combinationStatusTrainingBase.typeList.Length;i++)
            {
                result.Add( new PracticeSkillInfo(combinationStatusTrainingBase.typeList[i], new BigValue(combinationStatusTrainingBase.valueList[i]), PracticeSkillMasterType.CombinationStatusTrainingBase, 0, null) );
            }
        
            return result;
        }
        
        /// <summary>
        /// TrainingCardInspireから練習能力を取得
        /// </summary>
        public static List<PracticeSkillInfo> GetTrainingCardInspirationPracticeSkill(long mTrainingCardInspirationId)
        {
            TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(mTrainingCardInspirationId);
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            
            // Idと効果値を取得
            for(int i=0;i<mCard.typeList.Length;i++)
            {
                result.Add( new PracticeSkillInfo(mCard.typeList[i], new BigValue(mCard.valueList[i]), PracticeSkillMasterType.TrainingCardInspiration, 0, null) );
            }
        
            return result;
        }
        
        /// <summary>
        /// TrainingPointStatusEffectから練習能力を取得
        /// </summary>
        public static List<PracticeSkillInfo> GetTrainingPointStatusEffectPracticeSkill(long mTrainingPointStatusEffectId)
        {
            TrainingPointStatusEffectMasterObject mEffect = MasterManager.Instance.trainingPointStatusEffectMaster.FindData(mTrainingPointStatusEffectId);
            // 結果格納用
            List<PracticeSkillInfo> result = new List<PracticeSkillInfo>();
            
            // Idと効果値を取得
            for(int i=0;i<mEffect.typeList.Length;i++)
            {
                result.Add( new PracticeSkillInfo(mEffect.typeList[i], new BigValue(mEffect.valueList[i]), PracticeSkillMasterType.TrainingPointStatusEffect, 0, null) );
            }
        
            return result;
        }

        private static PracticeSkillInfo GetPracticeSkillInfoFromTrainingPointStatusEffectChara(TrainingPointStatusEffectCharaMasterObject mCharaEffect)
        {
            PracticeSkillInfo practiceSkillInfo = new PracticeSkillInfo(0, BigValue.Zero, PracticeSkillMasterType.TrainingPointStatusEffectChara, mCharaEffect.id, null);
            return practiceSkillInfo;
        }
        
           /// <summary> 引数のスキルリストから効果値範囲を計算しリストデータとして返す </summary>
        public static List<PracticeSkillValueRangeInfo> GetPracticeSkillValueRangeList(List<PracticeSkillInfo> skillList)
        {
            // 結果用
            List<PracticeSkillValueRangeInfo> result = new List<PracticeSkillValueRangeInfo>();
            
            // スキルタイプ毎にわける
            Dictionary<long, List<PracticeSkillInfo>> skillTypeDetailDictionary = new Dictionary<long, List<PracticeSkillInfo>>();
            
            foreach (PracticeSkillInfo skillInfo in skillList)
            {
                if (skillTypeDetailDictionary.TryGetValue(skillInfo.TrainingStatusTypeDetailId, out List<PracticeSkillInfo> infos) == false)
                {
                    infos = new List<PracticeSkillInfo>();
                    skillTypeDetailDictionary.Add(skillInfo.TrainingStatusTypeDetailId, infos);
                }
                infos.Add(skillInfo);
            }

            foreach (List<PracticeSkillInfo> skillInfos in skillTypeDetailDictionary.Values)
            {
                PracticeSkillInfo min = default;
                PracticeSkillInfo max = default;
                // 初期化済みか
                bool isInitialize = false;
                
                foreach (PracticeSkillInfo skill in skillInfos)
                {
                    // 初期化前は最初のデータの値でセット
                    if (isInitialize == false)
                    {
                        min = skill;
                        max = skill;
                        isInitialize = true;
                    }
                    else
                    {
                        // 効果値の最大値と最小値を更新していく
                        
                        if (min.Value > skill.Value)
                        {
                            min = skill;
                        }

                        if (max.Value < skill.Value)
                        {
                            max = skill;
                        }
                    }
                }

                result.Add(new PracticeSkillValueRangeInfo(min, max));
            }

            return result.OrderBy(x => x.SkillInfo.TrainingStatusTypeDetailId).ToList();
        }
    }
}