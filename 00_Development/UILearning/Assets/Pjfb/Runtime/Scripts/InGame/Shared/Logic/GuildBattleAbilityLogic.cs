using System.Collections.Generic;
using System.Linq;
#if !PJFB_LAMBDA
using MagicOnion;
#endif
using Pjfb.InGame;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
#if !MAGIC_ONION_SERVER
//using Pjfb.MasterData;
#endif

#if !PJFB_LAMBDA
namespace Pjfb
{
    public class GuildBattleAbilityLogic
    {
        
        public static void GuildBattleAbilityEffects(GuildBattlePlayerData battlePlayerData)
        {
            foreach (GuildBattleAbilityData abilityData in battlePlayerData.GuildBattleActivatedAbilityList.Where(t => t.RemainTurn > 0))
            {
                BattleAbilityModel battleAbilityModel = new BattleAbilityModel();
                BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(a => 
                    a.id == abilityData.AbilityId);
                if(battleV2Ability == null)
                {
                    continue;
                }
                
                battleAbilityModel.SetData(
                    battleV2Ability,
                    abilityData.AbilityLevel);
                GuildBattleAbilityEffect(battlePlayerData, abilityData, battleAbilityModel);
            }
        }

        public static void GuildBattleAbilityEffect(GuildBattlePlayerData battlePlayerData , GuildBattleAbilityData abilityData,BattleAbilityModel battleAbilityModel)
        {
            foreach (BattleAbilityEffectModel abilityEffectModel in battleAbilityModel.BattleAbilityEffectModels)
            {
                var effectType = (BattleConst.AbilityEffectType)abilityEffectModel.AbilityEffectMaster.effectType;

                if (GuildBattleAbilityLogic.IsGuildBattleAbilityEffect(effectType))
                {
                    // ギルドバトル用の効果はここで処理
                    GuildBattleAbilityLogic.ApplyGuildAbilityEffect(battlePlayerData, abilityData, abilityEffectModel);
                }
            }
        }
        
        // アドバイザースキルで即効果が発動するもの
        private static bool IsGuildBattleAbilityEffect(BattleConst.AbilityEffectType effectType)
        {
            // キャラクター関連のパラメータ
            switch (effectType)
            {
                case BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrement:
                case BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrementMultiply:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnAddition:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnMultiply:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryAdditionOverHeal:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryMultiplyOverHeal:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryAdditionUntilMaxHeal:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryMultiplyUntilMaxHeal:
                    return true;
            }

            return false;
        }

        // ギルドバトルのボール回復効果で、最大まで回復するもの
        public static bool IsGuildBattleAbilityBallRecoveryEffectUntilMaxHeal(BattleAbilityModel battleAbilityModel)
        {
            foreach (BattleAbilityEffectModel effectModel in battleAbilityModel.BattleAbilityEffectModels)
            {
                if (IsGuildBattleAbilityBallRecoveryEffectUntilMaxHeal((BattleConst.AbilityEffectType)effectModel.AbilityEffectMaster.effectType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsGuildBattleAbilityBallRecoveryEffectUntilMaxHeal(BattleConst.AbilityEffectType effectType)
        {
            // ギルドバトルのボール回復効果で、最大まで回復するもの
            return effectType == BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryAdditionUntilMaxHeal ||
                   effectType == BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryMultiplyUntilMaxHeal;
        }
        
        private static void ApplyGuildAbilityEffect(GuildBattlePlayerData battlePlayerData ,GuildBattleAbilityData guildBattleAbilityData, BattleAbilityEffectModel abilityEffect)
        {
            BattleConst.AbilityEffectType effectType = (BattleConst.AbilityEffectType)abilityEffect.AbilityEffectMaster.effectType;
            int addMilitaryStrength = 0;
            bool isOverHeal = false;

            switch (effectType)
            {
                case BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrement:
                    // クールタイムの
                    PjfbGuildBattleDataMediator.Instance
                        .GetBattlePartiesFromPlayerIndex((int)battlePlayerData.PlayerIndex)
                        .Where(unit => unit.RevivalTurn > 0)
                        .ToList()
                        .ForEach(unit => unit.RevivalTurn = System.Math.Max(0, unit.RevivalTurn - (int)abilityEffect.GetEffectValue(guildBattleAbilityData.AbilityLevel)));

                    break;
                case BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrementMultiply:
                    PjfbGuildBattleDataMediator.Instance
                        .GetBattlePartiesFromPlayerIndex((int)battlePlayerData.PlayerIndex)
                        .Where(unit => unit.RevivalTurn > 0)
                        .ToList()
                        .ForEach(unit => unit.RevivalTurn = System.Math.Max(0, unit.RevivalTurn - (int)(unit.RevivalTurn * abilityEffect.GetEffectValue(guildBattleAbilityData.AbilityLevel))));
                    break;

                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnAddition:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnMultiply:
                    break;
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryAdditionOverHeal:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryAdditionUntilMaxHeal:
                    isOverHeal = BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryAdditionOverHeal == effectType;
                    addMilitaryStrength = (int)abilityEffect.GetEffectValue(guildBattleAbilityData.AbilityLevel);
                    break;
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryMultiplyOverHeal:
                case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryMultiplyUntilMaxHeal:
                    isOverHeal = BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryMultiplyOverHeal == effectType;
                    addMilitaryStrength = (int)(battlePlayerData.MaxMilitaryStrength * abilityEffect.GetEffectValue(guildBattleAbilityData.AbilityLevel));
                    break;
            }
            if( addMilitaryStrength > 0)
            {
                battlePlayerData.AddAvailableMilitaryStrength(addMilitaryStrength, isOverHeal);
            }
#if MAGIC_ONION_SERVER_DEBUG
            Console.WriteLine($"playerIndex: {battlePlayerData.PlayerIndex}, effectType: {effectType}, addMilitaryStrength: {addMilitaryStrength}, isOverHeal: {isOverHeal}");
#endif
        }
        
        public static bool IsActivateAdviserAbility(GuildBattlePlayerData playerData, BattleAbilityModel abilityModel)
        {
            // ゲーム中でない場合は有効なスキルはない！
            if (PjfbGuildBattleDataMediator.Instance.GameState != GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                return false;
            }
            
            // アビリティモデルがnullの場合は無効
            // タイミングが追加されたため、コンディションは必須ではない
            if (abilityModel == null)
            {
                return false;
            }
            
            // 条件は必須で、すべての条件が一致する必要がある
            foreach (List<BattleAbilityActivationCondition> conditionList in abilityModel.ActivationConditions)
            {
                if (!EvaluateAbilityActivationCondition(playerData, conditionList))
                {
                    return false;
                }
            }
            
            // 発動する場合は、発動率を確認
            // リアルタイムサーバーでは、NonState版を使う
            if (!BattleGameLogic.LotteryResultByRateNonState(abilityModel.GetInvokeRate(abilityModel.AbilityLevel)))
            {
                return false;
            }

            return true;
        }
        
        private static bool EvaluateAbilityActivationCondition( GuildBattlePlayerData playerData, List<BattleAbilityActivationCondition> conditionList)
        {
            // 条件がない場合は無効
            if (conditionList.Count == 0)
            {
                return false;
            }

            // 条件を満たすかどうかを確認
            foreach (BattleAbilityActivationCondition condition in conditionList)
            {
                switch (condition.ConditionType)
                {
                    case BattleConst.AbilityActivationConditionType.ClubRoyalBallCount:
                        if (condition.IsMeetMinMaxValue(new BigValue(playerData.AvailableMilitaryStrength)))
                        {
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }

            return false;
        }

        public static bool IsCoolTimeAbilityActivating(GuildBattlePlayerData battlePlayerData)
        {
            // ゲーム中でない場合は有効なスキルはない！
            if (PjfbGuildBattleDataMediator.Instance.GameState != GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                return false;
            }
            
            bool hasCoolTimeDecrementAbility = false;
            foreach (GuildBattleAbilityData ability in battlePlayerData.GuildBattleActivatedAbilityList)
            {
                if (ability.RemainTurn <= 0)
                {
                    continue;
                }

                BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == ability.AbilityId);
                if (battleV2Ability == null)
                {
                    continue;
                }

                if (!IsAdviserAbilityType((BattleConst.AbilityType)battleV2Ability.abilityType))
                {
                    continue;
                }

                foreach (BattleV2AbilityEffect effect in battleV2Ability.abilityEffectList)
                {
                    if (effect.effectType == (long)BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrement ||
                        effect.effectType == (long)BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrementMultiply)
                    {
                        hasCoolTimeDecrementAbility = true;
                        break;
                    }
                }

                if (hasCoolTimeDecrementAbility)
                {
                    break;
                }
            }
            return hasCoolTimeDecrementAbility;
        }
        
        public static bool IsAdviserAbilityType(BattleConst.AbilityType abilityType)
        {
            return abilityType == BattleConst.AbilityType.GuildBattleManual ||
                   abilityType == BattleConst.AbilityType.GuildBattleAuto;
        }

        // エールスキルかどうかの判定メソッド
        public static bool IsYellAbility(long abilityId)
        {
            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == abilityId);

            // 手動で発動するアビリティは、エールスキル
            if (battleV2Ability?.abilityType == (long)BattleConst.AbilityType.GuildBattleManual )
            {
                return true;
            }
            return false;
        }
        
        // 効果中のサポートスキルを取得するメソッド
        public static List<GuildBattleAbilityData> GetActiveSupportAbilityList(List<GuildBattleAbilityData> allAbilityList)
        {
            return allAbilityList
                .Where(ability => 
                    PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == ability.AbilityId)?.abilityType == (long)BattleConst.AbilityType.GuildBattleAuto &&
                    ability.RemainTurn > 0)
                .ToList();
        }

        public static bool IsPartyTargetedByAbilityEffect(GuildBattlePartyModel party, BattleV2AbilityEffect abilityEffect)
        {
            switch ((BattleConst.AbilityTargetType)abilityEffect.targetType)
            {
                case BattleConst.AbilityTargetType.GuildBattleSelfSortieUnits:
                    return party.IsOnMap();
                case BattleConst.AbilityTargetType.GuildBattleSelfCoolTimeUnits:
                    return party.GetRevivalTurnCount() > 0;
            }

            return false;
        }

        public static bool IsActivatingAdviserAbility(GuildBattleAbilityData ability)
        {
            // ゲーム中でない場合は有効なスキルはない！
            if (PjfbGuildBattleDataMediator.Instance.GameState != GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                return false;
            }
            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == ability.AbilityId);
            if (battleV2Ability == null)
            {
                return false;
            }

            // 効果ターン中ではない場合は発動していない
            if (ability.RemainTurn <= 0)
            {
                return false;
            }

            // アドバイザースキルのタイプかどうかの確認
            if (!IsAdviserAbilityType((BattleConst.AbilityType)battleV2Ability.abilityType))
            {
                return false;
            }
            return true;
        }

        public static bool IsBallRecoveryUpPerTurnAbility(GuildBattleAbilityData ability)
        {
            // ゲーム中でない場合は有効なスキルはない！
            if (PjfbGuildBattleDataMediator.Instance.GameState != GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                return false;
            }

            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == ability.AbilityId);
            if (battleV2Ability == null)
            {
                return false;
            }
            
            return battleV2Ability.abilityEffectList.Any(e=>
                e.effectType == (long)BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnAddition || 
                e.effectType == (long)BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnMultiply);
        }
        
        public static int GetRecoveryBallPerTurn(int baseRecovery , GuildBattleAbilityData abilityData)
        {
            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault( x => x.id == abilityData.AbilityId);
            if (battleV2Ability == null)
            {
                return 0;
            }
            
            var level = abilityData.AbilityLevel;
            
            BattleAbilityModel activateAbility = new BattleAbilityModel();
            activateAbility.SetData(battleV2Ability, level);

            int add = 0;
            foreach (BattleAbilityEffectModel effect in activateAbility.BattleAbilityEffectModels)
            {
                switch ((BattleConst.AbilityEffectType)effect.AbilityEffectMaster.effectType)
                {
                    case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnAddition:
                        add += (int)effect.GetEffectValue(level);
                        break;
                    case BattleConst.AbilityEffectType.GuildBattleMilitaryStrengthRecoveryPerTurnMultiply:
                        add += (int)(baseRecovery * effect.GetEffectValue(level));
                        break;
                }
            }

            return add;
        }

        public static bool IsActivatingBallRecoveryUpPerTurn(GuildBattlePlayerData battlePlayerData)
        {
            if (PjfbGuildBattleDataMediator.Instance.GameState != GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                return false;
            }
            
            return battlePlayerData.GuildBattleActivatedAbilityList
                .Where(s => s.RemainTurn > 0)
                .Any(GuildBattleAbilityLogic.IsBallRecoveryUpPerTurnAbility);
        }

        public static bool IsDirtyGuildBattleAbilityDataLists(List<GuildBattleAbilityData> oldList, List<GuildBattleAbilityData> newList)
        {
            // nullチェック
            if (oldList == null || newList == null)
            {
                return true;
            }

            // サイズが異なる場合は変更あり
            if (oldList.Count != newList.Count)
            {
                return true;
            }

            // 各要素を比較
            foreach (GuildBattleAbilityData ability in oldList)
            {
                if(!newList.Any(ability.IsSameData))
                {
                    // 一つでも一致しない要素があれば変更あり
                    return true;
                }
            }

            return false;
        }

        public static List<BattleV2AbilityEffect> GetActivatedAbilityEffectList(GuildBattlePartyModel partyModel, GuildBattlePlayerData battlePlayerData)
        {
            List<BattleV2AbilityEffect> activatedAbilityEffectList = new List<BattleV2AbilityEffect>();

            foreach (var activationAbility in battlePlayerData.GuildBattleActivatedAbilityList)
            {
                // 起動中でないアビリティはスキップ
                if (!GuildBattleAbilityLogic.IsActivatingAdviserAbility(activationAbility))
                {
                    continue;
                }

                // マスターデータを取得
                var abilityMasterData = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == activationAbility.AbilityId);

                if (abilityMasterData == null)
                {
                    continue;
                }

                // マスターデータからアビリティ効果を取得
                foreach (BattleV2AbilityEffect abilityEffect in abilityMasterData.abilityEffectList)
                {
                    if (IsPartyTargetedByAbilityEffect(partyModel, abilityEffect))
                    {
                        activatedAbilityEffectList.Add(abilityEffect);
                    }
                }
            }

            return activatedAbilityEffectList;
        }
    }    
}
#endif