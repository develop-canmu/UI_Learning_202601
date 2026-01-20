//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBattleMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long[] _mCharaNpcIdListEnemy {get{ return mCharaNpcIdListEnemy;} set{ this.mCharaNpcIdListEnemy = value;}}
		[MessagePack.Key(3)]
		public long[] _roleNumberList {get{ return roleNumberList;} set{ this.roleNumberList = value;}}
		[MessagePack.Key(4)]
		public long[] _unitNumberList {get{ return unitNumberList;} set{ this.unitNumberList = value;}}
		[MessagePack.Key(5)]
		public string _inspireLevelRateJson {get{ return inspireLevelRateJson;} set{ this.inspireLevelRateJson = value;}}
		[MessagePack.Key(6)]
		public long _mBattleWarFieldId {get{ return mBattleWarFieldId;} set{ this.mBattleWarFieldId = value;}}
		[MessagePack.Key(7)]
		public long _mDeckTacticsId {get{ return mDeckTacticsId;} set{ this.mDeckTacticsId = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] long[] mCharaNpcIdListEnemy = null; // 敵となる m_chara_npc のIDリスト
		[UnityEngine.SerializeField] long[] roleNumberList = null; // 敵キャラ配列ID配列に対応する役割番号一覧。ユニットがある場合、ユニット内でのキャラの役割番号を指定する
		[UnityEngine.SerializeField] long[] unitNumberList = null; // 敵キャラID配列に対応するユニット番号一覧。それぞれのキャラが属するユニットを上3桁がユニット役割・下3桁がユニット番号となる6桁の数字で表現する
		[UnityEngine.SerializeField] string inspireLevelRateJson = ""; // インスピレーションブーストレベル毎の乗算倍率
		[UnityEngine.SerializeField] long mBattleWarFieldId = 0; // この敵と戦闘を行う地形マスタのID
		[UnityEngine.SerializeField] long mDeckTacticsId = 0; // 戦術ID。このデッキが従う戦術を指定する。戦術IDを使用しないタイトルでは0を指定する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBattleMasterObjectBase {
		public virtual TrainingBattleMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long[] mCharaNpcIdListEnemy => _rawData._mCharaNpcIdListEnemy;
		public virtual long[] roleNumberList => _rawData._roleNumberList;
		public virtual long[] unitNumberList => _rawData._unitNumberList;
		public virtual string inspireLevelRateJson => _rawData._inspireLevelRateJson;
		public virtual long mBattleWarFieldId => _rawData._mBattleWarFieldId;
		public virtual long mDeckTacticsId => _rawData._mDeckTacticsId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBattleMasterValueObject _rawData = null;
		public TrainingBattleMasterObjectBase( TrainingBattleMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBattleMasterObject : TrainingBattleMasterObjectBase, IMasterObject {
		public TrainingBattleMasterObject( TrainingBattleMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBattleMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Battle;

        [UnityEngine.SerializeField]
        TrainingBattleMasterValueObject[] m_Training_Battle = null;
    }


}
