//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class EmblemMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(3)]
		public long _mProfilePartId {get{ return mProfilePartId;} set{ this.mProfilePartId = value;}}
		[MessagePack.Key(4)]
		public bool _isPrimary {get{ return isPrimary;} set{ this.isPrimary = value;}}
		[MessagePack.Key(5)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(6)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(7)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(8)]
		public long _imageBaseId {get{ return imageBaseId;} set{ this.imageBaseId = value;}}
		[MessagePack.Key(9)]
		public long _imageEffectId {get{ return imageEffectId;} set{ this.imageEffectId = value;}}
		[MessagePack.Key(10)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] string description = ""; // 説明文
		[UnityEngine.SerializeField] long mProfilePartId = 0; // データ解放用のID
		[UnityEngine.SerializeField] bool isPrimary = false; // 初期から選択できるか。1=>可能、2=>不可能
		[UnityEngine.SerializeField] long mRarityId = 0; // m_rarityのid
		[UnityEngine.SerializeField] long priority = 0; // mRarityId内の表示優先度
		[UnityEngine.SerializeField] long imageId = 0; // 画像ID
		[UnityEngine.SerializeField] long imageBaseId = 0; // ベース画像ID
		[UnityEngine.SerializeField] long imageEffectId = 0; // エフェクト画像ID
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class EmblemMasterObjectBase {
		public virtual EmblemMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string description => _rawData._description;
		public virtual long mProfilePartId => _rawData._mProfilePartId;
		public virtual bool isPrimary => _rawData._isPrimary;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual long priority => _rawData._priority;
		public virtual long imageId => _rawData._imageId;
		public virtual long imageBaseId => _rawData._imageBaseId;
		public virtual long imageEffectId => _rawData._imageEffectId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        EmblemMasterValueObject _rawData = null;
		public EmblemMasterObjectBase( EmblemMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class EmblemMasterObject : EmblemMasterObjectBase, IMasterObject {
		public EmblemMasterObject( EmblemMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class EmblemMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Emblem;

        [UnityEngine.SerializeField]
        EmblemMasterValueObject[] m_Emblem = null;
    }


}
