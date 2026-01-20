using System;
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.InGame
{
    public class BattleAbilityLogic
    {
        public static void EvaluateCharactersAbility(BattleConst.AbilityEvaluateTimingType timingType, List<BattleCharacterModel> characters, List<Tuple<BattleCharacterModel, BattleAbilityModel>> beforeLogList = null, List<Tuple<BattleCharacterModel, BattleAbilityModel>> afterLogList = null)
        {
            foreach (var character in characters)
            {
                EvaluateCharacterAbility(timingType, character, beforeLogList, afterLogList);
            }
        }

        public static bool EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType timingType, BattleCharacterModel character, List<Tuple<BattleCharacterModel, BattleAbilityModel>> beforeLogList = null, List<Tuple<BattleCharacterModel, BattleAbilityModel>> afterLogList = null, bool forceExtendAbilityLevel = false)
        {
            var isActivated = false;
            var timing = (int)timingType;
            foreach (var battleAbility in character.AbilityList)
            {
                if (battleAbility.BattleAbilityMaster.timing != timing)
                {
                    continue;
                }

                isActivated = EvaluateAbility(character, battleAbility, beforeLogList, afterLogList, extendAbilityEffectLevel: forceExtendAbilityLevel);
            }

            return isActivated;
        }

        public static bool EvaluateAbilityTiming(BattleAbilityModel ability, BattleConst.MatchUpActionType actionType, bool isTargetCharacter)
        {
            var timing = (BattleConst.AbilityEvaluateTimingType)ability.BattleAbilityMaster.timing;
            switch (actionType)
            {
                case BattleConst.MatchUpActionType.Through:
                    if (isTargetCharacter)
                    {
                        return false;
                    }

                    return timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughOF;
                
                case BattleConst.MatchUpActionType.Pass:
                    if (isTargetCharacter)
                    {
                        return false;
                    }

                    return timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectPassOF;
                case BattleConst.MatchUpActionType.Shoot:
                    if (isTargetCharacter)
                    {
                        return false;
                    }

                    return timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF;
                case BattleConst.MatchUpActionType.Cross:
                    if (isTargetCharacter)
                    {
                        return timing == BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross;
                    }

                    return timing == BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossOF;
            }

            return false;
        }

        public static bool EvaluateAbilityCondition(BattleCharacterModel character, BattleAbilityModel ability)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceActivateAbilityIds.Contains(ability.BattleAbilityMaster.id))
            {
                return true;
            }

            if (BattleDataMediator.Instance.ForceActivateAbilityIds.Any() && BattleDataMediator.Instance.ForceDontActivateAbilities)
            {
                return false;
            }
#endif
            if (!ability.IsRemainActivateCount())
            {
                return false;
            }

            foreach (var conditions in ability.ActivationConditions)
            {
                EvaluateAbilityActivationCondition(character, conditions);
            }

            return ability.IsMeetAllCondition();
        }
        
        public static bool EvaluateAbilityEffectCondition(BattleCharacterModel character, BattleAbilityEffectModel abilityEffect, long abilityId)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceActivateAbilityIds.Contains(abilityId))
            {
                return true;
            }

            if (BattleDataMediator.Instance.ForceActivateAbilityIds.Any() && BattleDataMediator.Instance.ForceDontActivateAbilities)
            {
                return false;
            }
#endif
            foreach (var conditions in abilityEffect.ActivationConditions)
            {
                EvaluateAbilityActivationCondition(character, conditions);
            }

            return abilityEffect.IsMeetAllCondition();
        }

        public static bool EvaluateAbility(BattleCharacterModel character, BattleAbilityModel ability, List<Tuple<BattleCharacterModel, BattleAbilityModel>> beforeLogList = null, List<Tuple<BattleCharacterModel, BattleAbilityModel>> afterLogList = null, bool forceInvoke = false, bool extendAbilityEffectLevel = false)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceActivateAbilityIds.Contains(ability.BattleAbilityMaster.id))
            {
                return true;
            }

            if (BattleDataMediator.Instance.ForceActivateAbilityIds.Any() && BattleDataMediator.Instance.ForceDontActivateAbilities)
            {
                return false;
            }
#endif
            if (!EvaluateAbilityCondition(character, ability))
            {
                return false;
            }

            var isActiveAbility = IsActiveAbility((BattleConst.AbilityEvaluateTimingType)ability.BattleAbilityMaster.timing);
            var isActivated = false;
            if (ability.IsMeetAllCondition())
            {
                if (!isActiveAbility)
                {
                    // パッシブは賢さによる発動率計算をいれる.
                    // あくまでもパッシブ自体の発動なので, Effectのほうには関係ない.
                    var invokeRateCoefByWise = character.AbilityInvokeRateCoefficient;
#if !PJFB_REL && (UNITY_IOS || UNITY_ANDROID)
                    if (BattleDataMediator.Instance.ShowPassiveInvokeRate)
                    {
                        CruFramework.Logger.Log($"Chara: {character.Name}, Ability: {ability.BattleAbilityMaster.name}, InvokeRate: {ability.GetInvokeRate(ability.AbilityLevel)}, Coefficient: {invokeRateCoefByWise}");
                    }

                    if (BattleDataMediator.Instance.ForceSuccessPassiveInvokeByRate)
                    {
                        // まあこんぐらいしとけば絶対成功するやろ. 1%未満の発動率はせんやろし.
                        invokeRateCoefByWise = 1000;
                    }
#endif
                    if (forceInvoke || !BattleGameLogic.LotteryResultByRate(ability.GetInvokeRate(ability.AbilityLevel) * invokeRateCoefByWise))
                    {
                        return false;
                    }
                }

                // アビリティの発動に成功した時点でエフェクトの発動如何に関わらず発動成功と定義
                isActivated = true;
                
                foreach (var abilityEffect in ability.BattleAbilityEffectModels)
                {
                    // デバッグ用だからあんまり気にしないで. そのうちこういう効果実装しそうだけど.
                    var additionalInvokeRate = 0.0f;
#if !PJFB_REL && (UNITY_IOS || UNITY_ANDROID)
                    if (BattleDataMediator.Instance.ForceSuccessPassiveEffectInvoke)
                    {
                        additionalInvokeRate = 1;
                    }
#endif
                    EvaluateAbilityEffectCondition(character, abilityEffect, ability.BattleAbilityMaster.id);
                    if (!abilityEffect.IsMeetAllCondition())
                    {
                        continue;
                    }
                    
                    if (!BattleGameLogic.LotteryResultByRate(abilityEffect.GetInvokeRate(ability.AbilityLevel) + additionalInvokeRate))
                    {
                        continue;
                    }

                    var targets = GetAbilityEffectTarget(abilityEffect, character);
                    foreach (var target in targets)
                    {
                        var level = ability.AbilityLevel;
                        if (extendAbilityEffectLevel)
                        {
                            level += 1;
                        }
                        
                        ApplyAbilityEffect(abilityEffect, level, character, target);
                    }
                }

                if (isActivated)
                {
                    ability.AddActivateCount();
                    if (IsAddLogBeforeMatchUp((BattleConst.AbilityEvaluateTimingType)ability.BattleAbilityMaster.timing))
                    {
                        beforeLogList?.Add(new Tuple<BattleCharacterModel, BattleAbilityModel>(character, ability));
                    }
                    else
                    {
                        afterLogList?.Add(new Tuple<BattleCharacterModel, BattleAbilityModel>(character, ability));
                    }
                }
            }

            return isActivated;
        }

        public static void EvaluateAbilityActivationCondition(BattleCharacterModel character, List<BattleAbilityActivationCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                condition.IsMeetCondition = false;
                switch (condition.ConditionType)
                {
                    case BattleConst.AbilityActivationConditionType.ScoreDiff:
                        condition.IsMeetCondition = EvaluateScoreDiffActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.AllyScore:
                        condition.IsMeetCondition = EvaluateAllyScoreActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.EnemyScore:
                        condition.IsMeetCondition = EvaluateEnemyScoreActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.SelfScoreCount:
                        condition.IsMeetCondition = EvaluateSelfScoreCountActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.SelfPosition:
                        condition.IsMeetCondition = EvaluateSelfPositionActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.StatusRankAtTeam:
                        condition.IsMeetCondition = EvaluateStatusRankAtTeamActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.StatusRankAtRound:
                        condition.IsMeetCondition = EvaluateStatusRankAtRoundTeamActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.StatusRankAtWholeTeam:
                        condition.IsMeetCondition = EvaluateStatusRankAtTeamActivationCondition(character, condition, true);
                        break;
                    case BattleConst.AbilityActivationConditionType.StatusRankAtRoundWholeTeam:
                        condition.IsMeetCondition = EvaluateStatusRankAtRoundTeamActivationCondition(character, condition, true);
                        break;
                    case BattleConst.AbilityActivationConditionType.BallPosition:
                        condition.IsMeetCondition = EvaluateBallPositionActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.CurrentStamina:
                        condition.IsMeetCondition = EvaluateCurrentStaminaActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.AllySpecificCharacterAtTeam:
                        condition.IsMeetCondition = EvaluateAllySpecificCharacterAtTeamActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.EnemySpecificCharacterAtTeam:
                        condition.IsMeetCondition = EvaluateEnemySpecificCharacterAtTeamActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.MatchUpSpecificCharacter:
                        condition.IsMeetCondition = EvaluateSpecificCharacterMatchUpActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.MatchUpPrimaryStatusDifference:
                        condition.IsMeetCondition = EvaluatePrimaryStatusDifferenceActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.AllySpecificCharacterBallOwner:
                        condition.IsMeetCondition = EvaluateAllySpecificBallOwner(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.EnemySpecificCharacterBallOwner:
                        condition.IsMeetCondition = EvaluateEnemySpecificBallOwner(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.BaseStatusValueMinMax:
                        condition.IsMeetCondition = EvaluateBaseStatusValueActivationCondition(character, condition);
                        break;
                    case BattleConst.AbilityActivationConditionType.BaseStatusRankAtWholeTeam:
                        condition.IsMeetCondition = EvaluateStatusRankAtTeamActivationCondition(character, condition, true, true);
                        break;
                }
            }
        }

        private static bool EvaluateScoreDiffActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var characterSide = BattleDataMediator.Instance.GetCharacterSide(character);
            var allyScore = BattleDataMediator.Instance.GetScore(characterSide);
            var enemyScore = BattleDataMediator.Instance.GetScore(BattleGameLogic.GetOtherSide(characterSide));
            var scoreDiff = allyScore - enemyScore;
            return condition.IsMeetMinMaxValue(new BigValue(scoreDiff));
        }

        private static bool EvaluateAllyScoreActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var characterSide = BattleDataMediator.Instance.GetCharacterSide(character);
            var allyScore = BattleDataMediator.Instance.GetScore(characterSide);
            return condition.IsMeetMinMaxValue(new BigValue(allyScore));
        }
        
        private static bool EvaluateEnemyScoreActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var characterSide = BattleDataMediator.Instance.GetCharacterSide(character);
            var enemyScore = BattleDataMediator.Instance.GetScore(BattleGameLogic.GetOtherSide(characterSide));
            return condition.IsMeetMinMaxValue(new BigValue(enemyScore));
        }

        private static bool EvaluateSelfScoreCountActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            return condition.IsMeetMinMaxValue(new BigValue(character.ScoredCount));
        }
        
        private static bool EvaluateSelfPositionActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            return condition.PlayerPositions.Contains(character.Position);
        }
        
        /// <summary>特定ステータスのチーム内順位</summary>
        /// <param name="character">対象のキャラ</param>
        /// <param name="condition">アビリティのコンディション</param>
        /// <param name="wholeTeam">チーム全体か</param>
        /// <param name="isBaseStatus">アビリティなしのステ＝タスか</param>
        /// <returns></returns>
        private static bool EvaluateStatusRankAtTeamActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition, bool wholeTeam = false, bool isBaseStatus = false)
        {
            var side = character.Side;
            var sideDeck = new List<BattleCharacterModel>(BattleDataMediator.Instance.GetTeamDeck(side));
            // チーム全体での順位を見る場合は相手チームも含める.
            if (wholeTeam)
            {
                sideDeck.AddRange(BattleDataMediator.Instance.GetTeamDeck(BattleGameLogic.GetOtherSide(side)));
            }
            var targetStatus = condition.StatusType;
            var rank = GetInListStatusRank(character, sideDeck, null, targetStatus, isBaseStatus);

            return condition.IsMeetMinMaxValue(new BigValue(rank));
        }
        
        private static bool EvaluateStatusRankAtRoundTeamActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition, bool wholeTeam = false)
        {
            var side = character.Side;
            var roundCharacters = new List<BattleCharacterModel>(BattleDataMediator.Instance.GetRoundCharacters(side));
            if (wholeTeam)
            {
                roundCharacters.AddRange(BattleDataMediator.Instance.GetRoundCharacters(BattleGameLogic.GetOtherSide(side)));
            }

            var targetStatus = condition.StatusType;
            var rank = GetInListStatusRank(character, roundCharacters, null, targetStatus);

            return condition.IsMeetMinMaxValue(new BigValue(rank));
        }

        public static int GetInListStatusRank(BattleCharacterModel target, List<BattleCharacterModel> characterListA, List<BattleCharacterModel> characterListB, BattleConst.StatusParamType param, bool isBaseStatus = false)
        {
            var status = isBaseStatus ? target.GetBaseParameter(param) : target.GetCurrentParameter(param);
            // 運用(非エンジニア)的にはMin1(=1位)~Max5(5位)みたいな指定方法のほうがわかりやすいと思うので1開始にしておく.
            var rank = 1;
            if (characterListA != null)
            {
                foreach (var character in characterListA)
                {
                    if (status < (isBaseStatus ? character.GetBaseParameter(param) : character.GetCurrentParameter(param)))
                    {
                        rank++;
                    }
                }
            }
            
            if (characterListB != null)
            {
                foreach (var character in characterListB)
                {
                    if (status < (isBaseStatus ? character.GetBaseParameter(param) : character.GetCurrentParameter(param)))
                    {
                        rank++;
                    }
                }
            }

            return rank;
        }
        
        private static bool EvaluateBaseStatusValueActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var targetStatus = condition.StatusType;
            BigValue value = character.GetBaseParameter(targetStatus);
            return condition.IsMeetMinMaxValue(value);
        }
        
        private static bool EvaluateBallPositionActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var side = BattleDataMediator.Instance.GetCharacterSide(character);
            var ballPosition = BattleDataMediator.Instance.GetBallPositionFromAllyGoal(side);
            return condition.IsMeetMinMaxValue(new BigValue(ballPosition));
        }

        private static bool EvaluateCurrentStaminaActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            // スタミナ割合は0.0f~1.0fなので.
            return condition.IsMeetMinMaxValue(new BigValue(Math.Ceiling(character.GetStaminaRate() * 100)));
        }

        private static bool EvaluateAllySpecificCharacterAtTeamActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var side = BattleDataMediator.Instance.GetCharacterSide(character);
            var sideDeck = BattleDataMediator.Instance.GetTeamDeck(side);
            var inDeckCount = GetInDeckSameCharaIdsCount(sideDeck, condition.SameCharaIds);

            return condition.IsMeetMinMaxValue(new BigValue(inDeckCount));
        }

        private static bool EvaluateEnemySpecificCharacterAtTeamActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var enemySide = BattleDataMediator.Instance.GetOtherSide(character);
            var enemyDeck = BattleDataMediator.Instance.GetTeamDeck(enemySide);
            var inDeckCount = GetInDeckSameCharaIdsCount(enemyDeck, condition.SameCharaIds);

            return condition.IsMeetMinMaxValue(new BigValue(inDeckCount));
        }
        
        private static int GetInDeckSameCharaIdsCount(List<BattleCharacterModel> characterList, List<long> targetSameCharaIds)
        {
            var count = 0;

            foreach (var character in characterList)
            {
                if (targetSameCharaIds.Contains(character.ParentMCharaId))
                {
                    count++;
                }
            }
            
            return count;
        }
        
        private static bool EvaluateSpecificCharacterMatchUpActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            return condition.SameCharaIds.Contains(character.MarkCharacter?.ParentMCharaId ?? -1);
        }
        
        private static bool EvaluatePrimaryStatusDifferenceActivationCondition(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var defenceCharacter = character.MarkCharacter;
            // failsafe
            if (defenceCharacter == null)
            {
                return false;
            }
            
            var matchUpParam = character.GetConsParam();
            var offencePrimaryParam = character.GetCurrentParameter(matchUpParam);
            var defencePrimaryParam = defenceCharacter.GetCurrentParameter(matchUpParam);
            var isMeetMinValue = condition.MinValue <= 0 || (offencePrimaryParam * condition.MinValue / 10000.0f) <= defencePrimaryParam;
            var isMeetMaxValue = condition.MaxValue <= 0 || defencePrimaryParam <= offencePrimaryParam * (condition.MaxValue / 10000.0f);

            return isMeetMinValue && isMeetMaxValue;
        }

        private static bool EvaluateAllySpecificBallOwner(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var side = BattleDataMediator.Instance.GetCharacterSide(character);
            if (side != BattleDataMediator.Instance.OffenceSide)
            {
                return false;
            }

            return EvaluateSpecificBallOwner(side, condition);
        }
        
        private static bool EvaluateEnemySpecificBallOwner(BattleCharacterModel character, BattleAbilityActivationCondition condition)
        {
            var side = BattleDataMediator.Instance.GetOtherSide(character);
            if (side != BattleDataMediator.Instance.OffenceSide)
            {
                return false;
            }

            return EvaluateSpecificBallOwner(side, condition);
        }

        private static bool EvaluateSpecificBallOwner(BattleConst.TeamSide side, BattleAbilityActivationCondition condition)
        {
            var ballOwner = BattleDataMediator.Instance.GetBattleCharacter(side, BattleDataMediator.Instance.BallOwnerCharacterId);
            if (ballOwner == null)
            {
                return false;
            }
            
            return condition.SameCharaIds.Contains(ballOwner.ParentMCharaId);
        }
        
        private static List<BattleCharacterModel> GetAbilityEffectTarget(BattleAbilityEffectModel abilityEffect, BattleCharacterModel character)
        {
            var targets = new List<BattleCharacterModel>();
            switch ((BattleConst.AbilityTargetType)abilityEffect.AbilityEffectMaster.targetType)
            {
                case BattleConst.AbilityTargetType.Self:
                    targets.Add(character);
                    break;
                case BattleConst.AbilityTargetType.MatchUpEnemy:
                    if (character.MarkCharacter != null)
                    {
                        targets.Add(character.MarkCharacter);
                    }
                    break;
                case BattleConst.AbilityTargetType.AllyTeam:
                    targets.AddRange(BattleDataMediator.Instance.GetTeamDeck(character.Side));
                    break;
                case BattleConst.AbilityTargetType.EnemyTeam:
                    targets.AddRange(BattleDataMediator.Instance.GetTeamDeck(BattleDataMediator.Instance.GetOtherSide(character)));
                    break;
                case BattleConst.AbilityTargetType.AllyRoundTeam:
                    targets.AddRange(BattleDataMediator.Instance.GetRoundCharacters(character.Side));
                    break;
                case BattleConst.AbilityTargetType.EnemyRoundTeam:
                    targets.AddRange(BattleDataMediator.Instance.GetRoundCharacters(BattleDataMediator.Instance.GetOtherSide(character)));
                    break;
                case BattleConst.AbilityTargetType.ActionTarget:
                    if (character.ActionTargetCharacter != null)
                    {
                        targets.Add(character.ActionTargetCharacter);
                    }
                    break;
                case BattleConst.AbilityTargetType.HighestStatusAllyTeam:
                    targets.Add(BattleDataMediator.Instance.GetHighestStatusCharacter(character.Side, (BattleConst.StatusParamType)abilityEffect.AbilityEffectMaster.targetStatusType));
                    break;
                case BattleConst.AbilityTargetType.HighestStatusEnemyTeam:
                    targets.Add(BattleDataMediator.Instance.GetHighestStatusCharacter(BattleDataMediator.Instance.GetOtherSide(character), (BattleConst.StatusParamType)abilityEffect.AbilityEffectMaster.targetStatusType));
                    break;
                case BattleConst.AbilityTargetType.LowestStatusAllyTeam:
                    targets.Add(BattleDataMediator.Instance.GetLowestStatusCharacter(character.Side, (BattleConst.StatusParamType)abilityEffect.AbilityEffectMaster.targetStatusType));
                    break;
                case BattleConst.AbilityTargetType.LowestStatusEnemyTeam:
                    targets.Add(BattleDataMediator.Instance.GetLowestStatusCharacter(BattleDataMediator.Instance.GetOtherSide(character), (BattleConst.StatusParamType)abilityEffect.AbilityEffectMaster.targetStatusType));
                    break;
            }

            // TODO これ優先順位とかないけど大丈夫…?
            return targets.Take((int)abilityEffect.AbilityEffectMaster.targetNumber).ToList();
        }

        private static void ApplyAbilityEffect(BattleAbilityEffectModel abilityEffect, long level, BattleCharacterModel character, BattleCharacterModel target)
        {
            target.AddActiveAbilityData(abilityEffect, level, character.id);
        }

        public static void DecrementAbilityRemainTurn(BattleConst.AbilityTurnDecrementTiming timing, List<List<BattleCharacterModel>> decks)
        {
            foreach (var deck in decks)
            {
                foreach (var character in deck)
                {
                    character.DecrementAbilityEffectTurn(timing);
                }
            }
        }

        public static bool IsStatusEffect(BattleConst.AbilityEffectType effectType)
        {
            switch (effectType)
            {
                case BattleConst.AbilityEffectType.BuffSpeedUpAddition:
                case BattleConst.AbilityEffectType.BuffTechniqueUpAddition:
                case BattleConst.AbilityEffectType.BuffPhysicalUpAddition:
                case BattleConst.AbilityEffectType.BuffWiseUpAddition:
                case BattleConst.AbilityEffectType.BuffKickUpAddition:
                case BattleConst.AbilityEffectType.BuffShootRangeUpAddition:
                case BattleConst.AbilityEffectType.BuffStaminaUpAddition:
                case BattleConst.AbilityEffectType.BuffSpeedUpMultiply:
                case BattleConst.AbilityEffectType.BuffTechniqueUpMultiply:
                case BattleConst.AbilityEffectType.BuffPhysicalUpMultiply:
                case BattleConst.AbilityEffectType.BuffWiseUpMultiply:
                case BattleConst.AbilityEffectType.BuffKickUpMultiply:
                case BattleConst.AbilityEffectType.BuffShootRangeUpMultiply:
                case BattleConst.AbilityEffectType.BuffStaminaUpMultiply:
                    return true;
            }

            return false;
        }

        public static BattleConst.DigestTiming ReplaceDigestTimingByAbilityTiming( BattleConst.AbilityEvaluateTimingType timing)
        {
            switch (timing)
            {
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughOF:
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughDF:
                    return BattleConst.DigestTiming.Through;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassOF:
                    return BattleConst.DigestTiming.Pass;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassDF:
                    return BattleConst.DigestTiming.PassCut;
                case BattleConst.AbilityEvaluateTimingType.ActiveReceivePass:
                    return BattleConst.DigestTiming.Pass;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF:
                    return BattleConst.DigestTiming.Shoot;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF:
                    return BattleConst.DigestTiming.ShootBlock;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossOF:
                    return BattleConst.DigestTiming.Cross;
                /*
                // リリース時点でない.
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossDF:
                    return BattleConst.DigestTiming.None;
                    */
                case BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross:
                    return BattleConst.DigestTiming.Shoot;
                case BattleConst.AbilityEvaluateTimingType.ActiveOnLooseBall:
                    return BattleConst.DigestTiming.SecondBall;
            }

            return BattleConst.DigestTiming.None;
        }
        
        public static bool IsReplaceDigestByAbilityTiming( BattleConst.AbilityEvaluateTimingType timing)
        {
            switch (timing)
            {
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughOF:
                    return false;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughDF:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassOF:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassDF:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveReceivePass:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossOF:
                    return true;
                /*
                // リリース時点でない.
                case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossDF:
                    return false;
                    */
                case BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross:
                    return true;
                case BattleConst.AbilityEvaluateTimingType.ActiveOnLooseBall:
                    return true;
            }

            return false;
        }
        
        public static bool IsAddLogBeforeMatchUp(BattleConst.AbilityEvaluateTimingType timing)
        {
            switch (timing)
            {
                    case BattleConst.AbilityEvaluateTimingType.OnGameStart:    // ゲーム開始前 (全員)
                    case BattleConst.AbilityEvaluateTimingType.OnRoundStart:   // 参加メンバ、マッチアップ対象の決定後 (ラウンド参加者)
                    case BattleConst.AbilityEvaluateTimingType.OnBeforeMatchUp:// マッチアップ処理時(コマンド選択前) (マッチアップ参加者)
                    case BattleConst.AbilityEvaluateTimingType.OnSelectThroughOF: // 突破選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.OnSelectThroughDF: // 突破選択時 (DF)
                        return true;
                    case BattleConst.AbilityEvaluateTimingType.OnSuccessThrough: // 突破成功時 (OF)
                        return false;
                    case BattleConst.AbilityEvaluateTimingType.OnSelectPassOF: // パス選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.OnSelectPassDF: // パス選択時 (DF)
                        return true;
                    case BattleConst.AbilityEvaluateTimingType.OnSuccessPass: // パス成功時(OF)
                    case BattleConst.AbilityEvaluateTimingType.OnReceivePass: // パス成功時(パス対象)
                        return false;
                    case BattleConst.AbilityEvaluateTimingType.OnSelectShootOF: // シュート選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.OnSelectShootDF: // シュート選択時 (DF)
                        return true;
                    case BattleConst.AbilityEvaluateTimingType.OnSuccessShoot: // シュート成功時(OF時限定)
                        return false;
                    case BattleConst.AbilityEvaluateTimingType.OnSelectCrossOF: // クロス選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.OnSelectCrossDF: // クロス選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.OnReceiveCross: // クロス受け取り時 (クロス対象)
                        return true;
        
                    case BattleConst.AbilityEvaluateTimingType.OnBeforePassCut: // パスカット処理時 (パスカット実行者)
                    case BattleConst.AbilityEvaluateTimingType.OnSuccessPassCut: // パスカット成功時 (パスカット成功者)
                    case BattleConst.AbilityEvaluateTimingType.OnBeforeShootBlock: // シュートブロック処理時 (シュートブロック実行者)
                    case BattleConst.AbilityEvaluateTimingType.OnSuccessShootBlock: // シュートブロック成功時 (シュートブロック成功者)
                    case BattleConst.AbilityEvaluateTimingType.OnLooseBall: // セカンドボール処理時 (セカンドボール参加者)
                        return false;
                    case BattleConst.AbilityEvaluateTimingType.OnAfterAllyGoal: // 味方がゴールを決めたとき(全員)
                    case BattleConst.AbilityEvaluateTimingType.OnAfterEnemyGoal: // 敵がゴールを決めたとき(全員)
                        // ここちょっとややこしいんだけど, そもそもこれマッチアップ後に評価タイミングがくるのでbefore/afterを分ける必要がないので.
                        return true;
                    
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughOF:    // 突破選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughDF:    // 突破選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassOF:       // パス選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassDF:       // パス選択時 (DF)
                        return true;
                    case BattleConst.AbilityEvaluateTimingType.ActiveReceivePass:        // パス成功時(パス対象)
                        return false;
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF:      // シュート選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF:      // シュート選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossOF:      // クロス選択時 (OF)
                    //case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossDF:      // クロス選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross:       // クロス受け取り時 (クロス対象)
                        return true;
                    case BattleConst.AbilityEvaluateTimingType.ActiveOnLooseBall:        // セカンドボール処理時 (セカンドボール参加者)
                        return false;
            }

            return false;
        }
        
        public static bool IsActiveAbility(BattleConst.AbilityEvaluateTimingType timing)
        {
            switch (timing)
            {
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughOF:    // 突破選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughDF:    // 突破選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassOF:       // パス選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectPassDF:       // パス選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveReceivePass:        // パス成功時(パス対象)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF:      // シュート選択時 (OF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF:      // シュート選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossOF:      // クロス選択時 (OF)
                    //case BattleConst.AbilityEvaluateTimingType.ActiveSelectCrossDF:      // クロス選択時 (DF)
                    case BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross:       // クロス受け取り時 (クロス対象)
                    case BattleConst.AbilityEvaluateTimingType.ActiveOnLooseBall:        // セカンドボール処理時 (セカンドボール参加者)
                        return true;
            }

            return false;
        }
    }
}