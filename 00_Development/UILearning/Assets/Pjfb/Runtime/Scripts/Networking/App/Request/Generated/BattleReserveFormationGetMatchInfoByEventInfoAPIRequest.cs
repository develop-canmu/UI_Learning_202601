//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
試合進行UIに表示する基本データを返却します。
紐づいているイベント情報的なものをベースに情報を組み立て、当日に該当ユーザーに対してあてがわれていると思われる
　

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationGetMatchInfoByEventInfoAPIPost : AppAPIPostBase {
		public long eventType = 0; // イベント種別。1の場合colosseum
		public long eventId = 0; // イベント・シーズンのID。typeが1の場合、sColosseumEventId

   }

   [Serializable]
   public class BattleReserveFormationGetMatchInfoByEventInfoAPIResponse : AppAPIResponseBase {
		public BattleReserveFormationMatchInfo matchInfo = null; // 試合情報

   }
      
   public partial class BattleReserveFormationGetMatchInfoByEventInfoAPIRequest : AppAPIRequestBase<BattleReserveFormationGetMatchInfoByEventInfoAPIPost, BattleReserveFormationGetMatchInfoByEventInfoAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/getMatchInfoByEventInfo"; } }
   }
}