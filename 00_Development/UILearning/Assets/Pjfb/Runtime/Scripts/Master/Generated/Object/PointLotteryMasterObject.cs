//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointLotteryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(2)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mPointId = 0; // 対象のポイントID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointLotteryMasterObjectBase {
		public virtual PointLotteryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mPointId => _rawData._mPointId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointLotteryMasterValueObject _rawData = null;
		public PointLotteryMasterObjectBase( PointLotteryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointLotteryMasterObject : PointLotteryMasterObjectBase, IMasterObject {
		public PointLotteryMasterObject( PointLotteryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointLotteryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Lottery;

        [UnityEngine.SerializeField]
        PointLotteryMasterValueObject[] m_Point_Lottery = null;
    }


}
