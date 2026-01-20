using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logger = CruFramework.Logger;
using Cysharp.Threading.Tasks;
using Pjfb.InGame;
using Pjfb.Battle;

namespace Pjfb
{
    public class TutorialSingleBattle : SingleBattle
    {
        private TutorialSettings.InGameSettingData settingData;
        private TutorialSettings.SelectMatchUpActionStateData selectMatchUpActionData;
        private int selectRoundMemberIndex;
        private int setMarkTargetStateIndex;
        private int justRunActionStateIndex;
        private int selectMatchUpActionStateIndex;

        public TutorialSingleBattle()
        {
            settingData = AppManager.Instance.TutorialManager.GetBattleScenarioData();
            selectRoundMemberIndex = 0;
            setMarkTargetStateIndex = 0;
            justRunActionStateIndex = 0;
            selectMatchUpActionStateIndex = 0;
        }

        private TutorialSettings.SelectRoundMemberData GetNextSelectRoundMember()
        {
            if (selectRoundMemberIndex >= settingData.selectRoundMemberDataList.Count)
            {
                return null;
            }
            var ret = settingData.selectRoundMemberDataList[selectRoundMemberIndex];
            selectRoundMemberIndex++;
            return ret;
        }

        private TutorialSettings.SetMarkTargetStateData GetNextSetMarkTargetState()
        {
            if (setMarkTargetStateIndex >= settingData.setMarkTargetStateDataList.Count)
            {
                return null;
            }
            var ret = settingData.setMarkTargetStateDataList[setMarkTargetStateIndex];
            setMarkTargetStateIndex++;
            return ret;
        }

        private TutorialSettings.JustRunActionStateData GetNextJustRunActionState()
        {
            if (justRunActionStateIndex >= settingData.justRunActionStateDataList.Count)
            {
                return null;
            }
            var ret = settingData.justRunActionStateDataList[justRunActionStateIndex];
            SetChangedStaminaDictionary(ref ret.matchUpResult);
            justRunActionStateIndex++;
            return ret;
        }

        private TutorialSettings.SelectMatchUpActionStateData GetNextSelectMatchUpActionState()
        {
            if (selectMatchUpActionStateIndex >= settingData.selectMatchUpActionStateDataList.Count)
            {
                return null;
            }
            var ret = settingData.selectMatchUpActionStateDataList[selectMatchUpActionStateIndex];
            SetChangedStaminaDictionary(ref ret.matchUpResult);
            selectMatchUpActionStateIndex++;
            return ret;
        }

        protected override void SelectRoundMemberStateAction()
        {

            var data = GetNextSelectRoundMember();
            if (data == null)
            {
                Logger.Log("TutorialError!! SelectRoundMemberが不足");
                return;
            }

            var isStartAfterKickOff = BattleDataMediator.Instance.BallOwnerCharacterId <= 0;

            BattleDataMediator.Instance.BallOwnerCharacterId = data.ownerId;

            LotteryPlayers(BattleDataMediator.Instance.GetOffenceDeck(),
                BattleDataMediator.Instance.OffenceCharacters,
                BattleDataMediator.Instance.BallOwnerCharacterId,
                data.offenceCharacterList, true, isStartAfterKickOff);

            LotteryPlayers(BattleDataMediator.Instance.GetDefenceDeck(),
                BattleDataMediator.Instance.DefenceCharacters,
                BattleDataMediator.Instance.BallOwnerCharacterId,
                data.defenceCharacterList, false, false);

            if (isStartAfterKickOff)
            {
                var matchUpResult = new BattleMatchUpResult();
                matchUpResult.NextBallOwnerId = BattleDataMediator.Instance.BallOwnerCharacterId;
                matchUpResult.NextBallPosition = BattleDataMediator.Instance.BallPosition;
                matchUpResult.RoundOffenceCharacterIds = BattleDataMediator.Instance.GetRoundOffenceCharacters().Select(c => c.id).ToList();
                matchUpResult.RoundDefenceCharacterIds = BattleDataMediator.Instance.GetRoundDefenceCharacters().Select(c => c.id).ToList();
                BattleLogMediator.Instance.AddKickOffLog(matchUpResult);
            }
            StateAction(BattleState.KickOff);
        }

        protected override void KickOffStateAction()
        {
            base.KickOffStateAction();
        }

        protected override void SetMarkTargetStateAction()
        {

            BattleDataMediator.Instance.ResetMarkTarget();

            var data = GetNextSetMarkTargetState();

            if (data == null)
            {
                Logger.Log("TutorialError!! SetMarkTargetStateが不足");
                return;
            }

            foreach (var c in BattleDataMediator.Instance.OffenceCharacters)
            {
                c.PrimaryParam = c.GetConsParam();
                var markData = data.markDataList.FirstOrDefault(data => data.offenceCharacterId == c.id);
                if (markData == null || markData.defenceCharacterIdList.Count == 0)
                {
                    continue;
                }
                foreach (var defenceMemberId in markData.defenceCharacterIdList)
                {
                    var markCharacter = BattleDataMediator.Instance.DefenceCharacters.FirstOrDefault(data => data.id == defenceMemberId);
                    c.MarkCharacter = markCharacter;
                    markCharacter.MarkedCount++;
                }
            }

            StateAction(BattleState.JustRunAction);
        }

        protected override void JustRunActionStateAction()
        {
            var settingData = GetNextJustRunActionState();

            if (settingData == null)
            {
                Logger.Log("TutorialError!! JustRunActionStateが不足");
                return;
            }

            var dribbleResult = settingData.matchUpResult;
            var digestController = BattleDigestController.Instance as TutorialBattleDigestController;
            digestController.SetTutorialPlaySpeed(settingData.normalDigestSpeed);

            BattleDataMediator.Instance.ApplyBattleMatchUpResult(dribbleResult);
            BattleLogMediator.Instance.AddMatchUpResultLog(dribbleResult);

            StateAction(BattleState.SelectMatchUpAction);

        }


        protected override async void JudgeMatchUpFinalResultStateAction()
        {
            if (selectMatchUpActionData == null) return;
            var matchUpResult = selectMatchUpActionData.matchUpResult;
            var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();

            for (var i = 0; i < matchUpResult.replaceAbilityIds.Count; i++)
            {
                if (matchUpResult.replaceAbilityIds.Count <= i || matchUpResult.replaceCharacterIds.Count <= i || matchUpResult.replaceDigestTimings.Count <= i)
                {
                    break;
                }

                var abilityId = matchUpResult.replaceAbilityIds[i];
                var charaId = matchUpResult.replaceCharacterIds[i];
                var digestTiming = matchUpResult.replaceDigestTimings[i];
                var chara = BattleDataMediator.Instance.GetBattleCharacter(charaId);
                var ability = chara?.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.id == abilityId);
                if (chara == null || ability == null)
                {
                    break;
                }

                matchUpResult.AddReplaceDigest(digestTiming, chara, ability);
            }

            for (var i = 0; i < matchUpResult.insertAbilityIds.Count; i++)
            {
                if (matchUpResult.insertAbilityIds.Count <= i || matchUpResult.insertCharacterIds.Count <= i)
                {
                    break;
                }

                var abilityId = matchUpResult.insertAbilityIds[i];
                var charaId = matchUpResult.insertCharacterIds[i];
                var chara = BattleDataMediator.Instance.GetBattleCharacter(charaId);
                var ability = chara?.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.id == abilityId);
                if (chara == null || ability == null)
                {
                    break;
                }

                matchUpResult.AddInsertDigest(chara, ability);
            }

            // TODO 1試合目の馬狼はマッチアップあり
            if (matchUpResult.OffenceAbilityId <= 0)
            {
                BattleLogMediator.Instance.AddMatchUpActivatedLog(matchUpResult);
            }

            BattleDataMediator.Instance.ApplyBattleMatchUpResult(matchUpResult);
            BattleLogMediator.Instance.AddAbilityLog(matchUpResult.BeforeAbilityLogs);
            BattleLogMediator.Instance.AddMatchUpResultLog(matchUpResult);
            BattleLogMediator.Instance.AddAbilityLog(matchUpResult.AfterAbilityLogs);
            BattleDataMediator.Instance.ApplyFieldDataByMatchUpResult(matchUpResult);

            // 最終処理しきったのでアクセス出来ないように.
            BattleDataMediator.Instance.NextMatchUpResult.ResetActivatedAbilityData();
            selectMatchUpActionData = null;

            if (matchUpResult.AddEndLog)
            {
                BattleLogMediator.Instance.AddBattleEndLog();
                StateAction(BattleState.Finish);
                return;
            }

            await UniTask.DelayFrame(3);
            BattleLogMediator.Instance.AddNextMatchUpLog();
        }


        protected override void JudgeMatchUpPreResultStateAction(BattleConst.MatchUpActionType offenceAction,
            long targetCharacterId, BattleConst.MatchUpActionDetailType actionDetailType)
        {
            var offenceCharacter = BattleDataMediator.Instance.GetBallOwner();
            selectMatchUpActionData = GetNextSelectMatchUpActionState();
            var matchUpResult = selectMatchUpActionData.matchUpResult;

            BattleDataMediator.Instance.NextMatchUpResult = matchUpResult;

            var logList = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();

            for (var i = 0; i < matchUpResult.preResultAbilityIds.Count; i++)
            {
                if (matchUpResult.preResultAbilityIds.Count <= i || matchUpResult.preResultCharacterIds.Count <= i)
                {
                    break;
                }
                var abilityId = matchUpResult.preResultAbilityIds[i];
                var charaId = matchUpResult.preResultCharacterIds[i];
                var chara = BattleDataMediator.Instance.GetBattleCharacter(charaId);
                var ability = chara?.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.id == abilityId);
                if (chara == null || ability == null)
                {
                    break;
                }
                logList.Add(new Tuple<BattleCharacterModel, BattleAbilityModel>(chara, ability));
            }

            BattleLogMediator.Instance.AddAbilityLog(logList);

            // 必殺技発動なしだったら介入要素がないためそのまま最終ジャッジまでつなげる.
            if (matchUpResult.OffenceAbilityId <= 0 || BattleDataMediator.Instance.IsSkipToFinish)
            {
                StateAction(BattleState.JudgeMatchUpFinalResult);
            }
            // 必殺技発動アリだったら介入の結果変わる可能性があるため一時演出
            else
            {
                BattleLogMediator.Instance.AddMatchUpActivatedLog(matchUpResult);
                BattleLogMediator.Instance.AddPreMatchUpResultLog(matchUpResult);
            }
        }

        #region TutorialLogic
        private void LotteryPlayers(List<BattleCharacterModel> teamCharacters, List<BattleCharacterModel> refCharacters, long ballOwnerId, List<int> offenceCharacterIds, bool offence, bool isStartAtKickOff)
        {
            // オーナー追加
            var ballOwner = teamCharacters.Find(character => character.id == ballOwnerId);
            if (ballOwner != null)
            {
                refCharacters.Add(ballOwner);
            }

            foreach (var id in offenceCharacterIds)
            {
                var character = teamCharacters.FirstOrDefault(character => character.id == id);
                if (character != null && refCharacters.Contains(character))
                {
                    continue;
                }
                refCharacters.Add(character);
            }

            if (!offence)
            {
                // ディフェンス側はマーク時に縦軸合わせ
                return;
            }
        }

        private void AddStaminaValueList(Dictionary<long, BigValue> dic, List<long> ids, float value)
        {
            foreach (var id in ids)
            {
                AddStaminaValue(dic, id, value);
            }
        }

        private void AddStaminaValue(Dictionary<long, BigValue> dic, long id, float value)
        {
            if (!dic.ContainsKey(id))
            {
                dic[id] = BigValue.Zero;
            }
            dic[id] += (BigValue)(BattleDataMediator.Instance.AverageMaxStamina * value);
        }

        private void SetChangedStaminaDictionary(ref TutorialSettings.TutorialBattleMatchUpResult matchUpResult)
        {
            var dic = new Dictionary<long, BigValue>();
            var type = matchUpResult.ActionType;
            if (type == BattleConst.MatchUpActionType.None)
            {
                AddStaminaValueList(dic, matchUpResult.RoundOffenceCharacterIds, BattleConst.AdjustableValueRecoveryStaminaValueOnAttack);
                AddStaminaValueList(dic, matchUpResult.RoundDefenceCharacterIds, BattleConst.AdjustableValueRecoveryStaminaValueOnAttack);
                AddStaminaValue(dic, matchUpResult.NextBallOwnerId, BattleConst.AdjustableValueConsumeStaminaValueOnAttack);
                AddStaminaValue(dic, matchUpResult.NextBallOwnerId, -BattleConst.AdjustableValueRecoveryStaminaValueOnAttack);
            }
            else
            {
                AddStaminaValue(dic, matchUpResult.OffenceCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForMatchUp);
                AddStaminaValue(dic, matchUpResult.DefenceCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForMatchUp);
                switch (type)
                {
                    case BattleConst.MatchUpActionType.Pass:
                        AddStaminaValue(dic, matchUpResult.OffenceCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForPass);
                        if (matchUpResult.DefenceActionCharacterId > 0)
                            AddStaminaValue(dic, matchUpResult.DefenceActionCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForPassCut);
                        break;
                    case BattleConst.MatchUpActionType.Through:
                        break;
                    case BattleConst.MatchUpActionType.Shoot:
                        AddStaminaValue(dic, matchUpResult.OffenceCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForShoot);
                        if (matchUpResult.DefenceActionCharacterId > 0)
                            AddStaminaValue(dic, matchUpResult.DefenceActionCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForShootBlock);
                        break;
                    case BattleConst.MatchUpActionType.Cross:
                        AddStaminaValue(dic, matchUpResult.OffenceCharacterId, BattleConst.AdjustableValueConsumeStaminaValueForCross);
                        break;
                }
            }
            matchUpResult.ChangedStaminaDict = dic;
        }
        #endregion
    }
}