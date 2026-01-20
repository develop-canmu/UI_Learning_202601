//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckEnhanceMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public long _mPlayerEnhanceId {get{ return mPlayerEnhanceId;} set{ this.mPlayerEnhanceId = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理名
		[UnityEngine.SerializeField] long mPlayerEnhanceId = 0; // $mPlayerEnhanceId m_player_enhanceのid
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckEnhanceMasterObjectBase {
		public virtual DeckEnhanceMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual long mPlayerEnhanceId => _rawData._mPlayerEnhanceId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckEnhanceMasterValueObject _rawData = null;
		public DeckEnhanceMasterObjectBase( DeckEnhanceMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckEnhanceMasterObject : DeckEnhanceMasterObjectBase, IMasterObject {
		public DeckEnhanceMasterObject( DeckEnhanceMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckEnhanceMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Enhance;

        [UnityEngine.SerializeField]
        DeckEnhanceMasterValueObject[] m_Deck_Enhance = null;
    }


}
