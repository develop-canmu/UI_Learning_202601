//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
GameLiftサーバからのログを保存する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftSaveCloudLogAPIPost : AppAPIPostBase {
		public BattleGameliftSession session = null; // GameLiftゲームセッション登録用構造体と共通
		public BattleGameliftCloudLog[] logList = null; // ログ情報のリスト

   }

   [Serializable]
   public class BattleGameliftSaveCloudLogAPIResponse : AppAPIResponseBase {

   }
      
   public partial class BattleGameliftSaveCloudLogAPIRequest : AppAPIRequestBase<BattleGameliftSaveCloudLogAPIPost, BattleGameliftSaveCloudLogAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/saveCloudLog"; } }
   }
}