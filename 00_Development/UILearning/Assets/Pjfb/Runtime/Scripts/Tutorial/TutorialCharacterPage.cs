using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb
{

    public class TutorialCharacterPage : CharacterPage
    {
        protected override string GetAddress(CharacterPageType page)
        {
            return $"Prefabs/UI/Page/TutorialCharacter/{page}Page.prefab";
        }
        
        /// <summary>事前準備</summary>
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialCharacter);
            await base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialCharacter);
            base.OnClosed();
        }
    }
}