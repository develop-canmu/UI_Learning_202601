using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Pjfb.Adv;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Pjfb
{
    public class AdvDebugPage : Page
    {
    
        [SerializeField]
        private GameObject menuRoot = null;
        [SerializeField]
        private AppAdvManager advManager = null;
        
        [SerializeField]
        private TMP_InputField scenarioIdField = null;
        
        [SerializeField]
        private TMP_Text messageText = null;
        
        
        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Footer.Hide();
            AppManager.Instance.UIManager.Header.Hide();
            
            advManager.OnEnded -= OnEndAdv;
            advManager.OnEnded += OnEndAdv;
            
#if CRUFRAMEWORK_DEBUG || UNITY_EDITOR
            // デバッグモード
            advManager.IsDebugMode = true;
#endif
            
            advManager.PlayOpenAnimationAsync().Forget();
        }
        
        private void OnEndAdv()
        {
            menuRoot.SetActive(true);
            advManager.PlayOpenAnimationAsync().Forget();
        }
        
        public void OnPlayScenarioButton()
        {
            OnPlayScenarioButtonAsync().Forget();
        }
        
        public async UniTask OnPlayScenarioButtonAsync()
        {
            try
            {
                if(advManager.IsPlaying)
                {
                    advManager.End();
                }
                await advManager.LoadAdvFile(ResourcePathManager.GetPath("AdvCommand", scenarioIdField.text) );
                menuRoot.SetActive(false);
            }
            catch
            {
                messageText.text = scenarioIdField.text + "は再生できませんでした";
                
            }
        }
        
        public void OnHomeButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
        }
        
        public void OnMenuButton()
        {
            menuRoot.SetActive(!menuRoot.activeSelf);
        }
    }
}