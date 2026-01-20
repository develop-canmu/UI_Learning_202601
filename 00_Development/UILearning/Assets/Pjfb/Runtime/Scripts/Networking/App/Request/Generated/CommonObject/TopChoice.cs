//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TopChoice {
		public long id = 0; // mGachaChoiceId
		public string releasedRecently = ""; // 最も最近、新しい要素が追加された日時（これを活用し、既読管理する）
		public long isSelected = 0; // ユーザーが必要な事項を選択済み（してない場合、ガチャ実行不可）
		public TopChoiceElement[] elementList = null; // ※クライアント側には送らない

   }
   
}