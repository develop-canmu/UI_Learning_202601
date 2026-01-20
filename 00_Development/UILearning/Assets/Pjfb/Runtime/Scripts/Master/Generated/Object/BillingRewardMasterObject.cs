//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BillingRewardMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _storeType {get{ return storeType;} set{ this.storeType = value;}}
		[MessagePack.Key(3)]
		public long _appProductId {get{ return appProductId;} set{ this.appProductId = value;}}
		[MessagePack.Key(4)]
		public long _price {get{ return price;} set{ this.price = value;}}
		[MessagePack.Key(5)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(6)]
		public long _paidPointValue {get{ return paidPointValue;} set{ this.paidPointValue = value;}}
		[MessagePack.Key(7)]
		public double _pointGemRate {get{ return pointGemRate;} set{ this.pointGemRate = value;}}
		[MessagePack.Key(8)]
		public string _appleProductKey {get{ return appleProductKey;} set{ this.appleProductKey = value;}}
		[MessagePack.Key(9)]
		public string _googleProductKey {get{ return googleProductKey;} set{ this.googleProductKey = value;}}
		[MessagePack.Key(10)]
		public bool _displayFlg {get{ return displayFlg;} set{ this.displayFlg = value;}}
		[MessagePack.Key(11)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(12)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザ側で使用する名称
		[UnityEngine.SerializeField] long storeType = 0; // 課金処理で利用するストア区分
		[UnityEngine.SerializeField] long appProductId = 0; // アプリ側で購入するアイテムを判別する際に必要なID
		[UnityEngine.SerializeField] long price = 0; // 価格
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // 付与商品JSON
		[UnityEngine.SerializeField] long paidPointValue = 0; // 得られる有償通貨の量
		[UnityEngine.SerializeField] double pointGemRate = 0.0; //  m_billing_reward_bonus.prizeJsonの無償ジェム数量に対してかける倍率
		[UnityEngine.SerializeField] string appleProductKey = ""; // app store上での、対応する商品のproductKeyを設定する
		[UnityEngine.SerializeField] string googleProductKey = ""; // play store上での、対応する商品のproductKeyを設定する
		[UnityEngine.SerializeField] bool displayFlg = false; // 表示/非表示のフラグ（1 => 表示, 2 => 非表示）
		[UnityEngine.SerializeField] long displayPriority = 0; // 表示優先度（数値が大きいほど上に表示）
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BillingRewardMasterObjectBase {
		public virtual BillingRewardMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long storeType => _rawData._storeType;
		public virtual long appProductId => _rawData._appProductId;
		public virtual long price => _rawData._price;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual long paidPointValue => _rawData._paidPointValue;
		public virtual double pointGemRate => _rawData._pointGemRate;
		public virtual string appleProductKey => _rawData._appleProductKey;
		public virtual string googleProductKey => _rawData._googleProductKey;
		public virtual bool displayFlg => _rawData._displayFlg;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BillingRewardMasterValueObject _rawData = null;
		public BillingRewardMasterObjectBase( BillingRewardMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BillingRewardMasterObject : BillingRewardMasterObjectBase, IMasterObject {
		public BillingRewardMasterObject( BillingRewardMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BillingRewardMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Billing_Reward;

        [UnityEngine.SerializeField]
        BillingRewardMasterValueObject[] m_Billing_Reward = null;
    }


}
