//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointRankingSettingRealTimeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(4)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(5)]
		public string _enabledTypeFlg {get{ return enabledTypeFlg;} set{ this.enabledTypeFlg = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称を入れる
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId 集計する通貨の種類
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始時刻
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了時刻
		[UnityEngine.SerializeField] string enabledTypeFlg = ""; // $enabledTypeFlg 実際に集計をしたいランキングの種類をビットフラグでまとめる。例：11111。この場合、全パターンの集計を行う。1 => 集計する、0 => 集計しない。大きい桁から順に「ユーザールーキー、ギルドデイリー、ユーザーデイリー、ギルド、ユーザー」の設定となっている
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointRankingSettingRealTimeMasterObjectBase {
		public virtual PointRankingSettingRealTimeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long mPointId => _rawData._mPointId;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual string enabledTypeFlg => _rawData._enabledTypeFlg;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointRankingSettingRealTimeMasterValueObject _rawData = null;
		public PointRankingSettingRealTimeMasterObjectBase( PointRankingSettingRealTimeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointRankingSettingRealTimeMasterObject : PointRankingSettingRealTimeMasterObjectBase, IMasterObject {
		public PointRankingSettingRealTimeMasterObject( PointRankingSettingRealTimeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointRankingSettingRealTimeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Ranking_Setting_Real_Time;

        [UnityEngine.SerializeField]
        PointRankingSettingRealTimeMasterValueObject[] m_Point_Ranking_Setting_Real_Time = null;
    }


}
