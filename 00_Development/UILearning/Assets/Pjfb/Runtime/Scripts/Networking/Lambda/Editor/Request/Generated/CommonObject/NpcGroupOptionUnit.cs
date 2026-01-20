//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NpcGroupOptionUnit {
		public long mColosseumNpcId { get; set; } = 0; // NPCユニットID
		public long playerIndex { get; set; } = 0; // clientData.playerList の各 player に振られたインデックス
		public long unitNumber { get; set; } = 0; // clientData.charaList の各 chara が持つユニットの番号

   }
   
}

#endif