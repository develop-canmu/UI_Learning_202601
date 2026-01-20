//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TutorialSettingMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public string _helpDescriptionList {get{ return helpDescriptionList;} set{ this.helpDescriptionList = value;}}
		[MessagePack.Key(3)]
		public string _helpImageIdList {get{ return helpImageIdList;} set{ this.helpImageIdList = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long type = 0; // $type 種別。クライアント側で指定
		[UnityEngine.SerializeField] string helpDescriptionList = ""; // $helpDescriptionList 遊び方表示部分で使用する説明テキスト配列。string[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] string helpImageIdList = ""; // $helpImageIdList 遊び方表示部分で使用する画像ID配列。int[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TutorialSettingMasterObjectBase {
		public virtual TutorialSettingMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual string helpDescriptionList => _rawData._helpDescriptionList;
		public virtual string helpImageIdList => _rawData._helpImageIdList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TutorialSettingMasterValueObject _rawData = null;
		public TutorialSettingMasterObjectBase( TutorialSettingMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TutorialSettingMasterObject : TutorialSettingMasterObjectBase, IMasterObject {
		public TutorialSettingMasterObject( TutorialSettingMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TutorialSettingMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Tutorial_Setting;

        [UnityEngine.SerializeField]
        TutorialSettingMasterValueObject[] m_Tutorial_Setting = null;
    }


}
