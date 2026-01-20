//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
クライアントのリクエスト通りにアクションログを作成する

基盤の処理等で例外が発生しない限りは、常にステータスコードを 0 で返します。
ログの保存に失敗した場合もエラーレスポンスを返しません。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class LogRegisterLogAPIPost : AppAPIPostBase {
		public long logType = 0; // ログ種別（200000000 ～ 299999999 の範囲で指定する）
		public string message = ""; // ログ情報（u_action_log.message に保存したい文字列）

   }

   [Serializable]
   public class LogRegisterLogAPIResponse : AppAPIResponseBase {

   }
      
   public partial class LogRegisterLogAPIRequest : AppAPIRequestBase<LogRegisterLogAPIPost, LogRegisterLogAPIResponse> {
      public override string apiName{get{ return "log/registerLog"; } }
   }
}