using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;

namespace Pjfb.ClubMatch
{

    public enum ClubMatchPageType
    {
        ClubMatchTop,
        ClubMatchMatching,
    }
    
    public enum TurnStateEnum
    {
        OnTurn,
        NextTurn,
        ExitTurn
    }

    public class PageData
    {
        public ColosseumSeasonData SeasonData;
        public ClubMatchPageType targetPage;
        public bool isStackedPage = true;
        public PageType callerPage = PageType.Home;
    }

    public class ClubMatchPage : PageManager<ClubMatchPageType>
    {
        protected override string GetAddress(ClubMatchPageType page)
        {
            return $"Prefabs/UI/Page/ClubMatch/{page}Page.prefab";
        }

        public static void OpenPage(bool stack, object args = null)
        {
            if (AppManager.Instance.TutorialManager.OpenScenarioTutorial(PageType.ClubMatch, stack, args))
            {
                return;
            }
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubMatch, stack, args);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            var pageData = ((PageData)args);
            await OpenPageAsync(pageData.targetPage, true, args);   
        }
        
        public static void ShowErrorDialog(Action cb, string msgTitleKey = "common.error", string msgBodyKey = "common.error")
        {
            CruFramework.Logger.LogError(StringValueAssetLoader.Instance[msgBodyKey]);
            ConfirmModalData modalData = new ConfirmModalData
            {
                Title = StringValueAssetLoader.Instance[msgTitleKey],
                Message = StringValueAssetLoader.Instance[msgBodyKey],
                PositiveButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.ok"],
                    window =>
                    {
                        window.Close();
                        cb.Invoke();
                    }
                )
            };
            AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, modalData);
        }

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("Pjfb/Debug/GoToClubMatchTop")]
        public static void GoToClubMatchTop()
        {
            var cData = UserDataManager.Instance.GetColosseumSeasonData(175);
            // クラブマッチトップへ遷移
            OpenPage(true,new ClubMatchTopPage.Data(cData){callerPage = PageType.Home});
        }
        #endif
    }
}