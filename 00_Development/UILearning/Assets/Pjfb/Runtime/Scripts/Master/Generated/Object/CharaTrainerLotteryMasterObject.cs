//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotteryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainerLotterySlotGroupId {get{ return mCharaTrainerLotterySlotGroupId;} set{ this.mCharaTrainerLotterySlotGroupId = value;}}
		[MessagePack.Key(2)]
		public long[] _mCharaTrainerLotteryFrameTableGroupIdList {get{ return mCharaTrainerLotteryFrameTableGroupIdList;} set{ this.mCharaTrainerLotteryFrameTableGroupIdList = value;}}
		[MessagePack.Key(3)]
		public long _lotteryRarityCoefficient {get{ return lotteryRarityCoefficient;} set{ this.lotteryRarityCoefficient = value;}}
		[MessagePack.Key(4)]
		public long _mCharaTrainerLotteryReloadGroupId {get{ return mCharaTrainerLotteryReloadGroupId;} set{ this.mCharaTrainerLotteryReloadGroupId = value;}}
		[MessagePack.Key(5)]
		public long _iconType {get{ return iconType;} set{ this.iconType = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainerLotterySlotGroupId = 0; // $mCharaTrainerLotterySlotGroupId スロット数の抽選グループID
		[UnityEngine.SerializeField] long[] mCharaTrainerLotteryFrameTableGroupIdList = null; // $mCharaTrainerLotteryFrameTableGroupIdList 抽選枠の一覧配列。[50,50,12,4]と並べた場合、1,2枠目は50番目の枠グループから引いて、…といった指定となる
		[UnityEngine.SerializeField] long lotteryRarityCoefficient = 0; // $lotteryRarityCoefficient 抽選レアリティ係数（万分率）。獲得難度の異なる可変補助キャラのレアリティ値を同一のランクテーブルで扱えるようにするための調整用数値の意味合いを持つ
		[UnityEngine.SerializeField] long mCharaTrainerLotteryReloadGroupId = 0; // $mCharaTrainerLotteryReloadGroupId 再抽選設定。0であれば、再抽選は不可能
		[UnityEngine.SerializeField] long iconType = 0; // $iconType アイコン種別
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotteryMasterObjectBase {
		public virtual CharaTrainerLotteryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainerLotterySlotGroupId => _rawData._mCharaTrainerLotterySlotGroupId;
		public virtual long[] mCharaTrainerLotteryFrameTableGroupIdList => _rawData._mCharaTrainerLotteryFrameTableGroupIdList;
		public virtual long lotteryRarityCoefficient => _rawData._lotteryRarityCoefficient;
		public virtual long mCharaTrainerLotteryReloadGroupId => _rawData._mCharaTrainerLotteryReloadGroupId;
		public virtual long iconType => _rawData._iconType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotteryMasterValueObject _rawData = null;
		public CharaTrainerLotteryMasterObjectBase( CharaTrainerLotteryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotteryMasterObject : CharaTrainerLotteryMasterObjectBase, IMasterObject {
		public CharaTrainerLotteryMasterObject( CharaTrainerLotteryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotteryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery;

        [UnityEngine.SerializeField]
        CharaTrainerLotteryMasterValueObject[] m_Chara_Trainer_Lottery = null;
    }


}
