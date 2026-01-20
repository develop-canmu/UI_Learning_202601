using System;
using System.Collections;
using System.Collections.Generic;
#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
using System.Text.Json;
#endif

namespace Pjfb.InGame
{
    public class BattleAbilityActivationCondition : BattleAbilityConditionBase
    {
        public BattleConst.AbilityActivationConditionType ConditionType
        {
            get;
            private set;
        } = BattleConst.AbilityActivationConditionType.None;

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public override void SetData(Dictionary<string, object> dictionary)
        {
            base.SetData(dictionary);
            
            if (dictionary.TryGetValue(ConditionTypeKey, out var obj))
            {
                ConditionType = (BattleConst.AbilityActivationConditionType)Convert.ToInt32(obj);
            }
        }
#else
        public override void SetData(Dictionary<string, JsonElement> dictionary)
        {
            base.SetData(dictionary);
            
            if (dictionary.TryGetValue(ConditionTypeKey, out var obj))
            {
                ConditionType = (BattleConst.AbilityActivationConditionType)obj.GetInt32();
            }
        }
#endif
    }
}