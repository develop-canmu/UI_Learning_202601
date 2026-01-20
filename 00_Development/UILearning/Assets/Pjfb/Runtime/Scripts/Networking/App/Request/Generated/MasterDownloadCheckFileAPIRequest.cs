//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ダウンロードすべきファイル一覧を取得するためのAPI

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MasterDownloadCheckFileAPIPost : AppAPIPostBase {
		public long currentMasterVer = 0; // クライアント側のマスタバージョン（デフォルト値: 0）

   }

   [Serializable]
   public class MasterDownloadCheckFileAPIResponse : AppAPIResponseBase {
		public long serverMasterVer = 0; // サーバー側のマスタバージョン
		public long size = 0; // マスタデータの通算サイズ量
		public string cdnPath = ""; // CDNのドメイン + マスタデータ格納ディレクトリのベース（fileListと結合して使用）
		public string[] fileList = null; // マスタデータの各ファイルパス

   }
      
   public partial class MasterDownloadCheckFileAPIRequest : AppAPIRequestBase<MasterDownloadCheckFileAPIPost, MasterDownloadCheckFileAPIResponse> {
      public override string apiName{get{ return "master-download/checkFile"; } }
   }
}