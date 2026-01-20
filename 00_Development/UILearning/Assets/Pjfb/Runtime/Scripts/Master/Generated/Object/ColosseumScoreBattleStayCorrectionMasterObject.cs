//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumScoreBattleStayCorrectionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public long _threshold {get{ return threshold;} set{ this.threshold = value;}}
		[MessagePack.Key(3)]
		public long _additionRate {get{ return additionRate;} set{ this.additionRate = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long type = 0; // $type 適用分類。1 => 在位期間（分）、 2 => 返り討ち数
		[UnityEngine.SerializeField] long threshold = 0; // $threshold しきい値。typeによって参照値が変化（分や返り討ちすうなど）
		[UnityEngine.SerializeField] long additionRate = 0; // $additionRate 加算倍率。万分率。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumScoreBattleStayCorrectionMasterObjectBase {
		public virtual ColosseumScoreBattleStayCorrectionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual long threshold => _rawData._threshold;
		public virtual long additionRate => _rawData._additionRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumScoreBattleStayCorrectionMasterValueObject _rawData = null;
		public ColosseumScoreBattleStayCorrectionMasterObjectBase( ColosseumScoreBattleStayCorrectionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumScoreBattleStayCorrectionMasterObject : ColosseumScoreBattleStayCorrectionMasterObjectBase, IMasterObject {
		public ColosseumScoreBattleStayCorrectionMasterObject( ColosseumScoreBattleStayCorrectionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumScoreBattleStayCorrectionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Score_Battle_Stay_Correction;

        [UnityEngine.SerializeField]
        ColosseumScoreBattleStayCorrectionMasterValueObject[] m_Colosseum_Score_Battle_Stay_Correction = null;
    }


}
