using System;
using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;
#if !MAGIC_ONION_SERVER
using Pjfb.UserData;
#endif

namespace Pjfb
{
    public class PjfbGuildBattleDataMediator : GuildBattleCommonDataMediator
    {
        protected static PjfbGuildBattleDataMediator instance;
        public new static PjfbGuildBattleDataMediator Instance => instance;
        
        public BattleV2ClientData OriginalBattleData;
        public new GuildBattleFieldModel BattleField { get; private set; }
        public List<BattlePlayerModel> Players { get; private set; }
        public Dictionary<long, BattlePlayerModel> PjfbBattlePlayers { get; private set; } = new Dictionary<long, BattlePlayerModel>(); // PlayerId, PlayerModel
        public Dictionary<long, GuildBattlePlayerData> PjfbBattlePlayerData { get; protected set; } = new Dictionary<long, GuildBattlePlayerData>(); // PlayerIndex, PlayerData
        public SortedDictionary<int, GuildBattlePartyModel> OnMapPjfbBattleParties { get; protected set; } = new SortedDictionary<int, GuildBattlePartyModel>(); // PartyId, Party
        public Dictionary<int, GuildBattlePartyModel> AllPjfbBattleParties { get; protected set; } = new Dictionary<int, GuildBattlePartyModel>(); // PartyIdentifier, Party
        public Dictionary<int, List<GuildBattlePartyModel>> OnMapPjfbBattlePartyListByPlayerIndex { get; protected set; } = new Dictionary<int, List<GuildBattlePartyModel>>(); // PlayerIndex, Party
        public Dictionary<int, List<GuildBattlePartyModel>> AllPjfbBattlePartyListByPlayerIndex { get; protected set; } = new Dictionary<int, List<GuildBattlePartyModel>>(); // PlayerIndex, Party
        public Dictionary<long, BattleV2Chara> BattleCharaData { get; private set; } = new Dictionary<long, BattleV2Chara>(); // uCharaId, Chara
        public Dictionary<long, List<BattleV2Chara>> BattleCharaDataList { get; private set; } = new Dictionary<long, List<BattleV2Chara>>(); // PlayerIndex, List<Chara>

        // pjshは1プレイヤーにつき1軍師だったので、Listに変更。またBattleCharacterModel > BattleV2Charaに変更
        public Dictionary<int, List<BattleCharacterModel>> Advisers { get; private set; } = new Dictionary<int, List<BattleCharacterModel>>(); // PlayerIndex, BattleCharacter

        public int BattleCount = 0;
        
        public bool IsDataInitialized { get; private set; } = false;

        public static void Initialize()
        {
            instance = new PjfbGuildBattleDataMediator();
            GuildBattleCommonDataMediator.Instance = instance;
            
            // Stub
            instance.GuildBattleSetting = new PjfbGuildBattleSetting();
        }

        public void Release()
        {
            instance = null;
            GuildBattleCommonDataMediator.Instance = null;
        }

        public bool IsBattleDataInitialized()
        {
            return OriginalBattleData != null && IsDataInitialized;
        }

        public void InitializeOnServer(BattleV2ClientData battleData, long matchingId)
        {
            MatchingId = matchingId;
            OriginalBattleData = battleData;
            ApplyGuildBattleSetting(battleData);
            ParseCommonData(battleData);
            SetServerData(battleData);
            
            var fieldSituation = new GuildBattleFieldSituationModel();
            fieldSituation.SetData(RemainTurn, TurnNumber, BattleField.MapSpotsDictionary, OnMapPjfbBattleParties, null, WinningPoints, null, null, null, null);
            LatestSituation = fieldSituation;

            ResultData = new GuildBattleResultData(battleData);
            IsDataInitialized = true;
        }
        
        public bool InitializeOnClient(BattleV2ClientData battleData, DateTime utcStartAt)
        {
            if (battleData == null || OriginalBattleData != null)
            {
                return false;
            }

            ApplyGuildBattleSetting(battleData);
            ParseCommonData(battleData);
            UtcStartAt = utcStartAt;
            OriginalBattleData = battleData;
            
#if !MAGIC_ONION_SERVER
            var userId = UserDataManager.Instance.user.uMasterId;
            var battlePlayer = GetBattlePlayer(userId);
            if (battlePlayer != null)
            {
                playerIndex = battlePlayer.Index;
                playerSide = (GuildBattleCommonConst.GuildBattleTeamSide)battlePlayer.Side;
            }
#endif
            
            IsDataInitialized = true;
            return true;
        }
        
        private void ParseCommonData(BattleV2ClientData battleData)
        {
            Players = new List<BattlePlayerModel>(new BattlePlayerModel[battleData.playerList.Length]);
            BattleCharaData = new Dictionary<long, BattleV2Chara>();
            Advisers = new Dictionary<int, List<BattleCharacterModel>>();

            foreach (BattleV2Player battleV2Player in battleData.playerList)
            {
                var player = new BattlePlayerModel(battleV2Player);
                var index = battleV2Player.playerIndex - 1;
                Players[(int)index] = player;
            }

            foreach (var player in Players)
            {
                PjfbBattlePlayers.Add(player.UserId, player);
                var battlePlayerData = new GuildBattlePlayerData();
                battlePlayerData.SetData(player);
                PjfbBattlePlayerData.Add(player.Index, battlePlayerData);
                BattlePlayerDataById.Add(player.UserId, battlePlayerData);
                BattlePlayerDataByIndex.Add(player.Index, battlePlayerData);
                
                // プレイヤーごとにアドバイザーのリストを初期化。Nullにアクセスしないようにする
                if (!Advisers.ContainsKey(player.Index))
                {
                    Advisers.Add(player.Index, new List<BattleCharacterModel>());
                }
            }

            foreach (var charaData in battleData.charaList)
            {
                // NPCのデータで重複を考慮. 重複させるな.
                BattleCharaData.TryAdd(charaData.id, charaData);
                if (!BattleCharaDataList.ContainsKey(charaData.playerIndex))
                {
                    BattleCharaDataList.Add(charaData.playerIndex, new List<BattleV2Chara>());
                }
                
                // キャラクターのロールがStragegist(アドバイザー)の場合は、プレイヤーインデックスごとにリストに追加
                if ((int)BattleConst.RoleNumberType.Adviser == charaData.roleNumber )
                {
                    if( Advisers.TryGetValue((int)charaData.playerIndex , out List<BattleCharacterModel> targetAdvisers) )
                    {
                        BattleCharacterModel character = new BattleCharacterModel(charaData , false);
                        character.SetAbilityData(battleData.abilityList, charaData.abilityList);
                        // pjfbではアドバイザーにユーザーID,PlayerIndex,Sideを設定しないのでコメントアウト
                        //character.SetPlayerData(charaData.UserId, player.Index, player.Side);
                        targetAdvisers.Add(character);
                    }
                }
                else
                {
                    // アドバイザーは通常のキャラ(選手)とは別枠で扱うため、こちらに登録するのはRoleNumberがAdviser以外のキャラクターのみ
                    // Roleが増えた場合は追加対応が必要になります
                    BattleCharaDataList[charaData.playerIndex].Add(charaData);
                }
            }
            
            // マップ設定
            BattleField = new GuildBattleFieldModel();
            base.BattleField = BattleField;
            BattleField.SetData(battleData);
            
            // 軍師スキルの発動回数などを初期化
            foreach(KeyValuePair<int, List<BattleCharacterModel>> kvp in Advisers)
            {
                GuildBattlePlayerData playerData = PjfbBattlePlayerData[kvp.Key];
                playerData.GuildBattleActivatedAbilityList = new List<GuildBattleAbilityData>();

                foreach (BattleCharacterModel adviser in kvp.Value)
                {
                    foreach (BattleAbilityModel charaAbility in adviser.AbilityList)
                    {
                        BattleV2Ability battleV2Ability = battleData.abilityList.FirstOrDefault(val => val.id == charaAbility.BattleAbilityMaster.id);

                        if (battleV2Ability == null || !GuildBattleAbilityLogic.IsAdviserAbilityType((BattleConst.AbilityType)battleV2Ability.abilityType))
                        {
                            continue;
                        }

                        GuildBattleAbilityData abilityData = new GuildBattleAbilityData()
                        {
                            AbilityId = charaAbility.BattleAbilityMaster.id,
                            MCharaId = adviser.MCharaId,
                            CoolTime = 0,
                            UsableCount = (int)battleV2Ability.maxInvokeCount,
                            UsableCountMax = (int)battleV2Ability.maxInvokeCount,
                            AbilityLevel = (int)charaAbility.AbilityLevel,
                            RemainTurn = 0,
                        };
                        playerData.GuildBattleActivatedAbilityList.Add(abilityData);
                    }
                }
            }            
            
        }
        
        private void SetServerData(BattleV2ClientData battleData)
        {
            foreach (BattleV2Chara chara in battleData.charaList)
            {
                if( chara.roleNumber == (int)BattleConst.RoleNumberType.Adviser )
                {
                    // アドバイザーはパーティには所属しない
                    continue;
                }
                
                var partyIdentifier = (int)(chara.playerIndex * 1000 + chara.unitNumber);
                if (!AllPjfbBattleParties.ContainsKey(partyIdentifier))
                {
                    var playerData = BattlePlayerDataByIndex[chara.playerIndex];
                    var newParty = new GuildBattlePartyModel();
                    newParty.Identifier = partyIdentifier;
                    newParty.PlayerId = playerData.PlayerId;
                    newParty.PlayerIndex = (int)chara.playerIndex;
                    var deckName = string.Empty;
                    var tacticsId = (long)BattleConst.DefaultDeckStrategy;
                    var deckInfoList = battleData.playerList[chara.playerIndex - 1].deckInfoList;
                    var deckInfoIndex = chara.unitNumber - 1;
                    if (deckInfoList.Length  > deckInfoIndex)
                    {
                        tacticsId = deckInfoList[deckInfoIndex].optionValue;
                        deckName = deckInfoList[deckInfoIndex].name;
                    }
                    // failsafe
                    if (!Enum.IsDefined(typeof(BattleConst.DeckStrategy), (int)tacticsId))
                    {
                        tacticsId = (long)BattleConst.DefaultDeckStrategy;
                    }

                    newParty.TacticsId = tacticsId;
                    newParty.DeckName = deckName;
                    
                    AllPjfbBattleParties.Add(partyIdentifier, newParty);
                }

                AllPjfbBattleParties[partyIdentifier].UCharaIds.Add(chara.id);
            }

            foreach (var kvp in AllPjfbBattleParties)
            {
                var party = kvp.Value;
                if (!AllPjfbBattlePartyListByPlayerIndex.ContainsKey(party.PlayerIndex))
                {
                    AllPjfbBattlePartyListByPlayerIndex.Add(party.PlayerIndex, new List<GuildBattlePartyModel>());
                }

                AllPjfbBattlePartyListByPlayerIndex[party.PlayerIndex].Add(kvp.Value);

                party.RoleOperationIds = new long[party.UCharaIds.Count];
                for (var i = 0; i < party.UCharaIds.Count; i++)
                {
                    party.RoleOperationIds[i] = Instance.BattleCharaData[party.UCharaIds[i]].roleNumber;
                }
            }

            foreach (var kvp in AllPjfbBattlePartyListByPlayerIndex)
            {
                var playerPartyId = 1;
                foreach (var party in kvp.Value)
                {
                    party.PlayerPartyId = playerPartyId++;
                }
            }

            var invalidPartyIds = new List<int>();
            // 念の為パーティの有効チェック
            foreach (var kvp in AllPjfbBattleParties)
            {
                if (!kvp.Value.IsValid())
                {
                    invalidPartyIds.Add(kvp.Key);
                }
            }

            foreach (var partyId in invalidPartyIds)
            {
                AllPjfbBattleParties.Remove(partyId);
            }

            foreach (var kvp in AllPjfbBattleParties)
            {
                var playerIndex = kvp.Value.PlayerIndex;
                if (!OnMapPjfbBattlePartyListByPlayerIndex.ContainsKey(playerIndex))
                {
                    OnMapPjfbBattlePartyListByPlayerIndex.Add(playerIndex, new List<GuildBattlePartyModel>());
                }
                
                OnMapPjfbBattlePartyListByPlayerIndex[playerIndex].Add(kvp.Value);
            }
        }

        private void ApplyGuildBattleSetting(BattleV2ClientData originalData)
        {
            var setting = originalData.battleSetting;
            Instance.GuildBattleSetting.GuildBattleRequiredTurnToRecoveryMilitaryStrength = (int)setting.recoveryTurnCount;
            Instance.GuildBattleSetting.GuildBattleRecoveryMilitaryStrengthPerTurn = (int)setting.recoveryMilitaryStrengthPerTurn;
            Instance.GuildBattleSetting.GuildBattleMilitaryStrengthCaps[0] = (int)setting.maxMilitaryStrengthPerDeck;
            Instance.GuildBattleSetting.GuildBattleInitialMilitaryStrength = (int)setting.defaultMilitaryStrength;
            Instance.GuildBattleSetting.GuildBattleRecoveryValueOnUseItem = (int)setting.itemRecoveryValue;
            Instance.GuildBattleSetting.GuildBattleItemCoolDown = (int)setting.itemCoolTime;
            instance.GuildBattleSetting.GuildBattleRevivalTurn = (int)setting.revivalTurn;
            instance.GuildBattleSetting.GuildBattleRevivalTurnPenaltyPerBeaten = (int)setting.revivalTurnPenaltyPerBeaten;

            instance.GuildBattleSetting.GuildBattleAdditionalMilitaryStrengthPerSpotBroken = (int)originalData.battleConquestField.additionalMilitaryStrengthPerSpotBroken;
            if (originalData.battleConquestField.attackBonusRate != null)
            {
                foreach (var spotDamageBonusRate in originalData.battleConquestField.attackBonusRate)
                {
                    var spotCount = spotDamageBonusRate.l[0];
                    var coefficient = spotDamageBonusRate.l[1];

                    if (spotCount == 1)
                    {
                        instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientOneSpot = (int)(coefficient / 10000);
                    }
                
                    if (spotCount == 2)
                    {
                        instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientTwoSpot = (int)(coefficient / 10000);
                    }
                }
            }
            
            var baseSpotSetting = originalData.battleConquestField.battleConquestFieldSpotList.FirstOrDefault(spot => spot.isBase);
            var satelliteSpotSetting = originalData.battleConquestField.battleConquestFieldSpotList.FirstOrDefault(spot => !spot.isBase);
            if (baseSpotSetting != null)
            {
                instance.GuildBattleSetting.BaseSpotHP = (int)baseSpotSetting.hp;
            }

            if (satelliteSpotSetting != null)
            {
                instance.GuildBattleSetting.SpotHP = (int)satelliteSpotSetting.hp;
            }
        }
        #nullable enable
        public BattlePlayerModel? GetBattlePlayer(long userId)
        {
            PjfbBattlePlayers.TryGetValue(userId, out var ret);
            return ret;
        }

        public new GuildBattlePlayerData? GetBattlePlayerData(int playerIndex)
        {
            PjfbBattlePlayerData.TryGetValue(playerIndex, out var ret);
            return ret;
        }
        #nullable disable
        
        public int GetPlayerIndex(long userId)
        {
            return GetBattlePlayer(userId)?.Index ?? -1;
        }
        
        public override void AddBattleParty(GuildBattleCommonPartyModel party)
        {
            base.AddBattleParty(party);
            OnMapPjfbBattleParties.Add(party.PartyId, (GuildBattlePartyModel)party);
        }
        
        public override void RemoveBattleParty(GuildBattleCommonPartyModel party)
        {
            base.RemoveBattleParty(party);
            OnMapPjfbBattleParties.Remove(party.PartyId);
        }
        
        public void UpdateOnMapBattleParties(List<GuildBattlePartyModel> parties)
        {
            OnMapPjfbBattleParties.Clear();
            // 一旦クライアントで使っていないので.
            // OnMapPjfbBattlePartyListByPlayerIndex.Clear();
            BattleParties.Clear();
            foreach (var party in parties)
            {
                AddBattleParty(party);
            }
        }
        
        public void UpdatePlayerBattleParties(List<GuildBattlePartyModel> parties)
        {
            // ローカルで設定中のケースがあるため, 受け取ったデータに対して更新.
            foreach (var party in parties)
            {
                if (AllPjfbBattleParties.TryGetValue(party.Identifier, out var localParty))
                {
                    party.TacticsId = localParty.TacticsId;
                    for (var i = 0; i < party.RoleOperationIds.Length; i++)
                    {
                        party.RoleOperationIds[i] = localParty.RoleOperationIds[i];
                    }
                }
            }
            
            AllPjfbBattleParties.Clear();
            AllPjfbBattlePartyListByPlayerIndex.Clear();
            foreach (var party in parties)
            {
                AllPjfbBattleParties.Add(party.Identifier, party);
                if (!AllPjfbBattlePartyListByPlayerIndex.ContainsKey(party.PlayerIndex))
                {
                    AllPjfbBattlePartyListByPlayerIndex.Add(party.PlayerIndex, new List<GuildBattlePartyModel>(parties.Count));
                }
                AllPjfbBattlePartyListByPlayerIndex[party.PlayerIndex].Add(party);
            }
        }

        public void UpdateGuildBattlePlayerData(GuildBattlePlayerData playerData)
        {
            if (PjfbBattlePlayerData.ContainsKey(playerData.PlayerIndex))
            {
                PjfbBattlePlayerData[playerData.PlayerIndex] = playerData;
            }
        }

        public void UpdateLocalData(GuildBattleFieldSituationModel latestSituation)
        {
            LatestSituation = latestSituation;
            BattleField.UpdateSpotData(latestSituation.SpotOccupationInfo);
            UpdateOnMapBattleParties(latestSituation.PjfbParties);
            UpdateWinningPoints(latestSituation.WinningPoints);
        }
        
        public BattleV2Chara? GetBattleCharacterModel(int playerIndex, long mCharaId)
        {
            return PjfbGuildBattleDataMediator.Instance.BattleCharaDataList[playerIndex]
                .FirstOrDefault(chara => chara.mCharaId == mCharaId);
        }
        
        public List<GuildBattlePartyModel> GetBattlePartiesFromPlayerIndex(int playerIndex)
        {
            if (AllPjfbBattlePartyListByPlayerIndex.TryGetValue(playerIndex, out List<GuildBattlePartyModel> parties))
            {
                return parties;
            }
            return new List<GuildBattlePartyModel>();
        }
        
        public BattleCharacterModel? GetAdviserCharacterModel( int playerIndex, long mCharaId )
        {
            if (Advisers.TryGetValue(playerIndex, out List<BattleCharacterModel> advisers))
            {
                return advisers.FirstOrDefault(adviser => adviser.MCharaId == mCharaId);
            }
            return null;
        }
        
        public int GetAdviserCount(int playerIndex)
        {
            if (Advisers.TryGetValue(playerIndex, out List<BattleCharacterModel> advisers))
            {
                return advisers.Count;
            }
            return 0;
        }

        /// <summary>
        /// ターンごとのクラブ・ロワイヤル専用技能の効果を適用するところ
        /// </summary>
        public void GuildBattleAbilityEffectTurn()
        {
            // バトルプレイヤーデータのクールタイムを減少させる
            foreach (KeyValuePair<long, GuildBattlePlayerData> battlePlayerData in PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData)
            {
                var player = battlePlayerData.Value;
                GuildBattleAbilityLogic.GuildBattleAbilityEffects(player);
            }
        }
        
        public void DecrementGuildBattleAbilityEffectTurn()
        {
            // バトルプレイヤーデータのクールタイムを減少させる
            foreach (KeyValuePair<long, GuildBattlePlayerData> battlePlayerData in PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData)
            {
                var player = battlePlayerData.Value;
                foreach (GuildBattleAbilityData activatedAbilityData in player.GuildBattleActivatedAbilityList)
                {
                    // RemainTurnとCoolTimeは同時に設定される。RemainTurnが0になったらCoolTimeを減らす
                    if (activatedAbilityData.RemainTurn > 0)
                    {
                        activatedAbilityData.RemainTurn -= 1;
                    }

                    if (activatedAbilityData.CoolTime > 0)
                    {
                        activatedAbilityData.CoolTime -= 1;
                    }
                }
            }
        }

        /// <summary>
        /// 自動発動のアドバイザースキル(サポートスキル)の起動確認を行うところ
        /// </summary>
        public void ActivateAdviserSupportAbility(BattleConst.AbilityEvaluateTimingType timingType)
        {
            // プレイヤー単位で行う
            foreach (KeyValuePair<long, GuildBattlePlayerData> kvp in PjfbBattlePlayerData)
            {
                GuildBattlePlayerData playerData = kvp.Value;
                if (playerData.GuildBattleActivatedAbilityList == null || playerData.GuildBattleActivatedAbilityList.Count == 0)
                {
                    continue;
                }

                ActivateAdviserSupportAbility(playerData, timingType);
            }
        }

        public void ActivateAdviserSupportAbility(GuildBattlePlayerData playerData, BattleConst.AbilityEvaluateTimingType timingType)
        {
            foreach (GuildBattleAbilityData ability in playerData.GuildBattleActivatedAbilityList.Where(ability => ability.UsableCount > 0 && ability.CoolTime == 0))
            {
                // 一致するIDでかつ自動発動スキル(サポートスキル)
                BattleV2Ability v2Ability = Instance.OriginalBattleData.abilityList.FirstOrDefault(a => a.id == ability.AbilityId && a.abilityType == (long)BattleConst.AbilityType.GuildBattleAuto);

                // スキルが存在しない場合はスキルを発動しない
                if (v2Ability == null)
                {
                    continue;
                }
                
                // タイミングが一致しない場合はスキルを発動しない
                if( v2Ability.timing != (long)timingType )
                {
                    continue;
                }

                // invokeConditionなどを作るのに必要
                BattleAbilityModel battleAbilityModel = BattleAbilityModel.Build(Instance.OriginalBattleData.abilityList, ability.AbilityId, ability.AbilityLevel);

                // スキルが発動するかどうか
                if (GuildBattleAbilityLogic.IsActivateAdviserAbility(playerData, battleAbilityModel))
                {
#if MAGIC_ONION_SERVER_DEBUG
                    Console.WriteLine($"Activate Adviser Support Ability: {ability.AbilityId} timingType: {timingType} for Player Index: {playerData.PlayerIndex}, MCharaId: {ability.MCharaId}");
#endif
                    ability.UsableCount--;
                    ability.CoolTime = (int)battleAbilityModel.BattleAbilityMaster.coolDownTurnCount;
                    ability.RemainTurn = battleAbilityModel.GetRemainTurn();
                }
            }
        }
    }
}