//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NativeApiItemContainer {
		public NativeApiPoint[] point = null; // ポイント情報の配列（受け渡しがない場合は null）
		public long pointPaid = -1; // 有償通貨量
		public NativeApiCharaPiece[] charaPiece = null; // キャラの欠片情報の配列（受け渡しがない場合は null）
		public long[] icon = null; // アイコンIDの配列（受け渡しがない場合は null）
		public long[] title = null; // 称号IDの配列（受け渡しがない場合は null）
		public long[] chatStamp = null; // チャットスタンプIDの配列（受け渡しがない場合は null）
		public long[] tag = null; // タグIDの配列（受け渡しがない場合は null）
		public NativeApiTag[] tagObj = null; // タグの配列（受け渡しがない場合は null）
		public long[] tagLost = null; // 失ったタグID一覧（受け渡しがない場合は null）
		public CharaV2Base[] chara = null; // キャラ情報の配列（受け渡しがない場合は null）
		public CharaVariableMinimum[] charaVariable = null; // 可変キャラ情報の配列（受け渡しがない場合は null）
		public long[] charaVariableLost = null; // 失った可変キャラのID一覧（受け渡しがない場合は null）
		public CharaVariableTrainerBase[] charaVariableTrainer = null; // トレーニング補助キャラ情報の配列（受け渡しがない場合は null）
		public long[] charaVariableTrainerLost = null; // 失ったトレーニング補助キャラのID一覧（受け渡しがない場合は null）
		public StaminaBase[] stamina = null; // スタミナ配列（受け渡しがない場合は null）
		public long[] unlockedSystemNumber = null; // 解放済みの機能番号配列（受け渡しがない場合は null）
		public NativeApiSaleIntroduction[] saleIntroductionList = null; // 解放したシークレットセールリスト（受け渡しがない場合は null）
		public long[] saleIntroductionLostList = null; // 購入により失ったシークレットセールのID一覧（受け渡しがない場合は null）
		public MissionUserAndGuild[] updatedMissionList = null; // 更新したミッションリスト（受け渡しがない場合は null）
		public NativeApiPointExpiry[] pointExpiryList = null; // ポイント有効期限リスト（受け渡しがない場合は null）
		public long[] mProfilePartIdList = null; // プロフィール用データIDリスト（受け渡しがない場合は null）

   }
   
}