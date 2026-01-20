//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class IconMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public bool _isPrimary {get{ return isPrimary;} set{ this.isPrimary = value;}}
		[MessagePack.Key(3)]
		public long _imageEffectId {get{ return imageEffectId;} set{ this.imageEffectId = value;}}
		[MessagePack.Key(4)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(5)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] bool isPrimary = false; // 初期から選択できるか
		[UnityEngine.SerializeField] long imageEffectId = 0; // エフェクト画像IDを指定
		[UnityEngine.SerializeField] long mRarityId = 0; // m_rarityのid
		[UnityEngine.SerializeField] long priority = 0; // mRarityId内の表示優先度
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class IconMasterObjectBase {
		public virtual IconMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual bool isPrimary => _rawData._isPrimary;
		public virtual long imageEffectId => _rawData._imageEffectId;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual long priority => _rawData._priority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        IconMasterValueObject _rawData = null;
		public IconMasterObjectBase( IconMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class IconMasterObject : IconMasterObjectBase, IMasterObject {
		public IconMasterObject( IconMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class IconMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Icon;

        [UnityEngine.SerializeField]
        IconMasterValueObject[] m_Icon = null;
    }


}
