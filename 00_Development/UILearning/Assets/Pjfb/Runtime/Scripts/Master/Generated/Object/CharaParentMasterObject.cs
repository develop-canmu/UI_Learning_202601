//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaParentMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _parentMCharaId {get{ return parentMCharaId;} set{ this.parentMCharaId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long[] _mCharaIdList {get{ return mCharaIdList;} set{ this.mCharaIdList = value;}}
		[MessagePack.Key(4)]
		public long _mEnhanceIdTrust {get{ return mEnhanceIdTrust;} set{ this.mEnhanceIdTrust = value;}}
		[MessagePack.Key(5)]
		public long _mLevelRewardIdTrust {get{ return mLevelRewardIdTrust;} set{ this.mLevelRewardIdTrust = value;}}
		[MessagePack.Key(6)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(7)]
		public long[] _superiorCharaParentIdList {get{ return superiorCharaParentIdList;} set{ this.superiorCharaParentIdList = value;}}
		[MessagePack.Key(8)]
		public bool _isPlayable {get{ return isPlayable;} set{ this.isPlayable = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long parentMCharaId = 0; // $parentMCharaId 親キャラID
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long[] mCharaIdList = null; // $mCharaIdList 親キャラID
		[UnityEngine.SerializeField] long mEnhanceIdTrust = 0; // $mEnhanceIdTrust 信頼度成長テーブル
		[UnityEngine.SerializeField] long mLevelRewardIdTrust = 0; // $mLevelRewardIdTrust 信頼度報酬テーブル
		[UnityEngine.SerializeField] long sortNumber = 0; // $sortNumber 表示順番号（小さい方を優先）
		[UnityEngine.SerializeField] long[] superiorCharaParentIdList = null; // $superiorCharaParentIdList 格上の親キャラIDリスト。この親キャラにとって格上となる親キャラのIDを設定し、セリフが敬語になるかどうかの判断などに用いる
		[UnityEngine.SerializeField] bool isPlayable = false; // $isPlayable プレイアブルキャラか
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaParentMasterObjectBase {
		public virtual CharaParentMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long parentMCharaId => _rawData._parentMCharaId;
		public virtual string name => _rawData._name;
		public virtual long[] mCharaIdList => _rawData._mCharaIdList;
		public virtual long mEnhanceIdTrust => _rawData._mEnhanceIdTrust;
		public virtual long mLevelRewardIdTrust => _rawData._mLevelRewardIdTrust;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual long[] superiorCharaParentIdList => _rawData._superiorCharaParentIdList;
		public virtual bool isPlayable => _rawData._isPlayable;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaParentMasterValueObject _rawData = null;
		public CharaParentMasterObjectBase( CharaParentMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaParentMasterObject : CharaParentMasterObjectBase, IMasterObject {
		public CharaParentMasterObject( CharaParentMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaParentMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Parent;

        [UnityEngine.SerializeField]
        CharaParentMasterValueObject[] m_Chara_Parent = null;
    }


}
