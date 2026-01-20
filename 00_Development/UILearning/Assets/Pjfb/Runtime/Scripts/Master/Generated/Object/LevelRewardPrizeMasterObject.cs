//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class LevelRewardPrizeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mLevelRewardId {get{ return mLevelRewardId;} set{ this.mLevelRewardId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mLevelRewardId = 0; // $mLevelRewardId レベル報酬対応表の大本ID
		[UnityEngine.SerializeField] long level = 0; // $level レベル
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // $prizeJson 報酬情報
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class LevelRewardPrizeMasterObjectBase {
		public virtual LevelRewardPrizeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mLevelRewardId => _rawData._mLevelRewardId;
		public virtual long level => _rawData._level;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        LevelRewardPrizeMasterValueObject _rawData = null;
		public LevelRewardPrizeMasterObjectBase( LevelRewardPrizeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class LevelRewardPrizeMasterObject : LevelRewardPrizeMasterObjectBase, IMasterObject {
		public LevelRewardPrizeMasterObject( LevelRewardPrizeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class LevelRewardPrizeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Level_Reward_Prize;

        [UnityEngine.SerializeField]
        LevelRewardPrizeMasterValueObject[] m_Level_Reward_Prize = null;
    }


}
