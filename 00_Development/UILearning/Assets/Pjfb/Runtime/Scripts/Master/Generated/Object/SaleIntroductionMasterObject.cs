//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class SaleIntroductionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _imageFilePath {get{ return imageFilePath;} set{ this.imageFilePath = value;}}
		[MessagePack.Key(3)]
		public long _mBillingRewardBonusId {get{ return mBillingRewardBonusId;} set{ this.mBillingRewardBonusId = value;}}
		[MessagePack.Key(4)]
		public bool _remindsAtHome {get{ return remindsAtHome;} set{ this.remindsAtHome = value;}}
		[MessagePack.Key(5)]
		public long _displayType {get{ return displayType;} set{ this.displayType = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] string imageFilePath = ""; // 画像ファイル名。フルネイティブタイトルの場合は画像ファイル名ではなく画像番号を指定する。
		[UnityEngine.SerializeField] long mBillingRewardBonusId = 0; // 課金連携ボーナスID
		[UnityEngine.SerializeField] bool remindsAtHome = false; // home/index 遷移時に再度セールのポップアップを表示するかどうか
		[UnityEngine.SerializeField] long displayType = 0; // 表示種別。どこでポップアップを表示するかを指定。クライアント側で使用
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class SaleIntroductionMasterObjectBase {
		public virtual SaleIntroductionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string imageFilePath => _rawData._imageFilePath;
		public virtual long mBillingRewardBonusId => _rawData._mBillingRewardBonusId;
		public virtual bool remindsAtHome => _rawData._remindsAtHome;
		public virtual long displayType => _rawData._displayType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        SaleIntroductionMasterValueObject _rawData = null;
		public SaleIntroductionMasterObjectBase( SaleIntroductionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class SaleIntroductionMasterObject : SaleIntroductionMasterObjectBase, IMasterObject {
		public SaleIntroductionMasterObject( SaleIntroductionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class SaleIntroductionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Sale_Introduction;

        [UnityEngine.SerializeField]
        SaleIntroductionMasterValueObject[] m_Sale_Introduction = null;
    }


}
