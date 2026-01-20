using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.InGame;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine.Serialization;
using Logger = CruFramework.Logger;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameAdviserUI : MonoBehaviour
    {
        private const int MAX_COOLTIME_ICONS = 5;
        
        [SerializeField] private Transform coolTimeIconParent = null;
        [SerializeField] private ClubRoyalInGameAdviserCoolTimeGroupUI coolTimeIconPrefab = null;
        [SerializeField] private ClubRoyalInGameAdviserSkillActivationEffect skillActivationEffect = null;
        [SerializeField] private GameObject ballRecoveryUpPerTurnIconGameObject = null;
        [SerializeField] private ClubRoyalInGameActivatingSkillCounterBalloonUI activatingSupportSkillCounterBalloonUI = null;
        
        private List<ActivateAbilityCache> activatedSupportAbilityList = new List<ActivateAbilityCache>();
        private GuildBattlePlayerData lastPlayerData = null;
        private List<ClubRoyalInGameAdviserCoolTimeGroupUI> cachedCoolTimeIcons = new List<ClubRoyalInGameAdviserCoolTimeGroupUI>();
        private bool isInitialized = false;

        private class ActivateAbilityCache
        {
            private long abilityId;
            private long mCharaId;
            private int useableCount;
            
            public long AbilityId => abilityId;
            public long MCharaId => mCharaId;
            public int UseableCount => useableCount;
            public ActivateAbilityCache(GuildBattleAbilityData abilityData)
            {
                abilityId = abilityData.AbilityId;
                mCharaId = abilityData.MCharaId;
                useableCount = abilityData.UsableCount;
            }
        }

        public void Display(bool isShow)
        {
            coolTimeIconPrefab.gameObject.SetActive(false);
            gameObject.SetActive(isShow);
        }
        
        public void DisplayUpdate(GuildBattlePlayerData playerData)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                activatedSupportAbilityList.Clear();
                playerData.GuildBattleActivatedAbilityList.ForEach(b => activatedSupportAbilityList.Add(new ActivateAbilityCache(b)));
            }
            
            // 最後のプレイヤーデータを取得
            lastPlayerData = playerData;
            
            bool isShow = playerData != null && PjfbGuildBattleDataMediator.Instance.GetAdviserCount(playerData.PlayerIndex) > 0;
            Display(isShow);
            
            // アドバイザーがいない場合は表示を行わない
            if (!isShow)
            {
                return;
            }
            
            #region ターンあたりのボール回復量アップ状態の場合はアイコンを表示する

            ballRecoveryUpPerTurnIconGameObject.SetActive(GuildBattleAbilityLogic.IsActivatingBallRecoveryUpPerTurn(playerData));
            #endregion

            #region 新規に発動したサポートスキルのログ生成 
            List<GuildBattleAbilityData> newActivateAbilityList = CollectNewActivateAbilityList(
                activatedSupportAbilityList,
                playerData.GuildBattleActivatedAbilityList.ToList());

            // 必要に応じて新しいスキルに対する処理を追加
            foreach (GuildBattleAbilityData ability in newActivateAbilityList)
            {
                // 新規に発動したサポートスキルのログを表示する部分
                BattleCharacterModel battleCharacterModel = PjfbGuildBattleDataMediator.Instance.GetAdviserCharacterModel(playerData.PlayerIndex, (int)ability.MCharaId);
                if (battleCharacterModel == null)
                {
                    Logger.LogError($"Adviser character model not found for PlayerIndex: {playerData.PlayerIndex}, MCharaId: {ability.MCharaId}");
                    continue;
                }
                
                BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList
                    .FirstOrDefault(a => a.id == ability.AbilityId);
                BattleAbilityModel battleAbilityModel = BattleAbilityModel.Build(battleV2Ability, ability.AbilityLevel);
                
                List<Tuple<BattleCharacterModel, BattleAbilityModel>> abilityLogList = new List<Tuple<BattleCharacterModel, BattleAbilityModel>> { Tuple.Create(battleCharacterModel, battleAbilityModel) };
                BattleLogMediator.Instance.AddAbilityLogGuildBattle(abilityLogList);
            }

            activatedSupportAbilityList.Clear();
            playerData.GuildBattleActivatedAbilityList.ForEach(b => activatedSupportAbilityList.Add(new ActivateAbilityCache(b)));
            
            #endregion

            #region 起動中のサポートスキルのカウンター表示

            activatingSupportSkillCounterBalloonUI.Display(GuildBattleAbilityLogic.GetActiveSupportAbilityList(playerData.GuildBattleActivatedAbilityList).Count);

            #endregion
            

            #region クールタイム中のアイコン表示
            List<GuildBattleAbilityData> coolTimeAbilityList = playerData.GuildBattleActivatedAbilityList
                    .Where(ability => ability.CoolTime > 0)
                    .OrderBy(ability => ability.CoolTime)
                    .Take(MAX_COOLTIME_ICONS)
                    .ToList();
            DisplayCoolTimeIcons(coolTimeAbilityList);
            #endregion

        }
        
        private List<GuildBattleAbilityData> CollectNewActivateAbilityList(List<ActivateAbilityCache> oldCacheList, List<GuildBattleAbilityData> currentList)
        {
            List<GuildBattleAbilityData> newActivateList = new List<GuildBattleAbilityData>();
            foreach (GuildBattleAbilityData currentAbility in currentList)
            {
                ActivateAbilityCache sameAbility = oldCacheList.FirstOrDefault(ability =>
                    ability.AbilityId == currentAbility.AbilityId &&
                    ability.MCharaId == currentAbility.MCharaId);
                
                // 同じアビリティがない場合は新規に追加
                if (sameAbility == null)
                {
                    newActivateList.Add(currentAbility);
                    continue;
                }
                // 利用可能回数が変化した場合はスキル利用した判定
                else if( currentAbility.UsableCount != sameAbility.UseableCount)
                {
                    newActivateList.Add(currentAbility);
                }
            }
            return newActivateList;
        }
        
        private void DisplayCoolTimeIcons(List<GuildBattleAbilityData> coolTimeAbilityList)
        {
            if (coolTimeIconParent == null || coolTimeIconPrefab == null)
            {
                return;
            }
            
            // Clear existing cached icons
            foreach (ClubRoyalInGameAdviserCoolTimeGroupUI coolTimeGroupUI in cachedCoolTimeIcons)
            {
                if (coolTimeGroupUI != null)
                {
                    Destroy(coolTimeGroupUI.gameObject);
                }
            }
            cachedCoolTimeIcons.Clear();

            // Instantiate cool time icons for each ability
            foreach (var abilityData in coolTimeAbilityList)
            {
                ClubRoyalInGameAdviserCoolTimeGroupUI coolTimeIcon = Instantiate(coolTimeIconPrefab, coolTimeIconParent);
                coolTimeIcon.Initialize(abilityData);
                coolTimeIcon.gameObject.SetActive(true);
                cachedCoolTimeIcons.Add(coolTimeIcon);
            }
        }

        public void OnOpenYellAbilityModal()
        {
            ClubRoyalInGameAdviserYellSkillSelectConfirmModal.Arguments data = new ClubRoyalInGameAdviserYellSkillSelectConfirmModal.Arguments(
                lastPlayerData,
                OnActivateYellSkillEffect);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameAdviserYellSkillSelectConfirm, data);
        }

        public void OnOpenCheckSupportAbilityModal()
        {
            ClubRoyalInGameAdviserSupportSkillSelectConfirmModal.Arguments data = new ClubRoyalInGameAdviserSupportSkillSelectConfirmModal.Arguments(lastPlayerData);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameAdviserSupportSkillSelectConfirm,data);
        }

        private void OnActivateYellSkillEffect(long mCharaId ,long abilityId , string useMessage)
        {
            if (abilityId == 0)
            {
                // スキルが選択されなかった場合は何もしない
                Logger.LogWarning("No ability selected for activation.");
                return;
            }
            
            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == abilityId);
            if (battleV2Ability == null || battleV2Ability.cutInType == 0)
            {
                // スキルが存在しない、またはカットインタイプが0の場合は何もしない
                Logger.LogWarning($"Ability with ID {abilityId} not found or has cutInType 0.");
                return;
            }

            // 選択されたスキルの発動エフェクトを再生
            if (skillActivationEffect != null)
            {
                skillActivationEffect.PlayAnimation(mCharaId, abilityId, useMessage).Forget();
            }
        }
    }
}