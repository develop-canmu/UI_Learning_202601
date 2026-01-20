//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumScoreBattleBonusTurnMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _rankTop {get{ return rankTop;} set{ this.rankTop = value;}}
		[MessagePack.Key(2)]
		public long _rankBottom {get{ return rankBottom;} set{ this.rankBottom = value;}}
		[MessagePack.Key(3)]
		public long _groupSelectRate {get{ return groupSelectRate;} set{ this.groupSelectRate = value;}}
		[MessagePack.Key(4)]
		public long _bonusRateMin {get{ return bonusRateMin;} set{ this.bonusRateMin = value;}}
		[MessagePack.Key(5)]
		public long _bonusRateMax {get{ return bonusRateMax;} set{ this.bonusRateMax = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long rankTop = 0; // $rankTop 対象となる先頭の順位（値が小さい方）
		[UnityEngine.SerializeField] long rankBottom = 0; // $rankBottom 対象となる末尾の順位（値が大きい方）
		[UnityEngine.SerializeField] long groupSelectRate = 0; // $groupSelectRate そもそもこの範囲が選択対象になるかどうか？の確率の基礎値
		[UnityEngine.SerializeField] long bonusRateMin = 0; // $bonusRateMin ボーナス加算割合値、最小値
		[UnityEngine.SerializeField] long bonusRateMax = 0; // $bonusRateMax ボーナス加算割合値、最大値
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumScoreBattleBonusTurnMasterObjectBase {
		public virtual ColosseumScoreBattleBonusTurnMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long rankTop => _rawData._rankTop;
		public virtual long rankBottom => _rawData._rankBottom;
		public virtual long groupSelectRate => _rawData._groupSelectRate;
		public virtual long bonusRateMin => _rawData._bonusRateMin;
		public virtual long bonusRateMax => _rawData._bonusRateMax;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumScoreBattleBonusTurnMasterValueObject _rawData = null;
		public ColosseumScoreBattleBonusTurnMasterObjectBase( ColosseumScoreBattleBonusTurnMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumScoreBattleBonusTurnMasterObject : ColosseumScoreBattleBonusTurnMasterObjectBase, IMasterObject {
		public ColosseumScoreBattleBonusTurnMasterObject( ColosseumScoreBattleBonusTurnMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumScoreBattleBonusTurnMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Score_Battle_Bonus_Turn;

        [UnityEngine.SerializeField]
        ColosseumScoreBattleBonusTurnMasterValueObject[] m_Colosseum_Score_Battle_Bonus_Turn = null;
    }


}
