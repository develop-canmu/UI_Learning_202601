//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ItemLockedSettingMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _unlockType {get{ return unlockType;} set{ this.unlockType = value;}}
		[MessagePack.Key(2)]
		public string _unlockValue {get{ return unlockValue;} set{ this.unlockValue = value;}}
		[MessagePack.Key(3)]
		public long _expireType {get{ return expireType;} set{ this.expireType = value;}}
		[MessagePack.Key(4)]
		public string _expireValue {get{ return expireValue;} set{ this.expireValue = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long unlockType = 0; // 獲得から受け取れるようになるまでの期限の種類(1 => 日時指定、 2 => 時間、3 => 日数)
		[UnityEngine.SerializeField] string unlockValue = ""; // 獲得から受け取れるようになるまでの期限。unlockTypeの種別によって形式が異なる
		[UnityEngine.SerializeField] long expireType = 0; // 解放から受け取りできなくなるようになるまでの期限の種類(1 => 日時指定、 2 => 時間、3 => 日数)
		[UnityEngine.SerializeField] string expireValue = ""; // 解放から受け取りできなくなるようになるまでの期限。expireTypeの種別によって形式が異なる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ItemLockedSettingMasterObjectBase {
		public virtual ItemLockedSettingMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long unlockType => _rawData._unlockType;
		public virtual string unlockValue => _rawData._unlockValue;
		public virtual long expireType => _rawData._expireType;
		public virtual string expireValue => _rawData._expireValue;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ItemLockedSettingMasterValueObject _rawData = null;
		public ItemLockedSettingMasterObjectBase( ItemLockedSettingMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ItemLockedSettingMasterObject : ItemLockedSettingMasterObjectBase, IMasterObject {
		public ItemLockedSettingMasterObject( ItemLockedSettingMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ItemLockedSettingMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Item_Locked_Setting;

        [UnityEngine.SerializeField]
        ItemLockedSettingMasterValueObject[] m_Item_Locked_Setting = null;
    }


}
