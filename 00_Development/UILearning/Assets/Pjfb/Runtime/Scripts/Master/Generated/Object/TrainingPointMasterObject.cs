//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingPointMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingPointConvertGroup {get{ return mTrainingPointConvertGroup;} set{ this.mTrainingPointConvertGroup = value;}}
		[MessagePack.Key(3)]
		public long _mTrainingPointHandResetCostGroup {get{ return mTrainingPointHandResetCostGroup;} set{ this.mTrainingPointHandResetCostGroup = value;}}
		[MessagePack.Key(4)]
		public long _mTrainingPointStatusLevelGroup {get{ return mTrainingPointStatusLevelGroup;} set{ this.mTrainingPointStatusLevelGroup = value;}}
		[MessagePack.Key(5)]
		public long _mTrainingPointStatusAdditionGroup {get{ return mTrainingPointStatusAdditionGroup;} set{ this.mTrainingPointStatusAdditionGroup = value;}}
		[MessagePack.Key(6)]
		public bool _enabledStatusEffectChara {get{ return enabledStatusEffectChara;} set{ this.enabledStatusEffectChara = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理名
		[UnityEngine.SerializeField] long mTrainingPointConvertGroup = 0; // $mTrainingPointConvertGroup ポイント変換グループID
		[UnityEngine.SerializeField] long mTrainingPointHandResetCostGroup = 0; // $mTrainingPointHandResetCostGroup 練習カードリセットコストグループID
		[UnityEngine.SerializeField] long mTrainingPointStatusLevelGroup = 0; // $mTrainingPointStatusLevelGroup レベルグループID
		[UnityEngine.SerializeField] long mTrainingPointStatusAdditionGroup = 0; // $mTrainingPointStatusAdditionGroup 追加ステータス加算グループID
		[UnityEngine.SerializeField] bool enabledStatusEffectChara = false; // $enabledStatusEffectChara m_training_point_status_effect_charaを使用したブーストが発動するか。1=>有効、2=>無効
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingPointMasterObjectBase {
		public virtual TrainingPointMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual long mTrainingPointConvertGroup => _rawData._mTrainingPointConvertGroup;
		public virtual long mTrainingPointHandResetCostGroup => _rawData._mTrainingPointHandResetCostGroup;
		public virtual long mTrainingPointStatusLevelGroup => _rawData._mTrainingPointStatusLevelGroup;
		public virtual long mTrainingPointStatusAdditionGroup => _rawData._mTrainingPointStatusAdditionGroup;
		public virtual bool enabledStatusEffectChara => _rawData._enabledStatusEffectChara;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingPointMasterValueObject _rawData = null;
		public TrainingPointMasterObjectBase( TrainingPointMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingPointMasterObject : TrainingPointMasterObjectBase, IMasterObject {
		public TrainingPointMasterObject( TrainingPointMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingPointMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Point;

        [UnityEngine.SerializeField]
        TrainingPointMasterValueObject[] m_Training_Point = null;
    }


}
