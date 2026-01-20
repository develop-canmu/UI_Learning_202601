//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBoardMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public long _conditionTier {get{ return conditionTier;} set{ this.conditionTier = value;}}
		[MessagePack.Key(3)]
		public long _battleCount {get{ return battleCount;} set{ this.battleCount = value;}}
		[MessagePack.Key(4)]
		public long _eventCountMin {get{ return eventCountMin;} set{ this.eventCountMin = value;}}
		[MessagePack.Key(5)]
		public long _eventCountMax {get{ return eventCountMax;} set{ this.eventCountMax = value;}}
		[MessagePack.Key(6)]
		public long _forkCountMin {get{ return forkCountMin;} set{ this.forkCountMin = value;}}
		[MessagePack.Key(7)]
		public long _forkCountMax {get{ return forkCountMax;} set{ this.forkCountMax = value;}}
		[MessagePack.Key(8)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(9)]
		public string _lotteryOrder {get{ return lotteryOrder;} set{ this.lotteryOrder = value;}}
		[MessagePack.Key(10)]
		public string _categoryRateCorrectionJson {get{ return categoryRateCorrectionJson;} set{ this.categoryRateCorrectionJson = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId マス盤番号
		[UnityEngine.SerializeField] long conditionTier = 0; // $conditionTier 本マス盤設定を適用するコンディションのティア。通常のマス盤に対するこの値の設定は0とし、特定のコンディションで発動する特別なマス盤を用意する場合は、この値を m_training_condition_tier.tier の値に設定したレコードを1組（battleCount数ぶん）追加で作成する
		[UnityEngine.SerializeField] long battleCount = 0; // $battleCount シナリオ起因のバトル通過回数、すなわちシナリオ進行度
		[UnityEngine.SerializeField] long eventCountMin = 0; // $eventCountMin 最小マスイベント数。上記ターン区間内に発生するマスイベントの最小個数を定める
		[UnityEngine.SerializeField] long eventCountMax = 0; // $eventCountMax 最大マスイベント数。上記ターン区間内に発生するマスイベントの最大個数を定める
		[UnityEngine.SerializeField] long forkCountMin = 0; // $forkCountMin 最小分岐数。上記ターン区間内に発生する分岐の最小個数を定める
		[UnityEngine.SerializeField] long forkCountMax = 0; // $forkCountMax 最大分岐数。上記ターン区間内に発生する分岐の最大個数を定める。分岐には必ずマスイベントが配置されるため、先に確定したイベント数の半分より多くの分岐が発生しないことに留意
		[UnityEngine.SerializeField] long priority = 0; // $priority マス抽選優先度。同じconditionTier内で、この優先度が高いシナリオ進行度から順にマス配置抽選を行う。優先度が同じ場合はランダム
		[UnityEngine.SerializeField] string lotteryOrder = ""; // $lotteryOrder ターン別マス抽選順序。[5,10]のような形で設定すれば、5ターン目、10ターン目の順にマス配置抽選を優先的に行う（他は順不同）
		[UnityEngine.SerializeField] string categoryRateCorrectionJson = ""; // $categoryRateCorrectionJson マスイベントカテゴリ採用確率補正設定。マスイベントのカテゴリごとにm_training_board_event.rateを加減算する値を次のような形式のJSONで設定する。[[mTrainingBoardEventCategoryId, rate補正値],...]。指定されていないカテゴリIDの補正値は0とする
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBoardMasterObjectBase {
		public virtual TrainingBoardMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long conditionTier => _rawData._conditionTier;
		public virtual long battleCount => _rawData._battleCount;
		public virtual long eventCountMin => _rawData._eventCountMin;
		public virtual long eventCountMax => _rawData._eventCountMax;
		public virtual long forkCountMin => _rawData._forkCountMin;
		public virtual long forkCountMax => _rawData._forkCountMax;
		public virtual long priority => _rawData._priority;
		public virtual string lotteryOrder => _rawData._lotteryOrder;
		public virtual string categoryRateCorrectionJson => _rawData._categoryRateCorrectionJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBoardMasterValueObject _rawData = null;
		public TrainingBoardMasterObjectBase( TrainingBoardMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBoardMasterObject : TrainingBoardMasterObjectBase, IMasterObject {
		public TrainingBoardMasterObject( TrainingBoardMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBoardMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Board;

        [UnityEngine.SerializeField]
        TrainingBoardMasterValueObject[] m_Training_Board = null;
    }


}
