//
// This file is auto-generated
//

using System;

namespace Pjfb.Master {
   
   [Serializable]
   [MessagePack.MessagePackObject]
   public partial class PrizeJsonArgs {
		[MessagePack.Key(0)]
		public long mPointId = 0; // typeがpointの場合ある
		[MessagePack.Key(1)]
		public long mAvatarId = 0; // typeがavatarの場合ある
		[MessagePack.Key(2)]
		public long mCharaId = 0; // typeがcharaの場合ある
		[MessagePack.Key(3)]
		public long pieceMCharaId = 0; // typeがcharaPieceの場合ある
		[MessagePack.Key(4)]
		public long mSkillPartId = 0; // typeがskillPartの場合ある
		[MessagePack.Key(5)]
		public long variableMCharaId = 0; // typeがcharaVariableの場合ある
		[MessagePack.Key(6)]
		public long variableTrainerMCharaId = 0; // typeがcharaVariableTrainerの場合ある
		[MessagePack.Key(7)]
		public CharaVariableTrainerLotteryProcess lotteryProcessJson = null; // typeがcharaVariableTrainerの場合のみ設定されていればある
		[MessagePack.Key(8)]
		public long mIconId = 0; // typeがiconの場合ある
		[MessagePack.Key(9)]
		public long mTitleId = 0; // typeがtitleの場合ある
		[MessagePack.Key(10)]
		public long mChatStampId = 0; // typeがchatStampの場合ある
		[MessagePack.Key(11)]
		public long adminTagId = 0; // typeがtagの場合ある
		[MessagePack.Key(12)]
		public long mProfilePartId = 0; // typeがprofilePartの場合ある
		[MessagePack.Key(13)]
		public long mGuildTitleId = 0; // typeがguildTitleの場合ある。guild用prizeJson限定
		[MessagePack.Key(14)]
		public long mGuildEmblemId = 0; // typeがguildEmblemの場合ある。guild用prizeJson限定
		[MessagePack.Key(15)]
		public long value = 0; // 個数
		[MessagePack.Key(16)]
		public long get = 0; // 初回入手かどうか
		[MessagePack.Key(17)]
		public long correctRate = 0; // 加算補正倍率（万分率。1倍の場合は0。1.1倍の場合は1000。2倍の場合は10000。補正がない場合は何もnull）
		[MessagePack.Key(18)]
		public long valueOriginal = 0; // 補正がかかった場合のみ存在。補正が掛かる前の値
		[MessagePack.Key(19)]
		public long lockId = 0; // 時限解放アイテムの場合に設定される。m_item_locked_settingのid
		[MessagePack.Key(20)]
		public string message = ""; // 時限解放アイテムの場合に設定される。プレゼントボックスに付与する際のメッセージ

   }
   
}