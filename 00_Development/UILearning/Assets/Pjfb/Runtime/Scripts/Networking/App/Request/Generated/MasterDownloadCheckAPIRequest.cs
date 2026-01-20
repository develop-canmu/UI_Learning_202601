//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
データダウンロードすべきかどうかチェックするためのAPI

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MasterDownloadCheckAPIPost : AppAPIPostBase {
		public long currentMasterVer = 0; // クライアント側のマスタバージョン（デフォルト値: 0）

   }

   [Serializable]
   public class MasterDownloadCheckAPIResponse : AppAPIResponseBase {
		public long serverMasterVer = 0; // サーバー側のマスタバージョン
		public string[] tableNames = null; // 対象テーブル名配列
		public long size = 0; // ダウンロードするデータのサイズ（未実装）

   }
      
   public partial class MasterDownloadCheckAPIRequest : AppAPIRequestBase<MasterDownloadCheckAPIPost, MasterDownloadCheckAPIResponse> {
      public override string apiName{get{ return "master-download/check"; } }
   }
}