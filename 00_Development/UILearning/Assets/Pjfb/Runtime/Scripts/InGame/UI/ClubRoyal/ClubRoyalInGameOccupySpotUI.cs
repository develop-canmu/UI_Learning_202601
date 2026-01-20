using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameOccupySpotUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private Queue<GuildBattleCommonConst.GuildBattleTeamSide> queuedSide = new Queue<GuildBattleCommonConst.GuildBattleTeamSide>();
        private const string allyTrigger = "GoalBreakthroughOwn";
        private const string enemyTrigger = "GoalBreakthroughEnemy";
        
        public bool IsPlaying => isPlaying || queuedSide.Count > 0;
        private bool isPlaying;

        public void PlayAnimation(GuildBattleCommonConst.GuildBattleTeamSide side)
        {
            if (ClubRoyalInGameUIMediator.Instance.GoalCutInUI.IsPlaying || isPlaying)
            {
                queuedSide.Enqueue(side);
                return;
            }

            StartAnimation(side);
        }
        
        private void StartAnimation(GuildBattleCommonConst.GuildBattleTeamSide side)
        {
            isPlaying = true;
            gameObject.SetActive(true);
            var trigger = PjfbGuildBattleDataMediator.Instance.PlayerSide == side ? enemyTrigger : allyTrigger;
            animator.Play(trigger);
        }

        public void OnEndGoalCutIn()
        {
            if (queuedSide.Count > 0)
            {
                OnEndAnimation();
            }
        }

        // Call by animation event.
        private void OnEndAnimation()
        {
            isPlaying = false;
            if (queuedSide.Count > 0)
            {
                var side = queuedSide.Dequeue();
                StartAnimation(side);
                return;
            }
            
            gameObject.SetActive(false);
        }
    }
}