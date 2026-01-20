//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumNpcMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mColosseumNpcGroupId {get{ return mColosseumNpcGroupId;} set{ this.mColosseumNpcGroupId = value;}}
		[MessagePack.Key(2)]
		public long _gradeNumber {get{ return gradeNumber;} set{ this.gradeNumber = value;}}
		[MessagePack.Key(3)]
		public long _mColosseumNpcGuildId {get{ return mColosseumNpcGuildId;} set{ this.mColosseumNpcGuildId = value;}}
		[MessagePack.Key(4)]
		public long _memberIndex {get{ return memberIndex;} set{ this.memberIndex = value;}}
		[MessagePack.Key(5)]
		public long _deckIndex {get{ return deckIndex;} set{ this.deckIndex = value;}}
		[MessagePack.Key(6)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(7)]
		public long _mIconId {get{ return mIconId;} set{ this.mIconId = value;}}
		[MessagePack.Key(8)]
		public long[] _mCharaNpcIdList {get{ return mCharaNpcIdList;} set{ this.mCharaNpcIdList = value;}}
		[MessagePack.Key(9)]
		public long[] _roleNumberList {get{ return roleNumberList;} set{ this.roleNumberList = value;}}
		[MessagePack.Key(10)]
		public string _maxCombatPower {get{ return maxCombatPower;} set{ this.maxCombatPower = value;}}
		[MessagePack.Key(11)]
		public long _dummyRank {get{ return dummyRank;} set{ this.dummyRank = value;}}
		[MessagePack.Key(12)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mColosseumNpcGroupId = 0; // $mColosseumNpcGroupId 闘技場NPCグループ紐づけ（テーブルは存在せず、参照の場合にのみ使う）
		[UnityEngine.SerializeField] long gradeNumber = 0; // $gradeNumber グレード番号
		[UnityEngine.SerializeField] long mColosseumNpcGuildId = 0; // $mColosseumNpcGuildId NPCギルドID。 NPCギルド経由で参照されるデータでない場合は0を指定する
		[UnityEngine.SerializeField] long memberIndex = 0; // $memberIndex ギルド員番号。同一ギルド内の何人目が持っているデッキなのかを指定する。NPCギルド経由で参照されるデータでない場合は0を指定する
		[UnityEngine.SerializeField] long deckIndex = 0; // $deckIndex デッキ番号。同一memberが所持する何番目のデッキであるかを、1以上（m_deck_extraで定めるデッキ個数）以下の値で指定する。NPCギルド経由で参照されるデータでない場合は0を指定する
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long mIconId = 0; // $mIconId アイコンID
		[UnityEngine.SerializeField] long[] mCharaNpcIdList = null; // $mCharaNpcIdList 内包する敵情報JSON
		[UnityEngine.SerializeField] long[] roleNumberList = null; // $roleNumberList NPCに割り振られた役割を設定（ゲーム性によっては不要）
		[UnityEngine.SerializeField] string maxCombatPower = ""; // $maxCombatPower マッチング時に参照する総合力
		[UnityEngine.SerializeField] long dummyRank = 0; // $dummyRank 閉じ込め部屋の場合に表示する、嘘の順位
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumNpcMasterObjectBase {
		public virtual ColosseumNpcMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mColosseumNpcGroupId => _rawData._mColosseumNpcGroupId;
		public virtual long gradeNumber => _rawData._gradeNumber;
		public virtual long mColosseumNpcGuildId => _rawData._mColosseumNpcGuildId;
		public virtual long memberIndex => _rawData._memberIndex;
		public virtual long deckIndex => _rawData._deckIndex;
		public virtual string name => _rawData._name;
		public virtual long mIconId => _rawData._mIconId;
		public virtual long[] mCharaNpcIdList => _rawData._mCharaNpcIdList;
		public virtual long[] roleNumberList => _rawData._roleNumberList;
		public virtual string maxCombatPower => _rawData._maxCombatPower;
		public virtual long dummyRank => _rawData._dummyRank;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumNpcMasterValueObject _rawData = null;
		public ColosseumNpcMasterObjectBase( ColosseumNpcMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumNpcMasterObject : ColosseumNpcMasterObjectBase, IMasterObject {
		public ColosseumNpcMasterObject( ColosseumNpcMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumNpcMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Npc;

        [UnityEngine.SerializeField]
        ColosseumNpcMasterValueObject[] m_Colosseum_Npc = null;
    }


}
