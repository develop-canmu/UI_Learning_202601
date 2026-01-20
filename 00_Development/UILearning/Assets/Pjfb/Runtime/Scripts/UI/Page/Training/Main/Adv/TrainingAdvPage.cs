using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CruFramework;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Networking.App.Request;
using UnityEngine;
using UnityEngine.UI;

using Pjfb.Master;
using UnityEngine.AddressableAssets;

namespace Pjfb.Training
{
    
    
    public class TrainingAdvPage : TrainingPageBase
    {
        protected override void OnOpened(object args)
        {
            // イベント名の表示
            Header.ShowEventView(true);
            
            // シナリオがないか、スキップする場合は再生しない
            if(string.IsNullOrEmpty(MainArguments.TrainingEvent.scenarioNumber) || IsScenarioSkip)
            {
                AdvEndAsync().Forget();
            }
            else
            {
                PlayAdv( MainArguments.TrainingEvent.scenarioNumber ).Forget();
            }
            base.OnOpened(args);
        }

        protected override void OnEnablePage(object args)
        {
            // ヘッダーの表示
            Header.SetActiveAdvPage();
            base.OnEnablePage(args);
        }
        
        protected void OnAdvEnd()
        {
            AdvEndAsync().Forget();
        }
        
        
        protected virtual async UniTask AdvEndAsync()
        {
            Adv.OnEnded -= OnAdvEnd;
            
            TrainingMainArguments arguments = null;

            // シナリオスキップ
            if(IsScenarioSkip)
            {
                // 次に進める
                TrainingProgressAPIResponse response = await TrainingUtility.ProgressAPI(true);
                arguments = new TrainingMainArguments(response, MainArguments.TrainingEvent.name, MainArguments.ArgumentsKeeps);
                
                // コンディションを表示
                if(MainArguments.Reward != null && MainArguments.Reward.condition != 0)
                {
                    if(arguments.AnyReward())
                    { 
                        // 結果画面で表示
                        ReservationMessage( TrainingUtility.GetConditionChangeMessage(MainArguments.Reward.condition) );
                    }
                    else
                    {
                        SetMessage( TrainingUtility.GetConditionChangeMessage(MainArguments.Reward.condition) );
                    }
                }
            }
            // 選択肢なし
            else if(MainArguments.TrainingEvent.choiceList.Length == 0)
            {
                // 次に進める
                TrainingProgressAPIResponse response = await TrainingUtility.ProgressAPI(TrainingUtility.ChoiceNullResult);
                arguments = new TrainingMainArguments(response, MainArguments.TrainingEvent.name, MainArguments.ArgumentsKeeps);
            }
            // 選択肢あり
            else
            {
                // 選択した番号
                // 1始まりなので-1
                int choiceIndex = Adv.GetValue<int>(AdvConstants.SelectDataKey) - 1;
                // 次に進める
                TrainingProgressAPIResponse response = await TrainingUtility.ProgressAPI(MainArguments.TrainingEvent.choiceList[choiceIndex]);
                arguments = new TrainingMainArguments(response, MainArguments.TrainingEvent.name, MainArguments.ArgumentsKeeps);
            }
            
            // フェードイン
            if(Adv.Transition.State == AdvTransition.FadeState.FadeOut)
            {
                await Adv.Transition.FadeIn();
            }
            
            // ステータス上昇があるかチェック
            if(arguments.AnyReward())
            {
                // 結果へ移動
                OpenPage(TrainingMainPageType.EventResult, arguments);
            }
            else
            {
                // Topへ移動
                OpenPage(TrainingMainPageType.Top, arguments);
            }
            

        }
        
        private async UniTask PlayAdv(string id)
        {
            // キャラ名を登録
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(MainArguments.TrainingCharacter.MCharId);
            Adv.AddMessageReplaceString(AppAdvConstants.ReplaceCharacterName, mChar.shortName);
            
            Adv.OnEnded -= OnAdvEnd;
            Adv.OnEnded += OnAdvEnd;
            // チョイスリストを登録
            Adv.SetChoice(MainArguments.TrainingEvent.choiceList);
            // Advを再生
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            await Adv.LoadAdvFile( ResourcePathManager.GetPath(AppAdvManager.ResourcePathKey, id) );
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        }
    }
}
