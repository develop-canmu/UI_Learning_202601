using System;
using System.Collections.Generic;
using System.Linq;

#if !(MAGIC_ONION_SERVER || PJFB_LAMBDA)
using Pjfb.Master;
#endif

namespace Pjfb.InGame
{
    public class BattleGameLogic
    {
        /// <summary>8Byteの乱数を取得するためのバッファ</summary>
        private static byte[] randomBytes = new byte[8];
        
        public static int GetNonStateRandomValue(int min, int max)
        {
            var rand = new System.Random();
            return rand.Next(min, max);
        }
        
        public static float GetNonStateRandomValue(float min, float max)
        {
            var rand = new System.Random();
            return min + (float)((max - min) * rand.NextDouble());
        }

        public static float GetRandomValue(float min, float max)
        {
            var randValue = BattleDataMediator.Instance.Random.NextDouble();
            return min + (float)((max - min) * randValue);
        }
        
        public static int GetRandomValue(int min, int max)
        {
            return BattleDataMediator.Instance.Random.Next(min, max);
        }
        
        public static long GetRandomValue(long min, long max)
        {
            BattleDataMediator.Instance.Random.NextBytes(randomBytes);
            long value = (long)BitConverter.ToInt64(randomBytes, 0);
            return min + value % (max - min);
        }
        
        public static BigValue GetRandomValue(BigValue min, BigValue max)
        {
            BattleDataMediator.Instance.Random.NextBytes(randomBytes);
            BigValue value = new BigValue(BitConverter.ToInt64(randomBytes, 0));
            return min + value % (max - min);
        }

        public static int GetRandomIndex(IList<int> weights)
        {
            var total = weights.Sum();
            var lotValue = GetRandomValue(1, total + 1);
            for (var i = 0; i < weights.Count; i++)
            {
                lotValue -= weights[i];
                if (lotValue <= 0)
                {
                    return i;
                }
            }

            // failsafe
            return 0;
        }
        
        public static int GetRandomIndex(IList<long> weights)
        {
            var total = weights.Sum();
            var lotValue = GetRandomValue(1, total + 1);
            for (var i = 0; i < weights.Count; i++)
            {
                lotValue -= weights[i];
                if (lotValue <= 0)
                {
                    return i;
                }
            }

            // failsafe
            return 0;
        }
        
        public static int GetRandomIndex(IList<BigValue> weights)
        {
            var total = weights.Sum();
            BigValue lotValue = GetRandomValue(new BigValue(1), total + 1);
            for (var i = 0; i < weights.Count; i++)
            {
                lotValue -= weights[i];
                if (lotValue <= 0)
                {
                    return i;
                }
            }

            // failsafe
            return 0;
        }
        
        public static int GetNonStateRandomIndex(IList<int> weights)
        {
            var total = weights.Sum();
            var lotValue = GetNonStateRandomValue(1, total + 1);
            for (var i = 0; i < weights.Count; i++)
            {
                lotValue -= weights[i];
                if (lotValue <= 0)
                {
                    return i;
                }
            }

            // failsafe
            return 0;
        }

        public static int GetRandomIntValue()
        {
            return GetRandomValue(Int32.MinValue, Int32.MaxValue);
        }

        /// <summary>
        /// 0~1の成功率を引数にとって抽選結果を返却
        /// </summary>
        /// <param name="successRate">0.0f ~ 1.0f</param>
        /// <returns></returns>
        public static bool LotteryResultByRate(float successRate)
        {
            var randValue = GetRandomValue(0.0f, 1.0f);
            return randValue <= successRate;
        }
        
        public static bool LotteryResultByRateNonState(float successRate)
        {
            var randValue = GetNonStateRandomValue(0.0f, 1.0f);
            return randValue <= successRate;
        }
        

        public static BattleConst.TeamSide GetOtherSide(BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return BattleConst.TeamSide.TeamSizeMax;
            }

            return side == BattleConst.TeamSide.Left ? BattleConst.TeamSide.Right : BattleConst.TeamSide.Left;
        }

        public static BattleConst.TeamSide GetInitialAttackerSide(BattleConst.BattleType battleType, long initialAttackSideIndex)
        {
            if (initialAttackSideIndex == 1)
            {
                // 0 or 1 のみのはず. キャストするのはワンチャン例外発生しそうで怖いので, 1なら強制Rightで.
                return BattleConst.TeamSide.Right;
            }
            
            switch (battleType)
            {
                // ソロモードはすべて自分が初回攻撃サイド
                case BattleConst.BattleType.StoryBattle:
                case BattleConst.BattleType.TrainingBattle:
                case BattleConst.BattleType.RivalryBattle:
                    return BattleConst.TeamSide.Left;
                // ギルバトはランダム
                case BattleConst.BattleType.MagicOnionServerBattle:
                    return BattleDataMediator.Instance.Random.Next(0, 2) == 0
                        ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right; 
            }

            return BattleConst.TeamSide.Left;
        }

        public static long GetInitialBallOwnerId(long ballOwnerId, List<BattleCharacterModel> offenceCharacters)
        {
            // ボール奪取等でそもそもボール保有者が決まっていたらそのキャラクター.
            if (ballOwnerId > 0)
            {
                return ballOwnerId;
            }

            return offenceCharacters.OrderByDescending(chara => chara.IsAceCharacter).FirstOrDefault().id;
        }
        
        public static int GetRemainDistanceToGoal(BattleConst.TeamSide side, int currentBallPosition)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return -1;
            }

            if (side == BattleConst.TeamSide.Left)
            {
                return BattleConst.FieldSize - currentBallPosition;
            }
            else
            {
                return currentBallPosition;
            }
        }

        public static void LotteryOffencePlayers(List<BattleCharacterModel> teamCharacters, List<BattleCharacterModel> refOffenceCharacters, long ballOwnerId, bool isStartAtKickOff)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceJoinFullMemberToRound)
            {
                refOffenceCharacters.AddRange(teamCharacters);
                return;
            }
#endif
            // ボールを持っているプレイヤーは無条件で参加.
            var ballOwner = teamCharacters.Find(character => character.id == ballOwnerId);
            if (ballOwner != null)
            {
                refOffenceCharacters.Add(ballOwner);
            }

            // キックオフ時かどうかでそもそもフローが違う…
            if (isStartAtKickOff)
            {
                // OF, MFが参加
                var OFCharacters = teamCharacters.Where(character => character.Position == BattleConst.PlayerPosition.FW && character != ballOwner);
                refOffenceCharacters.AddRange(OFCharacters);
                var MFCharacters = teamCharacters.Where(character => character.Position == BattleConst.PlayerPosition.MF && character != ballOwner);
                refOffenceCharacters.AddRange(MFCharacters);
            }
            else
            {
                var maxSpeed = teamCharacters.Max(character => character.GetCurrentSpeed());
                foreach (var character in teamCharacters)
                {
                    // ボール所有者は無条件で追加, とかあるのでfailsafe
                    if (refOffenceCharacters.Contains(character))
                    {
                        continue;
                    }
                
                    var positionJoinCoef = BattleDataMediator.Instance.GetForwardJoinCoefficient(character.Position);
                    // 参加係数が0であればスキップ
                    if (positionJoinCoef == 0) continue;
                    var joinRate = BigValue.RatioCalculation(character.GetCurrentSpeed(), maxSpeed) * positionJoinCoef + character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffRoundJoinRateUp);
                    var randValue = GetRandomValue(0.0f, 1.0f);
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
                    CruFramework.Logger.Log($"ラウンド参加率(オフェンス) \n この選手の参加率(こっちの値が高ければ参加): {joinRate}\n 今回の値: {randValue}");
#endif
                    if (randValue <= joinRate)
                    {
                        refOffenceCharacters.Add(character);
                    }
                }                
            }
        }
        
        public static void LotteryDefencePlayers(List<BattleCharacterModel> teamCharacters, List<BattleCharacterModel> refDefenceCharacters, int offencePlayerCount, bool isStartAtKickOff)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceJoinFullMemberToRound)
            {
                refDefenceCharacters.AddRange(teamCharacters);
                return;
            }
#endif
            
            // キックオフ時かどうかでそもそもフローが違う…
            if (isStartAtKickOff)
            {
                // DF, MF
                var DFCharacters = teamCharacters.Where(character => character.Position == BattleConst.PlayerPosition.DF);
                refDefenceCharacters.AddRange(DFCharacters);
                var MFCharacters = teamCharacters.Where(character => character.Position == BattleConst.PlayerPosition.MF);
                refDefenceCharacters.AddRange(MFCharacters);
            }
            else
            {
                var maxSpeed = teamCharacters.Max(character => character.GetCurrentSpeed());
                foreach (var character in teamCharacters)
                {
                    // ボール所有者は無条件で追加, とかあるのでfailsafe
                    if (refDefenceCharacters.Contains(character))
                    {
                        continue;
                    }
                
                    var positionJoinCoef = BattleDataMediator.Instance.GetDefenceJoinCoefficient(character.Position);
                    // 参加係数が0であればスキップ
                    if (positionJoinCoef == 0) continue;
                    var joinRate = BigValue.RatioCalculation(character.GetCurrentSpeed(), maxSpeed) * positionJoinCoef + character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffRoundJoinRateUp);
                    var randValue = GetRandomValue(0.0f, 1.0f);
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
                    CruFramework.Logger.Log($"ラウンド参加率(ディフェンス) \n この選手の参加率(こっちの値が高ければ参加): {joinRate}\n 今回の値: {randValue}");
#endif
                    if (randValue <= joinRate)
                    {
                        refDefenceCharacters.Add(character);
                    }
                }                
            }

            // 攻撃側の人数より少ないならDFは強制的に参加とする.
            if (offencePlayerCount > refDefenceCharacters.Count)
            {
                // 頻度, 値の数がたかが知れてるのでわかりやすくLinqで.
                // 無理やり補充されるのに初期値の配置によると偏りが生まれるのでスタミナ順にしとく.
                var notJoinedDFCharacters = teamCharacters
                    .Where(character => character.Position == BattleConst.PlayerPosition.DF && !refDefenceCharacters.Contains(character))
                    .OrderByDescending(character => character.CurrentStamina);
                var addCount = Math.Min(offencePlayerCount - refDefenceCharacters.Count, notJoinedDFCharacters.Count());
                refDefenceCharacters.AddRange(notJoinedDFCharacters.Take(addCount));
            }
        }

        public static void SetMarkTargetPlayers(long ballOwnerCharacterId, List<BattleCharacterModel> offenceCharacters, List<BattleCharacterModel> defenceCharacters)
        {
            // ボール保有者 -> 強いOFの順に優先してマークをつける.
            var sortedOffenceCharacters = offenceCharacters
                .Where(character => character.ClearedNumOnRound < 0)
                .OrderByDescending(character => character.id == ballOwnerCharacterId)
                .ThenByDescending(character => BigValue.Max(BigValue.Max(character.Speed, character.Physical), character.Technique));
            foreach (var offenceCharacter in sortedOffenceCharacters)
            {
                var matchUpParam = offenceCharacter.GetConsParam();
                offenceCharacter.PrimaryParam = matchUpParam;
                BattleCharacterModel markCharacter = null;

                foreach (var defenceCharacter in defenceCharacters)
                {
                    // 既に突破されているDFなら(基本的に)マークにつかない.
                    if (defenceCharacter.ClearedNumOnRound > 0)
                    {
                        continue;
                    }

                    // すでに誰かのマークについてたらマークにつかない.
                    if (defenceCharacter.MarkedCount > 0)
                    {
                        continue;
                    }
                    
                    // まだ誰もマークについていない or マークしているキャラよりパラメータが高かったら優先
                    if ((markCharacter == null || defenceCharacter.GetCurrentParameter(matchUpParam) > markCharacter.GetCurrentParameter(matchUpParam)))
                    {
                        markCharacter = defenceCharacter;
                    }
                }
                
                if (markCharacter != null)
                {
                    offenceCharacter.MarkCharacter = markCharacter;
                    markCharacter.MarkedCount++;
                }
            }
        }

        public static BattleMatchUpResult JudgeDribbleResult(BattleCharacterModel offenceCharacter,
            List<BattleCharacterModel> offenceRoundCharacters, List<BattleCharacterModel> defenceRoundCharacters,
            List<BattleCharacterModel> offenceTeamCharacters, List<BattleCharacterModel> defenceTeamCharacters,
            int currentBallPosition)
        {
            // TODO BattleStateResultとかの名前のほうがいいかな…
            var matchUpResult = new BattleMatchUpResult();
            matchUpResult.RoundOffenceCharacterIds = offenceRoundCharacters.Select(character => character.id).ToList();
            matchUpResult.RoundDefenceCharacterIds = defenceRoundCharacters.Select(character => character.id).ToList();
            matchUpResult.NextBallPosition = currentBallPosition;

            // ドリブルによる消費分
            AddChangedStaminaValue(offenceCharacter, BattleConst.AdjustableValueConsumeStaminaValueOnAttack, ref matchUpResult);
            
            // 回復分
            AddChangedStaminaValues(offenceTeamCharacters, BattleConst.AdjustableValueRecoveryStaminaValueOnAttack, ref matchUpResult);
            AddChangedStaminaValues(defenceTeamCharacters, BattleConst.AdjustableValueRecoveryStaminaValueOnAttack, ref matchUpResult);
            
            // ↑でRemoveとかしてnewされるのが嫌で全員一度回復しているので, ドリブル実行中のキャラクターは↑の回復を取り消す.
            AddChangedStaminaValue(offenceCharacter, BattleConst.AdjustableValueRecoveryStaminaValueOnAttack * -1, ref matchUpResult);
            matchUpResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeDribble);

            var additionalDribbleDistance = (int)Math.Floor(offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffDribbleDistanceUp));

            matchUpResult.NextBallPosition = GetNextBallPosition(matchUpResult.NextBallPosition, offenceCharacter,
                Math.Max(BattleConst.AdjustableValueMinMoveValueForDribble + additionalDribbleDistance, BattleConst.MinDribbleMoveValue),
                Math.Max(BattleConst.AdjustableValueMaxMoveValueForDribble + additionalDribbleDistance, BattleConst.MinDribbleMoveValue),
                offenceRoundCharacters, defenceRoundCharacters);
            
            // ドリブルではボール所有者が変わらないため.
            matchUpResult.NextBallOwnerId = offenceCharacter.id;

            var remainDistanceToGoal = GetRemainDistanceToGoal(offenceCharacter.Side, matchUpResult.NextBallPosition);
            matchUpResult.IsInShootRange = offenceCharacter.GetCurrentShootRange() >= remainDistanceToGoal;

            return matchUpResult;
        }

        /// <summary>
        /// 突破, パス, シュート等への結果判定
        /// </summary>
        /// <param name="actionType">突破, パス, シュート, クロス(OF側)</param>
        /// <param name="defencePredictionType">OFがなにでくると予想したか(DF側)</param>
        /// <param name="offenceCharacter">マッチアップにおける主体OFキャラクター</param>
        /// <param name="offenceCharacterIds">マッチアップに参加しているOFキャラクターIds</param>
        /// <param name="offenceTeamCharacters">OF側全キャラクターIds</param>
        /// <param name="defenceCharacter">マッチアップにおける主体DFキャラクター</param>
        /// <param name="defenceCharacterIds">マッチアップに参加しているDFキャラクターIds</param>
        /// <param name="offenceTeamCharacters">DF側全キャラクターIds</param>
        /// <returns></returns>
        public static BattleMatchUpResult JudgeMatchUpPreResult(
            ref BattleMatchUpResult matchUpResult, BattleConst.MatchUpActionType actionType, BattleConst.MatchUpActionDetailType actionDetailType,
            BattleCharacterModel offenceCharacter, List<BattleCharacterModel> offenceRoundCharacters,
            BattleCharacterModel defenceCharacter, List<BattleCharacterModel> defenceRoundCharacters,
            int currentBallPosition, BattleCharacterModel targetCharacter, int distanceToGoal)
        {
            matchUpResult.ActionType = actionType;
            matchUpResult.ActionDetailType = actionDetailType;
            matchUpResult.OffenceCharacterId = offenceCharacter.id;
            matchUpResult.DefenceCharacterId = offenceCharacter.MarkCharacter.id;
            matchUpResult.TargetCharacterId = targetCharacter?.id ?? -1;
            matchUpResult.RoundOffenceCharacterIds = offenceRoundCharacters.Select(character => character.id).ToList();
            matchUpResult.RoundDefenceCharacterIds = defenceRoundCharacters.Select(character => character.id).ToList();
            matchUpResult.NextBallPosition = currentBallPosition;
            matchUpResult.IsInShootRange = offenceCharacter.GetCurrentShootRange() >= distanceToGoal;
            matchUpResult.IsProgress = true;
            
            // PreResultでの演出 -> スキル発動可否から最終決定 の流れとなるので, この段階では一旦キメで成功時の演出を流す.
            // JudgeFinalResultで上書きされたのちに最終結果演出を途中から再生.
            matchUpResult.MatchUpResult = BattleConst.MatchUpResult.Success;
            
            // マッチアップ参加者のスタミナ消費
            AddChangedStaminaValue(offenceCharacter, BattleConst.AdjustableValueConsumeStaminaValueForMatchUp, ref matchUpResult);
            AddChangedStaminaValue(offenceCharacter.MarkCharacter, BattleConst.AdjustableValueConsumeStaminaValueForMatchUp, ref matchUpResult);

            SetOffenceAbilityData(ref matchUpResult, actionType, offenceCharacter, targetCharacter);
            SetDefenceAbilityData(ref matchUpResult, actionType, defenceCharacter);

            EvaluatePassiveAbility(ref matchUpResult, actionType, offenceCharacter, targetCharacter, defenceCharacter);
            
            // DFのスキル発動は確定していたらこの時点で評価
            if (matchUpResult.DefenceAbilityId > 0)
            {
                EvaluateActiveAbility(ref matchUpResult, defenceCharacter, matchUpResult.DefenceAbilityId);
            }

            // クロスの場合だけ事前にナイスパス判定を行う. 必殺技が発動した場合は仕切り直し. まぁ発動した場合は演出自体差し替わるはずなので.
            if (actionType == BattleConst.MatchUpActionType.Cross)
            {
                matchUpResult.IsNicePass = JudgeIsNicePass(offenceCharacter, defenceRoundCharacters);
            }
            
            return matchUpResult;
        }
        
        public static BattleMatchUpResult JudgeMatchUpFinalResult(
            ref BattleMatchUpResult matchUpResult, BattleConst.MatchUpActionType actionType, BattleConst.MatchUpActionDetailType actionDetailType,
            BattleCharacterModel offenceCharacter, List<BattleCharacterModel> offenceRoundCharacters, List<BattleCharacterModel> offenceTeamCharacters,
            BattleCharacterModel defenceCharacter, List<BattleCharacterModel> defenceRoundCharacters, List<BattleCharacterModel> defenceTeamCharacters,
            int distanceToGoal, BattleCharacterModel targetCharacter, BattleBluelockManModel blueLockMan)
        {
            matchUpResult.IsProgress = false;
            if (matchUpResult.OffenceAbilityId > 0)
            {
                if (matchUpResult.OffenceAbilityUserCharacterId == offenceCharacter.id)
                {
                    EvaluateActiveAbility(ref matchUpResult, offenceCharacter, matchUpResult.OffenceAbilityId, matchUpResult.IsSwiped);
                }
                else
                {
                    EvaluateActiveAbility(ref matchUpResult, targetCharacter, matchUpResult.OffenceAbilityId, matchUpResult.IsSwiped);
                }
            }

            switch (actionType)
            {
                case BattleConst.MatchUpActionType.Through:
                {
                    var matchUpParam = offenceCharacter.PrimaryParam;
                    JudgeThroughResult(
                        matchUpParam, actionDetailType, offenceCharacter, defenceCharacter,
                        offenceTeamCharacters, defenceTeamCharacters,
                        ref matchUpResult);
                    break;
                }
                case BattleConst.MatchUpActionType.Pass:
                {
                    JudgePassResult(
                        offenceCharacter, targetCharacter, defenceCharacter,
                        defenceRoundCharacters,
                        offenceTeamCharacters, defenceTeamCharacters,
                        actionDetailType, ref matchUpResult);
                    break;
                }
                case BattleConst.MatchUpActionType.Shoot:
                {
                    JudgeShootResult(
                        actionDetailType, offenceCharacter, defenceCharacter,
                        offenceRoundCharacters, defenceRoundCharacters,
                        offenceTeamCharacters, defenceTeamCharacters,
                        ref matchUpResult, blueLockMan, distanceToGoal);
                    break;
                }
                case BattleConst.MatchUpActionType.Cross:
                {
                    JudgeCrossResult(
                        actionDetailType, offenceCharacter, targetCharacter, defenceCharacter,
                        offenceRoundCharacters, defenceRoundCharacters,
                        offenceTeamCharacters, defenceTeamCharacters,
                        ref matchUpResult, blueLockMan, distanceToGoal);
                    break;
                }
            }

            return matchUpResult;
        }

        private static void SetOffenceAbilityData(ref BattleMatchUpResult matchUpResult, BattleConst.MatchUpActionType actionType, BattleCharacterModel character, BattleCharacterModel target)
        {
            var activeAbility = character.GetAvailableActiveOFAbility();
            var targetActiveAbility = actionType is BattleConst.MatchUpActionType.Cross or BattleConst.MatchUpActionType.Pass ? target.GetAvailableActiveOFAbility() : null;
            if (activeAbility == null && targetActiveAbility == null)
            {
                return;
            }

            var isActivatedMain = activeAbility != null && BattleAbilityLogic.EvaluateAbilityTiming(activeAbility, actionType, false) &&
                                  LotteryResultByRate(activeAbility.GetInvokeRate(activeAbility.AbilityLevel) * character.AbilityInvokeRateCoefficient) &&
                                  BattleAbilityLogic.EvaluateAbilityCondition(character, activeAbility);
            var isActivatedTarget = !isActivatedMain && targetActiveAbility != null && BattleAbilityLogic.EvaluateAbilityTiming(targetActiveAbility, actionType, true) &&
                                    LotteryResultByRate(targetActiveAbility.GetInvokeRate(targetActiveAbility.AbilityLevel) * target.AbilityInvokeRateCoefficient) &&
                                    BattleAbilityLogic.EvaluateAbilityCondition(target, targetActiveAbility);
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceSuccessActiveInvokeByRate)
            {
                isActivatedMain = activeAbility != null && BattleAbilityLogic.EvaluateAbilityTiming(activeAbility, actionType, false);
                isActivatedTarget = !isActivatedMain && targetActiveAbility != null && BattleAbilityLogic.EvaluateAbilityTiming(targetActiveAbility, actionType, true);
            }
#endif
            // 発動抽選失敗したらなし
            if (!isActivatedMain && !isActivatedTarget)
            {
                return;
            }
            
            var timing = activeAbility != null ? (BattleConst.AbilityEvaluateTimingType)activeAbility.BattleAbilityMaster.timing : BattleConst.AbilityEvaluateTimingType.None;
            var abilityId = -1L;
            var abilityUserCharacterId = -1L;

            if (isActivatedMain)
            {
                abilityId = activeAbility.BattleAbilityMaster.id;
                abilityUserCharacterId = character.id;
                
            }else if (isActivatedTarget)
            {
                abilityId = targetActiveAbility.BattleAbilityMaster.id;
                abilityUserCharacterId = target.id;
                // このタイミングで受けて側が発動する場合はクロス受け取りのみ
                timing = BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross;
            }

            matchUpResult.OffenceAbilityId = abilityId;
            matchUpResult.OffenceAbilityUserCharacterId = abilityUserCharacterId;
            matchUpResult.ActivateAbilityTimingType = timing;
        }

        private static void EvaluateActiveAbility(ref BattleMatchUpResult matchUpResult, BattleCharacterModel character, long abilityId, bool extendAbilityEffectLevel = false)
        {
            var battleAbility = character.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.id == abilityId);
            if (battleAbility == null)
            {
                return;
            }

            if (!BattleAbilityLogic.EvaluateAbility(character, battleAbility, null, null, true, extendAbilityEffectLevel))
            {
                return;
            }

            var timing = (BattleConst.AbilityEvaluateTimingType)battleAbility.BattleAbilityMaster.timing;
            var isReplace = BattleAbilityLogic.IsReplaceDigestByAbilityTiming(timing);
            if (isReplace)
            {
                var replaceTiming = BattleAbilityLogic.ReplaceDigestTimingByAbilityTiming(timing);
                matchUpResult.AddReplaceDigest(replaceTiming, character, battleAbility);
            }
            else
            {
                matchUpResult.AddInsertDigest(character, battleAbility);
            }
        }

        private static void EvaluatePassiveAbility(ref BattleMatchUpResult battleResult, BattleConst.MatchUpActionType actionType,
            BattleCharacterModel offenceCharacter, BattleCharacterModel targetCharacter, BattleCharacterModel defenceCharacter)
        {
            var offenceAbilityType = BattleConst.AbilityEvaluateTimingType.None;
            var defenceAbilityType = BattleConst.AbilityEvaluateTimingType.None;

            switch (actionType)
            {
                case BattleConst.MatchUpActionType.Through:
                    offenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectThroughOF;
                    defenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectThroughDF;
                    break;
                case BattleConst.MatchUpActionType.Pass:
                    offenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectPassOF;
                    defenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectPassDF;
                    break;
                case BattleConst.MatchUpActionType.Shoot:
                    offenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectShootOF;
                    defenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectShootDF;
                    break;
                case BattleConst.MatchUpActionType.Cross:
                    offenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectCrossOF;
                    defenceAbilityType = BattleConst.AbilityEvaluateTimingType.OnSelectCrossDF;
                    break;
            }
            
            BattleAbilityLogic.EvaluateCharacterAbility(offenceAbilityType, offenceCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
            BattleAbilityLogic.EvaluateCharacterAbility(defenceAbilityType, defenceCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);

            if (actionType == BattleConst.MatchUpActionType.Cross)
            {
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnReceiveCross, targetCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
                
                // クロスは打ち上げ -> クロス対象がシュート の流れになるのでシュートパッシブも評価
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSelectShootOF, targetCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSelectShootDF, defenceCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
            }
        }

        private static void JudgeThroughResult(
            BattleConst.StatusParamType statusParamType, BattleConst.MatchUpActionDetailType actionDetailType, BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter,
            List<BattleCharacterModel> offenceTeamCharacters, List<BattleCharacterModel> defenceTeamCharacters,
            ref BattleMatchUpResult battleResult)
        {
            var offenceMatchUpValue = GetMatchUpParameterValue(statusParamType, offenceCharacter, actionDetailType);
            var defenceMatchUpValue = defenceCharacter != null ? GetMatchUpParameterValue(statusParamType, defenceCharacter, BattleConst.MatchUpActionDetailType.Type0) : BigValue.Zero;

            battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeRunThrough);
            
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceSuccessMatchUp)
            {
                offenceMatchUpValue = new BigValue(100);
                defenceMatchUpValue = BigValue.Zero;
            }else if (BattleDataMediator.Instance.ForceFailMatchUp)
            {
                offenceMatchUpValue = BigValue.Zero;
                defenceMatchUpValue = new BigValue(100);
            }
#endif

            // 勝利の場合
            if (offenceMatchUpValue >= defenceMatchUpValue)
            {
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSuccessThrough, offenceCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
                defenceCharacter.ClearedNumOnRound = GetMaxClearedNum(defenceTeamCharacters) + 1;
                battleResult.NextBallOwnerId = offenceCharacter.id;
                battleResult.MatchUpResult = BattleConst.MatchUpResult.Success;
                
                offenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Through, true);
                defenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.ThroughBlock, false);
                return;
            }
            
            // 敗北の場合はDFにボールが奪われた, セカンドボールになった等の処理
            var interferenceType = GetThroughResultInterferenceType(offenceCharacter, defenceCharacter);
            // ボールがDFに奪取された.
            // 以下, BallInterferenceTypeは特に意味はないが, 演出上切り分けとかをもしするならここらへんも抽選でいい感じにしたりするので一旦置いとく.
            if (interferenceType == BattleConst.BallInterferenceType.Catched)
            {
                battleResult.IsSideChanged = true;
                battleResult.IsResetRound = true;
                battleResult.NextBallOwnerId = defenceCharacter.id;
                battleResult.MatchUpResult = BattleConst.MatchUpResult.Failed;
                battleResult.BallInterferenceType = BattleConst.BallInterferenceType.Catched;
            }
            // セカンドボール争い
            else
            {
                SetLooseBallResult(offenceCharacter, defenceCharacter, offenceTeamCharacters, defenceTeamCharacters, ref battleResult, BattleConst.LooseBallReasonType.FailedRunThrough);
                battleResult.BallInterferenceType = BattleConst.BallInterferenceType.Blocked;
            }
            
            offenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Through, false);
            defenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.ThroughBlock, true);
        }

        private static BattleConst.BallInterferenceType GetThroughResultInterferenceType(BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter)
        {
            // BattleConst.DefaultMatchUpDFTakeBallWeight + BattleConst.DefaultMatchUpLooseBallWeight
            // 一旦キメだけどスキルの影響とか発生してきそうなのでラップ
            // 60%の確率でセカンドボール, 40%でボール奪取
            var isCatched = LotteryResultByRate(BattleConst.DefaultMatchUpDFTakeBallWeight);
            return isCatched ? BattleConst.BallInterferenceType.Catched : BattleConst.BallInterferenceType.Blocked;
        }
        
        private static void JudgePassResult(BattleCharacterModel offenceCharacter, BattleCharacterModel passTargetCharacter, BattleCharacterModel defenceCharacter,
            List<BattleCharacterModel> defenceRoundCharacters, List<BattleCharacterModel> offenceTeamCharacters, List<BattleCharacterModel> defenceTeamCharacters,
            BattleConst.MatchUpActionDetailType actionDetailType, ref BattleMatchUpResult battleResult)
        {
            // パスを出したプレイヤーのスタミナ消費
            AddChangedStaminaValue(offenceCharacter, BattleConst.AdjustableValueConsumeStaminaValueForPass, ref battleResult);
            offenceCharacter.ClearedNumOnRound = GetMaxClearedNum(offenceTeamCharacters) + 1;
            battleResult.TargetCharacterId = passTargetCharacter.id;

            var min = 0;
            var max = 0;
            var passDetailType = (BattleConst.PassDetailType)actionDetailType; 
            switch (passDetailType)
            {
                case BattleConst.PassDetailType.NormalPass:
                case BattleConst.PassDetailType.NormalNicePass:
                case BattleConst.PassDetailType.NormalBadPass:
                    min = BattleConst.AdjustableValueMinMoveValueForBackPass;
                    max = BattleConst.AdjustableValueMaxMoveValueForBackPass;
                    break;
                case BattleConst.PassDetailType.ForwardPass:
                case BattleConst.PassDetailType.ForwardNicePass:
                case BattleConst.PassDetailType.ForwardBadPass:
                    min = BattleConst.AdjustableValueMinMoveValueForForwardPass;
                    max = BattleConst.AdjustableValueMaxMoveValueForForwardPass;
                    break;
                case BattleConst.PassDetailType.LongPass:
                {
                    // フィールドの中央+-Min/Maxに着地するように調整
                    var baseMove = Math.Abs((int)Math.Floor(battleResult.NextBallPosition - BattleConst.FieldSize / 2.0f));
                    min = baseMove - BattleConst.AdjustableValueMoveRangeValueForLongPass;
                    max = baseMove + BattleConst.AdjustableValueMoveRangeValueForLongPass;
                }
                    break;
            }
            battleResult.NextBallPosition = GetNextBallPosition(battleResult.NextBallPosition, offenceCharacter, min, max);
            battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimePass);

            var markCharacter = offenceCharacter.MarkCharacter;
            var sortedDefence = defenceRoundCharacters.OrderByDescending(character => character == markCharacter).ToList();
            var denominator = offenceCharacter.GetCurrentWise() + offenceCharacter.GetCurrentTechnique() / BattleConst.AdjustableValueTechnicCoefficientForDodgePassCut;
            var passCutSuccessRateAddition = 0.0f;
            switch (passDetailType)
            {
                case BattleConst.PassDetailType.NormalNicePass:
                    passCutSuccessRateAddition = -0.2f;
                    break;
                case BattleConst.PassDetailType.NormalBadPass:
                    passCutSuccessRateAddition = 0.2f;
                    break;
                case BattleConst.PassDetailType.ForwardPass:
                    passCutSuccessRateAddition = 0.2f;
                    break;
                case BattleConst.PassDetailType.ForwardBadPass:
                    passCutSuccessRateAddition = 0.5f;
                    break;
            }
            
            foreach (var character in sortedDefence)
            {
                // 突破済みならパスカット判定はなし.
                if (character.ClearedNumOnRound > 0)
                {
                    continue;
                }
                
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnBeforePassCut, character, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);

                var passCutSuccessRatio = BigValue.RatioCalculation(character.GetCurrentWise(), denominator);
                BigValue requestValue = new BigValue(passCutSuccessRatio * BigValue.DefaultRateValue);
                BigValue requestMin = new BigValue(BattleConst.AdjustableValueRequiredMinWiseValueForPassCut * BigValue.DefaultRateValue);
                BigValue requestMax = new BigValue(BattleConst.AdjustableValueRequiredMaxWiseValueForPassCut * BigValue.DefaultRateValue);
                float convertedPassCutSuccessRatio = ConvertRequestValue(requestValue, requestMin, requestMax, BattleConst.AdjustableValueMinPassCutRate, BattleConst.AdjustableValueMaxPassCutRate);

                var passCutSuccessRateUp = character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffPassCutRateUp);
                convertedPassCutSuccessRatio += passCutSuccessRateUp;
                convertedPassCutSuccessRatio += passCutSuccessRateAddition;

                // パスカットにトライするプレイヤーのスタミナ消費(成否関係なし)
                AddChangedStaminaValue(character, BattleConst.AdjustableValueConsumeStaminaValueForPassCut, ref battleResult);
                
                var isSuccessPussCut = LotteryResultByRate(convertedPassCutSuccessRatio);
                character.Stats.AddActivityStat(BattleCharacterStatModel.StatType.PassCut, isSuccessPussCut);
                if (isSuccessPussCut)
                {
                    BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSuccessPassCut, character, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
                    battleResult.DefenceActionCharacterId = character.id;
                    
                    var interferenceType =  GetPassCutResult(character);
                    // マッチアップ相手に直でボールをとられたパターン
                    if (interferenceType == BattleConst.BallInterferenceType.Catched)
                    {
                        battleResult.MatchUpResult = BattleConst.MatchUpResult.Failed;
                        battleResult.BallInterferenceType = BattleConst.BallInterferenceType.Catched;
                        battleResult.NextBallOwnerId = character.id;
                        battleResult.NextBallPosition =
                            GetNextBallPosition(battleResult.NextBallPosition, offenceCharacter,
                                BattleConst.AdjustableValueMinMoveDistanceForPassCut, BattleConst.AdjustableValueMaxMoveDistanceForPassCut);
                        battleResult.IsSideChanged = true;
                        battleResult.IsResetRound = true;
                    }
                    // 弾かれてルーズボールになった場合
                    else
                    {
                        SetLooseBallResult(offenceCharacter, defenceCharacter, offenceTeamCharacters, defenceTeamCharacters, ref battleResult, BattleConst.LooseBallReasonType.PassCut);
                        battleResult.BallInterferenceType = BattleConst.BallInterferenceType.Blocked;
                    }

                    offenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Pass, false);
                    return;
                }
            }
            
            BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSuccessPass, offenceCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
            BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnReceivePass, passTargetCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
            
            var activeOnReceivePassAbility = passTargetCharacter.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.timing == (long) BattleConst.AbilityEvaluateTimingType.ActiveReceivePass);
            if (activeOnReceivePassAbility != null && battleResult.OffenceAbilityId < 0 &&
                // 結果確定後のアクティブはスワイプの如何に関わらずLvは+1判定. 結果を変えようがないのでスワイプしようがしまいが関係ないので.
                LotteryResultByRate(activeOnReceivePassAbility.GetInvokeRate(activeOnReceivePassAbility.AbilityLevel + 1) * passTargetCharacter.AbilityInvokeRateCoefficient) &&
                BattleAbilityLogic.EvaluateAbilityCondition(passTargetCharacter, activeOnReceivePassAbility))
            {
                if (BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.ActiveReceivePass, passTargetCharacter, null, null, true))
                {
                    // パスカットの必殺技じゃないけど、タイミング的にはパスカットと同じタイミングなので. 紛らわしくてすみません.
                    battleResult.AddReplaceDigest(BattleConst.DigestTiming.PassCut, passTargetCharacter, activeOnReceivePassAbility);
                }
            }
            
            battleResult.MatchUpResult = BattleConst.MatchUpResult.Success;
            battleResult.NextBallOwnerId = passTargetCharacter.id;
            offenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Pass, true);
        }

        private static BattleConst.BallInterferenceType GetPassCutResult(BattleCharacterModel defenceCharacter)
        {
            // 一旦キメだけどスキルの影響とか発生してきそうなのでラップ
            // 60%の確率でルーズボール, 40%でボール奪取
            var passCutCatchRateUp = 1.0f + defenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffPassCatchRateUp);
            var isCatched = LotteryResultByRate(BattleConst.AdjustableValuePassCutCatchRate * passCutCatchRateUp);
            return isCatched ? BattleConst.BallInterferenceType.Catched : BattleConst.BallInterferenceType.Blocked;
        }

        private static void JudgeShootResult(
            BattleConst.MatchUpActionDetailType actionDetailType, BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter,
            List<BattleCharacterModel> offenceRoundCharacters, List<BattleCharacterModel> defenceRoundCharacters,
            List<BattleCharacterModel> offenceTeamCharacters, List<BattleCharacterModel> defenceTeamCharacters,
            ref BattleMatchUpResult battleResult, BattleBluelockManModel bluelockMan, int distanceToGoal, bool isNicePass = false, BattleCharacterModel assistCharacter = null)
        {
            // シュートをうつプレイヤーのスタミナ消費
            AddChangedStaminaValue(offenceCharacter, BattleConst.AdjustableValueConsumeStaminaValueForShoot, ref battleResult);
            
            battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeShoot);

            var isSuccessShootBlock = false;
            var shootBlockType = BattleConst.BallInterferenceType.None;
            var denominator = offenceCharacter.GetCurrentWise() + offenceCharacter.GetCurrentKick() / BattleConst.AdjustableValueTechnicCoefficientToDodgeShootBlock;
            var markCharacter = offenceCharacter.MarkCharacter;
            var sortedDefence = defenceRoundCharacters.OrderByDescending(character => character == markCharacter).ToList();
            var blockSuccessRateAddition = 0.0f;
            switch (actionDetailType)
            {
                case BattleConst.MatchUpActionDetailType.Type3: // NiceCourse
                    blockSuccessRateAddition = -0.3f;
                    break;
                case BattleConst.MatchUpActionDetailType.Type4: // BadCourse
                    blockSuccessRateAddition = 0.3f;
                    break;
            }

            foreach (var character in sortedDefence)
            {
                // 突破済みならシュートブロック判定はなし.
                if (character.ClearedNumOnRound > 0)
                {
                    continue;
                }
                
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnBeforeShootBlock, character, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);

                // シュートブロックにトライしたプレイヤーのスタミナ消費(成否関係なし)
                AddChangedStaminaValue(character, BattleConst.AdjustableValueConsumeStaminaValueForShootBlock, ref battleResult);
                
                var shootBlockSuccessRatio = BigValue.RatioCalculation(character.GetCurrentWise(), denominator);
                BigValue requestValue = new BigValue(shootBlockSuccessRatio * BigValue.DefaultRateValue);
                BigValue requestMin = new BigValue(BattleConst.AdjustableValueRequiredMinWiseValueForShootBlock * BigValue.DefaultRateValue);
                BigValue requestMax = new BigValue(BattleConst.AdjustableValueRequiredMaxWiseValueForShootBlock * BigValue.DefaultRateValue);
                float convertedShootBlockSuccessRatio = ConvertRequestValue(requestValue, requestMin, requestMax, BattleConst.AdjustableValueMinShootBlockRate, BattleConst.AdjustableValueMaxShootBlockRate);
                
                if (character == markCharacter)
                {
                    convertedShootBlockSuccessRatio += BattleConst.AdjustableValueShootBlockSuccessRateForMarkCharacter;
                }

                var shootBlockSuccessRateUp = character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffShootBlockSuccessRateUp);
                convertedShootBlockSuccessRatio += shootBlockSuccessRateUp;
                convertedShootBlockSuccessRatio += blockSuccessRateAddition;

                isSuccessShootBlock = LotteryResultByRate(convertedShootBlockSuccessRatio);
                character.Stats.AddActivityStat(BattleCharacterStatModel.StatType.ShootBlock, isSuccessShootBlock);
                if (isSuccessShootBlock)
                {
                    BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSuccessShootBlock, character, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);
                    
                    battleResult.DefenceActionCharacterId = character.id;
                    shootBlockType = GetShootBlockInterferenceType(offenceCharacter, character);
                    battleResult.ShootBlockType = shootBlockType;
                    if (shootBlockType == BattleConst.BallInterferenceType.Blocked)
                    {
                        offenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Shoot, false);
                        assistCharacter?.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Cross, false);
                        SetLooseBallResult(offenceCharacter, defenceCharacter, offenceTeamCharacters, defenceTeamCharacters, ref battleResult, BattleConst.LooseBallReasonType.ShootBlockedByPlayer);
                        return;
                    }

                    // シュートブロックに入れるのは一人のみ.
                    break;
                }
            }

            var shootResult = GetShootResult(offenceCharacter, isNicePass, actionDetailType, isSuccessShootBlock, shootBlockType, bluelockMan, distanceToGoal);
            offenceCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Shoot, shootResult);
            assistCharacter?.Stats.AddActivityStat(BattleCharacterStatModel.StatType.Cross, shootResult);
            if (shootResult)
            {
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnSuccessShoot, offenceCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);

                battleResult.ScoredCharacterId = offenceCharacter.id;
                battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeGoal);
                battleResult.MatchUpResult = BattleConst.MatchUpResult.Success;
                battleResult.IsSideChanged = true;
                battleResult.IsResetRound = true;
            }
            else
            {
                var shootFailedResult = GetShootFailedInterferenceType(offenceCharacter);
                if (shootFailedResult == BattleConst.BallInterferenceType.Catched)
                {
                    battleResult.IsSideChanged = true;
                    battleResult.IsResetRound = true;
                    battleResult.MatchUpResult = BattleConst.MatchUpResult.Failed;
                    battleResult.BallInterferenceType = BattleConst.BallInterferenceType.Catched;
                    battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeKeeperThrow);
                    battleResult.NextBallOwnerId = BattleDataMediator.Instance.GetTeamDeck(defenceCharacter.Side).FirstOrDefault(character => character.IsAceCharacter)?.id ?? -1;
                }
                else
                {
                    // TODO 仕様書に記載ないので一旦50%キメでキーパーパンチングかゴールポスト
                    var reason = LotteryResultByRate(0.5f) ? BattleConst.LooseBallReasonType.ShootBlockedByKeeper : BattleConst.LooseBallReasonType.ShootBlockedByGoalpost;
                    SetLooseBallResult(offenceCharacter, defenceCharacter, offenceTeamCharacters, defenceTeamCharacters, ref battleResult, reason);
                    battleResult.BallInterferenceType = reason == BattleConst.LooseBallReasonType.ShootBlockedByKeeper ? BattleConst.BallInterferenceType.Blocked : BattleConst.BallInterferenceType.HitGoalPost;
                }
            }
        }
        
        private static void JudgeCrossResult(
            BattleConst.MatchUpActionDetailType actionDetailType, BattleCharacterModel offenceCharacter, BattleCharacterModel targetCharacter, BattleCharacterModel defenceCharacter,
            List<BattleCharacterModel> offenceRoundCharacters, List<BattleCharacterModel> defenceRoundCharacters,
            List<BattleCharacterModel> offenceTeamCharacters, List<BattleCharacterModel> defenceTeamCharacters,
            ref BattleMatchUpResult battleResult, BattleBluelockManModel blueLockMan, int distanceToGoal)
        {
            // クロスを出すプレイヤーのスタミナ消費
            AddChangedStaminaValue(offenceCharacter, BattleConst.AdjustableValueConsumeStaminaValueForCross, ref battleResult);
            
            battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeCross);
            battleResult.TargetCharacterId = targetCharacter.id;

            battleResult.IsNicePass = JudgeIsNicePass(offenceCharacter, defenceRoundCharacters);
            JudgeShootResult(actionDetailType, targetCharacter, defenceCharacter,
                offenceRoundCharacters, defenceRoundCharacters, offenceTeamCharacters, defenceTeamCharacters,
                ref battleResult, blueLockMan, distanceToGoal, battleResult.IsNicePass, offenceCharacter);
        }
        
        private static bool JudgeIsNicePass(BattleCharacterModel offenceCharacter, List<BattleCharacterModel> defenceRoundCharacters)
        {
            var denominator = offenceCharacter.GetCurrentWise() + offenceCharacter.GetCurrentKick() / BattleConst.AdjustableValueKickCoefficientForNicePass;
            var defenceAverageWise = defenceRoundCharacters.Sum(character => character.GetCurrentWise()) / defenceRoundCharacters.Count;
            var nicePassRate = BigValue.RatioCalculation(defenceAverageWise, denominator);
            BigValue requestValue = new BigValue(nicePassRate * BigValue.DefaultRateValue);
            BigValue requestMin = new BigValue(BattleConst.AdjustableValueRequiredMinValueForNicePass * BigValue.DefaultRateValue);
            BigValue requestMax = new BigValue(BattleConst.AdjustableValueRequiredMaxValueForNicePass * BigValue.DefaultRateValue);
            var convertedNicePassRate = ConvertRequestValue(requestValue, requestMin, requestMax, BattleConst.AdjustableValueMinNicePassActiveRate, BattleConst.AdjustableValueMaxNicePassActiveRate);

            var nicePassRateUpRatio = offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffNicePassSuccessRateUp);
            convertedNicePassRate += nicePassRateUpRatio;
            
            return LotteryResultByRate(convertedNicePassRate);
        }

        private static bool GetShootResult(BattleCharacterModel offenceCharacter, bool isNicePass, BattleConst.MatchUpActionDetailType actionDetailType, bool isShootBlocked, BattleConst.BallInterferenceType shootBlockType, BattleBluelockManModel bluelockMan, int distanceToGoal)
        {
            // ここらへんの定数変更ありそうな気配がするので一旦仮置.(仕様書通りにはしてる.)
            // 係数の扱い, 全部加算してから乗算じゃなくて大丈夫? 結構下方修正されそうだけど.
            var shootPower = offenceCharacter.GetCurrentKick() * GetRandomValue(BattleConst.AdjustableValueShootRandValueMin, BattleConst.AdjustableValueShootRandValueMax);
            var shootPowerRatio = 1.0f;
            if (isShootBlocked)
            {
                switch (shootBlockType)
                {
                    case BattleConst.BallInterferenceType.None:
                        shootPowerRatio -= 0.05f;
                        break;
                    case BattleConst.BallInterferenceType.Touched:
                        shootPowerRatio -= 0.3f;
                        break;
                }
            }

            // 適正シュートレンジ外だったらマイナス補正
            var isIgnoreShootRangePenaltyActive = offenceCharacter.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffIgnoreShootRangePenalty);
            var isEnoughCloseToShoot = IsEnoughCloseToShoot(offenceCharacter, distanceToGoal);
            if (!isIgnoreShootRangePenaltyActive && !isEnoughCloseToShoot)
            {
                shootPowerRatio -= 0.25f;
            }

            if (isNicePass)
            {
                // すんごい必殺技関連入りそう.
                shootPowerRatio += 0.2f;
            }

            switch (actionDetailType)
            {
                case BattleConst.MatchUpActionDetailType.Type1: // NiceMeet
                    shootPowerRatio += 0.3f;
                    break;
                case BattleConst.MatchUpActionDetailType.Type2: // BadMeet
                    shootPowerRatio -= 0.3f;
                    break;
            }

            shootPower *= shootPowerRatio;
            BigValue requiredMin = new BigValue(bluelockMan.RequestValueMin);
            BigValue requiredMax = new BigValue(bluelockMan.RequestValueMax);
            
            // 対人戦ではrequestValueMin/Maxをそれぞれ係数として扱い, 守備側デッキの賢さ平均を基準値として扱う
            if (BattleDataMediator.Instance.BattleType == BattleConst.BattleType.VersusPlayerBattle ||
                BattleDataMediator.Instance.BattleType == BattleConst.BattleType.ExecLeagueMatch ||
                BattleDataMediator.Instance.BattleType == BattleConst.BattleType.MagicOnionServerBattle)
            {
                var deck = BattleDataMediator.Instance.GetTeamDeck(GetOtherSide(offenceCharacter.Side));
                var maxWise = deck.Max(chara => chara.GetCurrentWise());
                requiredMin = maxWise * requiredMin / 10000.0f;
                requiredMax = maxWise * requiredMax / 10000.0f;
            }
            
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
            CruFramework.Logger.Log($"シュート要求値MinMax: {requiredMax} : {requiredMax}");
#endif

            var shootSuccessRate = ConvertRequestValue(shootPower, requiredMin, requiredMax,
                BattleConst.AdjustableValueShootSuccessRateMin, BattleConst.AdjustableValueShootSuccessRateMax);

            var shootSuccessRateUp = offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffShootSuccessRateUp);
            shootSuccessRate += shootSuccessRateUp;
            var shootResult = LotteryResultByRate(shootSuccessRate);
            
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceSuccessMatchUp)
            {
                shootResult = true;
            }else if (BattleDataMediator.Instance.ForceFailMatchUp)
            {
                shootResult = false;
            }
#endif
            return shootResult;
        }

        private static BattleConst.BallInterferenceType GetShootBlockInterferenceType(BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter)
        {
            /*
	        1.ブロック成功判定
		        DFのスピード/2+テクニック/2 VS OFのキック で割合算出
		        要求値MIN:MAX　0.5:2
		        確率変動MIN:MAX　10%:60%
	        2.かすめる判定
		        DFのスピード/2+テクニック/2 VS OFのキック で割合算出
		        要求値MIN:MAX　0.5:2
		        確率変動MIN:MAX　20%:100%
		    */
            var blockRateUpRatio = 1.0f + defenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffShootBlockBlockRateUp);
            var offenceShootPower = offenceCharacter.GetCurrentKick();
            var defenceBlockPower = (defenceCharacter.GetCurrentSpeed() + defenceCharacter.GetCurrentTechnique()) / 2.0f;
            var shootBlockRatio = BigValue.RatioCalculation( defenceBlockPower, offenceShootPower);

            BigValue shootBlockRequestValue = new BigValue(shootBlockRatio * BigValue.DefaultRateValue);
            BigValue shootBlockRequestMin = new BigValue(BattleConst.AdjustableValueShootBlockInterferenceTypeRequestMin * BigValue.DefaultRateValue);
            BigValue shootBlockRequestMax = new BigValue(BattleConst.AdjustableValueShootBlockInterferenceTypeRequestMax * BigValue.DefaultRateValue);
            float shootBlockSuccessRate = ConvertRequestValue(shootBlockRequestValue, shootBlockRequestMin, shootBlockRequestMax, BattleConst.AdjustableValueShootBlockInterferenceTypeBlockedRateMin, BattleConst.AdjustableValueShootBlockInterferenceTypeBlockedRateMax) * blockRateUpRatio;
            var isSuccessShootBlock = GetRandomValue(0.0f, 1.0f) <= shootBlockSuccessRate;
            if (isSuccessShootBlock)
            {
                return BattleConst.BallInterferenceType.Blocked;
            }

            BigValue shootTouchRequestValue = new BigValue(shootBlockRatio * BigValue.DefaultRateValue);
            BigValue shootTouchRequestMin = new BigValue(BattleConst.AdjustableValueShootBlockInterferenceTypeRequestMin * BigValue.DefaultRateValue);
            BigValue shootTouchRequestMax = new BigValue(BattleConst.AdjustableValueShootBlockInterferenceTypeRequestMax * BigValue.DefaultRateValue);
            float shootTouchSuccessRate = ConvertRequestValue(shootTouchRequestValue, shootTouchRequestMin, shootTouchRequestMax, BattleConst.AdjustableValueShootBlockInterferenceTypeTouchRateMin, BattleConst.AdjustableValueShootBlockInterferenceTypeTouchRateMax);
            
            var isSuccessShootTouch = GetRandomValue(0.0f, 1.0f) <= shootTouchSuccessRate;
            if (isSuccessShootTouch)
            {
                return BattleConst.BallInterferenceType.Touched;
            }

            return BattleConst.BallInterferenceType.None;
        }

        private static BattleConst.BallInterferenceType GetShootFailedInterferenceType(BattleCharacterModel offenceCharacter)
        {
            // 一旦キメだけどスキルの影響とか発生してきそうなのでラップ
            // 30% BLMがキャッチしてスローインから開始, 70% ポストにあたったり弾かれたりでセカンドボール
            var randValue = GetRandomValue(0, 100);
            var ret = randValue switch
            {
                < 30 => BattleConst.BallInterferenceType.Catched,
                _ => BattleConst.BallInterferenceType.Blocked,
            };

            return ret;
        }
        
        private static void SetLooseBallResult(BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter, List<BattleCharacterModel> offenceTeamCharacters, List<BattleCharacterModel> defenceTeamCharacters, ref BattleMatchUpResult battleResult, BattleConst.LooseBallReasonType reason)
        {
            // セカンドボール基礎時間経過
            battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeLooseBall);

            // スローイン抽選
            if (LotteryResultByRate(BattleConst.DefaultThrowInWeight))
            {
                battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeBallOut);
                battleResult.ChangedGameTimeValue += GetAddGameTimeValue(BattleConst.AdjustableValueAddGameTimeThrowIn);
                
                // ゴールポストに弾かれてスローインになったときのみ攻守交代してスローインとなる
                battleResult.MatchUpResult = BattleConst.MatchUpResult.BallOut;
                battleResult.NextBallPosition = GetNextLooseBallPosition(battleResult.NextBallPosition, offenceCharacter, reason);
                battleResult.IsSideChanged = reason == BattleConst.LooseBallReasonType.ShootBlockedByGoalpost;
                // スローインの場合は該当チームのエースからにする
                if (battleResult.IsSideChanged)
                {
                    battleResult.NextBallOwnerId = BattleDataMediator.Instance.GetTeamDeck(defenceCharacter.Side).FirstOrDefault(character => character.IsAceCharacter)?.id ?? -1;
                }
                else
                {
                    battleResult.NextBallOwnerId = BattleDataMediator.Instance.GetTeamDeck(offenceCharacter.Side).FirstOrDefault(character => character.IsAceCharacter)?.id ?? -1;
                }
            }
            else
            {
                var looseBallJoinCharacters = GetJoinLooseBallCompetitionCharacterIds(offenceCharacter, defenceCharacter, offenceTeamCharacters, defenceTeamCharacters, ref battleResult);
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
                CruFramework.Logger.Log($"セカンドボール奪い合い(抽選結果) 左: ${string.Join(",", looseBallJoinCharacters.Where(c => c.Side == BattleConst.TeamSide.Left).Select(c => c.id))}");
                CruFramework.Logger.Log($"セカンドボール奪い合い(抽選結果) 右: ${string.Join(",",looseBallJoinCharacters.Where(c => c.Side == BattleConst.TeamSide.Right).Select(c => c.id))}");
#endif
                battleResult.JoinLooseBallCompetitionCharacterIds = looseBallJoinCharacters.Select(character => character.id).ToList();
                foreach (var joinCharacter in looseBallJoinCharacters)
                {
                    BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnLooseBall, joinCharacter, battleResult.BeforeAbilityLogs, battleResult.AfterAbilityLogs);

                    var activeOnLooseBallAbility = joinCharacter.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.timing == (long) BattleConst.AbilityEvaluateTimingType.ActiveOnLooseBall);
                    if (activeOnLooseBallAbility != null &&
                        // 結果確定後のアクティブはスワイプの如何に関わらずLvは+1判定. 結果を変えようがないのでスワイプしようがしまいが関係ないので.
                        LotteryResultByRate(activeOnLooseBallAbility.GetInvokeRate(activeOnLooseBallAbility.AbilityLevel + 1) * joinCharacter.AbilityInvokeRateCoefficient) &&
                        BattleAbilityLogic.EvaluateAbilityCondition(joinCharacter, activeOnLooseBallAbility))
                    {
                        if (BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.ActiveOnLooseBall, joinCharacter, null, null, true))
                        {
                            battleResult.AddReplaceDigest(BattleConst.DigestTiming.SecondBall, joinCharacter, activeOnLooseBallAbility);
                        }
                    }
                }
                
                battleResult.NextBallOwnerId = GetLooseBallCompetitionWinnerCharacterId(looseBallJoinCharacters);
                battleResult.MatchUpResult = BattleConst.MatchUpResult.LooseBall;
                battleResult.NextBallPosition = GetNextLooseBallPosition(battleResult.NextBallPosition, offenceCharacter, reason);
                var nextBallOwnerId = battleResult.NextBallOwnerId;
                var nextBallOwner = offenceTeamCharacters.FirstOrDefault(character => character.id == nextBallOwnerId);
                if (nextBallOwner == null)
                {
                    nextBallOwner = defenceTeamCharacters.FirstOrDefault(character => character.id == nextBallOwnerId);
                }
                
                foreach (var joinCharacter in looseBallJoinCharacters)
                {
                    joinCharacter.Stats.AddActivityStat(BattleCharacterStatModel.StatType.SecondBall, joinCharacter == nextBallOwner);
                }

                var isSideChanged = offenceCharacter.Side != nextBallOwner.Side;
                battleResult.IsSideChanged = isSideChanged;
            }

            // セカンドボールになった時点でラウンドリセット
            battleResult.IsResetRound = true;
        }

        private static List<BattleCharacterModel> GetJoinLooseBallCompetitionCharacterIds(BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter,
            List<BattleCharacterModel> offenceCharacters, List<BattleCharacterModel> defenceCharacters, ref BattleMatchUpResult battleMatchUpResult)
        {
            var lottedCharacters = new List<BattleCharacterModel>();
            var lottedOFTeamCount = 0;
            var lottedDFTeamCount = 0;
            var offenceSide = offenceCharacter.Side;
            var participationCharacters = new List<BattleCharacterModel>();
            var targetCharacters = new List<BattleCharacterModel>();
            List<long> executedForceNonParticipationCharacterIds = new List<long>();

#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
            CruFramework.Logger.Log($"セカンドボール奪い合い(参加者候補) 攻め: {string.Join(",", offenceCharacters.Select(c => c.id))}");
            CruFramework.Logger.Log($"セカンドボール奪い合い(参加者候補) 守り: {string.Join(",", defenceCharacters.Select(c => c.id))}");
            
            CruFramework.Logger.Log($"セカンドボール奪い合い(強制不参加者) 攻め: {string.Join(",", offenceCharacters.Where(character => character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceNonParticipation)).Select(c => c.id))}");
            CruFramework.Logger.Log($"セカンドボール奪い合い(強制不参加者) 守り: {string.Join(",", defenceCharacters.Where(character => character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceNonParticipation)).Select(c => c.id))}");
            
            CruFramework.Logger.Log($"セカンドボール奪い合い(強制参加者) 攻め: {string.Join(",", offenceCharacters.Where(character => character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceParticipation)).Select(c => c.id))}");
            CruFramework.Logger.Log($"セカンドボール奪い合い(強制参加者) 守り: {string.Join(",", offenceCharacters.Where(character => character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceParticipation)).Select(c => c.id))}");
#endif
            
            // 強制参加のアビリティが発動している選手を取得し並び替え
            participationCharacters.AddRange(offenceCharacters.Where(character => character != offenceCharacter && character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceParticipation)));
            participationCharacters.AddRange(defenceCharacters.Where(character => character != defenceCharacter && character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceParticipation)));
            participationCharacters = participationCharacters.OrderBy(_ => GetRandomIntValue()).ToList();
            
            // 強制参加の選手抽選
            foreach (BattleCharacterModel character in participationCharacters)
            {
                // 4人以上になったら終了.
                if (lottedOFTeamCount + lottedDFTeamCount >= BattleConst.MaxLooseBallMatchUpMemberNum)
                {
                    break;
                }
                
                bool isOffence = character.Side == offenceSide;
                if ((isOffence ? lottedOFTeamCount : lottedDFTeamCount) < BattleConst.MaxLooseBallMatchUpMemberNum - 1)
                {
                    int forceParticipationAbilityCount = character.GetSpecificActiveAbilityEffectTypeActiveCount(BattleConst.AbilityEffectType.BuffLooseBallForceParticipation);
                    int forceNonParticipationAbilityCount = character.GetSpecificActiveAbilityEffectTypeActiveCount(BattleConst.AbilityEffectType.BuffLooseBallForceNonParticipation);
                    // 強制不参加 - 強制参加　で個数分打ち消し
                    int nonForceAbilityRemainingCount = forceNonParticipationAbilityCount - forceParticipationAbilityCount;
                    
                    // 強制不参加のこりが0以下であれば発動ずみ
                    if (nonForceAbilityRemainingCount <= 0)
                    {
                        executedForceNonParticipationCharacterIds.Add(character.id);
                    }
                    
                    // 強制不参加のこりが0以上であれば打ち消しで強制参加しない
                    if (nonForceAbilityRemainingCount >= 0)
                    {
                        continue;
                    }
                    
                    lottedCharacters.Add(character);
                    if (isOffence)
                    {
                        lottedOFTeamCount++;
                    }
                    else
                    {
                        lottedDFTeamCount++;
                    }
                }
            }

#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
            CruFramework.Logger.Log($"セカンドボール奪い合い(強制参加者 結果) ${string.Join(",", lottedCharacters.Select(c => c.id))}");
#endif
            
            // スキル適応して選ばれた選手
            List<BattleCharacterModel> offenceJoinCharacters = offenceCharacters.Where(character => character != offenceCharacter && !lottedCharacters.Contains(character) &&
                                                                                                    // 強制不参加が発動していない
                                                                                                    (!character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceNonParticipation) ||
                                                                                                     // または発動しているが効果使用済み
                                                                                                     executedForceNonParticipationCharacterIds.Contains(character.id))).ToList(); 
            List<BattleCharacterModel> defenceJoinCharacters = defenceCharacters.Where(character => character != defenceCharacter && !lottedCharacters.Contains(character) &&
                                                                                                    // 強制不参加が発動していない
                                                                                                    (!character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffLooseBallForceNonParticipation) ||
                                                                                                     // または発動しているが効果使用済み
                                                                                                     executedForceNonParticipationCharacterIds.Contains(character.id))).ToList();
            
            // 絶対に両方のチームから一人は出るようになるため, 最大参加人数-1を対象とする.
            // スキル効果を適応して選ばれた選手の数+強制参加の選手が0人の場合はランダムで.
            targetCharacters.AddRange((offenceCharacters.Count + lottedOFTeamCount > 0 ? offenceJoinCharacters : offenceCharacters.Where(character => character != offenceCharacter))
                .OrderBy(_ => GetRandomIntValue()).Take(BattleConst.MaxLooseBallMatchUpMemberNum - 1));
            targetCharacters.AddRange((defenceCharacters.Count + lottedDFTeamCount > 0 ? defenceJoinCharacters : defenceCharacters.Where(character => character != defenceCharacter))
                .OrderBy(_ => GetRandomIntValue()).Take(BattleConst.MaxLooseBallMatchUpMemberNum - 1));
            targetCharacters = targetCharacters.OrderBy(_ => GetRandomIntValue()).ToList();
            foreach (var character in targetCharacters)
            {
                // 4人以上になったら終了.
                if (lottedOFTeamCount + lottedDFTeamCount >= BattleConst.MaxLooseBallMatchUpMemberNum)
                {
                    break;
                }
                
                var isOffence = character.Side == offenceSide;
                int joinCompetitionRate = LotteryJoinLooseBallCompetition(isOffence ? lottedOFTeamCount : lottedDFTeamCount);
                int lotteryValue = GetRandomValue(0, 100);
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
                CruFramework.Logger.Log($"セカンドボール奪い合い(通常参加者情報)" +
                                            $"\n 攻守:{character.Side.ToString()}" +
                                            $"\n キャラ情報:{character.id} : {character.Name}" +
                                            $"\n 参加人数に対する重み:{joinCompetitionRate}" +
                                            $"\n 抽選値:{lotteryValue}");
#endif
                if (lotteryValue <= joinCompetitionRate)
                {
                    lottedCharacters.Add(character);
                    
                    if (isOffence)
                    {
                        lottedOFTeamCount++;
                    }
                    else
                    {
                        lottedDFTeamCount++;
                    }
                }
            }

            AddChangedStaminaValues(lottedCharacters, BattleConst.AdjustableValueConsumeStaminaValueForLooseBall, ref battleMatchUpResult);
            return lottedCharacters.ToList();
        }

        private static int LotteryJoinLooseBallCompetition(int alreadyJoinedCountInTeam)
        {
            // スキル周りのなんかはありそう…
            var ratio = 0;
            switch (alreadyJoinedCountInTeam)
            {
                case 0:
                    ratio = 100;
                    break;
                case 1:
                    ratio = 50;
                    break;
                case 2:
                    ratio = 20;
                    break;
                default:
                    ratio = 0;
                    break;
            }

            return ratio;
        }

        private static long GetLooseBallCompetitionWinnerCharacterId(List<BattleCharacterModel> participants)
        {
            var abilityWeights = new int[participants.Count];
            var weights = new BigValue[participants.Count];
            for (var i = 0; i < participants.Count; i++)
            {
                abilityWeights[i] = (int)participants[i].GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffLooseBallCatchRateUp);
                // 誤差レベルになると思うのでまあ適当にint丸めで.
                // 全てのキャラクターのweightsが0になると後続でしぬのでMin1に
                // まーーデバッグ機能で変なキャラ作らないと起こらないとは思いますが.
                weights[i] = BigValue.Max(participants[i].GetCurrentPhysical(), new BigValue(1));
            }

            var index = 0;
            if (abilityWeights.Any(weight => weight > 0))
            {
                index = GetRandomIndex(abilityWeights);
                return participants[index].id;
            }
            
            
            index = GetRandomIndex(weights);
            return participants[index].id;
        }
        
        private static float GetMinMatchUpCoefficient(BattleConst.StatusParamType statusParamType, BattleCharacterModel character)
        {
            var mainParameterValue = character.GetCurrentParameter(statusParamType);
            var subParameterValue = character.GetCurrentParameter(BattleConst.GetMatchUpSubParameter(statusParamType));

            // failsafe
            if (mainParameterValue.Equals(0.0f))
            {
                return 0.0f;
            }

            double subMainRatio = BigValue.RatioCalculation(subParameterValue, mainParameterValue);
            BigValue requestValue = new BigValue(subMainRatio * BigValue.DefaultRateValue);
            BigValue requestMin = new BigValue(BattleConst.AdjustableValueRequiredSubParamMinValueForMatchUp * BigValue.DefaultRateValue);
            BigValue requestMax = new BigValue(BattleConst.AdjustableValueRequiredSubParamMaxValueForMatchUp * BigValue.DefaultRateValue);
            return ConvertRequestValue(requestValue, requestMin, requestMax, BattleConst.AdjustableValueMatchUpParamMinRandValueMin, BattleConst.AdjustableValueMatchUpParamMinRandValueMax);
        }
        
        private static float GetMaxMatchUpCoefficient()
        {
            // どうせスキルとかで変わるだろうからラップしとく.
            return BattleConst.AdjustableValueMatchUpParamRandValueMax;
        }

        private static BigValue GetMatchUpParameterValue(BattleConst.StatusParamType statusParamType, BattleCharacterModel character, BattleConst.MatchUpActionDetailType actionDetailType)
        {
            var parameter = character.GetCurrentParameter(statusParamType);
            var matchUpCoefMinUpRate = 1.0f + character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffMatchUpPowerRandMinUp);
            var matchUpCoefMaxUpRate = 1.0f + character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffMatchUpPowerRandMaxUp);
            var matchUpCoefMin = GetMinMatchUpCoefficient(statusParamType, character) * matchUpCoefMinUpRate;
            var matchUpCoefMax = GetMaxMatchUpCoefficient() * matchUpCoefMaxUpRate;
            
            var randomMatchUpCoefficient = GetRandomValue(matchUpCoefMin, matchUpCoefMax);
            var resultValue = parameter * randomMatchUpCoefficient;

            var matchUpValueRatioUp = 1.0f + character.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffThroughMatchUpValueUp);
            switch (actionDetailType)
            {
                case BattleConst.MatchUpActionDetailType.Type1: // Nice
                    matchUpValueRatioUp += 0.2f;
                    break;
                case BattleConst.MatchUpActionDetailType.Type2: // Bad
                    matchUpValueRatioUp -= 0.2f;
                    break;
                case BattleConst.MatchUpActionDetailType.Type3: // TooBad
                    matchUpValueRatioUp -= 0.4f;
                    break;
            }
            resultValue *= matchUpValueRatioUp;

            return resultValue;
        }

        private static void AddChangedStaminaValue(BattleCharacterModel character, float changedValueCoefficient, ref BattleMatchUpResult result)
        {
            if (character == null)
            {
                return;
            }
            
            // ラップ…しておくけどパッシブでスタミナ変動量変化とかあるからパラメータ追加する必要ある.
            var changedValue = BigValue.CalculationCeiling(BattleDataMediator.Instance.AverageMaxStamina, changedValueCoefficient);
            result.AddChangedStaminaValue(character.id, changedValue);
        }
        
        private static void AddChangedStaminaValues(List<BattleCharacterModel> characters, float changedValueCoefficient, ref BattleMatchUpResult result)
        {
            // ラップ…しておくけどパッシブでスタミナ変動量変化とかあるからパラメータ追加する必要ある.
            foreach (var character in characters)
            {
                AddChangedStaminaValue(character, changedValueCoefficient, ref result);
            }
        }

        // 納得感的に「敵味方のラウンド参加者の中から一番スピードが高いやつに対する相対」で出してるけどおっけーかな.(?)
        public static int GetNextBallPosition(int currentBallPosition, BattleCharacterModel offenceCharacter, int min, int max,
            List<BattleCharacterModel> offenceRoundCharacters = null, List<BattleCharacterModel> defenceRoundCharacters = null)
        {
            // ラップ…しておくけどパッシブでボール移動量変化とかありそうだからパラメータ追加する必要あるかも.
            var moveDistance = 0;
            if (offenceRoundCharacters != null && defenceRoundCharacters != null)
            {
                var offenceMax = offenceRoundCharacters.Max(character => character.GetCurrentSpeed());
                var defenceMax = defenceRoundCharacters.Max(character => character.GetCurrentSpeed());
                var maxSpeed = BigValue.Max(offenceMax, defenceMax);
                double speedRate = BigValue.RatioCalculation(offenceCharacter.GetCurrentSpeed(), maxSpeed);
                moveDistance = Math.Clamp((int)Math.Round(speedRate * max), min, max);
            }
            else
            {
                var range = max - min;
                moveDistance = (int)Math.Round(GetRandomValue(0.0f, 1.0f) * range + min);
            }

            // ランダム振れ幅
            moveDistance += GetRandomValue(-5, 5);
            // Leftを基準としているので(右方向正), 右サイドのプレイヤーからの移動だったら正負反転
            if (offenceCharacter.Side == BattleConst.TeamSide.Right)
            {
                moveDistance *= -1;
            }

            return Math.Clamp(currentBallPosition + moveDistance, 1, BattleConst.FieldSize);
        }

        public static int GetNextLooseBallPosition(int currentBallPosition, BattleCharacterModel offenceCharacter, BattleConst.LooseBallReasonType reason)
        {
            var randMin = 0;
            var randMax = 0;
            switch (reason)
            {
                // パスカットと突破失敗からのセカンドボールでは移動なし.
                case BattleConst.LooseBallReasonType.FailedRunThrough:
                    return currentBallPosition;
                case BattleConst.LooseBallReasonType.PassCut:
                    return currentBallPosition;
                case BattleConst.LooseBallReasonType.ShootBlockedByPlayer:
                    randMin = BattleConst.AdjustableValueMinMoveDistanceForShootBlockByPlayer;
                    randMax = BattleConst.AdjustableValueMaxMoveDistanceForShootBlockByPlayer;
                    break;
                case BattleConst.LooseBallReasonType.ShootBlockedByKeeper:
                    // キーパーにブロックされた場合は基準点がゴールになるため
                    currentBallPosition = offenceCharacter.Side == BattleConst.TeamSide.Left ? BattleConst.FieldSize : 0;
                    randMin = BattleConst.AdjustableValueMinMoveDistanceForShootBlockByKeeper;
                    randMax = BattleConst.AdjustableValueMaxMoveDistanceForShootBlockByKeeper;
                    break;
                case BattleConst.LooseBallReasonType.ShootBlockedByGoalpost:
                    // TODO 仕様書に定義ないので一旦キーパーパンチと同じにしておく.
                    // キーパーにブロックされた場合は基準点がゴールになるため
                    currentBallPosition = offenceCharacter.Side == BattleConst.TeamSide.Left ? BattleConst.FieldSize : 0;
                    randMin = BattleConst.AdjustableValueMinMoveDistanceForShootBlockByKeeper;
                    randMax = BattleConst.AdjustableValueMaxMoveDistanceForShootBlockByKeeper;
                    break;
            }
            
            var range = randMax - randMin;
            var moveDistance = (int)Math.Round(GetRandomValue(0.0f, 1.0f) * range + randMin);
            // Leftを基準としているので(右方向正), 右サイドのプレイヤーからの移動だったら正負反転
            if (offenceCharacter.Side == BattleConst.TeamSide.Right)
            {
                moveDistance *= -1;
            }

            return Math.Clamp(currentBallPosition + moveDistance, 1, BattleConst.FieldSize);
        }

        private static int GetMaxClearedNum(List<BattleCharacterModel> characters)
        {
            var ret = 1;
            foreach (var character in characters)
            {
                if (character.ClearedNumOnRound > ret)
                {
                    ret = character.ClearedNumOnRound;
                }
            }

            return ret;
        }

        private static float GetAddGameTimeValue(int defaultValue)
        {
            return defaultValue * GetRandomValue(0.9f, 1.1f);
        }

        public static List<BattleCharacterModel> GetPassableCharacters(BattleCharacterModel offenceCharacter, List<BattleCharacterModel> offenceRoundCharacters)
        {
            return offenceRoundCharacters.Where(character => character != offenceCharacter && character.ClearedNumOnRound < 0).ToList();
        }

        public static bool CanShoot(BattleCharacterModel offenceCharacter, int distanceToGoal)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.IgnoreShootRange)
            {
                return true;
            }
#endif
            return offenceCharacter.GetCurrentShootRange() >= distanceToGoal;
        }

        public static bool IsEnoughCloseToShoot(BattleCharacterModel offenceCharacter, int distanceToGoal)
        {
            return distanceToGoal <= offenceCharacter.GetCurrentShootRange() * BattleConst.AdjustableValueAppropriateShootRangeDistanceCoefficient;
        }
        
        public static List<BattleCharacterModel> GetCrossableCharacters(BattleCharacterModel offenceCharacter, List<BattleCharacterModel> offenceRoundCharacters, int distanceToGoal)
        {
            return offenceRoundCharacters.Where(character => character != offenceCharacter && character.ClearedNumOnRound < 0 && CanShoot(character, distanceToGoal)).ToList();
        }
        
        /// <summary>パス対象選出</summary>
        /// <param name="passableCharacterList">ステータス順で並び替えたものを設定</param>
        private static List<BattleCharacterModel> GetLotteryPassableCharacters(IEnumerable<BattleCharacterModel> passableCharacterList)
        {
            // 選出率アップ発動回数
            int executedRateUpCount = 0;
            
            List<BattleCharacterModel> passableCharacters = passableCharacterList.ToList();
            List<long> executedRateDownSkill = new List<long>();
            List<long> executedRateUpSkill = new List<long>();
            
            // アップとダウンの打ち消し
            foreach (BattleCharacterModel character in passableCharacterList.Where(character => character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffPassableRateDown) ||
                                                                                                character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffPassableRateUp)))
            {
                int rateUpCount = character.GetSpecificActiveAbilityEffectTypeActiveCount(BattleConst.AbilityEffectType.BuffPassableRateUp);
                int rateDownCount = character.GetSpecificActiveAbilityEffectTypeActiveCount(BattleConst.AbilityEffectType.BuffPassableRateDown);
                // 選出率アップと選出率ダウンが同時に発動している場合打ち消し
                switch (rateUpCount - rateDownCount)
                {
                    // 選出率アップが多く発動している場合、ダウン発動済み
                    case > 0:
                        executedRateDownSkill.Add(character.id);
                        break;
                    // 選出率ダウンが多く発動している場合
                    case < 0:
                        executedRateUpSkill.Add(character.id);
                        break;
                    // 両方同数の場合
                    default:
                        executedRateDownSkill.Add(character.id);
                        executedRateUpSkill.Add(character.id);
                        break;
                }
            }


            for (int i = 0; i < passableCharacters.Count; i++)
            {
                if (i == 0 && passableCharacters[i].IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffPassableRateDown) 
                           && !executedRateDownSkill.Contains(passableCharacters[i].id) && passableCharacters.Count > i + 1)
                {
                    executedRateDownSkill.Add(passableCharacters[i].id);
                    // 選出率ダウンが発動できたら次のキャラと入れ替え
                    (passableCharacters[i], passableCharacters[i + 1]) = (passableCharacters[i + 1], passableCharacters[i]);
                    i--;
                    continue;
                }
                
                // 選出率アップが発動していたら対象にする
                if (passableCharacters[i].IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffPassableRateUp) && !executedRateUpSkill.Contains(passableCharacters[i].id))
                {
                    executedRateUpSkill.Add(passableCharacters[i].id);
                    BattleCharacterModel passableCharacter = passableCharacters[i];
                    passableCharacters.RemoveAt(i);
                    // 発動していたキャラ順に入れ直す
                    passableCharacters.Insert(executedRateUpCount, passableCharacter);
                    executedRateUpCount++;
                }
            }
            
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
            CruFramework.Logger.Log($"パス対象スキル適応前 : {string.Join(",", passableCharacterList.Select(c => c.id))}");
            CruFramework.Logger.Log($"パス対象スキル適応後 : {string.Join(",", passableCharacters.Select(c => c.id))}");
#endif
            
            return passableCharacters;
        }
        
        public static List<BattleMatchUpCommandData> GetMatchUpCommandData(BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter, List<BattleCharacterModel> offenceRoundCharacters, List<BattleCharacterModel> offenceDeck, List<BattleCharacterModel> defenceDeck, int distanceToGoal, int tactics)
        {
            var ret = new List<BattleMatchUpCommandData>();
            var isInShootRange = CanShoot(offenceCharacter, distanceToGoal);
            var isEnoughCloseToShoot = IsEnoughCloseToShoot(offenceCharacter, distanceToGoal);
            var passableCharacters = GetPassableCharacters(offenceCharacter, offenceRoundCharacters);
            var crossableCharacters = GetCrossableCharacters(offenceCharacter, offenceRoundCharacters, distanceToGoal);

            var wise = offenceCharacter.GetCurrentWise();
            var wiseRank = BattleAbilityLogic.GetInListStatusRank(offenceCharacter, offenceDeck, defenceDeck, BattleConst.StatusParamType.Wise);
            
            if (isInShootRange)
            {
                var shootCommand = GetShootCommandData(offenceCharacter, wise, wiseRank, false);
                shootCommand.Index = (byte)ret.Count;
                shootCommand.IsEnoughClose = isEnoughCloseToShoot;
                ret.Add(shootCommand);
            }

            var throughCommand = GetThroughCommandData(offenceCharacter, defenceCharacter, wise, wiseRank);
            throughCommand.Index = (byte)ret.Count;
            ret.Add(throughCommand);

            BattleCharacterModel targetCharacter = null;
            
            // 縦パス/クロス はクロス優先でどちらか一方
            if (crossableCharacters.Any())
            {
                targetCharacter = crossableCharacters.OrderByDescending(character => character.GetCurrentKick()).FirstOrDefault();
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
                CruFramework.Logger.Log($"クロス対象 : {string.Join(",", crossableCharacters.OrderByDescending(character => character.GetCurrentKick()).Select(c => c.id))}");
#endif
                // クロスをあげているキャラクターは今回のoffenceCharacterなので, コマンド種別抽選に必要な賢さはoffenceCharacterのものを使う.
                var crossCommand = GetShootCommandData(targetCharacter, wise, wiseRank, true);
                crossCommand.Index = (byte)ret.Count;
                crossCommand.IsEnoughClose = IsEnoughCloseToShoot(targetCharacter, distanceToGoal);

                ret.Add(crossCommand);

                passableCharacters.Remove(targetCharacter);
            }

            // パス複雑ですね〜w 実装の汚さエグい
            BattleCharacterModel passTargetCharacter = null;
            if (passableCharacters.Any())
            {
                // 必殺技枠の判定のためtargetCharacterとは別にしておく.
                // クロスが取られていた場合はバックパスのみ
                if (crossableCharacters.Any())
                {
                    passTargetCharacter = GetLotteryPassableCharacters(passableCharacters.OrderByDescending(character => character.GetCurrentParameter(character.GetConsParam()))).FirstOrDefault();
                    var backPassCommand = GetBackPassCommandData(passTargetCharacter, wise, wiseRank, distanceToGoal);
                    backPassCommand.Index = (byte)ret.Count;
                    ret.Add(backPassCommand);
                }
                // クロスがとられていない && 二人以上いる場合は縦パス & バックパス
                else if(passableCharacters.Count > 2)
                {
                    passableCharacters = GetLotteryPassableCharacters(passableCharacters.OrderByDescending(character => character.GetCurrentParameter(character.GetConsParam())));
                    if (IsEnoughWiseToForwardPass(wise, wiseRank))
                    {
                        passTargetCharacter = passableCharacters[0];
                        var forwardPassCommand = GetForwardPassCommandData(passTargetCharacter, wise, wiseRank);
                        forwardPassCommand.Index = (byte)ret.Count;
                        ret.Add(forwardPassCommand);
                        passableCharacters.RemoveAt(0);
                        
                        int activeSkillCharacterCount = passableCharacters.Count(character => character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffPassableRateUp) || 
                                                                                          character.IsSpecificActiveAbilityEffectTypeActive(BattleConst.AbilityEffectType.BuffPassableRateDown));
                        // スキル発動者が1人の場合はもう一度スキル適応。
                        if (activeSkillCharacterCount == 1)
                        {
                            passableCharacters = GetLotteryPassableCharacters(passableCharacters);
                        }

                        // 1人目をRemoveしてるので0番目で取得
                        passTargetCharacter = passableCharacters[0];
                        var backPassCommand = GetBackPassCommandData(passTargetCharacter, wise, wiseRank, distanceToGoal);
                        backPassCommand.Index = (byte)ret.Count;
                        ret.Add(backPassCommand);
                    }
                    else
                    {
                        passTargetCharacter = passableCharacters[0];
                        var backPassCommand = GetBackPassCommandData(passTargetCharacter, wise, wiseRank, distanceToGoal);
                        backPassCommand.Index = (byte)ret.Count;
                        ret.Add(backPassCommand);
                    }                    
                }
                // クロスがとられていない & パス対象が一人しかいないなら縦 or バック抽選
                else
                {
                    passTargetCharacter = GetLotteryPassableCharacters(passableCharacters).FirstOrDefault();
                    // パス/バックパス抽選
                    var isForwardPass = LotteryResultByRate(0.5f);
                    if (IsEnoughWiseToForwardPass(wise, wiseRank) && isForwardPass)
                    {
                        // パスをだしているキャラクターは今回のoffenceCharacterなので, コマンド種別抽選に必要な賢さはoffenceCharacterのものを使う.
                        var forwardPassCommand = GetForwardPassCommandData(passTargetCharacter, wise, wiseRank);
                        forwardPassCommand.Index = (byte)ret.Count;
                        ret.Add(forwardPassCommand);
                    }
                    else
                    {
                        // バックパスは対象がいるなら絶対に作れる.
                        var backPassCommand = GetBackPassCommandData(passTargetCharacter, wise, wiseRank, distanceToGoal);
                        backPassCommand.Index = (byte)ret.Count;
                        ret.Add(backPassCommand);
                    }                    
                }
            }
            
            // 優先コマンド設定
            // ロングパスがあれば.
            var longPassCommand = ret.FirstOrDefault(command => IsLongPass(command.ActionType, command.ActionDetailType));
            if (longPassCommand != null)
            {
                longPassCommand.IsAutoChosen = true;
            }
            else
            {
                SetAutoChooseCommandIndex(ret, tactics, offenceCharacter);
            }

            return ret;
        }

        private static void SetAutoChooseCommandIndex(List<BattleMatchUpCommandData> commands, int strategyType, BattleCharacterModel offenceCharacter)
        {
            // キックオフ後は必ず一回はマッチアップを挟むようにする. (マッチアップなしで速攻シュート/クロスにいかれてどうしようもない状況を回避するため.)
            if (!BattleDataMediator.Instance.IsExecutedMatchUpAfterKickOff && commands.Exists(command => command.ActionType == BattleConst.MatchUpActionType.Through))
            {
                BattleDataMediator.Instance.IsExecutedMatchUpAfterKickOff = true;
                commands.First(command => command.ActionType == BattleConst.MatchUpActionType.Through).IsAutoChosen = true;
                return;
            }
            
            int[] weights;
            // TODO APIのほう整備したら定数に置き換え.
            switch (strategyType)
            {
                case 1:
                    weights = (int[])BattleConst.LongRangeShootStrategyCommandWeight.Clone();
                    break;
                case 2:
                    weights = (int[])BattleConst.ShootRangeShootStrategyCommandWeight.Clone();
                    break;
                case 3:
                    weights = (int[])BattleConst.PassStrategyCommandWeight.Clone();
                    break;
                default:
                    weights = (int[])BattleConst.DefaultStrategyCommandWeight.Clone();
                    break;
            }
            
            // index. 
            // 突破, 縦パス, バックパス, シュート, ロングシュート, クロス, ロングクロス
            // 縦パス/バックパスとかでActionTypeをそもそも変えようと思ったけど, 影響範囲がバカほどでかくなるので一旦据え置き…
            // まだ変わる可能性を否定しきれないのでこのタイミングで全部変えてまた復活させて, とかやるのバカバカしすぎる.
            const byte throughIndex = 0;
            const byte forwardPassIndex = 1;
            const byte backPassIndex = 2;
            const byte shootIndex = 3;
            const byte longShootIndex = 4;
            const byte crossIndex = 5;
            const byte longCrossIndex = 6;
            
            // commandsの件数がたかが知れてる(最大でも5)なので可読性優先
            var passCommand = commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Pass);
            var shootCommand = commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Shoot);
            var crossCommand = commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Cross);

            var throughWeight = offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffChooseThroughRateUp);
            var passWeight = offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffChoosePassRateUp);
            var shootWeight = offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffChooseShootRateUp);
            var crossWeight = offenceCharacter.GetTotalActiveAbilityEffectValue(BattleConst.AbilityEffectType.BuffChooseCrossRateUp);
            
            // マイナス値は排除
            weights[throughIndex] = Math.Max(0, weights[throughIndex] + (int)Math.Floor(weights[throughIndex] * throughWeight));
            weights[forwardPassIndex] = Math.Max(0, weights[forwardPassIndex] + (int)Math.Floor(weights[forwardPassIndex] * passWeight));
            weights[backPassIndex] = Math.Max(0, weights[backPassIndex] + (int)Math.Floor(weights[backPassIndex] * passWeight));
            weights[shootIndex] = Math.Max(0, weights[shootIndex] + (int)Math.Floor(weights[shootIndex] * shootWeight));
            weights[longShootIndex] = Math.Max(0, weights[longShootIndex] + (int)Math.Floor(weights[longShootIndex] * shootWeight));
            weights[crossIndex] = Math.Max(0, weights[crossIndex] + (int)Math.Floor(weights[crossIndex] * crossWeight));
            weights[longCrossIndex] = Math.Max(0, weights[longCrossIndex] + (int)Math.Floor(weights[longCrossIndex] * crossWeight));

            if (passCommand == null)
            {
                weights[forwardPassIndex] = 0;
                weights[backPassIndex] = 0;
            }
            else
            {
                if (IsBackPass(passCommand.ActionType, passCommand.ActionDetailType))
                {
                    weights[backPassIndex] = 0;
                }
                else
                {
                    weights[forwardPassIndex] = 0;
                }
            }

            if (shootCommand == null)
            {
                weights[shootIndex] = 0;
                weights[longShootIndex] = 0;
            }
            else
            {
                if (shootCommand.IsEnoughClose)
                {
                    weights[longShootIndex] = 0;
                }
                else
                {
                    weights[shootIndex] = 0;
                }
            }

            if (crossCommand == null)
            {
                weights[crossIndex] = 0;
                weights[longCrossIndex] = 0;
            }
            else
            {
                if (crossCommand.IsEnoughClose)
                {
                    weights[longCrossIndex] = 0;
                }
                else
                {
                    weights[crossIndex] = 0;
                }
            }
            
#if !PJFB_REL && (UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)
            CruFramework.Logger.Log("コマンド抽選重み" +
                                    $"\n 突破: {weights[throughIndex]}" +
                                    $"\n 縦パス: {weights[forwardPassIndex]}" +
                                    $"\n バックパス: {weights[backPassIndex]}" +
                                    $"\n シュート: {weights[shootIndex]}" +
                                    $"\n ロングシュート: {weights[longShootIndex]}" +
                                    $"\n クロス: {weights[crossIndex]}" +
                                    $"\n ロングクロス: {weights[longCrossIndex]}");
            
            if (BattleDataMediator.Instance.AddMatchUpActionWeightThrough && weights[throughIndex] > 0)
            {
                weights[throughIndex] = 100000;
            }
            if (BattleDataMediator.Instance.AddMatchUpActionWeightPass)
            {
                if (weights[backPassIndex] > 0)
                {
                    weights[backPassIndex] = 100000;
                }
                if (weights[forwardPassIndex] > 0)
                {
                    weights[forwardPassIndex] = 100000;
                }
            }
            if (BattleDataMediator.Instance.AddMatchUpActionWeightShoot)
            {
                if (weights[shootIndex] > 0)
                {
                    weights[shootIndex] = 100000;
                }
                if (weights[longShootIndex] > 0)
                {
                    weights[longShootIndex] = 100000;
                }
            }
            if (BattleDataMediator.Instance.AddMatchUpActionWeightCross)
            {
                if (weights[crossIndex] > 0)
                {
                    weights[crossIndex] = 100000;
                }
                if (weights[longCrossIndex] > 0)
                {
                    weights[longCrossIndex] = 100000;
                }
            }
#endif

            
            var actionTypeIndex = GetRandomIndex(weights);
            switch (actionTypeIndex)
            {
                case throughIndex:
                    commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Through).IsAutoChosen = true;
                    return;
                case forwardPassIndex:
                case backPassIndex:
                    commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Pass).IsAutoChosen = true;
                    return;
                case shootIndex:
                case longShootIndex:
                    commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Shoot).IsAutoChosen = true;
                    return;
                case crossIndex:
                case longCrossIndex:
                    commands.FirstOrDefault(command => command.ActionType == BattleConst.MatchUpActionType.Cross).IsAutoChosen = true;
                    return;
            }
        }

        private static BattleMatchUpCommandData GetThroughCommandData(BattleCharacterModel offenceCharacter, BattleCharacterModel defenceCharacter, BigValue wise, int wiseRank)
        {
            var typeWeights = new int[BattleConst.ThroughDetailTypeWeight.Length];
            for (var i = 0; i < typeWeights.Length; i++)
            {
                if (wise >= BattleConst.ThroughDetailTypeRequiredWise[i] && wiseRank <= BattleConst.ThroughDetailTypeRequiredWiseRank[i])
                {
                    typeWeights[i] = BattleConst.ThroughDetailTypeWeight[i];
                }
                else
                {
                    typeWeights[i] = 0;
                }
            }

            var detailType = (BattleConst.ThroughDetailType)GetRandomIndex(typeWeights);
            var matchUpParam = offenceCharacter.GetConsParam();
            var offenceParma = offenceCharacter.GetCurrentParameter(matchUpParam);
            var defenceParam = defenceCharacter.GetCurrentParameter(matchUpParam);
            switch (detailType)
            {
                case BattleConst.ThroughDetailType.Nice:
                    offenceParma *= 1.2f;
                    break;
                case BattleConst.ThroughDetailType.Bad:
                    offenceParma *= 0.8f;
                    break;
                case BattleConst.ThroughDetailType.TooBad:
                    offenceParma *= 0.6f;
                    break;
            }

            var ret = new BattleMatchUpCommandData();
            ret.ActionType = BattleConst.MatchUpActionType.Through;
            ret.ActionDetailType = (BattleConst.MatchUpActionDetailType)detailType;

            return ret;
        }
        
        private static BattleMatchUpCommandData GetShootCommandData(BattleCharacterModel offenceCharacter, BigValue wise, int wiseRank, bool viaCross)
        {
            var typeWeights = new int[BattleConst.ShootDetailTypeWeight.Length];
            for (var i = 0; i < typeWeights.Length; i++)
            {
                if (wise >= BattleConst.ShootDetailTypeRequiredWise[i] && wiseRank <= BattleConst.ShootDetailTypeRequiredWiseRank[i])
                {
                    typeWeights[i] = BattleConst.ShootDetailTypeWeight[i];
                }
                else
                {
                    typeWeights[i] = 0;
                }
            }

            var detailType = (BattleConst.ShootDetailType)GetRandomIndex(typeWeights);

            var ret = new BattleMatchUpCommandData();
            ret.ActionType = BattleConst.MatchUpActionType.Shoot;
            ret.ActionDetailType = (BattleConst.MatchUpActionDetailType)detailType;
            if (viaCross)
            {
                ret.ActionType = BattleConst.MatchUpActionType.Cross;
                ret.TargetCharaId = offenceCharacter.id;
            }

            return ret;
        }

        public static bool IsEnoughWiseToForwardPass(BigValue wise, int wiseRank)
        {
            var typeWeights = new int[BattleConst.ForwardPassDetailTypeWeight.Length];
            for (var i = 0; i < typeWeights.Length; i++)
            {
                if (wise >= BattleConst.ForwardPassDetailTypeRequiredWise[i] && wiseRank <= BattleConst.ForwardPassDetailTypeRequiredWiseRank[i])
                {
                    typeWeights[i] = BattleConst.ForwardPassDetailTypeWeight[i];
                }
                else
                {
                    typeWeights[i] = 0;
                }
            }

            return typeWeights.Any(weight => weight > 0);
        }

        private static BattleMatchUpCommandData GetForwardPassCommandData(BattleCharacterModel targetCharacter, BigValue wise, int wiseRank)
        {
            var typeWeights = new int[BattleConst.ForwardPassDetailTypeWeight.Length];
            for (var i = 0; i < typeWeights.Length; i++)
            {
                if (wise >= BattleConst.ForwardPassDetailTypeRequiredWise[i] && wiseRank <= BattleConst.ForwardPassDetailTypeRequiredWiseRank[i])
                {
                    typeWeights[i] = BattleConst.ForwardPassDetailTypeWeight[i];
                }
                else
                {
                    typeWeights[i] = 0;
                }
            }

            // 縦パスは賢さ要求なしのものがない=全ての条件を満たさない場合があるため
            if (typeWeights.All(weight => weight == 0))
            {
                return null;
            }
            
            var detailType = (BattleConst.PassDetailType)GetRandomIndex(typeWeights);
            var ret = new BattleMatchUpCommandData();
            ret.ActionType = BattleConst.MatchUpActionType.Pass;
            ret.ActionDetailType = (BattleConst.MatchUpActionDetailType)detailType;
            ret.TargetCharaId = targetCharacter.id;

            return ret;
        }
        
        private static BattleMatchUpCommandData GetBackPassCommandData(BattleCharacterModel targetCharacter, BigValue wise, int wiseRank, int distanceToGoal)
        {
            var ret = new BattleMatchUpCommandData();
            ret.ActionType = BattleConst.MatchUpActionType.Pass;
            ret.TargetCharaId = targetCharacter.id;

            if (distanceToGoal >= BattleConst.AdjustableValueNeedDistanceFromGoalToLongPass)
            {
                ret.ActionDetailType = BattleConst.MatchUpActionDetailType.Type6;
                return ret;
            }
            
            var typeWeights = new int[BattleConst.BackPassDetailTypeWeight.Length];
            for (var i = 0; i < typeWeights.Length; i++)
            {
                if (wise >= BattleConst.BackPassDetailTypeRequiredWise[i] && wiseRank <= BattleConst.BackPassDetailTypeRequiredWiseRank[i])
                {
                    typeWeights[i] = BattleConst.BackPassDetailTypeWeight[i];
                }
                else
                {
                    typeWeights[i] = 0;
                }
            }

            var detailType = (BattleConst.PassDetailType)GetRandomIndex(typeWeights);
            ret.ActionDetailType = (BattleConst.MatchUpActionDetailType)detailType;

            return ret;
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public static CharaVoiceLocationType GetCommandPhraseType(BattleConst.MatchUpActionType actionType, BigValue wise, int wiseRank, bool isInShootRange = false, int distanceToShootRange = 0, bool isBackPass = false)
        {
            int[] typeWeights = null;
            int[] requiredWises = null;
            int[] requiredWiseRanks = null;
            switch (actionType)
            {
                case BattleConst.MatchUpActionType.Through:
                    typeWeights = new int[BattleConst.ThroughCommandPhraseRequiredWise.Length];
                    requiredWises = BattleConst.ThroughCommandPhraseRequiredWise;
                    requiredWiseRanks = BattleConst.ThroughCommandPhraseRequiredWiseRank;
                    break;
                case BattleConst.MatchUpActionType.Shoot:
                    if (isInShootRange)
                    {
                        typeWeights = new int[BattleConst.InShootRangeShootCommandPhraseRequiredWise.Length];
                        requiredWises = BattleConst.InShootRangeShootCommandPhraseRequiredWise;
                        requiredWiseRanks = BattleConst.InShootRangeShootCommandPhraseRequiredWiseRank;
                    }
                    else
                    {
                        switch (distanceToShootRange)
                        {
                            case <= BattleConst.OutOfShootRangePattern1ConditionRange:
                                return CharaVoiceLocationType.CommandPhraseBitFarToShoot;
                            case <= BattleConst.OutOfShootRangePattern2ConditionRange:
                                return CharaVoiceLocationType.CommandPhraseFarToShoot;
                            default:
                                return CharaVoiceLocationType.CommandPhraseTooFarToShoot;
                        }
                    }
                    break;
                case BattleConst.MatchUpActionType.Pass:
                {
                    if (isBackPass)
                    {
                        typeWeights = new int[BattleConst.BackPassCommandPhraseRequiredWise.Length];
                        requiredWises = BattleConst.BackPassCommandPhraseRequiredWise;
                        requiredWiseRanks = BattleConst.BackPassCommandPhraseRequiredWiseRank;
                    }
                    else
                    {
                        typeWeights = new int[BattleConst.ForwardPassCommandPhraseRequiredWise.Length];
                        requiredWises = BattleConst.ForwardPassCommandPhraseRequiredWise;
                        requiredWiseRanks = BattleConst.ForwardPassCommandPhraseRequiredWiseRank;
                    }
                }
                    break;
                case BattleConst.MatchUpActionType.Cross:
                    typeWeights = new int[BattleConst.CrossCommandPhraseRequiredWise.Length];
                    requiredWises = BattleConst.CrossCommandPhraseRequiredWise;
                    requiredWiseRanks = BattleConst.CrossCommandPhraseRequiredWiseRank;
                    break;
            }

            if (typeWeights == null)
            {
                return CharaVoiceLocationType.None;
            }

            for (var i = 0; i < typeWeights.Length; i++)
            {
                if (wise >= requiredWises[i] && wiseRank <= requiredWiseRanks[i])
                {
                    typeWeights[i] = 1;
                }
                else
                {
                    typeWeights[i] = 0;
                }
            }

            // ちょっとこれ演出なので一旦Seed依存しないやつにする.
            var index = GetNonStateRandomIndex(typeWeights);
            switch (actionType)
            {
                case BattleConst.MatchUpActionType.Through:
                    return CharaVoiceLocationType.CommandPhraseThrough + index;
                case BattleConst.MatchUpActionType.Shoot:
                    if (isInShootRange)
                    {
                        return CharaVoiceLocationType.CommandPhraseInShootRange + index;
                    }
                    else
                    {
                        return CharaVoiceLocationType.CommandPhraseTooFarToShoot + index;
                    }
                case BattleConst.MatchUpActionType.Pass:
                {
                    if (isBackPass)
                    {
                        return CharaVoiceLocationType.CommandPhraseBackPass + index;
                    }
                    else
                    {
                        return CharaVoiceLocationType.CommandPhrasePass + index;
                    }
                }
                case BattleConst.MatchUpActionType.Cross:
                    return CharaVoiceLocationType.CommandPhraseCross + index;
            }

            return CharaVoiceLocationType.None;
        }
#endif
        
        private static void SetDefenceAbilityData(ref BattleMatchUpResult matchUpResult, BattleConst.MatchUpActionType actionType, BattleCharacterModel defenceCharacter)
        {
            var activeAbility = defenceCharacter.GetAvailableActiveDFAbility();
            if (activeAbility == null)
            {
                return;
            }
            
            // タイミングが敵の行動と一致しなかった無効
            switch (actionType)
            {
                case BattleConst.MatchUpActionType.Through:
                    if ((BattleConst.AbilityEvaluateTimingType)activeAbility.BattleAbilityMaster.timing != BattleConst.AbilityEvaluateTimingType.ActiveSelectThroughDF)
                    {
                        activeAbility = null;
                    }
                    break;
                case BattleConst.MatchUpActionType.Pass:
                    if ((BattleConst.AbilityEvaluateTimingType)activeAbility.BattleAbilityMaster.timing != BattleConst.AbilityEvaluateTimingType.ActiveSelectPassDF)
                    {
                        activeAbility = null;
                    }
                    break;
                case BattleConst.MatchUpActionType.Shoot:
                    if ((BattleConst.AbilityEvaluateTimingType)activeAbility.BattleAbilityMaster.timing != BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF)
                    {
                        activeAbility = null;
                    }
                    break;
                case BattleConst.MatchUpActionType.Cross:
                    if ((BattleConst.AbilityEvaluateTimingType)activeAbility.BattleAbilityMaster.timing != BattleConst.AbilityEvaluateTimingType.ActiveSelectShootDF)
                    {
                        activeAbility = null;
                    }
                    break;
            }

            if (activeAbility == null)
            {
                return;
            }

            if (!LotteryResultByRate(activeAbility.GetInvokeRate(activeAbility.AbilityLevel) * defenceCharacter.AbilityInvokeRateCoefficient) ||
                !BattleAbilityLogic.EvaluateAbilityCondition(defenceCharacter, activeAbility))
            {
                return;
            }

            matchUpResult.DefenceAbilityId = activeAbility?.BattleAbilityMaster.id ?? -1;
        }

        public static void AddAbilityActivityStat(BattleMatchUpResult matchUpResult)
        {
            foreach (var logData in matchUpResult.ReplaceDigests)
            {
                logData.Item2.Stats.AddActivityStat(
                    logData.Item3.IsGeneralAbility()
                        ? BattleCharacterStatModel.StatType.LowRarityActiveSkill : BattleCharacterStatModel.StatType.HighRarityActiveSkill,
                    true, logData.Item3.BattleAbilityMaster.id);
            }
            
            foreach (var logData in matchUpResult.InsertDigests)
            {
                logData.Item2.Stats.AddActivityStat(
                    logData.Item3.IsGeneralAbility()
                        ? BattleCharacterStatModel.StatType.LowRarityActiveSkill : BattleCharacterStatModel.StatType.HighRarityActiveSkill,
                    true, logData.Item3.BattleAbilityMaster.id);
            }
        }

        public static bool IsMeetEndBattleConditionByScore(List<int> scores)
        {
            foreach (var score in scores)
            {
                if (score >= BattleConst.RequiredScore)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool IsMeetEndBattleConditionByTime(float time)
        {
            return time >= BattleConst.BattleTime;
        }

        /// <summary>
        /// 要求値と確率の範囲を元に要求値を確率に変換する
        /// </summary>
        /// <param name="requestValue">要求値</param>
        /// <param name="requestMin">要求最小値</param>
        /// <param name="requestMax">要求最大値</param>
        /// <param name="rateMin">確率最小値</param>
        /// <param name="rateMax">確率最大値</param>
        /// <returns></returns>
        private static float ConvertRequestValue(BigValue requestValue, BigValue requestMin, BigValue requestMax, float rateMin, float rateMax)
        {
            var requestRange = requestMax - requestMin;
            var rateRange = rateMax - rateMin;
            double rateValue = BigValue.RatioCalculation((requestValue - requestMin) * rateRange, requestRange) + rateMin;
            
            return (float)Math.Clamp(rateValue, (double)rateMin, (double)rateMax);
        }

        public static bool IsShowingHeaderDigestType(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                    case BattleConst.DigestType.KickOffL:
                    case BattleConst.DigestType.KickOffR:
                    case BattleConst.DigestType.DribbleL:
                    case BattleConst.DigestType.DribbleR:
                    case BattleConst.DigestType.MatchUp:
                        return true;
                    case BattleConst.DigestType.TechnicMatchUpWinL:
                    case BattleConst.DigestType.TechnicMatchUpWinR:
                    case BattleConst.DigestType.TechnicMatchUpLoseL:
                    case BattleConst.DigestType.TechnicMatchUpLoseR:
                    case BattleConst.DigestType.PhysicalMatchUpWinL:
                    case BattleConst.DigestType.PhysicalMatchUpWinR:
                    case BattleConst.DigestType.PhysicalMatchUpLoseL:
                    case BattleConst.DigestType.PhysicalMatchUpLoseR:
                    case BattleConst.DigestType.SpeedMatchUpLoseL:
                    case BattleConst.DigestType.SpeedMatchUpLoseR:
                    case BattleConst.DigestType.SpeedMatchUpWinL:
                    case BattleConst.DigestType.SpeedMatchUpWinR:
                        return true;
                    case BattleConst.DigestType.Cross:
                        return false;
                    case BattleConst.DigestType.PassFailed:
                    case BattleConst.DigestType.PassSuccess:
                    case BattleConst.DigestType.PassCutBlock:
                    case BattleConst.DigestType.PassCutCatch:
                        return true;
                    case BattleConst.DigestType.Shoot:
                    case BattleConst.DigestType.ShootBlockL:
                    case BattleConst.DigestType.ShootBlockR:
                    case BattleConst.DigestType.ShootBlockTouchL:
                    case BattleConst.DigestType.ShootBlockTouchR:
                    case BattleConst.DigestType.ShootBlockNotReachL:
                    case BattleConst.DigestType.ShootBlockNotReachR:
                    case BattleConst.DigestType.ShootResultSuccessL:
                    case BattleConst.DigestType.ShootResultSuccessR:
                    case BattleConst.DigestType.ShootResultPunchL:
                    case BattleConst.DigestType.ShootResultPunchR:
                    case BattleConst.DigestType.ShootResultPostL:
                    case BattleConst.DigestType.ShootResultPostR:
                    case BattleConst.DigestType.ShootResultCatch:
                    case BattleConst.DigestType.Goal:
                    case BattleConst.DigestType.Goal_GameSet:
                        return false;
                    case BattleConst.DigestType.SecondBall2:
                    case BattleConst.DigestType.SecondBall3:
                    case BattleConst.DigestType.SecondBall4:
                    case BattleConst.DigestType.OutBall:
                    case BattleConst.DigestType.ThrowIn:
                    case BattleConst.DigestType.ThrowInKeeper:
                    case BattleConst.DigestType.TimeUp:
                        return true;
        
                    case BattleConst.DigestType.Special:
                        return false;
            }

            return true;
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public static bool IsPlayAsNormalSpeedDigest(BattleConst.DigestType digestType, BattleDigestCharacterData mainCharacter, BattleDigestCharacterData targetCharacter, bool isLastScore)
        {
            var isMainCharacterPlayerCaptain = mainCharacter is { Side: BattleConst.TeamSide.Left, IsAce: true };
            var isTargetCharacterPlayerCaptain = targetCharacter is { Side: BattleConst.TeamSide.Left, IsAce: true };
            return IsPlayAsNormalSpeedDigest(digestType, isMainCharacterPlayerCaptain, isTargetCharacterPlayerCaptain, isLastScore);
        }
#endif

        public static bool IsPlayAsNormalSpeedDigest(BattleConst.DigestType digestType, BattleCharacterModel mainCharacter, BattleCharacterModel targetCharacter, bool isLastScore)
        {
            return IsPlayAsNormalSpeedDigest(digestType, mainCharacter.IsPlayerAceCharacter, targetCharacter?.IsPlayerAceCharacter ?? false, isLastScore);
        }

        public static bool IsPlayAsNormalSpeedDigest(BattleConst.DigestType digestType, bool isMainCharacterPlayerCaptain, bool isTargetCharacterPlayerCaptain, bool isLastScore)
        {
            // プレイヤーキャプテンだったら再生.
            if (isMainCharacterPlayerCaptain)
            {
                return true;
            }

            // プレイヤーキャプテンへのパスorクロスはボール渡すところから再生
            if (digestType is BattleConst.DigestType.PassSuccess or BattleConst.DigestType.Cross &&
                isTargetCharacterPlayerCaptain)
            {
                return true;
            }

            // 残り一点で終了の場合はシュート関連の演出を再生
            if (isLastScore)
            {
                switch (digestType)
                {
                    case BattleConst.DigestType.Cross:
                    case BattleConst.DigestType.Shoot:
                    case BattleConst.DigestType.ShootBlockL:
                    case BattleConst.DigestType.ShootBlockR:
                    case BattleConst.DigestType.ShootBlockTouchL:
                    case BattleConst.DigestType.ShootBlockTouchR:
                    case BattleConst.DigestType.ShootBlockNotReachL:
                    case BattleConst.DigestType.ShootBlockNotReachR:
                    case BattleConst.DigestType.ShootResultSuccessL:
                    case BattleConst.DigestType.ShootResultSuccessR:
                    case BattleConst.DigestType.ShootResultPunchL:
                    case BattleConst.DigestType.ShootResultPunchR:
                    case BattleConst.DigestType.ShootResultPostL:
                    case BattleConst.DigestType.ShootResultPostR:
                    case BattleConst.DigestType.ShootResultCatch:
                        return true;
                }
            }

            // プレイヤーキャプテンとか関係なく以下は再生.
            switch (digestType)
            {
                case BattleConst.DigestType.MatchUp:
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.Goal:
                case BattleConst.DigestType.Goal_GameSet:
                case BattleConst.DigestType.Special:
                    return true;
            }

            return false;            
        }

        public static int GetGroundSpriteIndex(int distanceToGoal)
        {
            var index = 0;
            foreach(var distance in BattleConst.MatchUpGroundSpriteTypeByDistanceFromGoal)
            {
                if (distanceToGoal <= distance)
                {
                    return index;
                }

                index++;
            }

            return index;
        }

        public static void SetAbilityInvokeRateCoefficient(List<List<BattleCharacterModel>> decks)
        {
            var average = decks.Sum(deck => deck.Sum(chara => chara.baseWise)) / decks.Sum(deck => deck.Count);
            
            foreach (var deck in decks)
            {
                foreach (var chara in deck)
                {
                    double rate = BigValue.RatioCalculation(chara.baseWise, average);
                    BigValue requestValue = new BigValue(rate * BigValue.DefaultRateValue);
                    BigValue requestMin = new BigValue(BattleConst.AdjustableValueRequiredMinWiseValueForAbilityInvokeRate * BigValue.DefaultRateValue);
                    BigValue requestMax = new BigValue(BattleConst.AdjustableValueRequiredMaxWiseValueForAbilityInvokeRate * BigValue.DefaultRateValue);
                    chara.AbilityInvokeRateCoefficient = ConvertRequestValue(requestValue, requestMin, requestMax, BattleConst.AdjustableValueMinAbilityInvokeRateCoefficient, BattleConst.AdjustableValueMaxAbilityInvokeRateCoefficient);
                }
            }
        }

        public static bool IsBackPass(BattleConst.MatchUpActionType actionType, BattleConst.MatchUpActionDetailType actionDetailType)
        {
            return actionType == BattleConst.MatchUpActionType.Pass &&
                   actionDetailType == BattleConst.MatchUpActionDetailType.Type0 ||
                   actionDetailType == BattleConst.MatchUpActionDetailType.Type1 ||
                   actionDetailType == BattleConst.MatchUpActionDetailType.Type2;
        }
        
        public static bool IsLongPass(BattleConst.MatchUpActionType actionType, BattleConst.MatchUpActionDetailType actionDetailType)
        {
            return actionType == BattleConst.MatchUpActionType.Pass && actionDetailType == BattleConst.MatchUpActionDetailType.Type6;
        }

        public static void SetMatchUpDigestType(ref BattleMatchUpResult matchUpResult)
        {
            var offenceCharacter = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            if (!offenceCharacter.IsPlayerAceCharacter)
            {
                return;
            }
            
            matchUpResult.MatchUpDigestType = GetMatchUpDigestType(matchUpResult);
        }

        private static readonly int[] MatchUpTypeWeightsWhenThroughLose = { 100, 0, 0 }; 
        private static readonly int[] MatchUpTypeWeightsWhenThroughWin = { 90, 0, 10 }; 
        private static readonly int[] MatchUpTypeWeightsWhenThroughAbility = { 0, 50, 50 }; 
        private static readonly int[] MatchUpTypeWeightsWhenPassLose = { 100, 0, 0 }; 
        private static readonly int[] MatchUpTypeWeightsWhenPassWin = { 100, 0, 0 }; 
        private static readonly int[] MatchUpTypeWeightsWhenPassAbility = { 0, 50, 50 }; 
        private static readonly int[] MatchUpTypeWeightsWhenShootLose = { 80, 0, 20 }; 
        private static readonly int[] MatchUpTypeWeightsWhenShootWin = { 30, 40, 30 }; 
        private static readonly int[] MatchUpTypeWeightsWhenShootAbility = { 0, 50, 50 }; 
        private static BattleConst.MatchUpDigestType GetMatchUpDigestType(BattleMatchUpResult matchUpResult)
        {
            // パスはマッチアップ演出なし
            if (matchUpResult.ActionType == BattleConst.MatchUpActionType.Pass)
            {
                return BattleConst.MatchUpDigestType.None;
            }
            
            /*
                ■ドリブル突破時
                敗北時　なし:100%　
                勝利時　なし:90%　弱:10%
                スキル発動時　弱:50%　強:50%
                ■パス時(パススキルのときだけ…)
                敗北時　なし:100%　
                勝利時　なし:100%
                スキル発動時　弱:50%　強:50%
                ■シュート/クロス時
                敗北時　なし:80%　弱:20%
                勝利時　なし30%　弱:30%　強:40%
                スキル発動時　弱:50%　強:50%
             */
            var index = 0;
            int[] weights = MatchUpTypeWeightsWhenThroughLose;
            if (matchUpResult.OffenceAbilityId > 0)
            {
                switch (matchUpResult.ActionType)
                {
                    case BattleConst.MatchUpActionType.Through:
                        weights = MatchUpTypeWeightsWhenThroughAbility;
                        break;
                    case BattleConst.MatchUpActionType.Pass:
                        weights = MatchUpTypeWeightsWhenPassAbility;
                        break;
                    case BattleConst.MatchUpActionType.Shoot:
                    case BattleConst.MatchUpActionType.Cross:
                        weights = MatchUpTypeWeightsWhenShootAbility;
                        break;
                }
            }
            else if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.Success)
            {
                switch (matchUpResult.ActionType)
                {
                    case BattleConst.MatchUpActionType.Through:
                        weights = MatchUpTypeWeightsWhenThroughWin;
                        break;
                    case BattleConst.MatchUpActionType.Pass:
                        weights = MatchUpTypeWeightsWhenPassWin;
                        break;
                    case BattleConst.MatchUpActionType.Shoot:
                    case BattleConst.MatchUpActionType.Cross:
                        weights = MatchUpTypeWeightsWhenShootWin;
                        break;
                }
            }
            else
            {
                switch (matchUpResult.ActionType)
                {
                    case BattleConst.MatchUpActionType.Through:
                        weights = MatchUpTypeWeightsWhenThroughLose;
                        break;
                    case BattleConst.MatchUpActionType.Pass:
                        weights = MatchUpTypeWeightsWhenPassLose;
                        break;
                    case BattleConst.MatchUpActionType.Shoot:
                    case BattleConst.MatchUpActionType.Cross:
                        weights = MatchUpTypeWeightsWhenShootLose;
                        break;
                }
            }
            
            index = GetRandomIndex(weights);

#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceMatchUpType >= 0)
            {
                index = BattleDataMediator.Instance.ForceMatchUpType;
            }
#endif

            return (BattleConst.MatchUpDigestType)index;
        }
    }
}