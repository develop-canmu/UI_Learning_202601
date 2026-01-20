//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumGradeRankLabelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mColosseumGradeGroupId {get{ return mColosseumGradeGroupId;} set{ this.mColosseumGradeGroupId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _gradeNumber {get{ return gradeNumber;} set{ this.gradeNumber = value;}}
		[MessagePack.Key(4)]
		public long _rankTop {get{ return rankTop;} set{ this.rankTop = value;}}
		[MessagePack.Key(5)]
		public long _rankBottom {get{ return rankBottom;} set{ this.rankBottom = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mColosseumGradeGroupId = 0; // $mColosseumGradeGroupId グレードグループID（グレードグループのテーブル自体は現状は無し）
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long gradeNumber = 0; // $gradeNumber 1始まり。大きいほうがより上位を示す（グレード管理がない場合、ユーザーデータ上は1になる）
		[UnityEngine.SerializeField] long rankTop = 0; // $rankTop このデータが担当するランクの先頭
		[UnityEngine.SerializeField] long rankBottom = 0; // $rankBottom このデータが担当するランクの末尾
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumGradeRankLabelMasterObjectBase {
		public virtual ColosseumGradeRankLabelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mColosseumGradeGroupId => _rawData._mColosseumGradeGroupId;
		public virtual string name => _rawData._name;
		public virtual long gradeNumber => _rawData._gradeNumber;
		public virtual long rankTop => _rawData._rankTop;
		public virtual long rankBottom => _rawData._rankBottom;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumGradeRankLabelMasterValueObject _rawData = null;
		public ColosseumGradeRankLabelMasterObjectBase( ColosseumGradeRankLabelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumGradeRankLabelMasterObject : ColosseumGradeRankLabelMasterObjectBase, IMasterObject {
		public ColosseumGradeRankLabelMasterObject( ColosseumGradeRankLabelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumGradeRankLabelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Grade_Rank_Label;

        [UnityEngine.SerializeField]
        ColosseumGradeRankLabelMasterValueObject[] m_Colosseum_Grade_Rank_Label = null;
    }


}
