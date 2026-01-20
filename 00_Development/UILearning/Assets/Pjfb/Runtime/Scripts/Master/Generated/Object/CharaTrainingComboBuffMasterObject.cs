//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainingComboBuffMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(3)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 効果開始日時。クライアント側でも表示に用いる
		[UnityEngine.SerializeField] long sortNumber = 0; // $sortNumber 表示順。デフォルトでは昇順
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainingComboBuffMasterObjectBase {
		public virtual CharaTrainingComboBuffMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string startAt => _rawData._startAt;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainingComboBuffMasterValueObject _rawData = null;
		public CharaTrainingComboBuffMasterObjectBase( CharaTrainingComboBuffMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainingComboBuffMasterObject : CharaTrainingComboBuffMasterObjectBase, IMasterObject {
		public CharaTrainingComboBuffMasterObject( CharaTrainingComboBuffMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainingComboBuffMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Training_Combo_Buff;

        [UnityEngine.SerializeField]
        CharaTrainingComboBuffMasterValueObject[] m_Chara_Training_Combo_Buff = null;
    }


}
