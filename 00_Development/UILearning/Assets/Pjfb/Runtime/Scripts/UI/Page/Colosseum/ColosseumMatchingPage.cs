using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Colosseum
{
    public class ColosseumMatchingPage : Page
    {
        
        public class Data
        {
            public ColosseumSeasonData colosseumSeasonData;

            public Data(ColosseumSeasonData _colosseumSeasonData)
            {
                colosseumSeasonData = _colosseumSeasonData;
            }
        }
        
        #region SerializeFields
        [SerializeField] private PoolListContainer rankingContainer;
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text totalPowerText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private DeckRankImage deckRankImage;
        [SerializeField] private UIButton updateButton;
        [SerializeField] private StaminaView staminaView;
        #endregion

        private ColosseumRankingUser[] rankingUserList;
        private ColosseumSeasonData currentColosseumSeasonData;

        #region OverrideMethods

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            var user = UserDataManager.Instance.user;
            nameText.text = user.name;

            totalPowerText.text = user.maxCombatPower.Value.ToDisplayString(omissionTextSetter.GetOmissionData());
            deckRankImage.SetTexture(user.maxDeckRank.Value);
            
            var pageParam = (Data)args;
            currentColosseumSeasonData = pageParam.colosseumSeasonData;
            
            updateButton.SetClickIntervalTimer(updateButton.ClickTriggerInterval);
            UpdateRankingView().Forget();
            await staminaView.UpdateAsync(StaminaUtility.StaminaType.Colosseum,currentColosseumSeasonData.MColosseumEvent.mStaminaId);
            await base.OnPreOpen(args, token);
        }

        #endregion

        #region PrivateMethods

        private async UniTask UpdateRankingView()
        {
            var season = currentColosseumSeasonData.UserSeasonStatus;
            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],season.ranking);

            if (season != null)
            {
                rankingUserList = await ColosseumManager.RequestTargetDataAsync(season.sColosseumEventId);
            }

            UpdateUserList();
        }
        
        private void UpdateUserList()
        {
            var data = new List<ColosseumRankingItem.Data>();
            if (rankingUserList == null || rankingUserList.Length == 0)
            {
                return;
            }
            
            foreach (var user in rankingUserList)
            {
                data.Add(new ColosseumRankingItem.Data
                {
                    userData = user,colosseumSeasonData = currentColosseumSeasonData,
                    OnSizeChanged = rankingContainer.RefreshView
                });
            }
            
            rankingContainer.SetDataList(data,slideIn:false).Forget();
        }
        #endregion

        #region EventListeners

        public void OnClickBack()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Colosseum, false,null);
        }

        public void OnClickUpdateUserList()
        {
            UpdateRankingView().Forget();
        }

        #endregion
    }
}