//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointGetBonusMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _adminTagId {get{ return adminTagId;} set{ this.adminTagId = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _route {get{ return route;} set{ this.route = value;}}
		[MessagePack.Key(4)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long adminTagId = 0; // $adminTagId タグID
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId ポイントID
		[UnityEngine.SerializeField] long route = 0; // $route 経路種別（1 => hunt, 2 => colosseum）
		[UnityEngine.SerializeField] long rate = 0; // $rate 加算倍率。万分率。1.1倍にしたい場合は、1000と入力。複数のボーナスが重複してかかる場合は、rate同士を合算して最終倍率とする（1000 + 2000 = 3000 => 1.3倍ボーナス））
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointGetBonusMasterObjectBase {
		public virtual PointGetBonusMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long adminTagId => _rawData._adminTagId;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long route => _rawData._route;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointGetBonusMasterValueObject _rawData = null;
		public PointGetBonusMasterObjectBase( PointGetBonusMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointGetBonusMasterObject : PointGetBonusMasterObjectBase, IMasterObject {
		public PointGetBonusMasterObject( PointGetBonusMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointGetBonusMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Get_Bonus;

        [UnityEngine.SerializeField]
        PointGetBonusMasterValueObject[] m_Point_Get_Bonus = null;
    }


}
