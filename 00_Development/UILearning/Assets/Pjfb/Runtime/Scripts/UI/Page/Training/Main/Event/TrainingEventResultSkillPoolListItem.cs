using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UI;

namespace Pjfb
{
    public class TrainingEventResultSkillPoolListItem : PoolListItemBase
    {
        public class TrainingEventResultSkillPoolListItemParam : ItemParamsBase
        {
            public long SkillId { get; }
            public long SkillLevel { get; }
            public bool IsBoostOnly { get; }
            public TrainingEventResultSkillPoolListItemParam(long skillId, long skillLevel, bool isBoostOnly)
            {
                SkillId = skillId;
                SkillLevel = skillLevel;
                IsBoostOnly = isBoostOnly;
            }
        }
        
        [SerializeField]
        private CharacterDetailSkillView skillView;
        [SerializeField]
        private GameObject boostOnlyIcon;

        private TrainingEventResultSkillPoolListItemParam _itemParam;
        
        public override void Init(ItemParamsBase itemParamsBase)
        {
            _itemParam = (TrainingEventResultSkillPoolListItemParam)itemParamsBase;
            skillView.SetSkillId(_itemParam.SkillId, _itemParam.SkillLevel);
            boostOnlyIcon.SetActive(_itemParam.IsBoostOnly);
        }
    }
}