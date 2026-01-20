using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Unity.VisualScripting;

namespace Pjfb.Colosseum
{

    public enum ColosseumPageType
    {
        ColosseumTop,
        ColosseumMatching
    }

    public class ColosseumPage : PageManager<ColosseumPageType>
    {        
        protected override string GetAddress(ColosseumPageType page)
        {
            return $"Prefabs/UI/Page/Colosseum/{page}Page.prefab";
        }

        public static void OpenPage(bool stack, object args = null)
        {
            if (!ColosseumManager.IsUnLockColosseum())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(ColosseumManager.ColosseumLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData( StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            
            var master = MasterManager.Instance.colosseumEventMaster.GetAvailableColosseumMaster();
            if (master == null)
            {
                var title = StringValueAssetLoader.Instance["pvp.rankmatch"];
                var body = StringValueAssetLoader.Instance["pvp.period.error"];

                ConfirmModalWindow.Open(new ConfirmModalData(
                    title, body,
                    string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => window.Close())));
                return;
            }
            
            var pageType = PageType.Colosseum;
            if (AppManager.Instance.TutorialManager.OpenScenarioTutorialWithUnlockSystem(pageType, stack, args, SystemUnlockDataManager.SystemUnlockNumber.Pvp)) return;
            AppManager.Instance.UIManager.PageManager.OpenPage(pageType, stack, args);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            
            var master = MasterManager.Instance.colosseumEventMaster.GetAvailableColosseumMaster();
            if (master == null)
            {
                return;
            }
            
            var seasonHome = UserDataManager.Instance.GetAvailableSeasonHome(master.id);
            var sColosseumEventId = seasonHome?.id ?? 0;
            var colosseumState = ColosseumManager.GetColosseumState(seasonHome);

            if (seasonHome != null)
            {
                // 集計中以外ならステータス取得
                await ColosseumManager.GetUserSeasonStatusAsync(sColosseumEventId);
            }
            else
            {
                // seasonHomeが存在しないのでローカルでキャッシュしたseasonStatusがあれば破棄
                UserDataManager.Instance.ResetColosseumUserSeasonStatus(master.id);
            }

            var colosseumSeasonData = UserDataManager.Instance.GetColosseumSeasonData(sColosseumEventId);
            
            var firstView = ColosseumPageType.ColosseumTop;
            if (args != null)
            {
                firstView = (ColosseumPageType)args;
            }
            // 最初のページを開く
            switch (firstView)
            {
                case ColosseumPageType.ColosseumTop:
                    await OpenPageAsync(ColosseumPageType.ColosseumTop, true, new ColosseumTopPage.Data(colosseumSeasonData,master,colosseumState));
                    break;
                case ColosseumPageType.ColosseumMatching:
                    await OpenPageAsync(ColosseumPageType.ColosseumMatching, false, new ColosseumMatchingPage.Data(colosseumSeasonData));
                    break;
            }

        }
    }
}