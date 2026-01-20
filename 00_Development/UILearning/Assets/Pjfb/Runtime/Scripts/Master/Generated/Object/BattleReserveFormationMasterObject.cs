//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BattleReserveFormationMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mBattleReserveFormationRoundGroupId {get{ return mBattleReserveFormationRoundGroupId;} set{ this.mBattleReserveFormationRoundGroupId = value;}}
		[MessagePack.Key(2)]
		public long _deckUseType {get{ return deckUseType;} set{ this.deckUseType = value;}}
		[MessagePack.Key(3)]
		public string _formationLockAt {get{ return formationLockAt;} set{ this.formationLockAt = value;}}
		[MessagePack.Key(4)]
		public long _battleStartIntervalMinutes {get{ return battleStartIntervalMinutes;} set{ this.battleStartIntervalMinutes = value;}}
		[MessagePack.Key(5)]
		public long _roundIntervalMinutes {get{ return roundIntervalMinutes;} set{ this.roundIntervalMinutes = value;}}
		[MessagePack.Key(6)]
		public string _optionJson {get{ return optionJson;} set{ this.optionJson = value;}}
		[MessagePack.Key(7)]
		public long _eventType {get{ return eventType;} set{ this.eventType = value;}}
		[MessagePack.Key(8)]
		public long _needsUserCount {get{ return needsUserCount;} set{ this.needsUserCount = value;}}
		[MessagePack.Key(9)]
		public long _oneUserReserveCount {get{ return oneUserReserveCount;} set{ this.oneUserReserveCount = value;}}
		[MessagePack.Key(10)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mBattleReserveFormationRoundGroupId = 0; // $mBattleReserveFormationRoundGroupId 回戦グループID
		[UnityEngine.SerializeField] long deckUseType = 0; // $deckUseType 使用デッキ区分（colosseum側と同一のものを指定する）
		[UnityEngine.SerializeField] string formationLockAt = ""; // $formationLockAt 編成ロック時刻。日付部分は参照せず、時刻部分のみ見る（1980-01-01 [12:30]の[]部分のみ参照する）
		[UnityEngine.SerializeField] long battleStartIntervalMinutes = 0; // $battleStartIntervalMinutes 編成ロック時刻から、対戦の1ラウンド目が始まるまでのインターバル分数
		[UnityEngine.SerializeField] long roundIntervalMinutes = 0; // $roundIntervalMinutes 試合ごとのインターバル分数
		[UnityEngine.SerializeField] string optionJson = ""; // $optionJson json。タイトル限定等の特殊な指定を行う。オプション値：judgeFirst：先行後攻の指定を行うオプション（1 => マッチング時に先行後攻判定がある、1以外・未指定 => 判定がない）
		[UnityEngine.SerializeField] long eventType = 0; // $eventType どのマッチングシステムによって管理されているか。1 => colosseum
		[UnityEngine.SerializeField] long needsUserCount = 0; // 必要参加人数。編成予約を行ったユーザー数が、この値未満の場合にはペナルティが発生する
		[UnityEngine.SerializeField] long oneUserReserveCount = 0; // 編成可能数。1ユーザーがこの数を超えて編成予約する事はできない
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BattleReserveFormationMasterObjectBase {
		public virtual BattleReserveFormationMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mBattleReserveFormationRoundGroupId => _rawData._mBattleReserveFormationRoundGroupId;
		public virtual long deckUseType => _rawData._deckUseType;
		public virtual string formationLockAt => _rawData._formationLockAt;
		public virtual long battleStartIntervalMinutes => _rawData._battleStartIntervalMinutes;
		public virtual long roundIntervalMinutes => _rawData._roundIntervalMinutes;
		public virtual string optionJson => _rawData._optionJson;
		public virtual long eventType => _rawData._eventType;
		public virtual long needsUserCount => _rawData._needsUserCount;
		public virtual long oneUserReserveCount => _rawData._oneUserReserveCount;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BattleReserveFormationMasterValueObject _rawData = null;
		public BattleReserveFormationMasterObjectBase( BattleReserveFormationMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BattleReserveFormationMasterObject : BattleReserveFormationMasterObjectBase, IMasterObject {
		public BattleReserveFormationMasterObject( BattleReserveFormationMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BattleReserveFormationMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Battle_Reserve_Formation;

        [UnityEngine.SerializeField]
        BattleReserveFormationMasterValueObject[] m_Battle_Reserve_Formation = null;
    }


}
