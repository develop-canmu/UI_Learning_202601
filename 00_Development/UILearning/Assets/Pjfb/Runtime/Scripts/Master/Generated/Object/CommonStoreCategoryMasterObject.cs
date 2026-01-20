//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CommonStoreCategoryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _mCommonStoreLotteryCostCategoryId {get{ return mCommonStoreLotteryCostCategoryId;} set{ this.mCommonStoreLotteryCostCategoryId = value;}}
		[MessagePack.Key(4)]
		public long _lotteryCount {get{ return lotteryCount;} set{ this.lotteryCount = value;}}
		[MessagePack.Key(5)]
		public long[] _adminTagIdList {get{ return adminTagIdList;} set{ this.adminTagIdList = value;}}
		[MessagePack.Key(6)]
		public long _imageNumber {get{ return imageNumber;} set{ this.imageNumber = value;}}
		[MessagePack.Key(7)]
		public long _sliderDefaultValueType {get{ return sliderDefaultValueType;} set{ this.sliderDefaultValueType = value;}}
		[MessagePack.Key(8)]
		public long _sortPriority {get{ return sortPriority;} set{ this.sortPriority = value;}}
		[MessagePack.Key(9)]
		public bool _visibleFlg {get{ return visibleFlg;} set{ this.visibleFlg = value;}}
		[MessagePack.Key(10)]
		public bool _bulkButtonVisibleFlg {get{ return bulkButtonVisibleFlg;} set{ this.bulkButtonVisibleFlg = value;}}
		[MessagePack.Key(11)]
		public string _categoryGroupNumber {get{ return categoryGroupNumber;} set{ this.categoryGroupNumber = value;}}
		[MessagePack.Key(12)]
		public string _groupName {get{ return groupName;} set{ this.groupName = value;}}
		[MessagePack.Key(13)]
		public string _groupPriority {get{ return groupPriority;} set{ this.groupPriority = value;}}
		[MessagePack.Key(14)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}
		[MessagePack.Key(15)]
		public string _releaseDatetime {get{ return releaseDatetime;} set{ this.releaseDatetime = value;}}
		[MessagePack.Key(16)]
		public string _closedDatetime {get{ return closedDatetime;} set{ this.closedDatetime = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 店のユーザー向け名称を記述する
		[UnityEngine.SerializeField] long type = 0; // 店の種類。店をカテゴリ分けして表示するために使用され、値の意味はクライアント側の実装に依存する。
		[UnityEngine.SerializeField] long mCommonStoreLotteryCostCategoryId = 0; // 表示商品を再抽選する場合のコスト設定のID。そもそも表示商品の抽選をしない、あるいは抽選はするが再抽選はできないようにするなら 0 を指定する。
		[UnityEngine.SerializeField] long lotteryCount = 0; // 表示商品の抽選数。この数だけストア内の商品をランダムに抽選してゲーム画面上に表示する。表示商品の抽選をしない（全商品を常に表示する）場合は 0 を指定する。
		[UnityEngine.SerializeField] long[] adminTagIdList = null; // 店の表示に必要なタグのIDをJSON配列で指定する。必要なタグのうちいずれかひとつを所持していれば表示される。
		[UnityEngine.SerializeField] long imageNumber = 0; // バナー画像番号
		[UnityEngine.SerializeField] long sliderDefaultValueType = 0; // 商品個数選択のスライダーのデフォルト値を決める（0: 1、1: MAX）
		[UnityEngine.SerializeField] long sortPriority = 0; // 表示順。数字が大きいほど上に表示される。
		[UnityEngine.SerializeField] bool visibleFlg = false; // 可視フラグ
		[UnityEngine.SerializeField] bool bulkButtonVisibleFlg = false; // 一括交換ボタン表示フラグ
		[UnityEngine.SerializeField] string categoryGroupNumber = ""; //  番号ごとにストアカテゴリをグループ分け
		[UnityEngine.SerializeField] string groupName = ""; // グループ分けをした際のリンクボタン上の表示名
		[UnityEngine.SerializeField] string groupPriority = ""; // 同一グループを並べた際の表示順。数字が大きい方が左に表示される。
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除
		[UnityEngine.SerializeField] string releaseDatetime = ""; // 公開開始日時を記述する
		[UnityEngine.SerializeField] string closedDatetime = ""; // 公開終了日時を記述する

    }

    public class CommonStoreCategoryMasterObjectBase {
		public virtual CommonStoreCategoryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long type => _rawData._type;
		public virtual long mCommonStoreLotteryCostCategoryId => _rawData._mCommonStoreLotteryCostCategoryId;
		public virtual long lotteryCount => _rawData._lotteryCount;
		public virtual long[] adminTagIdList => _rawData._adminTagIdList;
		public virtual long imageNumber => _rawData._imageNumber;
		public virtual long sliderDefaultValueType => _rawData._sliderDefaultValueType;
		public virtual long sortPriority => _rawData._sortPriority;
		public virtual bool visibleFlg => _rawData._visibleFlg;
		public virtual bool bulkButtonVisibleFlg => _rawData._bulkButtonVisibleFlg;
		public virtual string categoryGroupNumber => _rawData._categoryGroupNumber;
		public virtual string groupName => _rawData._groupName;
		public virtual string groupPriority => _rawData._groupPriority;
		public virtual bool deleteFlg => _rawData._deleteFlg;
		public virtual string releaseDatetime => _rawData._releaseDatetime;
		public virtual string closedDatetime => _rawData._closedDatetime;

        CommonStoreCategoryMasterValueObject _rawData = null;
		public CommonStoreCategoryMasterObjectBase( CommonStoreCategoryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CommonStoreCategoryMasterObject : CommonStoreCategoryMasterObjectBase, IMasterObject {
		public CommonStoreCategoryMasterObject( CommonStoreCategoryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CommonStoreCategoryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Common_Store_Category;

        [UnityEngine.SerializeField]
        CommonStoreCategoryMasterValueObject[] m_Common_Store_Category = null;
    }


}
