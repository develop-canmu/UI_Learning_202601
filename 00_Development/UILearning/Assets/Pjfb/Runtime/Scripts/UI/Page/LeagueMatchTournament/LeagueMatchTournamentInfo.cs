using System;
using Pjfb.Colosseum;
using Pjfb.LeagueMatch;
using Pjfb.Master;

namespace Pjfb.LeagueMatchTournament
{
    // 大会の分類データ
    public class LeagueMatchTournamentInfo : LeagueMatchInfo
    {
        // バナーステータス
        private LeagueMatchTournamentBanner.Status bannerStatus;
        public LeagueMatchTournamentBanner.Status BannerStatus => bannerStatus;
        
        // 分類グループ
        private LeagueMatchTournamentManager.BannerGroupType groupType;
        public LeagueMatchTournamentManager.BannerGroupType GroupType => groupType;

        private DateTime updateTime = DateTime.MaxValue;
        // 次の更新時間
        public DateTime UpdateTime => updateTime;
        
        public LeagueMatchTournamentInfo(ColosseumEventMasterObject mColosseumEvent) : base(mColosseumEvent)
        {
            UpdateStatus();
        }

        public LeagueMatchTournamentInfo(ColosseumSeasonData seasonData) : base(seasonData)
        {
            UpdateStatus();
        }
        
        // ステータスの更新
        public void UpdateStatus()
        {
            bannerStatus = LeagueMatchTournamentManager.GetBannerStatus(this);
            groupType = LeagueMatchTournamentManager.GetBannerGroupType(bannerStatus);
            updateTime = LeagueMatchTournamentManager.GetNextUpdateTime(this);
        }
    }
}