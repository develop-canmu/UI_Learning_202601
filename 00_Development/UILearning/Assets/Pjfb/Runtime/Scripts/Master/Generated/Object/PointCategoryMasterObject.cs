//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointCategoryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public bool _visibleFlg {get{ return visibleFlg;} set{ this.visibleFlg = value;}}
		[MessagePack.Key(3)]
		public long _mServerId {get{ return mServerId;} set{ this.mServerId = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}
		[MessagePack.Key(5)]
		public string _releaseDatetime {get{ return releaseDatetime;} set{ this.releaseDatetime = value;}}
		[MessagePack.Key(6)]
		public string _closedDatetime {get{ return closedDatetime;} set{ this.closedDatetime = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザー向け名称を記述する
		[UnityEngine.SerializeField] bool visibleFlg = false; // 可視フラグ
		[UnityEngine.SerializeField] long mServerId = 0; // サーバーID
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除
		[UnityEngine.SerializeField] string releaseDatetime = ""; // 公開開始日時を記述する
		[UnityEngine.SerializeField] string closedDatetime = ""; // 公開終了日時を記述する

    }

    public class PointCategoryMasterObjectBase {
		public virtual PointCategoryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual bool visibleFlg => _rawData._visibleFlg;
		public virtual long mServerId => _rawData._mServerId;
		public virtual bool deleteFlg => _rawData._deleteFlg;
		public virtual string releaseDatetime => _rawData._releaseDatetime;
		public virtual string closedDatetime => _rawData._closedDatetime;

        PointCategoryMasterValueObject _rawData = null;
		public PointCategoryMasterObjectBase( PointCategoryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointCategoryMasterObject : PointCategoryMasterObjectBase, IMasterObject {
		public PointCategoryMasterObject( PointCategoryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointCategoryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Category;

        [UnityEngine.SerializeField]
        PointCategoryMasterValueObject[] m_Point_Category = null;
    }


}
