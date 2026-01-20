//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBattleAllyNpcMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaNpcId {get{ return mCharaNpcId;} set{ this.mCharaNpcId = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId 練習試合で味方キャラとなるサポートキャラの mCharaId
		[UnityEngine.SerializeField] long mCharaNpcId = 0; // $mCharaNpcId 実際に練習試合で使用される mCharaNpcId
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBattleAllyNpcMasterObjectBase {
		public virtual TrainingBattleAllyNpcMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long mCharaNpcId => _rawData._mCharaNpcId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBattleAllyNpcMasterValueObject _rawData = null;
		public TrainingBattleAllyNpcMasterObjectBase( TrainingBattleAllyNpcMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBattleAllyNpcMasterObject : TrainingBattleAllyNpcMasterObjectBase, IMasterObject {
		public TrainingBattleAllyNpcMasterObject( TrainingBattleAllyNpcMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBattleAllyNpcMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Battle_Ally_Npc;

        [UnityEngine.SerializeField]
        TrainingBattleAllyNpcMasterValueObject[] m_Training_Battle_Ally_Npc = null;
    }


}
