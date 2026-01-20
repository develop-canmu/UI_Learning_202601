//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotteryFrameTableMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainerLotteryFrameTableGroupId {get{ return mCharaTrainerLotteryFrameTableGroupId;} set{ this.mCharaTrainerLotteryFrameTableGroupId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaTrainerLotteryContentGroupId {get{ return mCharaTrainerLotteryContentGroupId;} set{ this.mCharaTrainerLotteryContentGroupId = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public long _priorityGroupId {get{ return priorityGroupId;} set{ this.priorityGroupId = value;}}
		[MessagePack.Key(5)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainerLotteryFrameTableGroupId = 0; // $mCharaTrainerLotteryFrameTableGroupId 枠抽選時に使うグループ
		[UnityEngine.SerializeField] long mCharaTrainerLotteryContentGroupId = 0; // $mCharaTrainerLotteryContentGroupId 枠抽選の結果得られる、コンテンツ抽選に使うグループ
		[UnityEngine.SerializeField] long rate = 0; // $rate 抽選重み付け
		[UnityEngine.SerializeField] long priorityGroupId = 0; // $priorityGroupId 優先グループID。m_chara_trainer_lottery_reloadのenhanceRateListで使用
		[UnityEngine.SerializeField] long mRarityId = 0; // $mRarityId レアリティ
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotteryFrameTableMasterObjectBase {
		public virtual CharaTrainerLotteryFrameTableMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainerLotteryFrameTableGroupId => _rawData._mCharaTrainerLotteryFrameTableGroupId;
		public virtual long mCharaTrainerLotteryContentGroupId => _rawData._mCharaTrainerLotteryContentGroupId;
		public virtual long rate => _rawData._rate;
		public virtual long priorityGroupId => _rawData._priorityGroupId;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotteryFrameTableMasterValueObject _rawData = null;
		public CharaTrainerLotteryFrameTableMasterObjectBase( CharaTrainerLotteryFrameTableMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotteryFrameTableMasterObject : CharaTrainerLotteryFrameTableMasterObjectBase, IMasterObject {
		public CharaTrainerLotteryFrameTableMasterObject( CharaTrainerLotteryFrameTableMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotteryFrameTableMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery_Frame_Table;

        [UnityEngine.SerializeField]
        CharaTrainerLotteryFrameTableMasterValueObject[] m_Chara_Trainer_Lottery_Frame_Table = null;
    }


}
