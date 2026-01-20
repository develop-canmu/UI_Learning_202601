//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TopCategory {
		public long id = 0; // m_gacha_categoryのid
		public long mCommonPrizeId = 0; // m_common_prizeのid
		public long mPointId = 0; // ガチャを実行するのに必要なポイントの種別
		public long value = 0; // ガチャを実行するのに必要なポイントの量
		public long prizeCount = 0; // 報酬数。○連ガチャ等の表記に使用
		public TopDiscount discount = null; // 割引情報
		public TopSubPoint[] subPointList = null; // 代替ポイント設定
		public bool canRush = false; // 確変があるガチャかどうか
		public TopRush rush = null; // 発生中の確変の情報
		public ModelsMAdReward mAdReward = null; // 広告マスタ

   }
   
}