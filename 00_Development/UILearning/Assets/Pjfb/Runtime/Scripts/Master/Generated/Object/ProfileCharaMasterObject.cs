//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ProfileCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _mProfilePartId {get{ return mProfilePartId;} set{ this.mProfilePartId = value;}}
		[MessagePack.Key(3)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(4)]
		public bool _isPrimary {get{ return isPrimary;} set{ this.isPrimary = value;}}
		[MessagePack.Key(5)]
		public long _mCharaParentId {get{ return mCharaParentId;} set{ this.mCharaParentId = value;}}
		[MessagePack.Key(6)]
		public long _effectType {get{ return effectType;} set{ this.effectType = value;}}
		[MessagePack.Key(7)]
		public long _imageEffectId {get{ return imageEffectId;} set{ this.imageEffectId = value;}}
		[MessagePack.Key(8)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(9)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(10)]
		public long _displayCharaImageId {get{ return displayCharaImageId;} set{ this.displayCharaImageId = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long mProfilePartId = 0; // データ解放用のID
		[UnityEngine.SerializeField] long mCharaId = 0; // 対象のmCharaIdを所持していた場合に解放
		[UnityEngine.SerializeField] bool isPrimary = false; // 初期から選択できるか。1=>可能、2=>不可能
		[UnityEngine.SerializeField] long mCharaParentId = 0; // 紐づく親キャラID
		[UnityEngine.SerializeField] long effectType = 0; // 1=>エフェクトなし、2=>エフェクトあり
		[UnityEngine.SerializeField] long imageEffectId = 0; // エフェクト画像IDを指定
		[UnityEngine.SerializeField] long priority = 0; // 表示優先度
		[UnityEngine.SerializeField] string description = ""; // 説明文
		[UnityEngine.SerializeField] long displayCharaImageId = 0; // 選手の画像ID
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ProfileCharaMasterObjectBase {
		public virtual ProfileCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long mProfilePartId => _rawData._mProfilePartId;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual bool isPrimary => _rawData._isPrimary;
		public virtual long mCharaParentId => _rawData._mCharaParentId;
		public virtual long effectType => _rawData._effectType;
		public virtual long imageEffectId => _rawData._imageEffectId;
		public virtual long priority => _rawData._priority;
		public virtual string description => _rawData._description;
		public virtual long displayCharaImageId => _rawData._displayCharaImageId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ProfileCharaMasterValueObject _rawData = null;
		public ProfileCharaMasterObjectBase( ProfileCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ProfileCharaMasterObject : ProfileCharaMasterObjectBase, IMasterObject {
		public ProfileCharaMasterObject( ProfileCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ProfileCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Profile_Chara;

        [UnityEngine.SerializeField]
        ProfileCharaMasterValueObject[] m_Profile_Chara = null;
    }


}
