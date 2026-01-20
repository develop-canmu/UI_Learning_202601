using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.LockedItem;
using Pjfb.UI;

namespace Pjfb.Training
{
    public class TrainingBoostEffectScrollItem : PoolListItemBase
    {
        
        public class BoostData : PoolListItemBase.ItemParamsBase
        {
            private long effectId = 0;
            /// <summary>mTrainingPointStatusEffect</summary>
            public long EffectId{get{return effectId;}}

            private TrainingCharacterData characterData;
            /// <summary>トレーニング中のキャラデータ</summary>
            public TrainingCharacterData CharacterData { get { return characterData; } }
            
            public BoostData(long effectId, TrainingCharacterData characterData = default)
            {
                this.effectId = effectId;
                this.characterData = characterData;
            }
        }
        
        
        [SerializeField]
        private TrainingBoostEffectView view = null;


        public override void Init(ItemParamsBase itemParamsBase)
        {
            BoostData boostData = (BoostData)itemParamsBase;
            
            view.Set(boostData.EffectId, boostData.CharacterData);
        }
    }
}