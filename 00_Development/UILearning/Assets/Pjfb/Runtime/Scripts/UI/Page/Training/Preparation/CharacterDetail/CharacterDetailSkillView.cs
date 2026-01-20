using UnityEngine;
using Pjfb.Character;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb
{

    public class CharacterDetailSkillView : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text nameText = null;
        [SerializeField] private TMPro.TMP_Text lvText = null;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private CharacterSkillRarityBaseImage rarityImage;
        [SerializeField] private RawImage skillImage;
        
        private long skillId = 0;
        private long skillLevel = 0; 
        private bool enableDetail = true;
        
        public void SetEnableDetail(bool enable)
        {
            enableDetail = enable;
        }
        
        /// <summary>名前表示</summary>
        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        /// <summary>Lv表示</summary>
        public void SetLv(long lv, bool isLvMax)
        {
            if(isLvMax)
            {
                lvText.text = StringValueAssetLoader.Instance["training.practice_card.lv_max"];
            }
            else
            {
                lvText.text = string.Format(StringValueAssetLoader.Instance["character.status.lv_value"], lv);
            }
        }
        
        public void SetIconActive(bool active)
        {
            skillImage.gameObject.SetActive(active);
        }
        
        public void SetSkill(SkillData skillData)
        {
            SetSkillId(skillData.Id, skillData.Level);
        }

        public void SetSkillId(long skillId, long lv)
        {
            this.skillId = skillId;
            skillLevel = lv;
            AbilityMasterObject mAbility = MasterManager.Instance.abilityMaster.FindData(skillId);
            nameText.text = mAbility.name;
            lvText.text =
                string.Format(StringValueAssetLoader.Instance[(lv == mAbility.maxLevel) ? "character.max_level" : "character.status.lv_value"], lv);

            if (descriptionText != null)
            {
                descriptionText.text = mAbility.description;
            }
            
            // アイコンの表示
            skillImage.gameObject.SetActive(true);
            // レアリティ表示
            SetRarity(mAbility.rarity, mAbility.CategoryEnum);
            // 詳細表示
            SetEnableDetail(true);
        }
        
        public void SetRarity(long rarity, AbilityMasterObject.AbilityCategory abilityCategory = AbilityMasterObject.AbilityCategory.Normal)
        {
            if (rarityImage != null)
            {
                // 画像設定
                rarityImage.SetTexture(rarity);
                
                // 文字色取得
                string colorKey = SkillUtility.GetSkillTextColorKey(abilityCategory);
                
                // 文字色設定
                nameText.color = ColorValueAssetLoader.Instance[colorKey];
                lvText.color = ColorValueAssetLoader.Instance[colorKey];
            }
        }
        
        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelected()
        {
            if(enableDetail == false)return;
            CharacterSkillModal.WindowParams p = new CharacterSkillModal.WindowParams(){id = skillId, level = skillLevel};
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterSkill, p);
        }
    }
}