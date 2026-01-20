//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntEnemyScenarioMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntEnemyId {get{ return mHuntEnemyId;} set{ this.mHuntEnemyId = value;}}
		[MessagePack.Key(2)]
		public long _beforeScenarioNumber {get{ return beforeScenarioNumber;} set{ this.beforeScenarioNumber = value;}}
		[MessagePack.Key(3)]
		public long _afterScenarioNumber {get{ return afterScenarioNumber;} set{ this.afterScenarioNumber = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mHuntEnemyId = 0; // $mHuntEnemyId 狩猟イベント敵ID
		[UnityEngine.SerializeField] long beforeScenarioNumber = 0; // $beforeScenarioNumber 戦闘前シナリオ指定（0の場合、無し）
		[UnityEngine.SerializeField] long afterScenarioNumber = 0; // $afterScenarioNumber 戦闘後シナリオ指定（0の場合、無し）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntEnemyScenarioMasterObjectBase {
		public virtual HuntEnemyScenarioMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntEnemyId => _rawData._mHuntEnemyId;
		public virtual long beforeScenarioNumber => _rawData._beforeScenarioNumber;
		public virtual long afterScenarioNumber => _rawData._afterScenarioNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntEnemyScenarioMasterValueObject _rawData = null;
		public HuntEnemyScenarioMasterObjectBase( HuntEnemyScenarioMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntEnemyScenarioMasterObject : HuntEnemyScenarioMasterObjectBase, IMasterObject {
		public HuntEnemyScenarioMasterObject( HuntEnemyScenarioMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntEnemyScenarioMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Enemy_Scenario;

        [UnityEngine.SerializeField]
        HuntEnemyScenarioMasterValueObject[] m_Hunt_Enemy_Scenario = null;
    }


}
