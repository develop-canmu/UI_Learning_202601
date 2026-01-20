using System;
using System.Collections.Generic;

namespace Pjfb.InGame
{
    [Serializable]
    public class BattleCharacterStatModel
    {
        public enum StatType
        {
            Through,
            Pass,
            Shoot,
            Cross,
            ThroughBlock,
            PassCut,
            ShootBlock,
            SecondBall,
            HighRarityActiveSkill,
            LowRarityActiveSkill,
            PassiveSkill,
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public long CharacterId;
        public int ActivityPoint;
        public int GoalCount;
        public int GoalAssistCount;
        public int ThroughSucceedCount;
        public int ThroughFailedCount;
        public int PassSucceedCount;
        public int PassFailedCount;
        public int ShootSucceedCount;
        public int ShootFailedCount;
        public int CrossSucceedCount;
        public int CrossFailedCount;
        public int ThroughBlockSucceedCount;
        public int ThroughBlockFailedCount;
        public int PassCutSucceedCount;
        public int ShootBlockSucceedCount;
        public int SecondBallSucceedCount;
        public int SecondBallFailedCount;
        public List<long> ActivatedSkillIds = new List<long>();
        public List<int> SkillActivatedCounts = new List<int>();
#else
        public long CharacterId
        {
            get;
            set;
        } = 0;

        public int ActivityPoint
        {
            get;
            set;
        } = 0;
        
        public int GoalCount
        {
            get;
            set;
        } = 0;
        
        public int GoalAssistCount
        {
            get;
            set;
        } = 0;
        
        public int ThroughSucceedCount
        {
            get;
            set;
        } = 0;

        public int ThroughFailedCount
        {
            get;
            set;
        } = 0;

        public int PassSucceedCount
        {
            get;
            set;
        } = 0;

        public int PassFailedCount
        {
            get;
            set;
        } = 0;

        public int ShootSucceedCount
        {
            get;
            set;
        } = 0;

        public int ShootFailedCount
        {
            get;
            set;
        } = 0;

        public int CrossSucceedCount
        {
            get;
            set;
        } = 0;

        public int CrossFailedCount
        {
            get;
            set;
        } = 0;

        
        public int ThroughBlockSucceedCount
        {
            get;
            set;
        } = 0;

        public int ThroughBlockFailedCount
        {
            get;
            set;
        } = 0;

        public int PassCutSucceedCount
        {
            get;
            set;
        } = 0;

        public int ShootBlockSucceedCount
        {
            get;
            set;
        } = 0;
        
        public int SecondBallSucceedCount
        {
            get;
            set;
        } = 0;

        public int SecondBallFailedCount
        {
            get;
            set;
        } = 0;

        public List<long> ActivatedSkillIds
        {
            get;
            set;
        } = new List<long>();

        public List<int> SkillActivatedCounts
        {
            get;
            set;
        } = new List<int>();
#endif

        public BattleCharacterStatModel(long characterId)
        {
            CharacterId = characterId;
        }
        
        private const int SucceedGeneralActionActivityPoint = 100;
        private const int FailedGeneralActionActivityPoint = 50;
        private const int SucceedShootActionActivityPoint = 1000;
        private const int SucceedShootAssistActionActivityPoint = 500;
        private const int HighRarityActiveAbilityActivityPoint = 250;
        private const int LowRarityActiveAbilityActivityPoint = 100;
        private const int PassiveAbilityActivityPoint = 50;

        public void AddActivityStat(StatType statType, bool isSuccess, long abilityId = -1)
        {
            switch (statType)
            {
                case StatType.Through:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedGeneralActionActivityPoint;
                        ThroughSucceedCount++;
                    }
                    else
                    {
                        ActivityPoint += FailedGeneralActionActivityPoint;
                        ThroughFailedCount++;
                    }
                    break;
                case StatType.Pass:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedGeneralActionActivityPoint;
                        PassSucceedCount++;
                    }
                    else
                    {
                        ActivityPoint += FailedGeneralActionActivityPoint;
                        PassFailedCount++;
                    }
                    break;
                case StatType.Shoot:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedShootActionActivityPoint;
                        GoalCount++;
                        ShootSucceedCount++;
                    }
                    else
                    {
                        ActivityPoint += FailedGeneralActionActivityPoint;
                        ShootFailedCount++;
                    }
                    break;
                case StatType.Cross:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedShootAssistActionActivityPoint;
                        GoalAssistCount++;
                        CrossSucceedCount++;
                    }
                    else
                    {
                        ActivityPoint += FailedGeneralActionActivityPoint;
                        CrossFailedCount++;
                    }
                    break;
                case StatType.ThroughBlock:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedGeneralActionActivityPoint;
                        ThroughBlockSucceedCount++;
                    }
                    else
                    {
                        ActivityPoint += FailedGeneralActionActivityPoint;
                        ThroughBlockFailedCount++;
                    }
                    break;
                case StatType.PassCut:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedGeneralActionActivityPoint;
                        PassCutSucceedCount++;
                    }
                    break;
                case StatType.ShootBlock:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedGeneralActionActivityPoint;
                        ShootBlockSucceedCount++;
                    }
                    break;
                case StatType.SecondBall:
                    if (isSuccess)
                    {
                        ActivityPoint += SucceedGeneralActionActivityPoint;
                        SecondBallSucceedCount++;
                    }
                    else
                    {
                        ActivityPoint += FailedGeneralActionActivityPoint;
                        SecondBallFailedCount++;
                    }
                    break;
                case StatType.HighRarityActiveSkill:
                    ActivityPoint += HighRarityActiveAbilityActivityPoint;
                    break;
                case StatType.LowRarityActiveSkill:
                    ActivityPoint += LowRarityActiveAbilityActivityPoint;
                    break;
                case StatType.PassiveSkill:
                    ActivityPoint += PassiveAbilityActivityPoint;
                    break;
            }

            if (abilityId > 0)
            {
                AddSkillActivatedCount(abilityId);
            }
        }

        private void AddSkillActivatedCount(long abilityId)
        {
            for (var i = 0; i < ActivatedSkillIds.Count; i++)
            {
                if (ActivatedSkillIds[i] == abilityId)
                {
                    SkillActivatedCounts[i]++;
                    return;
                }
            }
            
            ActivatedSkillIds.Add(abilityId);
            SkillActivatedCounts.Add(1);
        }
    }
}