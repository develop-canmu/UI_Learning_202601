using System.Collections.Generic;
using System.Linq;
using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSpotModalSheetMyTeam : Sheet
    {
        [SerializeField] 
        private ScrollGrid scrollGrid;
        [SerializeField]
        private GameObject noPartyMessage = null;

        // ScrollRectは上端が1なので初期位置に1を設定する
        private const float ScrollInitValue = 1.0f;
        private bool isContentOverViewSize = false;
        
        public void SetData(List<GuildBattlePartyModel> parties)
        {
            if (parties.Count == 0)
            {
                noPartyMessage.SetActive(true);
                return;
            }

            var data = parties.Select(party => new ClubRoyalInGameSelectPartyScrollItem.Arguments(party, false)).ToList();
            noPartyMessage.SetActive(false);
            
            // 現在のスクロール量を保持
            float scrollValue = isContentOverViewSize ? scrollGrid.GetScrollValueNormalized() : ScrollInitValue;
            scrollGrid.SetItems(data);
            
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