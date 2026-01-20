//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Player {
		public long playerId = 0; // プレイヤーID（DB上に保存される際のID）。playerTypeによって意味合いが変化。
		public long playerType = 0; // ユーザー種別（1 => u_status, 2 => NPC（IDには-1が入る。ただしグループ戦においてNPC間での識別を行う必要がある場合はNPC内でユニークな番号が振られる））
		public long playerIndex = 0; // バトルデータ内のプレイヤー区別の識別子
		public long groupIndex = 0; // バトルデータ内の所属グループ区別用の識別子
		public long mIconId = 0; // ユーザーアイコン
		public string name = ""; // ユーザー名
		public long optionValue = 0; // オプション値（作戦番号等）
		public BattleV2GvgItem[] gvgItemList = null; // GVGインゲームに持ち込むアイテム情報のリスト
		public PlayerGameliftOptionSetting setting = null; // GameLift対戦で使用する事前設定
		public bool canIncludeComboAbility = false; // コンボスキルが発動できるかどうか。コンボスキル情報を clientData に内包させクライアント側で個別に発動判定を行う際に使用する
		public PlayerGameliftOptionDeckInfo[] deckInfoList = null; // デッキ情報リスト

   }
   
}