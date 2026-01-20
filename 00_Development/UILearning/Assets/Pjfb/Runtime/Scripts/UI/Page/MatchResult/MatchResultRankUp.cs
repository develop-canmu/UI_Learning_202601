using System;
using UnityEngine;
using TMPro;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.ClubMatch;

namespace Pjfb.MatchResult
{
    public class MatchResultRankUp : MonoBehaviour
    {
        [SerializeField] private GameObject middleLayerRoot;
        [SerializeField] private Animator pageAnimator;

        public void OpenNext()
        {
            middleLayerRoot.SetActive(true);
            pageAnimator.SetTrigger("Open");
        }
    }
}