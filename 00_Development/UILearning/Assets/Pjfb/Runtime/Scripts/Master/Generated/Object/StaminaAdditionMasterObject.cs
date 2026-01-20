//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class StaminaAdditionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mStaminaId {get{ return mStaminaId;} set{ this.mStaminaId = value;}}
		[MessagePack.Key(2)]
		public long _adminTagId {get{ return adminTagId;} set{ this.adminTagId = value;}}
		[MessagePack.Key(3)]
		public long _additionValue {get{ return additionValue;} set{ this.additionValue = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mStaminaId = 0; // $mStaminaId スタミナマスタID。設計上、無用になった情報も検索に引っかかり続けてしまうので、不要になったらここを0等に変更して退避させてほしい
		[UnityEngine.SerializeField] long adminTagId = 0; // $adminTagId タグマスタID
		[UnityEngine.SerializeField] long additionValue = 0; // $additionValue 追加スタミナ量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class StaminaAdditionMasterObjectBase {
		public virtual StaminaAdditionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mStaminaId => _rawData._mStaminaId;
		public virtual long adminTagId => _rawData._adminTagId;
		public virtual long additionValue => _rawData._additionValue;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        StaminaAdditionMasterValueObject _rawData = null;
		public StaminaAdditionMasterObjectBase( StaminaAdditionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class StaminaAdditionMasterObject : StaminaAdditionMasterObjectBase, IMasterObject {
		public StaminaAdditionMasterObject( StaminaAdditionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class StaminaAdditionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Stamina_Addition;

        [UnityEngine.SerializeField]
        StaminaAdditionMasterValueObject[] m_Stamina_Addition = null;
    }


}
