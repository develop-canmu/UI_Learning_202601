using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.InGame
{
    public class InGameStatsListModal : ModalWindow
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private InGameSimpleStatUI playerOriginalStatUI;
        [SerializeField] private InGameSimpleStatUI enemyOriginalStatUI;

        private readonly List<InGameSimpleStatUI> playerStatUis = new List<InGameSimpleStatUI>();
        private readonly List<InGameSimpleStatUI> enemyStatUis = new List<InGameSimpleStatUI>();

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetData();
            SetUserName();
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnAwake()
        {
            playerOriginalStatUI.gameObject.SetActive(false);
            enemyOriginalStatUI.gameObject.SetActive(false);

            base.OnAwake();
        }

        private void SetData()
        {
            var lists = BattleDataMediator.Instance.Decks;
            if (lists.Count != 2)
            {
                return;
            }
            
            var playerCharacters = lists[(int)BattleDataMediator.Instance.PlayerSide];
            var enemyCharacters = lists[(int)BattleDataMediator.Instance.EnemySide];
            if (playerCharacters.Count != enemyCharacters.Count)
            {
                return;
            }

            var orderedPlayerCharacters = playerCharacters.OrderByDescending(chara => chara.Stats.ActivityPoint).ToArray();
            var orderedEnemyCharacters = enemyCharacters.OrderByDescending(chara => chara.Stats.ActivityPoint).ToArray();
            
            InstantiateStatUI(playerCharacters.Count);
            for (var i = 0; i < playerCharacters.Count; i++)
            {
                playerStatUis[i].SetData(orderedPlayerCharacters[i]);
                enemyStatUis[i].SetData(orderedEnemyCharacters[i]);
            }
        }

        private void SetUserName()
        {
            var playerList = BattleDataMediator.Instance.Players;
            if (playerList.Count != 2)
            {
                return;
            }

            playerNameText.text = playerList[(int)BattleDataMediator.Instance.PlayerSide].Name;
            enemyNameText.text = playerList[(int)BattleDataMediator.Instance.EnemySide].Name;
        }

        private void InstantiateStatUI(int characterCount)
        {
            // 敵味方は同数前提.
            if (playerStatUis.Count >= characterCount)
            {
                return;
            }
            
            for (var i = 0; i < characterCount; i++)
            {
                var playerStatInstance = Instantiate(playerOriginalStatUI, playerOriginalStatUI.transform.parent, false);
                var enemyStatInstance = Instantiate(enemyOriginalStatUI, enemyOriginalStatUI.transform.parent, false);
                
                playerStatUis.Add(playerStatInstance);
                enemyStatUis.Add(enemyStatInstance);
            }
        }
    }
}