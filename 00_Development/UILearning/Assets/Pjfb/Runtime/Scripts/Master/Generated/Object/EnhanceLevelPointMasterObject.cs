//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class EnhanceLevelPointMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mEnhanceId {get{ return mEnhanceId;} set{ this.mEnhanceId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(4)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mEnhanceId = 0; // $mEnhanceId 強化設定（レベル管理）ID
		[UnityEngine.SerializeField] long level = 0; // $level レベル
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId mPoint種類
		[UnityEngine.SerializeField] long value = 0; // $totalNum レベルに達するのに必要なmPointの数
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class EnhanceLevelPointMasterObjectBase {
		public virtual EnhanceLevelPointMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mEnhanceId => _rawData._mEnhanceId;
		public virtual long level => _rawData._level;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        EnhanceLevelPointMasterValueObject _rawData = null;
		public EnhanceLevelPointMasterObjectBase( EnhanceLevelPointMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class EnhanceLevelPointMasterObject : EnhanceLevelPointMasterObjectBase, IMasterObject {
		public EnhanceLevelPointMasterObject( EnhanceLevelPointMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class EnhanceLevelPointMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Enhance_Level_Point;

        [UnityEngine.SerializeField]
        EnhanceLevelPointMasterValueObject[] m_Enhance_Level_Point = null;
    }


}
