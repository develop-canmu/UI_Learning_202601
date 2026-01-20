//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntEnemyPrizeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntId {get{ return mHuntId;} set{ this.mHuntId = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _difficulty {get{ return difficulty;} set{ this.difficulty = value;}}
		[MessagePack.Key(4)]
		public long _rarity {get{ return rarity;} set{ this.rarity = value;}}
		[MessagePack.Key(5)]
		public long _choiceNumber {get{ return choiceNumber;} set{ this.choiceNumber = value;}}
		[MessagePack.Key(6)]
		public long _mHuntEnemyId {get{ return mHuntEnemyId;} set{ this.mHuntEnemyId = value;}}
		[MessagePack.Key(7)]
		public long _battleResult {get{ return battleResult;} set{ this.battleResult = value;}}
		[MessagePack.Key(8)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(9)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(10)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mHuntId = 0; // $mHuntId 狩猟イベントID
		[UnityEngine.SerializeField] long type = 0; // $type 区分（1 ⇒ 指名、2 ⇒ 初回、 11～ ⇒ 汎用）
		[UnityEngine.SerializeField] long difficulty = 0; // $difficulty 難易度。0の場合無条件
		[UnityEngine.SerializeField] long rarity = 0; // $rarity レアリティ。0の場合無条件
		[UnityEngine.SerializeField] long choiceNumber = 0; // 選択報酬番号。0の場合無条件
		[UnityEngine.SerializeField] long mHuntEnemyId = 0; // $mHuntEnemyId 敵ID。0の場合無条件
		[UnityEngine.SerializeField] long battleResult = 0; // $battleResult バトル結果。無条件 ⇒ 0, 勝利 ⇒ 1, 敗北 ⇒ 2, 引き分け ⇒ 3
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 表示優先度。クライアント側でのみ使用する
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // $prizeJson 報酬
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntEnemyPrizeMasterObjectBase {
		public virtual HuntEnemyPrizeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntId => _rawData._mHuntId;
		public virtual long type => _rawData._type;
		public virtual long difficulty => _rawData._difficulty;
		public virtual long rarity => _rawData._rarity;
		public virtual long choiceNumber => _rawData._choiceNumber;
		public virtual long mHuntEnemyId => _rawData._mHuntEnemyId;
		public virtual long battleResult => _rawData._battleResult;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntEnemyPrizeMasterValueObject _rawData = null;
		public HuntEnemyPrizeMasterObjectBase( HuntEnemyPrizeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntEnemyPrizeMasterObject : HuntEnemyPrizeMasterObjectBase, IMasterObject {
		public HuntEnemyPrizeMasterObject( HuntEnemyPrizeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntEnemyPrizeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Enemy_Prize;

        [UnityEngine.SerializeField]
        HuntEnemyPrizeMasterValueObject[] m_Hunt_Enemy_Prize = null;
    }


}
