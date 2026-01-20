//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルバトの負荷試験を実行するにあたって必要なパラメータ群を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetGuildBattleLoadTestParamsAPIPost : AppAPIPostBase {
		public bool skipsFleetId = false; // fleetId の取得が不要な場合は true を指定する。
		public bool skipsLoadTestGuildBattleUserStartId = false; // loadTestGuildBattleUserStartId の取得が不要な場合は true を指定する。

   }

   [Serializable]
   public class DebugGetGuildBattleLoadTestParamsAPIResponse : AppAPIResponseBase {
		public long sColosseumEventId = 0; // 現在の s_colosseum_event.id
		public string fleetId = ""; // 本日の s_battle_gamelift_group.fleetId
		public long loadTestGuildBattleUserStartId = 0; // 設定値 loadTestGuildBattleUserStartId の値

   }
      
   public partial class DebugGetGuildBattleLoadTestParamsAPIRequest : AppAPIRequestBase<DebugGetGuildBattleLoadTestParamsAPIPost, DebugGetGuildBattleLoadTestParamsAPIResponse> {
      public override string apiName{get{ return "debug/getGuildBattleLoadTestParams"; } }
   }
}