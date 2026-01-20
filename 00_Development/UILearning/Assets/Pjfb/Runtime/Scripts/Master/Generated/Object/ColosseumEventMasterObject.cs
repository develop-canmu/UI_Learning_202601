//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumEventMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(3)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(4)]
		public long _cycleDays {get{ return cycleDays;} set{ this.cycleDays = value;}}
		[MessagePack.Key(5)]
		public long _intervalMarginDays {get{ return intervalMarginDays;} set{ this.intervalMarginDays = value;}}
		[MessagePack.Key(6)]
		public long _requireDaysForJoin {get{ return requireDaysForJoin;} set{ this.requireDaysForJoin = value;}}
		[MessagePack.Key(7)]
		public long _turnUnitMinute {get{ return turnUnitMinute;} set{ this.turnUnitMinute = value;}}
		[MessagePack.Key(8)]
		public long _mColosseumGradeGroupId {get{ return mColosseumGradeGroupId;} set{ this.mColosseumGradeGroupId = value;}}
		[MessagePack.Key(9)]
		public long _mColosseumBattlePrizeGroupId {get{ return mColosseumBattlePrizeGroupId;} set{ this.mColosseumBattlePrizeGroupId = value;}}
		[MessagePack.Key(10)]
		public long _mColosseumRankingPrizeGroupId {get{ return mColosseumRankingPrizeGroupId;} set{ this.mColosseumRankingPrizeGroupId = value;}}
		[MessagePack.Key(11)]
		public long _npcUseType {get{ return npcUseType;} set{ this.npcUseType = value;}}
		[MessagePack.Key(12)]
		public long _mColosseumNpcGroupId {get{ return mColosseumNpcGroupId;} set{ this.mColosseumNpcGroupId = value;}}
		[MessagePack.Key(13)]
		public long _mCommonStoreCategoryId {get{ return mCommonStoreCategoryId;} set{ this.mCommonStoreCategoryId = value;}}
		[MessagePack.Key(14)]
		public long _gameRuleType {get{ return gameRuleType;} set{ this.gameRuleType = value;}}
		[MessagePack.Key(15)]
		public long _clientHandlingType {get{ return clientHandlingType;} set{ this.clientHandlingType = value;}}
		[MessagePack.Key(16)]
		public long _inGameType {get{ return inGameType;} set{ this.inGameType = value;}}
		[MessagePack.Key(17)]
		public long _inGameSystemId {get{ return inGameSystemId;} set{ this.inGameSystemId = value;}}
		[MessagePack.Key(18)]
		public long _groupSourceType {get{ return groupSourceType;} set{ this.groupSourceType = value;}}
		[MessagePack.Key(19)]
		public long _gradeShiftType {get{ return gradeShiftType;} set{ this.gradeShiftType = value;}}
		[MessagePack.Key(20)]
		public long _mStaminaId {get{ return mStaminaId;} set{ this.mStaminaId = value;}}
		[MessagePack.Key(21)]
		public long _useStaminaValue {get{ return useStaminaValue;} set{ this.useStaminaValue = value;}}
		[MessagePack.Key(22)]
		public long _mCostPointEscalationGroupId {get{ return mCostPointEscalationGroupId;} set{ this.mCostPointEscalationGroupId = value;}}
		[MessagePack.Key(23)]
		public long _deckUseType {get{ return deckUseType;} set{ this.deckUseType = value;}}
		[MessagePack.Key(24)]
		public long[] _deckUseTypeListSub {get{ return deckUseTypeListSub;} set{ this.deckUseTypeListSub = value;}}
		[MessagePack.Key(25)]
		public long _mColosseumEventSeriesId {get{ return mColosseumEventSeriesId;} set{ this.mColosseumEventSeriesId = value;}}
		[MessagePack.Key(26)]
		public MasterSeriesOption _seriesOption {get{ return seriesOption;} set{ this.seriesOption = value;}}
		[MessagePack.Key(27)]
		public string _entryStartAt {get{ return entryStartAt;} set{ this.entryStartAt = value;}}
		[MessagePack.Key(28)]
		public string _entryEndAt {get{ return entryEndAt;} set{ this.entryEndAt = value;}}
		[MessagePack.Key(29)]
		public string _entryConditionJson {get{ return entryConditionJson;} set{ this.entryConditionJson = value;}}
		[MessagePack.Key(30)]
		public long _participantLimit {get{ return participantLimit;} set{ this.participantLimit = value;}}
		[MessagePack.Key(31)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(32)]
		public string _displayEndAt {get{ return displayEndAt;} set{ this.displayEndAt = value;}}
		[MessagePack.Key(33)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] string startAt = ""; // 最初の開始日（日付までしか見ない。1時にオープン想定（集計インターバル考慮））
		[UnityEngine.SerializeField] string endAt = ""; // イベントの終了日時（特に設けない場合は、2099-12-31等の、すごく遠い日付を指定）
		[UnityEngine.SerializeField] long cycleDays = 0; // イベントを実施する日数周期（-1の場合、ループさせない）
		[UnityEngine.SerializeField] long intervalMarginDays = 0; // シーズンごとに発生するインターバル。cycleDays内における「余白」的なものを表現する値となっている。cycleDaysが15でintervalMarginDaysが7だった場合「8日間実施、7日間休止　→ 8日間実施、7日間休止」がでサイクルする。
		[UnityEngine.SerializeField] long requireDaysForJoin = 0; // 初回参加操作をする際に、残日数が何日以上あれば参加可能かを設定する（0の場合、この縛りは無しで参加が可能）
		[UnityEngine.SerializeField] long turnUnitMinute = 0; // スコア方式バトルの場合のみ使用する。1ターンの単位になる分（minute）の設定。基本ターンは日をまたがず初日の場合も0時から始まる計算とする。1440の約数以外を入力した場合以外の挙動は保証しない
		[UnityEngine.SerializeField] long mColosseumGradeGroupId = 0; // '対応するグレードグループのID（グレード管理をしない場合0にする想定だが、現状は未実装。同一グレードグループの大会が並走しないように、注意する（昇降格管理の処理が多重で走る等の懸念あり））'
		[UnityEngine.SerializeField] long mColosseumBattlePrizeGroupId = 0; // 対応する対戦報酬グループのID
		[UnityEngine.SerializeField] long mColosseumRankingPrizeGroupId = 0; // 対応するランキング報酬グループのID
		[UnityEngine.SerializeField] long npcUseType = 0; // NPC使用タイプ。1 => m_colosseum_npc を直接参照、2 => m_colosseum_npc_guild を通して m_colosseum_npc を参照し、グループ単位でダミーデッキ群を取り扱う
		[UnityEngine.SerializeField] long mColosseumNpcGroupId = 0; // 対応するNPCグループのID
		[UnityEngine.SerializeField] long mCommonStoreCategoryId = 0; // 対応する交換所のID
		[UnityEngine.SerializeField] long gameRuleType = 0; // この設定によって、別なコンテンツして機能する。1：通常のPVP、2：グループ対抗戦、3 => グループ対抗リーグ形式戦, 4 => グループ対抗トーナメント戦（未実装）
		[UnityEngine.SerializeField] long clientHandlingType = 0; // クライアント側でどの機能・コンテンツとして取り扱うかの分岐（どの値にするかは、企画とクライアントチームですり合わせ）
		[UnityEngine.SerializeField] long inGameType = 0; // 実際の対戦方法。1 => 指定なし（従来の形式）、 2 => 編成予約戦、 3 => ゲームサーバ上
		[UnityEngine.SerializeField] long inGameSystemId = 0; // inGameType = 2の場合（m_battle_reserve_formation.id）、inGameType = 3の場合（m_battle_gamelift.id）
		[UnityEngine.SerializeField] long groupSourceType = 0; // gameRuleTypeが1の場合は参照しない。何をもとにグループを生成するか。1 => 存在ギルドandギルドグレードデータ（初回は条件を満たしたギルドを抽出し、ギルドグレードを生成する。以降はギルドグレードを使用）,2 => エントリーandギルドグレード（エントリー時にギルドグレードの生成が行われる。初回エントリー後は最下位以外のギルドグレードの使いまわしが行われる※未実装。大会用）3 => エントリー（エントリーデータのみを用いる。グレードの生成も行われるが、参照はされない）99 => ギルド（この場合、シーズン結果がギルドランクの昇降格に関与する）,
		[UnityEngine.SerializeField] long gradeShiftType = 0; // グレード変動種別（現在、グループ戦かつソースがgroupSourceTypeがグループの場合にのみ機能）1 => 定常（順位準拠でポイントを付与し、クラスポイント準拠で発生）、2 => 入れ替え戦方式、97 => 終了時にグレードリセット（未実装）、98 => グレードキャパシティに応じて足切り（削除）、99 => 昇降格動作させない（未実装）
		[UnityEngine.SerializeField] long mStaminaId = 0; // 消費するスタミナがどれであるか（0の場合、PVPで指定されているデフォルトのスタミナを用いる）
		[UnityEngine.SerializeField] long useStaminaValue = 0; // 消費スタミナ情報（0の場合は無制限。-1の場合は、スタミナを消費せずにポイントを消費）
		[UnityEngine.SerializeField] long mCostPointEscalationGroupId = 0; // スタミナが無い際に代わりに消費するポイントの定義（0の場合、ポイント消費は不可能）
		[UnityEngine.SerializeField] long deckUseType = 0; // 使用デッキの特殊区分指定。4の場合、通常のバトルと同じ。ほかは、1001,1101,1201,1301等が指定可能
		[UnityEngine.SerializeField] long[] deckUseTypeListSub = null; // $deckUseTypeListSub サブ使用デッキ指定。deckUseType の他にインゲームで参照する必要があるゲームでは指定する
		[UnityEngine.SerializeField] long mColosseumEventSeriesId = 0; // 同一IDのイベントをグループ化する。0の場合、特にグループ化はない想定。現状、この値を0以外にする場合にはサイクル設定を行わないようにする（後で出来るようになる可能性もある）
		[UnityEngine.SerializeField] MasterSeriesOption seriesOption = null; // シリーズに設定がなされる場合の特殊な設定を格納。特にシリーズとして運用しない場合は、nullや空の配列を入れる
		[UnityEngine.SerializeField] string entryStartAt = ""; // エントリー開始日時（ない場合はnull）
		[UnityEngine.SerializeField] string entryEndAt = ""; // エントリー終了日時（ない場合はnull）
		[UnityEngine.SerializeField] string entryConditionJson = ""; // エントリー条件例：[{"type": 1, "value": 1, "targetId": 1}]type => 1:ギルドランク、2:リーグマッチのシーズンのグレード番号、3:ギルド人数、4:ギルド総戦力targetId => type=2の場合に対象のmColosseumEventのidを設定
		[UnityEngine.SerializeField] long participantLimit = 0; // 参加可能数。上限を超えた場合は戦力順で足切り。-1の場合は無制限
		[UnityEngine.SerializeField] long displayPriority = 0; // 表示優先度
		[UnityEngine.SerializeField] string displayEndAt = ""; // 表示終了日時
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumEventMasterObjectBase {
		public virtual ColosseumEventMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual long cycleDays => _rawData._cycleDays;
		public virtual long intervalMarginDays => _rawData._intervalMarginDays;
		public virtual long requireDaysForJoin => _rawData._requireDaysForJoin;
		public virtual long turnUnitMinute => _rawData._turnUnitMinute;
		public virtual long mColosseumGradeGroupId => _rawData._mColosseumGradeGroupId;
		public virtual long mColosseumBattlePrizeGroupId => _rawData._mColosseumBattlePrizeGroupId;
		public virtual long mColosseumRankingPrizeGroupId => _rawData._mColosseumRankingPrizeGroupId;
		public virtual long npcUseType => _rawData._npcUseType;
		public virtual long mColosseumNpcGroupId => _rawData._mColosseumNpcGroupId;
		public virtual long mCommonStoreCategoryId => _rawData._mCommonStoreCategoryId;
		public virtual long gameRuleType => _rawData._gameRuleType;
		public virtual long clientHandlingType => _rawData._clientHandlingType;
		public virtual long inGameType => _rawData._inGameType;
		public virtual long inGameSystemId => _rawData._inGameSystemId;
		public virtual long groupSourceType => _rawData._groupSourceType;
		public virtual long gradeShiftType => _rawData._gradeShiftType;
		public virtual long mStaminaId => _rawData._mStaminaId;
		public virtual long useStaminaValue => _rawData._useStaminaValue;
		public virtual long mCostPointEscalationGroupId => _rawData._mCostPointEscalationGroupId;
		public virtual long deckUseType => _rawData._deckUseType;
		public virtual long[] deckUseTypeListSub => _rawData._deckUseTypeListSub;
		public virtual long mColosseumEventSeriesId => _rawData._mColosseumEventSeriesId;
		public virtual MasterSeriesOption seriesOption => _rawData._seriesOption;
		public virtual string entryStartAt => _rawData._entryStartAt;
		public virtual string entryEndAt => _rawData._entryEndAt;
		public virtual string entryConditionJson => _rawData._entryConditionJson;
		public virtual long participantLimit => _rawData._participantLimit;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual string displayEndAt => _rawData._displayEndAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumEventMasterValueObject _rawData = null;
		public ColosseumEventMasterObjectBase( ColosseumEventMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumEventMasterObject : ColosseumEventMasterObjectBase, IMasterObject {
		public ColosseumEventMasterObject( ColosseumEventMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumEventMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Event;

        [UnityEngine.SerializeField]
        ColosseumEventMasterValueObject[] m_Colosseum_Event = null;
    }


}
