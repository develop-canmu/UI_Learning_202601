//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotteryContentMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainerLotteryContentGroupId {get{ return mCharaTrainerLotteryContentGroupId;} set{ this.mCharaTrainerLotteryContentGroupId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaTrainerLotteryStatusId {get{ return mCharaTrainerLotteryStatusId;} set{ this.mCharaTrainerLotteryStatusId = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainerLotteryContentGroupId = 0; // $mCharaTrainerLotteryContentGroupId 実体抽選時に使うグループ
		[UnityEngine.SerializeField] long mCharaTrainerLotteryStatusId = 0; // $mCharaTrainerLotteryStatusId 性能実体
		[UnityEngine.SerializeField] long rate = 0; // $rate 抽選重み付け
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotteryContentMasterObjectBase {
		public virtual CharaTrainerLotteryContentMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainerLotteryContentGroupId => _rawData._mCharaTrainerLotteryContentGroupId;
		public virtual long mCharaTrainerLotteryStatusId => _rawData._mCharaTrainerLotteryStatusId;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotteryContentMasterValueObject _rawData = null;
		public CharaTrainerLotteryContentMasterObjectBase( CharaTrainerLotteryContentMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotteryContentMasterObject : CharaTrainerLotteryContentMasterObjectBase, IMasterObject {
		public CharaTrainerLotteryContentMasterObject( CharaTrainerLotteryContentMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotteryContentMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery_Content;

        [UnityEngine.SerializeField]
        CharaTrainerLotteryContentMasterValueObject[] m_Chara_Trainer_Lottery_Content = null;
    }


}
