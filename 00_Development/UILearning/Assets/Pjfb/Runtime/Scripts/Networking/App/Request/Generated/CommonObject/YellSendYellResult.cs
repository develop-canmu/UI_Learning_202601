//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class YellSendYellResult {
		public long successCount = 0; // エール送信成功人数
		public NativeApiPoint[] yellPointList = null; // 獲得エールポイントリスト
		public NativeApiPoint[] yelledPointList = null; // エール受信者の獲得エールポイントリスト
		public long[] yelledUMasterIdList = null; // エール受信者のユーザーIDリスト

   }
   
}