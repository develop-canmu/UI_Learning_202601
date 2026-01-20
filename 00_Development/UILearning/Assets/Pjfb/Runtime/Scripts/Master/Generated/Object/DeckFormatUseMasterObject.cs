//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckFormatUseMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(2)]
		public long _mDeckFormatId {get{ return mDeckFormatId;} set{ this.mDeckFormatId = value;}}
		[MessagePack.Key(3)]
		public string _defaultName {get{ return defaultName;} set{ this.defaultName = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long useType = 0; // $useType バトルの種別等 (4 => 通常のバトル, 4001 => フレンド貸出, 1000001 => 育成, 2000000 => バリデーション用育成, 2000001 => バリデーション用キャラ, 2005001 => バリデーション用フレンド, 2100001 => バリデーション用サポート)
		[UnityEngine.SerializeField] long mDeckFormatId = 0; // $mDeckFormatId デッキフォーマットID
		[UnityEngine.SerializeField] string defaultName = ""; // デッキデフォルト名。:number:を文字列の中に入れておくと、デッキ種別内での番号に置換される。例：「デッキ:number:」→「デッキ1」
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckFormatUseMasterObjectBase {
		public virtual DeckFormatUseMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long useType => _rawData._useType;
		public virtual long mDeckFormatId => _rawData._mDeckFormatId;
		public virtual string defaultName => _rawData._defaultName;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckFormatUseMasterValueObject _rawData = null;
		public DeckFormatUseMasterObjectBase( DeckFormatUseMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckFormatUseMasterObject : DeckFormatUseMasterObjectBase, IMasterObject {
		public DeckFormatUseMasterObject( DeckFormatUseMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckFormatUseMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Format_Use;

        [UnityEngine.SerializeField]
        DeckFormatUseMasterValueObject[] m_Deck_Format_Use = null;
    }


}
