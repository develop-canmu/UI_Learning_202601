//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaRankPointMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _rankNumber {get{ return rankNumber;} set{ this.rankNumber = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long rankNumber = 0; // $rankNumber ランク番号
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId ポイントID
		[UnityEngine.SerializeField] long value = 0; // $value 売却額
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaRankPointMasterObjectBase {
		public virtual CharaRankPointMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long rankNumber => _rawData._rankNumber;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaRankPointMasterValueObject _rawData = null;
		public CharaRankPointMasterObjectBase( CharaRankPointMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaRankPointMasterObject : CharaRankPointMasterObjectBase, IMasterObject {
		public CharaRankPointMasterObject( CharaRankPointMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaRankPointMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Rank_Point;

        [UnityEngine.SerializeField]
        CharaRankPointMasterValueObject[] m_Chara_Rank_Point = null;
    }


}
