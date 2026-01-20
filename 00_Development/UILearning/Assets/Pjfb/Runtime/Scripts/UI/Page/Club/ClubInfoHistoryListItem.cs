using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;
using CruFramework.UI;
using System;
using System.Text;
using Pjfb.Master;

namespace Pjfb.Club
{
    public class ClubInfoHistoryListItem : ScrollGridItem {
        
        public enum ShiftMatchProgressType
        {
            NotMatching  = -1,
            Matching     = 0,
            Win          = 1,
            Lose         = 2,
            Draw         = 3,
        }
        
        private enum RankUpType
        {
            /// <summary>昇格</summary>
            Up,
            /// <summary>降格</summary>
            Down,
            /// <summary>残留</summary>
            Stay
        }
        
        public class Param {
            public string startAt;   
            public string endAt;
            public string name;
            public long rank;
            public long gradeBefore;
            public long gradeAfter;
            public ShiftMatchProgressType shiftMatchProgress;
            public long eventId;
            // 順位表示の際にランク変動を考慮したテキスト表示にするか
            public bool isRankChangeConsidered;
        }

        [SerializeField]
        private TextMeshProUGUI _dateText = null;
        [SerializeField]
        private TextMeshProUGUI _nameText = null;
        [SerializeField]
        private TextMeshProUGUI _rankText = null;

        protected override void OnSetView(object value){
            var param = value as Param;
            
            // 大会名のセット
            SetNameText(param.name);
            
            // 開催期間のセット
            SetDateText(param.startAt, param.endAt);
            
            // ランクのセット
            SetRankText(param.rank, param.gradeBefore, param.gradeAfter, param.shiftMatchProgress, param.isRankChangeConsidered);
        }

        /// <summary>大会名をセットする</summary>
        private void SetNameText(string tournamentName)
        {
            _nameText.text = tournamentName;
        }
        
        public void SetDateText(string startAtStr, string endAtStr)
        {
            DateTime startAt = DateTime.Parse(startAtStr);
            DateTime endAt = DateTime.Parse(endAtStr);
            _dateText.text = string.Format( StringValueAssetLoader.Instance["club.history_list_open_at"], startAt.Year, startAt.Month, startAt.Day, endAt.Year, endAt.Month, endAt.Day );
        }

        /// <summary>ランクのセット</summary>
        private void SetRankText(long rank, long gradeBefore, long gradeAfter, ShiftMatchProgressType shiftMatchProgress, bool isRankChangeConsidered)
        {
            // ランク変動を考慮しない場合はそのまま表示
            if (!isRankChangeConsidered)
            {
                _rankText.text = rank.ToString();
                return;
            }
            
            // ランク変動を考慮したテキスト表示
            StringBuilder rankTextBuilder = new StringBuilder();
            rankTextBuilder.AppendLine( string.Format(StringValueAssetLoader.Instance["league.match.season_battle_ranking"], rank) );
            
            switch(shiftMatchProgress)
            {
                case ShiftMatchProgressType.Win:
                {
                    rankTextBuilder.Append(StringValueAssetLoader.Instance["league.match.win"]);
                    break;
                }
                case ShiftMatchProgressType.Lose:
                {
                    rankTextBuilder.Append(StringValueAssetLoader.Instance["league.match.lose"]);
                    break;
                }
                case ShiftMatchProgressType.Draw:
                {
                    rankTextBuilder.Append(StringValueAssetLoader.Instance["league.match.draw"]);
                    break;
                }
            }
            
            // ランク変動
            
            // 昇格
            if(gradeBefore < gradeAfter)
            {
                rankTextBuilder.Append(StringValueAssetLoader.Instance["league.match.rank_up"]);
            }
            // 降格
            else if(gradeBefore > gradeAfter)
            {
                rankTextBuilder.Append(StringValueAssetLoader.Instance["league.match.rank_down"]);
            }
            // 残留
            else
            {
                rankTextBuilder.Append(StringValueAssetLoader.Instance["league.match.rank_stay"]);
            }
            
            _rankText.text = rankTextBuilder.ToString();
        }
    }
}