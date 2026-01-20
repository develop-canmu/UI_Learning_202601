using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingMenuModal : ModalWindow
    {
    
        [SerializeField]
        private UIButton pauseButton = null;

        [SerializeField]
        private UIButton abortButton = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 退出不可のときはグレーアウト
            pauseButton.interactable = TrainingUtility.CanExit;
            abortButton.interactable = TrainingUtility.CanExit;
            
            SetCloseParameter(false);
            return base.OnPreOpen(args, token);
        }
        protected override void OnClosed()
        {
        }
        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnAbortButton()
        {
            TrainingUtility.OpenAbordModal(()=>SetCloseParameter(true));
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnMemberInfoButton()
        {
            TrainingMainArguments arguments = (TrainingMainArguments)ModalArguments;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingMemberInfo, arguments);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnPauseButton()
        {
            SetCloseParameter(true);
            Close();
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnOptionButton()
        {
            ConfigurationModalWindow.WindowParams p = new ConfigurationModalWindow.WindowParams();
            p.Type = ConfigurationModalWindow.SettingType.Training;
            TrainingMainArguments arguments = (TrainingMainArguments)ModalArguments;
            p.TrainingScenarioId = arguments.Pending.mTrainingScenarioId;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Configuration, p);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnHelpButton()
        {
            TrainingUtility.OpenHelpModal();
        }
    }
}