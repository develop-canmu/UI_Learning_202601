//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CommonStoreMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCommonStoreCategoryId {get{ return mCommonStoreCategoryId;} set{ this.mCommonStoreCategoryId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(4)]
		public long _costMPointId {get{ return costMPointId;} set{ this.costMPointId = value;}}
		[MessagePack.Key(5)]
		public long _costValue {get{ return costValue;} set{ this.costValue = value;}}
		[MessagePack.Key(6)]
		public long _buyLimit {get{ return buyLimit;} set{ this.buyLimit = value;}}
		[MessagePack.Key(7)]
		public long _buyLimitResetCycleType {get{ return buyLimitResetCycleType;} set{ this.buyLimitResetCycleType = value;}}
		[MessagePack.Key(8)]
		public string _releaseDatetime {get{ return releaseDatetime;} set{ this.releaseDatetime = value;}}
		[MessagePack.Key(9)]
		public string _closedDatetime {get{ return closedDatetime;} set{ this.closedDatetime = value;}}
		[MessagePack.Key(10)]
		public bool _viewFlg {get{ return viewFlg;} set{ this.viewFlg = value;}}
		[MessagePack.Key(11)]
		public long _emphasisType {get{ return emphasisType;} set{ this.emphasisType = value;}}
		[MessagePack.Key(12)]
		public long _sortPriority {get{ return sortPriority;} set{ this.sortPriority = value;}}
		[MessagePack.Key(13)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(14)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mCommonStoreCategoryId = 0; // 汎用ストア種別IDを指定
		[UnityEngine.SerializeField] string name = ""; // 商品名
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // 付与する商品をJSON形式で記述する
		[UnityEngine.SerializeField] long costMPointId = 0; // 購入時に消費するポイントのID
		[UnityEngine.SerializeField] long costValue = 0; // 購入時に消費するポイントの量
		[UnityEngine.SerializeField] long buyLimit = 0; // 個人の購入回数上限を記述する。0以下の値の場合は制限なしとみなす。
		[UnityEngine.SerializeField] long buyLimitResetCycleType = 0; // 個人の購入回数のリセット方式。詳しい説明は schema.d に記載。
		[UnityEngine.SerializeField] string releaseDatetime = ""; // 購入ページ上での交換開始日時を記述する
		[UnityEngine.SerializeField] string closedDatetime = ""; // 購入ページ上での交換終了日時を記述する
		[UnityEngine.SerializeField] bool viewFlg = false; // 表示フラグ
		[UnityEngine.SerializeField] long emphasisType = 0; // 強調タイプ(0 => 通常, 1 => 強調タイプ1) デフォルト0
		[UnityEngine.SerializeField] long sortPriority = 0; // 表示順。数字が大きいほど上に表示される。
		[UnityEngine.SerializeField] long priority = 0; // 一括交換の優先度
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CommonStoreMasterObjectBase {
		public virtual CommonStoreMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCommonStoreCategoryId => _rawData._mCommonStoreCategoryId;
		public virtual string name => _rawData._name;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual long costMPointId => _rawData._costMPointId;
		public virtual long costValue => _rawData._costValue;
		public virtual long buyLimit => _rawData._buyLimit;
		public virtual long buyLimitResetCycleType => _rawData._buyLimitResetCycleType;
		public virtual string releaseDatetime => _rawData._releaseDatetime;
		public virtual string closedDatetime => _rawData._closedDatetime;
		public virtual bool viewFlg => _rawData._viewFlg;
		public virtual long emphasisType => _rawData._emphasisType;
		public virtual long sortPriority => _rawData._sortPriority;
		public virtual long priority => _rawData._priority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CommonStoreMasterValueObject _rawData = null;
		public CommonStoreMasterObjectBase( CommonStoreMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CommonStoreMasterObject : CommonStoreMasterObjectBase, IMasterObject {
		public CommonStoreMasterObject( CommonStoreMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CommonStoreMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Common_Store;

        [UnityEngine.SerializeField]
        CommonStoreMasterValueObject[] m_Common_Store = null;
    }


}
