//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleConquestField {
		public long id { get; set; } = 0; // mBattleConquestFieldId
		public long sizeX { get; set; } = 0; // $sizeX 地形のX軸方向の長さ。pjshinでは味方本陣から敵本陣までの距離として定義
		public long sizeY { get; set; } = 0; // $sizeY 地形のY軸方向の長さ。pjshinでは味方および敵ユニットが移動するレーンの数として定義
		public long additionalMilitaryStrengthPerSpotBroken { get; set; } = 0; // $additionalMilitaryStrengthPerSpotBroken PJFBでは拠点制圧された時のボール追加量
		public long winRecoveryMilitaryStrength { get; set; } = 0; // $winRecoveryMilitaryStrength PJFBでは勝利時のボール回復量
		public WrapperIntList[] attackBonusRate { get; set; } = null; // $attackBonusRate PJFBでは拠点への攻撃ボーナス係数のjson
		public BattleV2BattleConquestFieldSpot[] battleConquestFieldSpotList { get; set; } = null; // 地形上の拠点リスト

   }
   
}

#endif