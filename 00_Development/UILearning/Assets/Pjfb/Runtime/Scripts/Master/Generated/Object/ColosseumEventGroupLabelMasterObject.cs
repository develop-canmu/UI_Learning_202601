//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumEventGroupLabelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 表示優先度
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumEventGroupLabelMasterObjectBase {
		public virtual ColosseumEventGroupLabelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumEventGroupLabelMasterValueObject _rawData = null;
		public ColosseumEventGroupLabelMasterObjectBase( ColosseumEventGroupLabelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumEventGroupLabelMasterObject : ColosseumEventGroupLabelMasterObjectBase, IMasterObject {
		public ColosseumEventGroupLabelMasterObject( ColosseumEventGroupLabelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumEventGroupLabelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Event_Group_Label;

        [UnityEngine.SerializeField]
        ColosseumEventGroupLabelMasterValueObject[] m_Colosseum_Event_Group_Label = null;
    }


}
