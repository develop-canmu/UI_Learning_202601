using System;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pjfb.LeagueMatchTournament
{
    // 大会バナー
    public class LeagueMatchTournamentBanner : MonoBehaviour
    {
        // 開催状況
        public enum Status
        {
            // エントリー受付中
            EntryStart,
            // エントリー受付済み
            EntryAccept,
            // 不参加
            NotEntry,
            // チームエントリー受付開始
            TeamEntryStart,
            // 初戦試合開始前
            BeforeFirstBattleStart,
            // 試合進行中
            BattleStart,
            // 本日の試合全て終了(最終日以外)
            FinishTodayBattle,
            // 大会の全試合終了(最終日)
            FinishAllBattle,
            // 終了した大会
            EndTournament,
        }

        // バナーイメージ
        [SerializeField]
        private CancellableImage bannerImage;
        
        // 期間表示テキスト
        [SerializeField] 
        private TMP_Text termText = null;

        // バナーのステータス状況テキストルートオブジェクト
        [SerializeField] 
        private GameObject bannerScheduleRoot = null;
        
        // バナーのステータス状況テキスト
        [SerializeField] 
        private TMP_Text bannerRemainingStatusText = null;
        
        // 次のスケジュールまでの残り時間テキストルート
        [SerializeField] 
        private GameObject nextScheduleRemainingRoot = null;
        
        // 次のスケジュールまでの残り時間テキスト
        [SerializeField] 
        private TMP_Text nextScheduleRemainingText = null;

        // フェーズ段階イメージ(予選、本戦..)
        [SerializeField]
        private CancellableImage phaseImage = null;

        // Newバッジ
        [SerializeField]
        private UIBadgeNotification newBadge = null;
        
        private LeagueMatchTournamentInfo matchInfo = null;

        // 更新用タイマー
        private float updateTimer = 0f;
        
        // ビューセット
        public void SetView(LeagueMatchTournamentInfo matchInfo)
        {
            this.matchInfo = matchInfo;

            bannerImage.SetTexture(ResourcePathManager.GetPath("LeagueMatchTournamentBannerImage", matchInfo.MColosseumEvent.seriesOption.bannerImageId));
            // 大会フェーズイメージセット
            phaseImage.SetTexture(ResourcePathManager.GetPath("LeagueMatchTournamentPhaseImage", matchInfo.MColosseumEvent.seriesOption.imageNumber));
            
            // 開催期間
            string startAt = matchInfo.SeasonStartAt.GetDateTimeString();
            string endAt = matchInfo.SeasonEndAt.GetDateTimeString();
            termText.gameObject.SetActive(true);
            termText.text = string.Format(StringValueAssetLoader.Instance["common.event_period"], startAt, endAt);

            // オブジェクトのアクティブ初期化
            bannerScheduleRoot.gameObject.SetActive(true);
            nextScheduleRemainingRoot.gameObject.SetActive(true);
            // バッジ表示
            newBadge.SetActive(ColosseumManager.IsNewEvent(matchInfo.MColosseumEvent.id));
            
            // 既読リストに追加
            ColosseumManager.SaveViewedEventId(matchInfo.MColosseumEvent.id);
            
            updateTimer = 0f;
 
            switch (matchInfo.BannerStatus)
            {
                // エントリー受付中
                case Status.EntryStart:
                {
                    // エントリー受付終了まで
                    bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.entry"];
                    break;
                }
                // エントリー受付済み
                case Status.EntryAccept:
                {
                    // エントリー期間中ならエントリー受付終了までの表記に
                    if (matchInfo.IsEntryTerm)
                    {
                        // エントリー受付終了まで
                        bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.entry"];   
                    }
                    // シーズン開始前ならシーズン開始まで
                    else if (matchInfo.SeasonStartAt.IsFuture(AppTime.Now))
                    {
                        bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.start-season"];   
                    }
                    // それ以外の場合はシーズンデータ待ちなので残り時間の表示を非表示に
                    else
                    {
                        bannerScheduleRoot.gameObject.SetActive(false);
                    }
                    break;
                }
                // 不参加の大会は次のスケジュールを表示しない
                case Status.NotEntry:
                {
                    bannerScheduleRoot.gameObject.SetActive(false);
                    break;
                }
                // チームエントリー受付中
                case Status.TeamEntryStart:
                {
                    // チームエントリー受付終了まで
                    bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.team-entry"];
                    break;
                }
                // 初戦開始前
                case Status.BeforeFirstBattleStart:
                {
                    // 第1試合開始まで
                    bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.first-battle-start"];
                    break;
                }
                // 試合進行中
                case Status.BattleStart:
                {
                    // 次のラウンド数
                    long nextRound = 0; 
                    // 終了したラウンド数
                    long finishRound = 0;
                    LeagueMatchInfoSchedule nextSchedule = matchInfo.GetNextSchedule();
                    if (nextSchedule != null)
                    {
                        nextRound = nextSchedule.RoundNumber;
                        finishRound = nextRound - 1;
                    }
                    // 第＊試合まで終了　第＊試合開始まで
                    bannerRemainingStatusText.text = string.Format(StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.battle-start"], finishRound, nextRound);
                    break;
                }
                // 本日の試合終了
                case Status.FinishTodayBattle:
                {
                    // 本日の全試合終了！次回マッチングまで
                    bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.today-finish-battle"];
                    break;
                }
                // 全試合終了
                case Status.FinishAllBattle:
                {
                    // 残り時間の表示は無し
                    nextScheduleRemainingRoot.gameObject.SetActive(false);
                    // 全試合終了！
                    bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.all_battle_ended"];
                    break;
                }
                // 大会終了
                case Status.EndTournament:
                {
                    // 試合結果確認終了まで
                    bannerRemainingStatusText.text = StringValueAssetLoader.Instance["league.match.tournament.banner.next-status.end-tournament"];
                    break;
                }
            }

            // 残り時間の表示
            UpdateRemainingTime();
        }

        private void Update()
        {
            if (matchInfo == null)
            {
                return;
            }

            updateTimer += Time.deltaTime;
            // 更新時間を過ぎたら更新をかける
            if (updateTimer >= LeagueMatchTournamentManager.UpdateTimeInterval)
            {
                updateTimer = 0f;
                // 残り時間更新
                UpdateRemainingTime();   
            }
        }

        /// <summary> 残り時間の更新 </summary>
        private void UpdateRemainingTime()
        {
            // 残り時間の表示が無いなら更新しない
            if (nextScheduleRemainingRoot.gameObject.activeSelf == false)
            {
                return;
            }
            
            // 次のスケジュールまでの残り時間
            string remainingString = StringValueAssetLoader.Instance["common.remain"] + matchInfo.UpdateTime.GetRemainingString(AppTime.Now);
            nextScheduleRemainingText.text = remainingString;
        }
        
        
        /// <summary> バナークリック時の処理 </summary>
        public void OnClickBanner()
        {
            // ステータスを更新
            matchInfo.UpdateStatus();
            
            switch (matchInfo.BannerStatus)
            {
                case Status.EntryStart:
                case Status.EntryAccept:
                case Status.NotEntry:
                {
                    // 大会エントリーページに移動
                    LeagueMatchTournamentPage m = (LeagueMatchTournamentPage)AppManager.Instance.UIManager.PageManager.CurrentPageObject;
                    LeagueMatchTournamentEntryPage.TournamentEntryPageData data = new LeagueMatchTournamentEntryPage.TournamentEntryPageData(matchInfo);
                    m.OpenPage(LeagueMatchTournamentPageType.Entry, true, data);
                    
                    break;
                }
                // リーグマッチTopページに直接移動
                case Status.TeamEntryStart:
                case Status.BeforeFirstBattleStart:
                case Status.BattleStart:
                case Status.FinishTodayBattle:
                case Status.FinishAllBattle:
                {
                    LeagueMatchPage.OpenPage(true, new LeagueMatchPage.Data(matchInfo) { callerPage = PageType.LeagueMatchTournament });
                    break;
                }
                // シーズン終了ページを開く
                case Status.EndTournament:
                {
                    LeagueMatchPage.OpenPage(true, new LeagueMatchSeasonEndPage.Data(matchInfo) { callerPage = PageType.LeagueMatchTournament });
                    break;
                }
            }
            
        }
    }
}