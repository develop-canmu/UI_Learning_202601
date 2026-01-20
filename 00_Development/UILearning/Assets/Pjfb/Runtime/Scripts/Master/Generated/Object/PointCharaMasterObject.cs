//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PointCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _valueToBuy {get{ return valueToBuy;} set{ this.valueToBuy = value;}}
		[MessagePack.Key(4)]
		public long _valueToSale {get{ return valueToSale;} set{ this.valueToSale = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mCharaId = 0; // キャラID
		[UnityEngine.SerializeField] long mPointId = 0; // ポイントID
		[UnityEngine.SerializeField] long valueToBuy = 0; // chara購入時の値段
		[UnityEngine.SerializeField] long valueToSale = 0; // chara売却時の値段
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PointCharaMasterObjectBase {
		public virtual PointCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long valueToBuy => _rawData._valueToBuy;
		public virtual long valueToSale => _rawData._valueToSale;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PointCharaMasterValueObject _rawData = null;
		public PointCharaMasterObjectBase( PointCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PointCharaMasterObject : PointCharaMasterObjectBase, IMasterObject {
		public PointCharaMasterObject( PointCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PointCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Point_Chara;

        [UnityEngine.SerializeField]
        PointCharaMasterValueObject[] m_Point_Chara = null;
    }


}
