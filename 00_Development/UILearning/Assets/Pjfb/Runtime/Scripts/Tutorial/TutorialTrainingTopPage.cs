using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

namespace Pjfb
{
    public class TutorialTrainingTopPage : TrainingTopPage
    {
        protected override void InitializePage()
        {
            InitializePageAsync().Forget();
        }

        private async UniTask InitializePageAsync()
        {
            
            // TODO チュートリアルでは目標の表示なし

            // トレーニング終わり
            if(MainArguments.IsEndTraining)
            {
                // トレーニング結果へ
                OpenPage(TrainingMainPageType.TrainingResult, MainArguments);
                return;
            }
            
            // ヘッダーの表示
            Header.Show();
            // フッターの表示
            Footer.Show();
            // イベントの非表示
            Header.ShowEventView(false);
            // ヘッダーの更新
            Header.SetView(MainArguments);
            // ターン表示更新
            Header.UpdateTurn(MainArguments);
            // 現在の情報を更新
            MainPageManager.UpdateData(MainArguments);
            
            TrainingEventType eventType = (TrainingEventType)MainArguments.TrainingEvent.eventType;
            
            // スキップボタンの表示
            bool isScenario = eventType == TrainingEventType.Scenario || eventType == TrainingEventType.Rest;
            Footer.EnableSkipButton(isScenario);
            
            // チュートリアルではAPI叩かないので、同フレームでTopから次pageに遷移しないよう待ちを入れる
            await UniTask.NextFrame();
            
            switch(eventType)
            {
                // シナリオの再生
                case TrainingEventType.Scenario:
                case TrainingEventType.Rest:
                {
                    OpenPage(TrainingMainPageType.Adv, MainArguments);
                    break;
                }
                
                // ユーザーの行動
                case TrainingEventType.Action:
                {
                    OpenPage(TrainingMainPageType.Action, MainArguments);
                    break;
                }
                
                // 練習試合
                case TrainingEventType.Battle:
                {
                    OpenPage(TrainingMainPageType.PracticeGamePreparation, MainArguments);
                    break;
                }
            }
        }
    }
}