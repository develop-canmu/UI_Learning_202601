//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class EnhanceLevelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mEnhanceId {get{ return mEnhanceId;} set{ this.mEnhanceId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _totalExp {get{ return totalExp;} set{ this.totalExp = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mEnhanceId = 0; // 強化設定（レベル管理）ID
		[UnityEngine.SerializeField] long level = 0; // レベル
		[UnityEngine.SerializeField] long totalExp = 0; // 累計経験値（そのレベルに到達するのに最低限必要な量。）
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class EnhanceLevelMasterObjectBase {
		public virtual EnhanceLevelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mEnhanceId => _rawData._mEnhanceId;
		public virtual long level => _rawData._level;
		public virtual long totalExp => _rawData._totalExp;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        EnhanceLevelMasterValueObject _rawData = null;
		public EnhanceLevelMasterObjectBase( EnhanceLevelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class EnhanceLevelMasterObject : EnhanceLevelMasterObjectBase, IMasterObject {
		public EnhanceLevelMasterObject( EnhanceLevelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class EnhanceLevelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Enhance_Level;

        [UnityEngine.SerializeField]
        EnhanceLevelMasterValueObject[] m_Enhance_Level = null;
    }


}
