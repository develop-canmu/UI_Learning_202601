using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;

namespace Pjfb.Rivalry
{
    public class RivalryEventPage : Page
    {
        #region PageParams
        public class PageParams
        {
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public List<HuntEnemyMasterObject> HuntEnemyMasterObjectList;
            public Action overrideClickBackButton;
            public bool autoTransitToEventTop = false;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private StaminaView staminaView;
        [SerializeField] private RivalryEventBannerImage bannerImage;
        [SerializeField] private PossessionItemUi eventPossessionItemUi;
        [SerializeField] private GameObject eventPeriodRoot;
        [SerializeField] private TMP_Text eventPeriodText;
        [SerializeField] private GameObject ownedPointRoot;
        [SerializeField] private Animator rivalryTowerCompleteBadge;
        [SerializeField] private RivalryTowerComplete rivalryTowerComplete;
        [SerializeField] private UIButton howToPlayButton;
        [SerializeField] private UIButton exchangeButton;
        [SerializeField] private GameObject rewardBoostObject;
        [SerializeField] private TMP_Text rewardBoostText;
        #endregion

        #region PrivateProperties
        private PageParams _pageParams;
        private List<RivalryEventPoolListItem.ItemParams> _listItemParams = new();
        private HuntGetTimetableDetailAPIResponse _response;
        #endregion
        
        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _pageParams = (PageParams) args;

            var huntTimetableMasterObject = _pageParams.huntTimetableMasterObject;
            var endDateTime = huntTimetableMasterObject.endAt.TryConvertToDateTime();
            var IsFarFuture = endDateTime.GetTimeSpanRemaining(AppTime.Now).TotalDays >= 365;
            eventPossessionItemUi.SetPossessionUi(huntTimetableMasterObject.mPointId);
            eventPeriodRoot.SetActive(!IsFarFuture);
            eventPeriodText.text = string.Format(StringValueAssetLoader.Instance["rivalry.top.event_period"], 
                huntTimetableMasterObject.startAt.TryConvertToDateTime().GetNewsDateTimeString(),
                huntTimetableMasterObject.endAt.TryConvertToDateTime().GetNewsDateTimeString());
            
            howToPlayButton.interactable = !string.IsNullOrEmpty(_pageParams.huntMasterObject.helpDescriptionList);
            exchangeButton.gameObject.SetActive(_pageParams.huntTimetableMasterObject.mCommonStoreCategoryId > 0);
            ownedPointRoot.SetActive(_pageParams.huntTimetableMasterObject.mPointId > 0);
            staminaView.InitWithoutUpdateAsync(StaminaUtility.StaminaType.RivalryBattle);
            // 報酬ブースト
            var mHuntSpecificChara = RivalryManager.GetRewardBoost(huntTimetableMasterObject.id);
            if (mHuntSpecificChara != null)
            {
                rewardBoostObject.SetActive(true);
                rewardBoostText.text = string.Format(StringValueAssetLoader.Instance["rivalry.rewardboost.activated"]);
            }
            else
            {
                rewardBoostObject.SetActive(false);
            }
            await GetHuntGetTimetableDetailAPI(_pageParams.huntTimetableMasterObject.id);
            await bannerImage.SetTextureAsync(_pageParams.huntMasterObject.id);
            await base.OnPreOpen(args, token);
        }

        protected override async void OnOpened(object args)
        {
            await ShowPoolList();
            
            ShowRivalryTowerComplete(() =>
            {
                // シークレットセール表示
                ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.Rivalry);
            });
            base.OnOpened(args);
        }
        
        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            await poolListContainer.SlideOut();
            // アニメーションの同期リスト破棄
            DOTweenSyncManager.Clear(RivalryEventPoolListItem.CrossFadeSyncKey);
            return await base.OnPreClose(token);
        }
        #endregion
        
        
        #region PrivateMethods
        private async UniTask ShowPoolList()
        {
            var difficultyDictionary = MasterManager.Instance.huntDifficultyMaster.values.ToDictionary(aData => aData.id);
            var challengedEventMatchDataContainer = RivalryManager.challengedEventMatchDataContainer;
            var seenUnlockedEventMatchDataContainer = RivalryManager.seenUnlockedEventMatchDataContainer;
            challengedEventMatchDataContainer.OnShowEventMatchPage(_response.enemyHistory);

            var enemyMasterObjectList = _pageParams.HuntEnemyMasterObjectList ?? new List<HuntEnemyMasterObject>();
            if (_pageParams.huntMasterObject.isClosedOnceWin)
            {
                enemyMasterObjectList = enemyMasterObjectList.Where(aData => !_response.enemyHistory.mHuntEnemyIdList.Contains(aData.id)).ToList();
            }
            
            enemyMasterObjectList = enemyMasterObjectList.OrderBy(aData => aData.id).ToList();
            _listItemParams = enemyMasterObjectList
                .Select(aData => new RivalryEventPoolListItem.ItemParams(
                    huntEnemyMasterObject: aData,
                    huntTimetableMasterObject: _pageParams.huntTimetableMasterObject,
                    huntMasterObject: _pageParams.huntMasterObject,
                    huntDifficultyMasterObject: difficultyDictionary.TryGetValue(aData.difficulty, out var difficulty) ? difficulty : null,
                    huntEnemyHistory: _response.enemyHistory,
                    challengedEventMatchDataContainer: challengedEventMatchDataContainer,
                    seenUnlockedEventMatchDataContainer: seenUnlockedEventMatchDataContainer,
                    huntEnemyList: enemyMasterObjectList,
                    huntEnemyPrizeList: MasterManager.Instance.huntEnemyPrizeMaster
                        .FindEventRewardList(aData.mHuntId, aData.difficulty, aData.rarity).ToList(),
                    onClickItemParams: OnClickPoolListItem,
                    onUnlockAnimFinished:
                        () => {
                            if (_pageParams.autoTransitToEventTop)
                            {
                                OnClickBack();
                            }
                        }
                    ))
                .ToList();

            var hasNewMatch = _listItemParams.Exists(anItem => anItem.status == RivalryEventPoolListItem.Status.New);
            bool hasUnlockAnimation = _listItemParams.Exists(anItem => anItem.IsUnlocked());
            if ( (!hasNewMatch || hasUnlockAnimation == false) && _pageParams.autoTransitToEventTop) // 該当イベント戦全部クリアしたかつ挑戦回数が切れた場合、直接イベント戦トップページへ遷移
            {
                OnClickBack();
                return;
            }
            
            RivalryManager.showingNewIconEventDataContainer.OnRivalryEventPageShowListItem(
                mHuntId: _pageParams.huntMasterObject.id,
                mHuntTimetableId: _pageParams.huntTimetableMasterObject.id,
                hasNewMatch: hasNewMatch);
            await poolListContainer.SetDataList(_listItemParams);
        }

        private void ShowRivalryTowerComplete(Action onComplete = null)
        {
            bool isTowerEvent = _pageParams.huntMasterObject.isClosedOnceWin;
            bool isComplete = true;
            foreach (var enemy in _pageParams.HuntEnemyMasterObjectList)
            {
                if (!_response.enemyHistory.mHuntEnemyIdList.Any(data => data == enemy.id))
                {
                    isComplete = false;
                    RivalryManager.towerCompleteEventMatchDataContainer.ResetIfCompleted(_pageParams.huntTimetableMasterObject.id);
                    break;
                }
            }

            rivalryTowerCompleteBadge.gameObject.SetActive(isTowerEvent && isComplete);
            if (isTowerEvent && isComplete)
            {
                rivalryTowerCompleteBadge.SetTrigger("Open");
            }
            var showAnimation = isTowerEvent && isComplete && !RivalryManager.towerCompleteEventMatchDataContainer.towerCompleteDataList.Exists(aData => aData.Equals(_pageParams.huntTimetableMasterObject.id));
            rivalryTowerComplete.gameObject.SetActive(showAnimation);
            if (showAnimation)
            {
                rivalryTowerComplete.Open(onComplete);
                RivalryManager.towerCompleteEventMatchDataContainer.OnTowerCompleteAnimation(_pageParams.huntTimetableMasterObject.id);
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        private Dictionary<long, List<PrizeJsonWrap>> GetPrizeDictionary(HuntEnemyPrizeMasterObject.Type type)
        {
            return MasterManager.Instance.huntEnemyPrizeMaster.values
                .Where(aData => aData.typeEnum == type)
                .GroupBy(aData => aData.mHuntEnemyId)
                .ToDictionary(
                    keySelector: aData => aData.Key,
                    elementSelector: aData => aData.SelectMany(aPrize => aPrize.prizeJson).ToList());
        }
        #endregion

        #region API
        private async UniTask GetHuntGetTimetableDetailAPI(long mHuntTimetableId)
        {
            HuntGetTimetableDetailAPIRequest request = new HuntGetTimetableDetailAPIRequest();
            HuntGetTimetableDetailAPIPost post = new HuntGetTimetableDetailAPIPost();
            post.mHuntTimetableId = mHuntTimetableId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            _response = request.GetResponseData();
        }
        #endregion
        
        #region EventListeners
        public void OnClickBack()
        {
            if (_pageParams.overrideClickBackButton != null) _pageParams.overrideClickBackButton.Invoke();
            else {
                RivalryPage m = (RivalryPage)Manager;
                m.OpenPage(RivalryPageType.RivalryTop, false, null);    
            }
        }
        
        private void OnClickPoolListItem(RivalryEventPoolListItem.ItemParams itemParams)
        {
            if (itemParams.status == RivalryEventPoolListItem.Status.Locked) return;

            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.TeamConfirm, 
                true, 
                new TeamConfirmPage.PageParams(
                    PageType.Rivalry, 
                    new RivalryPage.Data(RivalryPageType.RivalryEvent, _pageParams), 
                    itemParams.huntMasterObject,
                    itemParams.huntTimetableMasterObject,
                    itemParams.huntEnemyMasterObject)
            );
        }

        public void OnClickHowToPlay()
        {
            HuntMasterObject mHunt = MasterManager.Instance.huntMaster.FindData(_pageParams.huntMasterObject.id);

            // Id
            MatchCollection ids = Regex.Matches(mHunt.helpImageIdList, "[0-9]+");
            // 説明
            List<object> descriptions = (List<object>)MiniJSON.Json.Deserialize(mHunt.helpDescriptionList);
            
            HowToPlayModal.HowToData howtoData = new HowToPlayModal.HowToData();
            // タイトル
            howtoData.Title = StringValueAssetLoader.Instance["rivalry.event.rules"];
            int index = 0;
            foreach(var id in ids)
            {
                // テクスチャアドレスと説明を追加
                howtoData.Descriptions.Add(new HowToPlayModal.DescriptionData( PageResourceLoadUtility.GetRivalryHowToImagePath(id.ToString()), (string)descriptions[index]));
                index++;
            }
            // 遊び方モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HowToPlay, howtoData);
        }
        
        public void OnClickExchangeButton()
        {
            ShopExchangeModal.Open(_pageParams.huntTimetableMasterObject.mCommonStoreCategoryId, () => 
            {
                eventPossessionItemUi.SetPossessionUi(_pageParams.huntTimetableMasterObject.mPointId);
            });
        }

        public void OnClickBoostEffect()
        {
            BoostEffectModal.Open(
                new BoostEffectModal.Data{
                    huntSpecificCharaMasterObject = RivalryManager.GetRewardBoost(_pageParams.huntTimetableMasterObject.id), 
                    huntTimetableMasterObject = _pageParams.huntTimetableMasterObject, 
                    showCurrentEffect = false,
                    showCharaIconActivation = false,
                }
            );
        }
        
        #endregion
    }
}
