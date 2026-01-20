//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotteryReloadMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaTrainerLotteryReloadGroupId {get{ return mCharaTrainerLotteryReloadGroupId;} set{ this.mCharaTrainerLotteryReloadGroupId = value;}}
		[MessagePack.Key(2)]
		public long _subNumber {get{ return subNumber;} set{ this.subNumber = value;}}
		[MessagePack.Key(3)]
		public long _reloadType {get{ return reloadType;} set{ this.reloadType = value;}}
		[MessagePack.Key(4)]
		public long _reloadValue {get{ return reloadValue;} set{ this.reloadValue = value;}}
		[MessagePack.Key(5)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(6)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(7)]
		public long _mCharaTrainerLotteryDetailGroupId {get{ return mCharaTrainerLotteryDetailGroupId;} set{ this.mCharaTrainerLotteryDetailGroupId = value;}}
		[MessagePack.Key(8)]
		public string _enhanceRateList {get{ return enhanceRateList;} set{ this.enhanceRateList = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaTrainerLotteryReloadGroupId = 0; // $mCharaTrainerLotteryReloadGroupId グループ指定
		[UnityEngine.SerializeField] long subNumber = 0; // $subNumber groupIdとsubNumberが同じなレコードの設定ポイントがすべて対象となる（要は方式1はジェムだけ、方式2は〇〇チケットと〇〇ポイントを使って～的なグループ化をするイメージ）
		[UnityEngine.SerializeField] long reloadType = 0; // $reloadType 再抽選方式（1 => 完全再抽選、2以降は未定）
		[UnityEngine.SerializeField] long reloadValue = 0; // $reloadValue reloadTypeによって設定する値が異なる。<br>1: 使用しない、2: 枠数、3: 1=>元ついていた値を除外する（同じ値にならない）, 2=>元ついていた値を含めて抽選する、4: 使用しない
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId 料金通貨ID
		[UnityEngine.SerializeField] long value = 0; // $value 価格
		[UnityEngine.SerializeField] long mCharaTrainerLotteryDetailGroupId = 0; // $mCharaTrainerLotteryDetailGroupId m_chara_trainer_lottery_reload_detailのグループID。reloadTypeが2と4の場合に設定
		[UnityEngine.SerializeField] string enhanceRateList = ""; // $enhanceRateList m_chara_trainer_lottery_frame_tableのpriorityGroupIdに応じて乗算する抽選倍率リスト。[[priorityGroupId,rate]]
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotteryReloadMasterObjectBase {
		public virtual CharaTrainerLotteryReloadMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaTrainerLotteryReloadGroupId => _rawData._mCharaTrainerLotteryReloadGroupId;
		public virtual long subNumber => _rawData._subNumber;
		public virtual long reloadType => _rawData._reloadType;
		public virtual long reloadValue => _rawData._reloadValue;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual long mCharaTrainerLotteryDetailGroupId => _rawData._mCharaTrainerLotteryDetailGroupId;
		public virtual string enhanceRateList => _rawData._enhanceRateList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotteryReloadMasterValueObject _rawData = null;
		public CharaTrainerLotteryReloadMasterObjectBase( CharaTrainerLotteryReloadMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotteryReloadMasterObject : CharaTrainerLotteryReloadMasterObjectBase, IMasterObject {
		public CharaTrainerLotteryReloadMasterObject( CharaTrainerLotteryReloadMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotteryReloadMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery_Reload;

        [UnityEngine.SerializeField]
        CharaTrainerLotteryReloadMasterValueObject[] m_Chara_Trainer_Lottery_Reload = null;
    }


}
