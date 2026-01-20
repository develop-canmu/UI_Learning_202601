//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class LoginStampMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _prizeCount {get{ return prizeCount;} set{ this.prizeCount = value;}}
		[MessagePack.Key(3)]
		public long _effectDate {get{ return effectDate;} set{ this.effectDate = value;}}
		[MessagePack.Key(4)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(5)]
		public long _displayType {get{ return displayType;} set{ this.displayType = value;}}
		[MessagePack.Key(6)]
		public string _voiceList {get{ return voiceList;} set{ this.voiceList = value;}}
		[MessagePack.Key(7)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(8)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(9)]
		public long _bgImageId {get{ return bgImageId;} set{ this.bgImageId = value;}}
		[MessagePack.Key(10)]
		public long _textImageId {get{ return textImageId;} set{ this.textImageId = value;}}
		[MessagePack.Key(11)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(12)]
		public string _conditionJson {get{ return conditionJson;} set{ this.conditionJson = value;}}
		[MessagePack.Key(13)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // id
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long prizeCount = 0; // 報酬が存在する全体の日数
		[UnityEngine.SerializeField] long effectDate = 0; // 効果表示日数
		[UnityEngine.SerializeField] long displayPriority = 0; // ログインボーナス画面が連続して表示される場合の表示優先度。この値が高いものから順に表示される。
		[UnityEngine.SerializeField] long displayType = 0; // 表示形式。1 => 通常、 2 => 不可視（報酬は付与されるが、ログボ受け取り演出は発生しない）
		[UnityEngine.SerializeField] string voiceList = ""; // ボイスをJSONの配列で指定する（複数のボイスを指定した場合はランダムで再生される）
		[UnityEngine.SerializeField] string startAt = ""; // 開始日
		[UnityEngine.SerializeField] string endAt = ""; // 終了日時
		[UnityEngine.SerializeField] long bgImageId = 0; // 背景画像ID。クライアントでのみ使用する
		[UnityEngine.SerializeField] long textImageId = 0; // ロゴ画像ID。クライアントでのみ使用する
		[UnityEngine.SerializeField] long imageId = 0; // イラストID。クライアントでのみ使用する
		[UnityEngine.SerializeField] string conditionJson = ""; // 条件Json
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class LoginStampMasterObjectBase {
		public virtual LoginStampMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long prizeCount => _rawData._prizeCount;
		public virtual long effectDate => _rawData._effectDate;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual long displayType => _rawData._displayType;
		public virtual string voiceList => _rawData._voiceList;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual long bgImageId => _rawData._bgImageId;
		public virtual long textImageId => _rawData._textImageId;
		public virtual long imageId => _rawData._imageId;
		public virtual string conditionJson => _rawData._conditionJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        LoginStampMasterValueObject _rawData = null;
		public LoginStampMasterObjectBase( LoginStampMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class LoginStampMasterObject : LoginStampMasterObjectBase, IMasterObject {
		public LoginStampMasterObject( LoginStampMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class LoginStampMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Login_Stamp;

        [UnityEngine.SerializeField]
        LoginStampMasterValueObject[] m_Login_Stamp = null;
    }


}
