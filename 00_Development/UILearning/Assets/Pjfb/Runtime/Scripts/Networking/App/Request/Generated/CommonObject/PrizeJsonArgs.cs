//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class PrizeJsonArgs {
		public long mPointId = 0; // typeがpointの場合ある
		public long mAvatarId = 0; // typeがavatarの場合ある
		public long mCharaId = 0; // typeがcharaの場合ある
		public long pieceMCharaId = 0; // typeがcharaPieceの場合ある
		public long mSkillPartId = 0; // typeがskillPartの場合ある
		public long variableMCharaId = 0; // typeがcharaVariableの場合ある
		public long variableTrainerMCharaId = 0; // typeがcharaVariableTrainerの場合ある
		public CharaVariableTrainerLotteryProcess lotteryProcessJson = null; // typeがcharaVariableTrainerの場合のみ設定されていればある
		public long mIconId = 0; // typeがiconの場合ある
		public long mTitleId = 0; // typeがtitleの場合ある
		public long mChatStampId = 0; // typeがchatStampの場合ある
		public long adminTagId = 0; // typeがtagの場合ある
		public long mProfilePartId = 0; // typeがprofilePartの場合ある
		public long mGuildTitleId = 0; // typeがguildTitleの場合ある。guild用prizeJson限定
		public long mGuildEmblemId = 0; // typeがguildEmblemの場合ある。guild用prizeJson限定
		public long value = 0; // 個数
		public long get = 0; // 初回入手かどうか
		public long correctRate = 0; // 加算補正倍率（万分率。1倍の場合は0。1.1倍の場合は1000。2倍の場合は10000。補正がない場合は何もnull）
		public long valueOriginal = 0; // 補正がかかった場合のみ存在。補正が掛かる前の値
		public long lockId = 0; // 時限解放アイテムの場合に設定される。m_item_locked_settingのid
		public string message = ""; // 時限解放アイテムの場合に設定される。プレゼントボックスに付与する際のメッセージ

   }
   
}