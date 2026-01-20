//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class UserProfileUserStatus {
		public long uMasterId = 0; // ユーザID
		public string name = ""; // ユーザ名
		public long mIconId = 0; // 設定しているアイコンID
		public string lastLogin = ""; // ログイン日時
		public long mTitleId = 0; // 称号
		public string wordIntroduction = ""; // 自己紹介文
		public GuildGuildSummary guild = null; // 所属ギルド情報
		public DeckBase deck = null; // 最も戦力の高いデッキ
		public CharaV2Base[] friendDeckCharaList = null; // フレンド貸出デッキに編成されているキャラクターリスト
		public CharaV2Base[] charaList = null; // デッキに編成されているキャラクターリスト
		public CharaVariableProfileStatus[] charaVariableList = null; // デッキに編成されている可変キャラクターリスト
		public bool canYell = false; // 該当のユーザーにエールできるか
		public bool canBlock = false; // 該当のユーザーをブロックできるか
		public bool canFollow = false; // 該当のユーザーをフォローできるか
		public bool canInvitation = false; // 該当のユーザーをギルド勧誘できるか
		public bool canLike = false; // 該当のユーザーをいいねできるか
		public UserProfileCardData profileCardData = null; // プロフィールカード情報
		public long likeCount = 0; // いいね数

   }
   
}