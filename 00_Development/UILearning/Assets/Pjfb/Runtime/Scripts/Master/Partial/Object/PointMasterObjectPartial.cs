
namespace Pjfb.Master {
	public partial class PointMasterObject : PointMasterObjectBase, IMasterObject {  
		public enum PointType
		{
			Gem = 1, // ジェム
			Item = 2, // アイテム
			RankingPoint = 3, // ランキングポイント
			MissionClearPoint = 4, // ミッションクリアポイント
			GuildPoint = 11, // ギルドポイント
			GuildRankingPoint = 13, // ギルドランキングポイント
			ExternalPoint = 21, // 外部決済ポイント
		}
	}

}
