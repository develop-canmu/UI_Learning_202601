//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingScenarioMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _parentMTrainingScenarioId {get{ return parentMTrainingScenarioId;} set{ this.parentMTrainingScenarioId = value;}}
		[MessagePack.Key(3)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(4)]
		public long _trainingTurn {get{ return trainingTurn;} set{ this.trainingTurn = value;}}
		[MessagePack.Key(5)]
		public long _requiredStamina {get{ return requiredStamina;} set{ this.requiredStamina = value;}}
		[MessagePack.Key(6)]
		public string _commonMTrainingCardIdList {get{ return commonMTrainingCardIdList;} set{ this.commonMTrainingCardIdList = value;}}
		[MessagePack.Key(7)]
		public long _trustExpRate {get{ return trustExpRate;} set{ this.trustExpRate = value;}}
		[MessagePack.Key(8)]
		public long _intentionalEventCount {get{ return intentionalEventCount;} set{ this.intentionalEventCount = value;}}
		[MessagePack.Key(9)]
		public long _practiceMEnhanceId {get{ return practiceMEnhanceId;} set{ this.practiceMEnhanceId = value;}}
		[MessagePack.Key(10)]
		public string _mCharaVariableConditionIdListForConditionEffect {get{ return mCharaVariableConditionIdListForConditionEffect;} set{ this.mCharaVariableConditionIdListForConditionEffect = value;}}
		[MessagePack.Key(11)]
		public string _mCharaVariableConditionIdListForInspireExp {get{ return mCharaVariableConditionIdListForInspireExp;} set{ this.mCharaVariableConditionIdListForInspireExp = value;}}
		[MessagePack.Key(12)]
		public long _mSystemLockSystemNumber {get{ return mSystemLockSystemNumber;} set{ this.mSystemLockSystemNumber = value;}}
		[MessagePack.Key(13)]
		public string _useStatusList {get{ return useStatusList;} set{ this.useStatusList = value;}}
		[MessagePack.Key(14)]
		public string _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(15)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(16)]
		public string _goalDescription {get{ return goalDescription;} set{ this.goalDescription = value;}}
		[MessagePack.Key(17)]
		public string _bgmCueNameList {get{ return bgmCueNameList;} set{ this.bgmCueNameList = value;}}
		[MessagePack.Key(18)]
		public long _isTutorial {get{ return isTutorial;} set{ this.isTutorial = value;}}
		[MessagePack.Key(19)]
		public long _mTrainingPointId {get{ return mTrainingPointId;} set{ this.mTrainingPointId = value;}}
		[MessagePack.Key(20)]
		public long _enabledTrainingAuto {get{ return enabledTrainingAuto;} set{ this.enabledTrainingAuto = value;}}
		[MessagePack.Key(21)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(22)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(23)]
		public string _trainingAutoStartAt {get{ return trainingAutoStartAt;} set{ this.trainingAutoStartAt = value;}}
		[MessagePack.Key(24)]
		public long _mDeckFormatId {get{ return mDeckFormatId;} set{ this.mDeckFormatId = value;}}
		[MessagePack.Key(25)]
		public long _concentrationMTrainingPointConvertGroup {get{ return concentrationMTrainingPointConvertGroup;} set{ this.concentrationMTrainingPointConvertGroup = value;}}
		[MessagePack.Key(26)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] long parentMTrainingScenarioId = 0; // $parentMTrainingScenarioId 親シナリオID。クライアント側でのみ使用
		[UnityEngine.SerializeField] long type = 0; // $type シナリオ種別。1: 通常、2: コンセントレーションあり、11: FLOW(PJFB)
		[UnityEngine.SerializeField] long trainingTurn = 0; // $trainingTurn ターン数
		[UnityEngine.SerializeField] long requiredStamina = 0; // $requiredStamina 必要スタミナ値
		[UnityEngine.SerializeField] string commonMTrainingCardIdList = ""; // $commonMTrainingCardIdList 本シナリオで使用される通常練習カードのIDリスト
		[UnityEngine.SerializeField] long trustExpRate = 0; // $trustExpRate 信頼度経験値に掛けられる倍率（万分率）
		[UnityEngine.SerializeField] long intentionalEventCount = 0; // $intentionalEventCount 毎ターン何種類の実行可能な任意試合が抽選されるか
		[UnityEngine.SerializeField] long practiceMEnhanceId = 0; // $practiceMEnhanceId 練習系統が従う強化ID。強化されない場合は0を指定する
		[UnityEngine.SerializeField] string mCharaVariableConditionIdListForConditionEffect = ""; // $mCharaVariableConditionIdListForConditionEffect コンディションに応じた練習ボーナス倍率テーブルを選定する基準として使用する可変キャラパラメータ条件リスト
		[UnityEngine.SerializeField] string mCharaVariableConditionIdListForInspireExp = ""; // $mCharaVariableConditionIdListForInspireExp コンディションに応じた練習ボーナス倍率テーブルを選定する基準として使用する可変キャラパラメータ条件リスト（インスピレーションブースト用）
		[UnityEngine.SerializeField] long mSystemLockSystemNumber = 0; // $mSystemLockSystemNumber シナリオの解放条件となる機能ロックのシステム番号。無条件解放の場合は0とする
		[UnityEngine.SerializeField] string useStatusList = ""; // $useStatusList このシナリオで使用するステータスのリスト。ここに含めなければ、裏で獲得していても表には表示させない。未入力の場合、設定値 useStatusList に従う
		[UnityEngine.SerializeField] string imageId = ""; // 画像ID
		[UnityEngine.SerializeField] string description = ""; // 説明
		[UnityEngine.SerializeField] string goalDescription = ""; // 目標テキスト
		[UnityEngine.SerializeField] string bgmCueNameList = ""; // BGMキュー名リスト。トレーニングのシナリオ進行度ごとにBGMを出しわけたい場合に ["cue1","cue2"] のように指定する。n番目のキュー名は進行度nに対応し、Pending/GoalList にキュー名が載るが、クライアント側でしか使用されない
		[UnityEngine.SerializeField] long isTutorial = 0; // チュートリアル用かどうか。1 => チュートリアル用、2 => 通常
		[UnityEngine.SerializeField] long mTrainingPointId = 0; // トレーニング専用ポイントマスタID。使用しない場合は0
		[UnityEngine.SerializeField] long enabledTrainingAuto = 0; // 自動トレーニングが有効か。1=>有効、2=>無効
		[UnityEngine.SerializeField] string startAt = ""; // $startAt プレイ可能期間の開始日時
		[UnityEngine.SerializeField] string endAt = ""; // $endAt プレイ可能期間の終了日時
		[UnityEngine.SerializeField] string trainingAutoStartAt = ""; // $trainingAutoStartAt 自動トレーニング開始日時
		[UnityEngine.SerializeField] long mDeckFormatId = 0; // トレーニングにおけるmDeckFormatのidに紐づく編成制限を設定する。使用しない場合は0
		[UnityEngine.SerializeField] long concentrationMTrainingPointConvertGroup = 0; // type=11の場合に使用するconcentrationのexp獲得テーブル
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingScenarioMasterObjectBase {
		public virtual TrainingScenarioMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long parentMTrainingScenarioId => _rawData._parentMTrainingScenarioId;
		public virtual long type => _rawData._type;
		public virtual long trainingTurn => _rawData._trainingTurn;
		public virtual long requiredStamina => _rawData._requiredStamina;
		public virtual string commonMTrainingCardIdList => _rawData._commonMTrainingCardIdList;
		public virtual long trustExpRate => _rawData._trustExpRate;
		public virtual long intentionalEventCount => _rawData._intentionalEventCount;
		public virtual long practiceMEnhanceId => _rawData._practiceMEnhanceId;
		public virtual string mCharaVariableConditionIdListForConditionEffect => _rawData._mCharaVariableConditionIdListForConditionEffect;
		public virtual string mCharaVariableConditionIdListForInspireExp => _rawData._mCharaVariableConditionIdListForInspireExp;
		public virtual long mSystemLockSystemNumber => _rawData._mSystemLockSystemNumber;
		public virtual string useStatusList => _rawData._useStatusList;
		public virtual string imageId => _rawData._imageId;
		public virtual string description => _rawData._description;
		public virtual string goalDescription => _rawData._goalDescription;
		public virtual string bgmCueNameList => _rawData._bgmCueNameList;
		public virtual long isTutorial => _rawData._isTutorial;
		public virtual long mTrainingPointId => _rawData._mTrainingPointId;
		public virtual long enabledTrainingAuto => _rawData._enabledTrainingAuto;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual string trainingAutoStartAt => _rawData._trainingAutoStartAt;
		public virtual long mDeckFormatId => _rawData._mDeckFormatId;
		public virtual long concentrationMTrainingPointConvertGroup => _rawData._concentrationMTrainingPointConvertGroup;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingScenarioMasterValueObject _rawData = null;
		public TrainingScenarioMasterObjectBase( TrainingScenarioMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingScenarioMasterObject : TrainingScenarioMasterObjectBase, IMasterObject {
		public TrainingScenarioMasterObject( TrainingScenarioMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingScenarioMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Scenario;

        [UnityEngine.SerializeField]
        TrainingScenarioMasterValueObject[] m_Training_Scenario = null;
    }


}
