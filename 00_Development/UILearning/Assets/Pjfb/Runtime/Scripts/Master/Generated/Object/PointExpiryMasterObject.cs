//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointExpiryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _expiryType {get{ return expiryType;} set{ this.expiryType = value;}}
		[MessagePack.Key(2)]
		public long _expiryDays {get{ return expiryDays;} set{ this.expiryDays = value;}}
		[MessagePack.Key(3)]
		public string _expiryDate {get{ return expiryDate;} set{ this.expiryDate = value;}}
		[MessagePack.Key(4)]
		public string _giftBoxMessage {get{ return giftBoxMessage;} set{ this.giftBoxMessage = value;}}
		[MessagePack.Key(5)]
		public long _itemPreviewType {get{ return itemPreviewType;} set{ this.itemPreviewType = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long expiryType = 0; // $expiryType 有効期限区分。1=>日数指定、 2=>期日指定
		[UnityEngine.SerializeField] long expiryDays = 0; // $expiryDays expiryType = 1のとき参照。有効な日数を指定（付与後X日の0:00:00が期限日になる。日未満の時刻は切り捨てになるため、同日0時と23時の付与ポイントは同一の有効期限になる）
		[UnityEngine.SerializeField] string expiryDate = ""; // $expiryDate expireType = 2のとき参照。期限切れ期日を記載（20250421と記載した場合20250419 23:59:59まで有効で、20250421 00:00:00に期限切れとなる）
		[UnityEngine.SerializeField] string giftBoxMessage = ""; // $giftBoxMessage giftBoxに投入する際のメッセージ文言
		[UnityEngine.SerializeField] long itemPreviewType = 0; // $itemPreviewType アイテム閲覧画面等での見え方についての記載。1=>通常アイテム（ポイント）と同等の見栄えになる、2=>履歴や有効期限をユーザーに明示する形で表示する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointExpiryMasterObjectBase {
		public virtual PointExpiryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long expiryType => _rawData._expiryType;
		public virtual long expiryDays => _rawData._expiryDays;
		public virtual string expiryDate => _rawData._expiryDate;
		public virtual string giftBoxMessage => _rawData._giftBoxMessage;
		public virtual long itemPreviewType => _rawData._itemPreviewType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointExpiryMasterValueObject _rawData = null;
		public PointExpiryMasterObjectBase( PointExpiryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointExpiryMasterObject : PointExpiryMasterObjectBase, IMasterObject {
		public PointExpiryMasterObject( PointExpiryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointExpiryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Expiry;

        [UnityEngine.SerializeField]
        PointExpiryMasterValueObject[] m_Point_Expiry = null;
    }


}
