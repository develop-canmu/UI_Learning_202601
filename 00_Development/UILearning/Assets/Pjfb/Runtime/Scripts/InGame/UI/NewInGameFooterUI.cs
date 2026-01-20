using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Adv;

namespace Pjfb.InGame
{
    public class NewInGameFooterUI : MonoBehaviour
    {
        [SerializeField] private GameObject autoSwipeButtonRoot;
        [SerializeField] private UIButton autoSwipeOffButton;
        [SerializeField] private UIButton autoSwipeOnButton;
        [SerializeField] private UIButton autoOffButton;
        [SerializeField] private UIButton autoOnButton;
        [SerializeField] private UIButton skipToFinishButton;
        [SerializeField] private UIButton skipReplayButton;
        [SerializeField] private UIButton normalSpeedButton;
        [SerializeField] private UIButton doubleSpeedButton;

        private void Awake()
        {
            autoSwipeOffButton.OnClickEx.AddListener(OnClickAutoSwipeOff);
            autoSwipeOnButton.OnClickEx.AddListener(OnClickAutoSwipeOn);
            autoOffButton.OnClickEx.AddListener(OnClickAutoOff);
            autoOnButton.OnClickEx.AddListener(OnClickAutoOn);
            skipToFinishButton.OnClickEx.AddListener(OnClickSkipToFinish);
            skipReplayButton.OnClickEx.AddListener(OnClickSkipToFinish);
            normalSpeedButton.OnClickEx.AddListener(OnClickNormalSpeed);
            doubleSpeedButton.OnClickEx.AddListener(OnClickDoubleSpeed);

            skipReplayButton.gameObject.SetActive(false);
            autoSwipeOnButton.gameObject.SetActive(false);
            autoOnButton.gameObject.SetActive(false);
            doubleSpeedButton.gameObject.SetActive(false);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetAsReplayMode()
        {
            SetVisible(true);

            skipReplayButton.gameObject.SetActive(true);
            skipToFinishButton.gameObject.SetActive(false);
            autoOffButton.gameObject.SetActive(false);
            autoOnButton.gameObject.SetActive(false);
            normalSpeedButton.gameObject.SetActive(false);
            doubleSpeedButton.gameObject.SetActive(false);
            HideAutoSwipeButton();
        }

        public void HideAutoSwipeButton()
        {
            autoSwipeButtonRoot.SetActive(false);
        }

        // ↓4つ 「AutoOffの表示になっているボタンを」なので反対になる
        private void OnClickAutoOff()
        {
            BattleDataMediator.Instance.IsSpeedUpPlayerDigest = true;
            autoOnButton.gameObject.SetActive(true);
            autoOffButton.gameObject.SetActive(false);
        }

        private void OnClickAutoOn()
        {
            BattleDataMediator.Instance.IsSpeedUpPlayerDigest = false;
            autoOnButton.gameObject.SetActive(false);
            autoOffButton.gameObject.SetActive(true);
        }
        
        private void OnClickAutoSwipeOff()
        {
            BattleDataMediator.Instance.IsAutoSwipe = true;
            autoSwipeOnButton.gameObject.SetActive(true);
            autoSwipeOffButton.gameObject.SetActive(false);
        }

        private void OnClickAutoSwipeOn()
        {
            BattleDataMediator.Instance.IsAutoSwipe = false;
            autoSwipeOnButton.gameObject.SetActive(false);
            autoSwipeOffButton.gameObject.SetActive(true);
        }
        
        private void OnClickNormalSpeed()
        {
            BattleDataMediator.Instance.IsDoubleSpeed = true;
            normalSpeedButton.gameObject.SetActive(false);
            doubleSpeedButton.gameObject.SetActive(true);
        }
        
        private void OnClickDoubleSpeed()
        {
            BattleDataMediator.Instance.IsDoubleSpeed = false;
            normalSpeedButton.gameObject.SetActive(true);
            doubleSpeedButton.gameObject.SetActive(false);
        }

        private void OnClickSkipToFinish()
        {
            BattleEventDispatcher.Instance.OnClickFooterSkipButtonCallback();
        }
    }
}