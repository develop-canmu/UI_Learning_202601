//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ChoiceUserChoice {
		public long mGachaChoiceElementId = 0; // セルフピックアップ枠ID
		public long[] mCommonPrizeContentIdList = null; // ユーザーが選択したピックアップキャラのコンテントID（ユーザーが選択したが、後にピックアップ対象から外れて選択できなくなるような場合があるが、そのときはそのidは選択されていないものとみなし処理する）

   }
   
}