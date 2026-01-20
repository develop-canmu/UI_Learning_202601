//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Chara {
		public long id { get; set; } = 0; // ID
		public long tableType { get; set; } = 0; // テーブル種別（1 => u_chara, 2 => u_chara_variable, 3 => m_chara_npc）
		public long charaIndex { get; set; } = 0; // バトルデータ内のキャラ区別の識別子
		public long playerIndex { get; set; } = 0; // バトルデータ内のプレイヤー区別の識別子
		public long mCharaId { get; set; } = 0; // 性能ベースキャラID
		public long parentMCharaId { get; set; } = 0; // 性能親キャラID
		public string visualKey { get; set; } = ""; // キャラIDか画像パス（キャラの見た目指定用文字列）
		public long level { get; set; } = 0; // レベル
		public string name { get; set; } = ""; // 名前
		public string nickname { get; set; } = ""; // 名前
		public long rank { get; set; } = 0; // ランク
		public long roleNumber { get; set; } = 0; // 役割番号
		public long unitNumber { get; set; } = 0; // ユニット番号
		public long actionPattern { get; set; } = 0; // 自動で行動決定する時のアクション設定値
		public long exParam1 { get; set; } = 0; // 特殊追加パラメーター1（レベル上昇等で変動しない。タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long exParam2 { get; set; } = 0; // 特殊追加パラメーター2（レベル上昇等で変動しない。タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long exParam3 { get; set; } = 0; // 特殊追加パラメーター3（レベル上昇等で変動しない。タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long hp { get; set; } = 0; // HP(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long mp { get; set; } = 0; // MP(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long atk { get; set; } = 0; // 攻撃力(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long def { get; set; } = 0; // 防御力(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long spd { get; set; } = 0; // 素早さ(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long tec { get; set; } = 0; // 技巧(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param1 { get; set; } = 0; // 追加パラメーター1(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param2 { get; set; } = 0; // 追加パラメーター2(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param3 { get; set; } = 0; // 追加パラメーター3(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param4 { get; set; } = 0; // 追加パラメーター4(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param5 { get; set; } = 0; // 追加パラメーター5(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public WrapperIntList[] abilityList { get; set; } = null; // アクティブスキルを表現するのに必要なJSON構造（abilityIdとlevelの対応情報）
		public long combatPower { get; set; } = 0;

   }
   
}

#endif