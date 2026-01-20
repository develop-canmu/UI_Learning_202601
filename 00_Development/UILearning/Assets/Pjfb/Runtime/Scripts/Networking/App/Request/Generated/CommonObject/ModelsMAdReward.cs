//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ModelsMAdReward {
		public long id = 0; // $id
		public long type = 0; // $type 設置箇所の種別。1=PVP 2=放置 3=ガチャ 4=汎用 1001=ガチャ(mGachaCategoryId) 1002=billingRewardBonus 1003=commonStore 1004=自動トレーニング 1005=スタミナ回復（1002以降は未実装）1001以降はnativeタイトルでの運用を想定～1000は広告閲覧後受取待機状態になり、受取操作で実際に付与される（4は例外）。1001～は閲覧直後に受取操作まで行われる
		public long paramId = 0; // $paramId typeによって用途が変わるID<br />ガチャ(type=3)ならMGachaSettingId<br />汎用/PVP(type=1or2or4)なら0ガチャカテゴリ(type=1001)ならmGachaCategoryId
		public long intervalSecond = 0; // $intervalSecond 次までのインターバル秒数
		public long limit = 0; // $limit デイリーリミット
		public string startAt = ""; // $startAt 開始日時（空であれば制限なし）
		public string endAt = ""; // $endAt 終了日時（空であれば制限なし）
		public string prizeJson = ""; // $prizeJson "報酬内容<br />汎用ページ(type=4)でしか使わない"
		public string adminTagIdListCanSkip = ""; // $adminTagIdListCanSkip スキップ可能タグIDリスト。<br />ユーザーが配列に内包されたタグのいずれかを所有していれば、広告を閲覧せずに報酬が獲得可能。例：[1,2,3]
		public long clientHandlingType = 0; // $clientHandlingType クライアント側でどの機能・コンテンツとして取り扱うかの分岐
		public bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除
		public string createdAt = ""; // $createdAt レコード登録日時
		public string updatedAt = ""; // $updatedAt レコード更新日時

   }
   
}