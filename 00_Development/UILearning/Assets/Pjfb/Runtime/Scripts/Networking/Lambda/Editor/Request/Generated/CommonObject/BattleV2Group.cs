//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Group {
		public long groupId { get; set; } = 0; // グループID（DB上に保存される際のID）。groupTypeによって意味合いが変化。
		public long groupType { get; set; } = 0; // グループ種別(1 => ギルド（g_master）、2 => ダミー。PVPやPVEなどの本来グループ情報が不要な区分で便宜上設けるデータ、3 => NPCギルド（m_colosseum_npc_guild）)
		public long groupIndex { get; set; } = 0; // バトルデータ内の所属グループ区別用の識別子
		public string name { get; set; } = ""; // 名称
		public long mGuildEmblemId { get; set; } = 0; // ギルドエンブレムID
		public NpcGroupOptionOption npcGroupOption { get; set; } = null; // NPCグループの行動ロジックの指定に必要な情報

   }
   
}

#endif