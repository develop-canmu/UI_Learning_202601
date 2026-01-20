//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingStatusTypeDetailCategoryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(3)]
		public long _detailCategoryId {get{ return detailCategoryId;} set{ this.detailCategoryId = value;}}
		[MessagePack.Key(4)]
		public long _targetType {get{ return targetType;} set{ this.targetType = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 表示優先度
		[UnityEngine.SerializeField] long detailCategoryId = 0; // $detailCategoryId 詳細カテゴリID
		[UnityEngine.SerializeField] long targetType = 0; // $targetType 種別
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingStatusTypeDetailCategoryMasterObjectBase {
		public virtual TrainingStatusTypeDetailCategoryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual long detailCategoryId => _rawData._detailCategoryId;
		public virtual long targetType => _rawData._targetType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingStatusTypeDetailCategoryMasterValueObject _rawData = null;
		public TrainingStatusTypeDetailCategoryMasterObjectBase( TrainingStatusTypeDetailCategoryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingStatusTypeDetailCategoryMasterObject : TrainingStatusTypeDetailCategoryMasterObjectBase, IMasterObject {
		public TrainingStatusTypeDetailCategoryMasterObject( TrainingStatusTypeDetailCategoryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingStatusTypeDetailCategoryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Status_Type_Detail_Category;

        [UnityEngine.SerializeField]
        TrainingStatusTypeDetailCategoryMasterValueObject[] m_Training_Status_Type_Detail_Category = null;
    }


}
