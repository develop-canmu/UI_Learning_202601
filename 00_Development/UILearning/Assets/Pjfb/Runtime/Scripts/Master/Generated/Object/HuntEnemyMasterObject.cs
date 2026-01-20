//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntEnemyMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntId {get{ return mHuntId;} set{ this.mHuntId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public string _subName {get{ return subName;} set{ this.subName = value;}}
		[MessagePack.Key(4)]
		public long _difficulty {get{ return difficulty;} set{ this.difficulty = value;}}
		[MessagePack.Key(5)]
		public long _rarity {get{ return rarity;} set{ this.rarity = value;}}
		[MessagePack.Key(6)]
		public string _combatPowerRecommended {get{ return combatPowerRecommended;} set{ this.combatPowerRecommended = value;}}
		[MessagePack.Key(7)]
		public string _specialDisplay {get{ return specialDisplay;} set{ this.specialDisplay = value;}}
		[MessagePack.Key(8)]
		public long _progress {get{ return progress;} set{ this.progress = value;}}
		[MessagePack.Key(9)]
		public long _keyMPointValue {get{ return keyMPointValue;} set{ this.keyMPointValue = value;}}
		[MessagePack.Key(10)]
		public long _mHuntDeckRegulationId {get{ return mHuntDeckRegulationId;} set{ this.mHuntDeckRegulationId = value;}}
		[MessagePack.Key(11)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(12)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(13)]
		public long[] _mCharaNpcIdList {get{ return mCharaNpcIdList;} set{ this.mCharaNpcIdList = value;}}
		[MessagePack.Key(14)]
		public long[] _roleNumberList {get{ return roleNumberList;} set{ this.roleNumberList = value;}}
		[MessagePack.Key(15)]
		public long[] _unitNumberList {get{ return unitNumberList;} set{ this.unitNumberList = value;}}
		[MessagePack.Key(16)]
		public long[] _mCharaNpcIdListInvariable {get{ return mCharaNpcIdListInvariable;} set{ this.mCharaNpcIdListInvariable = value;}}
		[MessagePack.Key(17)]
		public long[] _roleNumberListInvariable {get{ return roleNumberListInvariable;} set{ this.roleNumberListInvariable = value;}}
		[MessagePack.Key(18)]
		public long[] _choiceNumberList {get{ return choiceNumberList;} set{ this.choiceNumberList = value;}}
		[MessagePack.Key(19)]
		public long _mBattleWarFieldId {get{ return mBattleWarFieldId;} set{ this.mBattleWarFieldId = value;}}
		[MessagePack.Key(20)]
		public long _mDeckTacticsId {get{ return mDeckTacticsId;} set{ this.mDeckTacticsId = value;}}
		[MessagePack.Key(21)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mHuntId = 0; // 狩猟イベントID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] string subName = ""; // サブ名称
		[UnityEngine.SerializeField] long difficulty = 0; // 難易度（難易度はテーブルではなく、定数定義。初期は4段階を想定）
		[UnityEngine.SerializeField] long rarity = 0; // レア度番号
		[UnityEngine.SerializeField] string combatPowerRecommended = ""; // 推奨戦力
		[UnityEngine.SerializeField] string specialDisplay = ""; // 推奨戦力の代わりに表示する文字列
		[UnityEngine.SerializeField] long progress = 0; // 要求されるユーザーのイベント進捗。1始まりで連番で設定する。m_hunt.lotteryType = 4以外の場合は、すべて1を指定する
		[UnityEngine.SerializeField] long keyMPointValue = 0; // m_hunt.lotteryType = 4 の際、必要なポイント数を設定する。0ならば必要なし
		[UnityEngine.SerializeField] long mHuntDeckRegulationId = 0; // 編成制限がある場合、m_hunt_deck_regulationのid、無い場合は0
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始時刻
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了時刻
		[UnityEngine.SerializeField] long[] mCharaNpcIdList = null; // 敵キャラ配列ID配列になるjson
		[UnityEngine.SerializeField] long[] roleNumberList = null; // 敵キャラ配列ID配列に対応する役割番号一覧。ユニットがある場合、ユニット内でのキャラの役割番号を指定する
		[UnityEngine.SerializeField] long[] unitNumberList = null; // 敵キャラID配列に対応するユニット番号一覧。それぞれのキャラが属するユニットを上3桁がユニット役割・下3桁がユニット番号となる6桁の数字で表現する
		[UnityEngine.SerializeField] long[] mCharaNpcIdListInvariable = null; // $mCharaNpcIdListInvariable 不可変キャラとしてインゲームに参加する敵のIDリスト
		[UnityEngine.SerializeField] long[] roleNumberListInvariable = null; // $roleNumberListInvariable 不可変キャラとしてインゲームに参加する敵の役割番号リスト
		[UnityEngine.SerializeField] long[] choiceNumberList = null; // 選択報酬選ばせるための選択肢一覧（int配列になるjson）
		[UnityEngine.SerializeField] long mBattleWarFieldId = 0; // この敵と戦闘を行う地形マスタのID
		[UnityEngine.SerializeField] long mDeckTacticsId = 0; // 戦術ID。このデッキが従う戦術を指定する。戦術IDを使用しないタイトルでは0を指定する
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntEnemyMasterObjectBase {
		public virtual HuntEnemyMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntId => _rawData._mHuntId;
		public virtual string name => _rawData._name;
		public virtual string subName => _rawData._subName;
		public virtual long difficulty => _rawData._difficulty;
		public virtual long rarity => _rawData._rarity;
		public virtual string combatPowerRecommended => _rawData._combatPowerRecommended;
		public virtual string specialDisplay => _rawData._specialDisplay;
		public virtual long progress => _rawData._progress;
		public virtual long keyMPointValue => _rawData._keyMPointValue;
		public virtual long mHuntDeckRegulationId => _rawData._mHuntDeckRegulationId;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual long[] mCharaNpcIdList => _rawData._mCharaNpcIdList;
		public virtual long[] roleNumberList => _rawData._roleNumberList;
		public virtual long[] unitNumberList => _rawData._unitNumberList;
		public virtual long[] mCharaNpcIdListInvariable => _rawData._mCharaNpcIdListInvariable;
		public virtual long[] roleNumberListInvariable => _rawData._roleNumberListInvariable;
		public virtual long[] choiceNumberList => _rawData._choiceNumberList;
		public virtual long mBattleWarFieldId => _rawData._mBattleWarFieldId;
		public virtual long mDeckTacticsId => _rawData._mDeckTacticsId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntEnemyMasterValueObject _rawData = null;
		public HuntEnemyMasterObjectBase( HuntEnemyMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntEnemyMasterObject : HuntEnemyMasterObjectBase, IMasterObject {
		public HuntEnemyMasterObject( HuntEnemyMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntEnemyMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Enemy;

        [UnityEngine.SerializeField]
        HuntEnemyMasterValueObject[] m_Hunt_Enemy = null;
    }


}
