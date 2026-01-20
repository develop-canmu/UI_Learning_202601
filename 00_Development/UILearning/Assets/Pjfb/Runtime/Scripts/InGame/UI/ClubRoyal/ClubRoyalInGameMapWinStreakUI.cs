using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameMapWinStreakUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Image[] countImages;
        [SerializeField] private Image countUnitImage;
        [SerializeField] private Sprite[] allyCountSprites;
        [SerializeField] private Sprite[] enemyCountSprites;
        [SerializeField] private Sprite[] countUnitSprites;
        
        private const string CountUpTrigger = "OpenConsecutiveVictories";
        private const string CloseTrigger = "Close";
        private readonly int IdleStateHash = Animator.StringToHash("Base Layer.Idle");
        
        public bool IsActive { get; private set; }

        public void PlayAnimation(int count, bool isAlly, bool immediately = false)
        {
            IsActive = true;
            gameObject.SetActive(true);

            if (immediately)
            {
                animator.Play(IdleStateHash, 0, 1.0f);
            }
            else
            {
                animator.SetTrigger(CountUpTrigger);                
            }

            var digit1 = count % 10;
            var digit10 = count / 10;
            
            countImages[1].sprite = isAlly ? allyCountSprites[digit1] : enemyCountSprites[digit1];
            countImages[0].enabled = digit10 > 0;
            if (digit10 > 0)
            {
                countImages[0].sprite = isAlly ? allyCountSprites[digit10] : enemyCountSprites[digit10];
            }
            
            countUnitImage.sprite = countUnitSprites[isAlly ? 0 : 1];
        }

        public void Close(bool immediately = false)
        {
            IsActive = false;
            if (immediately)
            {
                gameObject.SetActive(false);
                return;
            }
            
            animator.SetTrigger(CloseTrigger);
        }
    }
}