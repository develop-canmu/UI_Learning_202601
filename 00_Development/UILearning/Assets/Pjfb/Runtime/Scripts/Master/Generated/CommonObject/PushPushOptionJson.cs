//
// This file is auto-generated
//

using System;

namespace Pjfb.Master {
   
   [Serializable]
   [MessagePack.MessagePackObject]
   public partial class PushPushOptionJson {
		[MessagePack.Key(0)]
		public long delayMinutes = 0; // 一部の区分のPUSH通知にて、PUSH送信の遅延分数設定を行う。遅延でなく前倒しで送信する場合、マイナスの値を設定する

   }
   
}