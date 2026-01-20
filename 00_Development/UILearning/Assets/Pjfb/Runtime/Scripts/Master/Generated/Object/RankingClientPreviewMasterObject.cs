//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class RankingClientPreviewMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _tableType {get{ return tableType;} set{ this.tableType = value;}}
		[MessagePack.Key(2)]
		public long _masterId {get{ return masterId;} set{ this.masterId = value;}}
		[MessagePack.Key(3)]
		public long[] _mCommonRankingIdList {get{ return mCommonRankingIdList;} set{ this.mCommonRankingIdList = value;}}
		[MessagePack.Key(4)]
		public long _holdingType {get{ return holdingType;} set{ this.holdingType = value;}}
		[MessagePack.Key(5)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(6)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(7)]
		public string _displayStartAt {get{ return displayStartAt;} set{ this.displayStartAt = value;}}
		[MessagePack.Key(8)]
		public string _displayEndAt {get{ return displayEndAt;} set{ this.displayEndAt = value;}}
		[MessagePack.Key(9)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(10)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(11)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(12)]
		public string _helpImageIdList {get{ return helpImageIdList;} set{ this.helpImageIdList = value;}}
		[MessagePack.Key(13)]
		public string _helpDescriptionList {get{ return helpDescriptionList;} set{ this.helpDescriptionList = value;}}
		[MessagePack.Key(14)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long tableType = 0; // $tableType 1=>m_point_ranking_setting_real_time、2=>m_user_ranking_setting
		[UnityEngine.SerializeField] long masterId = 0; // $masterId tableTypeに紐づくマスタのid
		[UnityEngine.SerializeField] long[] mCommonRankingIdList = null; // $mCommonRankingIdList [1（ギルド）, 2（ユーザー）] enabledTypeFlgの順番と同じ順番で記載
		[UnityEngine.SerializeField] long holdingType = 0; // $holdingType 1=定常開催、2=期間限定開催
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始日時（m_common_rankingのstartAtと合わせる）
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了日時（m_common_rankingのendAtと合わせる）
		[UnityEngine.SerializeField] string displayStartAt = ""; // $displayStartAt 表示開始日時
		[UnityEngine.SerializeField] string displayEndAt = ""; // $displayEndAt 表示終了日時
		[UnityEngine.SerializeField] long imageId = 0; // $imageId 画像ID
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 表示優先度
		[UnityEngine.SerializeField] string description = ""; // $description 説明文
		[UnityEngine.SerializeField] string helpImageIdList = ""; // 遊び方表示部分で使用する画像ID配列。int[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] string helpDescriptionList = ""; // 遊び方表示部分で使用する説明テキスト配列。string[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class RankingClientPreviewMasterObjectBase {
		public virtual RankingClientPreviewMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long tableType => _rawData._tableType;
		public virtual long masterId => _rawData._masterId;
		public virtual long[] mCommonRankingIdList => _rawData._mCommonRankingIdList;
		public virtual long holdingType => _rawData._holdingType;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual string displayStartAt => _rawData._displayStartAt;
		public virtual string displayEndAt => _rawData._displayEndAt;
		public virtual long imageId => _rawData._imageId;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual string description => _rawData._description;
		public virtual string helpImageIdList => _rawData._helpImageIdList;
		public virtual string helpDescriptionList => _rawData._helpDescriptionList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        RankingClientPreviewMasterValueObject _rawData = null;
		public RankingClientPreviewMasterObjectBase( RankingClientPreviewMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class RankingClientPreviewMasterObject : RankingClientPreviewMasterObjectBase, IMasterObject {
		public RankingClientPreviewMasterObject( RankingClientPreviewMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class RankingClientPreviewMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Ranking_Client_Preview;

        [UnityEngine.SerializeField]
        RankingClientPreviewMasterValueObject[] m_Ranking_Client_Preview = null;
    }


}
