//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingAutoCostMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(4)]
		public long _count {get{ return count;} set{ this.count = value;}}
		[MessagePack.Key(5)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(6)]
		public long _targetValue {get{ return targetValue;} set{ this.targetValue = value;}}
		[MessagePack.Key(7)]
		public string _adminTagIdList {get{ return adminTagIdList;} set{ this.adminTagIdList = value;}}
		[MessagePack.Key(8)]
		public string _mBillingRewardBonusIdList {get{ return mBillingRewardBonusIdList;} set{ this.mBillingRewardBonusIdList = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long type = 0; // $type 1: 即完了、2: 時間短縮(m)、3: 回数増加、4: 同時実行枠解放
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId コストポイントID。タグでの解放の場合は0（type=4の場合はコストでの解放は不可。タグ解放のみ）
		[UnityEngine.SerializeField] long value = 0; // $value コスト量。タグでの解放の場合は0 （type=4の場合はコストでの解放は不可。タグ解放のみ）
		[UnityEngine.SerializeField] long count = 0; // $count mPointId単位で何回目の実行か
		[UnityEngine.SerializeField] long priority = 0; // $priority 消費順。降順
		[UnityEngine.SerializeField] long targetValue = 0; // $targetValue type=1: 設定なし, type=2: 何分短縮するか、type=3: 何回実行可能回数を増やすか、type=4: 解放枠数
		[UnityEngine.SerializeField] string adminTagIdList = ""; // $adminTagIdList コストポイントの代わりにタグで実行する場合に設定。[1, 2, 3]
		[UnityEngine.SerializeField] string mBillingRewardBonusIdList = ""; // $mBillingRewardBonusIdList 解放に使用するログインパスの報酬IDリスト。サーバからは使用せず、クライアントの表示用。[1, 2, 3]
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingAutoCostMasterObjectBase {
		public virtual TrainingAutoCostMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual long count => _rawData._count;
		public virtual long priority => _rawData._priority;
		public virtual long targetValue => _rawData._targetValue;
		public virtual string adminTagIdList => _rawData._adminTagIdList;
		public virtual string mBillingRewardBonusIdList => _rawData._mBillingRewardBonusIdList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingAutoCostMasterValueObject _rawData = null;
		public TrainingAutoCostMasterObjectBase( TrainingAutoCostMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingAutoCostMasterObject : TrainingAutoCostMasterObjectBase, IMasterObject {
		public TrainingAutoCostMasterObject( TrainingAutoCostMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingAutoCostMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Auto_Cost;

        [UnityEngine.SerializeField]
        TrainingAutoCostMasterValueObject[] m_Training_Auto_Cost = null;
    }


}
