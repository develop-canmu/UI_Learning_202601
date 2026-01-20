//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotterySlotMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainerLotterySlotGroupId {get{ return mCharaTrainerLotterySlotGroupId;} set{ this.mCharaTrainerLotterySlotGroupId = value;}}
		[MessagePack.Key(2)]
		public long _slotLength {get{ return slotLength;} set{ this.slotLength = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainerLotterySlotGroupId = 0; // $mCharaTrainerLotterySlotGroupId スロット数の抽選グループID
		[UnityEngine.SerializeField] long slotLength = 0; // $slotLength スロット数
		[UnityEngine.SerializeField] long rate = 0; // $rate 抽選率（重みづけ。万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotterySlotMasterObjectBase {
		public virtual CharaTrainerLotterySlotMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainerLotterySlotGroupId => _rawData._mCharaTrainerLotterySlotGroupId;
		public virtual long slotLength => _rawData._slotLength;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotterySlotMasterValueObject _rawData = null;
		public CharaTrainerLotterySlotMasterObjectBase( CharaTrainerLotterySlotMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotterySlotMasterObject : CharaTrainerLotterySlotMasterObjectBase, IMasterObject {
		public CharaTrainerLotterySlotMasterObject( CharaTrainerLotterySlotMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotterySlotMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery_Slot;

        [UnityEngine.SerializeField]
        CharaTrainerLotterySlotMasterValueObject[] m_Chara_Trainer_Lottery_Slot = null;
    }


}
