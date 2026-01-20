using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using Logger = CruFramework.Logger;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameAdviserYellSkillSelectConfirmModal : ModalWindow
    {
        [SerializeField] private AdviserSkillView adviserSkillView;
        [SerializeField] private TMP_Text abilityNameText;
        [SerializeField] private TMP_Text reactivateTurnText;
        [SerializeField] private TMP_Text abilityDescriptionText;
        [SerializeField] private UIButton cancelButton;
        [SerializeField] private UIButton activateButton;
        [SerializeField] private TMP_Text usableMessageText;

        [SerializeField] private ScrollGrid scrollGrid = null;
        private List<GuildBattleAbilityData> cachedYellAbilityDataList = new List<GuildBattleAbilityData>();
        private AdviserYellSkillSelectScrollItemScrollData[] scrollDataArray;
        private GuildBattleAbilityData selectedAbilityData;
        private GuildBattlePlayerData guildBattlePlayerData;
        private bool isSuccessActivate = false;
        private bool isInFight = false;
        
        // スクロール位置の更新用変数
        private bool isContentOverViewSize = false;
        private const float ScrollInitValue = 1.0f;
        
        public class Arguments
        {
            public GuildBattlePlayerData guildBattlePlayerData;
            public Action<long, long, string> OnActivateAbility = null;

            public Arguments(GuildBattlePlayerData playerData, Action<long, long, string> onActivateAbility)
            {
                guildBattlePlayerData = playerData;
                OnActivateAbility = onActivateAbility;
            }
        }
        private Arguments arguments;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;
            guildBattlePlayerData = arguments.guildBattlePlayerData;
            
            isInFight = PjfbGuildBattleDataMediator.Instance.IsInFight();
            UpdateScrollItems(arguments.guildBattlePlayerData.GuildBattleActivatedAbilityList, true);
            
            scrollGrid.OnSelectedItemEvent -= OnSelectItem;
            scrollGrid.OnSelectedItemEvent += OnSelectItem;

            if (cachedYellAbilityDataList.Count > 0)
            {
                scrollGrid.SelectItem(0);
            }

            // 初回更新後、イベントを登録
            ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.AddListener(OnPlayerDataUpdated);
            
            return base.OnPreOpen(args, token);
        }

        public void OnCloseModal()
        {
            // ボタン経由で閉じる場合はスキルを発動しないようにクリア処理を行う
            selectedAbilityData = null;
            Close();
        }


        protected override UniTask OnPreClose(CancellationToken token)
        {
            // 閉じるタイミングでイベントを解除
            ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.RemoveListener(OnPlayerDataUpdated);
            return base.OnPreClose(token);
        }

        protected override void OnClosed()
        {
            Logger.Log($"OnClosed called with selectedAbilityData: {selectedAbilityData}");
            if (arguments.OnActivateAbility != null && selectedAbilityData != null && isSuccessActivate)
            {
                AbilityMasterObject abilityMasterObject = MasterManager.Instance.abilityMaster.FindData(selectedAbilityData.AbilityId);
                arguments.OnActivateAbility.Invoke(
                    selectedAbilityData.MCharaId,
                    selectedAbilityData.AbilityId,
                    abilityMasterObject.useMessage);
            }
            else
            {
                Logger.Log("OnActivateAbility is null. Unable to invoke the ability.");
            }
            base.OnClosed();
        }

        private void OnSelectItem(ScrollGridItem gridItem)
        {
            AdviserYellSkillSelectScrollItem item = (AdviserYellSkillSelectScrollItem)gridItem;

            if (item == null)
            {
                Logger.LogError("Selected item or scroll data is null.");
                return;
            }
            
            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == item.ScrollData.guildBattleAbilityData.AbilityId);

            if (battleV2Ability == null)
            {
                Logger.LogError($"BattleV2Ability not found for AbilityId: {item.ScrollData.guildBattleAbilityData.AbilityId}");
                return;
            }
            
            // アドバイザーのキャラデータを取得
            UserDataChara targetAdviser = UserDataManager.Instance.userAdviserList.FirstOrDefault(a => a.charaId == item.ScrollData.guildBattleAbilityData.MCharaId);
            if (targetAdviser == null)
            {
                Logger.LogError($"Adviser character data not found for PlayerIndex: {guildBattlePlayerData.PlayerIndex}, MCharaId: {item.ScrollData.guildBattleAbilityData.MCharaId}");
                return;
            }

            if (!CharaAbilityInfo.Builder(out CharaAbilityInfo abilityInfo, item.ScrollData.guildBattleAbilityData, (BattleConst.AbilityType)battleV2Ability.abilityType))
            {
                Logger.LogError($"Failed to build CharaAbilityInfo. Please check the ability data. MCharaId: {item.ScrollData.guildBattleAbilityData.MCharaId}, AbilityId: {item.ScrollData.guildBattleAbilityData.AbilityId}, AbilityLevel: {item.ScrollData.guildBattleAbilityData.AbilityLevel}");
                return;
            }

            adviserSkillView.SetSkillView(abilityInfo,
                targetAdviser.level,
                item.ScrollData.guildBattleAbilityData.AbilityLevel,
                false);
            
            activateButton.interactable = IsYellAbilityUsable(guildBattlePlayerData, item.ScrollData.guildBattleAbilityData, out string message);
            usableMessageText.text = message;
            
            AbilityMasterObject abilityMasterObject = MasterManager.Instance.abilityMaster.FindData(battleV2Ability.id);
            if (abilityMasterObject != null)
            {
                abilityDescriptionText.text = abilityMasterObject.description;
                reactivateTurnText.text = abilityMasterObject.coolDownTurnCount.ToString();
            }
            selectedAbilityData = item.ScrollData.guildBattleAbilityData;
        }

        private void OnPlayerDataUpdated(GuildBattlePlayerData arg0)
        {
            UpdateScrollItems(arg0.GuildBattleActivatedAbilityList , isInFight != PjfbGuildBattleDataMediator.Instance.IsInFight());
        }

        private void UpdateScrollItems(List<GuildBattleAbilityData> arg0GuildBattleActivatedAbilityList , bool isForceUpdate)
        {
            List<GuildBattleAbilityData> yellAbilityList = arg0GuildBattleActivatedAbilityList.Where(v => GuildBattleAbilityLogic.IsYellAbility(v.AbilityId)).ToList();
            
            // 更新前とデータが変わっていない場合は更新を行わない
            if (GuildBattleAbilityLogic.IsDirtyGuildBattleAbilityDataLists(cachedYellAbilityDataList, yellAbilityList) || isForceUpdate)
            {
                Logger.Log($"Updating scroll items with {yellAbilityList.Count} yell abilities.");
            }
            else
            {
                return;
            }

            isInFight = PjfbGuildBattleDataMediator.Instance.IsInFight();
            cachedYellAbilityDataList = new List<GuildBattleAbilityData>();
            cachedYellAbilityDataList.AddRange(yellAbilityList);
            
            scrollDataArray = new AdviserYellSkillSelectScrollItemScrollData[yellAbilityList.Count];
            for( int i = 0 ; i < scrollDataArray.Length ; i++ )
            {
                scrollDataArray[i] = new AdviserYellSkillSelectScrollItemScrollData()
                {
                    guildBattleAbilityData = yellAbilityList[i],
                };
            }
            
            // アイテムのデータを更新
            float scrollValue = isContentOverViewSize == true ? scrollGrid.GetScrollValueNormalized() : ScrollInitValue;
            scrollGrid.SetItems(scrollDataArray.ToList());
            if(scrollGrid.content.rect.height > scrollGrid.viewport.rect.height)
            {
                // スクロールが必要な場合は更新前のスクロール位置を設定
                scrollGrid.SetScrollValueNormalized(scrollValue);
                // 表示対象アイテムを更新
                scrollGrid.Refresh(false);
                isContentOverViewSize = true;
            }
            else
            {
                isContentOverViewSize = false;
            }
            
        }

        public void OnRequestSelectedYellSkill()
        {
            if (selectedAbilityData == null)
            {
                Logger.LogError("Selected ability data is null. Cannot activate ability.");
                return;
            }

            if (!selectedAbilityData.CanUse())
            {
                Logger.LogError("Selected ability cannot be used. Please check the ability's state.");
                return;
            }
            
            RequestAdviserYellSkillActivate(selectedAbilityData).Forget();
        }
        
        private async UniTask RequestAdviserYellSkillActivate(GuildBattleAbilityData abilityData)
        {
            Logger.Log($"Requesting adviser yell skill activation for {abilityData}");

            var playerData = await PjfbGameHubClient.Instance.RequestUseYellAbility(abilityData.MCharaId, abilityData.AbilityId);

            if( playerData == null )
            {
                selectedAbilityData = null;
                Logger.LogError("Failed to activate adviser yell skill. Player data is null.");
                ConfirmModalData data = new ConfirmModalData(
                    StringValueAssetLoader.Instance["clubroyalingame.yellskill.gameover.title"],
                    StringValueAssetLoader.Instance["clubroyalingame.yellskill.gameover.message"],
                    null,
                    new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.close"], (window) => { window.Close(); } )
                );
                CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
                await modal.WaitCloseAsync();

            }
            else
            {
                isSuccessActivate = true;
            }

            Close();
        }

        private bool IsYellAbilityUsable(GuildBattlePlayerData playerData, GuildBattleAbilityData guildBattleAbilityData, out string message)
        {
            message = StringValueAssetLoader.Instance["clubroyalingame.yell_select.usable_message"];

            if (!PjfbGuildBattleDataMediator.Instance.IsInFight())
            {
                return false;
            }

            if (!guildBattleAbilityData.CanUse())
            {
                return false;
            }

            // ボールが最大値以上の場合は回復スキルで一部利用できない
            if (playerData.MaxMilitaryStrength <= playerData.AvailableMilitaryStrength)
            {
                BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == guildBattleAbilityData.AbilityId);
                if (battleV2Ability == null)
                {
                    Logger.LogError($"BattleV2Ability not found for AbilityId: {guildBattleAbilityData.AbilityId}");
                    return false;
                }

                BattleAbilityModel battleAbilityModel = new BattleAbilityModel();
                battleAbilityModel.SetData(
                    battleV2Ability,
                    guildBattleAbilityData.AbilityLevel);

                if (GuildBattleAbilityLogic.IsGuildBattleAbilityBallRecoveryEffectUntilMaxHeal(battleAbilityModel))
                {
                    message = StringValueAssetLoader.Instance["clubroyalingame.yell_select.unusable_message_overheal"];
                    return false;
                }
            }

            return true;
        }
    }
}