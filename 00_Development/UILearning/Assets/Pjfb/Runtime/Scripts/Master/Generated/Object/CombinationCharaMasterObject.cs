//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CombinationCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCombinationId {get{ return mCombinationId;} set{ this.mCombinationId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(3)]
		public long _materialExp {get{ return materialExp;} set{ this.materialExp = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCombinationId = 0; // $mCombinationId コンビネーションID
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId キャラID
		[UnityEngine.SerializeField] long materialExp = 0; // $materialExp コンビネーション素材にした時に強化対象が得られる経験値 / 0の場合、素材として運用不可能
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CombinationCharaMasterObjectBase {
		public virtual CombinationCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCombinationId => _rawData._mCombinationId;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long materialExp => _rawData._materialExp;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CombinationCharaMasterValueObject _rawData = null;
		public CombinationCharaMasterObjectBase( CombinationCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CombinationCharaMasterObject : CombinationCharaMasterObjectBase, IMasterObject {
		public CombinationCharaMasterObject( CombinationCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CombinationCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Combination_Chara;

        [UnityEngine.SerializeField]
        CombinationCharaMasterValueObject[] m_Combination_Chara = null;
    }


}
