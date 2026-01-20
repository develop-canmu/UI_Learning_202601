//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BattleGameliftMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public long _eventType {get{ return eventType;} set{ this.eventType = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public string _deckLockAt {get{ return deckLockAt;} set{ this.deckLockAt = value;}}
		[MessagePack.Key(5)]
		public string _battleStartAt {get{ return battleStartAt;} set{ this.battleStartAt = value;}}
		[MessagePack.Key(6)]
		public string _battleStartAtSub {get{ return battleStartAtSub;} set{ this.battleStartAtSub = value;}}
		[MessagePack.Key(7)]
		public string _battleFinishAt {get{ return battleFinishAt;} set{ this.battleFinishAt = value;}}
		[MessagePack.Key(8)]
		public string _availableItemJson {get{ return availableItemJson;} set{ this.availableItemJson = value;}}
		[MessagePack.Key(9)]
		public string _optionJson {get{ return optionJson;} set{ this.optionJson = value;}}
		[MessagePack.Key(10)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long type = 0; // $type ゲームタイプ。これによりサーバ側でクライアントに渡すマスタ情報を選定する。1 => SHIN同盟戦,2 => PJFB
		[UnityEngine.SerializeField] long eventType = 0; // $eventType どのマッチングシステムによって管理されているか。1 => colosseum
		[UnityEngine.SerializeField] string name = ""; // $name ゲーム名
		[UnityEngine.SerializeField] string deckLockAt = ""; // $deckLockAt デッキ編成ロック時刻。日付部分は参照せず、時刻部分のみ見る（1980-01-01 [12:30]の[]部分のみ参照する）
		[UnityEngine.SerializeField] string battleStartAt = ""; // $battleStartAt 対戦開始時刻。同様に時刻部分のみ参照
		[UnityEngine.SerializeField] string battleStartAtSub = ""; // $battleStartAtSub 対戦開始サブ時刻。battleStartAtの後に、それとは別に本格的に対戦が始動する時刻を設定する必要がある場合等に使用する。現状サーバ側では使用しない。同様に時刻部分のみ参照
		[UnityEngine.SerializeField] string battleFinishAt = ""; // $battleFinishAt 対戦終了時刻。同様に時刻部分のみ参照
		[UnityEngine.SerializeField] string availableItemJson = ""; // $availableItemJson 使用可能なアイテムや効果を指定するJSON。[{"mPointId":X,"value":A,"effectId":Z,"maxCount":Y},...] の形式で複数指定できる。maxCountは持ち込み可能数
		[UnityEngine.SerializeField] string optionJson = ""; // $optionJson typeごとに異なる設定値を指定
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BattleGameliftMasterObjectBase {
		public virtual BattleGameliftMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual long eventType => _rawData._eventType;
		public virtual string name => _rawData._name;
		public virtual string deckLockAt => _rawData._deckLockAt;
		public virtual string battleStartAt => _rawData._battleStartAt;
		public virtual string battleStartAtSub => _rawData._battleStartAtSub;
		public virtual string battleFinishAt => _rawData._battleFinishAt;
		public virtual string availableItemJson => _rawData._availableItemJson;
		public virtual string optionJson => _rawData._optionJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BattleGameliftMasterValueObject _rawData = null;
		public BattleGameliftMasterObjectBase( BattleGameliftMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BattleGameliftMasterObject : BattleGameliftMasterObjectBase, IMasterObject {
		public BattleGameliftMasterObject( BattleGameliftMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BattleGameliftMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Battle_Gamelift;

        [UnityEngine.SerializeField]
        BattleGameliftMasterValueObject[] m_Battle_Gamelift = null;
    }


}
