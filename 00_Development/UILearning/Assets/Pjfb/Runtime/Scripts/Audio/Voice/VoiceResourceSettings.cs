using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pjfb.InGame;
using Pjfb.Master;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Voice
{
    [CreateAssetMenu]
    public class VoiceResourceSettings : ScriptableObject
    {
        #region enum
        [Serializable]
        public enum LocationType
        {
            Unknown = 0,
            
            PART_UNNOWN = 1000,
            PART_CALL = 1001,
            PART_REPLY = 1004,
            PART_POSITIVE_SMALL = 1007,
            PART_POSITIVE_LARGE = 1008,
            PART_NEGATIVE_SMALL = 1009,
            PART_NEGATIVE_LARGE = 1010,
            PART_PROTEST = 1011,
            PART_THANK = 1012,
            PART_APOLOGY = 1013,
            PART_REQUEST = 1014,
            PART_TIRED = 1015,
            PART_RELIEF = 1016,
            PART_LAUGH_SMALL = 1017,
            PART_LAUGH_LAEGE = 1018,
            PART_WRY_SMILE = 1019,
            PART_ANGER_SMALL = 1020,
            PART_ANGER_MEDIUM = 1021,
            PART_ANGER_LARGE = 1022,
            PART_SORROW_SMALL = 1023,
            PART_SORROW_LARGE = 1024,
            PART_QUESTION_SMALL = 1025,
            PART_QUESTION_LARGE = 1026,
            PART_SURPRISE_SMALL = 1027,
            PART_SURPRISE_LARGE = 1028,
            PART_DELIGHT = 1029,
            PART_CONFIDENCE = 1030,
            PART_ANXIETY = 1031,
            PART_DESPAIR = 1032,
            PART_UNPLEASANT = 1033,
            PART_AMAZED = 1034,
            PART_WORRIES = 1035,
            PART_CONTEMPLATION = 1036,
            PART_YELL = 1037,
            PART_IMPRESSED_SMALL = 1039,
            PART_IMPRESSED_LARGE = 1040,
            PART_SOCCER_YELL = 1041,
            PART_SOCCER_PASS = 1043,
            PART_SOCCER_SHOOT = 1045,
            PART_SOCCER_DRIBBLE = 1047,
            PART_SOCCER_SURPRISE_SMALL = 1049,
            PART_SOCCER_SURPRISE_LARGE = 1050,
            PART_SOCCER_QUESTION_SMALL = 1051,
            PART_SOCCER_QUESTION_LARGE = 1052,

            SYSTEM_UNNOWN = 2000,
            SYSTEM_TITLE = 2001,
            SYSTEM_GACHA = 2002,
            SYSTEM_TAP = 2003,
            SYSTEM_LV_UP = 2013,
            SYSTEM_REARITY_UP = 2018,
            SYSTEM_TRAINING_SELECT = 2021,
            SYSTEM_TRAINING_START = 2022,
            SYSTEM_TRAINING_SUCCESS = 2023,
            SYSTEM_TRAINING_BAD = 2033,
            SYSTEM_TRAINING_REST = 2043,
            SYSTEM_TRAINING_EXIT_VERY_GOOD = 2045,
            SYSTEM_TRAINING_EXIT_GOOD = 2046,
            SYSTEM_TRAINING_EXIT_NORMAL = 2047,
            SYSTEM_TRAINING_PROMISING = 2048,
            SYSTEM_TRAINING_TIRED = 2051,
            
            IN_UNNOWN = 3000,
            IN_KICKOFF_PRECEDENCE = 3001,
            IN_KICKOFF_RECEIVED = 3006,
            IN_DRIBBLE_PROMISING = 3011,
            IN_DRIBBLE_IMPATIENCE = 3014,
            IN_DRIBBLE_TIRED = 3017,
            IN_DRIBBLE_ALLY = 3020,
            IN_DRIBBLE_MARK = 3023,
            IN_MATCHUP_OPPONENT_OFFENSE = 3026,
            IN_MATCHUP_OPPONENT_DEFENSE = 3027,
            IN_MATCHUP_ALLY_OFFENSE = 3032,
            IN_MATCHUP_ALLY_DEFENSE = 3035,
            IN_SPEED_MATCHUP_OFFENSE = 3038,
            IN_SPEED_MATCHUP_DEFENSE = 3041,
            IN_PHYSICAL_MATCHUP_OFFENSE = 3044,
            IN_PHYSICAL_MATCHUP_DEFENSE = 3047,
            IN_TECHNIQUE_MATCHUP_OFFENSE = 3050,
            IN_TECHNIQUE_MATCHUP_DEFENSE = 3053,
            IN_MATCHUP_WIN_OFFENSE = 3056,
            IN_MATCHUP_LOSE_OFFENSE = 3058,
            IN_MATCHUP_WIN_DEFENSE = 3060,
            IN_MATCHUP_LOSE_DEFENSE = 3062,
            IN_THROWIN = 3064,
            IN_SECOND_BALL_SUCCESS = 3066,
            IN_SECOND_BALL_FAILURE = 3067,
            IN_CROSS = 3070,
            IN_PASS_GIVE = 3073,
            IN_PASS_RECEIVE = 3076,
            IN_SHOOT = 3079,
            IN_AFTER_GALL = 3082,
            IN_EXCLUSIVE_SHOOT = 3085,
            IN_EXCLUSIVE_GALL = 3086,
            IN_GALL_HAPPY = 3087,
            IN_SHOOT_BLOCK_MISS = 3092,
            IN_SHOOT_BLOCK_TOUCH = 3093,
            IN_SHOOT_BLOCK_STOP = 3094,
            IN_PASS_CUT = 3095,
            IN_YELL = 3098,
            IN_WIN = 3103,
            IN_LOSE = 3106,
            IN_BLOCK = 3109,
            IN_SKILL = 3110,
        }

        [Serializable]
        public enum VoiceType
        {
            Other = 0,
            Part = 1,
            System = 2,
            In = 3,
            InSp = 4,
            Scenario = 5,
            Skill = 6,
            Max = 7,
        }
        #endregion

        [Serializable]
        public class UsageSetting
        {
            public int index;
            public LocationType type;
        }
        
        [Serializable]
        public class UsageCount
        {
            public int count;
            public LocationType usageType;

            public UsageCount(int count, LocationType usageType)
            {
                this.count = count;
                this.usageType = usageType;
            }
        }
        
        [Serializable]
        public class VoiceDetail
        {
            public string path;
            public int personalId;
            public int personalUniqueId;
            public VoiceType voiceType;
            public LocationType locationType;
            public int useType;
        }

        [Serializable]
        public class CharaData
        {
            [HideInInspector]
            public string viewNames;
            [HideInInspector]
            public int personalId;
            [HideInInspector]
            public List<UsageCount> usageCountList;
            public List<VoiceDetail> partVoiceList;
            public List<VoiceDetail> systemVoiceList;
            public List<VoiceDetail> inVoiceList;
            public List<VoiceDetail> inSpVoiceList;
            public List<VoiceDetail> skillVoiceList;
            public List<VoiceDetail> otherVoiceList;

            public CharaData(int id)
            {
                viewNames = String.Format("{0:D4}", id);
                personalId = id;
                otherVoiceList = new List<VoiceDetail>();
                partVoiceList = new List<VoiceDetail>();
                systemVoiceList = new List<VoiceDetail>();
                inVoiceList = new List<VoiceDetail>();
                inSpVoiceList = new List<VoiceDetail>();
                skillVoiceList = new List<VoiceDetail>();
            }
            
            public void AddVoiceDetail(VoiceDetail detail)
            {
                var targetList = GetVoiceList(detail.voiceType);
                if (targetList.Any(data => data.useType == detail.useType && data.personalUniqueId == detail.personalUniqueId))
                {
                    Logger.LogError($"使用済みのボイスIDとなるためリストに追加されませんでした:{detail.path}");
                    return;
                }
                targetList.Add(detail);
            }

            public VoiceDetail FindVoiceDetailForIndex(VoiceType voiceType, long useType, long personalUniqueId)
            {
                var targetList = GetVoiceList(voiceType);
                return targetList?.FirstOrDefault(detail => detail.useType == useType && detail.personalUniqueId == personalUniqueId) ?? null;
            }
            
            public bool ValidateUniqueVoice(VoiceType voiceType, long useType, long personalUniqueId)
            {
                if (personalUniqueId == 0)
                {
                    return false;
                }
                var targetList = GetVoiceList(voiceType);
                return targetList?.Any(detail => detail.useType == useType && detail.personalUniqueId == personalUniqueId) ?? false;
            }
            
            public void UpdateUsageTypeCount()
            {
                usageCountList = new List<UsageCount>();
                for(var voiceType = VoiceType.Other; voiceType < VoiceType.Max; voiceType++)
                {
                    var voiceList = GetVoiceList(voiceType);
                    foreach (var detail in voiceList)
                    {
                        // 差し替えボイスは個数に含めない
                        if (detail.personalUniqueId != 0)
                        {
                            continue;
                        }
                        var type = detail.locationType;
                        var usageCount = usageCountList.FirstOrDefault(data => data.usageType == type);
                        if (usageCount == null)
                        {
                            usageCount = new UsageCount(0, type);
                            usageCountList.Add(usageCount);
                        }
                        usageCount.count++;
                    }
                }
            }
            
            public int GetUsageTypeCount(LocationType usageType)
            {
                return usageCountList.FirstOrDefault(data => data.usageType == usageType)?.count ?? 0;
            }

            public List<VoiceDetail> GetVoiceList(VoiceType voiceType)
            {
                switch (voiceType)
                {
                    case VoiceType.Part:
                        return partVoiceList;
                    case VoiceType.System:
                        return systemVoiceList;
                    case VoiceType.In:
                        return inVoiceList;
                    case VoiceType.InSp:
                        return inSpVoiceList;
                    case VoiceType.Skill:
                        return skillVoiceList;
                    default:
                        return otherVoiceList;
                }
            }

            public List<VoiceDetail> GetVoiceList(VoiceType voiceType, LocationType usageType, long personalUniqueId)
            {
                var voiceList = GetVoiceList(voiceType);
                // 結果格納用
                List<VoiceDetail> result = new List<VoiceDetail>();
                // 種別が一致するものを取得
                foreach(VoiceDetail voice in voiceList)
                {
                    if(voice.locationType == usageType)
                    {
                        result.Add(voice);
                    }
                }
                
                // 固有ボイスが存在するかチェック
                bool existsUniqueVoice = false;
                foreach(VoiceDetail voice in result)
                {
                    if(voice.personalUniqueId == personalUniqueId)
                    {
                        existsUniqueVoice = true;
                        break;
                    }
                }
                
                // 固有ボイスが存在しない場合は汎用(id:0)を使用する
                long uniqueId = existsUniqueVoice ? personalUniqueId : 0;
                // 不要なものをリストから削除
                result.RemoveAll(voice => voice.personalUniqueId != uniqueId);
                return result;
            }
        }
        
        [Header("ボイス用途 予約設定 (予約番号増えるなら手動で追加)")] [CanBeNull] 
        public List<UsageSetting> systemTypeSettings;
        public List<UsageSetting> partTypeSettings;
        public List<UsageSetting> inTypeSettings;

        [Header("ボイスリスト (Tools/Voice/UpdateVoiceListから更新)")]
        public List<CharaData> charaVoiceList;

        [Header("チュートリアルシナリオリスト (Tools/Voice/UpdateVoiceListから更新)")]
        public List<VoiceDetail> scenarioVoiceList;
        
        public LocationType IndexToUsageType(VoiceType voiceType, int index)
        {
            switch (voiceType)
            {
                case VoiceType.In:
                    return inTypeSettings.FirstOrDefault(data => data.index == index)?.type ?? LocationType.Unknown;
                case VoiceType.Part:
                    return partTypeSettings.FirstOrDefault(data => data.index == index)?.type ?? LocationType.Unknown;
                case VoiceType.System:
                    return systemTypeSettings.FirstOrDefault(data => data.index == index)?.type ?? LocationType.Unknown;
            }
            return LocationType.Unknown;
        }

        public string GetVoicePathForIndex(VoiceType voiceType, long personalId, long useType, long personalUniqueId)
        {
            // 指定のuseTypeから一致するボイスを返す、personalUniqueId指定のボイスがあればそちらを優先
            var voiceList = charaVoiceList.FirstOrDefault(chara => chara.personalId == personalId);

            if (voiceList == null) return string.Empty;
            personalUniqueId = voiceList.ValidateUniqueVoice(voiceType, useType, personalUniqueId) ? personalUniqueId : 0;
            var voiceDetail = voiceList.FindVoiceDetailForIndex(voiceType, useType, personalUniqueId);
            return voiceDetail?.path ?? string.Empty;
        }
        
        public string GetVoicePathForScenarioId(long locationType, long useType)
        {
            // チュートリアルシナリオのボイスのみ人物IDの代わりにlocationTypeとvoice.personalIdをぶつける
            var voiceDetail = scenarioVoiceList.FirstOrDefault(voice => voice.personalId == locationType && voice.useType == useType);
            return voiceDetail?.path ?? string.Empty;
        }
        
        public string GetVoicePathForUsageType(VoiceType voiceType, CharaMasterObject mChara, LocationType usageType)
        {
            var personalId = mChara.GetPersonalId();
            var personalUniqueId = mChara.GetPersonalUniqueId();
            var voiceList = charaVoiceList.FirstOrDefault(chara => chara.personalId == personalId);
            
            var locationVoice = voiceList?.GetVoiceList(voiceType, usageType, personalUniqueId);
            
            if (locationVoice == null || locationVoice.Count == 0)
            {
                return string.Empty;
            }

            var index = BattleGameLogic.GetNonStateRandomValue(0, locationVoice.Count);
            var useType = locationVoice[index].useType;
            
            return GetVoicePathForIndex(voiceType, personalId, useType, personalUniqueId);
        }
    }
}
