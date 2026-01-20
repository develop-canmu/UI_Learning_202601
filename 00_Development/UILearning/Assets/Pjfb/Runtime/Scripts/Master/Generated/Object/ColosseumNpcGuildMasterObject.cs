//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumNpcGuildMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mColosseumNpcGroupId {get{ return mColosseumNpcGroupId;} set{ this.mColosseumNpcGroupId = value;}}
		[MessagePack.Key(2)]
		public long _gradeNumber {get{ return gradeNumber;} set{ this.gradeNumber = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public long _mGuildEmblemId {get{ return mGuildEmblemId;} set{ this.mGuildEmblemId = value;}}
		[MessagePack.Key(5)]
		public string _optionJson {get{ return optionJson;} set{ this.optionJson = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mColosseumNpcGroupId = 0; // $mColosseumNpcGroupId 闘技場NPCグループ紐づけ（テーブルは存在せず、参照の場合にのみ使う）
		[UnityEngine.SerializeField] long gradeNumber = 0; // $gradeNumber グレード番号
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long mGuildEmblemId = 0; // $mGuildEmblemId ギルドエンブレムID
		[UnityEngine.SerializeField] string optionJson = ""; // $optionJson NPCギルドの行動ロジックなどを定めるJSON。タイトルや闘技場イベント内容によって内容や形式は異なる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumNpcGuildMasterObjectBase {
		public virtual ColosseumNpcGuildMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mColosseumNpcGroupId => _rawData._mColosseumNpcGroupId;
		public virtual long gradeNumber => _rawData._gradeNumber;
		public virtual string name => _rawData._name;
		public virtual long mGuildEmblemId => _rawData._mGuildEmblemId;
		public virtual string optionJson => _rawData._optionJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumNpcGuildMasterValueObject _rawData = null;
		public ColosseumNpcGuildMasterObjectBase( ColosseumNpcGuildMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumNpcGuildMasterObject : ColosseumNpcGuildMasterObjectBase, IMasterObject {
		public ColosseumNpcGuildMasterObject( ColosseumNpcGuildMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumNpcGuildMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Npc_Guild;

        [UnityEngine.SerializeField]
        ColosseumNpcGuildMasterValueObject[] m_Colosseum_Npc_Guild = null;
    }


}
