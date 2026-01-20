//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainingComboBuffElementMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainingComboBuffId {get{ return mCharaTrainingComboBuffId;} set{ this.mCharaTrainingComboBuffId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainingComboBuffId = 0; // $mCharaTrainingComboBuffId バフID
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId キャラID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainingComboBuffElementMasterObjectBase {
		public virtual CharaTrainingComboBuffElementMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainingComboBuffId => _rawData._mCharaTrainingComboBuffId;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainingComboBuffElementMasterValueObject _rawData = null;
		public CharaTrainingComboBuffElementMasterObjectBase( CharaTrainingComboBuffElementMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainingComboBuffElementMasterObject : CharaTrainingComboBuffElementMasterObjectBase, IMasterObject {
		public CharaTrainingComboBuffElementMasterObject( CharaTrainingComboBuffElementMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainingComboBuffElementMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Training_Combo_Buff_Element;

        [UnityEngine.SerializeField]
        CharaTrainingComboBuffElementMasterValueObject[] m_Chara_Training_Combo_Buff_Element = null;
    }


}
