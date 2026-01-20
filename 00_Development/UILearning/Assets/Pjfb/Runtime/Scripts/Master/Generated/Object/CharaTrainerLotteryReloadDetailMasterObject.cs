//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotteryReloadDetailMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainerLotteryDetailGroupId {get{ return mCharaTrainerLotteryDetailGroupId;} set{ this.mCharaTrainerLotteryDetailGroupId = value;}}
		[MessagePack.Key(2)]
		public long _number {get{ return number;} set{ this.number = value;}}
		[MessagePack.Key(3)]
		public long _lotteryType {get{ return lotteryType;} set{ this.lotteryType = value;}}
		[MessagePack.Key(4)]
		public string _text {get{ return text;} set{ this.text = value;}}
		[MessagePack.Key(5)]
		public long _effectType {get{ return effectType;} set{ this.effectType = value;}}
		[MessagePack.Key(6)]
		public string _lotteryList {get{ return lotteryList;} set{ this.lotteryList = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainerLotteryDetailGroupId = 0; // $mCharaTrainerLotteryDetailGroupId グループID
		[UnityEngine.SerializeField] long number = 0; // $number 枠番号（m_chara_training_lottery_reload.reloadType=4の場合に使用）
		[UnityEngine.SerializeField] long lotteryType = 0; // $lotteryType 抽選種別（m_chara_training_lottery_reload.reloadType=4の場合に使用）。1:練習能力を再抽選（flame単位）、2:効果量を再抽選（content単位）
		[UnityEngine.SerializeField] string text = ""; // $text ラベルテキスト
		[UnityEngine.SerializeField] long effectType = 0; // $effectType ラベル種別
		[UnityEngine.SerializeField] string lotteryList = ""; // $lotteryList 抽選リスト　[[mCharaTrainerLotteryFrameTableGroupId, 優先度]]
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotteryReloadDetailMasterObjectBase {
		public virtual CharaTrainerLotteryReloadDetailMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainerLotteryDetailGroupId => _rawData._mCharaTrainerLotteryDetailGroupId;
		public virtual long number => _rawData._number;
		public virtual long lotteryType => _rawData._lotteryType;
		public virtual string text => _rawData._text;
		public virtual long effectType => _rawData._effectType;
		public virtual string lotteryList => _rawData._lotteryList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotteryReloadDetailMasterValueObject _rawData = null;
		public CharaTrainerLotteryReloadDetailMasterObjectBase( CharaTrainerLotteryReloadDetailMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotteryReloadDetailMasterObject : CharaTrainerLotteryReloadDetailMasterObjectBase, IMasterObject {
		public CharaTrainerLotteryReloadDetailMasterObject( CharaTrainerLotteryReloadDetailMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotteryReloadDetailMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery_Reload_Detail;

        [UnityEngine.SerializeField]
        CharaTrainerLotteryReloadDetailMasterValueObject[] m_Chara_Trainer_Lottery_Reload_Detail = null;
    }


}
