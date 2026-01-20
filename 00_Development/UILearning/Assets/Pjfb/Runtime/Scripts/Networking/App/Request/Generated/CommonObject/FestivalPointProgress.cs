//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalPointProgress {
		public long mFestivalId = 0; // トレーニングイベントID
		public long beforeValue = 0; // 獲得前のポイント数
		public long afterValue = 0; // 獲得後のポイント数
		public long valueDelta = 0; // 獲得ポイント量
		public FestivalOriginalPoint[] valueDeltaOriginalPointList = null; // 獲得要因と獲得ポイント量の原点の構造体。
		public FestivalSpecificCharaBonusRate[] specificCharaBonusRateList = null; // mCharaId と特定キャラによるボーナス率の構造体
		public long effectStatusBonusRate = 0; // イベント特殊効果によるボーナス率

   }
   
}