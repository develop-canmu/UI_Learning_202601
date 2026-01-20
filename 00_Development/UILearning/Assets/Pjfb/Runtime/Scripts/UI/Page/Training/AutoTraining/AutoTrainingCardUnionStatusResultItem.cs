using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingCardUnionStatusResultItem : ScrollDynamicItem
    {
        public class Argument : TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem
        {
            // レベルボーナス獲得量
            private long levelBonus;
            public long LevelBonus => levelBonus;
            
            // ステータス
            private CharacterStatus status;
            public CharacterStatus Status => status;
            
            public Argument(long levelBonus, CharacterStatus status)
            {
                this.levelBonus = levelBonus;
                this.status = status;
            }
        }
        
        [SerializeField]
        private TextMeshProUGUI levelBonusValueText = null;
        
        [SerializeField]
        private TextMeshProUGUI staminaValueText = null;
        
        [SerializeField]
        private TextMeshProUGUI speedValueText = null;
        
        [SerializeField]
        private TextMeshProUGUI physicalValueText = null;
        
        [SerializeField]
        private TextMeshProUGUI techniqueValueText = null;
        
        [SerializeField]
        private TextMeshProUGUI intelligenceValueText = null;
        
        [SerializeField]
        private TextMeshProUGUI kickValueText = null;
        
        [SerializeField]
        private OmissionTextSetter statusOmissionTextSetter = null;
        
        /// <summary>各種パラメータ獲得量をセット</summary>
        protected override void OnSetView(object value)
        {
            Argument data = (Argument)value;
            levelBonusValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_percent"], data.LevelBonus);
            staminaValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.Status.Stamina.ToDisplayString(statusOmissionTextSetter.GetOmissionData()));
            speedValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.Status.Speed.ToDisplayString(statusOmissionTextSetter.GetOmissionData()));
            physicalValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.Status.Physical.ToDisplayString(statusOmissionTextSetter.GetOmissionData()));
            techniqueValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.Status.Technique.ToDisplayString(statusOmissionTextSetter.GetOmissionData()));
            intelligenceValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.Status.Intelligence.ToDisplayString(statusOmissionTextSetter.GetOmissionData()));
            kickValueText.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.Status.Kick.ToDisplayString(statusOmissionTextSetter.GetOmissionData()));
        }
        
    }
}