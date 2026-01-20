//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaDeckTacticsAbilityMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _charaLevelMin {get{ return charaLevelMin;} set{ this.charaLevelMin = value;}}
		[MessagePack.Key(3)]
		public long _mDeckTacticsId {get{ return mDeckTacticsId;} set{ this.mDeckTacticsId = value;}}
		[MessagePack.Key(4)]
		public long _tacticsLevel {get{ return tacticsLevel;} set{ this.tacticsLevel = value;}}
		[MessagePack.Key(5)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(6)]
		public string _abilityList {get{ return abilityList;} set{ this.abilityList = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId キャラID
		[UnityEngine.SerializeField] long charaLevelMin = 0; // $charaLevelMin 必要キャラレベル。作戦の獲得やスキルの発動に必要な最低レベルを定める
		[UnityEngine.SerializeField] long mDeckTacticsId = 0; // $mDeckTacticsId 解放される作戦ID
		[UnityEngine.SerializeField] long tacticsLevel = 0; // $tacticsLevel 作戦レベル。クライアントでの表示にのみ使用する
		[UnityEngine.SerializeField] string description = ""; // $description 作戦レベルに応じた説明文
		[UnityEngine.SerializeField] string abilityList = ""; // $abilityList 発動スキル。mDeckTacticsId で指定した作戦を使用した際、作戦の持ち主であるキャラが charaLevelMin 以上であれば発動する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaDeckTacticsAbilityMasterObjectBase {
		public virtual CharaDeckTacticsAbilityMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long charaLevelMin => _rawData._charaLevelMin;
		public virtual long mDeckTacticsId => _rawData._mDeckTacticsId;
		public virtual long tacticsLevel => _rawData._tacticsLevel;
		public virtual string description => _rawData._description;
		public virtual string abilityList => _rawData._abilityList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaDeckTacticsAbilityMasterValueObject _rawData = null;
		public CharaDeckTacticsAbilityMasterObjectBase( CharaDeckTacticsAbilityMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaDeckTacticsAbilityMasterObject : CharaDeckTacticsAbilityMasterObjectBase, IMasterObject {
		public CharaDeckTacticsAbilityMasterObject( CharaDeckTacticsAbilityMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaDeckTacticsAbilityMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Deck_Tactics_Ability;

        [UnityEngine.SerializeField]
        CharaDeckTacticsAbilityMasterValueObject[] m_Chara_Deck_Tactics_Ability = null;
    }


}
