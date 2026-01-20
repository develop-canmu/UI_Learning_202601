//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumRankingPrizeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mColosseumRankingPrizeGroupId {get{ return mColosseumRankingPrizeGroupId;} set{ this.mColosseumRankingPrizeGroupId = value;}}
		[MessagePack.Key(2)]
		public long _causeType {get{ return causeType;} set{ this.causeType = value;}}
		[MessagePack.Key(3)]
		public long _gradeNumber {get{ return gradeNumber;} set{ this.gradeNumber = value;}}
		[MessagePack.Key(4)]
		public long _rankTop {get{ return rankTop;} set{ this.rankTop = value;}}
		[MessagePack.Key(5)]
		public long _rankBottom {get{ return rankBottom;} set{ this.rankBottom = value;}}
		[MessagePack.Key(6)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(7)]
		public long _point {get{ return point;} set{ this.point = value;}}
		[MessagePack.Key(8)]
		public string _message {get{ return message;} set{ this.message = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mColosseumRankingPrizeGroupId = 0; // $mColosseumRankingPrizeGroupId 報酬グループID（実際のグループテーブルは無し）
		[UnityEngine.SerializeField] long causeType = 0; // なんの報酬か（1 => 個人ランキング報酬、 2 => ギルド順位準拠の個人向け報酬、 3 => 個人スコアランキング報酬）、4 => ギルド順位基準で付与される、対ギルド報酬（g_masterに紐づく報酬を指定）、5 => ギルド順位基準で付与されるで付与される、ギルドポイント・ギルド経験値指定（prizeは無視されます）
		[UnityEngine.SerializeField] long gradeNumber = 0; // $gradeNumber 対象グレード
		[UnityEngine.SerializeField] long rankTop = 0; // $rankTop 対象になるランクの先頭
		[UnityEngine.SerializeField] long rankBottom = 0; // $rankBottom 対象となるランクの末尾
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // $prizeJson 報酬のPrizeJson。相当のデータを大量に処理する可能性が高いので、そのままプレゼントBOXに入れて問題ないデータ以外は入れない。ユーザーに直接付与して欲しいデータ、性別フィルタリングの上付与してほしいアバターなどは入れない。causeType = 4の場合は、ギルド用のprizeJsonフォーマットである必要がある
		[UnityEngine.SerializeField] long point = 0; // causeType = 5以外の場合は無視される。付与想定のギルド経験値指定
		[UnityEngine.SerializeField] string message = ""; // $message プレゼントBOXに入る際に付与されるメッセージ
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumRankingPrizeMasterObjectBase {
		public virtual ColosseumRankingPrizeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mColosseumRankingPrizeGroupId => _rawData._mColosseumRankingPrizeGroupId;
		public virtual long causeType => _rawData._causeType;
		public virtual long gradeNumber => _rawData._gradeNumber;
		public virtual long rankTop => _rawData._rankTop;
		public virtual long rankBottom => _rawData._rankBottom;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual long point => _rawData._point;
		public virtual string message => _rawData._message;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumRankingPrizeMasterValueObject _rawData = null;
		public ColosseumRankingPrizeMasterObjectBase( ColosseumRankingPrizeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumRankingPrizeMasterObject : ColosseumRankingPrizeMasterObjectBase, IMasterObject {
		public ColosseumRankingPrizeMasterObject( ColosseumRankingPrizeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumRankingPrizeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Ranking_Prize;

        [UnityEngine.SerializeField]
        ColosseumRankingPrizeMasterValueObject[] m_Colosseum_Ranking_Prize = null;
    }


}
