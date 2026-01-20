using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.UI;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public class PracticeSkillTypeView : MonoBehaviour
    {
        [SerializeField] private PracticeSkillImage practiceSkillImage;
        [SerializeField] private TMPro.TMP_Text nameText;
        [SerializeField] private TMPro.TMP_Text valueText;
        [SerializeField] private OmissionTextSetter valueOmissionSetter;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private ScrollRect textScrollRect = null;
        [SerializeField] private RectTransform contentTransform = null;

        public void SetSkillData(PracticeSkillInfo skillData, bool showValue = true)
        {
            // スクロールを切る
            textScrollRect.enabled = false;
            
            // テクスチャ
            practiceSkillImage.SetTexture(skillData.GetIconId());
            // 名前
            nameText.text = skillData.GetName();
            // ユニークスキルの場合効果値は非表示
            valueText.gameObject.SetActive(showValue && skillData.ShowValue());
            // 効果値
            valueText.text = skillData.ToValueName(valueOmissionSetter.GetOmissionData());
            // 説明
            descriptionText.text = skillData.GetDescription();

            // スクロールできるか
            // contentをリサイズ
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
            textScrollRect.enabled = contentTransform.rect.height > textScrollRect.viewport.rect.height;
        }
    }
}