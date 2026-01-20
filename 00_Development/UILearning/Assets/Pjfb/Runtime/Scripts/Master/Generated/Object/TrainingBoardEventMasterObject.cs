//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBoardEventMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(3)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(4)]
		public long _searchType {get{ return searchType;} set{ this.searchType = value;}}
		[MessagePack.Key(5)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(6)]
		public long _mTrainingUnitId {get{ return mTrainingUnitId;} set{ this.mTrainingUnitId = value;}}
		[MessagePack.Key(7)]
		public long _mTrainingBoardEventCategoryId {get{ return mTrainingBoardEventCategoryId;} set{ this.mTrainingBoardEventCategoryId = value;}}
		[MessagePack.Key(8)]
		public long _mTrainingBoardEventForkGroupId {get{ return mTrainingBoardEventForkGroupId;} set{ this.mTrainingBoardEventForkGroupId = value;}}
		[MessagePack.Key(9)]
		public long _conditionTierMin {get{ return conditionTierMin;} set{ this.conditionTierMin = value;}}
		[MessagePack.Key(10)]
		public long _conditionTierMax {get{ return conditionTierMax;} set{ this.conditionTierMax = value;}}
		[MessagePack.Key(11)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(12)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(13)]
		public long _effectType {get{ return effectType;} set{ this.effectType = value;}}
		[MessagePack.Key(14)]
		public long _masterId {get{ return masterId;} set{ this.masterId = value;}}
		[MessagePack.Key(15)]
		public long _effectValue {get{ return effectValue;} set{ this.effectValue = value;}}
		[MessagePack.Key(16)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(17)]
		public long _rarity {get{ return rarity;} set{ this.rarity = value;}}
		[MessagePack.Key(18)]
		public long _suddenRate {get{ return suddenRate;} set{ this.suddenRate = value;}}
		[MessagePack.Key(19)]
		public bool _canRepeat {get{ return canRepeat;} set{ this.canRepeat = value;}}
		[MessagePack.Key(20)]
		public string _excludedTurnList {get{ return excludedTurnList;} set{ this.excludedTurnList = value;}}
		[MessagePack.Key(21)]
		public bool _isPreserved {get{ return isPreserved;} set{ this.isPreserved = value;}}
		[MessagePack.Key(22)]
		public long _sourceType {get{ return sourceType;} set{ this.sourceType = value;}}
		[MessagePack.Key(23)]
		public long _overwritePriority {get{ return overwritePriority;} set{ this.overwritePriority = value;}}
		[MessagePack.Key(24)]
		public string _lotteryOrder {get{ return lotteryOrder;} set{ this.lotteryOrder = value;}}
		[MessagePack.Key(25)]
		public string _occurrenceCountRateJson {get{ return occurrenceCountRateJson;} set{ this.occurrenceCountRateJson = value;}}
		[MessagePack.Key(26)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理表示名
		[UnityEngine.SerializeField] string description = ""; // $description 効果等の説明文
		[UnityEngine.SerializeField] long searchType = 0; // $searchType 検索タイプ。0以外の値を指定した場合、指定した検索方法でのみヒットする。1: シナリオ由来(mTrainingScenarioId), 2: キャラ由来(mTrainingBoardEventChara), 3: ユニット由来(mTrainingUnitId)
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId シナリオID。searchTypeが1の場合、この値により検索を行う。searchTypeが1以外の場合、0以外の値を設定すると検索結果のフィルタリングに使用され、プレイ中のシナリオIDと一致しない場合、その結果は除外される
		[UnityEngine.SerializeField] long mTrainingUnitId = 0; // $mTrainingUnitId ユニットID。searchTypeが3の場合、この値により検索を行う。searchTypeが3以外の場合、0以外の値を設定すると検索結果のフィルタリングに使用され、ユニットが成立していない場合、その結果は除外される
		[UnityEngine.SerializeField] long mTrainingBoardEventCategoryId = 0; // $mTrainingBoardEventCategoryId マスイベントカテゴリID
		[UnityEngine.SerializeField] long mTrainingBoardEventForkGroupId = 0; // $mTrainingBoardEventForkGroupId 分岐グループID。分岐グループが同じマスイベントのみ、分岐の組み合わせとして採用されうる
		[UnityEngine.SerializeField] long conditionTierMin = 0; // $conditionTierMin 本マスイベントが発生しうるコンディションのティア最小値。m_training_board.conditionTier がこの値以上となるようなマス盤上にのみ、このマスイベントが発生する
		[UnityEngine.SerializeField] long conditionTierMax = 0; // $conditionTierMax 本マスイベントが発生しうるコンディションのティア最大値。m_training_board.conditionTier がこの値以下となるようなマス盤上にのみ、このマスイベントが発生する
		[UnityEngine.SerializeField] long priority = 0; // $priority 優先度
		[UnityEngine.SerializeField] long rate = 0; // $rate 確率重み
		[UnityEngine.SerializeField] long effectType = 0; // $effectType 効果タイプ。0 => なし、1 => 臨時練習能力等の獲得、2 => イベント発生、3 => 特定練習カードの発生、4 => m_training_event_reward の獲得
		[UnityEngine.SerializeField] long masterId = 0; // $masterId マスイベント内容ID。参照するテーブルはeffectTypeによって異なる。0 => 参照されない、1 => mTrainingBoardEventContentGroupId、2 => mTrainingEventId、3 => mTrainingCardId、4 => mTrainingEventRewardId
		[UnityEngine.SerializeField] long effectValue = 0; // $effectValue 効果値または効果に付随する補助定数。効果タイプごとに意味合いが異なる。effectType:1 の場合、効果が継続するターン数。effectType:2 の場合、発生イベントを予約するtiming
		[UnityEngine.SerializeField] long imageId = 0; // $imageId 画像ID
		[UnityEngine.SerializeField] long rarity = 0; // $rarity レア度
		[UnityEngine.SerializeField] long suddenRate = 0; // $suddenRate 隠しイベント（空白マスに見えるがマスを踏んだときに突如効果が発生するように見える）となる確率
		[UnityEngine.SerializeField] bool canRepeat = false; // $canRepeat 繰り返し発生可能かどうか
		[UnityEngine.SerializeField] string excludedTurnList = ""; // $excludedTurnList 抽選を避けるターンのリスト。[5,10]のような形で設定すれば、5ターン目、10ターン目にはこのイベントは抽選されなくなる
		[UnityEngine.SerializeField] bool isPreserved = false; // $isPreserved 複数のコンディション間でイベントの発生場所（ターン数）が保存されるか。mTrainingBoardEventForkGroupId = 0 かつ canRepeat = 2 のときのみ指定可能で、どのコンディション帯のときでも同じターンにこのイベントを配置したい場合は1を指定する
		[UnityEngine.SerializeField] long sourceType = 0; // $sourceType マスイベント種別。1 => 通常イベント、2 => サポートマスイベント（m_chara.cardType:12 のキャラ（不可変サポートキャラ）が持つもの）。サポートマスイベントはマスイベント配置時の抽選手法が異なり、配置優先度が高くなる
		[UnityEngine.SerializeField] long overwritePriority = 0; // $overwritePriority 上書き優先度。サポートマスイベント配置時に既にマスイベントが配置されていた場合、各イベントの上書き優先度により上書きの是非を定める。シナリオ進行度（battleCount）に寄与する試合マスの上書き優先度は正の無限大、空白マスの上書き優先度は負の無限大として取り扱う
		[UnityEngine.SerializeField] string lotteryOrder = ""; // $lotteryOrder マス抽選優先順序づけ。サポートマスイベントのみ使用。[5,10]のような形で設定すれば、5ターン目、10ターン目の順にマス配置抽選を優先的に行う（他は順不同）。battleCount を横断して指定可能
		[UnityEngine.SerializeField] string occurrenceCountRateJson = ""; // $occurrenceCountRateJson 発生回数確率設定。サポートマスイベントのみ使用。本マスイベントが発生する回数ごとに確率を指定する。[[1,6000],[2,4000]] のように指定すると、60％で1回出現・40％で2回出現、という設定となる。指定がない場合は必ず1回のみ抽選される
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBoardEventMasterObjectBase {
		public virtual TrainingBoardEventMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string adminName => _rawData._adminName;
		public virtual string description => _rawData._description;
		public virtual long searchType => _rawData._searchType;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long mTrainingUnitId => _rawData._mTrainingUnitId;
		public virtual long mTrainingBoardEventCategoryId => _rawData._mTrainingBoardEventCategoryId;
		public virtual long mTrainingBoardEventForkGroupId => _rawData._mTrainingBoardEventForkGroupId;
		public virtual long conditionTierMin => _rawData._conditionTierMin;
		public virtual long conditionTierMax => _rawData._conditionTierMax;
		public virtual long priority => _rawData._priority;
		public virtual long rate => _rawData._rate;
		public virtual long effectType => _rawData._effectType;
		public virtual long masterId => _rawData._masterId;
		public virtual long effectValue => _rawData._effectValue;
		public virtual long imageId => _rawData._imageId;
		public virtual long rarity => _rawData._rarity;
		public virtual long suddenRate => _rawData._suddenRate;
		public virtual bool canRepeat => _rawData._canRepeat;
		public virtual string excludedTurnList => _rawData._excludedTurnList;
		public virtual bool isPreserved => _rawData._isPreserved;
		public virtual long sourceType => _rawData._sourceType;
		public virtual long overwritePriority => _rawData._overwritePriority;
		public virtual string lotteryOrder => _rawData._lotteryOrder;
		public virtual string occurrenceCountRateJson => _rawData._occurrenceCountRateJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBoardEventMasterValueObject _rawData = null;
		public TrainingBoardEventMasterObjectBase( TrainingBoardEventMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBoardEventMasterObject : TrainingBoardEventMasterObjectBase, IMasterObject {
		public TrainingBoardEventMasterObject( TrainingBoardEventMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBoardEventMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Board_Event;

        [UnityEngine.SerializeField]
        TrainingBoardEventMasterValueObject[] m_Training_Board_Event = null;
    }


}
