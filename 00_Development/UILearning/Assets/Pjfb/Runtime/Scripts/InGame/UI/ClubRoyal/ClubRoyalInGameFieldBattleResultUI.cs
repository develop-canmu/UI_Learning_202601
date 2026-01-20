using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameFieldBattleResultUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text allyScoreText;
        [SerializeField] private TMP_Text enemyScoreText;

        private const string OpenTrigger = "Open";
        private const string CloseTrigger = "Close";
        private const string PointAllyTrigger = "PointOwn";
        private const string PointEnemyTrigger = "PointEnemy";

        private int allyScore = 0;
        private int enemyScore = 0;
        
        public async UniTask PlayAnimation(int allyPoint, int enemyPoint, bool isAllyFirst, Vector3 worldPosition)
        {
            gameObject.transform.position = worldPosition;
            gameObject.SetActive(true);
            allyScoreText.text = "0";
            enemyScoreText.text = "0";
            allyScore = allyPoint;
            enemyScore = enemyPoint;
            animator.SetTrigger(OpenTrigger);

            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: destroyCancellationToken);
            animator.SetTrigger(isAllyFirst ? PointAllyTrigger : PointEnemyTrigger);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            animator.SetTrigger(isAllyFirst ? PointEnemyTrigger : PointAllyTrigger);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: destroyCancellationToken);
            animator.SetTrigger(CloseTrigger);
        }
        
        
        /// <summary>
        /// called by animation event.
        /// </summary>
        public void OnSetAllyPoint()
        {
            allyScoreText.text = allyScore.ToString();
        }
        
        /// <summary>
        /// called by animation event.
        /// </summary>
        public void OnSetEnemyPoint()
        {
            enemyScoreText.text = enemyScore.ToString();
        }

        /// <summary>
        /// called by animation event.
        /// </summary>
        public void OnEndClose()
        {
            gameObject.SetActive(false);
        }
    }
}