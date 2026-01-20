//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
可変キャラ一括ロック処理
今回新たにロックするキャラではなく「そのユーザーの所有するキャラ」の中でどれをロックするか指定する。
逆に言うと、指定しなかったキャラのロックはすべて外れる。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableLockBulkAPIPost : AppAPIPostBase {
		public long[] idList = null; // ロック対象キャラID

   }

   [Serializable]
   public class CharaVariableLockBulkAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CharaVariableLockBulkAPIRequest : AppAPIRequestBase<CharaVariableLockBulkAPIPost, CharaVariableLockBulkAPIResponse> {
      public override string apiName{get{ return "chara-variable/lockBulk"; } }
   }
}