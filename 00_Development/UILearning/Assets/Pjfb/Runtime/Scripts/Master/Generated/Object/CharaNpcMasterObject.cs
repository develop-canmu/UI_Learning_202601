//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaNpcMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public string _visualKey {get{ return visualKey;} set{ this.visualKey = value;}}
		[MessagePack.Key(3)]
		public long _hp {get{ return hp;} set{ this.hp = value;}}
		[MessagePack.Key(4)]
		public long _mp {get{ return mp;} set{ this.mp = value;}}
		[MessagePack.Key(5)]
		public long _atk {get{ return atk;} set{ this.atk = value;}}
		[MessagePack.Key(6)]
		public long _def {get{ return def;} set{ this.def = value;}}
		[MessagePack.Key(7)]
		public long _spd {get{ return spd;} set{ this.spd = value;}}
		[MessagePack.Key(8)]
		public long _tec {get{ return tec;} set{ this.tec = value;}}
		[MessagePack.Key(9)]
		public long _param1 {get{ return param1;} set{ this.param1 = value;}}
		[MessagePack.Key(10)]
		public long _param2 {get{ return param2;} set{ this.param2 = value;}}
		[MessagePack.Key(11)]
		public long _param3 {get{ return param3;} set{ this.param3 = value;}}
		[MessagePack.Key(12)]
		public long _param4 {get{ return param4;} set{ this.param4 = value;}}
		[MessagePack.Key(13)]
		public long _param5 {get{ return param5;} set{ this.param5 = value;}}
		[MessagePack.Key(14)]
		public WrapperIntList[] _abilityList {get{ return abilityList;} set{ this.abilityList = value;}}
		[MessagePack.Key(15)]
		public WrapperIntList[] _abilityPassiveList {get{ return abilityPassiveList;} set{ this.abilityPassiveList = value;}}
		[MessagePack.Key(16)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId 性能ベース mCharaId
		[UnityEngine.SerializeField] string visualKey = ""; // $visualKey キャラIDか画像パス（キャラの見た目指定用）
		[UnityEngine.SerializeField] long hp = 0; // $hp hp
		[UnityEngine.SerializeField] long mp = 0; // $mp mp
		[UnityEngine.SerializeField] long atk = 0; // $atk atk
		[UnityEngine.SerializeField] long def = 0; // $def def
		[UnityEngine.SerializeField] long spd = 0; // $spd spd
		[UnityEngine.SerializeField] long tec = 0; // $tec tec
		[UnityEngine.SerializeField] long param1 = 0; // $param1 param1
		[UnityEngine.SerializeField] long param2 = 0; // $param2 param2
		[UnityEngine.SerializeField] long param3 = 0; // $param3 param3
		[UnityEngine.SerializeField] long param4 = 0; // $param4 param4
		[UnityEngine.SerializeField] long param5 = 0; // $param5 param5
		[UnityEngine.SerializeField] WrapperIntList[] abilityList = null; // $abilityList アクティブスキルを表現するのに必要なJSON構造
		[UnityEngine.SerializeField] WrapperIntList[] abilityPassiveList = null; // $abilityPassiveList パッシブスキルを表現するのに必要なJSON構造
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaNpcMasterObjectBase {
		public virtual CharaNpcMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual string visualKey => _rawData._visualKey;
		public virtual long hp => _rawData._hp;
		public virtual long mp => _rawData._mp;
		public virtual long atk => _rawData._atk;
		public virtual long def => _rawData._def;
		public virtual long spd => _rawData._spd;
		public virtual long tec => _rawData._tec;
		public virtual long param1 => _rawData._param1;
		public virtual long param2 => _rawData._param2;
		public virtual long param3 => _rawData._param3;
		public virtual long param4 => _rawData._param4;
		public virtual long param5 => _rawData._param5;
		public virtual WrapperIntList[] abilityList => _rawData._abilityList;
		public virtual WrapperIntList[] abilityPassiveList => _rawData._abilityPassiveList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaNpcMasterValueObject _rawData = null;
		public CharaNpcMasterObjectBase( CharaNpcMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaNpcMasterObject : CharaNpcMasterObjectBase, IMasterObject {
		public CharaNpcMasterObject( CharaNpcMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaNpcMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Npc;

        [UnityEngine.SerializeField]
        CharaNpcMasterValueObject[] m_Chara_Npc = null;
    }


}
