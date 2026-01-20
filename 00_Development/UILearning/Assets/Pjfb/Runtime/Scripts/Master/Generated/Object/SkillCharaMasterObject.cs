//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class SkillCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _mSkillId {get{ return mSkillId;} set{ this.mSkillId = value;}}
		[MessagePack.Key(3)]
		public long _skillLevel {get{ return skillLevel;} set{ this.skillLevel = value;}}
		[MessagePack.Key(4)]
		public long _skillSlotNumber {get{ return skillSlotNumber;} set{ this.skillSlotNumber = value;}}
		[MessagePack.Key(5)]
		public long _openLevel {get{ return openLevel;} set{ this.openLevel = value;}}
		[MessagePack.Key(6)]
		public long _openLiberationLevel {get{ return openLiberationLevel;} set{ this.openLiberationLevel = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId m_chara.id
		[UnityEngine.SerializeField] long mSkillId = 0; // $mSkillId m_ability.id
		[UnityEngine.SerializeField] long skillLevel = 0; // $skillLevel スキルを獲得する際のスキルレベル。schemaVersion 2 以降でのみ使用する
		[UnityEngine.SerializeField] long skillSlotNumber = 0; // $skillSlotNumber スキル枠番号
		[UnityEngine.SerializeField] long openLevel = 0; // $openLevel スキルが使えるようになる為の必要レベル
		[UnityEngine.SerializeField] long openLiberationLevel = 0; // $openLiberationLevel スキルが使えるようになる為の必要潜在解放レベル
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class SkillCharaMasterObjectBase {
		public virtual SkillCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long mSkillId => _rawData._mSkillId;
		public virtual long skillLevel => _rawData._skillLevel;
		public virtual long skillSlotNumber => _rawData._skillSlotNumber;
		public virtual long openLevel => _rawData._openLevel;
		public virtual long openLiberationLevel => _rawData._openLiberationLevel;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        SkillCharaMasterValueObject _rawData = null;
		public SkillCharaMasterObjectBase( SkillCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class SkillCharaMasterObject : SkillCharaMasterObjectBase, IMasterObject {
		public SkillCharaMasterObject( SkillCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class SkillCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Skill_Chara;

        [UnityEngine.SerializeField]
        SkillCharaMasterValueObject[] m_Skill_Chara = null;
    }


}
