//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _practiceName {get{ return practiceName;} set{ this.practiceName = value;}}
		[MessagePack.Key(2)]
		public string _alterName {get{ return alterName;} set{ this.alterName = value;}}
		[MessagePack.Key(3)]
		public long _cardGroupType {get{ return cardGroupType;} set{ this.cardGroupType = value;}}
		[MessagePack.Key(4)]
		public long _practiceType {get{ return practiceType;} set{ this.practiceType = value;}}
		[MessagePack.Key(5)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(6)]
		public long _nameImageId {get{ return nameImageId;} set{ this.nameImageId = value;}}
		[MessagePack.Key(7)]
		public long _backgroundId {get{ return backgroundId;} set{ this.backgroundId = value;}}
		[MessagePack.Key(8)]
		public long _routeType {get{ return routeType;} set{ this.routeType = value;}}
		[MessagePack.Key(9)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(10)]
		public long _exp {get{ return exp;} set{ this.exp = value;}}
		[MessagePack.Key(11)]
		public long _grade {get{ return grade;} set{ this.grade = value;}}
		[MessagePack.Key(12)]
		public long _inspireRate {get{ return inspireRate;} set{ this.inspireRate = value;}}
		[MessagePack.Key(13)]
		public long _coachEnhanceRate {get{ return coachEnhanceRate;} set{ this.coachEnhanceRate = value;}}
		[MessagePack.Key(14)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(15)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string practiceName = ""; // $practiceName 練習名
		[UnityEngine.SerializeField] string alterName = ""; // $alterName 練習名代替テキスト。演出などに使用
		[UnityEngine.SerializeField] long cardGroupType = 0; // $cardGroupType カードグループ。0: 基本、1: スペシャルトレーニング
		[UnityEngine.SerializeField] long practiceType = 0; // $practiceType 練習タイプ。行動では毎回、各練習タイプのカードが1枚ずつ選択される。練習カードが5枚とする場合は0から4で指定する。
		[UnityEngine.SerializeField] long imageId = 0; // $imageId 練習カードの画像ID
		[UnityEngine.SerializeField] long nameImageId = 0; // $nameImageId 練習カードの名前画像ID
		[UnityEngine.SerializeField] long backgroundId = 0; // $backgroundId 練習カードの背景の画像ID
		[UnityEngine.SerializeField] long routeType = 0; // $routeType 練習カードの獲得経路タイプ。1:MCharaから、2:マス盤から。サーバ側ではこの値を使用しない
		[UnityEngine.SerializeField] long priority = 0; // $priority 基本練習をこの練習で置き換える抽選を行う際、この値が高いカードから優先して抽選する
		[UnityEngine.SerializeField] long exp = 0; // $exp この練習カードを選択した際の獲得経験値
		[UnityEngine.SerializeField] long grade = 0; // $grade グレード
		[UnityEngine.SerializeField] long inspireRate = 0; // $inspireRate インスピレーション付与確率（万分率）。0を基準とし、10000でこのカードにしか付与されなくなる
		[UnityEngine.SerializeField] long coachEnhanceRate = 0; // $coachEnhanceRate 特訓発生確率アップ率。この練習カードにおける特訓の発生率に上昇補正をかける（万分率）
		[UnityEngine.SerializeField] string description = ""; // $description 説明文
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardMasterObjectBase {
		public virtual TrainingCardMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string practiceName => _rawData._practiceName;
		public virtual string alterName => _rawData._alterName;
		public virtual long cardGroupType => _rawData._cardGroupType;
		public virtual long practiceType => _rawData._practiceType;
		public virtual long imageId => _rawData._imageId;
		public virtual long nameImageId => _rawData._nameImageId;
		public virtual long backgroundId => _rawData._backgroundId;
		public virtual long routeType => _rawData._routeType;
		public virtual long priority => _rawData._priority;
		public virtual long exp => _rawData._exp;
		public virtual long grade => _rawData._grade;
		public virtual long inspireRate => _rawData._inspireRate;
		public virtual long coachEnhanceRate => _rawData._coachEnhanceRate;
		public virtual string description => _rawData._description;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardMasterValueObject _rawData = null;
		public TrainingCardMasterObjectBase( TrainingCardMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardMasterObject : TrainingCardMasterObjectBase, IMasterObject {
		public TrainingCardMasterObject( TrainingCardMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card;

        [UnityEngine.SerializeField]
        TrainingCardMasterValueObject[] m_Training_Card = null;
    }


}
