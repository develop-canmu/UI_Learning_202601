using System;
using UnityEngine;

namespace Pjfb.InGame
{
    public class NewInGameDigestUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup rootCanvasGroup;
        [SerializeField] private NewInGameDialogueUI dialogueUI;

        private void Awake()
        {
            SetDigestUiActive(false);
        }

        public void SetDigestUiActive(bool isActive)
        {
            rootCanvasGroup.alpha = isActive ? 1 : 0;
        }

    }
}