//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class RushRushCategoryInfo {
		public long mGachaCategoryId = 0; // ガチャカテゴリ
		public long mGachaRushId = 0; // ガチャ確変マスタID
		public string expiredAt = ""; // 有効期限
		public long effectNumber = 0; // 演出番号
		public long imageNumber = 0; // （nativeタイトル用）rush時に表示する画像などの設定
		public long isFinished = 0; // 終了したかどうか（ガチャ実施時に、そのガチャに紐づく確変が「なかった場合」1の情報を返す） 0 => 終了してない、 1 => 終了

   }
   
}