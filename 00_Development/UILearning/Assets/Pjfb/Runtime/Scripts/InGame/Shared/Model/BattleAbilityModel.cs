using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Networking.App.Request;
#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
using System.Text.Json;
#endif

namespace Pjfb.InGame
{
    public class BattleAbilityModel
    {
        public static BattleAbilityModel Build(BattleV2Ability[] battleAbilityMasters, long abilityId , long level)
        {
            BattleAbilityModel battleAbilityModel = new BattleAbilityModel();
            BattleV2Ability? abilityMaster = battleAbilityMasters.FirstOrDefault(a => a.id == abilityId);
            if (abilityMaster != null)
            {
                battleAbilityModel.SetData(abilityMaster, level);
            }
            return battleAbilityModel;
        }
        
        public static BattleAbilityModel Build(BattleV2Ability battleAbilityMaster, long level)
        {
            BattleAbilityModel battleAbilityModel = new BattleAbilityModel();
            if (battleAbilityMaster != null)
            {
                battleAbilityModel.SetData(battleAbilityMaster, level);
            }
            return battleAbilityModel;
        }
        
        public long AbilityLevel
        {
            get;
            private set;
        }
        
        public BattleV2Ability BattleAbilityMaster
        {
            get;
            private set;
        }

        public List<BattleAbilityEffectModel> BattleAbilityEffectModels
        {
            get;
            private set;
        } = new List<BattleAbilityEffectModel>();

        public List<List<BattleAbilityActivationCondition>> ActivationConditions
        {
            get;
            private set;
        } = new List<List<BattleAbilityActivationCondition>>();

        public int ActivateCount { get; private set; }
        
        public void SetData(BattleV2Ability battleAbilityMaster, long level)
        {
            AbilityLevel = level;
            BattleAbilityMaster = battleAbilityMaster;
            ActivateCount = 0;
            ParseData();
        }
        
        private void ParseData()
        {
            if (BattleAbilityMaster == null)
            {
                return;
            }

            foreach (var abilityEffectData in BattleAbilityMaster.abilityEffectList)
            {
                var abilityEffectModel = new BattleAbilityEffectModel();
                abilityEffectModel.SetData(abilityEffectData);
                BattleAbilityEffectModels.Add(abilityEffectModel);
            }

            if (string.IsNullOrEmpty(BattleAbilityMaster.invokeCondition))
            {
                return;
            }
            
            ParseCondition();
        }

        public int GetRemainTurn()
        {
            int remainTurn = 0;
            foreach (BattleAbilityEffectModel abilityEffect in BattleAbilityEffectModels)
            {
                remainTurn = Math.Max(remainTurn, (int)abilityEffect.GetTurnCount(AbilityLevel));
            }

            return remainTurn;
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        private void ParseCondition()
        {
            var conditions = JsonUtils.Parse<List<object>>(BattleAbilityMaster.invokeCondition);
            if (conditions != null)
            {
                foreach (var conditionData in conditions)
                {
                    var activationConditionList = new List<BattleAbilityActivationCondition>();
                    if (conditionData is List<object>)
                    {
                        var conditionList = conditionData as List<object>;
                        foreach (var condition in conditionList)
                        {
                            var dict = condition as Dictionary<string, object>;
                            var activationCondition = new BattleAbilityActivationCondition();
                            activationCondition.SetData(dict);
                            activationConditionList.Add(activationCondition);
                        }
                    }
                    else if (conditionData is Dictionary<string, object>)
                    {
                        var dict = conditionData as Dictionary<string, object>;
                        var activationCondition = new BattleAbilityActivationCondition();
                        activationCondition.SetData(dict);
                        activationConditionList.Add(activationCondition);
                    }

                    if (activationConditionList.Count > 0)
                    {
                        ActivationConditions.Add(activationConditionList);
                    }
                }
            }
        }
#else
        private void ParseCondition()
        {
            var conditions = JsonSerializer.Deserialize<List<JsonElement>>(BattleAbilityMaster.invokeCondition);
            if (conditions != null)
            {
                foreach (var conditionData in conditions)
                {
                    var activationConditionList = new List<BattleAbilityActivationCondition>();
                    if (conditionData.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var obj in conditionData.EnumerateArray())
                        {
                            var dict = obj.Deserialize<Dictionary<string, JsonElement>>();
                            var activationCondition = new BattleAbilityActivationCondition();
                            activationCondition.SetData(dict);
                            activationConditionList.Add(activationCondition);
                        }
                        
                    }else if (conditionData.ValueKind == JsonValueKind.Object)
                    {
                        var dict = conditionData.Deserialize<Dictionary<string, JsonElement>>();
                        var activationCondition = new BattleAbilityActivationCondition();
                        activationCondition.SetData(dict);
                        activationConditionList.Add(activationCondition);
                    }

                    if (activationConditionList.Count > 0)
                    {
                        ActivationConditions.Add(activationConditionList);                        
                    }
                }
            }
        }
#endif

        public bool IsMeetAllCondition()
        {
            if (ActivationConditions.Count == 0)
            {
                return true;
            }
            
            foreach (var conditions in ActivationConditions)
            {
                if (conditions.All(condition => condition.IsMeetCondition))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddActivateCount()
        {
            ActivateCount++;
        }

        public bool IsRemainActivateCount()
        {
            return ActivateCount < BattleAbilityMaster.maxInvokeCount;
        }
        
        /// <summary>
        /// 0~100% (invokeRateは100倍された値=100%->10000)
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetInvokeRate(long level)
        {
            return (BattleAbilityMaster.invokeRate + ((level - 1) * BattleAbilityMaster.additionInvokeRate)) / 10000.0f;
        }

        public string GetColoredAbilityName()
        {
            return $"<color=#{BattleConst.AbilityColorCode}>{BattleAbilityMaster.name}</color>";
        }
        
        /// <summary>
        ///  汎用スキルか
        /// </summary>
        /// <returns></returns>
        public bool IsGeneralAbility()
        {
            return BattleAbilityMaster.cutInType == 1;
        }
    }
}