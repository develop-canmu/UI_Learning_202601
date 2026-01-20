//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class GachaRetryPriceMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mGachaRetryId {get{ return mGachaRetryId;} set{ this.mGachaRetryId = value;}}
		[MessagePack.Key(2)]
		public long _invokeCount {get{ return invokeCount;} set{ this.invokeCount = value;}}
		[MessagePack.Key(3)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(4)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}
		[MessagePack.Key(6)]
		public string _adjustAdminPrice {get{ return adjustAdminPrice;} set{ this.adjustAdminPrice = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mGachaRetryId = 0; // $mGachaRetryId リトライ設定ID
		[UnityEngine.SerializeField] long invokeCount = 0; // $invokeCount 実行回数（0始まり）
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId コストポイントID
		[UnityEngine.SerializeField] long value = 0; // $value コストポイント量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除
		[UnityEngine.SerializeField] string adjustAdminPrice = ""; // ガチャ１回あたりの価値(10連等は回数で割った値)の減算値

    }

    public class GachaRetryPriceMasterObjectBase {
		public virtual GachaRetryPriceMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mGachaRetryId => _rawData._mGachaRetryId;
		public virtual long invokeCount => _rawData._invokeCount;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;
		public virtual string adjustAdminPrice => _rawData._adjustAdminPrice;

        GachaRetryPriceMasterValueObject _rawData = null;
		public GachaRetryPriceMasterObjectBase( GachaRetryPriceMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class GachaRetryPriceMasterObject : GachaRetryPriceMasterObjectBase, IMasterObject {
		public GachaRetryPriceMasterObject( GachaRetryPriceMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class GachaRetryPriceMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Gacha_Retry_Price;

        [UnityEngine.SerializeField]
        GachaRetryPriceMasterValueObject[] m_Gacha_Retry_Price = null;
    }


}
