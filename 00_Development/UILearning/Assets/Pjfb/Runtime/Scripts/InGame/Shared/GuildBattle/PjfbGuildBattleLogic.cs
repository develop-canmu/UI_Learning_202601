using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MagicOnion;

namespace Pjfb.InGame.ClubRoyal
{
    public static class PjfbGuildBattleLogic
    {
        // ステータス効果のマッピング辞書（静的に初期化）
        private static readonly Dictionary<BattleConst.AbilityEffectType, Action<BattleCharacterModel, float>> StatusEffectActions =
            new Dictionary<BattleConst.AbilityEffectType, Action<BattleCharacterModel, float>>
            {
                // Addition effects
                { BattleConst.AbilityEffectType.BuffSpeedUpAddition, (chara, value) => chara.baseSpeed += (long)value },
                { BattleConst.AbilityEffectType.BuffTechniqueUpAddition, (chara, value) => chara.baseTechnique += (long)value },
                { BattleConst.AbilityEffectType.BuffPhysicalUpAddition, (chara, value) => chara.basePhysical += (long)value },
                { BattleConst.AbilityEffectType.BuffWiseUpAddition, (chara, value) => chara.baseWise += (long)value },
                { BattleConst.AbilityEffectType.BuffKickUpAddition, (chara, value) => chara.baseKick += (long)value },
                { BattleConst.AbilityEffectType.BuffShootRangeUpAddition, (chara, value) => chara.baseShootRange += (long)value },
                { BattleConst.AbilityEffectType.BuffStaminaUpAddition, (chara, value) => chara.baseStamina += (long)value },
                
                // Multiply effects
                { BattleConst.AbilityEffectType.BuffSpeedUpMultiply, (chara, value) => chara.baseSpeed = BigValue.CalculationCeiling(chara.baseSpeed, value) },
                { BattleConst.AbilityEffectType.BuffTechniqueUpMultiply, (chara, value) => chara.baseTechnique = BigValue.CalculationCeiling(chara.baseTechnique, value) },
                { BattleConst.AbilityEffectType.BuffPhysicalUpMultiply, (chara, value) => chara.basePhysical = BigValue.CalculationCeiling(chara.basePhysical, value) },
                { BattleConst.AbilityEffectType.BuffWiseUpMultiply, (chara, value) => chara.baseWise = BigValue.CalculationCeiling(chara.baseWise, value) },
                { BattleConst.AbilityEffectType.BuffKickUpMultiply, (chara, value) => chara.baseKick = BigValue.CalculationCeiling(chara.baseKick, value) },
                { BattleConst.AbilityEffectType.BuffShootRangeUpMultiply, (chara, value) => chara.baseShootRange = BigValue.CalculationCeiling(chara.baseShootRange, value) },
                { BattleConst.AbilityEffectType.BuffStaminaUpMultiply, (chara, value) => chara.baseStamina = BigValue.CalculationCeiling(chara.baseStamina, value) }
            };

        public static void UpdatePlayerAvailableMilitaryStrength(ICollection<GuildBattlePlayerData> players)
        {
            var perTurnRecoveryValue = PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleRecoveryMilitaryStrengthPerTurn;
            var requiredTurnToRecoveryBall = PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleRequiredTurnToRecoveryMilitaryStrength;
            foreach (var player in players)
            {
                // 自動配置で未接続なのに兵力を消費しているパターンがあるため, 未接続なら回復しない
                if (player.IsJoinedFirstTime)
                {
                    continue;
                }
                
                // 拠点占拠でキャップを超えて回復することがあるのでその場合はいじらない.
                if (player.AvailableMilitaryStrength >= player.MaxMilitaryStrength)
                {
                    continue;
                }
                
                player.RemainTurnToRecoveryBallCount--;
                if (player.RemainTurnToRecoveryBallCount > 0)
                {
                    continue;
                }
                
                int additionalRecoveryBallPerTurn = player.GuildBattleActivatedAbilityList
                    .Where(ability => ability.RemainTurn > 0 && GuildBattleAbilityLogic.IsBallRecoveryUpPerTurnAbility(ability))
                    .Sum(ability => GuildBattleAbilityLogic.GetRecoveryBallPerTurn(PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleRecoveryMilitaryStrengthPerTurn, ability));
                
                player.RemainTurnToRecoveryBallCount = requiredTurnToRecoveryBall;
                player.AddAvailableMilitaryStrength(perTurnRecoveryValue + additionalRecoveryBallPerTurn, false);
            }
        }

        public static void UpdatePartyRevivalTurn(Dictionary<int, GuildBattlePartyModel> allParties)
        {
            foreach (var kvp in allParties)
            {
                var party = kvp.Value;
                if (party.RevivalTurn > 0)
                {
                    party.RevivalTurn--;
                }
            }
        }

        
        public static Dictionary<int, int> DealSpotDamage(SortedDictionary<int, GuildBattlePartyModel> battleParties,
            Dictionary<int, GuildBattleMapSpotModel> mapSpots, int elapsedTurnCount,
            Dictionary<int, List<GuildBattleCommonPartyModel>> matchingParties)
        {
            var scoredPartyData = new Dictionary<int, int>(matchingParties.Count);
            var isAnyPartyOnSpots = new Dictionary<int, bool>(mapSpots.Count);
            var brokenSpotCounts = new int[] { 0, 0 };
            foreach (var mapSpot in mapSpots)
            {
                if (mapSpot.Value.IsBroken)
                {
                    brokenSpotCounts[(int)mapSpot.Value.OccupyingSide]++;
                }

                isAnyPartyOnSpots.Add(mapSpot.Key, false);
            }

            // 侵入可能かのフラグセット
            foreach (var partyKvp in battleParties)
            {
                var party = partyKvp.Value;
                if (party.IsOnMap() && party.IsDefendingAtAnySpot())
                {
                    isAnyPartyOnSpots[party.TargetSpotId] = true;
                }
            }

            foreach (var partyKvp in matchingParties)
            {
                foreach (var party in partyKvp.Value)
                {
                    // 勝利している場合は関係ない.
                    if (party == null || party.IsOnMap())
                    {
                        continue;
                    }

                    // 敗北している場合, 拠点上で戦闘していたら戦闘したターンには拠点ダメージを入れられたくないので.
                    if (party.IsStayingAtAnySpot())
                    {
                        isAnyPartyOnSpots[party.LastJoinedSpotId] = true;
                    }
                }
            }

            var processedSpotId = new HashSet<int>();
            foreach (var partyKvp in battleParties)
            {
                var party = partyKvp.Value;
                // 戦場にいなければスルー
                if (!party.IsOnMap())
                {
                    continue;
                }

                // 防衛ならスルー
                if (party.IsDefendingAtAnySpot())
                {
                    continue;
                }

                // 防衛軍隊がいるなら拠点攻撃出来ないのでスルー
                if (isAnyPartyOnSpots[party.TargetSpotId])
                {
                    continue;
                }

                var targetSpot = mapSpots[party.TargetSpotId];
                // ないはずだけど一応.
                if (targetSpot.IsBroken)
                {
                    continue;
                }

                var isExceedSpotPosition = party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left
                    ? party.ExpectedXPosition >= targetSpot.PositionX
                    : party.ExpectedXPosition <= targetSpot.PositionX;
                // 移動先が拠点を超えないようならスルー
                if (!isExceedSpotPosition)
                {
                    continue;
                }
                
                // 拠点に対するダメージは1ターンに一度のみ.
                if (!processedSpotId.Contains(targetSpot.Id))
                {
                    processedSpotId.Add(targetSpot.Id);
                    var dealtDamage = targetSpot.TakeDamage(brokenSpotCounts[(int)targetSpot.OccupyingSide], partyKvp.Value.GetBallCount());
                    if (PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData.TryGetValue(party.PlayerIndex, out var playerData))
                    {
                        // すでにオーバーヒールしている状態からは触らない.
                        if (playerData.AvailableMilitaryStrength < playerData.MaxMilitaryStrength)
                        {
                            playerData.AddAvailableMilitaryStrength(party.GetBallCount(), false);
                        }
                    }
                    party.SetAsStandby();
                    scoredPartyData.Add(party.PartyId, dealtDamage);
                }

                // HPが残っているようなら拠点を追い越さないように停止.
                if (targetSpot.RemainHP > 0)
                {
                    party.IsFighting = true;
                    party.ExpectedXPosition = party.XPosition;
                }
            }

            return scoredPartyData;
        }
        
        public static void ProcessSpotBattleResult(BattleConst.BattleResult resultType, GuildBattlePartyModel leftParty, GuildBattlePartyModel rightParty, List<int> score)
        {
            var winParty = resultType == BattleConst.BattleResult.WinLeft ? leftParty : rightParty;
            var loseParty = resultType == BattleConst.BattleResult.WinLeft ? rightParty : leftParty;
            winParty.WinStreakCount = Math.Min(winParty.WinStreakCount + 1, GuildBattleCommonConst.MaxWinStreakCount);
            winParty.SyncLastMilitaryStrengthToMilitaryStrength();
            loseParty.SyncLastMilitaryStrengthToMilitaryStrength();

            PjfbGuildBattleDataMediator.Instance.AddWinningPoint(winParty.Side, PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointPerWinBattle);
            var winPartyScore = score[(int)winParty.Side];
            var losePartyScore = score[(int)loseParty.Side];
            PjfbGuildBattleDataMediator.Instance.AddWinningPoint(winParty.Side, PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointPerInBattlePoint * winPartyScore);
            PjfbGuildBattleDataMediator.Instance.AddWinningPoint(loseParty.Side, PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointPerInBattlePoint * losePartyScore);
            
            loseParty.OnDeadParty();
            winParty.OnWonFight(score[(int)loseParty.Side]);
        }

        public static void ApplyStatusAdviserAbilityEffects(List<BattleCharacterModel> characterList, List<GuildBattleAbilityData> adviserAbilities)
        {
            // remainTurnが0のものは効果がないのでスルー
            List<GuildBattleAbilityData> validAbilities = adviserAbilities.Where(ability => ability.RemainTurn > 0).ToList();
            if (validAbilities.Count == 0)
            {
                return;
            }

            foreach (BattleCharacterModel chara in characterList)
            {
                foreach (GuildBattleAbilityData abilityData in validAbilities)
                {
                    ApplyStatusAdviserAbilityEffect(chara, abilityData);
                }

                chara.ReCalculateParam();
            }
        }


        private static void ApplyStatusAdviserAbilityEffect(BattleCharacterModel chara, GuildBattleAbilityData abilityData)
        {
            // マスターデータを取得
            BattleAbilityModel battleAbilityModel = new BattleAbilityModel();
            battleAbilityModel.SetData(
                PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(a => a.id == abilityData.AbilityId),
                abilityData.AbilityLevel);
            foreach (BattleAbilityEffectModel abilityEffect in battleAbilityModel.BattleAbilityEffectModels)
            {
                if(abilityEffect.AbilityEffectMaster.targetType == (int)BattleConst.AbilityTargetType.GuildBattleSelfSortieUnits)
                {
                    ApplyStatusAdviserAbilityEffectToCharacter(chara, abilityEffect, abilityData.AbilityLevel);
                }
            }
        }

        private static void ApplyStatusAdviserAbilityEffectToCharacter(BattleCharacterModel chara, BattleAbilityEffectModel abilityEffect, int abilityLevel)
        {
            BattleConst.AbilityEffectType effectType = (BattleConst.AbilityEffectType)abilityEffect.AbilityEffectMaster.effectType;
            
            // Dictionaryから対応するアクションを取得して実行（高パフォーマンス）
            if (StatusEffectActions.TryGetValue(effectType, out Action<BattleCharacterModel, float>? action))
            {
                float effectValue = abilityEffect.GetEffectValue(abilityLevel);
                action(chara, effectValue);
            }
        }        
    }
}