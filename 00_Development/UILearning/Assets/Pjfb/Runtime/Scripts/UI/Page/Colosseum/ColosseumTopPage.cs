using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Pjfb.Master;
using Pjfb.Shop;
using Pjfb.UserData;
using Pjfb.Extensions;
using TMPro;
using UnityEngine;

namespace Pjfb.Colosseum
{

    public class ColosseumTopPage : Page
    {
        public class Data
        {
            public ColosseumSeasonData colosseumSeasonData;
            public ColosseumEventMasterObject colosseumEventMaster;
            public ColosseumState colosseumState;

            public Data(ColosseumSeasonData _colosseumSeasonData, ColosseumEventMasterObject _colosseumEventMaster, ColosseumState _colosseumState)
            {
                colosseumSeasonData = _colosseumSeasonData;
                colosseumEventMaster = _colosseumEventMaster;
                colosseumState = _colosseumState;
            }
        }

        #region SerializeFields

        [SerializeField] private GameObject exitSeasonRoot;
        [SerializeField] private ColosseumSeasonDetail onSeasonDetail;
        [SerializeField] private UIButton rankingButton;
        [SerializeField] private UIButton recordButton;
        [SerializeField] private UIButton historyButton;
        [SerializeField] private UIButton rewardButton;
        [SerializeField] private UIButton shopButton;
        [SerializeField] private UIButton matchingButton;
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private StaminaView staminaView;
        [SerializeField] private TMP_Text seasonTitleText;
        [SerializeField] private TMP_Text seasonPeriodText;

        #endregion

        private ColosseumState colosseumState = ColosseumState.ExitSeason;
        private ColosseumSeasonData currentColosseumSeasonData;
        private ColosseumEventMasterObject currentColosseumEvent;

        #region OverrideMethods

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {

            var param = (Data)args;
            
            colosseumState = param.colosseumState;
            
            // seasonDataはシーズン未開催だとnullの場合があるのでマスタと分けて持っておく
            currentColosseumSeasonData = param.colosseumSeasonData;
            currentColosseumEvent = param.colosseumEventMaster;
            
            var staminaId = currentColosseumEvent.mStaminaId;
            var deckList = await DeckUtility.GetDeckList(DeckType.Battle);
            var leaderId = deckList.DeckDataList[0].GetMemberId(0);
            var variableChara = UserDataManager.Instance.charaVariable.Find(leaderId);
            
            await characterCardImage.SetTextureAsync(variableChara.charaId);
            await staminaView.UpdateAsync(StaminaUtility.StaminaType.Colosseum,staminaId);
            
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            InitializeSeasonDetail();
            InitializeButtonState();
            AppManager.Instance.TutorialManager.OpenColosseumTutorialAsync().Forget();
            // シークレットセール表示
            ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.ColosseumTop);
            base.OnOpened(args);
        }

        #endregion

        #region PrivateMethods

        private void InitializeSeasonDetail()
        {
            var isNextSeason = IsNextSeason();
            var seasonTitleKey = isNextSeason ? "pvp.season.next" : "pvp.season";
            seasonTitleText.text = StringValueAssetLoader.Instance[seasonTitleKey];
            
            var isExitSeason = IsExitSeason();
            exitSeasonRoot.SetActive(isExitSeason);
            onSeasonDetail.gameObject.SetActive(!isExitSeason);

            if (isExitSeason)
            {
                seasonPeriodText.text = StringValueAssetLoader.Instance["pvp.totalling"];
            }
            else
            {
                var seasonHome = currentColosseumSeasonData.SeasonHome;
                if (seasonHome == null) return;
                var startAt = seasonHome.startAt.TryConvertToDateTime().GetDateTimeString();
                var endAt = seasonHome.endAt.TryConvertToDateTime().GetDateTimeString(); 
                seasonPeriodText.text = string.Format(StringValueAssetLoader.Instance["pvp.period"],startAt,endAt);
                onSeasonDetail.SetOnSeasonValue(currentColosseumSeasonData,colosseumState);
            }
        }

        private void InitializeButtonState()
        {
            var isOnSeason = IsOnSeason();
            rankingButton.interactable = isOnSeason;
            recordButton.interactable = true;
            historyButton.interactable = true;
            rewardButton.interactable = isOnSeason;
            shopButton.interactable = (currentColosseumEvent?.mCommonStoreCategoryId ?? 0) != 0;
            matchingButton.interactable = isOnSeason;
        }

        private bool IsExitSeason()
        {
            return colosseumState == ColosseumState.ExitSeason;
        }

        private bool IsOnSeason()
        {
            return colosseumState == ColosseumState.OnSeason;
        }

        private bool IsNextSeason()
        {
            return colosseumState == ColosseumState.NextSeason;
        }

        #endregion

        #region EventListeners

        public void OnClickRanking()
        {
            ColosseumRankingModal.Open(currentColosseumSeasonData);
        }
        
        public void OnClickRecord()
        {
            ColosseumRecordModal.Open(new ColosseumRecordModal.ModalParams(currentColosseumEvent));
        }
        
        public void OnClickHistory()
        {
            ColosseumHistoryModal.Open(currentColosseumEvent);
        }
        
        public void OnClickReward()
        {
            ColosseumRewardModal.Open(currentColosseumSeasonData);
        }
        
        public void OnClickExchange()
        {
            ShopExchangeModal.Open(currentColosseumEvent?.mCommonStoreCategoryId ?? 0);
        }

        public void OnClickMatching()
        {
            var m = (ColosseumPage)Manager;
            m.OpenPage(ColosseumPageType.ColosseumMatching, true, new ColosseumMatchingPage.Data(currentColosseumSeasonData));
        }

        public void OnClickBack()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false,null);
        }

        #endregion
    }
}
