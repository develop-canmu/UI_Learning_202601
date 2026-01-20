//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BoxCurrentBox {
		public long boxNumber = 0; // 何箱目か
		public BoxBoxContent[] contentList = null; // 商品情報のリスト
		public bool canReset = false; // リセット可能なら真

   }
   
}