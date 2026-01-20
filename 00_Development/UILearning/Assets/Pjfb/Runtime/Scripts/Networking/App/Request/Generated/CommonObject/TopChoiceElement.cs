//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TopChoiceElement {
		public long id = 0; // mGachaChoiceElementId
		public long choiceCount = 0; // 選択要求数
		public long[] contentIdList = null; // 選択可能contentId一覧
		public string releasedRecently = ""; // 最も最近、新しい要素が追加された日時
		public long mCommonPrizeFrameId = 0; // mCommonPrizeFrameId（クライアント側には送られない）
		public long[] contentIdListSelected = null; // ユーザーが選択中のcontentId（クライアント側には送られない）
		public long[] defaultContentIdList = null; // ユーザーの選択が存在しないときデフォルトで選択されるcontentIdリスト（クライアント側には送られない）

   }
   
}