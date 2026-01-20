//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class UserRankingUser {
		public long myRank = 0; // 自分のギルドの順位
		public string myValue = ""; // 自分のポイント数
		public RankDeckChara myChara = null; // 育成キャラ情報
		public RankDeckChara[] myDeckCharaList = null; // 自分のデッキキャラリスト
		public RankUserDeck[] deckRankList = null; // デッキ戦力ランクごとの情報
		public RankCharaVariable[] charaRankList = null; // キャラ戦力ランクごとの情報
		public RankUser[] pointRankList = null; // ポイント量ランクごとの情報
		public RankingPrize[] rankingPrizeList = null;

   }
   
}