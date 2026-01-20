//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _parentMCharaId {get{ return parentMCharaId;} set{ this.parentMCharaId = value;}}
		[MessagePack.Key(2)]
		public long _liberationGroupMCharaId {get{ return liberationGroupMCharaId;} set{ this.liberationGroupMCharaId = value;}}
		[MessagePack.Key(3)]
		public string _standingImageMCharaId {get{ return standingImageMCharaId;} set{ this.standingImageMCharaId = value;}}
		[MessagePack.Key(4)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(5)]
		public string _rubyName {get{ return rubyName;} set{ this.rubyName = value;}}
		[MessagePack.Key(6)]
		public string _shortName {get{ return shortName;} set{ this.shortName = value;}}
		[MessagePack.Key(7)]
		public string _nickname {get{ return nickname;} set{ this.nickname = value;}}
		[MessagePack.Key(8)]
		public string _appearanceType {get{ return appearanceType;} set{ this.appearanceType = value;}}
		[MessagePack.Key(9)]
		public string _motionType {get{ return motionType;} set{ this.motionType = value;}}
		[MessagePack.Key(10)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(11)]
		public long _mRarityId {get{ return mRarityId;} set{ this.mRarityId = value;}}
		[MessagePack.Key(12)]
		public long _mCharaRarityChangeCategoryId {get{ return mCharaRarityChangeCategoryId;} set{ this.mCharaRarityChangeCategoryId = value;}}
		[MessagePack.Key(13)]
		public long _hp {get{ return hp;} set{ this.hp = value;}}
		[MessagePack.Key(14)]
		public long _mp {get{ return mp;} set{ this.mp = value;}}
		[MessagePack.Key(15)]
		public long _atk {get{ return atk;} set{ this.atk = value;}}
		[MessagePack.Key(16)]
		public long _def {get{ return def;} set{ this.def = value;}}
		[MessagePack.Key(17)]
		public long _spd {get{ return spd;} set{ this.spd = value;}}
		[MessagePack.Key(18)]
		public long _tec {get{ return tec;} set{ this.tec = value;}}
		[MessagePack.Key(19)]
		public long _param1 {get{ return param1;} set{ this.param1 = value;}}
		[MessagePack.Key(20)]
		public long _param2 {get{ return param2;} set{ this.param2 = value;}}
		[MessagePack.Key(21)]
		public long _param3 {get{ return param3;} set{ this.param3 = value;}}
		[MessagePack.Key(22)]
		public long _param4 {get{ return param4;} set{ this.param4 = value;}}
		[MessagePack.Key(23)]
		public long _param5 {get{ return param5;} set{ this.param5 = value;}}
		[MessagePack.Key(24)]
		public long _exParam1 {get{ return exParam1;} set{ this.exParam1 = value;}}
		[MessagePack.Key(25)]
		public long _exParam2 {get{ return exParam2;} set{ this.exParam2 = value;}}
		[MessagePack.Key(26)]
		public long _exParam3 {get{ return exParam3;} set{ this.exParam3 = value;}}
		[MessagePack.Key(27)]
		public long _maxLevel {get{ return maxLevel;} set{ this.maxLevel = value;}}
		[MessagePack.Key(28)]
		public long _charaType {get{ return charaType;} set{ this.charaType = value;}}
		[MessagePack.Key(29)]
		public string _cardType {get{ return cardType;} set{ this.cardType = value;}}
		[MessagePack.Key(30)]
		public long _mSizeId {get{ return mSizeId;} set{ this.mSizeId = value;}}
		[MessagePack.Key(31)]
		public long _mEnhanceIdGrowth {get{ return mEnhanceIdGrowth;} set{ this.mEnhanceIdGrowth = value;}}
		[MessagePack.Key(32)]
		public long _mStatusAdditionIdGrowth {get{ return mStatusAdditionIdGrowth;} set{ this.mStatusAdditionIdGrowth = value;}}
		[MessagePack.Key(33)]
		public long _mEnhanceIdLiberation {get{ return mEnhanceIdLiberation;} set{ this.mEnhanceIdLiberation = value;}}
		[MessagePack.Key(34)]
		public long _mStatusAdditionIdLiberation {get{ return mStatusAdditionIdLiberation;} set{ this.mStatusAdditionIdLiberation = value;}}
		[MessagePack.Key(35)]
		public long _liberationExpRate {get{ return liberationExpRate;} set{ this.liberationExpRate = value;}}
		[MessagePack.Key(36)]
		public long _priceFromPiece {get{ return priceFromPiece;} set{ this.priceFromPiece = value;}}
		[MessagePack.Key(37)]
		public long _mCharaTrainerLotteryId {get{ return mCharaTrainerLotteryId;} set{ this.mCharaTrainerLotteryId = value;}}
		[MessagePack.Key(38)]
		public long _voiceCharaNumber {get{ return voiceCharaNumber;} set{ this.voiceCharaNumber = value;}}
		[MessagePack.Key(39)]
		public long _gachaEffectType {get{ return gachaEffectType;} set{ this.gachaEffectType = value;}}
		[MessagePack.Key(40)]
		public long _actionPattern {get{ return actionPattern;} set{ this.actionPattern = value;}}
		[MessagePack.Key(41)]
		public string _recommendCharaStartAt {get{ return recommendCharaStartAt;} set{ this.recommendCharaStartAt = value;}}
		[MessagePack.Key(42)]
		public long _isExtraSupport {get{ return isExtraSupport;} set{ this.isExtraSupport = value;}}
		[MessagePack.Key(43)]
		public long _deckConditionGroup {get{ return deckConditionGroup;} set{ this.deckConditionGroup = value;}}
		[MessagePack.Key(44)]
		public long _leaveWeaponPriority {get{ return leaveWeaponPriority;} set{ this.leaveWeaponPriority = value;}}
		[MessagePack.Key(45)]
		public long _imageEffectId {get{ return imageEffectId;} set{ this.imageEffectId = value;}}
		[MessagePack.Key(46)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long parentMCharaId = 0; // 親キャラクターID（同一パーティへの編成を制限する。また、スキーマバージョン1でのみ潜在開放上の同一キャラとみなす）
		[UnityEngine.SerializeField] long liberationGroupMCharaId = 0; // 潜在解放共有キャラIDグループ（スキーマバージョン2でのみ使用）
		[UnityEngine.SerializeField] string standingImageMCharaId = ""; // 立ち絵表示用キャラクターID文字列（スキーマバージョン2でのみ使用）
		[UnityEngine.SerializeField] string name = ""; // ユーザ側で使用する名称
		[UnityEngine.SerializeField] string rubyName = ""; // ルビ（ふりがな）付きの名称。「<r=なまえ>名前</r>」のように設定する
		[UnityEngine.SerializeField] string shortName = ""; // 短縮名/呼称
		[UnityEngine.SerializeField] string nickname = ""; // 二つ名/ニックネーム
		[UnityEngine.SerializeField] string appearanceType = ""; // 外見パターン
		[UnityEngine.SerializeField] string motionType = ""; // 動作パターン
		[UnityEngine.SerializeField] string description = ""; // キャラクター説明
		[UnityEngine.SerializeField] long mRarityId = 0; // レアリティID
		[UnityEngine.SerializeField] long mCharaRarityChangeCategoryId = 0; // レアリティ変更設定カテゴリID
		[UnityEngine.SerializeField] long hp = 0; // HP
		[UnityEngine.SerializeField] long mp = 0; // MP
		[UnityEngine.SerializeField] long atk = 0; // 攻撃力
		[UnityEngine.SerializeField] long def = 0; // 防御力
		[UnityEngine.SerializeField] long spd = 0; // 素早さ
		[UnityEngine.SerializeField] long tec = 0; // 技巧
		[UnityEngine.SerializeField] long param1 = 0; // 追加パラメーター1（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param2 = 0; // 追加パラメーター2（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param3 = 0; // 追加パラメーター3（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param4 = 0; // 追加パラメーター4（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long param5 = 0; // 追加パラメーター5（タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long exParam1 = 0; // 特殊追加パラメーター1（レベル上昇等で変動しない。タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long exParam2 = 0; // 特殊追加パラメーター2（レベル上昇等で変動しない。タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long exParam3 = 0; // 特殊追加パラメーター3（レベル上昇等で変動しない。タイトルごとに意味合い変動）
		[UnityEngine.SerializeField] long maxLevel = 0; // レベル上限 最大値は999
		[UnityEngine.SerializeField] long charaType = 0; // キャラクター区分
		[UnityEngine.SerializeField] string cardType = ""; // カード区分（キャラクターカード・スペシャルサポートカード等の役割に相当。インゲームのルールによって、環境ごとに設定する）
		[UnityEngine.SerializeField] long mSizeId = 0; // サイズID
		[UnityEngine.SerializeField] long mEnhanceIdGrowth = 0; // 育成時の消費コスト定義（スキーマバージョン2でのみ使用）
		[UnityEngine.SerializeField] long mStatusAdditionIdGrowth = 0; // 育成時のステータス定義（スキーマバージョン2でのみ使用）
		[UnityEngine.SerializeField] long mEnhanceIdLiberation = 0; // 潜在開放時の消費コスト定義（スキーマバージョン2でのみ使用）
		[UnityEngine.SerializeField] long mStatusAdditionIdLiberation = 0; // 潜在開放時のステータス定義（スキーマバージョン2でのみ使用）
		[UnityEngine.SerializeField] long liberationExpRate = 0; // 潜在解放経験値レート（万分率）
		[UnityEngine.SerializeField] long priceFromPiece = 0; // ピース・キャラ変換する際に、かかるピース数を定義（-1を設定する場合は、ピース交換不可能）
		[UnityEngine.SerializeField] long mCharaTrainerLotteryId = 0; // スキル抽選設定。トレーナー機能以外やサブ抽選を行わない場合は0を指定する
		[UnityEngine.SerializeField] long voiceCharaNumber = 0; // ボイス番号
		[UnityEngine.SerializeField] long gachaEffectType = 0; // ガチャ演出種別
		[UnityEngine.SerializeField] long actionPattern = 0; // 自動で行動決定する時のアクション設定値
		[UnityEngine.SerializeField] string recommendCharaStartAt = ""; // 可変ユーザーキャラの検索テーブルの集計対象とする有効開始時刻
		[UnityEngine.SerializeField] long isExtraSupport = 0; // EXサポートカードか。1: EXサポート、2: 通常
		[UnityEngine.SerializeField] long deckConditionGroup = 0; // デッキ編成条件グループID
		[UnityEngine.SerializeField] long leaveWeaponPriority = 0; // おまかせ装備の優先度
		[UnityEngine.SerializeField] long imageEffectId = 0; // エフェクト画像ID
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaMasterObjectBase {
		public virtual CharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long parentMCharaId => _rawData._parentMCharaId;
		public virtual long liberationGroupMCharaId => _rawData._liberationGroupMCharaId;
		public virtual string standingImageMCharaId => _rawData._standingImageMCharaId;
		public virtual string name => _rawData._name;
		public virtual string rubyName => _rawData._rubyName;
		public virtual string shortName => _rawData._shortName;
		public virtual string nickname => _rawData._nickname;
		public virtual string appearanceType => _rawData._appearanceType;
		public virtual string motionType => _rawData._motionType;
		public virtual string description => _rawData._description;
		public virtual long mRarityId => _rawData._mRarityId;
		public virtual long mCharaRarityChangeCategoryId => _rawData._mCharaRarityChangeCategoryId;
		public virtual long hp => _rawData._hp;
		public virtual long mp => _rawData._mp;
		public virtual long atk => _rawData._atk;
		public virtual long def => _rawData._def;
		public virtual long spd => _rawData._spd;
		public virtual long tec => _rawData._tec;
		public virtual long param1 => _rawData._param1;
		public virtual long param2 => _rawData._param2;
		public virtual long param3 => _rawData._param3;
		public virtual long param4 => _rawData._param4;
		public virtual long param5 => _rawData._param5;
		public virtual long exParam1 => _rawData._exParam1;
		public virtual long exParam2 => _rawData._exParam2;
		public virtual long exParam3 => _rawData._exParam3;
		public virtual long maxLevel => _rawData._maxLevel;
		public virtual long charaType => _rawData._charaType;
		public virtual string cardType => _rawData._cardType;
		public virtual long mSizeId => _rawData._mSizeId;
		public virtual long mEnhanceIdGrowth => _rawData._mEnhanceIdGrowth;
		public virtual long mStatusAdditionIdGrowth => _rawData._mStatusAdditionIdGrowth;
		public virtual long mEnhanceIdLiberation => _rawData._mEnhanceIdLiberation;
		public virtual long mStatusAdditionIdLiberation => _rawData._mStatusAdditionIdLiberation;
		public virtual long liberationExpRate => _rawData._liberationExpRate;
		public virtual long priceFromPiece => _rawData._priceFromPiece;
		public virtual long mCharaTrainerLotteryId => _rawData._mCharaTrainerLotteryId;
		public virtual long voiceCharaNumber => _rawData._voiceCharaNumber;
		public virtual long gachaEffectType => _rawData._gachaEffectType;
		public virtual long actionPattern => _rawData._actionPattern;
		public virtual string recommendCharaStartAt => _rawData._recommendCharaStartAt;
		public virtual long isExtraSupport => _rawData._isExtraSupport;
		public virtual long deckConditionGroup => _rawData._deckConditionGroup;
		public virtual long leaveWeaponPriority => _rawData._leaveWeaponPriority;
		public virtual long imageEffectId => _rawData._imageEffectId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaMasterValueObject _rawData = null;
		public CharaMasterObjectBase( CharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaMasterObject : CharaMasterObjectBase, IMasterObject {
		public CharaMasterObject( CharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara;

        [UnityEngine.SerializeField]
        CharaMasterValueObject[] m_Chara = null;
    }


}
