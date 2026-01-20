//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCoachRankMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _stepNumber {get{ return stepNumber;} set{ this.stepNumber = value;}}
		[MessagePack.Key(2)]
		public long _rank {get{ return rank;} set{ this.rank = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public long _isHigh {get{ return isHigh;} set{ this.isHigh = value;}}
		[MessagePack.Key(5)]
		public long _effectNumber {get{ return effectNumber;} set{ this.effectNumber = value;}}
		[MessagePack.Key(6)]
		public long _conditionEffectMin {get{ return conditionEffectMin;} set{ this.conditionEffectMin = value;}}
		[MessagePack.Key(7)]
		public long _conditionEffectMax {get{ return conditionEffectMax;} set{ this.conditionEffectMax = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long stepNumber = 0; // $stepNumber 特訓の段階。練習に参加したキャラに由来し、まずstepNumber:1の特訓が発生することがある。m_chara.cardTypeが1のものと2のものが同時に練習カードに参加していればstepNumber:2の特訓が発生する
		[UnityEngine.SerializeField] long rank = 0; // $rank 特訓のランク
		[UnityEngine.SerializeField] long rate = 0; // $rate このランクが抽選される確率重み
		[UnityEngine.SerializeField] long isHigh = 0; // $isHigh 高ランクとして扱うか。高ランク発生率が上昇する練習能力の確率補正対象となるかを定める
		[UnityEngine.SerializeField] long effectNumber = 0; // $effectNumber 特訓発生時のエフェクト識別番号
		[UnityEngine.SerializeField] long conditionEffectMin = 0; // $conditionEffectMin 練習の効果倍率段階の上昇幅最小値
		[UnityEngine.SerializeField] long conditionEffectMax = 0; // $conditionEffectMax 練習の効果倍率段階の上昇幅最大値
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCoachRankMasterObjectBase {
		public virtual TrainingCoachRankMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long stepNumber => _rawData._stepNumber;
		public virtual long rank => _rawData._rank;
		public virtual long rate => _rawData._rate;
		public virtual long isHigh => _rawData._isHigh;
		public virtual long effectNumber => _rawData._effectNumber;
		public virtual long conditionEffectMin => _rawData._conditionEffectMin;
		public virtual long conditionEffectMax => _rawData._conditionEffectMax;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCoachRankMasterValueObject _rawData = null;
		public TrainingCoachRankMasterObjectBase( TrainingCoachRankMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCoachRankMasterObject : TrainingCoachRankMasterObjectBase, IMasterObject {
		public TrainingCoachRankMasterObject( TrainingCoachRankMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCoachRankMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Coach_Rank;

        [UnityEngine.SerializeField]
        TrainingCoachRankMasterValueObject[] m_Training_Coach_Rank = null;
    }


}
