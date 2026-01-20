//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckExtraTirednessMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(2)]
		public long _valueMax {get{ return valueMax;} set{ this.valueMax = value;}}
		[MessagePack.Key(3)]
		public long _valueMaxToAction {get{ return valueMaxToAction;} set{ this.valueMaxToAction = value;}}
		[MessagePack.Key(4)]
		public long _cureType {get{ return cureType;} set{ this.cureType = value;}}
		[MessagePack.Key(5)]
		public long _cureUnitMinutes {get{ return cureUnitMinutes;} set{ this.cureUnitMinutes = value;}}
		[MessagePack.Key(6)]
		public long _cureValue {get{ return cureValue;} set{ this.cureValue = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long useType = 0; // $useType 特殊用途番号。現在1001, 1101, 1201, 1301が存在。重複しないように設定する。
		[UnityEngine.SerializeField] long valueMax = 0; // $valueMax 最大値。これ以上は大きくならない。
		[UnityEngine.SerializeField] long valueMaxToAction = 0; // $valueMaxToAction そのデッキを使って行動を行わせる際の最大値
		[UnityEngine.SerializeField] long cureType = 0; // $cureType （3 ⇒ 1日をcureUnitMinutes単位に区切った上でその区切りの時刻をまたいだ場合に回復が発生※これをターンの周期と合わせるイメージ）
		[UnityEngine.SerializeField] long cureUnitMinutes = 0; // $cureUnitMinutes 回復単位時間
		[UnityEngine.SerializeField] long cureValue = 0; // $cureValue 回復量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckExtraTirednessMasterObjectBase {
		public virtual DeckExtraTirednessMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long useType => _rawData._useType;
		public virtual long valueMax => _rawData._valueMax;
		public virtual long valueMaxToAction => _rawData._valueMaxToAction;
		public virtual long cureType => _rawData._cureType;
		public virtual long cureUnitMinutes => _rawData._cureUnitMinutes;
		public virtual long cureValue => _rawData._cureValue;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckExtraTirednessMasterValueObject _rawData = null;
		public DeckExtraTirednessMasterObjectBase( DeckExtraTirednessMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckExtraTirednessMasterObject : DeckExtraTirednessMasterObjectBase, IMasterObject {
		public DeckExtraTirednessMasterObject( DeckExtraTirednessMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckExtraTirednessMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Extra_Tiredness;

        [UnityEngine.SerializeField]
        DeckExtraTirednessMasterValueObject[] m_Deck_Extra_Tiredness = null;
    }


}
