//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntDifficultyMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _difficulty {get{ return difficulty;} set{ this.difficulty = value;}}
		[MessagePack.Key(3)]
		public string _combatPowerRecommended {get{ return combatPowerRecommended;} set{ this.combatPowerRecommended = value;}}
		[MessagePack.Key(4)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long difficulty = 0; // $difficulty 難易度。1が初級。それは必須とし、以降は運営側が任意に作成する感じで良い。
		[UnityEngine.SerializeField] string combatPowerRecommended = ""; // $combatPowerRecommended 推奨戦力
		[UnityEngine.SerializeField] long imageId = 0; // $imageId 画像のID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntDifficultyMasterObjectBase {
		public virtual HuntDifficultyMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long difficulty => _rawData._difficulty;
		public virtual string combatPowerRecommended => _rawData._combatPowerRecommended;
		public virtual long imageId => _rawData._imageId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntDifficultyMasterValueObject _rawData = null;
		public HuntDifficultyMasterObjectBase( HuntDifficultyMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntDifficultyMasterObject : HuntDifficultyMasterObjectBase, IMasterObject {
		public HuntDifficultyMasterObject( HuntDifficultyMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntDifficultyMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Difficulty;

        [UnityEngine.SerializeField]
        HuntDifficultyMasterValueObject[] m_Hunt_Difficulty = null;
    }


}
