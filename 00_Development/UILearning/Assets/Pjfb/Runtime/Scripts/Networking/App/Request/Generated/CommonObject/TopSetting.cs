//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TopSetting {
		public long id = 0; // m_gacha_settingのid
		public long type = 0; // 種別（1 => 常設ガチャ, 2 => チケットガチャ）
		public long lotteryType = 0; // 抽選方式（1 => 通常抽選方式, 2 => ボックス抽選方式）
		public string name = ""; // ガチャ名称
		public string description = ""; // ガチャ説明。ガチャの訴求画像に文字を入れられない多言語版などで使用する
		public string endAt = ""; // いつまでガチャが開催されるか
		public long designNumber = 0; // デザイン番号（ガチャのバナーや訴求画像等に使用）
		public TopChoice choice = null; // セルフピックアップ設定　
		public string detailUrl = ""; // 詳細を押した際に展開されるお知らせがどれかを指定
		public long priority = 0; // 優先度番号。数字が大きいものを先に表示する
		public long enabled = 0; // 活性状態かどうか（0 => 非活性、 1 => 活性）
		public long mCommonStoreCategoryId = 0; // 紐づくショップID（紐付けない場合は0）
		public long storeMPointId = 0; // 紐づくショップIDで使用する通貨のmPointId
		public long executeLimitPersonal = 0; // ユーザーの実行回数上限。0以下の値の場合、制限無しとみなす
		public long executeCount = 0; // ユーザーの実行回数。executeLimitPersonalが入っていない場合は、基本あえて取得しない
		public long boxContentCount = 0; // ボックスガチャの場合のみ、現在のボックスに残っているアイテムの合計数が入る（ボックスガチャでない場合は-1）
		public bool canResetBox = false; // ボックスガチャの場合のみ、現在のボックスをリセット可能なら true が入る
		public TopCategory categoryMulti = null; // 複数回実行パターンのガチャ設定
		public TopCategory categorySingle = null; // 単発実行パターンのガチャ設定
		public long activationType = 0; // ガチャ活性化種別 ※クライアントには渡さなくていいが、判定には必要
		public long activationValue = 0; // ガチャ活性化設定値 ※クライアントには渡さなくていいが、判定には必要
		public string conditionJson = ""; // ガチャ活性化条件 ※クライアントには渡さなくていいが、判定には必要
		public string optionJson = ""; // ガチャ台の細かなデザインなどの設定。ガチャの訴求画像に「NEW!」などの言語依存のデザインを入れられない多言語版などで使用する
		public long disableType = 0; // 無効化区分。activationTypeと異なり、表示した上でガチャを使用不能にする必要がある場合に用いる（1 => 無効ではない、2 => カミングスーン）
		public long[] mGachaCategoryIdList = null;

   }
   
}