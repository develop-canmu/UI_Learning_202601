//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetTrainingBattleCountAndTurnListAPIPost : AppAPIPostBase {
		public long turn = 0; // 現在のターン数を何ターン目とするか
		public long[] addedTurnMapList = null; // 追加ターンマップ。過去の追加ターン数を指定する
		public long addedTurn = 0; // 追加ターン。現在の追加ターン数を指定する
		public long battleCount = 0; // シナリオ進行度
		public long f = 0; // 前方向何ターンぶんを返すか（設定値boardVisibleTurnCountと同じ）
		public long b = 0; // 後方向何ターンぶんを返すか（設定値boardVisibleTurnCountReverseと同じ）

   }

   [Serializable]
   public class DebugGetTrainingBattleCountAndTurnListAPIResponse : AppAPIResponseBase {
		public object[] battleCountAndTurnList = null; // シナリオ進行度のターン数の組み合わせリスト

   }
      
   public partial class DebugGetTrainingBattleCountAndTurnListAPIRequest : AppAPIRequestBase<DebugGetTrainingBattleCountAndTurnListAPIPost, DebugGetTrainingBattleCountAndTurnListAPIResponse> {
      public override string apiName{get{ return "debug/getTrainingBattleCountAndTurnList"; } }
   }
}