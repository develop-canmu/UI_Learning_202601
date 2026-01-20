//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingUnitMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _requireCount {get{ return requireCount;} set{ this.requireCount = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long type = 0; // $type ユニットのタイプ（サーバー的には区別しないが、ユーザーに対して何という名目で見せるかをクライアント・運用側が区別したい場合、別々の値を入れる）
		[UnityEngine.SerializeField] long requireCount = 0; // $requireCount 要求数
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingUnitMasterObjectBase {
		public virtual TrainingUnitMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long type => _rawData._type;
		public virtual long requireCount => _rawData._requireCount;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingUnitMasterValueObject _rawData = null;
		public TrainingUnitMasterObjectBase( TrainingUnitMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingUnitMasterObject : TrainingUnitMasterObjectBase, IMasterObject {
		public TrainingUnitMasterObject( TrainingUnitMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingUnitMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Unit;

        [UnityEngine.SerializeField]
        TrainingUnitMasterValueObject[] m_Training_Unit = null;
    }


}
