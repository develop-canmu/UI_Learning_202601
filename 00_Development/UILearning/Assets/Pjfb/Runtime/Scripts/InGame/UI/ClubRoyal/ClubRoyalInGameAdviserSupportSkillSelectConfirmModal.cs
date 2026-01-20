using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Character;
using Unity.VisualScripting;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameAdviserSupportSkillSelectConfirmModal : ModalWindow
    {
        [SerializeField] private ScrollGrid scrollGrid = null;
        [SerializeField] private GameObject notActiveAbilityLabel = null;
        private ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData[] scrollDataArray;
        private GuildBattlePlayerData guildBattlePlayerData;
        private List<GuildBattleAbilityData> cachedSupportAbilityDataList;
        
        // ScrollRectは上端が1なので初期位置に1を設定する
        private const float ScrollInitValue = 1.0f;
        
        private bool isContentOverViewSize = false;

        public class Arguments
        {
            public GuildBattlePlayerData playerData;
            public Arguments(GuildBattlePlayerData guildBattlePlayerData )
            {
                playerData = guildBattlePlayerData;
            }
        }
        private Arguments arguments;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;
            guildBattlePlayerData = arguments.playerData;
            UpdateScrollItems(guildBattlePlayerData.GuildBattleActivatedAbilityList);

            // 初回更新後、イベントを登録
            ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.AddListener(OnPlayerDataUpdated);
            
            return base.OnPreOpen(args, token);
        }
        
        private void OnPlayerDataUpdated(GuildBattlePlayerData arg0)
        {
            UpdateScrollItems(arg0.GuildBattleActivatedAbilityList);
        }

        private void UpdateScrollItems(List<GuildBattleAbilityData> arg0GuildBattleActivatedAbilityList)
        {
            List<GuildBattleAbilityData> activeSupportAbilityList = GuildBattleAbilityLogic.GetActiveSupportAbilityList(arg0GuildBattleActivatedAbilityList);

            // 更新前とデータが変わっていない場合は更新を行わない
            if (!GuildBattleAbilityLogic.IsDirtyGuildBattleAbilityDataLists(cachedSupportAbilityDataList, activeSupportAbilityList))
            {
                return;
            }

            cachedSupportAbilityDataList = new List<GuildBattleAbilityData>();
            cachedSupportAbilityDataList.AddRange(activeSupportAbilityList);
            notActiveAbilityLabel.SetActive(activeSupportAbilityList.Count == 0);

            scrollDataArray = new ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData[activeSupportAbilityList.Count];
            for( int i = 0 ; i < scrollDataArray.Length ; i++ )
            {
                scrollDataArray[i] = new ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData(activeSupportAbilityList[i]);
            }
            
            // 現在のスクロール量を保持
            float scrollValue = isContentOverViewSize ? scrollGrid.GetScrollValueNormalized() : ScrollInitValue;
            scrollGrid.SetItems(scrollDataArray.ToList());
            
            if(scrollGrid.viewport.rect.height < scrollGrid.content.rect.height)
            {
                // スクロール量を復元
                scrollGrid.SetScrollValueNormalized(scrollValue);
                // アイテムの位置等の情報を更新する
                scrollGrid.Refresh(false);
                isContentOverViewSize = true;
            }
            else
            {
                isContentOverViewSize = false;
            }
        }

    }
}