using CruFramework.UI;
using UnityEngine;

namespace Pjfb
{ 
    public class AutoTrainingGetBoostListScrollData
    {
        public long EffectId { get; }
        public int Count { get; }
        public TrainingCharacterData CharacterData { get; }
        
        public AutoTrainingGetBoostListScrollData(long effectId, int count, TrainingCharacterData characterData = default)
        {
            EffectId = effectId;
            Count = count;
            CharacterData = characterData;
        }
    }
    
    public class AutoTrainingGetBoostListScrollItem : ScrollGridItem
    {
        [SerializeField]
        private TrainingBoostEffectView view;
        
        [SerializeField]
        private TMPro.TMP_Text countText;
        
        private AutoTrainingGetBoostListScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (AutoTrainingGetBoostListScrollData)value;
            
            view.Set(scrollData.EffectId, scrollData.CharacterData);
            countText.text = $"x{scrollData.Count}";
        }
    }
}