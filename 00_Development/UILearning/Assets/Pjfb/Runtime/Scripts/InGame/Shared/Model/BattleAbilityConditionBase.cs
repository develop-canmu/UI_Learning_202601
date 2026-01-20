using System;
using System.Collections;
using System.Collections.Generic;
#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
using System.Text.Json;
#endif

namespace Pjfb.InGame
{
    public class BattleAbilityConditionBase
    {
        public bool IsMeetCondition = false;
        
        protected const string ConditionTypeKey = "type";
        
        private const string MinValueKey = "min";

        public BigValue MinValue
        {
            get;
            private set;
        }

        private const string MaxValueKey = "max";

        public BigValue MaxValue
        {
            get;
            private set;
        }
        
        private const string StatusTypeKey = "status"; 
        public BattleConst.StatusParamType StatusType
        {
            get;
            private set;
        }


        private const string SameCharaIdsKey = "sameCharaIds"; 
        public List<long> SameCharaIds
        {
            get;
            private set;
        } = new List<long>();

        private const string PositionsKey = "positions"; 
        public List<BattleConst.PlayerPosition> PlayerPositions
        {
            get;
            private set;
        } = new List<BattleConst.PlayerPosition>();
        
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public virtual void SetData(Dictionary<string, object> dictionary)
        {
            object obj;
            
            if (dictionary.TryGetValue(MinValueKey, out obj))
            {
                string strObj = Convert.ToString(obj);
                MinValue = new BigValue(strObj);
            }
            
            if (dictionary.TryGetValue(MaxValueKey, out obj))
            {
                string strObj = Convert.ToString(obj);
                MaxValue = new BigValue(strObj);
            }

            if (dictionary.TryGetValue(StatusTypeKey, out obj))
            {
                StatusType = (BattleConst.StatusParamType)Convert.ToInt64(obj);
            }

            if (dictionary.TryGetValue(SameCharaIdsKey, out obj))
            {
                var list = (List<object>)obj;
                if (list != null)
                {
                    foreach (var sameCharaId in list)
                    {
                        SameCharaIds.Add(Convert.ToInt64(sameCharaId));
                    }
                }
            }
            
            if (dictionary.TryGetValue(PositionsKey, out obj))
            {
                var list = (List<object>)obj;
                if (list != null)
                {
                    foreach (var position in list)
                    {
                        PlayerPositions.Add((BattleConst.PlayerPosition)Convert.ToInt16(position));
                    }
                }
            }
        }
#else
        public virtual void SetData(Dictionary<string, JsonElement> dictionary)
        {
            JsonElement obj;

            if (dictionary.TryGetValue(MinValueKey, out obj))
            {
                MinValue = new BigValue(obj.ToString());
            }
            
            if (dictionary.TryGetValue(MaxValueKey, out obj))
            {
                MaxValue = new BigValue(obj.ToString());
            }

            if (dictionary.TryGetValue(StatusTypeKey, out obj))
            {
                StatusType = (BattleConst.StatusParamType)obj.GetInt64();
            }

            if (dictionary.TryGetValue(SameCharaIdsKey, out obj))
            {
                var list = obj.EnumerateArray();
                foreach (var sameCharaId in list)
                {
                    SameCharaIds.Add(sameCharaId.GetInt64());
                }
            }
            
            if (dictionary.TryGetValue(PositionsKey, out obj))
            {
                var list = obj.EnumerateArray();
                foreach (var position in list)
                {
                    PlayerPositions.Add((BattleConst.PlayerPosition)position.GetInt16());
                }
            }
        }
#endif

        public bool IsMeetMinMaxValue(BigValue value)
        {
            return IsMeetMinValue(value) && IsMeetMaxValue(value);
        }

        private bool IsMeetMinValue(BigValue value)
        {
            return MinValue.Value <= value.Value;
        }
        
        private bool IsMeetMaxValue(BigValue value)
        {
            return value.Value <= MaxValue.Value;
        }

    }
}