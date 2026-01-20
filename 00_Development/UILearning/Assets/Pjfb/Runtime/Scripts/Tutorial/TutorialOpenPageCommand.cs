using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb
{
    public class TutorialOpenPageCommand : TutorialCommandSetting.TutorialCommandData
    {
        [SerializeField] private string pageName;
        public string PageName => pageName;
        
        public override async UniTask ActionCommand(CancellationToken token)
        {
            await OpenPage(token);
        }
        
        // 指定されたページを開く処理
        private async UniTask OpenPage(CancellationToken token)
        {
            await CloseModal();
            switch (pageName)
            {
                case "BaseCharaGrowthLiberationList":
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Character, true, new CharacterPage.Data(CharacterPageType.BaseCharaGrowthLiberationList),token);
                    break;
                case "BaseCharaTop":
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Character, true, new CharacterPage.Data(CharacterPageType.BaseCharaTop),token);
                    break;
            }
        }
        
                
        // 開いているモーダルを閉じる
        private async UniTask CloseModal()
        {
            var modalManager = AppManager.Instance.UIManager.ModalManager;
            modalManager.RemoveTopModalsIgnoreTop(_ => true);
            var modalWindow = modalManager.GetTopModalWindow();
            if (modalWindow != null) await modalWindow.CloseAsync();
        }
        
    }
}