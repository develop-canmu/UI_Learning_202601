//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointAlternativeMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(2)]
		public long _mPointIdAlternative {get{ return mPointIdAlternative;} set{ this.mPointIdAlternative = value;}}
		[MessagePack.Key(3)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(4)]
		public long[] _targetIdList {get{ return targetIdList;} set{ this.targetIdList = value;}}
		[MessagePack.Key(5)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(6)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(7)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mPointId = 0; // ジェム等のもととなる本来のポイントのID
		[UnityEngine.SerializeField] long mPointIdAlternative = 0; // 代わりに消費させたいポイントのID
		[UnityEngine.SerializeField] long useType = 0; // 用途：1 => ガチャ、999 => 全て
		[UnityEngine.SerializeField] long[] targetIdList = null; // useTypeに応じて使用先のIDリストを設定。useType=1=>mGachaCategoryId
		[UnityEngine.SerializeField] long priority = 0; // 優先度。降順
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始時刻
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了時刻
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointAlternativeMasterObjectBase {
		public virtual PointAlternativeMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long mPointIdAlternative => _rawData._mPointIdAlternative;
		public virtual long useType => _rawData._useType;
		public virtual long[] targetIdList => _rawData._targetIdList;
		public virtual long priority => _rawData._priority;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointAlternativeMasterValueObject _rawData = null;
		public PointAlternativeMasterObjectBase( PointAlternativeMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointAlternativeMasterObject : PointAlternativeMasterObjectBase, IMasterObject {
		public PointAlternativeMasterObject( PointAlternativeMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointAlternativeMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Alternative;

        [UnityEngine.SerializeField]
        PointAlternativeMasterValueObject[] m_Point_Alternative = null;
    }


}
