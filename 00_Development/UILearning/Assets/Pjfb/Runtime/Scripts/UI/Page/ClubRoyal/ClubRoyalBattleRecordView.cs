using System;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    // クラブの戦績表示View
    public class ClubRoyalBattleRecordView : MonoBehaviour
    {
        // 表示クラブタイプ
        private enum ClubType
        {
            // 自身の所属クラブ
            MyClub,
            // 対戦対手のクラブ
            OpponentClub,
        }

        // 表示するクラブのタイプ
        [SerializeField] private ClubType clubType = ClubType.MyClub;
        // クラブアイコン画像
        [SerializeField] private ClubEmblemImage clubIcon = null;
        // クラブ名
        [SerializeField] private TMP_Text clubName;
        // シーズン終了までの表示のルートオブジェクト
        [SerializeField] private GameObject endOfSeasonRoot;
        // シーズン終了までの時間
        [SerializeField] private TMP_Text seasonRemainsDate;
        // 戦績の表示
        [SerializeField] private TMP_Text battleRecord;
        // 現在の順位
        [SerializeField] private TMP_Text currentRank;
        // 対戦開始時間表示オブジェクト
        [SerializeField] private GameObject battleDateRoot;
        // 対戦開始時間
        [SerializeField] private TMP_Text battleStartAt;
        // 対戦結果が勝利時のバッジ
        [SerializeField] private GameObject badgeWin;
        // 対戦結果が敗北時のバッジ
        [SerializeField] private GameObject badgeLose;
        // 対戦結果が引き分け時のバッジ
        [SerializeField] private GameObject badgeDraw;

        // ギルドのマッチ情報
        GroupLeagueMatchGroupStatusDetail guildGroupStatusDetail = null;
        
        //// <summary> Viewをセット </summary>
        public async UniTask SetView(LeagueMatchInfo leagueMatchInfo, GroupLeagueMatchBoardInfo boardInfo, bool isShow)
        {
            // 表示するクラブのデータによって切り替え
            switch (clubType)
            {
                // 自分のクラブ表示
                case ClubType.MyClub:
                {
                    guildGroupStatusDetail = boardInfo.groupStatusDetailSelf;
                    break;
                }
                // 対戦クラブ表示
                case ClubType.OpponentClub:
                {
                    guildGroupStatusDetail = boardInfo.groupStatusDetailOpponent;
                    break;
                }
            }

            // 表示するか
            isShow = guildGroupStatusDetail != null && isShow;
           
            // 表示しないならアクティブを切る
            if (isShow == false)
            {
                // ギルドデータがないならエラーを出す
                if (guildGroupStatusDetail == null)
                {
                    CruFramework.Logger.LogError("GroupLeagueMatchGroupStatusDetail is null");
                }

                // 表示しない
                gameObject.SetActive(false);
                return;
            }
            
            // クラブアイコンをセット
            await clubIcon.SetTextureAsync(guildGroupStatusDetail.mGuildEmblemId);
            // クラブ名
            clubName.text = guildGroupStatusDetail.name;
            // 勝敗(引き分けは表示しない)
            battleRecord.text = string.Format(StringValueAssetLoader.Instance["club-royal.match_result.point_result_detail"], guildGroupStatusDetail.winCount, guildGroupStatusDetail.loseCount);
            // 順位がまだ決まっていない
            if (guildGroupStatusDetail.ranking <= 0)
            {
                currentRank.text = StringValueAssetLoader.Instance["league.match.first_day_ranking"];
            }
            // 順位が確定しているなら表示
            else
            {
                currentRank.text = string.Format(StringValueAssetLoader.Instance["league.match_result.ranking"], guildGroupStatusDetail.ranking);
            }
            
            // シーズン終了表示は自クラブの表示の時
            endOfSeasonRoot.gameObject.SetActive(clubType == ClubType.MyClub);
            // 対戦時間の表示は対戦相手クラブの表示の時
            battleDateRoot.gameObject.SetActive(clubType == ClubType.OpponentClub);
            
            // 自分のクラブの表示の場合はシーズン終了までの時間を表示
            if (clubType == ClubType.MyClub)
            {
                // シーズン情報
                ColosseumSeasonData seasonData = leagueMatchInfo.SeasonData;
                if (seasonData != null)
                {
                    // シーズン終了までの時間を表示
                    seasonRemainsDate.text = seasonData.SeasonHome.endAt.TryConvertToDateTime().GetRemainingString(AppTime.Now);
                    seasonRemainsDate.gameObject.SetActive(true);
                }
                // シーズン情報がないなら非表示
                else
                {
                    seasonRemainsDate.gameObject.SetActive(false);
                }   
            }

            // 対戦対手の表示の場合は対戦までの時間の結果を表示する
            else if (clubType == ClubType.OpponentClub)
            {
                // 今日の対戦スケジュールを取得
                LeagueMatchInfoSchedule schedule = leagueMatchInfo.GetBattleRoyalScheduleOfToday();
                
                if (schedule != null)
                {
                    // 対戦開始時間
                    DateTime startAt = schedule.StartAt;
                    // 対戦日時を表示
                    battleStartAt.text = startAt.GetNewsDateTimeString();
                }
                // スケジュールがnullになる可能性があるので対戦時間を空に
                else
                {
                    battleStartAt.text = "";
                }
                
                // 結果によってラベルを表示
                ShowBattleResult(boardInfo.todayMatch);
            }
            
            // 全てのセットが終わってから表示をオンに
            gameObject.SetActive(true);
        }

        //// <summary> 対戦結果表示バッジをすべて非表示 </summary>
        private void HideResultBadge()
        {
            badgeWin.SetActive(false);
            badgeLose.SetActive(false);
            badgeDraw.SetActive(false);
        }

        //// <summary> 今日の試合の結果によって勝敗を表示 </summary>
        private void ShowBattleResult(GroupLeagueMatchTodayMatch todayMatch)
        {
            // いったん全部非表示
            HideResultBadge();
            
            // 今日の試合データがないならリターン
            if (todayMatch == null)
            {
                return;
            }
            
            // 試合結果
            long result = todayMatch.result;

            // 勝利時
            if (result == ColosseumManager.ResultWin)
            {
                badgeWin.SetActive(true);
            }
            // 敗北時
            else if (result == ColosseumManager.ResultLose)
            {
                badgeLose.SetActive(true);
            }
            // 引き分け時
            else if (result == ColosseumManager.ResultDraw)
            {
                badgeDraw.SetActive(true);
            }
        }

        //// <summary> クラブアイコンロングタップ時の処理 </summary>
        public void OnLongTapClubIcon()
        {
            // 情報がない場合はリターン
            if (guildGroupStatusDetail == null)
            {
                return;
            }
            // クラブ情報を開く
            LeagueMatchUtility.OpenClubInfo(guildGroupStatusDetail.groupId, guildGroupStatusDetail.groupType).Forget();
        }
    }
}