using UnityEngine;

namespace Pjfb.Character
{
    public class AdviserSkillView : AbilityBaseView
    {
        // アドバイザースキル情報
        private CharaAbilityInfo abilityInfo;
        // キャラレベル
        private long charaLevel = 0;
        // 解放レベル
        private long liberationLevel = 0;
        
        public void SetSkillView(CharaAbilityInfo abilityInfo, long charaLevel, long liberationLevel, bool isLevelUp)
        {
            this.abilityInfo = abilityInfo;
            this.charaLevel = charaLevel;
            this.liberationLevel = liberationLevel;
            SetView(abilityInfo, isLevelUp);
        }
        
        /// <summary> アビリティ選択時の処理 </summary>
        protected override void SelectAbility()
        {
            AdviserSkillDetailModal.Param param = new AdviserSkillDetailModal.Param(abilityInfo.AbilityType, abilityInfo.MCharaId, abilityInfo.SkillId, charaLevel, liberationLevel);
            // アドバイザースキル詳細を開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AdviserSkillDetail, param);
        }
    }
}