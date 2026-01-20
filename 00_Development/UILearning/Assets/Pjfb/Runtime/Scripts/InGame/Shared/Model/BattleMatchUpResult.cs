using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.InGame
{
    [Serializable]
    public class BattleMatchUpResult
    {
        public long OffenceCharacterId;
        public long DefenceCharacterId;
        public long OffenceAbilityUserCharacterId;
        public bool IsSwiped;
        public bool IsInShootRange;
        public int RemainDistanceToShoot;
        public bool IsProgress;
        public long OffenceAbilityId;
        public long DefenceAbilityId;
        public List<Tuple<BattleConst.DigestTiming, BattleCharacterModel, BattleAbilityModel>> InsertDigests;
        public List<Tuple<BattleConst.DigestTiming, BattleCharacterModel, BattleAbilityModel>> ReplaceDigests;
        public List<long> JoinLooseBallCompetitionCharacterIds;
        public BattleConst.MatchUpActionType ActionType;
        public BattleConst.MatchUpActionDetailType ActionDetailType;
        public BattleConst.AbilityEvaluateTimingType ActivateAbilityTimingType;
        public BattleConst.MatchUpDigestType MatchUpDigestType;
        public BattleConst.MatchUpResult MatchUpResult;
        public BattleConst.BallInterferenceType ShootBlockType;
        public BattleConst.BallInterferenceType BallInterferenceType;
        public long ScoredCharacterId;
        public long NextBallOwnerId;
        public long TargetCharacterId;
        public long DefenceActionCharacterId;
        public bool IsNicePass;
        public int NextBallPosition;
        public float ChangedGameTimeValue;
        public bool IsSideChanged;
        public bool IsResetRound;
        public Dictionary<long, BigValue> ChangedStaminaDict; // Dictionary<uChara.id, changedValue>
        public List<long> RoundOffenceCharacterIds;
        public List<long> RoundDefenceCharacterIds;
        public List<Tuple<BattleCharacterModel, BattleAbilityModel>> BeforeAbilityLogs;
        public List<Tuple<BattleCharacterModel, BattleAbilityModel>> AfterAbilityLogs;

        public BattleMatchUpResult()
        {
            OffenceCharacterId = -1;
            DefenceCharacterId = -1;
            InsertDigests = new List<Tuple<BattleConst.DigestTiming, BattleCharacterModel, BattleAbilityModel>>();
            ReplaceDigests = new List<Tuple<BattleConst.DigestTiming, BattleCharacterModel, BattleAbilityModel>>();
            OffenceAbilityUserCharacterId = -1;
            OffenceAbilityId = -1;
            DefenceAbilityId = -1;
            JoinLooseBallCompetitionCharacterIds = new List<long>();
            ActionType = BattleConst.MatchUpActionType.None;
            ActionDetailType = BattleConst.MatchUpActionDetailType.Type0;
            ActivateAbilityTimingType = BattleConst.AbilityEvaluateTimingType.None;
            MatchUpDigestType = BattleConst.MatchUpDigestType.None;
            MatchUpResult = BattleConst.MatchUpResult.None;
            ShootBlockType = BattleConst.BallInterferenceType.None;
            BallInterferenceType = BattleConst.BallInterferenceType.None;
            ChangedGameTimeValue = 0;
            ScoredCharacterId = -1;
            TargetCharacterId = -1;
            DefenceActionCharacterId = -1;
            IsNicePass = false;
            NextBallOwnerId = -1;
            NextBallPosition = -1;
            ChangedStaminaDict = new Dictionary<long, BigValue>();
            RoundOffenceCharacterIds = new List<long>();
            RoundDefenceCharacterIds = new List<long>();
            BeforeAbilityLogs = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();
            AfterAbilityLogs = new List<Tuple<BattleCharacterModel, BattleAbilityModel>>();
        }

        public void ResetActivatedAbilityData()
        {
            OffenceAbilityId = -1;
            OffenceAbilityUserCharacterId = -1;
            ActivateAbilityTimingType = BattleConst.AbilityEvaluateTimingType.None;
        }

        public void AddChangedStaminaValue(long uCharaId, BigValue changedValue)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.DontConsumeStamina)
            {
                return;
            }
#endif            
            if (!ChangedStaminaDict.ContainsKey(uCharaId))
            {
                ChangedStaminaDict[uCharaId] = BigValue.Zero;
            }
            
            ChangedStaminaDict[uCharaId] += changedValue;
        }

        public void AddInsertDigest(BattleCharacterModel character, BattleAbilityModel ability)
        {
            InsertDigests.Add(new Tuple<BattleConst.DigestTiming, BattleCharacterModel, BattleAbilityModel>(BattleConst.DigestTiming.Special, character, ability));
        }
        
        public void AddReplaceDigest(BattleConst.DigestTiming timing, BattleCharacterModel character, BattleAbilityModel ability)
        {
            ReplaceDigests.Add(new Tuple<BattleConst.DigestTiming, BattleCharacterModel, BattleAbilityModel>(timing, character, ability));
        }
    }
}