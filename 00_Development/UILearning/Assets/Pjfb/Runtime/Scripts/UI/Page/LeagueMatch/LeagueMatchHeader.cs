using System;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;


namespace Pjfb.LeagueMatch
{
    public class LeagueMatchHeader : MonoBehaviour
    {
        [SerializeField] private ClubEmblemImage clubIcon;
        [SerializeField] private ClubRankImage rankIcon = null;
        [SerializeField] private GameObject rankIconDummy = null;
        [SerializeField] private TextMeshProUGUI leagueMatchType;
        [SerializeField] private TextMeshProUGUI myClubName;
        [SerializeField] private TextMeshProUGUI seasonRemainsDate;
        [SerializeField] private TextMeshProUGUI currentResult;
        [SerializeField] private TextMeshProUGUI currentRank;
        [SerializeField] private LeagueMatchSeasonEndResultLabelImage seasonEndResultLabelImage;
        
        public async UniTask Setup(ColosseumSeasonData seasonData, GroupLeagueMatchBoardInfo boardInfo, LeagueMatchInfo leagueMatchInfo)
        {
            if (seasonData != null)
            {
                seasonRemainsDate.text = seasonData.SeasonHome.endAt.TryConvertToDateTime().GetRemainingString(AppTime.Now);
                seasonRemainsDate.gameObject.SetActive(true);
            }
            else
            {
                seasonRemainsDate.gameObject.SetActive(false);
            }
            
            // ランクアイコン
            if (rankIcon != null)
            {
                // 簡易大会の場合はランク画像を固定化
                rankIconDummy.gameObject.SetActive(seasonData.IsInstantTournament);
                // 簡易大会でなければランク表示
                rankIcon.gameObject.SetActive(!seasonData.IsInstantTournament);
                if(!seasonData.IsInstantTournament)
                {
                    // 最低ランク
                    long gradeNumber = ColosseumManager.MinGradeNumber;
                    // gradeNumber更新
                    if(seasonData != null)
                    {
                        gradeNumber = UserDataManager.Instance.ColosseumGradeData.GetGradeNumber(seasonData.MColosseumEvent.mColosseumGradeGroupId, seasonData.UserSeasonStatus.groupSeasonStatus.groupId);
                    }
                    // 画像セット
                    await rankIcon.SetTextureAsync(gradeNumber);
                }
            }
            
            // クラブアイコン
            await clubIcon.SetTextureAsync(boardInfo.groupStatusDetailSelf.mGuildEmblemId);
            
            // シーズン戦か入れ替え戦かの表示
            if (leagueMatchType != null)
            {
                leagueMatchType.text = boardInfo.todayMatch.matchingType == 1 ? StringValueAssetLoader.Instance["league.type.season"] : StringValueAssetLoader.Instance["league.type.shift"];
            }
            
            // クラブ名
            myClubName.text = boardInfo.groupStatusDetailSelf.name;
            
            // 初日
            if(boardInfo.groupStatusDetailSelf.winCount == 0 && boardInfo.groupStatusDetailSelf.loseCount == 0 && boardInfo.groupStatusDetailSelf.drawCount == 0)
            {
                // ランキング
                currentRank.text = StringValueAssetLoader.Instance["league.match.first_day_ranking"];
            }
            // 2日目以降
            else
            {
                currentRank.text = string.Format(StringValueAssetLoader.Instance["league.match_result.ranking"], boardInfo.groupStatusDetailSelf.ranking);
            }
            
            currentResult.text = string.Format( StringValueAssetLoader.Instance["league.match_result.point_result_detail"], boardInfo.groupStatusDetailSelf.winCount, boardInfo.groupStatusDetailSelf.loseCount, boardInfo.groupStatusDetailSelf.drawCount);
            
            // 入れ替え戦ラベル
            if(seasonEndResultLabelImage != null)
            {
                // 簡易トーナメントの場合は非表示
                if(seasonData.IsInstantTournament)
                {
                    seasonEndResultLabelImage.gameObject.SetActive(false);
                }
                else
                {
                    // 対戦表画面から結果画面に遷移できるようになったタイミングで表示するようにする
                    // その日の最終試合から一定時間後の時間
                    DateTime aggregatingEndTimeOfToday = leagueMatchInfo.GetAggregatingEndTimeOfToday();
                    // シーズン戦最終日かつ最終試合から一定時間経過後の場合は入れ替え戦ラベルを表示
                    if(DateTimeExtensions.IsSameDay(aggregatingEndTimeOfToday, leagueMatchInfo.SeasonBattleEndAt) && aggregatingEndTimeOfToday.IsPast(AppTime.Now))
                    {
                        SeasonEndResultType seasonEndResultType = LeagueMatchUtility.GetSeasonResult(leagueMatchInfo);
                        await seasonEndResultLabelImage.SetTextureAsync(seasonEndResultType);
                        seasonEndResultLabelImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        seasonEndResultLabelImage.gameObject.SetActive(false);
                    }
                }
            }
        }

        public async UniTask SetupForSeasonEnd(ColosseumSeasonData seasonData, GroupLeagueMatchGroupStatusDetail groupStatusDetailSelf, LeagueMatchInfo leagueMatchInfo)
        {
            if (rankIcon != null)
            {
                // 簡易大会の場合はランク画像を固定化
                rankIconDummy.gameObject.SetActive(seasonData.IsInstantTournament);
                // 簡易大会でなければランク表示
                rankIcon.gameObject.SetActive(!seasonData.IsInstantTournament);
                if(!seasonData.IsInstantTournament)
                {
                    // 最低ランク
                    long gradeNumber = ColosseumManager.MinGradeNumber;
                    // gradeNumber更新
                    if(seasonData != null)
                    {
                        gradeNumber = UserDataManager.Instance.ColosseumGradeData.GetGradeNumber(seasonData.MColosseumEvent.mColosseumGradeGroupId, seasonData.UserSeasonStatus.groupSeasonStatus.groupId);
                    }
                    // 画像セット
                    await rankIcon.SetTextureAsync(gradeNumber);
                }
            }
            
            await clubIcon.SetTextureAsync(groupStatusDetailSelf.mGuildEmblemId);
            leagueMatchType.text = StringValueAssetLoader.Instance["league.match.season_result_report"];
            myClubName.text = groupStatusDetailSelf.name;
            currentRank.text = string.Format(StringValueAssetLoader.Instance["league.match_result.ranking"], groupStatusDetailSelf.ranking);
            currentResult.text = string.Format( StringValueAssetLoader.Instance["league.match_result.point_result_detail"], groupStatusDetailSelf.winCount, groupStatusDetailSelf.loseCount, groupStatusDetailSelf.drawCount);
                    
            // 入れ替え戦ラベル
            if(seasonEndResultLabelImage != null)
            {
                // 簡易トーナメントの場合は非表示
                if(seasonData.IsInstantTournament)
                {
                    seasonEndResultLabelImage.gameObject.SetActive(false);
                }
                else
                {
                    SeasonEndResultType seasonEndResultType = LeagueMatchUtility.GetSeasonResult(leagueMatchInfo);
                    await seasonEndResultLabelImage.SetTextureAsync(seasonEndResultType);
                    seasonEndResultLabelImage.gameObject.SetActive(true);
                }
            }
        }
    }
}