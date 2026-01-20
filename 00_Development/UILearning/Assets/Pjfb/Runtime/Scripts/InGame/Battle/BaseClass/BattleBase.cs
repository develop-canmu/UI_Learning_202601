using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
using Cysharp.Threading.Tasks;
using UnityEngine;
#endif


// ReSharper disable once CheckNamespace
namespace Pjfb.Battle
{
    public abstract class BattleBase
    {
        // バトル中の各処理の状態
        public enum BattleState
        {
            None,
            StartBattle,            // バトル開始時に一回だけ通るステート
            SelectRoundMember,      // 今回のラウンドの対象になるキャラクターの抽選とポジションの決定.
            KickOff,                // 初期ボール保有者の決定. スキル効果の適用とかやりそう.
            SetMarkTarget,          // 各キャラクターがマークする対象の決定.
            JustRunAction,          // マッチアップ発生までの走っている状態.
            SelectMatchUpAction,    // ボール保有者のマーク対象とのアクション選択.
            JudgeMatchUpPreResult,  // マッチアップ結果の判定.
            JudgeMatchUpFinalResult,// マッチアップ結果の判定.
            Finish,                 // 終了待機
        }
        
        public BattleState CurrentState { get; protected set; } = BattleState.None;
        
        public virtual void StateAction(BattleState state)
        {
            CurrentState = state;

            switch (state)
            {
                case BattleState.StartBattle:
                    StartBattleStateAction();
                    break;
                case BattleState.SelectRoundMember: // 今回のラウンドの対象になるキャラクターの抽選とポジションの決定.
                    SelectRoundMemberStateAction();
                    break;
                case BattleState.KickOff: // スキル効果の適用とかやりそう.
                    KickOffStateAction();
                    break;
                case BattleState.SetMarkTarget: // 各キャラクターがマークする対象の決定.
                    SetMarkTargetStateAction();
                    break;
                case BattleState.JustRunAction: // ドリブルの解決
                    JustRunActionStateAction();
                    break;
                case BattleState.SelectMatchUpAction: // ボール保有者のマーク対象とのアクション選択.
                    SelectMatchUpActionStateAction();
                    break;
                case BattleState.JudgeMatchUpPreResult: // マッチアップ結果の判定.
                    // JudgeMatchUpResultStateAction();
                    break;
                case BattleState.JudgeMatchUpFinalResult: // マッチアップ結果の判定.
                    JudgeMatchUpFinalResultStateAction();
                    break;
                case BattleState.Finish:
                    break;
            }
        }

        public void DoAutoResolveMatchUp()
        {
            var commands = BattleDataMediator.Instance.NextCommandData;
            BattleMatchUpCommandData targetCommand = null;
            targetCommand = commands.FirstOrDefault(command => command.IsAutoChosen);
            DoOffenseMatchUp(targetCommand);
        }

        protected virtual void DoOffenseMatchUp(BattleMatchUpCommandData commandData)
        {
            DoMatchUp(commandData.ActionType, commandData.TargetCharaId, commandData.ActionDetailType);
        }

        protected virtual void StartBattleStateAction()
        {
            var logList = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();
            foreach (var deck in BattleDataMediator.Instance.Decks)
            {
                BattleAbilityLogic.EvaluateCharactersAbility(BattleConst.AbilityEvaluateTimingType.OnGameStart, deck, logList);
            }
            
            BattleLogMediator.Instance.AddAbilityLog(logList);
            BattleDataMediator.Instance.OffenceSide = BattleGameLogic.GetInitialAttackerSide(BattleDataMediator.Instance.BattleType, BattleDataMediator.Instance.OriginalBattleV2ClientData.firstAttackIndex);
            StateAction(BattleState.SelectRoundMember);
        }

        protected virtual void SelectRoundMemberStateAction()
        {
            // 指定されたボール保有者がいない = フィールド中央からのキックオフ
            var isStartAfterKickOff = BattleDataMediator.Instance.BallOwnerCharacterId <= 0;
            // 参加者決定前にボール保有者を決定するのもようわからんけどいいのか…?
            BattleDataMediator.Instance.BallOwnerCharacterId = BattleGameLogic.GetInitialBallOwnerId(BattleDataMediator.Instance.BallOwnerCharacterId, BattleDataMediator.Instance.GetOffenceDeck());
            BattleGameLogic.LotteryOffencePlayers(BattleDataMediator.Instance.GetOffenceDeck(), BattleDataMediator.Instance.OffenceCharacters, BattleDataMediator.Instance.BallOwnerCharacterId, isStartAfterKickOff);
            BattleGameLogic.LotteryDefencePlayers(BattleDataMediator.Instance.GetDefenceDeck(), BattleDataMediator.Instance.DefenceCharacters, BattleDataMediator.Instance.OffenceCharacters.Count, isStartAfterKickOff);

            if (isStartAfterKickOff)
            {
                var matchUpResult = new BattleMatchUpResult();
                matchUpResult.NextBallOwnerId = BattleDataMediator.Instance.BallOwnerCharacterId;
                matchUpResult.NextBallPosition = BattleDataMediator.Instance.BallPosition;
                matchUpResult.RoundOffenceCharacterIds = BattleDataMediator.Instance.GetRoundOffenceCharacters().Select(c => c.id).ToList();
                matchUpResult.RoundDefenceCharacterIds = BattleDataMediator.Instance.GetRoundDefenceCharacters().Select(c => c.id).ToList();
                BattleLogMediator.Instance.AddKickOffLog(matchUpResult);
                BattleAbilityLogic.DecrementAbilityRemainTurn(BattleConst.AbilityTurnDecrementTiming.OnKickOff, BattleDataMediator.Instance.Decks);
                BattleDataMediator.Instance.IsExecutedMatchUpAfterKickOff = false;
            }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            Debug.Log($"Offence: ${string.Join(",", BattleDataMediator.Instance.OffenceCharacters.Select(c => c.id))}");
            Debug.Log($"Defence: ${string.Join(",", BattleDataMediator.Instance.DefenceCharacters.Select(c => c.id))}");
#endif

            BattleAbilityLogic.DecrementAbilityRemainTurn(BattleConst.AbilityTurnDecrementTiming.OnOffenceSideChanged, BattleDataMediator.Instance.Decks);
            var logList = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();
            BattleAbilityLogic.EvaluateCharactersAbility(BattleConst.AbilityEvaluateTimingType.OnRoundStart, BattleDataMediator.Instance.GetOffenceDeck(), logList);
            BattleAbilityLogic.EvaluateCharactersAbility(BattleConst.AbilityEvaluateTimingType.OnRoundStart, BattleDataMediator.Instance.GetDefenceDeck(), logList);
            BattleLogMediator.Instance.AddAbilityLog(logList);

            StateAction(BattleState.KickOff);
        }

        protected virtual void KickOffStateAction()
        {
            StateAction(BattleState.SetMarkTarget);
        }

        protected virtual void SetMarkTargetStateAction()
        {
            BattleAbilityLogic.DecrementAbilityRemainTurn(BattleConst.AbilityTurnDecrementTiming.OnMarkChanged, BattleDataMediator.Instance.Decks);

            var offenceDeck = BattleDataMediator.Instance.GetRoundOffenceCharacters();
            var defenceDeck = BattleDataMediator.Instance.GetRoundDefenceCharacters();
            RefillRoundPlayer(ref offenceDeck, BattleDataMediator.Instance.GetOffenceDeck());
            RefillRoundPlayer(ref defenceDeck, BattleDataMediator.Instance.GetDefenceDeck());

            BattleDataMediator.Instance.ResetMarkTarget();
            BattleGameLogic.SetMarkTargetPlayers(BattleDataMediator.Instance.BallOwnerCharacterId, BattleDataMediator.Instance.OffenceCharacters, BattleDataMediator.Instance.DefenceCharacters);

            StateAction(BattleState.JustRunAction);
        }

        protected virtual void RefillRoundPlayer(ref List<BattleCharacterModel> roundCharacters, List<BattleCharacterModel> deck)
        {
            // OF/DFともに有効な(=パスをしていない, 突破をされていない)キャラクターが一人しかいない場合は最後に無効になったキャラ以外を補充する
            if (roundCharacters.Count(chara => chara.ClearedNumOnRound < 0) > 1)
            {
                return;
            }

            var lastClearedNum = roundCharacters.Max(chara => chara.ClearedNumOnRound);
            // そもそもラウンド参加者が一人になっている場合があるため. 仕様バグみある.
            if (lastClearedNum < 0)
            {
                lastClearedNum = int.MaxValue;
            }

            var weights = new int[deck.Count];
            for (var i = 0; i < deck.Count; i++)
            {
                weights[i] = deck[i].ClearedNumOnRound == lastClearedNum || (roundCharacters.Contains(deck[i]) && deck[i].ClearedNumOnRound == -1) ? 0 : 1;
            }

            var randIndex = BattleGameLogic.GetRandomIndex(weights);
            var target = deck[randIndex];
            if (roundCharacters.Contains(target))
            {
                target.ClearedNumOnRound = -1;
            }
            else
            {
                roundCharacters.Add(target);
            }
        }

        protected virtual void JustRunActionStateAction()
        {
            var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();
            var offenceRoundCharacters = BattleDataMediator.Instance.GetRoundOffenceCharacters();
            var defenceRoundCharacters = BattleDataMediator.Instance.GetRoundDefenceCharacters();
            var offenceTeamCharacters = BattleDataMediator.Instance.GetOffenceDeck();
            var defenceTeamCharacters = BattleDataMediator.Instance.GetDefenceDeck();
            var currentBallPosition = BattleDataMediator.Instance.BallPosition;

            var dribbleResult = BattleGameLogic.JudgeDribbleResult(offenceCharacter, offenceRoundCharacters, defenceRoundCharacters, offenceTeamCharacters, defenceTeamCharacters, currentBallPosition);
            BattleDataMediator.Instance.ApplyBattleMatchUpResult(dribbleResult);

            // 今回マッチアップが発生するときだけログ&ダイジェストの演出を発生させる.
            if (defenceRoundCharacters.Any(character => character.ClearedNumOnRound < 0))
            {
                BattleLogMediator.Instance.AddMatchUpResultLog(dribbleResult);
            }

            StateAction(BattleState.SelectMatchUpAction);
        }

        protected virtual void SelectMatchUpActionStateAction()
        {
            var matchUpResult = new BattleMatchUpResult();
            matchUpResult.NextBallOwnerId = BattleDataMediator.Instance.BallOwnerCharacterId;
            matchUpResult.NextBallPosition = BattleDataMediator.Instance.BallPosition;
            matchUpResult.RoundOffenceCharacterIds = BattleDataMediator.Instance.GetRoundOffenceCharacters().Select(c => c.id).ToList();
            matchUpResult.RoundDefenceCharacterIds = BattleDataMediator.Instance.GetRoundDefenceCharacters().Select(c => c.id).ToList();
            BattleLogMediator.Instance.AddMatchUpLog(matchUpResult);

            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.NextBallOwnerId);
            var defencePlayer = offencePlayer.MarkCharacter;

            BattleAbilityLogic.DecrementAbilityRemainTurn(BattleConst.AbilityTurnDecrementTiming.OnBeforeMatchUp, BattleDataMediator.Instance.Decks);
            var logList = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();
            if (offencePlayer != null)
            {
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnBeforeMatchUp, offencePlayer, logList);
            }

            if (defencePlayer != null)
            {
                BattleAbilityLogic.EvaluateCharacterAbility(BattleConst.AbilityEvaluateTimingType.OnBeforeMatchUp, defencePlayer, logList);
            }
            BattleLogMediator.Instance.AddAbilityLog(logList);

            // コマンドの内容決定
            var offenceRoundCharacters = BattleDataMediator.Instance.GetRoundOffenceCharacters();
            var offenceDeck = BattleDataMediator.Instance.GetOffenceDeck();
            var defenceDeck = BattleDataMediator.Instance.GetDefenceDeck();
            var distanceToGoal = BattleGameLogic.GetRemainDistanceToGoal(BattleDataMediator.Instance.OffenceSide, BattleDataMediator.Instance.BallPosition);
            var tactics = BattleDataMediator.Instance.GetBattlePlayer(BattleDataMediator.Instance.OffenceSide)?.Tactics ?? 0;
            BattleDataMediator.Instance.NextCommandData = BattleGameLogic.GetMatchUpCommandData(offencePlayer, defencePlayer, offenceRoundCharacters, offenceDeck, defenceDeck, distanceToGoal, tactics);
            DoAutoResolveMatchUp();

            /*
            // プレイヤーキャプテン
            if (offencePlayer.IsPlayerAceCharacter)
            {
                // レジュームログあったら自動適用
                if (BattleDataMediator.Instance.ResumedCommandLog.Any())
                {
                    var index = BattleDataMediator.Instance.ResumedCommandLog[0];
                    BattleDataMediator.Instance.ResumedCommandLog.RemoveAt(0);
                    DoAutoResolveMatchUp(index, false);
                }
            }
            */
        }


        private void DoMatchUp(BattleConst.MatchUpActionType offenceAction, long targetCharacterId, BattleConst.MatchUpActionDetailType actionDetailType)
        {
            if (CurrentState != BattleState.SelectMatchUpAction)
            {
                return;
            }

            // failsafe validation
            if (offenceAction is BattleConst.MatchUpActionType.Pass or BattleConst.MatchUpActionType.Cross && targetCharacterId == -1)
            {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
                Debug.LogError("対象指定のないパス or クロスが発行されました.");
#endif
                return;
            }

            // failsafe validation
            if (offenceAction == BattleConst.MatchUpActionType.Shoot)
            {
                var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();
                var distanceToGoal = BattleGameLogic.GetRemainDistanceToGoal(offenceCharacter.Side, BattleDataMediator.Instance.BallPosition);
                if (!BattleGameLogic.CanShoot(offenceCharacter, distanceToGoal))
                {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
                    Debug.LogError("シュートレンジ外からシュートコマンドが発行されました.");
#endif
                    return;
                }
            }

            // failsafe validation
            if (offenceAction is BattleConst.MatchUpActionType.Pass or BattleConst.MatchUpActionType.Cross)
            {
                var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();
                var targetCharacter = BattleDataMediator.Instance.GetBattleCharacter(offenceCharacter.Side, targetCharacterId);
                if (targetCharacter == null)
                {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
                    Debug.LogError("対象指定が不正なパス/クロスコマンドが発行されました.");
#endif
                    return;
                }
                
                offenceCharacter.ActionTargetCharacter = targetCharacter;
            }

            JudgeMatchUpPreResultStateAction(offenceAction, targetCharacterId, actionDetailType);
        }

        // 相当歪なのは理解しているが, 別スレッドにするとTryCatch出来なくなるので…やばいデータが入っていたときに後続のLambda処理が全部しぬ.
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        protected virtual void JudgeMatchUpPreResultStateAction(BattleConst.MatchUpActionType offenceAction, long targetCharacterId, BattleConst.MatchUpActionDetailType actionDetailType)
#else
        protected virtual void JudgeMatchUpPreResultStateAction(BattleConst.MatchUpActionType offenceAction, long targetCharacterId, BattleConst.MatchUpActionDetailType actionDetailType)
#endif
        {
            var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();
            var defenceCharacter = offenceCharacter.MarkCharacter;
            var offenceMatchUpActionType = offenceAction;
            var offenceRoundCharacters = BattleDataMediator.Instance.GetRoundOffenceCharacters();
            var defenceRoundCharacters = BattleDataMediator.Instance.GetRoundDefenceCharacters();
            var currentBallPosition = BattleDataMediator.Instance.BallPosition;
            var distanceToGoal = BattleGameLogic.GetRemainDistanceToGoal(offenceCharacter.Side, currentBallPosition);

            BattleCharacterModel targetCharacter = null;
            if (offenceAction is BattleConst.MatchUpActionType.Pass or BattleConst.MatchUpActionType.Cross)
            {
                targetCharacter = BattleDataMediator.Instance.GetBattleCharacter(offenceCharacter.Side, targetCharacterId);
            }

            var matchUpResult = new BattleMatchUpResult();
            BattleGameLogic.JudgeMatchUpPreResult(
                ref matchUpResult, offenceMatchUpActionType, actionDetailType,
                offenceCharacter, offenceRoundCharacters,
                defenceCharacter, defenceRoundCharacters, currentBallPosition, targetCharacter, distanceToGoal);

            BattleDataMediator.Instance.NextMatchUpResult = matchUpResult;

            if (matchUpResult.OffenceAbilityId <= 0 || BattleDataMediator.Instance.IsSkipToFinish)
            {
                if (matchUpResult.OffenceAbilityId > 0)
                {
                    BattleGameLogic.SetMatchUpDigestType(ref matchUpResult);
                    BattleLogMediator.Instance.AddMatchUpActivatedLog(matchUpResult);
                    BattleLogMediator.Instance.AddPreMatchUpResultLog(matchUpResult);
                    BattleEventDispatcher.Instance.OnActivateActiveAbilityCallback();
                }
                
                StateAction(BattleState.JudgeMatchUpFinalResult);
            }
            // 必殺技発動アリだったら介入の結果変わる可能性があるため一時演出
            else
            {
                BattleGameLogic.SetMatchUpDigestType(ref matchUpResult);
                BattleLogMediator.Instance.AddMatchUpActivatedLog(matchUpResult);
                BattleLogMediator.Instance.AddPreMatchUpResultLog(matchUpResult);
            }
        }
        public void JudgeMatchUpFinalResult(bool isSwiped)
        {
            var result = BattleDataMediator.Instance.NextMatchUpResult;
            // 結果判定後の賑やかし演出(セカンドボールとかシュートブロックとか)だったら再度結果判定にいかせないため. わけわから〜んｗ
            if (!result.IsProgress)
            {
                return;
            }
            BattleDataMediator.Instance.NextMatchUpResult.IsSwiped = isSwiped;
            StateAction(BattleState.JudgeMatchUpFinalResult);
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        protected virtual async void JudgeMatchUpFinalResultStateAction()
#else
        protected virtual void JudgeMatchUpFinalResultStateAction()
#endif
        {
            var matchUpResult = BattleDataMediator.Instance.NextMatchUpResult;
            var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();
            var defenceCharacter = offenceCharacter.MarkCharacter;
            var offenceAction = matchUpResult.ActionType;
            var actionDetailType = matchUpResult.ActionDetailType;
            var targetCharacterId = matchUpResult.TargetCharacterId;
            var offenceRoundCharacters = BattleDataMediator.Instance.GetRoundOffenceCharacters();
            var defenceRoundCharacters = BattleDataMediator.Instance.GetRoundDefenceCharacters();
            var offenceTeamCharacters = BattleDataMediator.Instance.GetOffenceDeck();
            var defenceTeamCharacters = BattleDataMediator.Instance.GetDefenceDeck();
            var distanceToGoal = BattleGameLogic.GetRemainDistanceToGoal(offenceCharacter.Side, BattleDataMediator.Instance.BallPosition);
            BattleCharacterModel targetCharacter = null;
            if (offenceAction is BattleConst.MatchUpActionType.Pass or BattleConst.MatchUpActionType.Cross)
            {
                targetCharacter = BattleDataMediator.Instance.GetBattleCharacter(offenceCharacter.Side, targetCharacterId);
            }

            BattleBluelockManModel blueLockMan = null;
            if (offenceAction is BattleConst.MatchUpActionType.Shoot or BattleConst.MatchUpActionType.Cross)
            {
                blueLockMan = BattleDataMediator.Instance.GetBluelockManModel(BattleGameLogic.GetOtherSide(offenceCharacter.Side));
            }

            BattleGameLogic.JudgeMatchUpFinalResult(
                ref matchUpResult, offenceAction, actionDetailType,
                offenceCharacter, offenceRoundCharacters, offenceTeamCharacters,
                defenceCharacter, defenceRoundCharacters, defenceTeamCharacters, distanceToGoal, targetCharacter, blueLockMan);
            
            // Lambdaでログ生成時, Lambdaで見たPlayerAceCharacterと双方のプレイヤーから見たリプレイ時のPlayerAceCharacterは違うため, ログ生成時点では双方のAceCharacterに対して生成する.
            if (offenceCharacter.IsAceCharacter && matchUpResult.OffenceAbilityId <= 0)
            {
                BattleGameLogic.SetMatchUpDigestType(ref matchUpResult);
                BattleLogMediator.Instance.AddMatchUpActivatedLog(matchUpResult);
            }

            BattleDataMediator.Instance.ApplyBattleMatchUpResult(matchUpResult);
            BattleLogMediator.Instance.AddAbilityLog(matchUpResult.BeforeAbilityLogs);
            BattleLogMediator.Instance.AddMatchUpResultLog(matchUpResult);
            BattleLogMediator.Instance.AddAbilityLog(matchUpResult.AfterAbilityLogs);
            BattleDataMediator.Instance.ApplyFieldDataByMatchUpResult(matchUpResult);
            BattleGameLogic.AddAbilityActivityStat(matchUpResult);

            // 最終処理しきったのでアクセス出来ないように.
            BattleDataMediator.Instance.NextMatchUpResult.ResetActivatedAbilityData();

            if (matchUpResult.ScoredCharacterId > 0)
            {
                var logList = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();
                var scoredSide = offenceCharacter.Side;
                for (var i = 0; i < BattleDataMediator.Instance.Decks.Count; i++)
                {
                    var evaluateType = (int)scoredSide == i ? BattleConst.AbilityEvaluateTimingType.OnAfterAllyGoal : BattleConst.AbilityEvaluateTimingType.OnAfterEnemyGoal;
                    BattleAbilityLogic.EvaluateCharactersAbility(evaluateType, BattleDataMediator.Instance.Decks[i], logList);
                }
                BattleLogMediator.Instance.AddAbilityLog(logList);
            }

            if (CheckBattleEndCondition(matchUpResult))
            {
                return;
            }

            if (!BattleDataMediator.Instance.IsSkipToFinishWithoutView)
            {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
                await UniTask.DelayFrame(3);
#endif
            }

            BattleLogMediator.Instance.AddNextMatchUpLog();
        }

        protected virtual bool CheckBattleEndCondition(BattleMatchUpResult matchUpResult)
        {
            if (BattleGameLogic.IsMeetEndBattleConditionByScore(BattleDataMediator.Instance.Score))
            {
                BattleLogMediator.Instance.AddBattleEndLog();
                StateAction(BattleState.Finish);
                return true;
            }

            if (BattleGameLogic.IsMeetEndBattleConditionByTime(BattleDataMediator.Instance.GameTime) && matchUpResult.IsResetRound)
            {
                BattleLogMediator.Instance.AddTimeUpLog();
                StateAction(BattleState.Finish);
                return true;
            }

            return false;
        }

        public BattleState GetNextState(BattleMatchUpResult matchUpResult)
        {
            if (matchUpResult.IsResetRound)
            {
                return BattleState.SelectRoundMember;
            }
            else
            {
                return BattleState.SetMarkTarget;
            }
        }

        public virtual void SetData(BattleV2ClientData data)
        {
            BattleDataMediator.Instance.InitializeGameData(data);
        }
    }
}
