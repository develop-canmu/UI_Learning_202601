//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardComboMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _groupId {get{ return groupId;} set{ this.groupId = value;}}
		[MessagePack.Key(3)]
		public long _baseStatusRate {get{ return baseStatusRate;} set{ this.baseStatusRate = value;}}
		[MessagePack.Key(4)]
		public long _comboBonusRate {get{ return comboBonusRate;} set{ this.comboBonusRate = value;}}
		[MessagePack.Key(5)]
		public long _forceRate {get{ return forceRate;} set{ this.forceRate = value;}}
		[MessagePack.Key(6)]
		public long _forceLimit {get{ return forceLimit;} set{ this.forceLimit = value;}}
		[MessagePack.Key(7)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(8)]
		public long _effectNumber {get{ return effectNumber;} set{ this.effectNumber = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long groupId = 0; // $groupId mTrainingCardComboをまとめるためのID。同一のカードを使用したコンボ指定が複数発生している場合（AB、ABC、ABCD…）に、その中で最も効果の高い（コンボ数の多い）効果のみが発生する
		[UnityEngine.SerializeField] long baseStatusRate = 0; // $baseStatusRate 実行カードに対し、その他のカードから何割のステータスを加算するかを万分率指定
		[UnityEngine.SerializeField] long comboBonusRate = 0; // $comboBonusRate 当該レコードにおけるコンボ発生時のステータスボーナス値倍率を万分率指定
		[UnityEngine.SerializeField] long forceRate = 0; // $forceRate コンボグループに指定されたカードの何れかが当選した場合に、同じグループのいずれかのカードを強制的に当選させる確率を万分率指定。popRateUpと同じ
		[UnityEngine.SerializeField] long forceLimit = 0; // $forceLimit forceRateが発動する上限回数を整数指定
		[UnityEngine.SerializeField] long priority = 0; // $priority forceRate判定を実行する優先順位
		[UnityEngine.SerializeField] long effectNumber = 0; // $effectNumber 演出番号
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardComboMasterObjectBase {
		public virtual TrainingCardComboMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long groupId => _rawData._groupId;
		public virtual long baseStatusRate => _rawData._baseStatusRate;
		public virtual long comboBonusRate => _rawData._comboBonusRate;
		public virtual long forceRate => _rawData._forceRate;
		public virtual long forceLimit => _rawData._forceLimit;
		public virtual long priority => _rawData._priority;
		public virtual long effectNumber => _rawData._effectNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardComboMasterValueObject _rawData = null;
		public TrainingCardComboMasterObjectBase( TrainingCardComboMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardComboMasterObject : TrainingCardComboMasterObjectBase, IMasterObject {
		public TrainingCardComboMasterObject( TrainingCardComboMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardComboMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Combo;

        [UnityEngine.SerializeField]
        TrainingCardComboMasterValueObject[] m_Training_Card_Combo = null;
    }


}
