using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameWinStreakUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text[] playerNameText;
        [SerializeField] private Sprite[] allyWinCountSprites;
        [SerializeField] private Sprite[] enemyWinCountSprites;
        [SerializeField] private Image[] allyWinCountImages;
        [SerializeField] private Image[] enemyWinCountImages;
        [SerializeField] private CharacterCardImage[] characterCardImages;

        private const string AllyTrigger = "ConsecutiveVictoriesOwn";
        private const string EnemyTrigger = "ConsecutiveVictoriesEnemy";
        
        private readonly Queue<GuildBattlePartyModel> queuedParties = new Queue<GuildBattlePartyModel>();
        
        public void PlayEffect(GuildBattleFieldSituationModel fieldSituation)
        {
            foreach (var partyIds in fieldSituation.EachLaneMatchingPartyIds)
            {
                foreach (var partyId in partyIds)
                {
                    var party = fieldSituation.PjfbParties.FirstOrDefault(party => party.PartyId == partyId);
                    if (party == null || !party.IsOnMap())
                    {
                        continue;
                    }

                    if (!party.IsAchieveCutInCommendedWinStreakCount())
                    {
                        continue;
                    }
                
                    queuedParties.Enqueue(party);
                }
            }

            if (queuedParties.Count > 0)
            {
                var party = queuedParties.Dequeue();
                PlayWinStreakUI(party);
            }
        }

        private void PlayWinStreakUI(GuildBattlePartyModel partyModel)
        {
            gameObject.SetActive(true);
            var isAlly = PjfbGuildBattleDataMediator.Instance.PlayerSide == partyModel.Side;
            var player = PjfbGuildBattleDataMediator.Instance.GetBattlePlayer(partyModel.PlayerId);

            var winStreakCountFirstDigit = partyModel.WinStreakCount % 10;
            var winStreakCountSecondDigit = partyModel.WinStreakCount / 10;

            Image[] images;
            Sprite[] sprites;
            var index = 0;
            var triggerName = string.Empty;
            if (isAlly)
            {
                images = allyWinCountImages;
                sprites = allyWinCountSprites;
                index = (int)GuildBattleCommonConst.GuildBattleTeamSide.Left;
                triggerName = AllyTrigger;
            }
            else
            {
                images = enemyWinCountImages;
                sprites = enemyWinCountSprites;
                index = (int)GuildBattleCommonConst.GuildBattleTeamSide.Right;
                triggerName = EnemyTrigger;
            }
            playerNameText[index].text = player.Name;
            characterCardImages[index].SetTextureAsync(partyModel.GetLeaderCharacterData()?.mCharaId ?? -1);

            var over10 = winStreakCountSecondDigit > 0;
            images[0].sprite = sprites[winStreakCountFirstDigit];
            images[1].gameObject.SetActive(over10);
            if (over10)
            {
                images[1].sprite = sprites[winStreakCountSecondDigit];
            }
            
            animator.SetTrigger(triggerName);
        }

        // Call by animation event.
        private void OnEndCutIn()
        {
            if (queuedParties.Count > 0)
            {
                var party = queuedParties.Dequeue();
                PlayWinStreakUI(party);
                return;
            }
            else
            {
                // TODO Closeアニメーション
                gameObject.SetActive(false);
            }
        }
    }
}