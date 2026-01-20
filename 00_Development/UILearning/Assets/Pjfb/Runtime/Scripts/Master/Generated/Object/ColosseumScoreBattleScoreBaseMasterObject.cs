//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumScoreBattleScoreBaseMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _rankTop {get{ return rankTop;} set{ this.rankTop = value;}}
		[MessagePack.Key(2)]
		public long _rankBottom {get{ return rankBottom;} set{ this.rankBottom = value;}}
		[MessagePack.Key(3)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(4)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long rankTop = 0; // $rankTop 対象となる先頭の順位（値が小さい方）
		[UnityEngine.SerializeField] long rankBottom = 0; // $rankBottom 対象となる末尾の順位（値が大きい方）
		[UnityEngine.SerializeField] long type = 0; // $type 1 => 攻撃時に勝利、 2 => 攻撃時に敗北、 3 => 攻撃時に引き分け、4 => 滞在ターンボーナスベース値
		[UnityEngine.SerializeField] long value = 0; // $value ポイント
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumScoreBattleScoreBaseMasterObjectBase {
		public virtual ColosseumScoreBattleScoreBaseMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long rankTop => _rawData._rankTop;
		public virtual long rankBottom => _rawData._rankBottom;
		public virtual long type => _rawData._type;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumScoreBattleScoreBaseMasterValueObject _rawData = null;
		public ColosseumScoreBattleScoreBaseMasterObjectBase( ColosseumScoreBattleScoreBaseMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumScoreBattleScoreBaseMasterObject : ColosseumScoreBattleScoreBaseMasterObjectBase, IMasterObject {
		public ColosseumScoreBattleScoreBaseMasterObject( ColosseumScoreBattleScoreBaseMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumScoreBattleScoreBaseMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Score_Battle_Score_Base;

        [UnityEngine.SerializeField]
        ColosseumScoreBattleScoreBaseMasterValueObject[] m_Colosseum_Score_Battle_Score_Base = null;
    }


}
