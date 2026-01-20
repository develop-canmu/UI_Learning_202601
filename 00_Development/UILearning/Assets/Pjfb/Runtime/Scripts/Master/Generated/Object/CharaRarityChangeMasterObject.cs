//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaRarityChangeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaRarityChangeCategoryId {get{ return mCharaRarityChangeCategoryId;} set{ this.mCharaRarityChangeCategoryId = value;}}
		[MessagePack.Key(2)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(3)]
		public long _liberationLevel {get{ return liberationLevel;} set{ this.liberationLevel = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mCharaRarityChangeCategoryId = 0; // キャラレアリティ変更カテゴリのID
		[UnityEngine.SerializeField] long mRarityId = 0; // 変更後のレアリティのID
		[UnityEngine.SerializeField] long liberationLevel = 0; // レアリティの変更条件となる潜在解放レベル
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaRarityChangeMasterObjectBase {
		public virtual CharaRarityChangeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaRarityChangeCategoryId => _rawData._mCharaRarityChangeCategoryId;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual long liberationLevel => _rawData._liberationLevel;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaRarityChangeMasterValueObject _rawData = null;
		public CharaRarityChangeMasterObjectBase( CharaRarityChangeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaRarityChangeMasterObject : CharaRarityChangeMasterObjectBase, IMasterObject {
		public CharaRarityChangeMasterObject( CharaRarityChangeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaRarityChangeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Rarity_Change;

        [UnityEngine.SerializeField]
        CharaRarityChangeMasterValueObject[] m_Chara_Rarity_Change = null;
    }


}
