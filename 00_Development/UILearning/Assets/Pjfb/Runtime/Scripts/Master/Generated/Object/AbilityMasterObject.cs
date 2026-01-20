//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class AbilityMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(3)]
		public string _useMessage {get{ return useMessage;} set{ this.useMessage = value;}}
		[MessagePack.Key(4)]
		public long _mainTargetType {get{ return mainTargetType;} set{ this.mainTargetType = value;}}
		[MessagePack.Key(5)]
		public long _mainEffectType {get{ return mainEffectType;} set{ this.mainEffectType = value;}}
		[MessagePack.Key(6)]
		public long _mainMElementId {get{ return mainMElementId;} set{ this.mainMElementId = value;}}
		[MessagePack.Key(7)]
		public long _costType {get{ return costType;} set{ this.costType = value;}}
		[MessagePack.Key(8)]
		public long _costValue {get{ return costValue;} set{ this.costValue = value;}}
		[MessagePack.Key(9)]
		public long _rarity {get{ return rarity;} set{ this.rarity = value;}}
		[MessagePack.Key(10)]
		public long _maxLevel {get{ return maxLevel;} set{ this.maxLevel = value;}}
		[MessagePack.Key(11)]
		public long _combatPower {get{ return combatPower;} set{ this.combatPower = value;}}
		[MessagePack.Key(12)]
		public long _combatPowerAddValue {get{ return combatPowerAddValue;} set{ this.combatPowerAddValue = value;}}
		[MessagePack.Key(13)]
		public long _timing {get{ return timing;} set{ this.timing = value;}}
		[MessagePack.Key(14)]
		public string _invokeCondition {get{ return invokeCondition;} set{ this.invokeCondition = value;}}
		[MessagePack.Key(15)]
		public string _invokeRate {get{ return invokeRate;} set{ this.invokeRate = value;}}
		[MessagePack.Key(16)]
		public long _maxInvokeCount {get{ return maxInvokeCount;} set{ this.maxInvokeCount = value;}}
		[MessagePack.Key(17)]
		public long _coolDownTurnCount {get{ return coolDownTurnCount;} set{ this.coolDownTurnCount = value;}}
		[MessagePack.Key(18)]
		public long _abilityType {get{ return abilityType;} set{ this.abilityType = value;}}
		[MessagePack.Key(19)]
		public long _cutInType {get{ return cutInType;} set{ this.cutInType = value;}}
		[MessagePack.Key(20)]
		public long _cutInImageId {get{ return cutInImageId;} set{ this.cutInImageId = value;}}
		[MessagePack.Key(21)]
		public long _iconId {get{ return iconId;} set{ this.iconId = value;}}
		[MessagePack.Key(22)]
		public long _soundType {get{ return soundType;} set{ this.soundType = value;}}
		[MessagePack.Key(23)]
		public string _additionInvokeRate {get{ return additionInvokeRate;} set{ this.additionInvokeRate = value;}}
		[MessagePack.Key(24)]
		public long _abilityCategory {get{ return abilityCategory;} set{ this.abilityCategory = value;}}
		[MessagePack.Key(25)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザー側で使用する名称
		[UnityEngine.SerializeField] string description = ""; // 説明
		[UnityEngine.SerializeField] string useMessage = ""; // 使用時のメッセージ
		[UnityEngine.SerializeField] long mainTargetType = 0; // メインとなる対象種別
		[UnityEngine.SerializeField] long mainEffectType = 0; // メインとなる効果種別
		[UnityEngine.SerializeField] long mainMElementId = 0; // メインとなる属性ID
		[UnityEngine.SerializeField] long costType = 0; // 消費ポイント区分（1: SP, 2: HP, 3: SP割合, 4: HP割合, 5: CP）
		[UnityEngine.SerializeField] long costValue = 0; // ポイント消費量
		[UnityEngine.SerializeField] long rarity = 0; // レア度
		[UnityEngine.SerializeField] long maxLevel = 0; // 最大スキルレベル
		[UnityEngine.SerializeField] long combatPower = 0; // このスキルを所持していた場合に可変キャラに加算される総合力
		[UnityEngine.SerializeField] long combatPowerAddValue = 0; // スキルレベルが1上がるごとの combatPower の上昇値
		[UnityEngine.SerializeField] long timing = 0; // 発動タイミング
		[UnityEngine.SerializeField] string invokeCondition = ""; // 発動条件
		[UnityEngine.SerializeField] string invokeRate = ""; // 発動率（0~1で指定）
		[UnityEngine.SerializeField] long maxInvokeCount = 0; // 最大発動回数
		[UnityEngine.SerializeField] long coolDownTurnCount = 0; // 再使用までに必要なターン数。フルネイティブタイトルのクライアント側でのみ使用
		[UnityEngine.SerializeField] long abilityType = 0; // インゲームにおけるどのようなシーンで発動するかを識別する値。フルネイティブタイトルのクライアント側でのみ使用する想定
		[UnityEngine.SerializeField] long cutInType = 0; // カットインアニメーション区分
		[UnityEngine.SerializeField] long cutInImageId = 0; // カットインアニメーション用画像のid
		[UnityEngine.SerializeField] long iconId = 0; // アイコンID
		[UnityEngine.SerializeField] long soundType = 0; // 音声区分
		[UnityEngine.SerializeField] string additionInvokeRate = ""; // レベルによる増加発動率
		[UnityEngine.SerializeField] long abilityCategory = 0; // クライアント側で使用。1=>通常、2=>特殊(PJFBの場合はFLOW)
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class AbilityMasterObjectBase {
		public virtual AbilityMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string description => _rawData._description;
		public virtual string useMessage => _rawData._useMessage;
		public virtual long mainTargetType => _rawData._mainTargetType;
		public virtual long mainEffectType => _rawData._mainEffectType;
		public virtual long mainMElementId => _rawData._mainMElementId;
		public virtual long costType => _rawData._costType;
		public virtual long costValue => _rawData._costValue;
		public virtual long rarity => _rawData._rarity;
		public virtual long maxLevel => _rawData._maxLevel;
		public virtual long combatPower => _rawData._combatPower;
		public virtual long combatPowerAddValue => _rawData._combatPowerAddValue;
		public virtual long timing => _rawData._timing;
		public virtual string invokeCondition => _rawData._invokeCondition;
		public virtual string invokeRate => _rawData._invokeRate;
		public virtual long maxInvokeCount => _rawData._maxInvokeCount;
		public virtual long coolDownTurnCount => _rawData._coolDownTurnCount;
		public virtual long abilityType => _rawData._abilityType;
		public virtual long cutInType => _rawData._cutInType;
		public virtual long cutInImageId => _rawData._cutInImageId;
		public virtual long iconId => _rawData._iconId;
		public virtual long soundType => _rawData._soundType;
		public virtual string additionInvokeRate => _rawData._additionInvokeRate;
		public virtual long abilityCategory => _rawData._abilityCategory;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        AbilityMasterValueObject _rawData = null;
		public AbilityMasterObjectBase( AbilityMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class AbilityMasterObject : AbilityMasterObjectBase, IMasterObject {
		public AbilityMasterObject( AbilityMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class AbilityMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Ability;

        [UnityEngine.SerializeField]
        AbilityMasterValueObject[] m_Ability = null;
    }


}
