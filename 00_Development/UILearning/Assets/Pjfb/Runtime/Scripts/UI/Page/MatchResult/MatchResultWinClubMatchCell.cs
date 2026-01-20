using System;
using UnityEngine;
using TMPro;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.ClubMatch;
using Pjfb.Extensions;

namespace Pjfb.MatchResult
{
    public class MatchResultWinClubMatchCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text beforeRankingText;
        [SerializeField] private TMP_Text afterRankingText;
        [SerializeField] private TMP_Text beforeScoreText;
        [SerializeField] private TMP_Text afterScoreText;
        [SerializeField] private TMP_Text afterBonusText;
        [SerializeField] private GameObject periodicScoreAcquisitionObject;

        public void SetView(string name, BattleChangeResultPlayer result)
        {
            // ランキング順位
            beforeRankingText.text = result.before.ranking.ToString();
            afterRankingText.text = result.after.ranking.ToString();
            // 順位スコア
            var beforeScore = result.before.scorePlanned;
            beforeScoreText.text = beforeScore.GetStringNumberWithComma();
            var afterScore = result.after.scorePlanned;
            afterScoreText.text =  afterScore.GetStringNumberWithComma();
            // ボーナス合計
            string percentText = StringValueAssetLoader.Instance["clubmatch.result.bonus"];
            var afterBonusRate = result.after.bonusRate / 100.0;
            afterBonusText.text = string.Format(percentText,Math.Round(afterBonusRate, MidpointRounding.AwayFromZero));
            // スコアアップ親オブジェクト
            periodicScoreAcquisitionObject.SetActive(afterScore > beforeScore);
        }
    }
}