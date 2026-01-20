using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.InGame
{
    public class NewInGameMatchUpUi : MonoBehaviour
    {
        [SerializeField] private InGameMatchUpPhraseUI phraseUI;

        private const string OpenTriggerKey = "Open";
        
        public void OpenPhraseUI(bool hideSomePhrase)
        {
            phraseUI.Open(hideSomePhrase);
        }

        public void ClosePhraseUI()
        {
            phraseUI.Close();
        }
        
#if !PJFB_REL
        public void DebugOpenPhraseUI()
        {
            phraseUI.DebugOpen();
        }
        public void DebugClosePhraseUI()
        {
            phraseUI.DebugClose();
        }
#endif

    }
}