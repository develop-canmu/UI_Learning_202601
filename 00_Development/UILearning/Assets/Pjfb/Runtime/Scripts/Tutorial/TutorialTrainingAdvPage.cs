using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

using Pjfb.Adv;

namespace Pjfb
{
    public class TutorialTrainingAdvPage : TrainingAdvPage
    {
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            MainPageManager.Character.gameObject.SetActive(false);
            var m = Manager as TutorialTrainingMain;
            m.SetButtonInteractable(true);
            await base.OnPreOpen(args, token);
        }

        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            var m = Manager as TutorialTrainingMain;
            m.SetButtonInteractable(false);
            return base.OnPreClose(token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
        }

        protected override async UniTask AdvEndAsync()
        {
            var res = AppManager.Instance.TutorialManager.GetTrainingProgressData();
            
            // フェードイン
            if(Adv.Transition.State == AdvTransition.FadeState.FadeOut)
            {
                await Adv.Transition.FadeIn();
            }
            
            OpenPage(TrainingMainPageType.EventResult, new TrainingMainArguments(res, MainArguments.ActionName, MainArguments.ArgumentsKeeps));
            Adv.OnEnded -= OnAdvEnd;
            
            
        }
        
    }
}
