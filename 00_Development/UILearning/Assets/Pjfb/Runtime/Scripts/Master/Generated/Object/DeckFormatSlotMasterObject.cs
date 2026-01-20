//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckFormatSlotMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mDeckFormatId {get{ return mDeckFormatId;} set{ this.mDeckFormatId = value;}}
		[MessagePack.Key(2)]
		public long _index {get{ return index;} set{ this.index = value;}}
		[MessagePack.Key(3)]
		public long _roleNumber {get{ return roleNumber;} set{ this.roleNumber = value;}}
		[MessagePack.Key(4)]
		public long _unitNumber {get{ return unitNumber;} set{ this.unitNumber = value;}}
		[MessagePack.Key(5)]
		public long _conditionTableType {get{ return conditionTableType;} set{ this.conditionTableType = value;}}
		[MessagePack.Key(6)]
		public long _conditionCardType {get{ return conditionCardType;} set{ this.conditionCardType = value;}}
		[MessagePack.Key(7)]
		public bool _conditionRequired {get{ return conditionRequired;} set{ this.conditionRequired = value;}}
		[MessagePack.Key(8)]
		public long _traineeMinLevel {get{ return traineeMinLevel;} set{ this.traineeMinLevel = value;}}
		[MessagePack.Key(9)]
		public bool _isExtraSupport {get{ return isExtraSupport;} set{ this.isExtraSupport = value;}}
		[MessagePack.Key(10)]
		public long _mDeckFormatConditionId {get{ return mDeckFormatConditionId;} set{ this.mDeckFormatConditionId = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mDeckFormatId = 0; // $mDeckFormatId デッキの編成フォーマット
		[UnityEngine.SerializeField] long index = 0; // $index デッキの配置位置（0始まり）
		[UnityEngine.SerializeField] long roleNumber = 0; // $roleNumber 役割番号（役割…ピッチャー・キャッチャー等…を区別する値をint値で指定。何を設定するかは、クライアントと運用で決定できれば良い）
		[UnityEngine.SerializeField] long unitNumber = 0; // $unitNumber ユニット番号。この枠のキャラが属するユニットを上3桁がユニット役割・下3桁がユニット番号となる6桁の数字で表現する。0を指定すればこのデッキのこの枠ではユニット番号を使用しない
		[UnityEngine.SerializeField] long conditionTableType = 0; // $conditionTableType どのテーブルのデータを指定するか（0 ⇒ 指定しない、 1 ⇒ u_chara, 2 ⇒ u_chara_variable, 4 ⇒ u_chara_variable_trainer）
		[UnityEngine.SerializeField] long conditionCardType = 0; // $conditionCardType m_charaのcardTypeに対する条件（0 ⇒ 指定しない、 1 以上、特定のcardTypeと一致するものだけを受け入れる）
		[UnityEngine.SerializeField] bool conditionRequired = false; // $conditionRequired 該当の枠の編成を必須とするか（1 ⇒ 必須である、 2 ⇒ 必須ではない）
		[UnityEngine.SerializeField] long traineeMinLevel = 0; // $traineeMinLevel トレーニングで使用するm_deck_formatでのみ参照する。育成対象キャラのレベルがこのレベル以上なら、この枠がトレーニング時に有効となる。デッキ編成時に制限はしない
		[UnityEngine.SerializeField] bool isExtraSupport = false; // $isExtraSupport EXサポートカードか（1 ⇒ EXサポートカード、 2 ⇒ 通常）
		[UnityEngine.SerializeField] long mDeckFormatConditionId = 0; // $mDeckFormatConditionId 枠に対して編成条件を指定する場合、m_deck_format_conditionのid
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckFormatSlotMasterObjectBase {
		public virtual DeckFormatSlotMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mDeckFormatId => _rawData._mDeckFormatId;
		public virtual long index => _rawData._index;
		public virtual long roleNumber => _rawData._roleNumber;
		public virtual long unitNumber => _rawData._unitNumber;
		public virtual long conditionTableType => _rawData._conditionTableType;
		public virtual long conditionCardType => _rawData._conditionCardType;
		public virtual bool conditionRequired => _rawData._conditionRequired;
		public virtual long traineeMinLevel => _rawData._traineeMinLevel;
		public virtual bool isExtraSupport => _rawData._isExtraSupport;
		public virtual long mDeckFormatConditionId => _rawData._mDeckFormatConditionId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckFormatSlotMasterValueObject _rawData = null;
		public DeckFormatSlotMasterObjectBase( DeckFormatSlotMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckFormatSlotMasterObject : DeckFormatSlotMasterObjectBase, IMasterObject {
		public DeckFormatSlotMasterObject( DeckFormatSlotMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckFormatSlotMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Format_Slot;

        [UnityEngine.SerializeField]
        DeckFormatSlotMasterValueObject[] m_Deck_Format_Slot = null;
    }


}
