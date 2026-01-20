//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(4)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(5)]
		public long _gameType {get{ return gameType;} set{ this.gameType = value;}}
		[MessagePack.Key(6)]
		public long _mGachaSettingId {get{ return mGachaSettingId;} set{ this.mGachaSettingId = value;}}
		[MessagePack.Key(7)]
		public long _bonusInvokeRate {get{ return bonusInvokeRate;} set{ this.bonusInvokeRate = value;}}
		[MessagePack.Key(8)]
		public long _bonusRate {get{ return bonusRate;} set{ this.bonusRate = value;}}
		[MessagePack.Key(9)]
		public long _bonusExpireTime {get{ return bonusExpireTime;} set{ this.bonusExpireTime = value;}}
		[MessagePack.Key(10)]
		public string _helpImageIdList {get{ return helpImageIdList;} set{ this.helpImageIdList = value;}}
		[MessagePack.Key(11)]
		public string _helpDescriptionList {get{ return helpDescriptionList;} set{ this.helpDescriptionList = value;}}
		[MessagePack.Key(12)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理表示名
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId イベントポイントの m_point の ID
		[UnityEngine.SerializeField] long type = 0; // $type イベントのタイプ。1: 通常、2: 追加報酬のみ実施
		[UnityEngine.SerializeField] long gameType = 0; // $gameType イベントミニゲームのタイプ。現状サーバでは意味を持たないが、クライアント等の表示等で区別が必要な場合に使用する
		[UnityEngine.SerializeField] long mGachaSettingId = 0; // $mGachaSettingId イベント画面から遷移できるガチャ台のID。サーバではこのデータを参照しないが、クライアント等の表示等で必要な場合に使用する
		[UnityEngine.SerializeField] long bonusInvokeRate = 0; // $bonusInvokeRate イベント特殊効果が発動する確率（万分率）。これが0の場合、このイベントではイベントボーナスは発動しない
		[UnityEngine.SerializeField] long bonusRate = 0; // $bonusRate イベント特殊効果による獲得ポイントボーナス倍率（万分率）
		[UnityEngine.SerializeField] long bonusExpireTime = 0; // $bonusExpireTime イベント特殊効果の持続期間（秒）
		[UnityEngine.SerializeField] string helpImageIdList = ""; // 遊び方表示部分で使用する画像ID配列。int[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] string helpDescriptionList = ""; // 遊び方表示部分で使用する説明テキスト配列。string[]に相当するjsonを指定する。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalMasterObjectBase {
		public virtual FestivalMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual string name => _rawData._name;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long type => _rawData._type;
		public virtual long gameType => _rawData._gameType;
		public virtual long mGachaSettingId => _rawData._mGachaSettingId;
		public virtual long bonusInvokeRate => _rawData._bonusInvokeRate;
		public virtual long bonusRate => _rawData._bonusRate;
		public virtual long bonusExpireTime => _rawData._bonusExpireTime;
		public virtual string helpImageIdList => _rawData._helpImageIdList;
		public virtual string helpDescriptionList => _rawData._helpDescriptionList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalMasterValueObject _rawData = null;
		public FestivalMasterObjectBase( FestivalMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalMasterObject : FestivalMasterObjectBase, IMasterObject {
		public FestivalMasterObject( FestivalMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival;

        [UnityEngine.SerializeField]
        FestivalMasterValueObject[] m_Festival = null;
    }


}
