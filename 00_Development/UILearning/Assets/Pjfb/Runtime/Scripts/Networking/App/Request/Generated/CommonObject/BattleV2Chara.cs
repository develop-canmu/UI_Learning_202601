//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Chara {
		public long id = 0; // ID
		public long tableType = 0; // テーブル種別（1 => u_chara, 2 => u_chara_variable, 3 => m_chara_npc）
		public long charaIndex = 0; // バトルデータ内のキャラ区別の識別子
		public long playerIndex = 0; // バトルデータ内のプレイヤー区別の識別子
		public long mCharaId = 0; // 性能ベースキャラID
		public long parentMCharaId = 0; // 性能親キャラID
		public string visualKey = ""; // キャラIDか画像パス（キャラの見た目指定用文字列）
		public long level = 0; // レベル
		public string name = ""; // 名前
		public string nickname = ""; // 名前
		public long rank = 0; // ランク
		public long roleNumber = 0; // 役割番号
		public long unitNumber = 0; // ユニット番号
		public long actionPattern = 0; // 自動で行動決定する時のアクション設定値
		public long exParam1 = 0; // 特殊追加パラメーター1（レベル上昇等で変動しない。タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long exParam2 = 0; // 特殊追加パラメーター2（レベル上昇等で変動しない。タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long exParam3 = 0; // 特殊追加パラメーター3（レベル上昇等で変動しない。タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long hp = 0; // HP(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long mp = 0; // MP(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long atk = 0; // 攻撃力(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long def = 0; // 防御力(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long spd = 0; // 素早さ(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long tec = 0; // 技巧(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param1 = 0; // 追加パラメーター1(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param2 = 0; // 追加パラメーター2(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param3 = 0; // 追加パラメーター3(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param4 = 0; // 追加パラメーター4(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public long param5 = 0; // 追加パラメーター5(タイトルごとに意味合い変動。使用しない場合はnullが入る）
		public WrapperIntList[] abilityList = null; // アクティブスキルを表現するのに必要なJSON構造（abilityIdとlevelの対応情報）
		public long combatPower = 0;

   }
   
}