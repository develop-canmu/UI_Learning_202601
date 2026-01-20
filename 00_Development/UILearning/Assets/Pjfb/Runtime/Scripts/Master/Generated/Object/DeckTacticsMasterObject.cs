//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckTacticsMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public bool _isPlayable {get{ return isPlayable;} set{ this.isPlayable = value;}}
		[MessagePack.Key(3)]
		public bool _isPrimary {get{ return isPrimary;} set{ this.isPrimary = value;}}
		[MessagePack.Key(4)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(5)]
		public string _descriptionDetail {get{ return descriptionDetail;} set{ this.descriptionDetail = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 作戦名
		[UnityEngine.SerializeField] bool isPlayable = false; // $isPlayable プレイヤーが選択できる作戦かどうか
		[UnityEngine.SerializeField] bool isPrimary = false; // $isPrimary isPlayable = 1 のとき、解放条件なく最初から選択可能か。isPlayable = 2 のとき、常に 2
		[UnityEngine.SerializeField] string description = ""; // $description 説明文（作戦概要）
		[UnityEngine.SerializeField] string descriptionDetail = ""; // $descriptionDetail 詳細説明文（作戦効果）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckTacticsMasterObjectBase {
		public virtual DeckTacticsMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual bool isPlayable => _rawData._isPlayable;
		public virtual bool isPrimary => _rawData._isPrimary;
		public virtual string description => _rawData._description;
		public virtual string descriptionDetail => _rawData._descriptionDetail;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckTacticsMasterValueObject _rawData = null;
		public DeckTacticsMasterObjectBase( DeckTacticsMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckTacticsMasterObject : DeckTacticsMasterObjectBase, IMasterObject {
		public DeckTacticsMasterObject( DeckTacticsMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckTacticsMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Tactics;

        [UnityEngine.SerializeField]
        DeckTacticsMasterValueObject[] m_Deck_Tactics = null;
    }


}
