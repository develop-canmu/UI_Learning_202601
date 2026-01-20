//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CombinationProgressMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCombinationProgressGroupId {get{ return mCombinationProgressGroupId;} set{ this.mCombinationProgressGroupId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _conditionType {get{ return conditionType;} set{ this.conditionType = value;}}
		[MessagePack.Key(4)]
		public long _conditionValue {get{ return conditionValue;} set{ this.conditionValue = value;}}
		[MessagePack.Key(5)]
		public long _conditionValueSub {get{ return conditionValueSub;} set{ this.conditionValueSub = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCombinationProgressGroupId = 0; // $mCombinationProgressGroupId グループID
		[UnityEngine.SerializeField] long level = 0; // $level 段階。0スタート。「そのレベルから次のレベルに強化するときの条件」の設定を入れる。
		[UnityEngine.SerializeField] long conditionType = 0; // $conditionType 条件種別(1 => 所持数, 2 => 所持数・潜在レベル到達, 3 => 所持数・強化レベル到達)
		[UnityEngine.SerializeField] long conditionValue = 0; // $conditionValue 条件値1（type1・2のとき対象キャラの必要所持数）
		[UnityEngine.SerializeField] long conditionValueSub = 0; // $conditionValueSub 条件値2 （type2のとき、対象キャラたちが達している必要がある潜在レベル。それ以外の時は0とか入れる）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CombinationProgressMasterObjectBase {
		public virtual CombinationProgressMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCombinationProgressGroupId => _rawData._mCombinationProgressGroupId;
		public virtual long level => _rawData._level;
		public virtual long conditionType => _rawData._conditionType;
		public virtual long conditionValue => _rawData._conditionValue;
		public virtual long conditionValueSub => _rawData._conditionValueSub;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CombinationProgressMasterValueObject _rawData = null;
		public CombinationProgressMasterObjectBase( CombinationProgressMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CombinationProgressMasterObject : CombinationProgressMasterObjectBase, IMasterObject {
		public CombinationProgressMasterObject( CombinationProgressMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CombinationProgressMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Combination_Progress;

        [UnityEngine.SerializeField]
        CombinationProgressMasterValueObject[] m_Combination_Progress = null;
    }


}
