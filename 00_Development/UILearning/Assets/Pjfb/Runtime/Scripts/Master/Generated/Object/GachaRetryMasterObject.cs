//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class GachaRetryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public bool _canDuplicate {get{ return canDuplicate;} set{ this.canDuplicate = value;}}
		[MessagePack.Key(2)]
		public long _retryLimit {get{ return retryLimit;} set{ this.retryLimit = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] bool canDuplicate = false; // $canDuplicate 重複当選可能設定
		[UnityEngine.SerializeField] long retryLimit = 0; // $retryLimit 実行上限。-1が指定されている場合、無制限に実施可能。無制限実行の場合、極力canDuplicateは1にする。無制限実行設定の場合、ポイントの消費は発生しない。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class GachaRetryMasterObjectBase {
		public virtual GachaRetryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual bool canDuplicate => _rawData._canDuplicate;
		public virtual long retryLimit => _rawData._retryLimit;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        GachaRetryMasterValueObject _rawData = null;
		public GachaRetryMasterObjectBase( GachaRetryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class GachaRetryMasterObject : GachaRetryMasterObjectBase, IMasterObject {
		public GachaRetryMasterObject( GachaRetryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class GachaRetryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Gacha_Retry;

        [UnityEngine.SerializeField]
        GachaRetryMasterValueObject[] m_Gacha_Retry = null;
    }


}
