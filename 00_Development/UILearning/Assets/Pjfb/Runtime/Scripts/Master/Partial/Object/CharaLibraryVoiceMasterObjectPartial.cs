
namespace Pjfb.Master {
	public enum CharaVoiceLocationType
	{
		None = -1,
		NotDialogue = 0, // 掛け合い用ではない
		KickOffOut = 1, // キックオフ_先行
		KickOffIn = 2, // キックオフ_受け
		DribbleOwnerGood = 3, // ドリブル_保持者（好調）
		DribbleOwnerImpatience = 4, // ドリブル_保持者（焦り）
		DribbleOwnerTired = 5, // ドリブル_保持者（疲れ）
		DribbleSupporter = 6, // ドリブル_味方
		DribbleMaker = 7, // ドリブル_マーク者
		OnMatchUpOffence = 8, // 汎用マッチアップ相手参上_攻め
		OnMatchUpDefence = 9, // 汎用マッチアップ相手参上_守り
		OnMatchUpSupportOffence = 10, // 汎用マッチアップ味方_攻め
		OnMatchUpSupportDefence = 11, // 汎用マッチアップ味方_守り
		MatchUpWinOffence = 12, // マッチアップ勝利_攻め
		MatchUpLoseOffence = 13, // マッチアップ敗北_攻め
		MatchUpWinDefence = 14, // マッチアップ勝利_守り
		MatchUpLoseDefence = 15, // マッチアップ敗北_守り
		Cross = 16, // 汎用クロス
		PassOut = 17, // 汎用パス出し
		PassIn = 18, // 汎用パス受け
		PassCut = 19, // パスカット
		Shoot = 20, // 汎用シュート
		ShootBlockCatched = 21, // シュートブロック_止める
		ShootBlockTouched = 22, // シュートブロック_かする
		ShootBlockNotReach = 23, // シュートブロック_届かない
		GoalOwner = 24, // 汎用ゴール後
		GoalOther = 25, // 他人がゴールした後の喜び
		SecondBallGet = 26, // セカンドボール獲得_成功
		SecondBallFail = 27, // セカンドボール獲得_失敗
		ThrowIn = 28, // スローイン
		SpecialShoot = 29, // 専用シュート
		SpecialGoal = 30, // 専用ゴール
		Yell = 31, // 汎用気合い
		GameWin = 32, // 試合終了後_勝利
		GameLose = 33, // 試合終了後_敗北
		SpeedMatchUpOffence = 34, // スピードマッチアップ攻め
		SpeedMatchUpDefence = 35, // スピードマッチアップ守り
		PhysicalMatchUpOffence = 36, // フィジカルマッチアップ攻め
		PhysicalMatchUpDefence = 37, // フィジカルマッチアップ守り
		TechnicMatchUpOffence = 38, // テクニカルマッチアップ攻め
		TechnicMatchUpDefence = 39, // テクニカルマッチアップ守り
		ShootBlockReady = 40, // シュートブロック前
		SpecialAbility = 41, // 必殺技
		
		// コマンド選択セリフ
		CommandPhraseTooFarToShoot = 101,
		CommandPhraseFarToShoot = 102,
		CommandPhraseBitFarToShoot = 103,
		CommandPhraseInShootRange = 104,
		CommandPhraseInCloseShootRange = 105,
		CommandPhraseNiceMeetShoot = 106,
		CommandPhraseBadMeetShoot = 107,
		CommandPhraseNiceCourseShoot = 108,
		CommandPhraseBadCourseShoot = 109,
		
		CommandPhraseThrough = 110,
		CommandPhraseNiceThrough = 111,
		CommandPhraseBadThrough = 112,
		CommandPhraseTooBadThrough = 113,

		CommandPhraseCross = 114,
		CommandPhraseNiceMeetCross = 115,
		CommandPhraseBadMeetCross = 116,
		CommandPhraseNiceCourseCross = 117,
		CommandPhraseBadCourseCross = 118,
		
		CommandPhrasePass = 119,
		CommandPhraseNicePass = 120,
		CommandPhraseBadPass = 121,

		CommandPhraseBackPass = 122,
		CommandPhraseNiceBackPass = 123,
		CommandPhraseBadBackPass = 124,
		
		CommandPhraseLongPass = 125,
		CommandPhraseCharaFlavor = 126,
	}
	
	public partial class CharaLibraryVoiceMasterObject : CharaLibraryVoiceMasterObjectBase, IMasterObject {
		/// <summary>キャラタイプ</summary>
		public CharaVoiceLocationType LocationType => (CharaVoiceLocationType)base.locationType;
	}

}
