using System;
using Pjfb;
using UnityEngine;

/// <summary>ユーザーにどのコンテンツか表示するための判定</summary>
public enum ColosseumClientHandlingType
{
	/// <summary>個人</summary>
	PvP = 1,
	/// <summary>団体</summary>
	ClubMatch = 2,
	/// <summary>リーグマッチ</summary>
	LeagueMatch = 3,
	/// <summary>トーナメント</summary>
	InstantTournament = 4,
	//// <summary> クラブロワイヤル </summary>
	// todo: 数値はまだ不確定
	ClubRoyal = 5,
}

public enum ColosseumGradeShiftType
{
	/// <summary>定常</summary>
	Point = 1,
	/// <summary>入れ替え戦方式</summary>
	ShiftBattle = 2,
	/// <summary>リセット</summary>
	Reset = 97,
}

public enum ColosseumInGameType
{
	// 指定なし
	Default = 1,
	// 編成予約戦
	BattleReserveFormation = 2,
	// ゲームサーバー上
	BattleGameLift = 3,
}

/// <summary>参加条件</summary>
public enum TournamentConditionType
{
	/// <summary>クラブランク</summary>
	GuildRank = 1,
	/// <summary>シーズンランク</summary>
	SeasonRank = 2,
	/// <summary>クラブ人数</summary>
	MemberCount = 3,
	/// <summary>クラブ総戦力</summary>
	GuildCombatPower = 4,
}

[Serializable]
public class EntryCondition
{

	[SerializeField]
	private long type;
	/// <summary>条件type</summary>
	public TournamentConditionType ConditionType => (TournamentConditionType)type;
	
	[SerializeField]
	private string value;
	/// <summary>必要数 注:キャッシュすること</summary>
	public BigValue Value => new(value);


	[SerializeField]
	private long targetId;
	/// <summary>対象ID</summary>
	public long TargetId => targetId;
}

namespace Pjfb.Master {
	public partial class ColosseumEventMasterObject : ColosseumEventMasterObjectBase, IMasterObject {

		public new ColosseumClientHandlingType clientHandlingType
		{
			get { return (ColosseumClientHandlingType)base.clientHandlingType; }
		}
		
		public new ColosseumGradeShiftType gradeShiftType
		{
			get { return (ColosseumGradeShiftType)base.gradeShiftType; }
		}
	}
}
