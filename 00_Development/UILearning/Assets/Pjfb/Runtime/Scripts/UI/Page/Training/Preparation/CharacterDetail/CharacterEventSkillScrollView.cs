using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
	
	public enum CharacterEventSkillViewType
	{
		Default, Combo, IndividualCombo
	}
	
	[RequireComponent(typeof(ScrollView))]
	public class CharacterEventSkillScrollView : MonoBehaviour
	{
		
		public enum CharacterType
		{
			None    = 0,
			Main    = 1 << 0,
			Support = 1 << 1,
			
			All = Main | Support
		}
		
		
		private class TypeViewCache
		{
			public List<CharacterDetailEventScrollDynamicItemSelector.IEventScrollDynamicItem> itemList = new List<CharacterDetailEventScrollDynamicItemSelector.IEventScrollDynamicItem>();
		}
		
		[SerializeField]
		private ScrollDynamic scrollDynamic = null;
		
		[SerializeField]
		private CharacterEventSkillSheetManager sheetManager = null;
		
		[SerializeField]
		private GameObject noEventText = null;
		
		private long mCharacterId = 0;
		private long charaLevel = 0;
		private long trainingScenarioId = 0;
		private CharacterType types = CharacterType.None;

		// 生成したビュー
		private List<CharacterDetailEventView> eventViewList = new List<CharacterDetailEventView>();
		// 生成したラベル
		private List<CharacterEventNameLabel> eventNameList = new List<CharacterEventNameLabel>();
		
		// タイプごとに分ける
		private Dictionary<CharacterEventSkillViewType, TypeViewCache> typeViewList = new Dictionary<CharacterEventSkillViewType, TypeViewCache>();
		
		public void SetCharacter(long mCharaId, long charaLevel, CharacterType types)
		{
			SetCharacter(mCharaId, charaLevel, -1, types);
		}
		
		public void SetCharacter(long mCharacterId, long charaLevel, long trainingScenarioId, CharacterType types)
		{
			typeViewList.Clear();
			
			this.mCharacterId = mCharacterId;
			this.charaLevel = charaLevel;
			this.trainingScenarioId = trainingScenarioId;
			this.types = types;
			
			// 生成済みのViewを削除
			foreach(CharacterDetailEventView view in eventViewList)
			{
				GameObject.Destroy(view.gameObject);
			}
			eventViewList.Clear();

			foreach(CharacterEventNameLabel nameLabel in eventNameList)
			{
				GameObject.Destroy(nameLabel.gameObject);				
			}
			eventNameList.Clear();
			
			// シートを開く
			sheetManager.OpenSheet( CharacterEventSkillViewType.Default, null, true );
		}
		
		public void ShowNoEventText(bool show)
		{
			noEventText.SetActive(show);
		}
		
		private void AddDictionary(SortedDictionary<long, List<EventSkillData>> eventSkillDatas, List<EventSkillData> skillList)
		{
			foreach(EventSkillData skill in skillList)
			{
				TrainingEventMasterObject mEvent = MasterManager.Instance.trainingEventMaster.FindData(skill.EventId);
				// チュートリアル用は除外
				if(mEvent.mTrainingScenarioId != 0)
				{
					TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(mEvent.mTrainingScenarioId);					
					if(mScenario.IsTutorial())continue;
				}
						
				if(eventSkillDatas.TryGetValue(mEvent.mTrainingScenarioId, out List<EventSkillData> list) == false)
				{
					list = new List<EventSkillData>();
					eventSkillDatas.Add(mEvent.mTrainingScenarioId, list);
				}
				list.Add(skill);
			}
		}
		
		public void UpdateList(CharacterEventSkillViewType type)
		{
			if(typeViewList.TryGetValue(type, out TypeViewCache viewCache) == false)
			{
				viewCache = new TypeViewCache();
				
				switch(type)
				{
					case CharacterEventSkillViewType.Default:
					{
						SortedDictionary<long, List<EventSkillData>> eventSkillDatas = new SortedDictionary<long, List<EventSkillData>>();
						if( (types & CharacterType.Support) != CharacterType.None)
						{
							AddDictionary(eventSkillDatas, SkillUtility.GetSupportCharacterEventSkillList(mCharacterId, trainingScenarioId) );
						}
			
						if( (types & CharacterType.Main) != CharacterType.None)
						{
							AddDictionary(eventSkillDatas, SkillUtility.GetCharacterEventSkillList(mCharacterId, trainingScenarioId) );
						} 
						
						foreach(KeyValuePair<long, List<EventSkillData>> skillDic in eventSkillDatas)
						{
							// イベント名
							if(skillDic.Key == 0)
							{
								viewCache.itemList.Add(new CharacterDetailEventNameScrollDynamicItem.Param(StringValueAssetLoader.Instance["training.common_scenario"]));
							}
							else
							{
								TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(skillDic.Key);
								viewCache.itemList.Add(new CharacterDetailEventNameScrollDynamicItem.Param(mTrainingScenario.name) );
							}
							
							foreach(EventSkillData skill in skillDic.Value)
							{
								TrainingEventMasterObject mEvent = MasterManager.Instance.trainingEventMaster.FindData(skill.EventId);
								if(mEvent.mTrainingUnitId != 0)continue;
								// 強化後のイベントは除外
								if (IsRootEnhanceEvent(mEvent, type) == false) continue;

								CharacterDetailEventScrollDynamicItem.Param param = new CharacterDetailEventScrollDynamicItem.Param(mCharacterId, charaLevel, skill, TrainingUnitComboType.None);
								viewCache.itemList.Add(param);
							}
						}
						
						break;
					}
					
					case CharacterEventSkillViewType.IndividualCombo:
					case CharacterEventSkillViewType.Combo:
					{
						// 親Id
						long parentId = CharacterUtility.CharIdToParentId(mCharacterId);

						List<TrainingUnitMasterObject> unitList = new List<TrainingUnitMasterObject>();
						SortedDictionary<long, List<EventSkillData>> eventSkillDatas = new SortedDictionary<long, List<EventSkillData>>();
						
						foreach(TrainingUnitElementMasterObject mUnitElement in MasterManager.Instance.trainingUnitElementMaster.values)
						{
							switch(mUnitElement.type)
							{
								case TrainingUnitElementIdType.ParentId:
								{
									if(parentId != mUnitElement.masterId)continue;
									break;
								}
								case TrainingUnitElementIdType.MCharId:
								{
									if(mCharacterId != mUnitElement.masterId)continue;
									break;
								}
							}
							
							// mUnit
							TrainingUnitMasterObject mUnit = MasterManager.Instance.trainingUnitMaster.FindData(mUnitElement.mTrainingUnitId);
							unitList.Add(mUnit);							
						}
						
						foreach(TrainingUnitMasterObject mUnit in unitList)
						{
							switch(type)
							{
								case CharacterEventSkillViewType.Combo:
									if(mUnit.type != TrainingUnitComboType.Common)continue;
									break;
								case CharacterEventSkillViewType.IndividualCombo:
									if(mUnit.type != TrainingUnitComboType.Individual)continue;
									break;
							}
							
							AddDictionary(eventSkillDatas, SkillUtility.GetUnitEventList(mUnit.id, trainingScenarioId));
						}
						
						TrainingUnitComboType comboType = TrainingUnitComboType.None;

						switch(type)
						{
							case CharacterEventSkillViewType.Combo:
								comboType = TrainingUnitComboType.Common;
								break;
							case CharacterEventSkillViewType.IndividualCombo:
								comboType = TrainingUnitComboType.Individual;
								break;
						}
						
						foreach(KeyValuePair<long, List<EventSkillData>> skillDic in eventSkillDatas)
						{
							// イベント名
							if(skillDic.Key == 0)
							{
								viewCache.itemList.Add(new CharacterDetailEventNameScrollDynamicItem.Param(StringValueAssetLoader.Instance["training.common_scenario"]));
							}
							else
							{
								TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(skillDic.Key);
								viewCache.itemList.Add(new CharacterDetailEventNameScrollDynamicItem.Param(mTrainingScenario.name));
							}
							
							foreach(EventSkillData skill in skillDic.Value)
							{
								TrainingEventMasterObject mEvent = MasterManager.Instance.trainingEventMaster.FindData(skill.EventId);
								// 強化後のイベントは除外
								if (IsRootEnhanceEvent(mEvent, type) == false) continue;

								CharacterDetailEventScrollDynamicItem.Param param = new CharacterDetailEventScrollDynamicItem.Param(mCharacterId, charaLevel, skill, comboType);
								viewCache.itemList.Add(param);
							}
						}
												
						break;
					}
				}

				typeViewList.Add(type, viewCache);
			}
			
			// イベントなし
			ShowNoEventText( viewCache.itemList.Count == 0 );
			
			// スクロールDynamicにセット
			scrollDynamic.SetItems(viewCache.itemList);
		}
		
		/// <summary>
		/// 該当のイベントが強化イベントである場合、強化前のイベントであるか
		/// </summary>
		private bool IsRootEnhanceEvent(TrainingEventMasterObject mEvent, CharacterEventSkillViewType type)
		{
			// 強化イベントが設定されていない
			if (mEvent.enhanceGroup <= 0) return true;
			
			// 同一強化グループのイベントリスト
			var enhanceEvents = MasterManager.Instance.trainingEventMaster.values
				.Where(e => e.enhanceGroup == mEvent.enhanceGroup);
			
			bool isRoot = true;
			
			switch (type)
			{
				case CharacterEventSkillViewType.Default:
				case CharacterEventSkillViewType.Combo:
					// 強化グループの中で最小レベル条件のイベントであるか
					isRoot = enhanceEvents
						.OrderBy(e => e.minLevel)
						.First().id == mEvent.id;
					break;
				case CharacterEventSkillViewType.IndividualCombo:
					// 個別コンボの組み合わせの中で一番低いレベルの条件のイベントであるか
					isRoot = enhanceEvents.OrderBy(e =>
						{
							return MasterManager.Instance.trainingUnitElementMaster.FindDataByUnitId(e.mTrainingUnitId)
								.Where(ue => ue.type == TrainingUnitElementIdType.MCharId).Min(u => u.minLevel);
						})
						.First().id == mEvent.id;
					break;
			}

			return isRoot;
		}
	}
}