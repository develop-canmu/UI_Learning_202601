//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleWarField {
		public long id = 0; // $id
		public long sizeX = 0; // $sizeX 地形のX軸方向の長さ。pjshinでは味方本陣から敵本陣までの距離として定義
		public long sizeY = 0; // $sizeY 地形のY軸方向の長さ。pjshinでは味方および敵ユニットが移動するレーンの数として定義
		public long marginY = 0; // $marginY 地形のY軸方向1単位間の距離。pjshinではレーン間を移動する際の距離として定義
		public long allyRegularMilitaryStrength = 0; // $allyRegularMilitaryStrength この地形に紐付けられた味方側の規定兵力値。pjshinではこの規定兵力値が各ユニットに割り振られる
		public long enemyRegularMilitaryStrength = 0; // $enemyRegularMilitaryStrength この地形に紐付けられた敵側の規定兵力値。pjshinではこの規定兵力値が各ユニットに割り振られる

   }
   
}