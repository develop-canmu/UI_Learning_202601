//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointDescriptionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(2)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mPointId = 0; // ポイントID
		[UnityEngine.SerializeField] string description = ""; // ポイントの説明文
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointDescriptionMasterObjectBase {
		public virtual PointDescriptionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mPointId => _rawData._mPointId;
		public virtual string description => _rawData._description;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointDescriptionMasterValueObject _rawData = null;
		public PointDescriptionMasterObjectBase( PointDescriptionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointDescriptionMasterObject : PointDescriptionMasterObjectBase, IMasterObject {
		public PointDescriptionMasterObject( PointDescriptionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointDescriptionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Description;

        [UnityEngine.SerializeField]
        PointDescriptionMasterValueObject[] m_Point_Description = null;
    }


}
