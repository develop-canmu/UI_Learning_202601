//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingPointStatusEffectCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(3)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(4)]
		public long _mTrainingPointStatusEffectGroup {get{ return mTrainingPointStatusEffectGroup;} set{ this.mTrainingPointStatusEffectGroup = value;}}
		[MessagePack.Key(5)]
		public long _maxInvokeCount {get{ return maxInvokeCount;} set{ this.maxInvokeCount = value;}}
		[MessagePack.Key(6)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(7)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId キャラID
		[UnityEngine.SerializeField] long level = 0; // $level キャラの強化レベル。mCharaIdに紐づくlevelが最大値のレコードのみ取得
		[UnityEngine.SerializeField] long mTrainingPointStatusEffectGroup = 0; // $mTrainingPointStatusEffectGroup 効果内容。m_training_point_status_effect（ブースト効果）のgroupを設定
		[UnityEngine.SerializeField] long maxInvokeCount = 0; // $maxInvokeCount 最大発動回数。-1の場合は制限なし
		[UnityEngine.SerializeField] long priority = 0; // $priority 複数キャラのスペシャルブースト効果発生判定があった場合に優先して発生判定を行う設定。同率の場合はランダム
		[UnityEngine.SerializeField] long imageId = 0; // $imageId 画像ID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingPointStatusEffectCharaMasterObjectBase {
		public virtual TrainingPointStatusEffectCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long level => _rawData._level;
		public virtual long mTrainingPointStatusEffectGroup => _rawData._mTrainingPointStatusEffectGroup;
		public virtual long maxInvokeCount => _rawData._maxInvokeCount;
		public virtual long priority => _rawData._priority;
		public virtual long imageId => _rawData._imageId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingPointStatusEffectCharaMasterValueObject _rawData = null;
		public TrainingPointStatusEffectCharaMasterObjectBase( TrainingPointStatusEffectCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingPointStatusEffectCharaMasterObject : TrainingPointStatusEffectCharaMasterObjectBase, IMasterObject {
		public TrainingPointStatusEffectCharaMasterObject( TrainingPointStatusEffectCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingPointStatusEffectCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Point_Status_Effect_Chara;

        [UnityEngine.SerializeField]
        TrainingPointStatusEffectCharaMasterValueObject[] m_Training_Point_Status_Effect_Chara = null;
    }


}
