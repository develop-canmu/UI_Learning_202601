//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class CharaVariableMinimum {
		public long id = 0; // ID
		public long uMasterId = 0; // ユーザID
		public long mCharaId = 0; // 性能ベースキャラID
		public string visualKey = ""; // キャラIDか画像パス（キャラの見た目指定用）
		public long hp = 0; // hp
		public long mp = 0; // mp
		public long atk = 0; // atk
		public long def = 0; // def
		public long spd = 0; // spd
		public long tec = 0; // tec
		public long param1 = 0; // param1
		public long param2 = 0; // param2
		public long param3 = 0; // param3
		public long param4 = 0; // param4
		public long param5 = 0; // param5
		public long combatPower = 0; // 総合力
		public long rank = 0; // ランク
		public long mTrainingScenarioId = 0; // キャラを作成した際のトレーニングシナリオID
		public bool isLocked = false; // お気に入りロック
		public WrapperIntList[] abilityList = null; // アクティブスキルを表現するのに必要なJSON構造
		public WrapperIntList[] abilityPassiveList = null; // パッシブスキルを表現するのに必要なJSON構造
		public TrainingSupport[] supportDetailJson = null; // キャラを作成した際のサポートの情報を JSON 化して保存する
		public CombinationOpenedMinimum[] comboBuffList = null; // スキルコネクト発動情報

   }
   
}