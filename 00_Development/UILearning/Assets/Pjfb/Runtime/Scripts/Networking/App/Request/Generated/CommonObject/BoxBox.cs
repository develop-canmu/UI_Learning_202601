//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BoxBox {
		public long boxNumberMin = 0; // 何箱目からこのボックスが参照されるか
		public long boxNumberMax = 0; // 何箱目までこのボックスが参照されるか
		public BoxBoxContent[] contentList = null; // 目玉商品情報のリスト

   }
   
}