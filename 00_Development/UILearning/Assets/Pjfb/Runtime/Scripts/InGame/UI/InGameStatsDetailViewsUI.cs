using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.InGame
{
    public class InGameStatsDetailViewsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI activityPointText;
        [SerializeField] private TextMeshProUGUI goalCountText;
        [SerializeField] private TextMeshProUGUI goalAssistCountText;
        [SerializeField] private TextMeshProUGUI shootCountText;
        [SerializeField] private TextMeshProUGUI throughCountText;
        [SerializeField] private TextMeshProUGUI passCountText;
        [SerializeField] private TextMeshProUGUI crossCountText;
        [SerializeField] private TextMeshProUGUI shootBlockCountText;
        [SerializeField] private TextMeshProUGUI throughBlockCountText;
        [SerializeField] private TextMeshProUGUI passCutCountText;
        [SerializeField] private TextMeshProUGUI secondBallCountText;
        
        public void SetData(BattleCharacterModel characterModel)
        {
            var stats = characterModel.Stats;

            activityPointText.text = stats.ActivityPoint.ToString();
            goalCountText.text = stats.GoalCount.ToString();
            goalAssistCountText.text = stats.GoalAssistCount.ToString();
            shootCountText.text = $"{stats.ShootSucceedCount}/{stats.ShootSucceedCount + stats.ShootFailedCount}";
            throughCountText.text = $"{stats.ThroughSucceedCount}/{stats.ThroughSucceedCount + stats.ThroughFailedCount}";
            passCountText.text = $"{stats.PassSucceedCount}/{stats.PassSucceedCount + stats.PassFailedCount}";
            crossCountText.text = $"{stats.CrossSucceedCount}/{stats.CrossSucceedCount + stats.CrossFailedCount}";
            shootBlockCountText.text = $"{stats.ShootBlockSucceedCount}";
            throughBlockCountText.text = $"{stats.ThroughBlockSucceedCount}/{stats.ThroughBlockSucceedCount + stats.ThroughBlockFailedCount}";
            passCutCountText.text = $"{stats.PassCutSucceedCount}";
            secondBallCountText.text = $"{stats.SecondBallSucceedCount}/{stats.SecondBallSucceedCount + stats.SecondBallFailedCount}";
        }
    }
}