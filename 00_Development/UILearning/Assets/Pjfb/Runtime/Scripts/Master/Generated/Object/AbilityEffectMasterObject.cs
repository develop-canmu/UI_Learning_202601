//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class AbilityEffectMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mElementId {get{ return mElementId;} set{ this.mElementId = value;}}
		[MessagePack.Key(2)]
		public long _targetType {get{ return targetType;} set{ this.targetType = value;}}
		[MessagePack.Key(3)]
		public long _targetNumber {get{ return targetNumber;} set{ this.targetNumber = value;}}
		[MessagePack.Key(4)]
		public long _effectType {get{ return effectType;} set{ this.effectType = value;}}
		[MessagePack.Key(5)]
		public string _subEffectTypeList {get{ return subEffectTypeList;} set{ this.subEffectTypeList = value;}}
		[MessagePack.Key(6)]
		public long _statusEffectType {get{ return statusEffectType;} set{ this.statusEffectType = value;}}
		[MessagePack.Key(7)]
		public long _mStatusEffectGroupId {get{ return mStatusEffectGroupId;} set{ this.mStatusEffectGroupId = value;}}
		[MessagePack.Key(8)]
		public long _invokeRateAll {get{ return invokeRateAll;} set{ this.invokeRateAll = value;}}
		[MessagePack.Key(9)]
		public long _invokeRate {get{ return invokeRate;} set{ this.invokeRate = value;}}
		[MessagePack.Key(10)]
		public long _criticalRate {get{ return criticalRate;} set{ this.criticalRate = value;}}
		[MessagePack.Key(11)]
		public long _powerRate {get{ return powerRate;} set{ this.powerRate = value;}}
		[MessagePack.Key(12)]
		public long _powerValue {get{ return powerValue;} set{ this.powerValue = value;}}
		[MessagePack.Key(13)]
		public long _turnCount {get{ return turnCount;} set{ this.turnCount = value;}}
		[MessagePack.Key(14)]
		public long _additionPowerRate {get{ return additionPowerRate;} set{ this.additionPowerRate = value;}}
		[MessagePack.Key(15)]
		public long _additionPowerValue {get{ return additionPowerValue;} set{ this.additionPowerValue = value;}}
		[MessagePack.Key(16)]
		public long _additionTurnCount {get{ return additionTurnCount;} set{ this.additionTurnCount = value;}}
		[MessagePack.Key(17)]
		public long _animationType {get{ return animationType;} set{ this.animationType = value;}}
		[MessagePack.Key(18)]
		public long _isOmitSameAnimation {get{ return isOmitSameAnimation;} set{ this.isOmitSameAnimation = value;}}
		[MessagePack.Key(19)]
		public long _selfAnimationType {get{ return selfAnimationType;} set{ this.selfAnimationType = value;}}
		[MessagePack.Key(20)]
		public long _isWholeAnimation {get{ return isWholeAnimation;} set{ this.isWholeAnimation = value;}}
		[MessagePack.Key(21)]
		public long _soundType {get{ return soundType;} set{ this.soundType = value;}}
		[MessagePack.Key(22)]
		public string _motionTypeList {get{ return motionTypeList;} set{ this.motionTypeList = value;}}
		[MessagePack.Key(23)]
		public string _receiveMotionTypeList {get{ return receiveMotionTypeList;} set{ this.receiveMotionTypeList = value;}}
		[MessagePack.Key(24)]
		public string _additionInvokeRate {get{ return additionInvokeRate;} set{ this.additionInvokeRate = value;}}
		[MessagePack.Key(25)]
		public long _turnDecrementTiming {get{ return turnDecrementTiming;} set{ this.turnDecrementTiming = value;}}
		[MessagePack.Key(26)]
		public string _invokeCondition {get{ return invokeCondition;} set{ this.invokeCondition = value;}}
		[MessagePack.Key(27)]
		public string _applyingCondition {get{ return applyingCondition;} set{ this.applyingCondition = value;}}
		[MessagePack.Key(28)]
		public long _targetStatusType {get{ return targetStatusType;} set{ this.targetStatusType = value;}}
		[MessagePack.Key(29)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mElementId = 0; // スキル属性
		[UnityEngine.SerializeField] long targetType = 0; // $targetType ターゲットタイプ
		[UnityEngine.SerializeField] long targetNumber = 0; // 対象人数（ランダムの場合のみ有効）
		[UnityEngine.SerializeField] long effectType = 0; // 対象区分
		[UnityEngine.SerializeField] string subEffectTypeList = ""; // サブ効果リスト
		[UnityEngine.SerializeField] long statusEffectType = 0; // 効果区分
		[UnityEngine.SerializeField] long mStatusEffectGroupId = 0; // 現状はステータス効果解除スキルのみで使用。解除したいm_status_effect_groupのidを指定(0=指定なし)
		[UnityEngine.SerializeField] long invokeRateAll = 0; // この効果自体の発動成功率
		[UnityEngine.SerializeField] long invokeRate = 0; // 発動率
		[UnityEngine.SerializeField] long criticalRate = 0; // クリティカル率
		[UnityEngine.SerializeField] long powerRate = 0; // 威力倍率
		[UnityEngine.SerializeField] long powerValue = 0; // 威力固定値
		[UnityEngine.SerializeField] long turnCount = 0; // 持続ターン数
		[UnityEngine.SerializeField] long additionPowerRate = 0; // スキルレベルによる追加威力倍率補正の基礎値
		[UnityEngine.SerializeField] long additionPowerValue = 0; // スキルレベルによる追加威力固定値補正の基礎値
		[UnityEngine.SerializeField] long additionTurnCount = 0; // スキルレベルによる追加持続ターン数補正の基礎値
		[UnityEngine.SerializeField] long animationType = 0; // スキルヒット時にヒット対象に表示するアニメーションのタイプ
		[UnityEngine.SerializeField] long isOmitSameAnimation = 0; // 同じanimationTypeが続く時に演出を省略するか否か
		[UnityEngine.SerializeField] long selfAnimationType = 0; // スキル発動時に発動者自身に表示するアニメーションのタイプ
		[UnityEngine.SerializeField] long isWholeAnimation = 0; // 全体攻撃のアニメーションを画面全体に出すか（animationTypeにのみ適用）
		[UnityEngine.SerializeField] long soundType = 0; // 音声区分
		[UnityEngine.SerializeField] string motionTypeList = ""; // スキル発動時に再生するモーション番号の一覧
		[UnityEngine.SerializeField] string receiveMotionTypeList = ""; // スキルを受けた側が再生するモーション番号の一覧
		[UnityEngine.SerializeField] string additionInvokeRate = ""; // レベルによる増加発動率
		[UnityEngine.SerializeField] long turnDecrementTiming = 0; // 残りターン数の減算タイミング
		[UnityEngine.SerializeField] string invokeCondition = ""; // 発動条件
		[UnityEngine.SerializeField] string applyingCondition = ""; // 申請条件
		[UnityEngine.SerializeField] long targetStatusType = 0; // 対象ステータス種別
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class AbilityEffectMasterObjectBase {
		public virtual AbilityEffectMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mElementId => _rawData._mElementId;
		public virtual long targetType => _rawData._targetType;
		public virtual long targetNumber => _rawData._targetNumber;
		public virtual long effectType => _rawData._effectType;
		public virtual string subEffectTypeList => _rawData._subEffectTypeList;
		public virtual long statusEffectType => _rawData._statusEffectType;
		public virtual long mStatusEffectGroupId => _rawData._mStatusEffectGroupId;
		public virtual long invokeRateAll => _rawData._invokeRateAll;
		public virtual long invokeRate => _rawData._invokeRate;
		public virtual long criticalRate => _rawData._criticalRate;
		public virtual long powerRate => _rawData._powerRate;
		public virtual long powerValue => _rawData._powerValue;
		public virtual long turnCount => _rawData._turnCount;
		public virtual long additionPowerRate => _rawData._additionPowerRate;
		public virtual long additionPowerValue => _rawData._additionPowerValue;
		public virtual long additionTurnCount => _rawData._additionTurnCount;
		public virtual long animationType => _rawData._animationType;
		public virtual long isOmitSameAnimation => _rawData._isOmitSameAnimation;
		public virtual long selfAnimationType => _rawData._selfAnimationType;
		public virtual long isWholeAnimation => _rawData._isWholeAnimation;
		public virtual long soundType => _rawData._soundType;
		public virtual string motionTypeList => _rawData._motionTypeList;
		public virtual string receiveMotionTypeList => _rawData._receiveMotionTypeList;
		public virtual string additionInvokeRate => _rawData._additionInvokeRate;
		public virtual long turnDecrementTiming => _rawData._turnDecrementTiming;
		public virtual string invokeCondition => _rawData._invokeCondition;
		public virtual string applyingCondition => _rawData._applyingCondition;
		public virtual long targetStatusType => _rawData._targetStatusType;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        AbilityEffectMasterValueObject _rawData = null;
		public AbilityEffectMasterObjectBase( AbilityEffectMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class AbilityEffectMasterObject : AbilityEffectMasterObjectBase, IMasterObject {
		public AbilityEffectMasterObject( AbilityEffectMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class AbilityEffectMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Ability_Effect;

        [UnityEngine.SerializeField]
        AbilityEffectMasterValueObject[] m_Ability_Effect = null;
    }


}
