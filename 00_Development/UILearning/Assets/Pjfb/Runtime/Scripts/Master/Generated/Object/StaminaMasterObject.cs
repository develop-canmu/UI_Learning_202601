//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class StaminaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _cureType {get{ return cureType;} set{ this.cureType = value;}}
		[MessagePack.Key(3)]
		public long _max {get{ return max;} set{ this.max = value;}}
		[MessagePack.Key(4)]
		public long _cureSecond {get{ return cureSecond;} set{ this.cureSecond = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0;
		[UnityEngine.SerializeField] string name = ""; // ユーザー提示名
		[UnityEngine.SerializeField] long cureType = 0; // 回復区分
		[UnityEngine.SerializeField] long max = 0; // スタミナ最大量
		[UnityEngine.SerializeField] long cureSecond = 0; // スタミナ回復秒数
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class StaminaMasterObjectBase {
		public virtual StaminaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long cureType => _rawData._cureType;
		public virtual long max => _rawData._max;
		public virtual long cureSecond => _rawData._cureSecond;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        StaminaMasterValueObject _rawData = null;
		public StaminaMasterObjectBase( StaminaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class StaminaMasterObject : StaminaMasterObjectBase, IMasterObject {
		public StaminaMasterObject( StaminaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class StaminaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Stamina;

        [UnityEngine.SerializeField]
        StaminaMasterValueObject[] m_Stamina = null;
    }


}
