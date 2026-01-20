//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumGroupMinimum {
		public long groupType = 0; // $groupType 1 => ギルド, 2 => NPCグループ（現在は1のみ対応）
		public long groupId = 0; // $groupId groupTypeに紐づくid（ギルドIDなど）
		public string name = ""; // $name 名称
		public long mGuildEmblemId = 0; // $mGuildEmblemId ギルドエンブレムID

   }
   
}