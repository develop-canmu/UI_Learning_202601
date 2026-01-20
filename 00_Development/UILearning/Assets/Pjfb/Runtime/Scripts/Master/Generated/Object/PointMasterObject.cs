//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _max {get{ return max;} set{ this.max = value;}}
		[MessagePack.Key(3)]
		public long _pointType {get{ return pointType;} set{ this.pointType = value;}}
		[MessagePack.Key(4)]
		public long _lawType {get{ return lawType;} set{ this.lawType = value;}}
		[MessagePack.Key(5)]
		public string _symbolName {get{ return symbolName;} set{ this.symbolName = value;}}
		[MessagePack.Key(6)]
		public string _unitName {get{ return unitName;} set{ this.unitName = value;}}
		[MessagePack.Key(7)]
		public string _tradeTypeName {get{ return tradeTypeName;} set{ this.tradeTypeName = value;}}
		[MessagePack.Key(8)]
		public long _mPointCategoryId {get{ return mPointCategoryId;} set{ this.mPointCategoryId = value;}}
		[MessagePack.Key(9)]
		public long _mPointExpiryId {get{ return mPointExpiryId;} set{ this.mPointExpiryId = value;}}
		[MessagePack.Key(10)]
		public long _visibleType {get{ return visibleType;} set{ this.visibleType = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザ側で使用する名称
		[UnityEngine.SerializeField] long max = 0; // 所持数上限
		[UnityEngine.SerializeField] long pointType = 0; // ポイント種別1 => 個人課金消費型（課金通貨など）2 => 個人無料消費型（通常通貨など）3 => 個人無料累積型（ランキングPなど）4 => 個人無料累積型・非表示版（ミッションクリア判定用Pなど）11 => ギルド無料消費型（ギルドラ強化Pなど）13 => ギルド無料累積型（ギルドランキングPなど）21 => リアルマネー代替通貨
		[UnityEngine.SerializeField] long lawType = 0; // 景品表示法的にどのような扱いにするか0 => 特別な扱いはしない1 => おまけ（総付景品）として扱う
		[UnityEngine.SerializeField] string symbolName = ""; // 画像やスタイルを取得する際に使うキー文字列
		[UnityEngine.SerializeField] string unitName = ""; // そのポイントを数える際に用いる単位
		[UnityEngine.SerializeField] string tradeTypeName = ""; // そのポイントを消費して、物品を得ることをなんと呼称するか（購入・交換等）
		[UnityEngine.SerializeField] long mPointCategoryId = 0; // ポイントのカテゴリーIDを指定
		[UnityEngine.SerializeField] long mPointExpiryId = 0; // ポイントの失効設定。m_point_expiryのid
		[UnityEngine.SerializeField] long visibleType = 0; // 表示タイプ。クライアントで使用。1=>ポイントを持ってなかったら非表示、2=>持ってなくても表示（一度も取得したことがない場合でもクライアント側で所持数0でデフォルト表示）、3=>持ってても非表示、PJFBのみ現状使用可能
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointMasterObjectBase {
		public virtual PointMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long max => _rawData._max;
		public virtual long pointType => _rawData._pointType;
		public virtual long lawType => _rawData._lawType;
		public virtual string symbolName => _rawData._symbolName;
		public virtual string unitName => _rawData._unitName;
		public virtual string tradeTypeName => _rawData._tradeTypeName;
		public virtual long mPointCategoryId => _rawData._mPointCategoryId;
		public virtual long mPointExpiryId => _rawData._mPointExpiryId;
		public virtual long visibleType => _rawData._visibleType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointMasterValueObject _rawData = null;
		public PointMasterObjectBase( PointMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointMasterObject : PointMasterObjectBase, IMasterObject {
		public PointMasterObject( PointMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point;

        [UnityEngine.SerializeField]
        PointMasterValueObject[] m_Point = null;
    }


}
