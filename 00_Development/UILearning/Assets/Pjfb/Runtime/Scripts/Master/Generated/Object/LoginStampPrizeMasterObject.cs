//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class LoginStampPrizeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _prizeType {get{ return prizeType;} set{ this.prizeType = value;}}
		[MessagePack.Key(2)]
		public long _prizeNumber {get{ return prizeNumber;} set{ this.prizeNumber = value;}}
		[MessagePack.Key(3)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(4)]
		public string _dialog {get{ return dialog;} set{ this.dialog = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // id
		[UnityEngine.SerializeField] long prizeType = 0; // 報酬区分（m_login_stamp.prizeTypeと対応するためだけに使うやつ）
		[UnityEngine.SerializeField] long prizeNumber = 0; // 報酬区分（m_login_stampと対応するためだけに使うやつ）
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // 報酬json情報
		[UnityEngine.SerializeField] string dialog = ""; // ログインボーナス画面で表示されるテキスト。キャラクターにセリフとして喋らせることも多いため message とは別に定義している。
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class LoginStampPrizeMasterObjectBase {
		public virtual LoginStampPrizeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long prizeType => _rawData._prizeType;
		public virtual long prizeNumber => _rawData._prizeNumber;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual string dialog => _rawData._dialog;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        LoginStampPrizeMasterValueObject _rawData = null;
		public LoginStampPrizeMasterObjectBase( LoginStampPrizeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class LoginStampPrizeMasterObject : LoginStampPrizeMasterObjectBase, IMasterObject {
		public LoginStampPrizeMasterObject( LoginStampPrizeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class LoginStampPrizeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Login_Stamp_Prize;

        [UnityEngine.SerializeField]
        LoginStampPrizeMasterValueObject[] m_Login_Stamp_Prize = null;
    }


}
