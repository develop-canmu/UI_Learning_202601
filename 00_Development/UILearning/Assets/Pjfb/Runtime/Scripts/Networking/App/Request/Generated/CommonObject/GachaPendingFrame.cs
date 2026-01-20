//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GachaPendingFrame {
		public long index = 0; // ガチャ枠そのものindexを保持
		public long retryId = 0; // リトライ設定に対応するm_gacha_retry.id
		public long count = 0; // $count リトライ済みの回数(0始まり)

   }
   
}