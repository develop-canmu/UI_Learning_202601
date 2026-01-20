//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ModelsUChat {
		public long id = 0;
		public string createdAt = ""; // レコード登録日時
		public string updatedAt = ""; // レコード更新日時
		public long uMasterId = 0; // チャット受け取りユーザーID
		public long fromUMasterId = 0; // チャット送信ユーザーID
		public long readFlg = 0; // チャットを読んだかどうかのフラグ 1=>既読 2=>未読
		public long type = 0; // チャットの種別 1=>メッセージ 2=>スタンプ 3=>システムメッセージ
		public string body = ""; // メッセージ
		public long deleteFlg = 0; // 削除フラグ

   }
   
}