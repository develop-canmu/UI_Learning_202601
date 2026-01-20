//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class AbilityTrainingPointStatusMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mAbilityId {get{ return mAbilityId;} set{ this.mAbilityId = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mAbilityId = 0; // アビリティID
		[UnityEngine.SerializeField] long type = 0; // トレーニング専用ポイントのステータス種別。クライアント側で表示分けに使用。0=>通常、1=>mTrainingPointStatus専用
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class AbilityTrainingPointStatusMasterObjectBase {
		public virtual AbilityTrainingPointStatusMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mAbilityId => _rawData._mAbilityId;
		public virtual long type => _rawData._type;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        AbilityTrainingPointStatusMasterValueObject _rawData = null;
		public AbilityTrainingPointStatusMasterObjectBase( AbilityTrainingPointStatusMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class AbilityTrainingPointStatusMasterObject : AbilityTrainingPointStatusMasterObjectBase, IMasterObject {
		public AbilityTrainingPointStatusMasterObject( AbilityTrainingPointStatusMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class AbilityTrainingPointStatusMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Ability_Training_Point_Status;

        [UnityEngine.SerializeField]
        AbilityTrainingPointStatusMasterValueObject[] m_Ability_Training_Point_Status = null;
    }


}
