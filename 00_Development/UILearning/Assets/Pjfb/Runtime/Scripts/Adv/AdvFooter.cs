using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CruFramework.Adv;

namespace Pjfb.Adv
{
    public class AdvFooter : MonoBehaviour
    { 
        public  bool IsShow{get{return gameObject.activeInHierarchy;}}
        
        [SerializeField]
        private UIButton skipButton = null;
        [SerializeField]
        private UIButton autoModeButton = null;
        [SerializeField]
        private Image autoButtonImage = null;
        [SerializeField]
        private Sprite[] autoButtonSprites = null;
        
        public void SetAutoModeButton(AppAdvAutoMode mode)
        {
            autoButtonImage.sprite = autoButtonSprites[(int)mode];
        }
        
        public void EnableAutoButton(bool enable)
        {
            autoModeButton.interactable = enable;
        }
        
        public void EnableSkipButton(bool enable)
        {
            skipButton.interactable = enable;
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}