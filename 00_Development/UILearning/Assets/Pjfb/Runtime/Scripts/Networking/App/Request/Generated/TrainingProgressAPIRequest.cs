//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人トレーニングの進行

trainingEvent.eventType = 2（行動イベント）の場合は、カードのインデックス（0~4）または休息（-3）から選択する。
trainingEvent.eventType = 4（練習試合イベント）の場合は、勝利（1）・敗北（2）・引き分け（3）から選択する。
それ以外は、trainingEvent.choiceList 中の選択肢を選択する。
trainingEvent.choiceList が null か空の場合は、承認（-1）を選択する。')

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingProgressAPIPost : AppAPIPostBase {
		public long value = 0; // 選択肢。trainingEvent.eventType = 2（行動イベント）の場合は、カードのインデックス（0~4）または休息（-3）から選択する。trainingEvent.eventType = 4（練習試合イベント）の場合は、勝利（1）・敗北（2）・引き分け（3）から選択する。それ以外は、trainingEvent.choiceList 中の選択肢を選択する。trainingEvent.choiceList が null か空の場合は、承認（-1）を選択する。
		public TrainingProgressArgs args = null; // トレーニング進行に必要な追加情報

   }

   [Serializable]
   public class TrainingProgressAPIResponse : AppAPIResponseBase {
		public long code = 0; // 0:通常、98:トレーニング終了イベントによる強制終了、99:完了
		public TrainingEventReward eventReward = null; // 完了したイベントの報酬情報
		public TrainingCoachReward coachReward = null; // 秘奥伝授で獲得した報酬情報
		public TrainingTrainingEvent trainingEvent = null; // イベント情報
		public TrainingPending pending = null; // 個人トレーニング途中情報
		public TrainingCharaVariable charaVariable = null; // 育成中キャラ情報
		public TrainingBattlePending battlePending = null; // 個人トレーニング中バトル情報
		public TrainingFriend friend = null; // フレンド情報
		public EnhanceLevelProgress[] levelProgressList = null; // 信頼度変動
		public FestivalEffectStatus[] festivalEffectStatusList = null; // トレーニングイベント特殊効果情報
		public FestivalPointProgress[] festivalPointProgressList = null; // トレーニングイベントポイント変動
		public FestivalPrizeContentPending[] festivalPrizeContentList = null; // トレーニング完了時に獲得した追加報酬
		public TrainingPointStatus pointStatus = null; // トレーニング専用ポイント関連情報
		public long maxAddTurnValue = 0; // 最大加算ターン数

   }
      
   public partial class TrainingProgressAPIRequest : AppAPIRequestBase<TrainingProgressAPIPost, TrainingProgressAPIResponse> {
      public override string apiName{get{ return "training/progress"; } }
   }
}