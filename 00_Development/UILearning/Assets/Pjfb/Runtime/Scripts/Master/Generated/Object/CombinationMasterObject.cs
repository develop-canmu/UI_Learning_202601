//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CombinationMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(4)]
		public long _mCombinationProgressGroupId {get{ return mCombinationProgressGroupId;} set{ this.mCombinationProgressGroupId = value;}}
		[MessagePack.Key(5)]
		public long _mCombinationEnhanceGroupId {get{ return mCombinationEnhanceGroupId;} set{ this.mCombinationEnhanceGroupId = value;}}
		[MessagePack.Key(6)]
		public long _mCombinationMaterialGroupId {get{ return mCombinationMaterialGroupId;} set{ this.mCombinationMaterialGroupId = value;}}
		[MessagePack.Key(7)]
		public long _isOpened {get{ return isOpened;} set{ this.isOpened = value;}}
		[MessagePack.Key(8)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(9)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(10)]
		public long _mServerId {get{ return mServerId;} set{ this.mServerId = value;}}
		[MessagePack.Key(11)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理側で使用する名称
		[UnityEngine.SerializeField] string name = ""; // $name ユーザ側で使用する名称
		[UnityEngine.SerializeField] long mRarityId = 0; // レアリティID
		[UnityEngine.SerializeField] long mCombinationProgressGroupId = 0; // $mCombinationProgressGroupId コンビネーションの達成度合い上昇設定への参照
		[UnityEngine.SerializeField] long mCombinationEnhanceGroupId = 0; // $mCombinationEnhanceGroupId コンビネーションの強化レベル上昇設定への参照
		[UnityEngine.SerializeField] long mCombinationMaterialGroupId = 0; // $mCombinationMaterialGroupId コンビネーションの強化に使用可能なポイント素材群への参照 / 0の場合、ポイントを素材として用いることはできない
		[UnityEngine.SerializeField] long isOpened = 0; // $isOpened デフォルトで開放されているかどうか
		[UnityEngine.SerializeField] string startAt = ""; // $startAt いつから有効となるか
		[UnityEngine.SerializeField] long sortNumber = 0; // $sortNumber 表示順。デフォルトでは昇順
		[UnityEngine.SerializeField] long mServerId = 0; // $mServerId サーバーID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CombinationMasterObjectBase {
		public virtual CombinationMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual string name => _rawData._name;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual long mCombinationProgressGroupId => _rawData._mCombinationProgressGroupId;
		public virtual long mCombinationEnhanceGroupId => _rawData._mCombinationEnhanceGroupId;
		public virtual long mCombinationMaterialGroupId => _rawData._mCombinationMaterialGroupId;
		public virtual long isOpened => _rawData._isOpened;
		public virtual string startAt => _rawData._startAt;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual long mServerId => _rawData._mServerId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CombinationMasterValueObject _rawData = null;
		public CombinationMasterObjectBase( CombinationMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CombinationMasterObject : CombinationMasterObjectBase, IMasterObject {
		public CombinationMasterObject( CombinationMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CombinationMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Combination;

        [UnityEngine.SerializeField]
        CombinationMasterValueObject[] m_Combination = null;
    }


}
