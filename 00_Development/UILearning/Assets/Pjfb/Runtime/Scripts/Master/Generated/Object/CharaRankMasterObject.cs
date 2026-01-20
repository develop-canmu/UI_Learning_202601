//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaRankMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public string _minValue {get{ return minValue;} set{ this.minValue = value;}}
		[MessagePack.Key(4)]
		public long _rankNumber {get{ return rankNumber;} set{ this.rankNumber = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long type = 0; // $type 種別(1 => ステータス、2 => キャラ総合値、 3 => パーティ総合値、 4 => ギルド合計総合値, 11 => トレーニング補助キャラ能力レア度、13 => プレイヤーランクで参照するパーティ総合値（設定値separatesUserMaxDeckRankAndDeckRankが有効なときのみ）)
		[UnityEngine.SerializeField] string minValue = ""; // $minValue 最低限必要な値
		[UnityEngine.SerializeField] long rankNumber = 0; // $rankNumber 最低限必要な値。同一タイプ内で、必ず0になるレコードを入れる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaRankMasterObjectBase {
		public virtual CharaRankMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long type => _rawData._type;
		public virtual string minValue => _rawData._minValue;
		public virtual long rankNumber => _rawData._rankNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaRankMasterValueObject _rawData = null;
		public CharaRankMasterObjectBase( CharaRankMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaRankMasterObject : CharaRankMasterObjectBase, IMasterObject {
		public CharaRankMasterObject( CharaRankMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaRankMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Rank;

        [UnityEngine.SerializeField]
        CharaRankMasterValueObject[] m_Chara_Rank = null;
    }


}
