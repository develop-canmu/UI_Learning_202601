//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class StaminaCureMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mStaminaId {get{ return mStaminaId;} set{ this.mStaminaId = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(4)]
		public long _cureType {get{ return cureType;} set{ this.cureType = value;}}
		[MessagePack.Key(5)]
		public long _cureValue {get{ return cureValue;} set{ this.cureValue = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mStaminaId = 0; // $mStaminaId スタミナID
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId 使用するポイントのID
		[UnityEngine.SerializeField] long value = 0; // $value 使用するポイントの量
		[UnityEngine.SerializeField] long cureType = 0; // $cureType 回復種別：1 => 実数値加算、2 => 最大値割合回復（最大100%）
		[UnityEngine.SerializeField] long cureValue = 0; // $cureValue 回復量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class StaminaCureMasterObjectBase {
		public virtual StaminaCureMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mStaminaId => _rawData._mStaminaId;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual long cureType => _rawData._cureType;
		public virtual long cureValue => _rawData._cureValue;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        StaminaCureMasterValueObject _rawData = null;
		public StaminaCureMasterObjectBase( StaminaCureMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class StaminaCureMasterObject : StaminaCureMasterObjectBase, IMasterObject {
		public StaminaCureMasterObject( StaminaCureMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class StaminaCureMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Stamina_Cure;

        [UnityEngine.SerializeField]
        StaminaCureMasterValueObject[] m_Stamina_Cure = null;
    }


}
