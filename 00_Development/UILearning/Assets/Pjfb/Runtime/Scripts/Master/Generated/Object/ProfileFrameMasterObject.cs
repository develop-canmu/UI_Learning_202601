//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ProfileFrameMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _mProfilePartId {get{ return mProfilePartId;} set{ this.mProfilePartId = value;}}
		[MessagePack.Key(3)]
		public bool _isPrimary {get{ return isPrimary;} set{ this.isPrimary = value;}}
		[MessagePack.Key(4)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(5)]
		public long _thumbnailImageId {get{ return thumbnailImageId;} set{ this.thumbnailImageId = value;}}
		[MessagePack.Key(6)]
		public long _imageEffectId {get{ return imageEffectId;} set{ this.imageEffectId = value;}}
		[MessagePack.Key(7)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long mProfilePartId = 0; // データ解放用のID
		[UnityEngine.SerializeField] bool isPrimary = false; // 初期から選択できるか。1=>可能、2=>不可能
		[UnityEngine.SerializeField] long imageId = 0; // 画像ID
		[UnityEngine.SerializeField] long thumbnailImageId = 0; // サムネイル画像ID
		[UnityEngine.SerializeField] long imageEffectId = 0; // エフェクト画像IDを指定
		[UnityEngine.SerializeField] string description = ""; // 説明文
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ProfileFrameMasterObjectBase {
		public virtual ProfileFrameMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long mProfilePartId => _rawData._mProfilePartId;
		public virtual bool isPrimary => _rawData._isPrimary;
		public virtual long imageId => _rawData._imageId;
		public virtual long thumbnailImageId => _rawData._thumbnailImageId;
		public virtual long imageEffectId => _rawData._imageEffectId;
		public virtual string description => _rawData._description;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ProfileFrameMasterValueObject _rawData = null;
		public ProfileFrameMasterObjectBase( ProfileFrameMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ProfileFrameMasterObject : ProfileFrameMasterObjectBase, IMasterObject {
		public ProfileFrameMasterObject( ProfileFrameMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ProfileFrameMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Profile_Frame;

        [UnityEngine.SerializeField]
        ProfileFrameMasterValueObject[] m_Profile_Frame = null;
    }


}
