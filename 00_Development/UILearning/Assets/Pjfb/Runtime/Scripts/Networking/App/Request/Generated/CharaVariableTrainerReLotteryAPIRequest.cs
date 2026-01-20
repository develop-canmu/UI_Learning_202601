//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
トレーニング補助キャラのサブ能力を再抽選する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableTrainerReLotteryAPIPost : AppAPIPostBase {
		public long id = 0; // 再抽選対象キャラID（u_chara_variable_trainer.id）
		public long subNumber = 0; // 抽選料金の支払い方法。m_chara_trainer_lottery_reload.subNumberを参照する

   }

   [Serializable]
   public class CharaVariableTrainerReLotteryAPIResponse : AppAPIResponseBase {
		public CharaVariableTrainerBase[] charaVariableTrainer = null; // 再抽選後のトレーニング補助キャラ
		public long[] updatedNumberList = null; // 再抽選を行ったステータスの枠番号リスト

   }
      
   public partial class CharaVariableTrainerReLotteryAPIRequest : AppAPIRequestBase<CharaVariableTrainerReLotteryAPIPost, CharaVariableTrainerReLotteryAPIResponse> {
      public override string apiName{get{ return "chara-variable-trainer/reLottery"; } }
   }
}