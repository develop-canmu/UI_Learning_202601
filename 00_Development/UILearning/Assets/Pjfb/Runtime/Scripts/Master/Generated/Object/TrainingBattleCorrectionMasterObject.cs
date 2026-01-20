//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBattleCorrectionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public long _thresholdMin {get{ return thresholdMin;} set{ this.thresholdMin = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long type = 0; // $type1: 練習試合カードレベルによる敵 m_chara_npc ステータス強化倍率2: 育成キャラ総合力による味方 m_chara_npc ステータス強化倍率11: 練習試合カードレベルによる試合勝利時の獲得報酬倍率
		[UnityEngine.SerializeField] long thresholdMin = 0; // $thresholdMin 総合力またはレベルの最小値
		[UnityEngine.SerializeField] long rate = 0; // $rate 倍率（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBattleCorrectionMasterObjectBase {
		public virtual TrainingBattleCorrectionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual long thresholdMin => _rawData._thresholdMin;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBattleCorrectionMasterValueObject _rawData = null;
		public TrainingBattleCorrectionMasterObjectBase( TrainingBattleCorrectionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBattleCorrectionMasterObject : TrainingBattleCorrectionMasterObjectBase, IMasterObject {
		public TrainingBattleCorrectionMasterObject( TrainingBattleCorrectionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBattleCorrectionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Battle_Correction;

        [UnityEngine.SerializeField]
        TrainingBattleCorrectionMasterValueObject[] m_Training_Battle_Correction = null;
    }


}
