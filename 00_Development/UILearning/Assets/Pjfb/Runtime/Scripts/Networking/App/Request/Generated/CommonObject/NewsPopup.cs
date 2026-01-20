//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NewsPopup {
		public string imagePath = ""; // ポップアップに表示する画像のパス
		public string description = ""; // 説明文（バナー画像に文字を入れられない多言語版などで使用する）
		public string onClick = ""; // ポップアップ内の詳細ボタンタップ時に開くコンテンツを示す文字列
		public string endAt = ""; // ポップアップ表示期間の終了日時（クライアント側でキャッシュをいつ消すか判断するために使われる）

   }
   
}