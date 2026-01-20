//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingConcentrationMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _grade {get{ return grade;} set{ this.grade = value;}}
		[MessagePack.Key(4)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(5)]
		public long _effectNumber {get{ return effectNumber;} set{ this.effectNumber = value;}}
		[MessagePack.Key(6)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(7)]
		public long _exp {get{ return exp;} set{ this.exp = value;}}
		[MessagePack.Key(8)]
		public long _effectRate {get{ return effectRate;} set{ this.effectRate = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long type = 0; // $type 1=>完全抽選（既存）、2=>concentrationのレベルを使用
		[UnityEngine.SerializeField] long grade = 0; // $grade グレード
		[UnityEngine.SerializeField] long priority = 0; // $priority 優先度。降順
		[UnityEngine.SerializeField] long effectNumber = 0; // $effectNumber 演出番号
		[UnityEngine.SerializeField] long level = 0; // $level concentrationのレベル。type=2の場合に使用
		[UnityEngine.SerializeField] long exp = 0; // $exp レベルに到達するために必要な合計経験値。type=2の場合に使用
		[UnityEngine.SerializeField] long effectRate = 0; // $effectRate 効果倍率。獲得ステータスに反映する（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingConcentrationMasterObjectBase {
		public virtual TrainingConcentrationMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long type => _rawData._type;
		public virtual long grade => _rawData._grade;
		public virtual long priority => _rawData._priority;
		public virtual long effectNumber => _rawData._effectNumber;
		public virtual long level => _rawData._level;
		public virtual long exp => _rawData._exp;
		public virtual long effectRate => _rawData._effectRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingConcentrationMasterValueObject _rawData = null;
		public TrainingConcentrationMasterObjectBase( TrainingConcentrationMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingConcentrationMasterObject : TrainingConcentrationMasterObjectBase, IMasterObject {
		public TrainingConcentrationMasterObject( TrainingConcentrationMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingConcentrationMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Concentration;

        [UnityEngine.SerializeField]
        TrainingConcentrationMasterValueObject[] m_Training_Concentration = null;
    }


}
