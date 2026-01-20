using UnityEngine;
using CruFramework.UI;
using Pjfb.UI;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.LeagueMatch;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;

namespace Pjfb
{
    public class LeagueDeckPanelScrollGridItem : ScrollGridItem
    {   
        #region SerializeFields
        [SerializeField] private LeagueDeckPanelView deckPanelView;
        #endregion

        #region PrivateFields
        private DeckScrollData parameters;
        #endregion

        #region OverrideMethods
        protected override void OnSetView(object value)
        {
            parameters = (DeckScrollData) value;
            var iconParams = new List<DeckPanelCharaIconView.ViewParams>();
            var memberIdList = parameters.DeckData.Deck.memberIdList;
            for (var i = 0; i < memberIdList.Length; i++)
            {
                var member = memberIdList[i];
                iconParams.Add(new DeckPanelCharaIconView.ViewParams
                {
                    type = member.l[0],
                    nullableCharacterData = member.l[1] > 0 ? new CharacterVariableDetailData(UserData.UserDataManager.Instance.charaVariable.Find(member.l[1])) : null,
                    position = (RoleNumber) member.l[2]
                });
            }
            
            deckPanelView.Init();
            deckPanelView.SetDisplay(new LeagueDeckPanelView.ViewParams (
                deckId: parameters.DeckData.PartyNumber,
                isPlayerDeck: true,
                deckName: parameters.DeckData.Deck.name,
                strategy: (BattleConst.DeckStrategy)parameters.DeckData.Deck.optionValue,
                rankBgColor: ColorValueAssetLoader.Instance["league.match.player.color"],
                iconParams: iconParams,
                onClickDeckEditButton: OnClickDeckEditButton,
                onStrategyChanged: TrySaveDeck,
                penaltyValue: 0
            ));
        }
        #endregion
        
        private void OnClickDeckEditButton(long partyNumber)
        {
            // TODO:データ引き継ぎ
            var param = new DeckPageParameters{InitialPartyNumber = partyNumber, OpenFrom = PageType.LeagueMatchDeckEntry};
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.LeagueMatchDeck, true, param);
        }

        private async void TrySaveDeck(BattleConst.DeckStrategy strategy)
        {
            var deckListData = await DeckUtility.GetDeckList(DeckType.LeagueMatch);
            var selectedPartyNumber = parameters.DeckData.PartyNumber;
            foreach(DeckData deck in deckListData.DeckDataList)
            {
                if (deck.PartyNumber != selectedPartyNumber) continue;
                deck.Deck.optionValue = (int)strategy;
            }
            deckListData.SaveAsync(selectedPartyNumber).Forget();
        }
        
    }
}