//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaComboAbilityMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _requireCount {get{ return requireCount;} set{ this.requireCount = value;}}
		[MessagePack.Key(3)]
		public long[] _mAbilityIdList {get{ return mAbilityIdList;} set{ this.mAbilityIdList = value;}}
		[MessagePack.Key(4)]
		public long[] _abilityLevelList {get{ return abilityLevelList;} set{ this.abilityLevelList = value;}}
		[MessagePack.Key(5)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(6)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long requireCount = 0; // $requireCount 発動に必要なキャラの数
		[UnityEngine.SerializeField] long[] mAbilityIdList = null; // 発動スキルID一覧（int配列になるjson）
		[UnityEngine.SerializeField] long[] abilityLevelList = null; // 発動スキルレベル一覧（int配列になるjson）
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 効果開始日時。クライアント側でも表示に用いる
		[UnityEngine.SerializeField] long sortNumber = 0; // $sortNumber 表示順。デフォルトでは昇順
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaComboAbilityMasterObjectBase {
		public virtual CharaComboAbilityMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long requireCount => _rawData._requireCount;
		public virtual long[] mAbilityIdList => _rawData._mAbilityIdList;
		public virtual long[] abilityLevelList => _rawData._abilityLevelList;
		public virtual string startAt => _rawData._startAt;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaComboAbilityMasterValueObject _rawData = null;
		public CharaComboAbilityMasterObjectBase( CharaComboAbilityMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaComboAbilityMasterObject : CharaComboAbilityMasterObjectBase, IMasterObject {
		public CharaComboAbilityMasterObject( CharaComboAbilityMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaComboAbilityMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Combo_Ability;

        [UnityEngine.SerializeField]
        CharaComboAbilityMasterValueObject[] m_Chara_Combo_Ability = null;
    }


}
