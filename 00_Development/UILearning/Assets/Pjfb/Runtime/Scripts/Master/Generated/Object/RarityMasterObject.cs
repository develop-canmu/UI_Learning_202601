//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class RarityMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(3)]
		public long _frameId {get{ return frameId;} set{ this.frameId = value;}}
		[MessagePack.Key(4)]
		public long _detailImageType {get{ return detailImageType;} set{ this.detailImageType = value;}}
		[MessagePack.Key(5)]
		public bool _reinforcementFlg {get{ return reinforcementFlg;} set{ this.reinforcementFlg = value;}}
		[MessagePack.Key(6)]
		public long _cardType {get{ return cardType;} set{ this.cardType = value;}}
		[MessagePack.Key(7)]
		public string _displayStartAt {get{ return displayStartAt;} set{ this.displayStartAt = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long value = 0; // レアリティ
		[UnityEngine.SerializeField] long frameId = 0; // サムネ枠画像のID
		[UnityEngine.SerializeField] long detailImageType = 0; // UIパーツ格納先のディレクトリ
		[UnityEngine.SerializeField] bool reinforcementFlg = false; // 合成時警告フラグ 1 => 警告する, 2 => 警告しない
		[UnityEngine.SerializeField] long cardType = 0; // カード区分（キャラクターカード・スペシャルサポートカード等の役割に相当。インゲームのルールによって、環境ごとに設定する）
		[UnityEngine.SerializeField] string displayStartAt = ""; // $displayStartAt 表示開始日時
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class RarityMasterObjectBase {
		public virtual RarityMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long value => _rawData._value;
		public virtual long frameId => _rawData._frameId;
		public virtual long detailImageType => _rawData._detailImageType;
		public virtual bool reinforcementFlg => _rawData._reinforcementFlg;
		public virtual long cardType => _rawData._cardType;
		public virtual string displayStartAt => _rawData._displayStartAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        RarityMasterValueObject _rawData = null;
		public RarityMasterObjectBase( RarityMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class RarityMasterObject : RarityMasterObjectBase, IMasterObject {
		public RarityMasterObject( RarityMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class RarityMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Rarity;

        [UnityEngine.SerializeField]
        RarityMasterValueObject[] m_Rarity = null;
    }


}
