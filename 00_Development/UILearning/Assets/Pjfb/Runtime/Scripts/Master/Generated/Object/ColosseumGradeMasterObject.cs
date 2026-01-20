//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumGradeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mColosseumGradeGroupId {get{ return mColosseumGradeGroupId;} set{ this.mColosseumGradeGroupId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _gradeNumber {get{ return gradeNumber;} set{ this.gradeNumber = value;}}
		[MessagePack.Key(4)]
		public long _roomCapacity {get{ return roomCapacity;} set{ this.roomCapacity = value;}}
		[MessagePack.Key(5)]
		public long _roomNpcCountMinimum {get{ return roomNpcCountMinimum;} set{ this.roomNpcCountMinimum = value;}}
		[MessagePack.Key(6)]
		public long _rankingType {get{ return rankingType;} set{ this.rankingType = value;}}
		[MessagePack.Key(7)]
		public bool _isVisibleRealUser {get{ return isVisibleRealUser;} set{ this.isVisibleRealUser = value;}}
		[MessagePack.Key(8)]
		public long _promotionRankBottom {get{ return promotionRankBottom;} set{ this.promotionRankBottom = value;}}
		[MessagePack.Key(9)]
		public long _demotionRankTop {get{ return demotionRankTop;} set{ this.demotionRankTop = value;}}
		[MessagePack.Key(10)]
		public long _gradeCapacity {get{ return gradeCapacity;} set{ this.gradeCapacity = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mColosseumGradeGroupId = 0; // $mColosseumGradeGroupId グレードグループID（グレードグループのテーブル自体は現状は無し）
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long gradeNumber = 0; // $gradeNumber 1始まり。大きいほうがより上位を示す。連番で定義する（グレード管理がない場合、ユーザーデータ上は1になる）
		[UnityEngine.SerializeField] long roomCapacity = 0; // 1部屋に入れられる人数（無制限な場合は、-1を指定。5000以上にする場合は、要相談）。ここが有限な場合、該当のグレードに対して初アクセスだった場合、1人だけで他のメンバーはNPCである部屋に打ち込まれる。グレードが存在しない試合の場合、同一ルームに無制限に入っていく仕様に自動的になる。（※0や-1は現在未実装） 。グループ戦の場合は、要求グループ（ギルド等）の数を指定
		[UnityEngine.SerializeField] long roomNpcCountMinimum = 0; // 1部屋内に必ず所属させるNPCの下限人数。roomCapacity以上の値を指定した場合などの動作は保証しない。roomCapacityより1小さい値等の、極端すぎる設定はサーバー負荷の原因になりますので、控えてください。グループ戦の場合は、要求グループ（ギルド等）の数を指定
		[UnityEngine.SerializeField] long rankingType = 0; // 1 => 通常、2 => 自己完結型（u_colosseum_room_status.roomNumber = -1の場合は、rankingTypeは参照せず、自身&NPCだけの部屋となる）
		[UnityEngine.SerializeField] bool isVisibleRealUser = false; // 対戦相手のユーザー実態を表示する 1 => 表示、 2 => 非表
		[UnityEngine.SerializeField] long promotionRankBottom = 0; // $promotionRankBottom 昇格が発生するランクの末尾（1～100を昇格させる場合は、100を指定）。昇格がない場合は、-1。
		[UnityEngine.SerializeField] long demotionRankTop = 0; // $demotionRankTop 降格が発生するランクの先頭（500位～を降格させる場合は、500を指定）。降格がない場合は、-1。
		[UnityEngine.SerializeField] long gradeCapacity = 0; // グレードに残存できる人・いるべきグループ数。（現状リーグ戦以外では機能しない）。入れ替え戦方式の場合、入れ替え戦マッチングの総数をこの値に基づいて変更し、最終的にあるべき件数になるようにする。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumGradeMasterObjectBase {
		public virtual ColosseumGradeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mColosseumGradeGroupId => _rawData._mColosseumGradeGroupId;
		public virtual string name => _rawData._name;
		public virtual long gradeNumber => _rawData._gradeNumber;
		public virtual long roomCapacity => _rawData._roomCapacity;
		public virtual long roomNpcCountMinimum => _rawData._roomNpcCountMinimum;
		public virtual long rankingType => _rawData._rankingType;
		public virtual bool isVisibleRealUser => _rawData._isVisibleRealUser;
		public virtual long promotionRankBottom => _rawData._promotionRankBottom;
		public virtual long demotionRankTop => _rawData._demotionRankTop;
		public virtual long gradeCapacity => _rawData._gradeCapacity;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumGradeMasterValueObject _rawData = null;
		public ColosseumGradeMasterObjectBase( ColosseumGradeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumGradeMasterObject : ColosseumGradeMasterObjectBase, IMasterObject {
		public ColosseumGradeMasterObject( ColosseumGradeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumGradeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Grade;

        [UnityEngine.SerializeField]
        ColosseumGradeMasterValueObject[] m_Colosseum_Grade = null;
    }


}
