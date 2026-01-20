//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class StaminaUseToMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mStaminaId {get{ return mStaminaId;} set{ this.mStaminaId = value;}}
		[MessagePack.Key(2)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mStaminaId = 0; // $mStaminaId スタミナID
		[UnityEngine.SerializeField] long useType = 0; // $useType 用途区分：1 => クエスト用, 2 => 名称未定PVEコンテンツ用, 3 => 名称未定PVPコンテンツ用, 4 => トレーニング（サクセス）, 5 => 自動トレーニング（サクセス）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class StaminaUseToMasterObjectBase {
		public virtual StaminaUseToMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mStaminaId => _rawData._mStaminaId;
		public virtual long useType => _rawData._useType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        StaminaUseToMasterValueObject _rawData = null;
		public StaminaUseToMasterObjectBase( StaminaUseToMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class StaminaUseToMasterObject : StaminaUseToMasterObjectBase, IMasterObject {
		public StaminaUseToMasterObject( StaminaUseToMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class StaminaUseToMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Stamina_Use_To;

        [UnityEngine.SerializeField]
        StaminaUseToMasterValueObject[] m_Stamina_Use_To = null;
    }


}
