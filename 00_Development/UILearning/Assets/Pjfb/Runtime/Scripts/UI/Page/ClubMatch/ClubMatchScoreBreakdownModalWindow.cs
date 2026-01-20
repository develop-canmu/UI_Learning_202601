using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.ClubMatch
{
    public enum ScoreBaseType
    {
        Win = 1,
        Lose = 2,
        Draw = 3,
        BaseScore = 4
    }

    public class ClubMatchScoreBreakdownModalWindow : ModalWindow
    {
        #region Params

        enum BonusType
        {
            Stay = 1,
            Defense,
            Rank
        }

        public class WindowParams
        {
            public ColosseumSeasonData seasonData;
            public ColosseumRankingUser userSeasonStatus;
            public Action onClosed;
        }
        
        [SerializeField] private TextMeshProUGUI RankScoreText;
        [SerializeField] private TextMeshProUGUI RankBonusPercentText;
        [SerializeField] private TextMeshProUGUI TotalPercentText;
        [SerializeField] private TextMeshProUGUI TurnEndScoreText;
        
        private WindowParams _windowParams;
        private ColosseumGetScorePlanedDetailAPIResponse _response;
        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchScoreBreakdown, data);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            await base.OnPreOpen(args, token);
        }
        private void Init()
        {
            // 獲得予定スコア内訳データ取得
            var scoreData = ColosseumManager.GetExpectedScoreData(_windowParams.userSeasonStatus,
                _windowParams.seasonData.ScoreBattleTurn);

            string percentText = StringValueAssetLoader.Instance["common.percent_value"];
            
            // 順位スコア
            RankScoreText.text = scoreData.baseScore.ToString("N0");
            
            // 順位ボーナス
            var rankBonus = scoreData.rankBonusMultiplier / 100f;
            RankBonusPercentText.text = string.Format(percentText,Math.Round(rankBonus, MidpointRounding.AwayFromZero));
            // 合計
            var totalBonus = rankBonus;
            TotalPercentText.text = string.Format(percentText,Math.Round(totalBonus, MidpointRounding.AwayFromZero));
            
            // ターン終了時の獲得予定スコア
            TurnEndScoreText.text = scoreData.totalScore.ToString("N0");
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
    }
}