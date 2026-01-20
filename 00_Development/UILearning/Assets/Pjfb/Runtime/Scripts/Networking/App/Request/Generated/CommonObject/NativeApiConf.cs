//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NativeApiConf {
		public long mPointIdGem = 0; // 課金購入等で得られる通貨のID
		public long yellLimit = 0; // エール送信上限数
		public long maxGuildMemberCount = 0; // ギルドの最大人数
		public long mainMCharaId1 = 0; // 男主人公キャラのID
		public long mainMCharaId2 = 0; // 女主人公キャラのID
		public long changeUserNameMPointId = 0; // ユーザ名の変更に必要なポイントのID
		public long changeUserNameMPointValue = 0; // ユーザ名の変更に必要なポイントの量
		public long followingMaxCount = 0; // フォロー上限数
		public long uCharaVariableCountMax = 0; // 可変キャラ所持上限数
		public long uCharaVariableTrainerCountMax = 0; // トレーニング補助キャラ所持上限数
		public long trainingCardComboMultiCount = 0; // トレーニングコンボ最大同時発動回数
		public ConfGuildSearchParticipationPriorityData[] guildSearchParticipationPriorityTypeList = null; // ギルド参加優先度の検索項目リスト
		public float gachaEffectTimeUntilSkippable = 0.0f; // ガチャ演出をスキップ可能になるまでの時間（秒）
		public long[] allowDuplicateTrainingDeckCharaUseTypeList = null; // トレーニングでのキャラ重複可能なm_deck_format_useのuseTypeリスト
		public ConfBattleV2 battleV2 = null; // インゲーム関連
		public ConfColosseum colosseum = null; // 闘技場関連

   }
   
}