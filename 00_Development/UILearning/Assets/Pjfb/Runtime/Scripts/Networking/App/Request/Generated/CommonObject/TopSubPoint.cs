//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TopSubPoint {
		public long id = 0; // m_category_sub_pointのid
		public long mPointId = 0; // ガチャを実行するのに必要なポイントの種別
		public long value = 0; // ガチャを実行するのに必要なポイントの量
		public long priority = 0; // 優先度
		public bool displayFlg = false; // 表示/非表示のフラグ（1 => 表示, 2 => 非表示）

   }
   
}