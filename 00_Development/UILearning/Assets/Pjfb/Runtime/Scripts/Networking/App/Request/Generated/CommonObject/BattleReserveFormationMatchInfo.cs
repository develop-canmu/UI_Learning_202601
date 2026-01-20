//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationMatchInfo {
		public BattleReserveFormationMatchHeader matchHeader = null; // 試合ヘッダ
		public BattleReserveFormationGroupInfo groupInfo = null; // 自身のグループ
		public BattleReserveFormationGroupInfo groupInfoOpponent = null; // 相手のグループ
		public BattleReserveFormationMatchLineup[] matchLineupList = null; // 試合回戦
		public bool hasUnsetAuthority = false; // デッキを外す権限を所有しているかどうか
		public long progress = 0; // 開催進捗★1:エントリー受付（編成ロック時間前）　=> デッキ詳細は渡さない（ユーザー情報まではわかる・自陣営においてはpartyNumber等の情報もわかる）★2:エントリー受付終了（編成ロック後～試合時刻定時（マスタをもとに判断））　=> 簡易デッキも渡す（相手・相手デッキの総合力隠蔽）★3:開催中状態（試合時刻定時（マスタをもとに判断）～試合終了想定時刻）★4:集計中（試合終了想定時刻～、集計未完了）=> 簡易デッキも渡す　=> 試合の勝敗状態なども進捗度に寄っては渡す★5試合終了状態（試合終了時刻～、集計完了状態）　=> 全日程を含めての「勝敗」が出る
		public bool isLeft = false; // プレイヤーの陣営がleftかrightか

   }
   
}