//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumBattlePrizeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mColosseumBattlePrizeGroupId {get{ return mColosseumBattlePrizeGroupId;} set{ this.mColosseumBattlePrizeGroupId = value;}}
		[MessagePack.Key(2)]
		public long _gradeNumber {get{ return gradeNumber;} set{ this.gradeNumber = value;}}
		[MessagePack.Key(3)]
		public long _battleResult {get{ return battleResult;} set{ this.battleResult = value;}}
		[MessagePack.Key(4)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mColosseumBattlePrizeGroupId = 0; // $mColosseumBattlePrizeGroupId 報酬グループID
		[UnityEngine.SerializeField] long gradeNumber = 0; // $gradeNumber 対象グレード
		[UnityEngine.SerializeField] long battleResult = 0; // $battleResult 試合の結果（1=> 勝利, 2 => 敗北, 3 => 引き分け）
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // $prizeJson 報酬のPrizeJson
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumBattlePrizeMasterObjectBase {
		public virtual ColosseumBattlePrizeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mColosseumBattlePrizeGroupId => _rawData._mColosseumBattlePrizeGroupId;
		public virtual long gradeNumber => _rawData._gradeNumber;
		public virtual long battleResult => _rawData._battleResult;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumBattlePrizeMasterValueObject _rawData = null;
		public ColosseumBattlePrizeMasterObjectBase( ColosseumBattlePrizeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumBattlePrizeMasterObject : ColosseumBattlePrizeMasterObjectBase, IMasterObject {
		public ColosseumBattlePrizeMasterObject( ColosseumBattlePrizeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumBattlePrizeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Battle_Prize;

        [UnityEngine.SerializeField]
        ColosseumBattlePrizeMasterValueObject[] m_Colosseum_Battle_Prize = null;
    }


}
