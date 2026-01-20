//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class UserRankingGuild {
		public long myRank = 0; // 自分のギルドの順位
		public string myValue = ""; // 自分のギルドのポイント数
		public string guildName = ""; // 自分のギルド名
		public long mGuildEmblemId = 0; // 自分のギルドのエンブレムID
		public long mGuildRankId = 0; // 自分のギルドのランクID
		public RankGuild[] guildRankList = null; // ランクごとの情報
		public RankingPrize[] rankingPrizeList = null;

   }
   
}