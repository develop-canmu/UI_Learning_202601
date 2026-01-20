using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;

#if !(MAGIC_ONION_SERVER || PJFB_LAMBDA)
using Pjfb.Master;
#endif

namespace Pjfb.InGame
{
    public class BattleCharacterModel
    {
	    public class ActiveAbilityEffectData
	    {
		    public BattleAbilityEffectModel AbilityEffectModel;
		    public long level;
		    public long remainTurn;
		    public long activatedCharaId;
	    }

	    public class ActiveAbilityValueData
	    {
		    public float Addition = 0.0f;
		    public float Multiply = 0.0f;

		    public float GetMultiplyValue()
		    {
			    return Math.Max(1 + Multiply, 0.0f);
		    }
	    }

	    public long ParentMCharaId
	    {
		    get;
		    private set;
	    }
	    
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
	    public CharaMasterObject CharaMaster
	    {
		    get;
		    private set;
	    }
#endif
	    
	    public long id = 0; // UChara.id ではなく, レスポンス作成時に順に振られたただのインデックス
	    public long userId = 0; // User.id
	    public int playerIndex;
	    
	    // ベースステータス関連
	    public BigValue baseSpeed;
	    public BigValue baseTechnique;
	    public BigValue basePhysical;
	    public BigValue baseKick;
	    public BigValue baseWise;
	    public BigValue baseShootRange;
	    public BigValue baseStamina;
	    public BattleConst.PlayerPosition Position;
	    public float AbilityInvokeRateCoefficient = 1.0f;

	    public List<BattleAbilityModel> AbilityList = new List<BattleAbilityModel>();

	    // この形でいいのか…?
	    // mChara的なの参照することポコポコありそうだけど…
	    public long MCharaId;
	    public string Name;
	    public string Nickname;
	    public long Rank;
	    public BattleCharacterStatModel Stats;
		    
	    // インゲームステータス関連
	    public BigValue Speed;
	    public BigValue Physical;
	    public BigValue Technique;
	    public BigValue Kick;
	    public BigValue Wise;
	    public BigValue ShootRange;

	    public BigValue MaxStamina;
	    public BigValue CurrentStamina;
	    public BattleConst.StatusParamType PrimaryParam;
	    public BattleCharacterModel MarkCharacter;
	    public BattleCharacterModel ActionTargetCharacter;
	    public int MarkedCount; // DFのマーク済み対象数
	    public int ClearedNumOnRound; // あるラウンドでOF(パスをした)/DF(突破された)の順番
	    public int ScoredCount;
	    public BattleConst.TeamSide Side;
	    public bool IsAceCharacter;
	    public bool IsPlayerAceCharacter;
	    public BigValue CombatPower; // バフ等によって変動しない生の値.
	    
	    // アビリティ関連
	    public Dictionary<long, List<ActiveAbilityEffectData>> ActiveAbilityEffects = new Dictionary<long, List<ActiveAbilityEffectData>>(); // Dictionary<発動者.id, List<AbilityEffect>>
	    
	    // (AdditionValue, MultiplyValue) ((Base * (1 + MultiplyValue)) + AdditionValue) * StaminaCoef 
	    private ActiveAbilityValueData SpeedEffectValueData = new();
	    private ActiveAbilityValueData PhysicalEffectValueData = new();
	    private ActiveAbilityValueData TechniqueEffectValueData = new();
	    private ActiveAbilityValueData KickEffectValueData = new();
	    private ActiveAbilityValueData WiseEffectValueData = new();
	    private ActiveAbilityValueData ShootRangeEffectValueData = new();
	    private ActiveAbilityValueData StaminaEffectValueData = new();

	    public void SetId(int id)
	    {
		    this.id = id;
	    }
	    
	    public BattleCharacterModel(BattleV2Chara charaData, bool isAceCharacter)
	    {
		    id = charaData.charaIndex;
		    baseStamina = new BigValue(charaData.param1);
		    baseTechnique = new BigValue(charaData.tec);
		    baseSpeed = new BigValue(charaData.spd);
		    basePhysical = new BigValue(charaData.param2);
		    baseKick = new BigValue(charaData.param4);
		    baseWise = new BigValue(charaData.param5);
		    baseShootRange = new BigValue(charaData.exParam1);
		    IsAceCharacter = isAceCharacter;
		    MCharaId = charaData.mCharaId;
		    Name = charaData.name;
		    Nickname = charaData.nickname;
		    Rank = charaData.rank;
		    CurrentStamina = baseStamina;
		    MaxStamina = baseStamina;
		    Position = (BattleConst.PlayerPosition)charaData.roleNumber;
		    CombatPower = new BigValue(charaData.combatPower);
		    
		    ReCalculateParam();
		    
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
		    CharaMaster = MasterManager.Instance.charaMaster.FindData(MCharaId);
#endif
		    ParentMCharaId = charaData.parentMCharaId;
		    Stats = new BattleCharacterStatModel(id);
	    }

	    public string GetNameWithTeamSideColorCode()
	    {
		    if (Side == BattleDataMediator.Instance.PlayerSide)
		    {
			    return $"<color=#{BattleConst.AllyTeamStringColorCode}>{Name}</color>";
		    }
		    else
		    {
			    return $"<color=#{BattleConst.EnemyTeamStringColorCode}>{Name}</color>";
		    }
	    }

	    public void SetPlayerData(long _userId, int _playerIndex, BattleConst.TeamSide _side)
	    {
		    userId = _userId;
		    playerIndex = _playerIndex;
		    Side = _side;
		    IsPlayerAceCharacter = Side == BattleDataMediator.Instance.PlayerSide && IsAceCharacter;
	    }

	    public void SetAbilityData(BattleV2Ability[] allAbilities, WrapperIntList[] abilityIdAndLevelData)
	    {
		    foreach (var abilityIdAndLevel in abilityIdAndLevelData)
		    {
			    var abilityId = abilityIdAndLevel.l[0];
			    var level = abilityIdAndLevel.l[1];

			    BattleV2Ability targetAbilityData = null;
			    foreach (var abilityData in allAbilities)
			    {
				    if (abilityData.id == abilityId)
				    {
					    targetAbilityData = abilityData;
					    break;
				    }
			    }

			    if (targetAbilityData == null)
			    {
				    continue;
			    }

			    var abilityModel = new BattleAbilityModel();
			    abilityModel.SetData(targetAbilityData, level);
			    AbilityList.Add(abilityModel);
		    }
	    }

	    public void ReCalculateParam()
	    {
		    Speed = CalculateParamWithAbilityEffect(baseSpeed, SpeedEffectValueData);
		    Physical = CalculateParamWithAbilityEffect(basePhysical, PhysicalEffectValueData);
		    Technique = CalculateParamWithAbilityEffect(baseTechnique, TechniqueEffectValueData);
		    Kick = CalculateParamWithAbilityEffect(baseKick, KickEffectValueData);
		    Wise = CalculateParamWithAbilityEffect(baseWise, WiseEffectValueData);
		    ShootRange = CalculateParamWithAbilityEffect(baseShootRange, ShootRangeEffectValueData);
	    }

	    private BigValue CalculateParamWithAbilityEffect(BigValue baseValue, ActiveAbilityValueData abilityEffectValueData)
	    {
		    var ret = BigValue.CalculationCeiling(baseValue, abilityEffectValueData.GetMultiplyValue()) + abilityEffectValueData.Addition;
		    
		    // 計算上下限値を1とする.
		    if (ret <= 0)
		    {
			    ret.Value = 1;
		    }

		    return ret;
	    }

	    public float GetStaminaRate()
	    {
		    return (float)BigValue.RatioCalculation(CurrentStamina, MaxStamina);
	    }

	    private float GetCurrentStaminaCoefficientForParameter()
	    {
		    // 最大値の70%以上:100%		
		    // 最大値の50%以上:80%
		    // 最大値の30%以上:60%		
		    // 最大値の10%以上:50%		
		    // それ以下:40%	
		    var rateStamina = GetStaminaRate();
		    float rateParam = 1.0f;
		    switch (rateStamina)
		    {
			    case var r when r >= 0.7:
				    rateParam = 1.0f;
				    break;
			    case var r when r >= 0.5:
				    rateParam = 0.8f;
				    break;
			    case var r when r >= 0.3:
				    rateParam = 0.6f;
				    break;
			    case var r when r >= 0.1:
				    rateParam = 0.5f;
				    break;
			    default:
				    rateParam = 0.4f;
				    break;
		    }

		    return rateParam;
	    }

	    public BigValue GetCurrentParameter(BattleConst.StatusParamType statusParamType)
	    {
		    switch (statusParamType)
		    {
			    case BattleConst.StatusParamType.Speed:
				    return GetCurrentSpeed();
			    case BattleConst.StatusParamType.Physical:
				    return GetCurrentPhysical();
			    case BattleConst.StatusParamType.Technique:
				    return GetCurrentTechnique();
			    case BattleConst.StatusParamType.Kick:
				    return GetCurrentKick();
			    case BattleConst.StatusParamType.Wise:
				    return GetCurrentWise();
			    case BattleConst.StatusParamType.Stamina:
				    return CurrentStamina;
		    }
		    
		    return BigValue.Zero;
	    }
        
        public BigValue GetBaseParameter(BattleConst.StatusParamType statusParamType)
        {
            switch (statusParamType)
            {
                case BattleConst.StatusParamType.Speed:
                    return baseSpeed;
                case BattleConst.StatusParamType.Physical:
                    return basePhysical;
                case BattleConst.StatusParamType.Technique:
                    return baseTechnique;
                case BattleConst.StatusParamType.Kick:
                    return baseKick;
                case BattleConst.StatusParamType.Wise:
                    return baseWise;
                case BattleConst.StatusParamType.Stamina:
                    return baseStamina;
            }
		    
            return BigValue.Zero;
        }

	    public BattleConst.StatusParamType GetConsParam()
	    {
		    var currentSpeed = GetCurrentSpeed();
		    var currentTech = GetCurrentTechnique();
		    var currentPhysical = GetCurrentPhysical();

		    if (currentSpeed >= currentTech && currentSpeed >= currentPhysical)
		    {
			    return BattleConst.StatusParamType.Speed;
		    }

		    if (currentTech >= currentSpeed && currentTech >= currentPhysical)
		    {
			    return BattleConst.StatusParamType.Technique;
		    }

		    if (currentPhysical >= currentSpeed && currentPhysical >= currentTech)
		    {
			    return BattleConst.StatusParamType.Physical;
		    }

		    return BattleConst.StatusParamType.None;
	    }
	    
	    public BigValue GetCurrentSpeed()
	    {
		    // TODO ステータスは切り上げ統一でいいか?
		    return BigValue.CalculationCeiling(Speed, GetCurrentStaminaCoefficientForParameter());
	    }

	    public BigValue GetCurrentPhysical()
	    {
		    return BigValue.CalculationCeiling(Physical, GetCurrentStaminaCoefficientForParameter());
	    }
	    
	    public BigValue GetCurrentTechnique()
	    {
		    return BigValue.CalculationCeiling(Technique, GetCurrentStaminaCoefficientForParameter());
	    }

	    public BigValue GetCurrentKick()
	    {
		    return BigValue.CalculationCeiling(Kick, GetCurrentStaminaCoefficientForParameter());
	    }

	    public BigValue GetCurrentWise()
	    {
		    return BigValue.CalculationCeiling(Wise, GetCurrentStaminaCoefficientForParameter());
	    }

	    public BigValue GetCurrentShootRange()
	    {
		    // アビリティ関連がありそうなのでラップ
		    return ShootRange;
	    }

		/// <summary>
		/// 発動可能なアクティブアビリティの取得用. IF的にパッシブでも使えるけど使うケースがないので考慮してない.
		/// </summary>
		/// <param name="timingType"></param>
		/// <returns></returns>
	    public List<BattleAbilityModel> GetAvailableAbilities(BattleConst.AbilityEvaluateTimingType timingType)
		{
			var timing = (long) timingType;
			var ret = new List<BattleAbilityModel>();
			foreach(var ability in AbilityList)
			{
				if (ability.BattleAbilityMaster.timing != timing)
				{
					continue;
				}

				if (!ability.IsRemainActivateCount())
				{
					continue;
				}
				
				ret.Add(ability);
			}
			
			return ret;
		}
		
		public BattleAbilityModel GetAvailableActiveOFAbility()
		{
			foreach(var ability in AbilityList)
			{
				var timing = (BattleConst.AbilityEvaluateTimingType)ability.BattleAbilityMaster.timing;
				if (timing is
					    BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughOF or
						BattleConst.AbilityEvaluateTimingType.ActiveSelectPassOF or
						BattleConst.AbilityEvaluateTimingType.ActiveReceivePass or
						BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF or
						BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossOF or
						BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross)
				{
					if (ability.IsRemainActivateCount())
					{
						return ability;
					}
				}
			}
			
			return null;
		}
		
		public BattleAbilityModel GetAvailableActiveDFAbility()
		{
			foreach(var ability in AbilityList)
			{
				var timing = (BattleConst.AbilityEvaluateTimingType)ability.BattleAbilityMaster.timing;
				if (timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughDF ||
				    timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectPassDF ||
				    timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF/* ||
				    timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossDF*/)
				{
					if (ability.IsRemainActivateCount())
					{
						return ability;
					}
				}
			}
			
			return null;
		}
		
	    public void AddActiveAbilityData(BattleAbilityEffectModel abilityEffect, long level, long activatedCharaId)
	    {
		    if (!ActiveAbilityEffects.ContainsKey(activatedCharaId))
		    {
			    ActiveAbilityEffects.Add(activatedCharaId, new List<ActiveAbilityEffectData>());
		    }

		    // 発動済みのものがある場合はターンの更新のみ.
		    var activatedAbilityEffect = ActiveAbilityEffects[activatedCharaId].FirstOrDefault(effect => effect.AbilityEffectModel == abilityEffect);
		    if (activatedAbilityEffect != null)
		    {
			    activatedAbilityEffect.remainTurn = abilityEffect.GetTurnCount(level);
			    return;
		    }
		    
		    var abilityData = new ActiveAbilityEffectData();
		    abilityData.AbilityEffectModel = abilityEffect;
		    abilityData.level = level;
		    abilityData.activatedCharaId = activatedCharaId;
		    abilityData.remainTurn = abilityEffect.GetTurnCount(level);
		    
		    ActiveAbilityEffects[activatedCharaId].Add(abilityData);
		    if (BattleAbilityLogic.IsStatusEffect((BattleConst.AbilityEffectType)abilityEffect.AbilityEffectMaster.effectType))
		    {
			    OnStatusEffectChanged(abilityData, false);
		    }
	    }
	    
	    public void DecrementAbilityEffectTurn(BattleConst.AbilityTurnDecrementTiming timing)
	    {
		    var decrementTiming = (int)timing;
		    foreach (var activeAbilityEffects in ActiveAbilityEffects.Values)
		    {
			    for (var i = activeAbilityEffects.Count - 1; i >= 0 && activeAbilityEffects.Any(); i--)
			    {
				    if (activeAbilityEffects.Count == 0)
				    {
					    return;
				    }
				    
				    var activeAbilityEffect = activeAbilityEffects[i];
				    if (activeAbilityEffect.AbilityEffectModel.AbilityEffectMaster.turnDecrementTiming != decrementTiming)
				    {
					    continue;
				    }

				    // 持続制限なしだったらスルー
				    if (activeAbilityEffect.remainTurn <= -1)
				    {
					    continue;
				    }

				    activeAbilityEffect.remainTurn--;
				    if (activeAbilityEffect.remainTurn > 0)
				    {
					    continue;
				    }
				    
				    activeAbilityEffects.RemoveAt(i);
				    if (BattleAbilityLogic.IsStatusEffect((BattleConst.AbilityEffectType)activeAbilityEffect.AbilityEffectModel.AbilityEffectMaster.effectType))
				    {
					    OnStatusEffectChanged(activeAbilityEffect, true);
				    }
			    }
		    }
	    }

	    public void OnStatusEffectChanged(ActiveAbilityEffectData effectData, bool isRemoved)
	    {
		    var value = effectData.AbilityEffectModel.GetEffectValue(effectData.level);
		    if (isRemoved)
		    {
			    value *= -1;
		    }
		    
		    switch ((BattleConst.AbilityEffectType)effectData.AbilityEffectModel.AbilityEffectMaster.effectType)
		    {
                case BattleConst.AbilityEffectType.BuffSpeedUpAddition:
	                SpeedEffectValueData.Addition += value;
	                break;
                case BattleConst.AbilityEffectType.BuffTechniqueUpAddition:
	                TechniqueEffectValueData.Addition += value;
	                break;
                case BattleConst.AbilityEffectType.BuffPhysicalUpAddition:
	                PhysicalEffectValueData.Addition += value;
	                break;
                case BattleConst.AbilityEffectType.BuffWiseUpAddition:
	                WiseEffectValueData.Addition += value;
	                break;
                case BattleConst.AbilityEffectType.BuffKickUpAddition:
	                KickEffectValueData.Addition += value;
	                break;
                case BattleConst.AbilityEffectType.BuffShootRangeUpAddition:
	                ShootRangeEffectValueData.Addition += value;
	                break;
                case BattleConst.AbilityEffectType.BuffStaminaUpAddition:
	                StaminaEffectValueData.Addition += value;
	                OnStaminaEffectChanged(value, true, isRemoved);
	                break;
                case BattleConst.AbilityEffectType.BuffSpeedUpMultiply:
	                SpeedEffectValueData.Multiply += value;
	                break;
                case BattleConst.AbilityEffectType.BuffTechniqueUpMultiply:
	                TechniqueEffectValueData.Multiply += value;
	                break;
                case BattleConst.AbilityEffectType.BuffPhysicalUpMultiply:
	                PhysicalEffectValueData.Multiply += value;
	                break;
                case BattleConst.AbilityEffectType.BuffWiseUpMultiply:
	                WiseEffectValueData.Multiply += value;
	                break;
                case BattleConst.AbilityEffectType.BuffKickUpMultiply:
	                KickEffectValueData.Multiply += value;
	                break;
                case BattleConst.AbilityEffectType.BuffShootRangeUpMultiply:
	                ShootRangeEffectValueData.Multiply += value;
	                break;
                case BattleConst.AbilityEffectType.BuffStaminaUpMultiply:
	                StaminaEffectValueData.Multiply += value;
	                OnStaminaEffectChanged(value, false, isRemoved);
	                break;
		    }
		    
		    ReCalculateParam();
	    }

	    private void OnStaminaEffectChanged(float value, bool isAddition, bool isRemoved)
	    {
		    // 今回バフが追加され、その結果スタミナが増えるのなら最大値を超えない範囲で現在スタミナを加算
		    if (!isRemoved)
		    {
			    BigValue value2BigValue = value * BigValue.RateValue;
			    
			    BigValue bigValue = isAddition ? value2BigValue : new BigValue(baseStamina.Value * value2BigValue.Value);
			    var additionalValue = bigValue / BigValue.RateValue;
			    if (additionalValue > 0)
			    {
				    
				    CurrentStamina += additionalValue;
			    }
		    }

		    MaxStamina = (baseStamina * StaminaEffectValueData.GetMultiplyValue()) + StaminaEffectValueData.Addition;
		    CurrentStamina = BigValue.Clamp(CurrentStamina, BigValue.Zero, MaxStamina);
	    }

	    public bool IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType effectType)
	    {
		    return GetSpecificActiveAbilityEffectTypeActiveCount(effectType) > 0;
	    }

        /// <summary>アビリティ数の取得</summary>
        public int GetSpecificActiveAbilityEffectTypeActiveCount(BattleConst.AbilityEffectType effectType)
        {
            int abilityEffectCount = 0;
            var effectTypeId = (long)effectType;
            foreach (var activeAbilityEffects in ActiveAbilityEffects.Values)
            {
                foreach (var activeAbilityEffect in activeAbilityEffects)
                {
                    if (activeAbilityEffect.AbilityEffectModel.AbilityEffectMaster.effectType == effectTypeId)
                    {
                        abilityEffectCount++;
                    }
                }
            }

            return abilityEffectCount;
        }

        public float GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType effectType)
	    {
		    var effectTypeId = (long)effectType;
		    var ret = 0.0f;
		    foreach (var activeAbilityEffects in ActiveAbilityEffects.Values)
		    {
			    foreach (var activeAbilityEffect in activeAbilityEffects)
			    {
				    if (activeAbilityEffect.AbilityEffectModel.AbilityEffectMaster.effectType == effectTypeId)
				    {
					    ret += activeAbilityEffect.AbilityEffectModel.GetEffectValue(activeAbilityEffect.level);
				    }
			    }
		    }

		    return ret;
	    }

        /// <summary> スキルID から BattleAbilityModel を取得 </summary>
	    public BattleAbilityModel GetAbilityModelByAbilityId(long abilityId)
	    {
		    // 所持スキルの中から発動スキルを探す
		    foreach (var ability in AbilityList)
		    {
			    if (ability.BattleAbilityMaster.id == abilityId)
			    {
				    return ability;
			    }
		    }
		    
		    return null;
	    }
    }
}