//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NpcGroupOptionDefenseTeam {
		public long[] unitIdList = null; // 防衛軍隊を構成するユニット（m_colosseum_npc）のIDリスト
		public NpcGroupOptionUnit[] unitList = null; // 防衛軍隊を構成するユニットが、clientData 上のどのユニットから構成されるかを示すリスト
		public long spotIndex = 0; // 防衛軍隊が配置される拠点インデックス

   }
   
}