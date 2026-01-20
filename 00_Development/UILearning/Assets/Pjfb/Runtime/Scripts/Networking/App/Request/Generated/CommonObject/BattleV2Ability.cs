//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Ability {
		public long id = 0; // ID
		public string name = ""; // ユーザー側で使用する名称
		public string description = ""; // 説明
		public string useMessage = ""; // 使用時のメッセージ
		public long mainTargetType = 0; // メインとなる対象種別
		public long mainEffectType = 0; // メインとなる効果種別
		public long mainMElementId = 0; // メインとなる属性ID
		public long costType = 0; // 消費ポイント区分（1: SP, 2: HP, 3: SP割合, 4: HP割合, 5: CP）
		public long costValue = 0; // ポイント消費量
		public long rarity = 0; // レア度
		public long timing = 0; // 発動タイミング
		public string invokeCondition = ""; // 発動条件
		public long invokeRate = 0; // 発動率（0~1で指定）
		public long additionInvokeRate = 0; // レベルによる増加発動率
		public long maxInvokeCount = 0; // 最大発動回数
		public long coolDownTurnCount = 0; // 再使用までに必要なターン数。フルネイティブタイトルのクライアント側でのみ使用
		public long abilityType = 0; // インゲームにおけるどのようなシーンで発動するかを識別する値。フルネイティブタイトルのクライアント側でのみ使用する想定
		public long cutInType = 0; // カットインアニメーション区分
		public long cutInImageId = 0; // カットインアニメーション用画像のid
		public long soundType = 0; // 音声区分
		public BattleV2AbilityEffect[] abilityEffectList = null; // スキル効果
		public long abilityCategory = 0; // アビリティカテゴリ

   }
   
}