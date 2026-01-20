
using System;

namespace Pjfb.Master {
	public partial class AbilityMasterObject : AbilityMasterObjectBase, IMasterObject
	{
		/// <summary> スキルの種類 </summary>
		public enum AbilityCategory
		{
			// 通常スキル
			Normal = 1,
			// FLOWスキル
			Flow = 2,
		}
		// enum変換プロパティ
		public AbilityCategory CategoryEnum
		{
			get
			{
				// 存在するenum値であれば返す
				if (Enum.IsDefined(typeof(AbilityCategory), (int)abilityCategory))
				{
					return (AbilityCategory)abilityCategory;	
				}
				
				// 未実装の場合の例外処理
				throw new ArgumentOutOfRangeException(nameof(abilityCategory), abilityCategory, $"AbilityCategory:{abilityCategory} は未実装です");
			}
		}
		
		public BattleConst.AbilityType AbilityType
		{
			get
			{
				// 存在しないAbilityTypeはNoneとして返す
				if (Enum.IsDefined(typeof(BattleConst.AbilityType), (int) abilityType))
				{
					return (BattleConst.AbilityType)abilityType;	
				}

				return BattleConst.AbilityType.None;
			}
		} 
	}

}
