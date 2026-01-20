using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameKickOffEffectUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void StartAnimation()
        {
            gameObject.SetActive(true);
            animator.SetTrigger("Open");
        }
    }
}