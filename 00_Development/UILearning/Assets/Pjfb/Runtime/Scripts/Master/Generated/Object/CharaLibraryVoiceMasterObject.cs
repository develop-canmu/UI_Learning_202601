//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaLibraryVoiceMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _masterType {get{ return masterType;} set{ this.masterType = value;}}
		[MessagePack.Key(2)]
		public long _masterId {get{ return masterId;} set{ this.masterId = value;}}
		[MessagePack.Key(3)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(4)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(5)]
		public long _voiceCharaNumber {get{ return voiceCharaNumber;} set{ this.voiceCharaNumber = value;}}
		[MessagePack.Key(6)]
		public long _locationType {get{ return locationType;} set{ this.locationType = value;}}
		[MessagePack.Key(7)]
		public long[] _receivedLocationTypeList {get{ return receivedLocationTypeList;} set{ this.receivedLocationTypeList = value;}}
		[MessagePack.Key(8)]
		public long _voiceType {get{ return voiceType;} set{ this.voiceType = value;}}
		[MessagePack.Key(9)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(10)]
		public long _opponentMasterType {get{ return opponentMasterType;} set{ this.opponentMasterType = value;}}
		[MessagePack.Key(11)]
		public long _opponentMasterId {get{ return opponentMasterId;} set{ this.opponentMasterId = value;}}
		[MessagePack.Key(12)]
		public long _groupNumber {get{ return groupNumber;} set{ this.groupNumber = value;}}
		[MessagePack.Key(13)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(14)]
		public string _message {get{ return message;} set{ this.message = value;}}
		[MessagePack.Key(15)]
		public long _releaseTrustLevel {get{ return releaseTrustLevel;} set{ this.releaseTrustLevel = value;}}
		[MessagePack.Key(16)]
		public long _mCharaLibraryVoiceEffectId {get{ return mCharaLibraryVoiceEffectId;} set{ this.mCharaLibraryVoiceEffectId = value;}}
		[MessagePack.Key(17)]
		public long _mProfilePartId {get{ return mProfilePartId;} set{ this.mProfilePartId = value;}}
		[MessagePack.Key(18)]
		public bool _isPrimary {get{ return isPrimary;} set{ this.isPrimary = value;}}
		[MessagePack.Key(19)]
		public long _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(20)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long masterType = 0; // 1 => m_chara.parentMCharaId, 2 => m_chara.id
		[UnityEngine.SerializeField] long masterId = 0; // idTypeの区分に応じて、別なテーブルのIDを参照
		[UnityEngine.SerializeField] string adminName = ""; // 管理用名称
		[UnityEngine.SerializeField] string name = ""; // 表示名称
		[UnityEngine.SerializeField] long voiceCharaNumber = 0; // m_chara.voiceCharaNumber。 mChara.masterTypeが1の場合に使用する汎用ボイスをvoiceCharaNumberで切り替える
		[UnityEngine.SerializeField] long locationType = 0; // 各機能で参照する際の用途区分
		[UnityEngine.SerializeField] long[] receivedLocationTypeList = null; // 受け手として使用できるlocationTypeのリスト。主に修行の先行ボイスに対して受け手に使用できるlocationTypeを配列で設定する。未指定の場合はどの修行受け手ボイスでも返答できる想定
		[UnityEngine.SerializeField] long voiceType = 0; // ボイス種別。1 => part, 2 => system, 3 => インゲーム, 4 => インゲーム掛け合い, 5 => scenario, 6 => training_coach
		[UnityEngine.SerializeField] long useType = 0; // 種別内の分類番号(ファイル名のsys_0001_xxxx.oggのxxxx部分の番号)
		[UnityEngine.SerializeField] long opponentMasterType = 0; // 別キャラとの組み合わせ設定。0 => 無条件・汎用, 1 => m_chara.parentMCharaId, 2 => m_chara.id
		[UnityEngine.SerializeField] long opponentMasterId = 0; // 別キャラとの組み合わせ設定（opponentMasterTypeの区分に応じて、別なテーブルのIDを参照）
		[UnityEngine.SerializeField] long groupNumber = 0; // 複数ボイスで1セットの掛け合いとするパターンの場合の、組み合わせ定義用プロパティ
		[UnityEngine.SerializeField] long priority = 0; // 優先度（同一ロケーションでなるボイスが複数ある場合、どれを優先して鳴らすべきかを仕込む情報。大きい方を優先。）
		[UnityEngine.SerializeField] string message = ""; // ボイス再生時に画面に表示される内容
		[UnityEngine.SerializeField] long releaseTrustLevel = 0; // 解放信頼度
		[UnityEngine.SerializeField] long mCharaLibraryVoiceEffectId = 0; // $mCharaLibraryVoiceEffectId セリフ表示UIの演出パターンID
		[UnityEngine.SerializeField] long mProfilePartId = 0; // データ解放用のID
		[UnityEngine.SerializeField] bool isPrimary = false; // 初期から選択できるか。1=>可能、2=>不可能
		[UnityEngine.SerializeField] long description = 0; // 説明文
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaLibraryVoiceMasterObjectBase {
		public virtual CharaLibraryVoiceMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long masterType => _rawData._masterType;
		public virtual long masterId => _rawData._masterId;
		public virtual string adminName => _rawData._adminName;
		public virtual string name => _rawData._name;
		public virtual long voiceCharaNumber => _rawData._voiceCharaNumber;
		public virtual long locationType => _rawData._locationType;
		public virtual long[] receivedLocationTypeList => _rawData._receivedLocationTypeList;
		public virtual long voiceType => _rawData._voiceType;
		public virtual long useType => _rawData._useType;
		public virtual long opponentMasterType => _rawData._opponentMasterType;
		public virtual long opponentMasterId => _rawData._opponentMasterId;
		public virtual long groupNumber => _rawData._groupNumber;
		public virtual long priority => _rawData._priority;
		public virtual string message => _rawData._message;
		public virtual long releaseTrustLevel => _rawData._releaseTrustLevel;
		public virtual long mCharaLibraryVoiceEffectId => _rawData._mCharaLibraryVoiceEffectId;
		public virtual long mProfilePartId => _rawData._mProfilePartId;
		public virtual bool isPrimary => _rawData._isPrimary;
		public virtual long description => _rawData._description;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaLibraryVoiceMasterValueObject _rawData = null;
		public CharaLibraryVoiceMasterObjectBase( CharaLibraryVoiceMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaLibraryVoiceMasterObject : CharaLibraryVoiceMasterObjectBase, IMasterObject {
		public CharaLibraryVoiceMasterObject( CharaLibraryVoiceMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaLibraryVoiceMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Library_Voice;

        [UnityEngine.SerializeField]
        CharaLibraryVoiceMasterValueObject[] m_Chara_Library_Voice = null;
    }


}
