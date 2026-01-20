//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleConquestFieldSpot {
		public long positionX { get; set; } = 0; // 拠点が配置されているX座標。左側（味方本陣）を基準とする
		public long positionY { get; set; } = 0; // 拠点が配置されているY座標。pjshinではレーン番号を指す
		public bool isBase { get; set; } = false; // 本拠地かどうか
		public long occupyingSide { get; set; } = 0; // 初期状態でどちらの陣営の拠点になっているか。どの数字がどの状態に対応するかはクライアント側で決定。（SHINGVGでは、0 => 左側陣営 1 => 右側陣営 2 => どちらでもない）
		public long index { get; set; } = 0; // 同一の occupyingSide に設定された拠点内での識別番号（1始まり）
		public long hp { get; set; } = 0; // $hp 拠点の耐久度

   }
   
}

#endif