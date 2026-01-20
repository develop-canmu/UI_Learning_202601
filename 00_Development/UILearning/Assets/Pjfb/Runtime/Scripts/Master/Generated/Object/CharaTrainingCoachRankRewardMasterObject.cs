//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainingCoachRankRewardMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _minRank {get{ return minRank;} set{ this.minRank = value;}}
		[MessagePack.Key(3)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(4)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(5)]
		public long _mTrainingEventRewardId {get{ return mTrainingEventRewardId;} set{ this.mTrainingEventRewardId = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId mCharaId
		[UnityEngine.SerializeField] long minRank = 0; // $minRank この特訓報酬が発生する必要なランクの最小値
		[UnityEngine.SerializeField] long priority = 0; // $priority 抽選優先度。優先度の高いもののグループから順に特訓報酬を抽選する
		[UnityEngine.SerializeField] long rate = 0; // $rate 同一優先度内で、この報酬が選ばれる抽選重み
		[UnityEngine.SerializeField] long mTrainingEventRewardId = 0; // $mTrainingEventRewardId 報酬ID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainingCoachRankRewardMasterObjectBase {
		public virtual CharaTrainingCoachRankRewardMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long minRank => _rawData._minRank;
		public virtual long priority => _rawData._priority;
		public virtual long rate => _rawData._rate;
		public virtual long mTrainingEventRewardId => _rawData._mTrainingEventRewardId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainingCoachRankRewardMasterValueObject _rawData = null;
		public CharaTrainingCoachRankRewardMasterObjectBase( CharaTrainingCoachRankRewardMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainingCoachRankRewardMasterObject : CharaTrainingCoachRankRewardMasterObjectBase, IMasterObject {
		public CharaTrainingCoachRankRewardMasterObject( CharaTrainingCoachRankRewardMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainingCoachRankRewardMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Training_Coach_Rank_Reward;

        [UnityEngine.SerializeField]
        CharaTrainingCoachRankRewardMasterValueObject[] m_Chara_Training_Coach_Rank_Reward = null;
    }


}
