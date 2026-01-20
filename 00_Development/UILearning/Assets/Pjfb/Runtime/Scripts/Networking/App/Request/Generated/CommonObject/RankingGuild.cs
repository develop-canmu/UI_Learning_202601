//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class RankingGuild {
		public long myRank = 0; // 自分のギルドの順位
		public long myValue = 0; // 自分のギルドのポイント数
		public long mPointId = 0; // ポイントID
		public RankGuild[] guildRankList = null; // ランクごとの情報
		public RankingPrize[] rankingPrizeList = null;
		public string guildName = ""; // 自分のギルド名
		public long mGuildEmblemId = 0; // 自分のギルドのエンブレムID

   }
   
}