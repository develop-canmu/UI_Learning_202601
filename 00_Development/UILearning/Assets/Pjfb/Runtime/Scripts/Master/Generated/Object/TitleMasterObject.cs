//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TitleMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _mBackgroundId {get{ return mBackgroundId;} set{ this.mBackgroundId = value;}}
		[MessagePack.Key(4)]
		public long _imageEffectId {get{ return imageEffectId;} set{ this.imageEffectId = value;}}
		[MessagePack.Key(5)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(6)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(7)]
		public long _displayType {get{ return displayType;} set{ this.displayType = value;}}
		[MessagePack.Key(8)]
		public string _displayName {get{ return displayName;} set{ this.displayName = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long type = 0; // 称号タイプ指定 1 => バナー背景両方, 2 => バナーのみ, 3 => 背景のみ
		[UnityEngine.SerializeField] long mBackgroundId = 0; // 称号に紐づけるIDを指定
		[UnityEngine.SerializeField] long imageEffectId = 0; // エフェクト画像IDを指定
		[UnityEngine.SerializeField] long mRarityId = 0; // m_rarityのid
		[UnityEngine.SerializeField] long priority = 0; // mRarityId内の表示優先度
		[UnityEngine.SerializeField] long displayType = 0; // 称号画像に載せるテキストの表示方法。称号画像に文字を入れられない多言語版などで使用する。
		[UnityEngine.SerializeField] string displayName = ""; // 称号画像に載せるテキスト。称号画像に文字を入れられない多言語版などで使用する。
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TitleMasterObjectBase {
		public virtual TitleMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long type => _rawData._type;
		public virtual long mBackgroundId => _rawData._mBackgroundId;
		public virtual long imageEffectId => _rawData._imageEffectId;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual long priority => _rawData._priority;
		public virtual long displayType => _rawData._displayType;
		public virtual string displayName => _rawData._displayName;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TitleMasterValueObject _rawData = null;
		public TitleMasterObjectBase( TitleMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TitleMasterObject : TitleMasterObjectBase, IMasterObject {
		public TitleMasterObject( TitleMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TitleMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Title;

        [UnityEngine.SerializeField]
        TitleMasterValueObject[] m_Title = null;
    }


}
