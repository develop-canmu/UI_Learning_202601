using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using Pjfb.Adv;
using CruFramework;
using UnityEngine;

namespace Pjfb
{
    public class TutorialTrainingMain : TrainingMain
    {

        [SerializeField] public UIButton logButton;
        
        protected override string GetAddress(TrainingMainPageType page)
        {
            return $"Prefabs/UI/Page/TutorialTraining/{page}Page.prefab";
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialTraining);
            await TrainingUtility.LoadConfig();
            var trainingProgressData = AppManager.Instance.TutorialManager.GetTrainingProgressData();
            var trainingEvent = trainingProgressData.trainingEvent;
            var pending = trainingProgressData.pending;
            var charaVariable = trainingProgressData.charaVariable;
            var battlePending = trainingProgressData.battlePending;
            var topModal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
            if (topModal != null)
            {
                // モーダルを閉じる
                await topModal.CloseAsync();
            }

            var preloadAdvIdList = AppManager.Instance.TutorialManager.GetPreloadAdvIdList();
            foreach (var id in preloadAdvIdList)
            {
                await Adv.PreLoadAdvFile( ResourcePathManager.GetPath(AppAdvManager.ResourcePathKey, id) );
            }
            
            await base.OnPreOpen(new TrainingMainArguments(trainingEvent, pending, battlePending, charaVariable, null, new TrainingMainArgumentsKeeps(), TrainingMainArguments.Options.OpenTargetPage), token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
        }
        
        protected override UniTask OnMessage(object value)
        {
            // ユーザーのステップによってはタイトル後ここに直接来る場合もあるのでUI非表示対応
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.BeginFade:
                        AppManager.Instance.UIManager.Header.Hide(); 
                        AppManager.Instance.UIManager.Footer.Hide();
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        protected override void OnClosed()
        {
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialTraining);
            base.OnClosed();
        }
        
        protected override void OnBackkey()
        {
            // チュートリアル中はバックキーの操作をなくす
        }

        public void SetButtonInteractable(bool interactable)
        {
            logButton.interactable = interactable;
        }
    }
}
