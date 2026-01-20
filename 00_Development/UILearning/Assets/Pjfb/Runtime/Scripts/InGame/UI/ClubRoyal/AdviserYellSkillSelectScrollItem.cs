using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using MagicOnion;
using Pjfb.Master;

namespace Pjfb.InGame.ClubRoyal
{ 
    public class AdviserYellSkillSelectScrollItemScrollData
    {
        public GuildBattleAbilityData guildBattleAbilityData;
    }
    
    public class AdviserYellSkillSelectScrollItem : ScrollGridItem
    {
        [SerializeField] private TMP_Text usableCountText;
        [SerializeField] private TMP_Text coolTimeText;
        [SerializeField] private AdviserSkillImage adviserSkillImage;
        [SerializeField] private GameObject coverGameObject;
        [SerializeField] private GameObject selectingGameObject;
        [SerializeField] private AdviserIcon adviserIcon;
        
        private AdviserYellSkillSelectScrollItemScrollData scrollData;
        public AdviserYellSkillSelectScrollItemScrollData ScrollData => scrollData;

        protected override void OnSetView(object value)
        {
            scrollData = (AdviserYellSkillSelectScrollItemScrollData)value;

            AbilityMasterObject abilityMaster = MasterManager.Instance.abilityMaster.FindData(scrollData.guildBattleAbilityData.AbilityId);
            if (abilityMaster != null)
            {
                adviserSkillImage.SetTextureAsync(abilityMaster.iconId).Forget();
            }

            usableCountText.text = string.Format(StringValueAssetLoader.Instance["clubroyalingame.yell_select.usable_count_format"], scrollData.guildBattleAbilityData.UsableCount);
            bool isDisplayCover = !scrollData.guildBattleAbilityData.CanUse();
            coverGameObject.SetActive(isDisplayCover);
            // Adviserアイコンとカバーの前後関係調整のため、半透明のカバーとテキストを個別に表示切替処理を行う
            coolTimeText.gameObject.SetActive(isDisplayCover);
            coolTimeText.text = scrollData.guildBattleAbilityData.CoolTime > 0
                ? StringValueAssetLoader.Instance["clubroyalingame.yell_select.icon.cooltimeformat"].Format(scrollData.guildBattleAbilityData.CoolTime)
                : "";

            adviserIcon.SetIcon(scrollData.guildBattleAbilityData.MCharaId);
            adviserIcon.SetActiveRarity(false);
            adviserIcon.SetActiveCharacterTypeIcon(false);
            adviserIcon.SetActiveLv(false);
        }

        protected override void OnSelectedItem()
        {
            selectingGameObject.SetActive(true);
        }
        
        protected override void OnDeselectItem()
        {
            selectingGameObject.SetActive(false);
        }
        
    }
}