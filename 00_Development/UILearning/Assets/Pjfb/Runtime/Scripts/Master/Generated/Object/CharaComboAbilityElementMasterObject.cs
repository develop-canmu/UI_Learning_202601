//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaComboAbilityElementMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaComboAbilityId {get{ return mCharaComboAbilityId;} set{ this.mCharaComboAbilityId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(3)]
		public bool _isTarget {get{ return isTarget;} set{ this.isTarget = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaComboAbilityId = 0; // $mCharaComboAbilityId コンボスキルのID
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId 対象キャラID
		[UnityEngine.SerializeField] bool isTarget = false; // $isTarget スキルの付与対象かどうか（1 => 付与対象, 2 => 付与対象ではない）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaComboAbilityElementMasterObjectBase {
		public virtual CharaComboAbilityElementMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaComboAbilityId => _rawData._mCharaComboAbilityId;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual bool isTarget => _rawData._isTarget;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaComboAbilityElementMasterValueObject _rawData = null;
		public CharaComboAbilityElementMasterObjectBase( CharaComboAbilityElementMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaComboAbilityElementMasterObject : CharaComboAbilityElementMasterObjectBase, IMasterObject {
		public CharaComboAbilityElementMasterObject( CharaComboAbilityElementMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaComboAbilityElementMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Combo_Ability_Element;

        [UnityEngine.SerializeField]
        CharaComboAbilityElementMasterValueObject[] m_Chara_Combo_Ability_Element = null;
    }


}
