//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class SystemLockMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(3)]
		public long _triggerType {get{ return triggerType;} set{ this.triggerType = value;}}
		[MessagePack.Key(4)]
		public long _triggerValue {get{ return triggerValue;} set{ this.triggerValue = value;}}
		[MessagePack.Key(5)]
		public long _systemNumber {get{ return systemNumber;} set{ this.systemNumber = value;}}
		[MessagePack.Key(6)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(7)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(8)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(9)]
		public long _showsEffect {get{ return showsEffect;} set{ this.showsEffect = value;}}
		[MessagePack.Key(10)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] string adminName = ""; // 管理用名称
		[UnityEngine.SerializeField] long triggerType = 0; // トリガー種別。1 => デッキランク到達度、 2 => ストーリー到達
		[UnityEngine.SerializeField] long triggerValue = 0; // トリガー種別によって違う値。1 => デッキランク値、 2 => m_hunt_enemy.id
		[UnityEngine.SerializeField] long systemNumber = 0; // 解放対象システム番号
		[UnityEngine.SerializeField] string description = ""; // 説明文
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // 報酬情報
		[UnityEngine.SerializeField] long priority = 0; // 解放演出の表示優先度
		[UnityEngine.SerializeField] long showsEffect = 0; // 解放演出表示フラグ
		[UnityEngine.SerializeField] string startAt = ""; // 適用開始日時。ただし負荷軽減のため、ユーザーログイン時の機能解放チェック処理は、そのユーザーの前回までの最終ログイン時刻より未来の時刻になっているレコードのみ対象となる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除（デフォルトでロックされている）, 2 => 削除（デフォルトでアンロックされている）

    }

    public class SystemLockMasterObjectBase {
		public virtual SystemLockMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string adminName => _rawData._adminName;
		public virtual long triggerType => _rawData._triggerType;
		public virtual long triggerValue => _rawData._triggerValue;
		public virtual long systemNumber => _rawData._systemNumber;
		public virtual string description => _rawData._description;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual long priority => _rawData._priority;
		public virtual long showsEffect => _rawData._showsEffect;
		public virtual string startAt => _rawData._startAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        SystemLockMasterValueObject _rawData = null;
		public SystemLockMasterObjectBase( SystemLockMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class SystemLockMasterObject : SystemLockMasterObjectBase, IMasterObject {
		public SystemLockMasterObject( SystemLockMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class SystemLockMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_System_Lock;

        [UnityEngine.SerializeField]
        SystemLockMasterValueObject[] m_System_Lock = null;
    }


}
