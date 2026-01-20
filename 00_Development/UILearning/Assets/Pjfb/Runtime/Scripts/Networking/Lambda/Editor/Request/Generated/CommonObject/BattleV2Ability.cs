//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Ability {
		public long id { get; set; } = 0; // ID
		public string name { get; set; } = ""; // ユーザー側で使用する名称
		public string description { get; set; } = ""; // 説明
		public string useMessage { get; set; } = ""; // 使用時のメッセージ
		public long mainTargetType { get; set; } = 0; // メインとなる対象種別
		public long mainEffectType { get; set; } = 0; // メインとなる効果種別
		public long mainMElementId { get; set; } = 0; // メインとなる属性ID
		public long costType { get; set; } = 0; // 消費ポイント区分（1: SP, 2: HP, 3: SP割合, 4: HP割合, 5: CP）
		public long costValue { get; set; } = 0; // ポイント消費量
		public long rarity { get; set; } = 0; // レア度
		public long timing { get; set; } = 0; // 発動タイミング
		public string invokeCondition { get; set; } = ""; // 発動条件
		public long invokeRate { get; set; } = 0; // 発動率（0~1で指定）
		public long additionInvokeRate { get; set; } = 0; // レベルによる増加発動率
		public long maxInvokeCount { get; set; } = 0; // 最大発動回数
		public long coolDownTurnCount { get; set; } = 0; // 再使用までに必要なターン数。フルネイティブタイトルのクライアント側でのみ使用
		public long abilityType { get; set; } = 0; // インゲームにおけるどのようなシーンで発動するかを識別する値。フルネイティブタイトルのクライアント側でのみ使用する想定
		public long cutInType { get; set; } = 0; // カットインアニメーション区分
		public long cutInImageId { get; set; } = 0; // カットインアニメーション用画像のid
		public long soundType { get; set; } = 0; // 音声区分
		public BattleV2AbilityEffect[] abilityEffectList { get; set; } = null; // スキル効果
		public long abilityCategory { get; set; } = 0; // アビリティカテゴリ

   }
   
}

#endif