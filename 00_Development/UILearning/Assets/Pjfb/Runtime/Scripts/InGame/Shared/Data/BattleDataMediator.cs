using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Pjfb.Networking.App.Request;

#if !PJFB_LAMBDA
using MagicOnion;
using Pjfb.InGame.ClubRoyal;
#endif

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
using CodeStage.AntiCheat.ObscuredTypes;
using Pjfb.Storage;
#endif

namespace Pjfb.InGame
{
    public class BattleDataMediator
    {
        public static BattleDataMediator Instance
        {
            get;
            private set;
        }

        public void Release()
        {
            Instance = null;
        }
        
        public BattleDataMediator()
        {
            // APIレスポンスをセットするところからになるとおもうけど一旦.
            Instance = this;
            BattleType = BattleConst.BattleType.None;

            Players = new List<BattlePlayerModel>();
            Decks = new List<List<BattleCharacterModel>> { new (), new ()};
            BluelockMans = new List<BattleBluelockManModel>();

            OffenceSide = BattleGameLogic.GetInitialAttackerSide(BattleType, 0);
        }

        public void SetAsReplayDigestMode()
        {
            IsReplayDigestMode = true;
            IsSpeedUpPlayerDigest = false;
            IsDoubleSpeed = false;
            IsSkipToFinish = false;
            IsSkipToFinishWithoutView = false;
        }

        public void SetAsReplayGameMode()
        {
            IsReplayGameMode = true;
            IsSkipToFinish = false;
            IsSkipToFinishWithoutView = false;
        }

        #region BattleData バトルを開始するにあたり必要となるデータ. 基本的な部分はバトル内で不変.

        public BattleConst.BattleType BattleType;
        public List<BattlePlayerModel> Players;
        public List<List<BattleCharacterModel>> Decks;
        public BattleV2Ability[] Abilities;
        public List<BattleBluelockManModel> BluelockMans;
        private int Seed;

        public System.Random Random;

        #endregion
        
        #region InGameData バトル中の一時データ. 状況によってゴリゴリ変わったり変わらなかったり.

        public List<int> Score = new List<int> { 0, 0 };
        public List<long> ScoreLog = new List<long>();
        public float GameTime;
        public int BallPosition;
        public long BallOwnerCharacterId;
        public BattleConst.TeamSide OffenceSide;
        public BigValue AverageMaxStamina; // 参加者の最大スタミナ平均
        public bool IsExecutedMatchUpAfterKickOff;

        public BattleConst.TeamSide PlayerSide = BattleConst.TeamSide.Left;
        public BattleConst.TeamSide EnemySide = BattleConst.TeamSide.Right;

        public List<BattleCharacterModel> OffenceCharacters;
        public List<BattleCharacterModel> DefenceCharacters;

        // 以下設定系, ここから移すかも. BattleDataではないので.
        public bool IsReleaseAsset = true;
        public bool IsAutoSwipe;
        public bool IsSpeedUpPlayerDigest;
        public bool IsDoubleSpeed;
        public float PlaySpeed => IsDoubleSpeed ? 2.0f : 1.0f;

        public long ReplayTargetCharacterId;
        public bool IsReplayMode => IsReplayDigestMode || IsReplayGameMode;
        public bool IsReplayDigestMode;
        public bool IsReplayGameMode;
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public NewInGameResultPage.ResultParam ResultParam;
#endif
        
        /// <summary>スキップ</summary>
        public bool IsSkipToFinish;
        /// <summary>スキップ(ビューを表示しない)</summary>
        public bool IsSkipToFinishWithoutView;
        
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID 
        public int DefaultBgmVolume => LocalSaveManager.saveData.appConfig.bgmVolume;
        public bool Is2DFieldViewMode => LocalSaveManager.saveData.appConfig.fieldDisplayType == 1;
#endif
        
#if !PJFB_REL
        public bool ForceFlatPlaySpeed = false;
        public bool StopLogPlay = false;
        public bool ForceSuccessMatchUp = false;
        public bool ForceFailMatchUp = false;
        public bool DontConsumeStamina = false;
        public bool ShowPassiveInvokeRate = false;
        public bool ForceSuccessPassiveInvokeByRate = false;
        public bool ForceSuccessPassiveEffectInvoke = false;
        public bool ForceSuccessActiveInvokeByRate = false;
        public bool IgnoreShootRange = false;
        public bool ForceJoinFullMemberToRound = false;
        public bool AddMatchUpActionWeightThrough = false;
        public bool AddMatchUpActionWeightPass = false;
        public bool AddMatchUpActionWeightShoot = false;
        public bool AddMatchUpActionWeightCross = false;
        public int ForceMatchUpType = -1;
        public List<long> ForceActivateAbilityIds = new List<long>();
        public bool ForceDontActivateAbilities = false;
        public bool DontWasteTime = false;
        public bool DontAddScore = false;
#endif

        public BattleMatchUpResult NextMatchUpResult = new BattleMatchUpResult();
        public List<BattleMatchUpCommandData> NextCommandData = new List<BattleMatchUpCommandData>();
        public int CommandData;
        public BattleV2ClientData OriginalBattleV2ClientData;
        
#if !MAGIC_ONION_SERVER
        public void SetBattleType(PageType pageType)
        {
            switch (pageType)
            {
                case PageType.Story:
                    BattleType = BattleConst.BattleType.StoryBattle;
                    break;
                case PageType.Rivalry:
                    BattleType = BattleConst.BattleType.RivalryBattle;
                    break;
                case PageType.Training:
                    BattleType = BattleConst.BattleType.TrainingBattle;
                    break;
                case PageType.Colosseum:
                case PageType.ClubMatch:
                    BattleType = BattleConst.BattleType.VersusPlayerBattle;
                    break;
                case PageType.LeagueMatch:
                    BattleType = BattleConst.BattleType.ReplayLeagueMatch;
                    break;
                // モーダルから実行する都合上,練習試合導線がいろいろあるので…
                default:
                    BattleType = BattleConst.BattleType.VersusPlayerBattle;
                    break;
            }
        }
#endif
        
        public void InitializeGameData(BattleV2ClientData battleData)
        {
            OriginalBattleV2ClientData = battleData;
            ParseClientData(battleData);
            InitializeInGameData();
        }
        
#if !PJFB_LAMBDA
        public void InitializeGuildBattleGameData(BattleV2ClientData battleData, GuildBattlePartyModel leftParty, GuildBattlePartyModel rightParty, int battleCount)
        {
            OriginalBattleV2ClientData = battleData;
            ParsePartialClientData(battleData, leftParty, rightParty, battleCount);
            InitializeInGameData();
        }
#endif
        
        private void ParseClientData(BattleV2ClientData battleData)
        {
            Seed = (int)battleData.seedNum;
            Random = new System.Random(Seed);

            Players = new List<BattlePlayerModel>(new BattlePlayerModel[battleData.playerList.Length]);
            foreach (var playerData in battleData.playerList)
            {
                var player = new BattlePlayerModel(playerData);
                // indexって名前だけど1スタートなので…
                var index = playerData.playerIndex - 1;
                Players[(int)index] = player;
            }
            
            Abilities = battleData.abilityList;

            foreach (var charaData in battleData.charaList)
            {
                // キャラインデックスって仕組みからエースかどうかの判定エグい…やめてほしい…
                var character = new BattleCharacterModel(charaData, charaData.charaIndex % 5 == 1);
                var player = Players[(int)charaData.playerIndex - 1];
                character.SetPlayerData(player.UserId, player.Index, player.Side);
                character.SetAbilityData(Abilities, charaData.abilityList);
                Decks[(int)character.Side].Add(character);
            }

            foreach (var blmData in battleData.battleFbKeeperList)
            {
                BluelockMans.Add(new BattleBluelockManModel(blmData));
            }

            AverageMaxStamina = (BigValue)(Decks.Max(deck => deck.Sum(chara => chara.MaxStamina) / deck.Count));
            BattleGameLogic.SetAbilityInvokeRateCoefficient(Decks);
        }
        
#if !PJFB_LAMBDA
        private void ParsePartialClientData(BattleV2ClientData battleData, GuildBattlePartyModel leftParty, GuildBattlePartyModel rightParty, int battleCount)
        {
            Seed = (int)battleData.seedNum + battleCount;
            Random = new System.Random(Seed);

            Players = new List<BattlePlayerModel>(new BattlePlayerModel[2]);
            foreach (var playerData in battleData.playerList)
            {
                // 関係ないプレイヤーはいれない.
                if (playerData.playerId != leftParty.PlayerId && playerData.playerId != rightParty.PlayerId)
                {
                    continue;
                }
                
                var player = new BattlePlayerModel(playerData);
                var isLeftPlayer = playerData.playerId == leftParty.PlayerId;
                var playerIndex = (int)(isLeftPlayer ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right);
                player.SetIndex(playerIndex);
                player.SetTactics((int)(isLeftPlayer ? leftParty.TacticsId : rightParty.TacticsId));
                Players[playerIndex] = player;
            }
            
            Abilities = battleData.abilityList;

            var leftPlayer = Players[(int)BattleConst.TeamSide.Left];
            var leftPartyWinStreakPenalty = GuildBattleCommonLogic.GetWinStreakStatusPenaltyRatio(leftParty.WinStreakCount);
            for (var i = 0; i < 5; i++)
            {
                var charaId = leftParty.UCharaIds[i];
                var charaData = battleData.charaList.FirstOrDefault(chara => chara.id == charaId);
                var character = new BattleCharacterModel(charaData, i == 0);
                character.SetId(i + 1);
                character.SetPlayerData(leftPlayer.UserId, leftPlayer.Index, leftPlayer.Side);
                character.SetAbilityData(Abilities, charaData.abilityList);
                character.Position = (BattleConst.PlayerPosition)leftParty.RoleOperationIds[i];
                Decks[(int)character.Side].Add(character);

                if (leftPartyWinStreakPenalty < 1.0f)
                {
                    character.baseSpeed = BigValue.CalculationRound(character.baseSpeed, leftPartyWinStreakPenalty);
                    character.baseTechnique = BigValue.CalculationRound(character.baseTechnique, leftPartyWinStreakPenalty);
                    character.basePhysical = BigValue.CalculationRound(character.basePhysical, leftPartyWinStreakPenalty);
                    character.baseKick = BigValue.CalculationRound(character.baseKick, leftPartyWinStreakPenalty);
                    character.baseWise = BigValue.CalculationRound(character.baseWise, leftPartyWinStreakPenalty);
                    character.baseStamina = BigValue.CalculationRound(character.baseStamina, leftPartyWinStreakPenalty);
                    character.ReCalculateParam();
                }
            }
            
            var rightPlayer = Players[(int)BattleConst.TeamSide.Right];
            var rightPartyWinStreakPenalty = GuildBattleCommonLogic.GetWinStreakStatusPenaltyRatio(rightParty.WinStreakCount);
            for (var i = 0; i < 5; i++)
            {
                var charaId = rightParty.UCharaIds[i];
                var charaData = battleData.charaList.FirstOrDefault(chara => chara.id == charaId);
                var character = new BattleCharacterModel(charaData, i == 0);
                character.SetId(i + 6);
                character.SetPlayerData(rightPlayer.UserId, rightPlayer.Index, rightPlayer.Side);
                character.SetAbilityData(Abilities, charaData.abilityList);
                character.Position = (BattleConst.PlayerPosition)rightParty.RoleOperationIds[i];
                Decks[(int)character.Side].Add(character);
                
                if (rightPartyWinStreakPenalty < 1.0f)
                {
                    character.baseSpeed = BigValue.CalculationRound(character.baseSpeed, rightPartyWinStreakPenalty);
                    character.baseTechnique = BigValue.CalculationRound(character.baseTechnique, rightPartyWinStreakPenalty);
                    character.basePhysical = BigValue.CalculationRound(character.basePhysical, rightPartyWinStreakPenalty);
                    character.baseKick = BigValue.CalculationRound(character.baseKick, rightPartyWinStreakPenalty);
                    character.baseWise = BigValue.CalculationRound(character.baseWise, rightPartyWinStreakPenalty);
                    character.baseStamina = BigValue.CalculationRound(character.baseStamina, rightPartyWinStreakPenalty);
                    character.ReCalculateParam();
                }
            }

            foreach (var blmData in battleData.battleFbKeeperList)
            {
                BluelockMans.Add(new BattleBluelockManModel(blmData));
            }

            AverageMaxStamina = (BigValue)(Decks.Max(deck => deck.Sum(chara => chara.MaxStamina) / deck.Count));
            BattleGameLogic.SetAbilityInvokeRateCoefficient(Decks);
            
            GuildBattlePlayerData? leftPlayerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayerData(leftParty.PlayerIndex);
            GuildBattlePlayerData? rightPlayerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayerData(rightParty.PlayerIndex);
            if (leftPlayerData != null)
            {
                PjfbGuildBattleLogic.ApplyStatusAdviserAbilityEffects(Decks[(int)BattleConst.TeamSide.Left], leftPlayerData.GuildBattleActivatedAbilityList);
            }

            if (rightPlayerData != null)
            {
                PjfbGuildBattleLogic.ApplyStatusAdviserAbilityEffects(Decks[(int)BattleConst.TeamSide.Right], rightPlayerData.GuildBattleActivatedAbilityList);
            }
        }
#endif

        public void SetPlayerSide(long playerSide)
        {
            PlayerSide = playerSide == 0 ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right;
            EnemySide = playerSide == 0 ? BattleConst.TeamSide.Right : BattleConst.TeamSide.Left;
        }
        
        public void InitializeInGameData()
        {
            GameTime = 0.0f;
            OffenceSide = BattleConst.TeamSide.TeamSizeMax; // どちらから攻撃か抽選のため.
            
            OffenceCharacters = new List<BattleCharacterModel>();
            DefenceCharacters = new List<BattleCharacterModel>();
            
            ResetMarkTarget();
            ResetRoundData();
            ResetAfterGoal();
        }
        
        public void ResetAfterGoal()
        {
            BallPosition = BattleConst.FieldSize / 2; // 開始位置はフィールド中央.
        }

        public void ResetMarkTarget()
        {
            foreach (var deck in Decks)
            {
                foreach (var character in deck)
                {
                    character.MarkCharacter = null;
                    character.MarkedCount = 0;
                }
            }
        }
        
        public void ResetRoundData(long stayVerticalPositionCharacterId = -1)
        {
            OffenceCharacters.Clear();
            DefenceCharacters.Clear();
            foreach (var deck in Decks)
            {
                foreach (var character in deck)
                {
                    character.ActionTargetCharacter = null;
                    character.MarkCharacter = null;
                    character.MarkedCount = 0;
                    character.ClearedNumOnRound = -1;
                }
            }
        }

        #endregion
        
        #region データ取得系

        public BattleCharacterModel GetBattleCharacter(long uCharaId)
        {
            foreach (var deck in Decks)
            {
                var target = deck.FirstOrDefault(character => character.id == uCharaId);
                if (target != null)
                {
                    return target;
                }
            }

            return null;
        }
        
        public BattleCharacterModel GetBattleCharacter(BattleConst.TeamSide side, long uCharaId)
        {
            return GetTeamDeck(side).FirstOrDefault(chara => chara.id == uCharaId);
        }
        
        /// <summary>チームで一番特定ステータスの高いキャラの取得</summary>
        public BattleCharacterModel GetHighestStatusCharacter(BattleConst.TeamSide side, BattleConst.StatusParamType statusType)
        {
            return GetTeamDeck(side).OrderByDescending(chara => chara.GetCurrentParameter(statusType)).FirstOrDefault();
        }
        
        /// <summary>チームで一番特定ステータスの低いキャラの取得</summary>
        public BattleCharacterModel GetLowestStatusCharacter(BattleConst.TeamSide side, BattleConst.StatusParamType statusType)
        {
            return GetTeamDeck(side).OrderBy(chara => chara.GetCurrentParameter(statusType)).FirstOrDefault();
        }

        public BattlePlayerModel GetBattlePlayer(BattleConst.TeamSide side)
        {
            return Players[(int)side];
        }

        public List<BattleCharacterModel> GetOffenceDeck()
        {
            switch (OffenceSide)
            {
                case BattleConst.TeamSide.Left:
                    return Decks[(int)BattleConst.TeamSide.Left];
                case BattleConst.TeamSide.Right:
                    return Decks[(int)BattleConst.TeamSide.Right];
            }

            return null;
        }
        
        public List<BattleCharacterModel> GetDefenceDeck()
        {
            switch (OffenceSide)
            {
                case BattleConst.TeamSide.Left:
                    return Decks[(int)BattleConst.TeamSide.Right];
                case BattleConst.TeamSide.Right:
                    return Decks[(int)BattleConst.TeamSide.Left];
            }

            return null;
        }

        public List<BattleCharacterModel> GetTeamDeck(BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return null;
            }

            return Decks[(int)side];
        }

        public BattleBluelockManModel GetBluelockManModel(BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return null;
            }

            return BluelockMans[(int)side];
        }

        public BattleCharacterModel GetBallOwner()
        {
            return GetRoundOffenceCharacters().FirstOrDefault(character => character.id == BallOwnerCharacterId);
        }

        public List<BattleCharacterModel> GetRoundCharacters(BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return null;
            }

            return side == OffenceSide ? OffenceCharacters : DefenceCharacters;
        }
        
        public List<BattleCharacterModel> GetRoundOffenceCharacters()
        {
            return OffenceCharacters;
        }

        public List<BattleCharacterModel> GetRoundDefenceCharacters()
        {
            return DefenceCharacters;
        }
        
        public BattleConst.TeamSide GetCharacterSide(BattleCharacterModel character)
        {
            if (Decks[(int)BattleConst.TeamSide.Left].Contains(character))
            {
                return BattleConst.TeamSide.Left;
            }
            
            if (Decks[(int)BattleConst.TeamSide.Right].Contains(character))
            {
                return BattleConst.TeamSide.Right;
            }

            return BattleConst.TeamSide.TeamSizeMax;
        }

        public BattleConst.TeamSide GetOtherSide(BattleCharacterModel character)
        {
            return BattleGameLogic.GetOtherSide(GetCharacterSide(character));
        }

        public int GetScore(BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return 0;
            }

            return Score[(int)side];
        }

        public int GetBallPositionFromAllyGoal(BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.TeamSizeMax)
            {
                return -1;
            }

            if (side == BattleConst.TeamSide.Left)
            {
                return BallPosition;
            }
            else
            {
                return BattleConst.FieldSize - BallPosition;
            }
        }
        


        public void ApplyBattleMatchUpResult(BattleMatchUpResult result)
        {
            BallOwnerCharacterId = result.NextBallOwnerId;
            BallPosition = result.NextBallPosition;
            GameTime += result.ChangedGameTimeValue;
#if !PJFB_REL
            if (Instance.DontWasteTime)
            {
                GameTime -= result.ChangedGameTimeValue;
            }
#endif

            // キーでのアクセスにしたいのでデッキ/キャラクターからの走査
            foreach (var deck in Decks)
            {
                foreach (var character in deck)
                {
                    if (result.ChangedStaminaDict.TryGetValue(character.id, out var changedStaminaValue))
                    {
                        character.CurrentStamina = BigValue.Clamp(character.CurrentStamina - changedStaminaValue, BigValue.Zero, character.MaxStamina);
                    }
                }
            }
            
            if (result.ScoredCharacterId > 0)
            {
                var scoredCharacter = GetBattleCharacter(result.ScoredCharacterId);
                if (scoredCharacter != null)
                {
                    scoredCharacter.ScoredCount++;
                    Score[(int)scoredCharacter.Side]++;
                    ScoreLog.Add(scoredCharacter.id);
#if !PJFB_REL
                    if (Instance.DontAddScore)
                    {
                        Score[(int)scoredCharacter.Side]--;
                    }
#endif
                }
            }
        }

        public void ApplyFieldDataByMatchUpResult(BattleMatchUpResult result)
        {
            if (result.ScoredCharacterId > 0)
            {
                ResetAfterGoal();
            }
            
            if (result.IsSideChanged)
            {
                OffenceSide = BattleGameLogic.GetOtherSide(OffenceSide);
            }

            if (result.IsResetRound)
            {
                ResetRoundData(result.NextBallOwnerId);
            }
        }

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID 
        public StoryUtility.BattleResultType GetBattleResult()
        {
            var leftSideScore = Score[(int) BattleConst.TeamSide.Left];
            var rightSideScore = Score[(int) BattleConst.TeamSide.Right];

            if (leftSideScore == rightSideScore)
            {
                return StoryUtility.BattleResultType.Draw;
            }

            return leftSideScore > rightSideScore ? StoryUtility.BattleResultType.Win : StoryUtility.BattleResultType.Lose;
        }
#endif
        
        public BattleCharacterModel GetMVPCharacter(BattleConst.TeamSide side)
        {
            var targetIds = Decks[(int)side].Select(chara => chara.id).ToList();
            
            var sum = new Dictionary<long, int>();
            foreach (var charaId in ScoreLog)
            {
                if (!targetIds.Contains(charaId))
                {
                    continue;
                }
                
                if (!sum.ContainsKey(charaId))
                {
                    sum.Add(charaId, 0);
                }

                sum[charaId]++;
            }

            if (!sum.Any())
            {
                return null;
            }

            // 合計得点数 > id若い順(同得点の場合はプレイヤー側, キャプテンが優先されるように.)
            var targetId = sum.OrderByDescending(kvp => kvp.Value).ThenBy(kvp => kvp.Key).FirstOrDefault().Key;
            return GetBattleCharacter(targetId);
        }

        /// <summary>
        /// index0を基準にした勝(1)敗(2)分け(3)
        /// </summary>
        /// <returns></returns>
        public BattleConst.BattleResult GetBattleResultType()
        {
            var leftScore = Score[(int)BattleConst.TeamSide.Left];
            var rightScore = Score[(int)BattleConst.TeamSide.Right];

            if (leftScore == rightScore)
            {
                return BattleConst.BattleResult.Draw;
            }

            return leftScore > rightScore ? BattleConst.BattleResult.WinLeft : BattleConst.BattleResult.WinRight;
        }
        
#if !PJFB_LAMBDA
        public BattleConst.BattleResult GetGuildBattleSpotBattleResultType(GuildBattlePartyModel leftParty, GuildBattlePartyModel rightParty)
        {
            var leftScore = Score[(int)BattleConst.TeamSide.Left];
            var rightScore = Score[(int)BattleConst.TeamSide.Right];
            
            if(leftParty.GetBallCount() <= rightScore)
            {
                if (Score[(int)BattleConst.TeamSide.Right] <= Score[(int)BattleConst.TeamSide.Left])
                {
                    Score[(int)BattleConst.TeamSide.Right] = BattleConst.RequiredScore;
                }
                return BattleConst.BattleResult.WinRight;
            }
            
            if(rightParty.GetBallCount() <= leftScore)
            {
                if (Score[(int)BattleConst.TeamSide.Left] <= Score[(int)BattleConst.TeamSide.Right])
                {
                    Score[(int)BattleConst.TeamSide.Left] = BattleConst.RequiredScore;
                }
                return BattleConst.BattleResult.WinLeft;
            }

            if (leftScore != rightScore)
            {
                return leftScore > rightScore ? BattleConst.BattleResult.WinLeft : BattleConst.BattleResult.WinRight;
            }

            var leftTotalPoint = 0L;
            foreach (var uCharaId in leftParty.UCharaIds)
            {
                var charaData = PjfbGuildBattleDataMediator.Instance.BattleCharaData[uCharaId];
                leftTotalPoint += charaData.combatPower;
            }
            leftTotalPoint *= leftParty.GetBallCount();

            var rightTotalPoint = 0L;
            foreach (var uCharaId in rightParty.UCharaIds)
            {
                var charaData = PjfbGuildBattleDataMediator.Instance.BattleCharaData[uCharaId];
                rightTotalPoint += charaData.combatPower;
            }
            rightTotalPoint *= leftParty.GetBallCount();

            var winTeamSide = leftTotalPoint > rightTotalPoint ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right;
            var loseTeamSide = BattleGameLogic.GetOtherSide(winTeamSide);
            var winParty = winTeamSide == BattleConst.TeamSide.Left ? leftParty : rightParty;
            var loseParty = winTeamSide == BattleConst.TeamSide.Left ? rightParty : leftParty;
            
            // 3点を最大として、敵のスコアまでの値でとる
            Instance.Score[(int)winTeamSide] = Math.Min(loseParty.GetBallCount(), BattleConst.RequiredScore);
            // 両方しぬことがない&&スコア的におかしくならないように, 勝ったほうのボール数-1か相手のスコア-1の小さい方
            Instance.Score[(int)loseTeamSide] = Math.Min(winParty.GetBallCount() - 1, Instance.Score[(int)winTeamSide] - 1);

            return winTeamSide == BattleConst.TeamSide.Left ? BattleConst.BattleResult.WinLeft : BattleConst.BattleResult.WinRight;
        }
#endif

        public int GetLeftTeamPointGet()
        {
            return Score[(int)BattleConst.TeamSide.Left];
        }

        public int GetLeftTeamPointLost()
        {
            return Score[(int)BattleConst.TeamSide.Right];
        }
        
        /// <summary>オフェンス参加率取得</summary>
        public float GetForwardJoinCoefficient(BattleConst.PlayerPosition position)
        {
            float joinCoefficient = 0;
            if (OriginalBattleV2ClientData.offenseParticipationCoefficientList.Length >= (int)position)
            {
                // indexは0からなので-1する
                switch (position)
                {
                    case BattleConst.PlayerPosition.FW:
                        joinCoefficient = OriginalBattleV2ClientData.offenseParticipationCoefficientList[(int)BattleConst.PlayerPosition.FW - 1];
                        break;
                    case BattleConst.PlayerPosition.MF:
                        joinCoefficient = OriginalBattleV2ClientData.offenseParticipationCoefficientList[(int)BattleConst.PlayerPosition.MF - 1];
                        break;
                    case BattleConst.PlayerPosition.DF:
                        joinCoefficient = OriginalBattleV2ClientData.offenseParticipationCoefficientList[(int)BattleConst.PlayerPosition.DF - 1];
                        break;
                }
            }

            // 万分率なので10000で割る
            return joinCoefficient / 10000;
        }
        
        /// <summary>ディフェンス参加率取得</summary>
        public float GetDefenceJoinCoefficient(BattleConst.PlayerPosition position)
        {
            float joinCoefficient = 0;
            if (OriginalBattleV2ClientData.defenseParticipationCoefficientList.Length >= (int)position)
            {
                // indexは0からなので-1する
                switch (position)
                {
                    case BattleConst.PlayerPosition.FW:
                        joinCoefficient = OriginalBattleV2ClientData.defenseParticipationCoefficientList[(int)BattleConst.PlayerPosition.FW - 1];
                        break;
                    case BattleConst.PlayerPosition.MF:
                        joinCoefficient = OriginalBattleV2ClientData.defenseParticipationCoefficientList[(int)BattleConst.PlayerPosition.MF - 1];
                        break;
                    case BattleConst.PlayerPosition.DF:
                        joinCoefficient = OriginalBattleV2ClientData.defenseParticipationCoefficientList[(int)BattleConst.PlayerPosition.DF - 1];
                        break;
                }
            }
            
            // 万分率なので10000で割る
            return joinCoefficient / 10000;
        }
        
        #endregion

        #region Static Methods

        public static string GetRawStringByCompressedString(string compressedString)
        {
            var downloadedCompressedData = Convert.FromBase64String(compressedString);
            var decompressedData = DecompressByGZip(downloadedCompressedData);
            return Encoding.UTF8.GetString(decompressedData);
        }
        
        private static byte[] DecompressByGZip(byte[] src)
        {
            using (var ms = new MemoryStream(src))
            using (var gs = new GZipStream(ms, CompressionMode.Decompress))
            {
                using (var dest = new MemoryStream())
                {
                    gs.CopyTo(dest);

                    dest.Position = 0;
                    var decompressed = new byte[dest.Length];
                    dest.Read(decompressed, 0, decompressed.Length);
                    return decompressed;
                }
            }
        }

        #endregion
        
    }
}