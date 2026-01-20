//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CommonStoreLotteryCostMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCommonStoreLotteryCostCategoryId {get{ return mCommonStoreLotteryCostCategoryId;} set{ this.mCommonStoreLotteryCostCategoryId = value;}}
		[MessagePack.Key(2)]
		public long _stepNumber {get{ return stepNumber;} set{ this.stepNumber = value;}}
		[MessagePack.Key(3)]
		public long _availableCount {get{ return availableCount;} set{ this.availableCount = value;}}
		[MessagePack.Key(4)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(5)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mCommonStoreLotteryCostCategoryId = 0; // 店商品再抽選コストカテゴリID
		[UnityEngine.SerializeField] long stepNumber = 0; // ステップ番号
		[UnityEngine.SerializeField] long availableCount = 0; // このコストで再抽選を行える回数
		[UnityEngine.SerializeField] long mPointId = 0; // 再抽選時に消費するポイントのID（0にした場合は無料で再抽選できる）
		[UnityEngine.SerializeField] long value = 0; // 再抽選時に消費するポイントの量（0にした場合は無料で再抽選できる）
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CommonStoreLotteryCostMasterObjectBase {
		public virtual CommonStoreLotteryCostMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCommonStoreLotteryCostCategoryId => _rawData._mCommonStoreLotteryCostCategoryId;
		public virtual long stepNumber => _rawData._stepNumber;
		public virtual long availableCount => _rawData._availableCount;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CommonStoreLotteryCostMasterValueObject _rawData = null;
		public CommonStoreLotteryCostMasterObjectBase( CommonStoreLotteryCostMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CommonStoreLotteryCostMasterObject : CommonStoreLotteryCostMasterObjectBase, IMasterObject {
		public CommonStoreLotteryCostMasterObject( CommonStoreLotteryCostMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CommonStoreLotteryCostMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Common_Store_Lottery_Cost;

        [UnityEngine.SerializeField]
        CommonStoreLotteryCostMasterValueObject[] m_Common_Store_Lottery_Cost = null;
    }


}
