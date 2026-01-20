//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckFormatConditionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mDeckFormatId {get{ return mDeckFormatId;} set{ this.mDeckFormatId = value;}}
		[MessagePack.Key(2)]
		public long _conditionType {get{ return conditionType;} set{ this.conditionType = value;}}
		[MessagePack.Key(3)]
		public long _conditionTarget {get{ return conditionTarget;} set{ this.conditionTarget = value;}}
		[MessagePack.Key(4)]
		public string _conditionTargetColumn {get{ return conditionTargetColumn;} set{ this.conditionTargetColumn = value;}}
		[MessagePack.Key(5)]
		public string _operatorType {get{ return operatorType;} set{ this.operatorType = value;}}
		[MessagePack.Key(6)]
		public string _compareValue {get{ return compareValue;} set{ this.compareValue = value;}}
		[MessagePack.Key(7)]
		public long _charaCount {get{ return charaCount;} set{ this.charaCount = value;}}
		[MessagePack.Key(8)]
		public string _errorMessage {get{ return errorMessage;} set{ this.errorMessage = value;}}
		[MessagePack.Key(9)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(10)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(11)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(12)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mDeckFormatId = 0; // $mDeckFormatId 紐づくデッキフォーマット
		[UnityEngine.SerializeField] long conditionType = 0; // $conditionType 条件種別。1：制限（指定charaCount以下である） 2：必須（指定charaCount以上である）, 3:特殊, 4:デッキ枠に対する制限
		[UnityEngine.SerializeField] long conditionTarget = 0; // $conditionTarget 条件対象。1 => 編成時に指定したroleNumber（デッキ全体でのキャラ数制限を行う場合）, 2 => Crz7\Components\Simple\CharaVariable\DeckValidationやCrz7\Components\Simple\CharaV2\DeckValidationに存在するプロパティ, 3 => 編成時に指定したroleNumber（各ユニット内でのキャラ数制限を行う場合）, 11 => conditionTypeが3の場合のみ）
		[UnityEngine.SerializeField] string conditionTargetColumn = ""; // $conditionTargetColumn 条件対象カラム。conditionTargetが1の場合に参照。該当のキャラオブジェクトのどのプロパティを参照するか
		[UnityEngine.SerializeField] string operatorType = ""; // $operatorType 比較演算子。EQ,NE,GE,GT,LE,LT,BETWEEN,IN等。存在しないものを指定した場合、false扱いになる。MAX_DUPLICATEを指定した場合compareはせずに、conditionTargetColumnで指定した値のうち同一の値が含まれるものを集計し最大の数をcharaCountと比較させる
		[UnityEngine.SerializeField] string compareValue = ""; // $compareValue 指定のキャラクタから取り出したプロパティと比較する値。conditionTarget = 11の場合、u_deck.optionValueに対する検証
		[UnityEngine.SerializeField] long charaCount = 0; // $charaCount 上限/要求キャラ数。conditionType = 3の場合、参照しない。
		[UnityEngine.SerializeField] string errorMessage = ""; // $errorMessage バリデーションエラー時に返却するエラーメッセージ
		[UnityEngine.SerializeField] string description = ""; // $description 説明
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 編成制限ポップアップ内の表示順を制御する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckFormatConditionMasterObjectBase {
		public virtual DeckFormatConditionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mDeckFormatId => _rawData._mDeckFormatId;
		public virtual long conditionType => _rawData._conditionType;
		public virtual long conditionTarget => _rawData._conditionTarget;
		public virtual string conditionTargetColumn => _rawData._conditionTargetColumn;
		public virtual string operatorType => _rawData._operatorType;
		public virtual string compareValue => _rawData._compareValue;
		public virtual long charaCount => _rawData._charaCount;
		public virtual string errorMessage => _rawData._errorMessage;
		public virtual string description => _rawData._description;
		public virtual string name => _rawData._name;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckFormatConditionMasterValueObject _rawData = null;
		public DeckFormatConditionMasterObjectBase( DeckFormatConditionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckFormatConditionMasterObject : DeckFormatConditionMasterObjectBase, IMasterObject {
		public DeckFormatConditionMasterObject( DeckFormatConditionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckFormatConditionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Format_Condition;

        [UnityEngine.SerializeField]
        DeckFormatConditionMasterValueObject[] m_Deck_Format_Condition = null;
    }


}
