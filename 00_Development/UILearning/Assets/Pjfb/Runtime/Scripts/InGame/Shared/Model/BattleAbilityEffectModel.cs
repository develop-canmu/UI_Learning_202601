using System.Collections.Generic;
using System.Linq;
using Pjfb.Networking.App.Request;
#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
using System.Text.Json;
#endif


namespace Pjfb.InGame
{
    public class BattleAbilityEffectModel
    {
        public BattleV2AbilityEffect AbilityEffectMaster
        {
            get;
            private set;
        }

        public List<List<BattleAbilityActivationCondition>> ActivationConditions
        {
            get;
            private set;
        } = new List<List<BattleAbilityActivationCondition>>();

        public void SetData(BattleV2AbilityEffect abilityEffectMaster)
        {
            AbilityEffectMaster = abilityEffectMaster;
            ParseData();
        }
        
        // BattleAbilityModelと共通化したいけどどうすっかな.
        private void ParseData()
        {
            if (AbilityEffectMaster == null || string.IsNullOrEmpty(AbilityEffectMaster.invokeCondition))
            {
                return;
            }

            ParseCondition();
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        private void ParseCondition()
        {
            var conditions = JsonUtils.Parse<List<object>>(AbilityEffectMaster.invokeCondition);;
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
                    }else if (conditionData is Dictionary<string, object>)
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
            var conditions = JsonSerializer.Deserialize<List<JsonElement>>(AbilityEffectMaster.invokeCondition);;
            if (conditions != null)
            {
                foreach (var conditionData in conditions)
                {
                    var activationConditionList = new List<BattleAbilityActivationCondition>();
                    if (conditionData.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var condition in conditionData.EnumerateArray())
                        {
                            var dict = condition.Deserialize<Dictionary<string, JsonElement>>();
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

        public long GetTurnCount(long level)
        {
            return AbilityEffectMaster.turnCount + ((level - 1) * AbilityEffectMaster.additionTurnCount);
        }

        /// <summary>
        /// powerRateは100倍された値=100%->10000
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetEffectValue(long level)
        {
            return (AbilityEffectMaster.powerRate + ((level - 1) * AbilityEffectMaster.additionPowerRate)) / 10000.0f;
        }

        /// <summary>
        /// 0~100% (invokeRateは100倍された値=100%->10000)
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetInvokeRate(long level)
        {
            return (AbilityEffectMaster.invokeRate + ((level - 1) * AbilityEffectMaster.additionInvokeRate)) / 10000.0f;
        }
    }
}