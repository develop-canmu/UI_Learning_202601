//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BillingRewardAlternativePointMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mBillingRewardId {get{ return mBillingRewardId;} set{ this.mBillingRewardId = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(4)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(5)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(6)]
		public long _mServerId {get{ return mServerId;} set{ this.mServerId = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mBillingRewardId = 0; // 課金商品ID
		[UnityEngine.SerializeField] long mPointId = 0; // 交換に必要なポイントのID
		[UnityEngine.SerializeField] long value = 0; // 交換に必要なポイント数
		[UnityEngine.SerializeField] string startAt = ""; // 開始日時
		[UnityEngine.SerializeField] string endAt = ""; // 終了日時
		[UnityEngine.SerializeField] long mServerId = 0; // サーバーID
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BillingRewardAlternativePointMasterObjectBase {
		public virtual BillingRewardAlternativePointMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mBillingRewardId => _rawData._mBillingRewardId;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual long mServerId => _rawData._mServerId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BillingRewardAlternativePointMasterValueObject _rawData = null;
		public BillingRewardAlternativePointMasterObjectBase( BillingRewardAlternativePointMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BillingRewardAlternativePointMasterObject : BillingRewardAlternativePointMasterObjectBase, IMasterObject {
		public BillingRewardAlternativePointMasterObject( BillingRewardAlternativePointMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BillingRewardAlternativePointMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Billing_Reward_Alternative_Point;

        [UnityEngine.SerializeField]
        BillingRewardAlternativePointMasterValueObject[] m_Billing_Reward_Alternative_Point = null;
    }


}
