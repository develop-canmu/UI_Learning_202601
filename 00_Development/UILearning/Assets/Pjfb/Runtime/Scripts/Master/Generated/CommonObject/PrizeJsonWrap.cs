//
// This file is auto-generated
//

using System;

namespace Pjfb.Master {
   
   [Serializable]
   [MessagePack.MessagePackObject]
   public partial class PrizeJsonWrap {
		[MessagePack.Key(0)]
		public string type = ""; // 種別区分
		[MessagePack.Key(1)]
		public string description = ""; // 何らかの説明文言(ない場合もあり)
		[MessagePack.Key(2)]
		public PrizeJsonArgs args = null;

   }
   
}