//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
トレーニング補助キャラ一括ロック処理
今回新たにロックするキャラではなく「そのユーザーの所有するキャラ」の中でどれをロックするか指定する。
逆に言うと、指定しなかったキャラのロックはすべて外れる。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableTrainerLockBulkAPIPost : AppAPIPostBase {
		public long[] idList = null; // ロック対象キャラID

   }

   [Serializable]
   public class CharaVariableTrainerLockBulkAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CharaVariableTrainerLockBulkAPIRequest : AppAPIRequestBase<CharaVariableTrainerLockBulkAPIPost, CharaVariableTrainerLockBulkAPIResponse> {
      public override string apiName{get{ return "chara-variable-trainer/lockBulk"; } }
   }
}