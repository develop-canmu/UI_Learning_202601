using System;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.UI;
using Pjfb.Master;

namespace Pjfb.InGame
{
    public class InGameStatsDetailSkillCountUI : ScrollGridItem
    {
        [SerializeField] private UIButton uiButton;
        [SerializeField] private CharacterSkillRarityBaseImage rarityImage;
        
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillLevelText;
        [SerializeField] private TextMeshProUGUI activateCountText;
        [SerializeField] private Image[] lineImages;

        private long abilityId;
        private long abilityLevel;

        private void Awake()
        {
            uiButton.onClick.AddListener(OnClickCell);
        }

        protected override void OnSetView(object value)
        {
            var skillData = value as Tuple<BattleV2Ability, long, long>;
            if (skillData == null)
            {
                return;
            }
            
            // レアリティ表示
            rarityImage.SetTexture(skillData.Item1.rarity);
            
            // 文字色取得
            string colorKey = SkillUtility.GetSkillTextColorKey((AbilityMasterObject.AbilityCategory)skillData.Item1.abilityCategory);

            // 文字色設定
            skillNameText.color = ColorValueAssetLoader.Instance[colorKey];
            skillLevelText.color = ColorValueAssetLoader.Instance[colorKey];
            activateCountText.color = ColorValueAssetLoader.Instance[colorKey];
            
            // 区切り線の色設定
            foreach (var line in lineImages)
            {
                line.color = ColorValueAssetLoader.Instance[colorKey];
            }
            
            // 表示文字変更
            skillNameText.text = skillData.Item1.name;
            skillLevelText.text = skillData.Item2.ToString();
            activateCountText.text = skillData.Item3.ToString();

            abilityId = skillData.Item1.id;
            abilityLevel = skillData.Item2;
        }

        private void OnClickCell()
        {
            var p = new CharacterSkillModal.WindowParams(){id = abilityId, level = abilityLevel};
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterSkill, p);
        }
    }
}