//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BoxBoxContent {
		public PrizeJsonWrap[] prizeList = null; // 報酬情報
		public bool isFeatured = false; // 目玉商品なら真
		public long initialCount = 0; // ボックス内の初期個数
		public long count = 0; // ボックス内の残り個数

   }
   
}