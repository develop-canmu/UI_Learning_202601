//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class StatusAdditionLevelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mStatusAdditionId {get{ return mStatusAdditionId;} set{ this.mStatusAdditionId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _maxLevelGrowth {get{ return maxLevelGrowth;} set{ this.maxLevelGrowth = value;}}
		[MessagePack.Key(4)]
		public long _hp {get{ return hp;} set{ this.hp = value;}}
		[MessagePack.Key(5)]
		public long _mp {get{ return mp;} set{ this.mp = value;}}
		[MessagePack.Key(6)]
		public long _atk {get{ return atk;} set{ this.atk = value;}}
		[MessagePack.Key(7)]
		public long _def {get{ return def;} set{ this.def = value;}}
		[MessagePack.Key(8)]
		public long _spd {get{ return spd;} set{ this.spd = value;}}
		[MessagePack.Key(9)]
		public long _tec {get{ return tec;} set{ this.tec = value;}}
		[MessagePack.Key(10)]
		public long _param1 {get{ return param1;} set{ this.param1 = value;}}
		[MessagePack.Key(11)]
		public long _param2 {get{ return param2;} set{ this.param2 = value;}}
		[MessagePack.Key(12)]
		public long _param3 {get{ return param3;} set{ this.param3 = value;}}
		[MessagePack.Key(13)]
		public long _param4 {get{ return param4;} set{ this.param4 = value;}}
		[MessagePack.Key(14)]
		public long _param5 {get{ return param5;} set{ this.param5 = value;}}
		[MessagePack.Key(15)]
		public long _hpRate {get{ return hpRate;} set{ this.hpRate = value;}}
		[MessagePack.Key(16)]
		public long _mpRate {get{ return mpRate;} set{ this.mpRate = value;}}
		[MessagePack.Key(17)]
		public long _atkRate {get{ return atkRate;} set{ this.atkRate = value;}}
		[MessagePack.Key(18)]
		public long _defRate {get{ return defRate;} set{ this.defRate = value;}}
		[MessagePack.Key(19)]
		public long _spdRate {get{ return spdRate;} set{ this.spdRate = value;}}
		[MessagePack.Key(20)]
		public long _tecRate {get{ return tecRate;} set{ this.tecRate = value;}}
		[MessagePack.Key(21)]
		public long _param1Rate {get{ return param1Rate;} set{ this.param1Rate = value;}}
		[MessagePack.Key(22)]
		public long _param2Rate {get{ return param2Rate;} set{ this.param2Rate = value;}}
		[MessagePack.Key(23)]
		public long _param3Rate {get{ return param3Rate;} set{ this.param3Rate = value;}}
		[MessagePack.Key(24)]
		public long _param4Rate {get{ return param4Rate;} set{ this.param4Rate = value;}}
		[MessagePack.Key(25)]
		public long _param5Rate {get{ return param5Rate;} set{ this.param5Rate = value;}}
		[MessagePack.Key(26)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mStatusAdditionId = 0; // 成長テーブルID
		[UnityEngine.SerializeField] long level = 0; // その成長テーブル上での、成長段階
		[UnityEngine.SerializeField] long maxLevelGrowth = 0; // 育成レベル上限ベース
		[UnityEngine.SerializeField] long hp = 0; // HP上昇値
		[UnityEngine.SerializeField] long mp = 0; // MP上昇値
		[UnityEngine.SerializeField] long atk = 0; // 攻撃力上昇値
		[UnityEngine.SerializeField] long def = 0; // 防御力上昇値
		[UnityEngine.SerializeField] long spd = 0; // 素早さ上昇値
		[UnityEngine.SerializeField] long tec = 0; // 技巧上昇値
		[UnityEngine.SerializeField] long param1 = 0; // 追加パラメーター1上昇値（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param2 = 0; // 追加パラメーター2上昇値（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param3 = 0; // 追加パラメーター3上昇値（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param4 = 0; // 追加パラメーター4上昇値（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param5 = 0; // 追加パラメーター5上昇値（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long hpRate = 0; // HP上昇倍率[%]
		[UnityEngine.SerializeField] long mpRate = 0; // MP上昇倍率[%]
		[UnityEngine.SerializeField] long atkRate = 0; // 攻撃力上昇倍率[%]
		[UnityEngine.SerializeField] long defRate = 0; // 防御力上昇倍率[%]
		[UnityEngine.SerializeField] long spdRate = 0; // 素早さ上昇倍率[%]
		[UnityEngine.SerializeField] long tecRate = 0; // 技巧上昇倍率[%]
		[UnityEngine.SerializeField] long param1Rate = 0; // 追加パラメーター1上昇倍率[%]（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param2Rate = 0; // 追加パラメーター2上昇倍率[%]（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param3Rate = 0; // 追加パラメーター3上昇倍率[%]（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param4Rate = 0; // 追加パラメーター4上昇倍率[%]（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param5Rate = 0; // 追加パラメーター5上昇倍率[%]（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class StatusAdditionLevelMasterObjectBase {
		public virtual StatusAdditionLevelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mStatusAdditionId => _rawData._mStatusAdditionId;
		public virtual long level => _rawData._level;
		public virtual long maxLevelGrowth => _rawData._maxLevelGrowth;
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
		public virtual long hpRate => _rawData._hpRate;
		public virtual long mpRate => _rawData._mpRate;
		public virtual long atkRate => _rawData._atkRate;
		public virtual long defRate => _rawData._defRate;
		public virtual long spdRate => _rawData._spdRate;
		public virtual long tecRate => _rawData._tecRate;
		public virtual long param1Rate => _rawData._param1Rate;
		public virtual long param2Rate => _rawData._param2Rate;
		public virtual long param3Rate => _rawData._param3Rate;
		public virtual long param4Rate => _rawData._param4Rate;
		public virtual long param5Rate => _rawData._param5Rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        StatusAdditionLevelMasterValueObject _rawData = null;
		public StatusAdditionLevelMasterObjectBase( StatusAdditionLevelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class StatusAdditionLevelMasterObject : StatusAdditionLevelMasterObjectBase, IMasterObject {
		public StatusAdditionLevelMasterObject( StatusAdditionLevelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class StatusAdditionLevelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Status_Addition_Level;

        [UnityEngine.SerializeField]
        StatusAdditionLevelMasterValueObject[] m_Status_Addition_Level = null;
    }


}
