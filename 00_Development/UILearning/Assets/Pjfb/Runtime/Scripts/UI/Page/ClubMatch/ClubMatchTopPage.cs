using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Community;
using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;

namespace Pjfb.ClubMatch
{

    public class ClubMatchTopPage : Page
    {
        private static readonly string OpenKey = "Open";
        private static readonly string CloseKey = "Close";

        public class Data : PageData
        {
            public ColosseumState ClubMatchState {
                get
                {
                    if (SeasonData.SeasonHome == null)
                    {
                        return ColosseumState.Unknown;
                    }
                    else
                    {
                        return ColosseumManager.GetColosseumState(SeasonData.SeasonHome);
                    }
                } 
            }

            public Data(ColosseumSeasonData seasonData)
            {
                targetPage = ClubMatchPageType.ClubMatchTop;
                if (seasonData == null) return;
                SeasonData = seasonData;
            }

            public bool IsCalculatingRank => false;

            public TurnStateEnum GetTurnState()
            {
                if (SeasonData.ScoreBattleTurn == null) return default;
                var curTime = AppTime.Now;
                var start = SeasonData.ScoreBattleTurn.startAt.TryConvertToDateTime();
                var end = SeasonData.ScoreBattleTurn.endAt.TryConvertToDateTime();
                if (curTime.Ticks < start.Ticks) return TurnStateEnum.NextTurn;
                else if (curTime.Ticks > end.Ticks) return TurnStateEnum.ExitTurn;
                else return TurnStateEnum.OnTurn;
            }

        }

        public class IntervalUpdater
        {
            public class UpdaterData
            {
                public DateTime target;
                public TimeSpan delay;
                
                public bool isCanceled;
                public CancellationToken CancelationToken { get; internal set; }
                public TimeSpan Remaining { get; internal set; }
            }

            private UpdaterData _data;
            public IntervalUpdater(UpdaterData data, CancellationToken cancellationToken)
            {
                _data = data;
                _data.CancelationToken = cancellationToken;
            }

            public async UniTask UpdateInterval(Action<UpdaterData> intervalCB, CancellationToken token)
            {
                try
                {
                    while (!(_data.isCanceled || token.IsCancellationRequested))
                    {
                        _data.Remaining = _data.target - AppTime.Now;
                        intervalCB.Invoke(_data);
                        await UniTask.Delay(_data.delay, cancellationToken: token);
                    }
                }
                catch (Exception){}
            }
        }

        private enum CalculationTypeEnum
        {
            TurnCalculation = 0,
            SeasonCalculation = 1,
        }
        
        #region SerializeFields
        [SerializeField] private GameObject seasonMessage;
        [SerializeField] private GameObject turnMessage;
        [SerializeField] private TMP_Text seasonTitle;
        [SerializeField] private TMP_Text seasonPeriod;
        [SerializeField] private TMP_Text seasonMessageText;
        [SerializeField] private TMP_Text turnMessageLeft;
        [SerializeField] private TMP_Text turnMessageRight;
        [SerializeField] private UIButton positiveButton;
        [SerializeField] private UIButton personalRankButton;
        [SerializeField] private UIButton rewardConfirmationButton;
        [SerializeField] private RectTransform calculatingRankOverlay;
        [SerializeField] private UIBadgeBalloon batchPersonalRankBalloon;
        [SerializeField] private GameObject listItemPrefab;
        [SerializeField] private List<GameObject> rankChangeFeatureOnOffGroup;
        [SerializeField] private List<GameObject> overlayStatus;
        [SerializeField] private CharacterVariableIcon leaderIcon;
        [SerializeField] protected DeckRankImage deckRankImage;
        [SerializeField] private TMP_Text userClubName;
        [SerializeField] private TMP_Text userName;
        [SerializeField] private TMP_Text userScore;
        [SerializeField] private OmissionTextSetter userScoreOmissionTextSetter;
        [SerializeField] private TMP_Text userCombatPower;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private Animator menuModal;
        [SerializeField] private RectTransform menuModalCloseButton;
        [SerializeField] private UIButton refreshUIButton;
        [SerializeField] private UIBadgeNotification menuBadge;
        [SerializeField] private UIBadgeNotification clubChatBadge;
        [SerializeField] private TextMeshProUGUI exp;
        [SerializeField] private TextMeshProUGUI nextExp;
        [SerializeField] private ClubRankImage clubRankImage;
        // [SerializeField] private 
        #endregion

        private ClubAccessLevel myAccessLevel;
        private Data currentArgs;

        private CancellationTokenSource cancellationTokenSource;

        private List<ClubMatchTopRankListItem> rankListItems;

        #region OverrideMethods

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentArgs = (Data)args;
            if (currentArgs.SeasonData == null)
            {
                return;
            }
            if (rankListItems == null)
            {
                rankListItems = new List<ClubMatchTopRankListItem>();
            }
            else
            {
                rankListItems.Clear();
            }
            
            refreshUIButton.SetClickIntervalTimer(refreshUIButton.ClickTriggerInterval);
            await InitUI();
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            
            AppManager.Instance.TutorialManager.OpenClubMatchTutorialAsync().Forget();
            
            if (currentArgs.callerPage == PageType.MatchResult)
            {
                ShowNegativeResultModal();
            }
            
        }

        protected override UniTask<bool> OnPreLeave(CancellationToken token)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();    
            }
            return base.OnPreLeave(token);
        }

        #endregion

        #region PrivateMethods

        private async UniTask UpdateInterval()
        {
            var targetTime = default(DateTime);
            var remainingFormat = StringValueAssetLoader.Instance["clubmatch.top.remaning_time"];
            var turnStateEnum = currentArgs.GetTurnState();
            switch (currentArgs.ClubMatchState)
            {
                case ColosseumState.Unknown:
                    break;
                case ColosseumState.NextSeason:
                    targetTime = currentArgs.SeasonData.SeasonHome.startAt.TryConvertToDateTime();
                    break;
                case ColosseumState.OnSeason:
                    switch (turnStateEnum)
                    {
                        case TurnStateEnum.NextTurn:
                            targetTime = currentArgs.SeasonData.ScoreBattleTurn.startAt.TryConvertToDateTime();
                            break;
                        case TurnStateEnum.OnTurn:
                            targetTime = currentArgs.SeasonData.ScoreBattleTurn.endAt.TryConvertToDateTime();
                            break;
                    }
                    
                    break;
            }

            if (AppTime.Now > targetTime)
            {
                return;
            }

            var updaterData = new IntervalUpdater.UpdaterData()
            {
                target = targetTime,
                delay = TimeSpan.FromSeconds(0.5f)
            };
            var myTimer = new IntervalUpdater(updaterData ,  cancellationTokenSource.Token);
            
            await myTimer.UpdateInterval(data =>
            {
                if (data.Remaining.Ticks <= 0)
                {
                    data.isCanceled = true;
                    return;
                };
            }, cancellationTokenSource.Token);

            if (!updaterData.CancelationToken.IsCancellationRequested)
            {
                //シーズンが終了し集計に入る旨のポップアップ
                if (currentArgs.ClubMatchState == ColosseumState.ExitSeason)
                {
                    ClubMatchUtility.OpenSeasonChangeModal(ClubMatchSeasonChangeModalType.End);
                }
                else if (currentArgs.ClubMatchState != ColosseumState.Unknown)
                {
                    InitUI().Forget();
                }
            }
        }

        private string GetRemainingTimeUnit(TimeSpan remainingTime)
        {
            var unitList = StringValueAssetLoader.Instance["clubmatch.time.unit"].Split(',');
            string unit = "";
            if (remainingTime.TotalHours >= 24)
            {
                unit = $"{(int)remainingTime.Days}{unitList[3]}";
            } else if (remainingTime.TotalHours < 24 && remainingTime.TotalHours >= 1)
            {
                unit = $"{(int)remainingTime.TotalHours}{unitList[2]}";
            } else if (remainingTime.TotalHours < 1 && remainingTime.TotalMinutes >= 1)
            {
                unit = $"{(int)remainingTime.TotalMinutes}{unitList[1]}";
            }
            else if (remainingTime.Minutes < 1 && remainingTime.TotalSeconds >= 1)
            {
                unit = $"{(int)remainingTime.TotalSeconds}{unitList[0]}";
            }
            else
            {
                unit = String.Empty;
            }

            //Debug.Log($"[GetRemainingTimeUnit]{remainingTime.ToString()}");
            return unit;
        }

        private void CleanUpOldRankingInstance()
        {
            var parent = listItemPrefab.transform.parent;
            if (parent.childCount > 1)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    var child = parent.GetChild(i);
                    child.gameObject.SetActive(false);
                    if (!child.Equals(listItemPrefab.transform)) Destroy(child.gameObject);
                }
            }
        }

        private async UniTask InitUI(bool isUpdateInterval = true)
        {
            if (isUpdateInterval)
            {
                if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
            }
            menuModal.gameObject.SetActive(false);
            CleanUpOldRankingInstance();
            batchPersonalRankBalloon.SetText("");
            seasonMessageText.text = "";
            userScore.text = "-";
            userCombatPower.text = "-";
            leaderIcon.gameObject.SetActive(false);
            
            Debug.Log($"{currentArgs.SeasonData.UserSeasonStatus == null}");
            // ギルドId
            long groupId = currentArgs.SeasonData.UserSeasonStatus?.groupSeasonStatus.groupId ?? UserDataManager.Instance.user.gMasterId;
            // グレードId
            long mColosseumGradeGroupId = currentArgs.SeasonData.MColosseumEvent.mColosseumGradeGroupId;
            // 次ランクまでのEXP
            long nextRankPoint = UserDataManager.Instance.ColosseumGradeData.GetNextRankPoint(mColosseumGradeGroupId, groupId);
            if(nextRankPoint <= 0)
            {
                nextExp.text = "-";
            }
            else
            {
                nextExp.text = string.Format( StringValueAssetLoader.Instance["club.nextRankPoint"], nextRankPoint);
            }
            // 現在EXP
            exp.text = UserDataManager.Instance.ColosseumGradeData.GetRankPoint(mColosseumGradeGroupId, groupId).ToString();
            // クラブマッチランク
            long gradeNumber = UserDataManager.Instance.ColosseumGradeData.GetGradeNumber(mColosseumGradeGroupId, groupId);
            await clubRankImage.SetTextureAsync(gradeNumber);

            var deckList = await DeckUtility.GetDeckList(DeckType.Battle);
            var curDeck = deckList.DeckDataList[deckList.SelectingIndex];
            var leaderId = curDeck.GetMemberId(0);
            var leader = UserDataManager.Instance.charaVariable.Find(leaderId);
            
            var leaderRank = StatusUtility.GetCharacterRank(new BigValue(leader.combatPower));
            if (currentArgs.ClubMatchState == ColosseumState.Unknown)
            {
                leaderIcon.SetIconTextureWithEffectAsync(leader.charaId).Forget();
                leaderIcon.SetIcon(new BigValue(leader.combatPower), leaderRank);
                leaderIcon.gameObject.SetActive(true);
            }

            deckRankImage.gameObject.SetActive(false);
            userName.text = UserDataManager.Instance.user.name;
            userClubName.text = "";
            
            var userSeasonStatus = currentArgs.SeasonData.UserSeasonStatus;
            ColosseumRankingGroup[] battleRank = default;
            ColosseumRankingUser[] rankingUserList = default;

            if (currentArgs.ClubMatchState != ColosseumState.Unknown)
            {
                userScore.text = new BigValue(userSeasonStatus.score).ToDisplayString(userScoreOmissionTextSetter.GetOmissionData());
                //Battle Rank
                battleRank = await ColosseumManager.ColosseumGetGroupBattleRanking(currentArgs.SeasonData.SeasonId);
                var gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(
                    currentArgs.SeasonData.MColosseumEvent.mColosseumGradeGroupId, userSeasonStatus.gradeNumber);
                rankingUserList = await ColosseumManager.RequestGetRankingAsync(currentArgs.SeasonData.SeasonId,gradeMaster.roomCapacity * ConfigManager.Instance.maxGuildMemberCount);
                userClubName.text = currentArgs.SeasonData.UserSeasonStatus.groupSeasonStatus.name;
            }
            seasonPeriod.gameObject.SetActive(currentArgs.ClubMatchState != ColosseumState.Unknown);
            seasonPeriod.text = string.Format(StringValueAssetLoader.Instance["pvp.period"], 
                (currentArgs.ClubMatchState == ColosseumState.Unknown? currentArgs.SeasonData.MColosseumEvent.startAt : currentArgs.SeasonData.SeasonHome.startAt).TryConvertToDateTime().ToString("MM/dd HH:mm"),
                (currentArgs.ClubMatchState == ColosseumState.Unknown? currentArgs.SeasonData.MColosseumEvent.endAt : currentArgs.SeasonData.SeasonHome.endAt).TryConvertToDateTime().ToString("MM/dd HH:mm"));
            seasonTitle.text = currentArgs.SeasonData.MColosseumEvent.name;
            seasonMessage.gameObject.SetActive(false);
            turnMessage.gameObject.SetActive(false);
            positiveButton.interactable = false;
            personalRankButton.interactable = false;
            rewardConfirmationButton.interactable = false;
            batchPersonalRankBalloon.SetActive(false);
            refreshUIButton.gameObject.SetActive(currentArgs.SeasonData.SeasonHome != null);
            menuBadge.SetActive(CommunityManager.ShowClubChatBadge);
            switch (currentArgs.ClubMatchState)
            {
                case ColosseumState.Unknown:
                    if (currentArgs.SeasonData.SeasonHome == null)
                    {
                        overlayStatus[(int)CalculationTypeEnum.SeasonCalculation].SetActive(true);
                    }
                    break;
                case ColosseumState.ExitSeason:
                    seasonMessage.gameObject.SetActive(true);
                    seasonMessageText.text = StringValueAssetLoader.Instance["clubmatch.top.season_end"];
                    personalRankButton.interactable = true;
                    rewardConfirmationButton.interactable = true;
                    break;
                case ColosseumState.NextSeason:
                    if (isUpdateInterval) UpdateInterval().Forget();
                    break;
                case ColosseumState.OnSeason:
                    var turnStateEnum = currentArgs.GetTurnState();
                    switch (turnStateEnum)
                    {
                        case TurnStateEnum.ExitTurn:
                            break;
                        case TurnStateEnum.NextTurn:
                            positiveButton.interactable = true;
                            break;
                        case TurnStateEnum.OnTurn:
                            positiveButton.interactable = true;
                            batchPersonalRankBalloon.SetActive(true);
                            batchPersonalRankBalloon.SetText(string.Format(StringValueAssetLoader.Instance["clubmatch.top.personal_rank.baloon.format"], userSeasonStatus.ranking));
                            turnMessage.gameObject.SetActive(true);
                            break;
                    }
                    personalRankButton.interactable = true;
                    rewardConfirmationButton.interactable = true;
                    if (isUpdateInterval) UpdateInterval().Forget();
                    break;
            }
            
            //Update RankList
            if (userSeasonStatus != null && currentArgs.ClubMatchState != ColosseumState.Unknown)
            {
                await UpdateRankList(battleRank, rankingUserList);
            }
        }

        private async UniTask UpdateRankList(ColosseumRankingGroup[] list, ColosseumRankingUser[] rankingUsers)
        {
            if (list == default) return;
            var rankingUserList = new List<ColosseumRankingUser>(rankingUsers);
            rankingUserList.ForEach(user =>
            {
                if (UserDataManager.Instance.user.uMasterId.Equals(user.uMasterId))
                {
                    var clubMatchTotalCombatPower = ColosseumManager.GetClubMatchTotalCombatPower(user.defenseCount, new BigValue(user.combatPower));
                    deckRankImage.gameObject.SetActive(true);
                    deckRankImage.SetTexture(StatusUtility.GetPartyRank(clubMatchTotalCombatPower));
                    userCombatPower.text = clubMatchTotalCombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
                    var leaderRank = StatusUtility.GetCharacterRank( new BigValue(user.leaderIconChara.combatPower) );
                    leaderIcon.SetIconTextureWithEffectAsync(user.leaderIconChara.mCharaId).Forget();
                    leaderIcon.SetIcon( new BigValue(user.leaderIconChara.combatPower), leaderRank);
                    leaderIcon.gameObject.SetActive(true);
                }

            });
            var bAllowShowRankChange = true;
            list.ForEach(item => bAllowShowRankChange &= item.rankingChangeType > 0);
            foreach (var item in list)
            {
                var target = Instantiate(listItemPrefab, listItemPrefab.transform.parent);
                var itemui = target.GetComponent<ClubMatchTopRankListItem>();
                rankListItems.Add(itemui);
                var isSelf = item.groupId.Equals(currentArgs.SeasonData.UserSeasonStatus.groupSeasonStatus.groupId);
                itemui.Init(currentArgs.SeasonData, item, rankingUserList.FindAll(r => item.groupId.Equals(r.groupId) && item.groupType.Equals(r.groupType)).ToArray(), isSelf, isShowRankingChange: bAllowShowRankChange);
                target.SetActive(true);
                // if (isSelf)
                // {
                //     clubRank.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], item.ranking);
                //     clubScore.text = item.score.ToString();
                // }
            }
            await UniTask.Delay(1);
            foreach (var o in rankChangeFeatureOnOffGroup)
            {
                o.SetActive(bAllowShowRankChange);
            }
        }
        private void ShowNegativeResultModal()
        {
            // 敗北リザルト → クラブマッチTOP → チャット と遷移してprevPageでTOPに戻るとモーダルが再度表示されてしまうためcallerPageを戻しておく
            currentArgs.callerPage = PageType.Home;
            var title = StringValueAssetLoader.Instance["clubmatch.lose.title"];
            var body = StringValueAssetLoader.Instance["clubmatch.lose.body"];
            ConfirmModalWindow.Open(new ConfirmModalData(
                title, body,
                string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => window.Close())));
        }
        
        #endregion

        #region EventListeners

        public void OnClickPositiveButton()
        {
            ClubMatchPage.OpenPage(true, new ClubMatchMatchingPage.Data(currentArgs.SeasonData));
        }

        public void OnClickBack(UIButton sender)
        {
            cancellationTokenSource.Cancel();
            if (currentArgs.isStackedPage)
            {
                sender.BackPage();
            }
            else
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(currentArgs.callerPage, false,null);
            }
        }
        
        public void OnClickPersonalRankingButton()
        {
            ClubMatchPersonalRankingModal.Open(currentArgs.SeasonData);
        }

        public void OnClickMatchHistoryButton()
        {
            ClubMatchRecordModal.Open(currentArgs.SeasonData.MColosseumEvent, currentArgs.SeasonData.SeasonId);
        }

        public void OnClickPastRecordButton()
        {
            // 過去戦績モーダルに必要なパラメータ作成
            ClubMatchPastRecordModal.PastRecordArguments args = new ClubMatchPastRecordModal.PastRecordArguments(
                new ColosseumClientHandlingType[]
                {
                    ColosseumClientHandlingType.ClubMatch
                });
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchPastRecord, args);
        }

        public void OnClickRewardConfirmation()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchReward, currentArgs.SeasonData);
        }
        
        public void OnClickExchangeShopButton()
        {
            ShopExchangeModal.Open(currentArgs.SeasonData.MColosseumEvent.mCommonStoreCategoryId);
        }

        public void OnClickCommunityButton()
        {
            CommunityManager.OnClickCommunityButton();
        }
        
        public void OnClickHelpButton()
        {
            AppManager.Instance.TutorialManager.OpenClubMatchHelpAsync().Forget();
        }

        public async void OnClickMenuModals(bool val)
        {
            menuModal.ResetAllTriggers();
            if (val)
            {
                backKeyObject = menuModalCloseButton;
                clubChatBadge.SetActive(CommunityManager.ShowClubChatBadge);
                menuModal.gameObject.SetActive(true);
                await AnimatorUtility.WaitStateAsync(menuModal, OpenKey, cancellationTokenSource.Token);
            }
            else
            {
                backKeyObject = null;
                await AnimatorUtility.WaitStateAsync(menuModal, CloseKey, cancellationTokenSource.Token);
                menuModal.gameObject.SetActive(false);
            }
        }

        public void RefreshUI()
        {
            InitUI(false).Forget();
        }

        #endregion
    }
}
