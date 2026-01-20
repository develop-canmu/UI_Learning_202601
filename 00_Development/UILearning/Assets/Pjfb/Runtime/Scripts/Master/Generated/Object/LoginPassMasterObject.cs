//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class LoginPassMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mBillingRewardBonusId {get{ return mBillingRewardBonusId;} set{ this.mBillingRewardBonusId = value;}}
		[MessagePack.Key(2)]
		public long _adminTagId {get{ return adminTagId;} set{ this.adminTagId = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public long _pointDaily {get{ return pointDaily;} set{ this.pointDaily = value;}}
		[MessagePack.Key(5)]
		public long _pointTotal {get{ return pointTotal;} set{ this.pointTotal = value;}}
		[MessagePack.Key(6)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mBillingRewardBonusId = 0; // 課金パックID
		[UnityEngine.SerializeField] long adminTagId = 0; // タグID（m_billing_reward_bonus.prizeJson で付与するタグのIDと一致させる）
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long pointDaily = 0; // デイリーでもらえるジェムの数量（m_login_stamp_prize.prizeJson で付与するジェムの個数と一致させる）
		[UnityEngine.SerializeField] long pointTotal = 0; // 合計でもらえるジェムの数量（m_login_stamp_prize.prizeJson で付与するジェムの合計個数と一致させる）
		[UnityEngine.SerializeField] string description = ""; // ユーザーに対する提示内容を自由フォーマットで記入
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class LoginPassMasterObjectBase {
		public virtual LoginPassMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mBillingRewardBonusId => _rawData._mBillingRewardBonusId;
		public virtual long adminTagId => _rawData._adminTagId;
		public virtual string name => _rawData._name;
		public virtual long pointDaily => _rawData._pointDaily;
		public virtual long pointTotal => _rawData._pointTotal;
		public virtual string description => _rawData._description;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        LoginPassMasterValueObject _rawData = null;
		public LoginPassMasterObjectBase( LoginPassMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class LoginPassMasterObject : LoginPassMasterObjectBase, IMasterObject {
		public LoginPassMasterObject( LoginPassMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class LoginPassMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Login_Pass;

        [UnityEngine.SerializeField]
        LoginPassMasterValueObject[] m_Login_Pass = null;
    }


}
