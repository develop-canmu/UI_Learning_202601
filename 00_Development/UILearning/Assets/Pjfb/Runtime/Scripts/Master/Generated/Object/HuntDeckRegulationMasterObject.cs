//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntDeckRegulationMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _condtionCompleteBonusType {get{ return condtionCompleteBonusType;} set{ this.condtionCompleteBonusType = value;}}
		[MessagePack.Key(2)]
		public long _condtionCompleteBonusValue {get{ return condtionCompleteBonusValue;} set{ this.condtionCompleteBonusValue = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long condtionCompleteBonusType = 0; // 狩猟イベントデッキ編成条件ボーナス種別値
		[UnityEngine.SerializeField] long condtionCompleteBonusValue = 0; // 狩猟イベントデッキ編成条件ボーナス値
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntDeckRegulationMasterObjectBase {
		public virtual HuntDeckRegulationMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long condtionCompleteBonusType => _rawData._condtionCompleteBonusType;
		public virtual long condtionCompleteBonusValue => _rawData._condtionCompleteBonusValue;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntDeckRegulationMasterValueObject _rawData = null;
		public HuntDeckRegulationMasterObjectBase( HuntDeckRegulationMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntDeckRegulationMasterObject : HuntDeckRegulationMasterObjectBase, IMasterObject {
		public HuntDeckRegulationMasterObject( HuntDeckRegulationMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntDeckRegulationMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Deck_Regulation;

        [UnityEngine.SerializeField]
        HuntDeckRegulationMasterValueObject[] m_Hunt_Deck_Regulation = null;
    }


}
