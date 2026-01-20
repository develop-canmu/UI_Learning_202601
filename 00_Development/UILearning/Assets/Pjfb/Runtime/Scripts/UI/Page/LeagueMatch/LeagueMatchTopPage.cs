using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using UnityEngine;
using Pjfb.Networking.App.Request;
using TMPro;
using Pjfb.Master;
using Pjfb.UI;
using Pjfb.UserData;
using Pjfb.Menu;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchTopPage : Page
    {
        public class Data : PageData
        {
            public MatchType matchingType;
            public long inGameMatchId;
            
            public Data(MatchType matchingType, LeagueMatchInfo leagueMatchInfo, long inGameMatchId = 0) : base(leagueMatchInfo)
            {
                this.matchingType = matchingType;
                this.targetPage = LeagueMatchPageType.LeagueMatchTop;
                this.seasonData = leagueMatchInfo.SeasonData;
                this.inGameMatchId = inGameMatchId;
            }
        }
        
        [SerializeField] private TextMeshProUGUI myScore;
        [SerializeField] private TextMeshProUGUI teamPoint;
        [SerializeField] private TextMeshProUGUI matchName;
        
        #region header
        [SerializeField] private ClubRankImage playerRankIconHeader;
        [SerializeField] private GameObject playerRankIconDummy;
        [SerializeField] private ClubRankImage opponentRankIconHeader;
        [SerializeField] private GameObject opponentRankIconDummy;
        [SerializeField] private RectTransform opponentRankIconHeaderRoot;
        #endregion
        
        [SerializeField] private LeagueMatchTopHeaderTeamInfo teamLeftHeaderInfo;
        [SerializeField] private LeagueMatchTopHeaderTeamInfo teamRightHeaderInfo;
        
        [SerializeField] private PoolListContainer entryListContainer;
        [SerializeField] private PoolListContainer battleListContainer;
        [SerializeField] private TMP_Text scheduleText;
        [SerializeField] private TMP_Text matchCurrentStatusInfoText;
        [SerializeField] private TMP_Text scheduleTilEndText;
        [SerializeField] private TextMeshProUGUI remainEntryCount;
        [SerializeField] private Color warnColor;
        [SerializeField] private Color defaultColor;

        [SerializeField] private GameObject mainRoot;
        [SerializeField] private GameObject preparingOverlay;
        [SerializeField] private GameObject countingOverlay;
        
        [SerializeField] private UIBadgeNotification menuBadge;

        [SerializeField] private LeagueMatchHelp help;
        
        private Data currentArgs;
        private BattleReserveFormationMatchInfo matchInfo;

        private bool doRefresh = false;
        private DateTime refreshDateTime = DateTime.MaxValue;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentArgs = (Data)args;
            // 画面更新
            LeagueMatchUtility.UpdateMenuNotification(menuBadge);
            // アクティブがオフになってる場合があるので有効化
            mainRoot.SetActive(true);
            // リフレッシュ時間リセット
            refreshDateTime = DateTime.MaxValue;
            // ヘルプの設定
            help.SetHelp(currentArgs.leagueMatchInfo);
            
            await OnPreOpenExecution(false);
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            // ポップアップの表示
            AppManager.Instance.TutorialManager.OpenLeagueMatchTutorialAsync().Forget();
        }

        private async void Update()
        {
            // inGameMatchIdが0以下ならリフレッシュしない
            if (currentArgs.inGameMatchId <= 0) return;
            // リフレッシュしない
            if(!doRefresh) return;
            // リフレッシュ 
            if(refreshDateTime.IsPast(AppTime.Now))
            {
                // API実行中にUpdateしないように
                doRefresh = false;
                // 更新
                await OnPreOpenExecution(true);
                
                // エントリ受付中じゃない
                if(matchInfo.progress != (int)Progress.EntryOpen)
                {
                    scheduleTilEndText.text = string.Empty;
                }
                return;
            }
            
            // 文字列更新
            if(matchInfo.progress == (int)Progress.EntryOpen)
            {
                scheduleTilEndText.text = currentArgs.leagueMatchInfo.GetTilEntryEndText();
            }
        }
        
        private async UniTask OnPreOpenExecution(bool refresh)
        {
            if (currentArgs.inGameMatchId != -1)
            {
                if (refresh || currentArgs.matchingType == MatchType.Season)
                {
                    matchInfo = await LeagueMatchUtility.GetMatchInfo(currentArgs.inGameMatchId);
                }
                else if (currentArgs.matchingType == MatchType.Shift)
                {
                    var mBattleReserveFormationMasterObject = MasterManager.Instance.battleReserveFormationMaster.FindData(currentArgs.leagueMatchInfo.SeasonData.MColosseumEvent.inGameSystemId);
                    matchInfo = await LeagueMatchUtility.GetMatchInfoByEventInfo(mBattleReserveFormationMasterObject.eventType, currentArgs.leagueMatchInfo.SeasonData.UserSeasonStatus.sColosseumEventId);
                }
                currentArgs.inGameMatchId = matchInfo.matchHeader.id;
                LeagueMatchPage parentPage = (LeagueMatchPage)Manager;
                parentPage.InGameMatchId = matchInfo.matchHeader.id;
            }
            
            if(matchInfo == null)
            {
                doRefresh = false;
                await InitUI();
                return;
            }

            // progress指定なしor試合が終わってる
            if(matchInfo.progress == (int)Progress.None || matchInfo.progress == (int)Progress.MatchEnd)
            {
                // リフレッシュしない
                doRefresh = false;
            }
            // 集計期間中なら60秒ごとにリフレッシュ
            else if(matchInfo.progress == (int)Progress.Aggregating)
            {
                // リフレッシュする
                doRefresh = true;
                refreshDateTime = AppTime.Now + TimeSpan.FromSeconds(LeagueMatchUtility.RefreshInterval);
            }
            else
            {
                // リフレッシュする
                doRefresh = true;
                // 次のスケジュール時間
                var nextSchedule = currentArgs.leagueMatchInfo.GetNextSchedule();
                // 次のスケジュールがない or 当日行われない
                if(nextSchedule == null || !DateTimeExtensions.IsSameDay(nextSchedule.StartAt, AppTime.Now))
                {
                    refreshDateTime = AppTime.Now + TimeSpan.FromSeconds(LeagueMatchUtility.RefreshInterval);
                }
                else
                {
                    refreshDateTime = nextSchedule.StartAt;
                }
            }
            
            await InitUI();
        }
        
        private async UniTask InitUI()
        {
            scheduleText.text = currentArgs.leagueMatchInfo.IsOnSeason ? 
                LeagueMatchManager.GetDateTimeString(currentArgs.leagueMatchInfo.SeasonStartAt, currentArgs.leagueMatchInfo.SeasonEndAt) : 
                LeagueMatchManager.GetDateTimeString(currentArgs.leagueMatchInfo.NextSeasonStartAt, currentArgs.leagueMatchInfo.NextSeasonEndAt);
            
            if (currentArgs.inGameMatchId == -1)
            {
                if(currentArgs.leagueMatchInfo.MColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament)
                {
                    matchName.text = currentArgs.leagueMatchInfo.MColosseumEvent.name;
                }
                else
                {
                    matchName.text = StringValueAssetLoader.Instance["league.type.season"];
                }
                opponentRankIconHeaderRoot.gameObject.SetActive(false);
                playerRankIconHeader.gameObject.SetActive(false);
                playerRankIconDummy.SetActive(false);
                preparingOverlay.SetActive(true);
                mainRoot.SetActive(false);
                return;
            }
            
            if ((MatchType)matchInfo.matchHeader.matchingType == MatchType.Shift)
            {
                if(currentArgs.seasonData.IsInstantTournament)
                {
                    // ダミーランク画像
                    playerRankIconDummy.SetActive(true);
                    opponentRankIconDummy.SetActive(true);
                    // ランク画像
                    playerRankIconHeader.gameObject.SetActive(false);
                    opponentRankIconHeader.gameObject.SetActive(false);
                }
                else
                {
                    // ダミーランク画像
                    playerRankIconDummy.SetActive(false);
                    opponentRankIconDummy.SetActive(false);
                    // ランク画像
                    playerRankIconHeader.gameObject.SetActive(true);
                    opponentRankIconHeader.gameObject.SetActive(true);
                    await playerRankIconHeader.SetTextureAsync(matchInfo.groupInfo.gradeNumber);
                    await opponentRankIconHeader.SetTextureAsync(matchInfo.groupInfoOpponent.gradeNumber);
                }
                // タイトルテキスト
                matchName.text = StringValueAssetLoader.Instance["league.type.shift"];
                opponentRankIconHeaderRoot.gameObject.SetActive(true);
            }
            else if ((MatchType)matchInfo.matchHeader.matchingType == MatchType.Season)
            {
                if(currentArgs.seasonData.IsInstantTournament)
                {
                    // タイトルテキスト
                    matchName.text = currentArgs.leagueMatchInfo.MColosseumEvent.name;
                    // ダミーランク画像
                    playerRankIconDummy.SetActive(true);
                    // ランク画像
                    playerRankIconHeader.gameObject.SetActive(false);
                }
                else
                {
                    // タイトルテキスト
                    matchName.text = StringValueAssetLoader.Instance["league.type.season"];
                    // ダミーランク画像
                    playerRankIconDummy.SetActive(false);
                    // ランク画像
                    playerRankIconHeader.gameObject.SetActive(true);
                    await playerRankIconHeader.SetTextureAsync(matchInfo.matchHeader.gradeNumber);
                }
                opponentRankIconHeaderRoot.gameObject.SetActive(false);
            }
            
            Progress leagueMatchProgress = (Progress)matchInfo.progress;
            
            teamLeftHeaderInfo.IniUI(currentArgs.leagueMatchInfo.MColosseumEvent.clientHandlingType, matchInfo.groupInfo.groupId, matchInfo.groupInfo.groupType, matchInfo.groupInfo.mGuildEmblemId, matchInfo.groupInfo.name, matchInfo.groupInfo.ranking, (BattleResult)matchInfo.matchHeader.result, leagueMatchProgress, 
                (MatchType)matchInfo.matchHeader.matchingType == MatchType.Shift ? matchInfo.groupInfo.gradeNumber : -1);

            BattleResult resultForTeamRight;
            if ((BattleResult)matchInfo.matchHeader.result == BattleResult.Win)
            {
                resultForTeamRight = BattleResult.Lose;
            }
            else if ((BattleResult)matchInfo.matchHeader.result == BattleResult.Lose)
            {
                resultForTeamRight = BattleResult.Win;
            }
            else
            {
                resultForTeamRight = (BattleResult)matchInfo.matchHeader.result;
            }
            
            teamRightHeaderInfo.IniUI(currentArgs.leagueMatchInfo.MColosseumEvent.clientHandlingType, matchInfo.groupInfoOpponent.groupId, matchInfo.groupInfoOpponent.groupType,matchInfo.groupInfoOpponent.mGuildEmblemId, matchInfo.groupInfoOpponent.name, matchInfo.groupInfoOpponent.ranking, resultForTeamRight, leagueMatchProgress, 
                (MatchType)matchInfo.matchHeader.matchingType == MatchType.Shift ? matchInfo.groupInfoOpponent.gradeNumber : -1);
            
            preparingOverlay.gameObject.SetActive(leagueMatchProgress == Progress.None);
            countingOverlay.gameObject.SetActive(leagueMatchProgress == Progress.Aggregating);
            entryListContainer.gameObject.SetActive(leagueMatchProgress == Progress.EntryOpen);
            battleListContainer.gameObject.SetActive(leagueMatchProgress == Progress.InSession || leagueMatchProgress == Progress.EntryClose || leagueMatchProgress == Progress.Aggregating || leagueMatchProgress == Progress.MatchEnd);
            
            var battleReserveFormationMasterValueObject = currentArgs.leagueMatchInfo.GetBattleReserveFormationMasterObject();
            var clubEntryCount = matchInfo.matchLineupList
                // PlayerIdに変換
                .Select(data => data.playerInfo.player.playerId)
                // Idが0のものは除外
                .Where(id => id != 0)
                // 重複してるものは除外
                .Distinct()
                // 個数取得
                .Count();
            var entryCount = matchInfo.matchLineupList.Count(data => data.playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId);
            var remainingEntryCount = battleReserveFormationMasterValueObject.oneUserReserveCount - entryCount;
            
            remainEntryCount.SetText(string.Format("{0}/{1}", remainingEntryCount, battleReserveFormationMasterValueObject.oneUserReserveCount));
            matchCurrentStatusInfoText.text = currentArgs.leagueMatchInfo.GetLeagueMatchTopCurrentStatusInfoText(leagueMatchProgress, (BattleResult)matchInfo.matchHeader.result);
            
            switch (leagueMatchProgress)
            {
                case Progress.None:
                    break;
                case Progress.EntryOpen:
                    await ShowTeamRegister();
                    break;
                case Progress.EntryClose:
                case Progress.InSession:
                case Progress.Aggregating:
                case Progress.MatchEnd:
                    await ShowBattleList();
                    break;
            }
        }
        
        private void ShowTeamPoint()
        {
            // ①1試合も終了していなかった場合
            // テキスト：0-0
            // ②1試合以上終了している場合
            // テキスト：{※自クラブの勝ち点}-{※相手クラブの勝ち点}（{n勝}{n敗}{n分}）
            bool atLeastOneMatchFinished = false;
            foreach (var matchLineup in matchInfo.matchLineupList)
            {
                if ((BattleResult)matchLineup.result != BattleResult.Unprocessed)
                {
                    atLeastOneMatchFinished = true;
                }
            }
            
            teamPoint.text = matchInfo.groupInfo.winningPoint + "-" +　matchInfo.groupInfoOpponent.winningPoint;
            if (atLeastOneMatchFinished)
            {
                myScore.text = 
                    string.Format( StringValueAssetLoader.Instance["league.match_result.point_result_detail"], 
                        matchInfo.groupInfo.winCount,
                        matchInfo.groupInfo.loseCount,
                        matchInfo.groupInfo.drawCount);
            }
            else
            {
                myScore.text = string.Empty;
            }
        }

        private float _registerScrollValue = -1;
        private async UniTask ShowTeamRegister()
        {
            ShowTeamPoint();
            var matchLineupList= matchInfo.matchLineupList;
            var dataList = new List<LeagueMatchFightItem.ItemParams>();
            foreach (var matchLineup in matchLineupList)
            {
                var battleConfigInfo = MasterManager.Instance.battleReserveFormationRoundMaster.FindData(matchLineup.roundNumber);
                var data = new LeagueMatchFightItem.ItemParams
                {
                    battleConfig = battleConfigInfo,
                    matchLineup = matchLineup,
                    inGameMatchId = matchInfo.matchHeader.id,
                    isWithEntryTerm = currentArgs.leagueMatchInfo.IsWithEntryTermOfToday,
                    entryTermEndedWarnPopup = EntryTermEndedWarn,
                    enteredMax = EnteredMax,
                    enteredMaxWarnPopup = EnteredMaxWarn,
                    hasUnsetAuthority = matchInfo.hasUnsetAuthority,
                    mColosseumEvent = currentArgs.leagueMatchInfo.MColosseumEvent,
                    confirmClubMateEntry = (a,b,c,d,e)=>
                    {
                        if (currentArgs.leagueMatchInfo.IsWithEntryTermOfToday())
                        {
                            LeagueMatchManager.ConfirmClubMateEntry(a,b,c,d,e);
                            this._registerScrollValue = entryListContainer.scrollValue;
                        }
                        else
                        {
                            EntryTermEndedWarn();
                        }
                    },
                    confirmOtherClubEntry = LeagueMatchManager.ConfirmOtherClubEntry,
                    onCancelMateEntry = ()=> OnPreOpenExecution(false).Forget(),
                    onLongTapUserIcon = OpenProfile
                };
                dataList.Add(data);
            }

            if (_registerScrollValue < 0)
            {
                await entryListContainer.SetDataList(dataList, slideIn:false);
            }
            else
            {
                await entryListContainer.SetDataList(dataList, scrollValue:_registerScrollValue, slideIn:false);
            }
        }
        
        private async UniTask ShowBattleList()
        {
            ShowTeamPoint();
            var matchLineupList= matchInfo.matchLineupList;
            var dataList = new List<LeagueMatchVSItem.ItemParams>();
            for (var index = 0; index < matchLineupList.Length; index++)
            {
                var matchLineup = matchLineupList[index];
                var battleConfigInfo = MasterManager.Instance.battleReserveFormationRoundMaster.FindData(matchLineup.roundNumber);
                var data = new LeagueMatchVSItem.ItemParams
                {
                    battleConfig = battleConfigInfo,
                    matchLineup = matchLineup,
                    onLongTapUserIcon = OpenProfile
                };
                dataList.Add(data);
            }
            await battleListContainer.SetDataList(dataList);
        }
        
        public void OnClickBack(UIButton sender)
        {
            if (currentArgs != null)
            {
                if (currentArgs.matchingType == MatchType.Season)
                {
                    if (currentArgs.seasonData != null && currentArgs.seasonData.IsOnSeason)
                    {
                        LeagueMatchPage.OpenPage(false, new LeagueMatchBoardPage.Data(currentArgs.leagueMatchInfo) { callerPage = PageType.Home });
                    }
                    else
                    {
                        AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false,null);
                    }
                }
                if (currentArgs.matchingType == MatchType.Shift)
                {
                    // 入れ替え戦がまだ始まってない
                    if(currentArgs.leagueMatchInfo.ShiftBattleStartAt.IsFuture(AppTime.Now))
                    {
                        LeagueMatchPage.OpenPage(false, new LeagueMatchBoardPage.Data(currentArgs.leagueMatchInfo) { callerPage = PageType.Home });
                    }
                    else
                    {
                        AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false,null);
                    }
                }
            }
            else
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false,null);
            }
        }
        
        public void OnClickLeagueMatchMenu()
        {
            LeagueMatchManager.OnClickLeagueMatchMenu(currentArgs.leagueMatchInfo);
        }
        
        bool EnteredMax()
        {
            var battleReserveFormationMasterValueObject = currentArgs.leagueMatchInfo.GetBattleReserveFormationMasterObject();
            var entryCount = matchInfo.matchLineupList.Count(data => data.playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId);
            return battleReserveFormationMasterValueObject.oneUserReserveCount <= entryCount;
        }
        
        // デッキ編成画面登録ボタン押す場合
        void SeatTakenPopup()
        {
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["league.match.cant_entry"],
                StringValueAssetLoader.Instance["league.match.seat_taken_warn"], null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    (window => 
                    {
                        window.Close();
                    }))));
        }
        
        void EntryTermEndedWarn()
        {
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["league.match.entry_term_ended"],
                StringValueAssetLoader.Instance["league.match.entry_term_ended_warn"], null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    (window => 
                    {
                        window.Close();
                    }))));
        }

        void EnteredMaxWarn()
        {
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["league.match.cant_entry"],
                StringValueAssetLoader.Instance["league.match.entered_max_warn"], null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    (window => 
                    {
                        window.Close();
                    }))));
        }
        
        public void OpenProfile(long uMasterId, ColosseumPlayerType playerType)
        {
            ColosseumManager.OpenUserProfileModal(uMasterId, playerType);
        }
    }
}
