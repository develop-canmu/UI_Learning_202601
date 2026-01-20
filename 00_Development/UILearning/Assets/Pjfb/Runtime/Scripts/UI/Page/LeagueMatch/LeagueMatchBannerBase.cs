using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.LeagueMatch
{
    public abstract class LeagueMatchBannerBase : MonoBehaviour
    {
        protected LeagueMatchInfo leagueMatchInfo = null;
        
        /// <summary>クラブに参加済み</summary>
        protected bool IsJoinedClub
        {
            get { return UserDataManager.Instance.user.gMasterId != 0; }
        }
        
        /// <summary>マッチングに失敗</summary>
        protected virtual bool IsMatchingFailed
        {
            get
            {
                if(leagueMatchInfo == null) return true;
                if(leagueMatchInfo.SeasonData == null) return true;
                if(leagueMatchInfo.SeasonData.IsMatchingFailed) return true;
                return false;
            }
        }
        
        /// <summary>シーズン中</summary>
        protected bool IsOnSeason
        {
            get
            {
                if(leagueMatchInfo == null) return false;
                return leagueMatchInfo.IsOnSeason;
            }
        }
        
        /// <summary>シーズン戦中</summary>
        protected bool IsOnSeasonBattle
        {
            get
            {
                if(leagueMatchInfo == null) return false;
                return leagueMatchInfo.IsOnSeasonBattle;
            }
        }
        
        /// <summary>入れ替え戦中</summary>
        protected bool IsOnShiftBattle
        {
            get
            {
                if(leagueMatchInfo == null) return false;
                return leagueMatchInfo.IsOnShiftBattle;
            }
        }

        //// <summary> バナーの表示のセット </summary>
        public abstract void SetView(LeagueMatchInfo leagueMatchInfo);

        private void Update()
        {
            UpdateView();
        }

        //// <summary> 時間経過によるバナー表示の更新 </summary>
        protected virtual void UpdateView()
        {
            
        }
    }
}