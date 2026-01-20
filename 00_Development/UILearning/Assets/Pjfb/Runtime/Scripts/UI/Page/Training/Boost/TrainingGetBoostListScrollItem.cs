using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingGetBoostListScrollItem : ScrollGridItem
    {
        public class TrainingGetBoostListScrollData
        {
            public long EffectId { get; }
            public TrainingCharacterData CharacterData { get; }

            public TrainingGetBoostListScrollData(long effectId, TrainingCharacterData characterData = default)
            {
                EffectId = effectId;
                CharacterData = characterData;
            }
        }

        private TrainingGetBoostListScrollData scrollData;

        [SerializeField] private TrainingBoostEffectView view = null;

        protected override void OnSetView(object value)
        {
            scrollData = (TrainingGetBoostListScrollData)value;
            
            view.Set(scrollData.EffectId, scrollData.CharacterData);
        }
    }
}