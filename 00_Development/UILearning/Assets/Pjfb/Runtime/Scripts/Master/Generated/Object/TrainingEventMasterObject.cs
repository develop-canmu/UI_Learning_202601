//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingEventMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _eventType {get{ return eventType;} set{ this.eventType = value;}}
		[MessagePack.Key(3)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(4)]
		public long _trainingMCharaId {get{ return trainingMCharaId;} set{ this.trainingMCharaId = value;}}
		[MessagePack.Key(5)]
		public long _supportMCharaId {get{ return supportMCharaId;} set{ this.supportMCharaId = value;}}
		[MessagePack.Key(6)]
		public long _mTrainingUnitId {get{ return mTrainingUnitId;} set{ this.mTrainingUnitId = value;}}
		[MessagePack.Key(7)]
		public long _mCharaVariableConditionId {get{ return mCharaVariableConditionId;} set{ this.mCharaVariableConditionId = value;}}
		[MessagePack.Key(8)]
		public long _eventGroup {get{ return eventGroup;} set{ this.eventGroup = value;}}
		[MessagePack.Key(9)]
		public bool _isUsedAsGoal {get{ return isUsedAsGoal;} set{ this.isUsedAsGoal = value;}}
		[MessagePack.Key(10)]
		public string _goalMCharaVariableConditionId {get{ return goalMCharaVariableConditionId;} set{ this.goalMCharaVariableConditionId = value;}}
		[MessagePack.Key(11)]
		public string _goalDescription {get{ return goalDescription;} set{ this.goalDescription = value;}}
		[MessagePack.Key(12)]
		public string _scenarioNumber {get{ return scenarioNumber;} set{ this.scenarioNumber = value;}}
		[MessagePack.Key(13)]
		public long _mTrainingBattleId {get{ return mTrainingBattleId;} set{ this.mTrainingBattleId = value;}}
		[MessagePack.Key(14)]
		public WrapperIntList[] _choicePrizeJson {get{ return choicePrizeJson;} set{ this.choicePrizeJson = value;}}
		[MessagePack.Key(15)]
		public bool _isAddedTurnInvoke {get{ return isAddedTurnInvoke;} set{ this.isAddedTurnInvoke = value;}}
		[MessagePack.Key(16)]
		public long _minLevel {get{ return minLevel;} set{ this.minLevel = value;}}
		[MessagePack.Key(17)]
		public long _enhanceGroup {get{ return enhanceGroup;} set{ this.enhanceGroup = value;}}
		[MessagePack.Key(18)]
		public string _enhanceDescription {get{ return enhanceDescription;} set{ this.enhanceDescription = value;}}
		[MessagePack.Key(19)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(20)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(21)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] long eventType = 0; // $eventType 1: いわゆる「イベント」（シナリオや行動後イベント）、2: 行動、3: 休息、4: バトル（シナリオ由来カード由来の両方）、98: トレーニング終了
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId シナリオID。シナリオに紐づくイベントでは指定する。特定のシナリオに紐づかない場合は0
		[UnityEngine.SerializeField] long trainingMCharaId = 0; // $trainingMCharaId 育成キャラID。育成キャラが関わるイベントでは指定する。特定のキャラに紐づかない場合は0
		[UnityEngine.SerializeField] long supportMCharaId = 0; // $supportMCharaId サポートキャラID。サポートキャラが関わるイベントでは指定する。特定のキャラに紐づかない場合は0
		[UnityEngine.SerializeField] long mTrainingUnitId = 0; // $mTrainingUnitId トレーニングユニットID。ユニット由来で起きるイベントでは指定する。特定のユニットに紐づかない場合は0
		[UnityEngine.SerializeField] long mCharaVariableConditionId = 0; // $mCharaVariableConditionId イベントの発生条件として使用する条件のID。この条件を使用しない場合は0
		[UnityEngine.SerializeField] long eventGroup = 0; // $eventGroup 同じイベントのグルーピング用のID。canRepeatがfalseだった場合、同じグループIDのイベントは発生しない
		[UnityEngine.SerializeField] bool isUsedAsGoal = false; // $isUsedAsGoal このイベントを目標として使用するか。<br>達成条件は、バトルイベントの場合は勝利とし、それ以外の場合は goalMCharaVariableConditionIdList で定められた条件とする
		[UnityEngine.SerializeField] string goalMCharaVariableConditionId = ""; // $goalMCharaVariableConditionId eventType <> 4 かつ isUsedAsGoal = 1の場合、目標達成かどうかを判定するために使用する条件のID
		[UnityEngine.SerializeField] string goalDescription = ""; // $goalDescription 目標欄に表示される記述
		[UnityEngine.SerializeField] string scenarioNumber = ""; // $scenarioNumber 再生するシナリオの番号
		[UnityEngine.SerializeField] long mTrainingBattleId = 0; // $mTrainingBattleId (eventType=4のみ)起きるバトルのID
		[UnityEngine.SerializeField] WrapperIntList[] choicePrizeJson = null; // $choicePrizeJson 「選択肢を判別する番号」「mTrainingEventRewardId」「自動周回の場合に選択するフラグ（1:選択、2:しない）」を選択肢ごとに羅列したJSON（二重配列）。選択肢がない場合は選択肢番号0を指定する。<br>例：[[1,16,1],[2,17,2],[3,18,2],[4,19,2]]
		[UnityEngine.SerializeField] bool isAddedTurnInvoke = false; // $isAddedTurnInvoke isUsedGoal=1の場合、ターン消化後に追加ターンを発動するか。<br>1: 発動する、2: 発動しない。発動しない場合は追加ターンを次の目標に持ち越し
		[UnityEngine.SerializeField] long minLevel = 0; // $minLevel trainingMCharaIdとsupportMCharaIdに対する解放下限レベル。レベルを満たしている場合に解放される。特に設定ない場合は0
		[UnityEngine.SerializeField] long enhanceGroup = 0; // $enhanceGroup イベント強化グループ。このグループの中でminLevelの低い順に強化解放されていく。強化解放を行わない場合は0
		[UnityEngine.SerializeField] string enhanceDescription = ""; // $enhanceDescription イベント強化に関する説明文
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始時刻
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了時刻
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingEventMasterObjectBase {
		public virtual TrainingEventMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long eventType => _rawData._eventType;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long trainingMCharaId => _rawData._trainingMCharaId;
		public virtual long supportMCharaId => _rawData._supportMCharaId;
		public virtual long mTrainingUnitId => _rawData._mTrainingUnitId;
		public virtual long mCharaVariableConditionId => _rawData._mCharaVariableConditionId;
		public virtual long eventGroup => _rawData._eventGroup;
		public virtual bool isUsedAsGoal => _rawData._isUsedAsGoal;
		public virtual string goalMCharaVariableConditionId => _rawData._goalMCharaVariableConditionId;
		public virtual string goalDescription => _rawData._goalDescription;
		public virtual string scenarioNumber => _rawData._scenarioNumber;
		public virtual long mTrainingBattleId => _rawData._mTrainingBattleId;
		public virtual WrapperIntList[] choicePrizeJson => _rawData._choicePrizeJson;
		public virtual bool isAddedTurnInvoke => _rawData._isAddedTurnInvoke;
		public virtual long minLevel => _rawData._minLevel;
		public virtual long enhanceGroup => _rawData._enhanceGroup;
		public virtual string enhanceDescription => _rawData._enhanceDescription;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingEventMasterValueObject _rawData = null;
		public TrainingEventMasterObjectBase( TrainingEventMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingEventMasterObject : TrainingEventMasterObjectBase, IMasterObject {
		public TrainingEventMasterObject( TrainingEventMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingEventMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Event;

        [UnityEngine.SerializeField]
        TrainingEventMasterValueObject[] m_Training_Event = null;
    }


}
