
namespace Pjfb.Master {
	public partial class RankingClientPreviewMasterObject : RankingClientPreviewMasterObjectBase, IMasterObject {  
		
		public enum TableType
		{
			/// <summary>ポイントランキング</summary>
			PointRanking = 1,
			/// <summary>ユーザーランキング</summary>
			UserRanking = 2,
		}
		
		/// <summary>
		/// ランキング対象キャラ指定の種類
		/// </summary>
		public enum TargetType
		{
			None = -1,
			MChar = 1,
			ParentMChar = 2,
			MRarity = 3
		}

		/// <summary>
		/// 開催ランキングタイプ
		/// </summary>
		public enum RankingHoldingType
		{
			RegularRanking = 1,
			EventRanking = 2,
		}

		/// <summary>
		/// 総戦力値か育成時の戦力か
		/// </summary>
		public enum TriggerType
		{
			TotalPower = 1,
			CharacterPower = 2,
		}
		
		new public RankingHoldingType holdingType{get{return (RankingHoldingType)base.holdingType;}}
		new public TableType tableType{get{return (TableType)base.tableType;}}
	}

}
