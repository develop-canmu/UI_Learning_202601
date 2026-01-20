
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Pjfb.Master {
	
	public enum CardType
	{
		//Todo:Noneはintに変更する際に消す
		None = -1,
		Character = 1,
		SpecialSupportCharacter = 2,
		SupportEquipment = 3,
		Adviser = 11,
	}
	
	public enum CharacterExtraType
	{
		None = -1,
		Normal = 0,
		Extra = 1,
	}

	public enum CharacterType
	{
		None = -1,
		Speed = 1,
		Technique = 2,
		Stamina = 3,
		Physical = 4,
		FieldOfView = 5,
		Kick = 6,
		Intelligence = 7,
		Condition = 8
	}

	public enum SizeType
	{
		None = 0,
		SS = 1,
		S = 2,
		M = 3,
		L = 4,
		LL = 5,
		
		SP001 = 10,
	}
	
	public partial class CharaMasterObject : CharaMasterObjectBase, IMasterObject
	{
		private static readonly string SpeedStringValueKey = "character.status.speed";
		private static readonly string TechniqueStringValueKey = "character.status.technique";
		private static readonly string StaminaStringValueKey = "character.status.stamina";
		private static readonly string PhysicalStringValueKey = "character.status.physical";
		private static readonly string FieldOfViewStringValueKey = "character.status.field_of_view";
		private static readonly string KickStringValueKey = "character.status.kick";
		private static readonly string IntelligenceStringValueKey = "character.status.intelligence";
		private static readonly string ConditionStringValueKey = "character.condition";
		
		/// <summary>キャラタイプ</summary>
		public new CharacterType charaType
		{
			get
			{
				return (CharacterType)base.charaType;
			}
		}

		private CardType _cardType = CardType.None;
		/// <summary>カードタイプ</summary>
		public new CardType cardType
		{
			get
			{
				if (_cardType != CardType.None) return _cardType;
				// TODO 後でIntになる？
				if(int.TryParse(base.cardType, out int v))
				{
					_cardType = (CardType)v;
					return _cardType;
				}

				_cardType = CardType.None;
				return _cardType;
			}
		}
		
		/// <summary>サイズタイプ</summary>
		public SizeType SizeType => (SizeType)base.mSizeId;
		
		public static string SizeToString(SizeType size)
		{
			switch (size)
			{
				case SizeType.SS:
					return "SS";
				case SizeType.S:
					return "S";
				case SizeType.M:
					return "M";
				case SizeType.L:
					return "L";
				case SizeType.LL:
					return "LL";
				case SizeType.SP001:
					return "SP0022";
				default:
					return "";
			}
		}
		

		/// <summary>キャラタイプ名取得</summary>
		public string GetCharacterTypeName()
		{
			//Todo:StringValueAssetLoaderから呼び出す
			switch (charaType)
			{
				case CharacterType.Speed:
					return StringValueAssetLoader.Instance[SpeedStringValueKey];
				case CharacterType.Technique:
					return StringValueAssetLoader.Instance[TechniqueStringValueKey];
				case CharacterType.Stamina:
					return StringValueAssetLoader.Instance[StaminaStringValueKey];
				case CharacterType.Physical:
					return StringValueAssetLoader.Instance[PhysicalStringValueKey];
				case CharacterType.FieldOfView:
					return StringValueAssetLoader.Instance[FieldOfViewStringValueKey];
				case CharacterType.Kick:
					return StringValueAssetLoader.Instance[KickStringValueKey];
				case CharacterType.Intelligence:
					return StringValueAssetLoader.Instance[IntelligenceStringValueKey];
				case CharacterType.Condition:
					return StringValueAssetLoader.Instance[ConditionStringValueKey];
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		
		public Dictionary<CharacterProfileType, string> MCharaLibraryProfileDictionary
		{
			get
			{
				if (mCharaLibraryProfileDictionary is null)
				{
					mCharaLibraryProfileDictionary = new();
					foreach (var mCharaLibraryProfile in MasterManager.Instance.charaLibraryProfileMaster.values.Where(
						         x => x.masterType == 2 && x.masterId == id))
					{
						mCharaLibraryProfileDictionary.Add((CharacterProfileType)mCharaLibraryProfile.useType, mCharaLibraryProfile.text);
					}
				}
				return mCharaLibraryProfileDictionary;
			}
		}

		private Dictionary<CharacterProfileType, string> mCharaLibraryProfileDictionary;

		
		///////// 各ステータスをラップ
		public new BigValue hp{get{return new BigValue(base.hp);}}
		public new BigValue mp{get{return new BigValue(base.mp);}}
		public new BigValue atk{get{return new BigValue(base.atk);}}
		public new BigValue def{get{return new BigValue(base.def);}}
		public new BigValue spd{get{return new BigValue(base.spd);}}
		public new BigValue tec{get{return new BigValue(base.tec);}}
		public new BigValue param1{get{return new BigValue(base.param1);}}
		public new BigValue param2{get{return new BigValue(base.param2);}}
		public new BigValue param3{get{return new BigValue(base.param3);}}
		public new BigValue param4{get{return new BigValue(base.param4);}}
		public new BigValue param5{get{return new BigValue(base.param5);}}
		public new BigValue exParam1{get{return new BigValue(base.exParam1);}}
		public new BigValue exParam2{get{return new BigValue(base.exParam2);}}
		public new BigValue exParam3{get{return new BigValue(base.exParam3);}}
		
		public long GetPersonalId()
		{
			// 人物IDの取得
			return id / 1000 % 10000;
		}
		
		public long GetPersonalUniqueId()
		{
			// 人物内IDの取得
			return id % 1000;
		}
		
		/// <summary>練習メニューカード</summary>
		public TrainingCardCharaMasterObject[] MTrainingCardCharaList => MasterManager.Instance.trainingCardCharaMaster.values
			.Where(x => x.mCharaId == id).ToArray();
		
		public long Rarity => MasterManager.Instance.rarityMaster.FindData(mRarityId).value;
		
		/// <summary>Exサポート？</summary>
		new public bool isExtraSupport{get{return base.isExtraSupport == 1;}}
	}
}
