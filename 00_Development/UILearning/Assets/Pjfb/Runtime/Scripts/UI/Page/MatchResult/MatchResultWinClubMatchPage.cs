using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using Pjfb.UserData;
using Image = UnityEngine.UI.Image;
using Pjfb.Extensions;

namespace Pjfb.MatchResult
{
    public class MatchResultWinClubMatchPage : Page
    {
        #region Params
        public class Data
        {
            public string playerName;
            public ColosseumAttackAPIResponse colosseumAttackAPIResponse;
            public long useDeckId;

            public Data(ColosseumAttackAPIResponse _colosseumAttackAPIResponse,string _playerName, long _useDeckId)
            {
                playerName = _playerName;
                colosseumAttackAPIResponse = _colosseumAttackAPIResponse;
                useDeckId = _useDeckId;
            }
        }
        #endregion

        #region SerializeFields

        [SerializeField] private TMP_Text scoreValueText;
        [SerializeField] private Animator animator;
        [SerializeField] private MatchResultWinClubMatchCell playerCell;
        [SerializeField] private ConditionView conditionView;
        #endregion
        
        protected Data data;

        #region OverrideMethods

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data)args;
            SetView();
            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            base.OnEnablePage(args);
        }
        
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) 
            {
                switch(type) 
                {
                    case PageManager.MessageType.EndFade:
                        animator.SetTrigger("Open");
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        #endregion
        
        #region PrivateMethods

        private async void SetView()
        {
            var playerResult = data.colosseumAttackAPIResponse.battleChangeResult.player;
            playerCell.SetView(data.playerName, playerResult);
            var getScore = playerResult.after.score - playerResult.before.score;
            scoreValueText.text = getScore.GetStringNumberWithComma();

            var clubBattleDeck = await DeckUtility.GetClubBattleDeck();
            var useDeck = clubBattleDeck.DeckDataList.FirstOrDefault(deck => deck.PartyNumber == data.useDeckId);
            useDeck?.UpdateFatigueValue();
            conditionView.SetCondition(useDeck?.FixedClubConditionData ?? ClubMatchUtility.GetConditionData(ClubDeckCondition.Best));
        }
        
        #endregion
        
        #region EventListeners
        public void OnClickClose()
        {
            var sColosseumEventId = data.colosseumAttackAPIResponse.userSeasonStatus.sColosseumEventId;
            var param = new ClubMatchTopPage.Data(UserDataManager.Instance.GetColosseumSeasonData(sColosseumEventId));
            if (!param.SeasonData.IsOnSeason)
            {
                ClubMatchUtility.OpenSeasonChangeModal(ClubMatchSeasonChangeModalType.End);
            }
            else
            {
                ClubMatchPage.OpenPage(true,param);
            }
        }
        #endregion
    }
}
