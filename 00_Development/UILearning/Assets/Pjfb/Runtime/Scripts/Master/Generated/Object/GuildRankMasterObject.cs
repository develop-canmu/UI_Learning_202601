//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class GuildRankMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _previousMGuildRankId {get{ return previousMGuildRankId;} set{ this.previousMGuildRankId = value;}}
		[MessagePack.Key(3)]
		public long _nextMGuildRankId {get{ return nextMGuildRankId;} set{ this.nextMGuildRankId = value;}}
		[MessagePack.Key(4)]
		public long _colosseumGradeNumber {get{ return colosseumGradeNumber;} set{ this.colosseumGradeNumber = value;}}
		[MessagePack.Key(5)]
		public long _combatPowerRankNumber {get{ return combatPowerRankNumber;} set{ this.combatPowerRankNumber = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザ側で使用する名称
		[UnityEngine.SerializeField] long previousMGuildRankId = 0; // このランクの前のランク
		[UnityEngine.SerializeField] long nextMGuildRankId = 0; // このランクの次のランク
		[UnityEngine.SerializeField] long colosseumGradeNumber = 0; // colosseum側で使用するグレード番号と対応させる数値
		[UnityEngine.SerializeField] long combatPowerRankNumber = 0; // 総合力ランクと対応させるための数値
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class GuildRankMasterObjectBase {
		public virtual GuildRankMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long previousMGuildRankId => _rawData._previousMGuildRankId;
		public virtual long nextMGuildRankId => _rawData._nextMGuildRankId;
		public virtual long colosseumGradeNumber => _rawData._colosseumGradeNumber;
		public virtual long combatPowerRankNumber => _rawData._combatPowerRankNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        GuildRankMasterValueObject _rawData = null;
		public GuildRankMasterObjectBase( GuildRankMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class GuildRankMasterObject : GuildRankMasterObjectBase, IMasterObject {
		public GuildRankMasterObject( GuildRankMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class GuildRankMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Guild_Rank;

        [UnityEngine.SerializeField]
        GuildRankMasterValueObject[] m_Guild_Rank = null;
    }


}
