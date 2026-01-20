//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _lotteryType {get{ return lotteryType;} set{ this.lotteryType = value;}}
		[MessagePack.Key(3)]
		public bool _hasChoicePrize {get{ return hasChoicePrize;} set{ this.hasChoicePrize = value;}}
		[MessagePack.Key(4)]
		public bool _isClosedOnceWin {get{ return isClosedOnceWin;} set{ this.isClosedOnceWin = value;}}
		[MessagePack.Key(5)]
		public bool _canBulkStart {get{ return canBulkStart;} set{ this.canBulkStart = value;}}
		[MessagePack.Key(6)]
		public long _useMStaminaId {get{ return useMStaminaId;} set{ this.useMStaminaId = value;}}
		[MessagePack.Key(7)]
		public long _useStaminaValue {get{ return useStaminaValue;} set{ this.useStaminaValue = value;}}
		[MessagePack.Key(8)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(9)]
		public string _descriptionShort {get{ return descriptionShort;} set{ this.descriptionShort = value;}}
		[MessagePack.Key(10)]
		public string _helpImageIdList {get{ return helpImageIdList;} set{ this.helpImageIdList = value;}}
		[MessagePack.Key(11)]
		public string _helpDescriptionList {get{ return helpDescriptionList;} set{ this.helpDescriptionList = value;}}
		[MessagePack.Key(12)]
		public string _descriptionUrl {get{ return descriptionUrl;} set{ this.descriptionUrl = value;}}
		[MessagePack.Key(13)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long lotteryType = 0; // 対戦可能な相手のラインナップを(1 => 抽選しない、 2 => 完全無作為に件数分抽選、3 => 初級難易度のものを規定数抽選し、連動させる形で別難易度のものを追従させる、 4 => 進捗管理型。progressが1のenemyから順に倒していく必要があり、keyMPointValueの指定がある場合は該当ポイントを指定数有することも必要となる、5 => 難易度ごとに完全無作為にそれぞれ件数分抽選)
		[UnityEngine.SerializeField] bool hasChoicePrize = false; // 指名報酬機能を（1 ⇒ 使う、2 ⇒ 使わない）
		[UnityEngine.SerializeField] bool isClosedOnceWin = false; // 1回勝利したら再挑戦できないようにするか（1 ⇒ 再挑戦不可能、2 ⇒ 再挑戦可能）
		[UnityEngine.SerializeField] bool canBulkStart = false; // 一括演習が可能か？（1 ⇒ 可能、2 ⇒ 不可能）
		[UnityEngine.SerializeField] long useMStaminaId = 0; // 消費スタミナID
		[UnityEngine.SerializeField] long useStaminaValue = 0; // 消費スタミナ量
		[UnityEngine.SerializeField] string description = ""; // 説明文。バナー画像に文字を入れられない多言語版などで使用する。
		[UnityEngine.SerializeField] string descriptionShort = ""; // 短めの説明文。バナー画像に文字を入れられない多言語版などで使用する。
		[UnityEngine.SerializeField] string helpImageIdList = ""; // 遊び方表示部分で使用する画像ID配列。int[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] string helpDescriptionList = ""; // 遊び方表示部分で使用する説明テキスト配列。string[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] string descriptionUrl = ""; // $descriptionUrl お知らせ等を入れているURL
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntMasterObjectBase {
		public virtual HuntMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long lotteryType => _rawData._lotteryType;
		public virtual bool hasChoicePrize => _rawData._hasChoicePrize;
		public virtual bool isClosedOnceWin => _rawData._isClosedOnceWin;
		public virtual bool canBulkStart => _rawData._canBulkStart;
		public virtual long useMStaminaId => _rawData._useMStaminaId;
		public virtual long useStaminaValue => _rawData._useStaminaValue;
		public virtual string description => _rawData._description;
		public virtual string descriptionShort => _rawData._descriptionShort;
		public virtual string helpImageIdList => _rawData._helpImageIdList;
		public virtual string helpDescriptionList => _rawData._helpDescriptionList;
		public virtual string descriptionUrl => _rawData._descriptionUrl;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntMasterValueObject _rawData = null;
		public HuntMasterObjectBase( HuntMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntMasterObject : HuntMasterObjectBase, IMasterObject {
		public HuntMasterObject( HuntMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt;

        [UnityEngine.SerializeField]
        HuntMasterValueObject[] m_Hunt = null;
    }


}
