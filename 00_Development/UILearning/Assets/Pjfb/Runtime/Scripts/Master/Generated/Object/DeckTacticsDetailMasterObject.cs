//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckTacticsDetailMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mDeckTacticsId {get{ return mDeckTacticsId;} set{ this.mDeckTacticsId = value;}}
		[MessagePack.Key(2)]
		public long _unitNumber {get{ return unitNumber;} set{ this.unitNumber = value;}}
		[MessagePack.Key(3)]
		public long _mDeckUnitRoleId {get{ return mDeckUnitRoleId;} set{ this.mDeckUnitRoleId = value;}}
		[MessagePack.Key(4)]
		public long _positionX {get{ return positionX;} set{ this.positionX = value;}}
		[MessagePack.Key(5)]
		public long _positionY {get{ return positionY;} set{ this.positionY = value;}}
		[MessagePack.Key(6)]
		public long _militaryStrengthRate {get{ return militaryStrengthRate;} set{ this.militaryStrengthRate = value;}}
		[MessagePack.Key(7)]
		public string _displayName {get{ return displayName;} set{ this.displayName = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mDeckTacticsId = 0; // $mDeckTacticsId 作戦設定ID
		[UnityEngine.SerializeField] long unitNumber = 0; // $unitNumber 1~5のユニット番号。pjshinでは1に指定されたユニットが撃破されると敗北となる。
		[UnityEngine.SerializeField] long mDeckUnitRoleId = 0; // $mDeckUnitRoleId ロールID
		[UnityEngine.SerializeField] long positionX = 0; // $positionX 初期配置X座標。pjshinでは本陣からどれだけ離すかを指定する
		[UnityEngine.SerializeField] long positionY = 0; // $positionY 初期配置Y座標。pjshinでは何番目のレーンかを指定する
		[UnityEngine.SerializeField] long militaryStrengthRate = 0; // $militaryStrengthRate 割り当てられる兵力の割合。万分率
		[UnityEngine.SerializeField] string displayName = ""; // $displayName 表示名
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckTacticsDetailMasterObjectBase {
		public virtual DeckTacticsDetailMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mDeckTacticsId => _rawData._mDeckTacticsId;
		public virtual long unitNumber => _rawData._unitNumber;
		public virtual long mDeckUnitRoleId => _rawData._mDeckUnitRoleId;
		public virtual long positionX => _rawData._positionX;
		public virtual long positionY => _rawData._positionY;
		public virtual long militaryStrengthRate => _rawData._militaryStrengthRate;
		public virtual string displayName => _rawData._displayName;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckTacticsDetailMasterValueObject _rawData = null;
		public DeckTacticsDetailMasterObjectBase( DeckTacticsDetailMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckTacticsDetailMasterObject : DeckTacticsDetailMasterObjectBase, IMasterObject {
		public DeckTacticsDetailMasterObject( DeckTacticsDetailMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckTacticsDetailMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Tactics_Detail;

        [UnityEngine.SerializeField]
        DeckTacticsDetailMasterValueObject[] m_Deck_Tactics_Detail = null;
    }


}
