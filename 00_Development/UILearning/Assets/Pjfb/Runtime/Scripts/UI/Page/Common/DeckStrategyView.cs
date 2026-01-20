using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class DeckStrategyView : MonoBehaviour
    {
        
        [SerializeField]
        private TMPro.TMP_Text strategyText = null;
        
        public void SetStrategy(BattleConst.DeckStrategy strategy)
        {
            switch(strategy)
            {
                case BattleConst.DeckStrategy.Aggressive:
                    strategyText.text = StringValueAssetLoader.Instance["deck.strategy.aggressive"];
                    break;
                case BattleConst.DeckStrategy.Dribble:
                    strategyText.text = StringValueAssetLoader.Instance["deck.strategy.dribble"];
                    break;
                case BattleConst.DeckStrategy.Pass:
                    strategyText.text = StringValueAssetLoader.Instance["deck.strategy.pass"];
                    break;
                
                default:
                    strategyText.text = "-";
                    break;
            }
        }
        
    }
}