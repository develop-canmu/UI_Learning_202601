//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NpcGroupOptionDefenseTeam {
		public long[] unitIdList { get; set; } = null; // 防衛軍隊を構成するユニット（m_colosseum_npc）のIDリスト
		public NpcGroupOptionUnit[] unitList { get; set; } = null; // 防衛軍隊を構成するユニットが、clientData 上のどのユニットから構成されるかを示すリスト
		public long spotIndex { get; set; } = 0; // 防衛軍隊が配置される拠点インデックス

   }
   
}

#endif