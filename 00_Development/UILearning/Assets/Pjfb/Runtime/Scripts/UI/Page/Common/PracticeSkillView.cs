using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
	public class PracticeSkillView : MonoBehaviour
	{
		
		[SerializeField]
		private TMPro.TMP_Text nameText = null;
		[SerializeField]
		private OmissionTextSetter valueOmissionSetter = null;
		public OmissionTextSetter ValueOmissionSetter => valueOmissionSetter;
		[SerializeField]
		private TMPro.TMP_Text valueText = null;
		
		[SerializeField]
		private TMPro.TMP_Text descriptionText = null;
		
		[SerializeField]
		private CharacterIcon characterIcon = null;
		/// <summary>アイコン</summary>
		public CharacterIcon Icon{get{return characterIcon;}}
		
		[SerializeField]
		private Image specialAttackImage = null;
		
		[SerializeField]
		private ScrollRect textScrollRect = null;
		
		[SerializeField]
		private RectTransform contentTransform = null;

		public void SetSkillData(PracticeSkillInfo skillData, long mCharaId, long uCharId, long characterLevel = -1, long liberationLevel = -1, long[] statusList = null, bool canGrowth = false)
		{
			// 最初にスクロールを切る
			textScrollRect.enabled = false;
			// 初期位置に戻す
			contentTransform.anchoredPosition = Vector2.zero;
			SetNameValue(skillData);

			characterIcon.SetStatusIdList(statusList);
			characterIcon.SetActiveRarity(true);
			characterIcon.SetActiveLv(true);
			
			// アイコンのセット
			SetCharacterIcon(new CharacterDetailData(uCharId, mCharaId, characterLevel, liberationLevel), canGrowth);

			descriptionText.text = skillData.GetDescription();
			
			// スクロールできるか
			SetTextScrollEnableAsync(destroyCancellationToken).Forget();
		}
		
		private void OnEnable()
		{
			// 初期化時にActiveが切れているとScroll判定ができないため、ここでも判定を行う
			SetTextScrollEnableAsync(destroyCancellationToken).Forget();
		}

		// テキストのスクロール判定
		private async UniTask SetTextScrollEnableAsync(CancellationToken token)
		{
			// レイアウト更新待ちの1フレーム待機
			await UniTask.Yield(token);
			
			bool isScroll = contentTransform.rect.height > textScrollRect.viewport.rect.height;
			textScrollRect.enabled = isScroll;
			// Updateより前だとAutohideが効かないのでここでActiveを切り替える
			textScrollRect.verticalScrollbar.gameObject.SetActive(isScroll);
		}
		
		/// <summary>名前</summary>
		public void SetNameValue(PracticeSkillInfo skillInfo)
		{
			nameText.text = skillInfo.GetName();
			if (skillInfo.ShowValue())
			{
				valueText.gameObject.SetActive(true);
				valueText.text = skillInfo.ToValueName(valueOmissionSetter.GetOmissionData());
			}
			else
			{
				valueText.gameObject.SetActive(false);
			}
		}
		
		/// <summary>名前</summary>
		public void SetDescription(string description)
		{
			descriptionText.text = description;
		}

		private void SetCharacterIcon(CharacterDetailData characterDetailData, bool canGrowth)
		{
			characterIcon.SetIcon(characterDetailData);
			characterIcon.CanGrowth = canGrowth;
		}
		
		public void SetIcon(long mCharId)
		{
			characterIcon.SetIcon(mCharId);
			characterIcon.SetActiveLv(false);
			characterIcon.CanGrowth = false;
		}
		
		public void SetSpecialAttackEnable(bool enable)
		{
			specialAttackImage.gameObject.SetActive(enable);
		}
	}
}

