//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
u_battle_reserve_formation_history.id経由でバトル再生に必要な情報を返します

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationGetBattlePreviewDataFromHistoryAPIPost : AppAPIPostBase {
		public long id = 0; // u_battle_reserve_formation_history.id

   }

   [Serializable]
   public class BattleReserveFormationGetBattlePreviewDataFromHistoryAPIResponse : AppAPIResponseBase {
		public string srcData = ""; // バトル前に生成した、バトルに必要な一通りのデータ(json圧縮済み)
		public string destData = ""; // バトル後に生成される、プレビュー用のバトル内容データ(json圧縮済み)
		public long previewIndex = 0; // プレビュー時に参照する陣営index（プレイヤー主観がindex0の陣営か、1の陣営か）

   }
      
   public partial class BattleReserveFormationGetBattlePreviewDataFromHistoryAPIRequest : AppAPIRequestBase<BattleReserveFormationGetBattlePreviewDataFromHistoryAPIPost, BattleReserveFormationGetBattlePreviewDataFromHistoryAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/getBattlePreviewDataFromHistory"; } }
   }
}