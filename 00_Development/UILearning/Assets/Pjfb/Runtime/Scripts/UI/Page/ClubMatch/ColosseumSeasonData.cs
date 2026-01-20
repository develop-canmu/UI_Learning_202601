using System;
using System.Collections.Generic;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Colosseum
{
    public class ColosseumSeasonData
    {
        /// <summary>シーズンID</summary>
        public long SeasonId { get; set; } = 0;
        /// <summary>mColosseumEventId</summary>
        public long MColosseumEventId { get; set; } = 0;
        /// <summary>シーズン情報</summary>
        public ColosseumSeasonHome SeasonHome { get; set; } = null;
        /// <summary>ユーザー過去戦績</summary>
        public ColosseumUserSeasonStatus UnreadUserSeasonStatus { get; set; } = null;
        /// <summary>ユーザー戦績</summary>
        public ColosseumUserSeasonStatus UserSeasonStatus { get; set; } = null;

        public ColosseumRankingGroup[] BattleRankingGroups { get; set; }
        public ColosseumScoreBattleTurn ScoreBattleTurn { get; set; }
        
        private ColosseumEventMasterObject mColosseumEvent = null;
        /// <summary>マスタデータ</summary>
        public ColosseumEventMasterObject MColosseumEvent
        {
            get 
            {
                if(mColosseumEvent == null && MColosseumEventId != 0)
                {
                    mColosseumEvent = MasterManager.Instance.colosseumEventMaster.FindData(MColosseumEventId); 
                }
                return mColosseumEvent;
            }
        }
        
        /// <summary>シーズン中かどうか</summary>
        public bool IsOnSeason
        {
            get 
            { 
                if(SeasonHome == null) return false;
                if(SeasonHome.startAt.TryConvertToDateTime().IsFuture(AppTime.Now)) return false;
                if(SeasonHome.endAt.TryConvertToDateTime().IsPast(AppTime.Now)) return false;
                return true;
            }
        }
        
        /// <summary>handlingTypeが一致しているか</summary>
        public bool IsHandlingType(ColosseumClientHandlingType type)
        {
            return MColosseumEvent?.clientHandlingType == type;
        }
        
        /// <summary>クラブマッチかどうか</summary>
        public bool IsClubMatch
        {
            get { return MColosseumEvent?.clientHandlingType == ColosseumClientHandlingType.ClubMatch; }
        }
        
        /// <summary>ランクマッチかどうか</summary>
        public bool IsRankMatch
        {
            get { return MColosseumEvent?.clientHandlingType == ColosseumClientHandlingType.PvP; }
        }
        
        /// <summary>リーグマッチかどうか</summary>
        public bool IsLeagueMatch
        {
            get { return MColosseumEvent?.clientHandlingType == ColosseumClientHandlingType.LeagueMatch; }
        }
        
        /// <summary>簡易大会かどうか</summary>
        public bool IsInstantTournament
        {
            get { return MColosseumEvent?.clientHandlingType == ColosseumClientHandlingType.InstantTournament; }
        }

        /// <summary>クラブロワイヤルかどうか</summary>
        public bool IsClubRoyal
        {
            get { return MColosseumEvent?.clientHandlingType == ColosseumClientHandlingType.ClubRoyal; }
        }
        
        /// <summary>マッチングに失敗した</summary>
        public bool IsMatchingFailed
        {
            get
            {
                if(UserSeasonStatus == null) return true;
                if(UserSeasonStatus.groupSeasonStatus == null) return true;
                return false;
            }
        }
    }
}