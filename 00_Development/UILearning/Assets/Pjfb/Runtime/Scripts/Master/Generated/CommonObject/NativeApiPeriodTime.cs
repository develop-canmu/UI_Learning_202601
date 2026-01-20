//
// This file is auto-generated
//

using System;

namespace Pjfb.Master {
   
   [Serializable]
   [MessagePack.MessagePackObject]
   public partial class NativeApiPeriodTime {
		[MessagePack.Key(0)]
		public string startAt = ""; // 開始時刻(hh:mm:ss形式。時・分・秒まで記載。時は2時の場合は02と記載)
		[MessagePack.Key(1)]
		public string endAt = ""; // 終了時刻(hh:mm:ss形式。時・分・秒まで記載。時は2時の場合は02と記載)

   }
   
}