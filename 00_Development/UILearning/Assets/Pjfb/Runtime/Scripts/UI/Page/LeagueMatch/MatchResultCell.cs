using UnityEngine;
using Pjfb.LeagueMatch;

namespace Pjfb
{
    public class MatchResultCell : MonoBehaviour
    {
        [SerializeField] private RectTransform winObject;
        [SerializeField] private RectTransform loseObject;
        [SerializeField] private RectTransform drawObject;
        [SerializeField] private RectTransform blankObject;

        public void ShowResult(BattleResult result) // win: 1, lose : -1, draw:0
        {
            winObject.gameObject.SetActive(result == BattleResult.Win);
            loseObject.gameObject.SetActive(result == BattleResult.Lose);
            drawObject.gameObject.SetActive(result == BattleResult.Draw);
            blankObject.gameObject.SetActive(result == BattleResult.None);
        }
    }
}