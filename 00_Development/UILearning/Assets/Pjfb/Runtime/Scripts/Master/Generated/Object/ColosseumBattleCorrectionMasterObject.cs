//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ColosseumBattleCorrectionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public long _optionType {get{ return optionType;} set{ this.optionType = value;}}
		[MessagePack.Key(3)]
		public long _threshold {get{ return threshold;} set{ this.threshold = value;}}
		[MessagePack.Key(4)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(5)]
		public long _labelNumber {get{ return labelNumber;} set{ this.labelNumber = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long type = 0; // $type 種別：1 => 特殊デッキ使用時の、コンディション乱数に対応するバフ
		[UnityEngine.SerializeField] long optionType = 0; // $optionType typeによって意味合いを変える。1 => 抽選時に使用する、サブ引数値（勝利直後か否かで、抽選テーブルを変える等の運用で使うやつ）
		[UnityEngine.SerializeField] long threshold = 0; // $threshold 各種しきい値。typeによって挙動を変える。1 => 重み付け（0～10000）。並び順が結果に影響を与えるので、変更はしないようにする（サーバー、クライアントともにid順に処理するようにする）
		[UnityEngine.SerializeField] long rate = 0; // $rate ステータスに対する倍率（万分率）。複数の補正がかかる場合はrate同士を加算した後に、実数値に掛け算するようにする
		[UnityEngine.SerializeField] long labelNumber = 0; // $labelNumber typeによって意味合いを変える。1 => クライアントサイドでラベルに表示する「EXTREME」などの状態区別用に使う値
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ColosseumBattleCorrectionMasterObjectBase {
		public virtual ColosseumBattleCorrectionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual long optionType => _rawData._optionType;
		public virtual long threshold => _rawData._threshold;
		public virtual long rate => _rawData._rate;
		public virtual long labelNumber => _rawData._labelNumber;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ColosseumBattleCorrectionMasterValueObject _rawData = null;
		public ColosseumBattleCorrectionMasterObjectBase( ColosseumBattleCorrectionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ColosseumBattleCorrectionMasterObject : ColosseumBattleCorrectionMasterObjectBase, IMasterObject {
		public ColosseumBattleCorrectionMasterObject( ColosseumBattleCorrectionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ColosseumBattleCorrectionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Colosseum_Battle_Correction;

        [UnityEngine.SerializeField]
        ColosseumBattleCorrectionMasterValueObject[] m_Colosseum_Battle_Correction = null;
    }


}
