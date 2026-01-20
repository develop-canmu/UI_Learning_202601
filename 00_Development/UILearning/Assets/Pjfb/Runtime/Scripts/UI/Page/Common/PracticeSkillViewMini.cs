using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
	public class PracticeSkillViewMini : MonoBehaviour
	{
		public enum ColorType
		{
			Normal,
			After
		}

		private enum DataType
		{
			PracticeSkillInfo,
			PracticeSkillLotteryInfo,
			PracticeSkillValueRangeInfo,
		}
		
        [SerializeField] private PracticeSkillImage practiceSkillImage;
        // キャラスキルのレアリティ下地
        [SerializeField] private CharacterSkillRarityBaseImage skillRarityBaseImage = null;
        [SerializeField] private TMPro.TMP_Text nameText = null;
        [SerializeField] private TMPro.TMP_Text valueText = null;
        [SerializeField] private OmissionTextSetter valueOmissionSetter = null;
        [SerializeField] private GameObject lockRoot;
        [SerializeField] private TMPro.TMP_Text lockText;
        [SerializeField] private GameObject highlightObject;
        [SerializeField] private PracticeSkillViewLabel practiceSkillLabel;
        [SerializeField] private UIButton button;
        
        [SerializeField] [ColorValue] private string NormalColorId = string.Empty;
        [SerializeField] [ColorValue] private string AfterColorId = string.Empty;
        
        [SerializeField] [ColorValue] private string effectUpColorId = string.Empty;
        [SerializeField] [ColorValue] private string effectDownColorId = string.Empty;
        
        private PracticeSkillInfo practiceSkillInfo;
        private DataType dataType = DataType.PracticeSkillInfo;
        
        // スキル詳細が開けるか？
        private bool CanOpenDetailModal
        {
	        get => button.enabled;
	        set => button.enabled = value;
        }
        
        
        public void SetSkillData(PracticeSkillInfo skillData, bool isLevelUp, bool isLock)
        {
	        dataType = DataType.PracticeSkillInfo;
	        CanOpenDetailModal = true;
	        SetSkillData(skillData);
	        valueText.gameObject.SetActive(skillData.ShowValue());

	        switch (skillData.MasterType)
	        {
		        case PracticeSkillMasterType.CharaTrainingStatus:
			        var charaTrainingStatusMaster = MasterManager.Instance.charaTrainingStatusMaster.FindData(skillData.MasterId);
			        nameText.color = isLevelUp && charaTrainingStatusMaster.isUnique
				        ? GetTextColor(ColorType.After)
				        : GetTextColor(ColorType.Normal);
			        lockText.text = string.Format(StringValueAssetLoader.Instance["practice_skill.lock"], charaTrainingStatusMaster.level);
					break;
		        case PracticeSkillMasterType.TrainingPointStatusEffectChara:
			        var effectCharaMaster = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(skillData.MasterId);
			        lockText.text = string.Format(StringValueAssetLoader.Instance["practice_skill.lock"], effectCharaMaster.level);
			        break;
	        }

	        lockRoot.SetActive(isLock);
	        valueText.color = isLevelUp ? GetTextColor(ColorType.After) : GetTextColor(ColorType.Normal);
	        highlightObject.SetActive(false);
        }
        
		public void SetSkillData(PracticeSkillInfo skillData, bool showHighlight)
		{
			dataType = DataType.PracticeSkillInfo;
			CanOpenDetailModal = true;
			SetSkillData(skillData);
			
			if (skillData.MasterType == PracticeSkillMasterType.CharaTrainingStatus)
			{
				var charaTrainingStatusMaster = MasterManager.Instance.charaTrainingStatusMaster.FindData(skillData.MasterId);
				nameText.color = charaTrainingStatusMaster.isUnique
					? GetTextColor(ColorType.After)
					: GetTextColor(ColorType.Normal);
				valueText.gameObject.SetActive(!charaTrainingStatusMaster.isUnique);
			}
			
			valueText.color = GetTextColor(ColorType.Normal);
			lockRoot.SetActive(false);
			highlightObject.SetActive(showHighlight);
		}

		//// <summary> スキル画像、スキル名、効果量を設定 </summary>
		private void SetSkillData(PracticeSkillInfo skillData)
		{
			practiceSkillInfo = skillData;
			practiceSkillImage.SetTexture(skillData.GetIconId());
			practiceSkillImage.SetActiveImage(true);
			nameText.text = practiceSkillInfo.GetName();
			valueText.text = skillData.ToValueName(valueOmissionSetter.GetOmissionData());
			valueText.gameObject.SetActive(true);
		}
		
		// スキル抽選時
        public void SetSkillData(PracticeSkillLotteryInfo skillLotteryInfo, bool isShowLabel = true)
        {
	        dataType = DataType.PracticeSkillLotteryInfo;
	        
	        SetSkillData(skillLotteryInfo.SkillInfo);
	        
	        //スキルが抽選対象なら
	        if (skillLotteryInfo.IsLotterySubject() && !skillLotteryInfo.IsResult)
	        {
		        switch (skillLotteryInfo.ReloadMaster.reloadType)
		        {
			        case PracticeSkillLotteryReloadType.All:
			        case PracticeSkillLotteryReloadType.SelectFrame:
				        SetLotterySelectSkillText();
				        break;
			        case PracticeSkillLotteryReloadType.SelectValue:
				        SetLotterySelectValueText();
				        break;
			        case PracticeSkillLotteryReloadType.SelectTable:
				        SetSelectTableText(skillLotteryInfo);
				        break;
		        }

		        // 再抽選対象は詳細を開けないように
		        CanOpenDetailModal = false;
	        }
	        else
	        {
		        // 抽選対象以外、結果表示の場合は詳細を開ける
		        CanOpenDetailModal = true;
	        }
	        
	        lockRoot.SetActive(false);
	        // 効果量ので色を変更する
	        valueText.color = GetTextColor(skillLotteryInfo);
	        highlightObject.SetActive(false);


	        //ラベルの設定
	        practiceSkillLabel.Init(skillLotteryInfo, isShowLabel);
        }

        /// <summary> 練習能力の効果範囲表示 </summary>
        public void SetSkillData(PracticeSkillValueRangeInfo rangeInfo, long rarityId)
        {
	        dataType = DataType.PracticeSkillValueRangeInfo;
	        CanOpenDetailModal = true;
	        practiceSkillInfo = rangeInfo.SkillInfo;
	        practiceSkillImage.SetTexture(practiceSkillInfo.GetIconId());
	        practiceSkillImage.SetActiveImage(true);
	        long rarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
	        skillRarityBaseImage.SetTexture(rarity);
	        nameText.text = rangeInfo.SkillInfo.GetName();
	        bool showValue = practiceSkillInfo.ShowValue();
	        valueText.gameObject.SetActive(showValue);
	        if (showValue)
	        {
		        // 効果値範囲
		        valueText.text = string.Format(StringValueAssetLoader.Instance["common.value_range"], rangeInfo.MinValueSkill.ToValueName(valueOmissionSetter.GetOmissionData()), rangeInfo.MaxValueSkill.ToValueName(valueOmissionSetter.GetOmissionData()));
	        }
        }

        // スキル名とスキルイメージ画像,効果値を隠す
        private void SetLotterySelectSkillText()
        {
	        nameText.text = StringValueAssetLoader.Instance["character.support_equipment_lottery_after_selectSkill"];
	        practiceSkillImage.SetActiveImage(false);
	        valueText.gameObject.SetActive(false);
        }

        // 効果値のみ隠す
        private void SetLotterySelectValueText()
        {
	        valueText.text = StringValueAssetLoader.Instance["character.support_equipment_lottery_after_selectValue"];
        }

        // テーブル抽選の場合はDetailマスターのlotteryTypeで表示を変える
        private void SetSelectTableText(PracticeSkillLotteryInfo lotteryInfo)
        {
	        switch (lotteryInfo.ReloadDetailMaster.lotteryType)
	        {
		        case PracticeSkillLotteryReloadDetailType.None:
		        case PracticeSkillLotteryReloadDetailType.SelectSkill:
			        SetLotterySelectSkillText();
			        break;
		        case PracticeSkillLotteryReloadDetailType.SelectValue:
			        SetLotterySelectValueText();
			        break;
	        }
        }

		/// <summary>
		/// UGUI
		/// </summary>
		public void OnSelected()
		{
			// 詳細が開けないならリターン
			if (CanOpenDetailModal == false)
			{
				return;
			}
			// 効果値の範囲の場合はモーダル側の値表示をオフに
			bool showValue = dataType != DataType.PracticeSkillValueRangeInfo;
			CharacterPracticeSkillModal.Data modalData = new CharacterPracticeSkillModal.Data(practiceSkillInfo, showValue);
			AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterPracticeSkill, modalData);
		}

		private Color GetTextColor(ColorType type)
		{
			Color color = default;
			switch (type)
			{
				case ColorType.Normal:
					if(ColorValueAssetLoader.Instance.HasKey(NormalColorId))
					{
						color = ColorValueAssetLoader.Instance[NormalColorId];
					}
					break;
				case ColorType.After:
					if(ColorValueAssetLoader.Instance.HasKey(AfterColorId))
					{
						color = ColorValueAssetLoader.Instance[AfterColorId];
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

			return color;
		}
		
		private Color GetTextColor(PracticeSkillLotteryInfo lotteryInfo)
		{
			Color color = default;
			switch (lotteryInfo.GetLotteryEffectType())
			{
				case SupportEquipmentLotterySkillEffectType.None:
				case SupportEquipmentLotterySkillEffectType.EffectKeep:
					if(ColorValueAssetLoader.Instance.HasKey(NormalColorId))
					{
						color = ColorValueAssetLoader.Instance[NormalColorId];
					}
					break;
				case SupportEquipmentLotterySkillEffectType.EffectUp:
					if(ColorValueAssetLoader.Instance.HasKey(effectUpColorId))
					{
						color = ColorValueAssetLoader.Instance[effectUpColorId];
					}
					break;
				case SupportEquipmentLotterySkillEffectType.EffectDown:
					if(ColorValueAssetLoader.Instance.HasKey(effectDownColorId))
					{
						color = ColorValueAssetLoader.Instance[effectDownColorId];
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return color;
		}
	}
}

