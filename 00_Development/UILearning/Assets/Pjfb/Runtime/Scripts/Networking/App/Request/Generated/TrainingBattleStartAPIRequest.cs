//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
バトルの開始

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingBattleStartAPIPost : AppAPIPostBase {
		public WrapperIntList[] idRoleList = null; // [tableType, id, roleNumber]を列挙した2次元配列
		public long deckOptionValue = 0; // デッキのオプション値

   }

   [Serializable]
   public class TrainingBattleStartAPIResponse : AppAPIResponseBase {
		public TrainingBattlePending battlePending = null; // 個人トレーニング中バトル情報

   }
      
   public partial class TrainingBattleStartAPIRequest : AppAPIRequestBase<TrainingBattleStartAPIPost, TrainingBattleStartAPIResponse> {
      public override string apiName{get{ return "training/battleStart"; } }
   }
}