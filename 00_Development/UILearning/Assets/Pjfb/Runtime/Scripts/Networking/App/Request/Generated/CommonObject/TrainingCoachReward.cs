//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingCoachReward {
		public long mTrainingCoachRankId = 0; // 元となった mTrainingCoachRank の ID
		public long mTrainingEventId = 0; // 実施された mTrainingEvent の ID
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
		public long condition = 0; // コンディション（消費はマイナス、回復はプラス）
		public bool isConditionFree = false; // コンディション消費免除か
		public long maxConditionChange = 0; // 最大コンディションの増減
		public long exp = 0; // 練習種別に対して得られた経験値
		public TrainingAbility[] getAbilityMapList = null; // このイベントで得られたスキル情報を羅列したリスト。abilityFluctuationListの下位互換
		public TrainingAbilityFluctuation[] abilityFluctuationList = null; // このイベントで得られたスキル情報詳細を羅列したリスト
		public long[] addMCharaIdList = null; // このイベントから今回の個人トレーニングにサポートキャラとして追加される mChara の idList
		public long turnAddValue = 0; // 加算されたターン数
		public long coachEffectNumber = 0; // 特訓報酬の演出番号
		public long superCoachEffectNumber = 0; // 超特訓報酬の演出番号
		public long coachCharaEffectType = 0; // 特訓演出キャラの演出タイプ。 1 => m_chara_library_voice の掛け合い、2 => その他演出
		public long coachEffectMCharaIdChara = 0; // 特訓演出対象キャラID（カードタイプ：キャラのもの）。対象がない場合は-1
		public long coachEffectMCharaIdSpecialSupport = 0; // 特訓演出対象キャラID（カードタイプ：スペシャルサポートカードのもの）。対象がない場合は-1
		public long baseConditionEffectRate = 0; // 元の効果倍率
		public long coachConditionEffectRate = 0; // 特訓発生後の効果倍率
		public long superCoachConditionEffectRate = 0; // 超特訓発生後の効果倍率
		public long[] coachMCharaIdList = null; // 特訓対象となったキャラのIDリスト。先頭要素は特訓、次の要素は超特訓の対象となったキャラのIDを指す

   }
   
}