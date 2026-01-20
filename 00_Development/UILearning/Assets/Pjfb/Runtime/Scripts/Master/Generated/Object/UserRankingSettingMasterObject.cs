//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class UserRankingSettingMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _triggerType {get{ return triggerType;} set{ this.triggerType = value;}}
		[MessagePack.Key(2)]
		public long _targetType {get{ return targetType;} set{ this.targetType = value;}}
		[MessagePack.Key(3)]
		public long[] _targetIdList {get{ return targetIdList;} set{ this.targetIdList = value;}}
		[MessagePack.Key(4)]
		public string _enabledTypeFlg {get{ return enabledTypeFlg;} set{ this.enabledTypeFlg = value;}}
		[MessagePack.Key(5)]
		public long _displayRankCount {get{ return displayRankCount;} set{ this.displayRankCount = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long triggerType = 0; // $triggerType 1=>総戦力値、2=>育成時の戦力
		[UnityEngine.SerializeField] long targetType = 0; // $targetType triggerType=2の場合、1=>mCharaId、2=>parentMCharaId、3=>mRarityId
		[UnityEngine.SerializeField] long[] targetIdList = null; // $targetIdList targetTypeに応じた対象idの配列
		[UnityEngine.SerializeField] string enabledTypeFlg = ""; // $enabledTypeFlg 実際に集計をしたいランキングの種類をビットフラグでまとめる。例：11。この場合、全パターンの集計を行う。1 => 集計する、0 => 集計しない。大きい桁から順に「ギルド、ユーザー」の設定となっている
		[UnityEngine.SerializeField] long displayRankCount = 0; // $displayRankCount 表示ランキング数
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class UserRankingSettingMasterObjectBase {
		public virtual UserRankingSettingMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long triggerType => _rawData._triggerType;
		public virtual long targetType => _rawData._targetType;
		public virtual long[] targetIdList => _rawData._targetIdList;
		public virtual string enabledTypeFlg => _rawData._enabledTypeFlg;
		public virtual long displayRankCount => _rawData._displayRankCount;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        UserRankingSettingMasterValueObject _rawData = null;
		public UserRankingSettingMasterObjectBase( UserRankingSettingMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class UserRankingSettingMasterObject : UserRankingSettingMasterObjectBase, IMasterObject {
		public UserRankingSettingMasterObject( UserRankingSettingMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class UserRankingSettingMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_User_Ranking_Setting;

        [UnityEngine.SerializeField]
        UserRankingSettingMasterValueObject[] m_User_Ranking_Setting = null;
    }


}
