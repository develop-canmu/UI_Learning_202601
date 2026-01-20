//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
様々なバージョン情報を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class VersionGetVariousVerAPIPost : AppAPIPostBase {
		public long currentMasterVer = 0; // クライアント側のマスタバージョン（デフォルト値: 0）

   }

   [Serializable]
   public class VersionGetVariousVerAPIResponse : AppAPIResponseBase {
		public long assetVer = 0; // 最新アセットバージョン
		public long serverMasterVer = 0; // サーバー側のマスタバージョン
		public long masterSize = 0; // マスタデータの通算サイズ量
		public string cdnPath = ""; // CDNのドメイン + マスタデータ格納ディレクトリのベース（fileListと結合して使用）
		public string[] masterFileList = null; // マスタデータの各ファイルパス

   }
      
   public partial class VersionGetVariousVerAPIRequest : AppAPIRequestBase<VersionGetVariousVerAPIPost, VersionGetVariousVerAPIResponse> {
      public override string apiName{get{ return "version/getVariousVer"; } }
   }
}