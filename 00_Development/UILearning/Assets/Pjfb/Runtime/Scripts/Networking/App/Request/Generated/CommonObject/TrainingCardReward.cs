//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingCardReward {
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
		public long conditionMin = 0; // コンディション変動最小（消費はマイナス、回復はプラス）
		public long conditionMax = 0; // コンディション変動最大（消費はマイナス、回復はプラス）
		public long conditionFreeRate = 0; // コンディション変動が0となる確率（万分率）
		public long exp = 0; // その練習種別に対して得られる経験値量
		public long[] coachRankRewardIdList = null; // 当選したmTrainingCoachRankRewardのIDリスト
		public long[] coachRankIdList = null; // 当選したmTrainingCoachRankのIDリスト
		public long[] coachEffectNumber = null; // 当選したcoachEffectNumber
		public long[] superCoachEffectNumber = null; // 当選したcoachEffectNumber
		public long coachEffectMCharaIdChara = 0; // 特訓演出対象キャラID（カードタイプ：キャラのもの）。対象がない場合は-1
		public long coachEffectMCharaIdSpecialSupport = 0; // 特訓演出対象キャラID（カードタイプ：スペシャルサポートカードのもの）。対象がない場合は-1
		public long coachCharaEffectType = 0; // 特訓演出キャラの演出タイプ
		public long[] coachMCharaIdList = null; // 特訓対象となったキャラのIDリスト
		public long[] mTrainingCardComboIdList = null; // 発動したカードコンボIDリスト
		public long concentrationExp = 0; // 獲得可能なコンセントレーションのexp

   }
   
}