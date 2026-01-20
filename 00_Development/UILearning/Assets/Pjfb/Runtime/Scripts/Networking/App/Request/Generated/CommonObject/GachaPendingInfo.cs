//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GachaPendingInfo {
		public long uGachaResultPendingId = 0; // リトライ保留情報のID
		public long mGachaSettingId = 0; // m_gacha_setting.id
		public long gachaSettingType = 0; // m_gacha_setting.type
		public long storeMPointId = 0; // ガチャに紐づくショップで使用する通貨のmPointId
		public GachaPendingFrame[] frameList = null; // 各枠の保留情報
		public string expireAt = ""; // いつまで引き直しが実施できるか
		public WrapperPrizeList[] contentListList = null;
		public long mGachaCategoryId = 0; // m_gacha_category.id

   }
   
}