using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    
    /// <summary>
    /// データを見て各イベント処理を分岐させる
    /// </summary>
    public class TrainingTopPage : TrainingPageBase
    {
        protected virtual void InitializePage()
        {
            
            // 自動トレーニング完了
            if( (MainArguments.OptionFlags & TrainingMainArguments.Options.AutoTrainingFinished) != TrainingMainArguments.Options.None)
            {
                // トレーニング結果へ
                OpenPage(TrainingMainPageType.TrainingResult, MainArguments);
                return;
            }
            
            // スキップボタン無効化
            Footer.EnableSkipButton(false);
            
            // Advの獲得チップ数置換
            Adv.SetTrainingTipCount(MainArguments.CurrentTipCount);
            // トレーニングキャラId
            Adv.SetTrainingCharacterId(MainArguments.TrainingCharacter.MCharId);            

            // コンセントレーションのラベル
            Header.ShowCoentrationLabelEffect(false);
            // ブーストボーナス
            Header.BoostView.gameObject.SetActive(false);

            // ログの読み込み
            if(Adv.MessageLogs.Count == 0 && TrainingUtility.LogCount > 0)
            {
                TrainingUtility.LoadLog(Adv);
            }
            
            // インゲームから
            if(MainArguments.IsFromInGame)
            {
                MainArguments.IsFromInGame = false;
                // ステータスアップがある場合
                if(MainArguments.AnyReward())
                {
                    // 現在の情報を更新
                    MainPageManager.UpdateData(MainArguments);
                    // キャッシュしている進捗情報
                    TrainingOverallProgress progress = TrainingUtility.PerformanceProgressCache;
                    // ヘッダーを表示
                    Header.SetView(MainArguments);
                    // ターン表示更新
                    Header.UpdateTurn(MainArguments);
                    // イベント非表示
                    Header.ShowEventView(false);
                    // パフォーマンスビューを更新
                    Header.PerformaceView.SetTipCount(progress.currentValue);
                    Header.PerformaceView.Set(progress, 0, true);
                    OpenPage(TrainingMainPageType.EventResult, MainArguments);
                    return;
                }
            }
            
            // コンセントレーションエフェクト
            if(MainArguments.IsChangeConcentration())
            {
                // Viewの初期化
                InitializeView();
                
                if (MainArguments.IsFlow())
                {
                    // Flow演出
                    OpenPage(TrainingMainPageType.FlowEffect, MainArguments);
                }
                else
                {
                    // 演出画面へ
                    OpenPage(TrainingMainPageType.ConcentrationEffect, MainArguments);
                }

                return;
            }

            if (MainArguments.HasConcentrationEffect())
            {
                // コンセートレーションエフェクト
                if (MainPageManager.ConcentrationZoneEffectPlayer.IsPlaying)
                {
                    MainPageManager.ConcentrationZoneEffectPlayer.SetEffect(MainArguments.GetConcentrationEffectId());
                }
                else
                {
                    TrainingConcentrationEffectType type = MainArguments.IsFlow() ? TrainingConcentrationEffectType.Flow : TrainingConcentrationEffectType.Concentration;

                    MainPageManager.ConcentrationZoneEffectPlayer.PlayEffect(type, MainArguments.GetConcentrationEffectId(), 1.0f);
                }
            }

            // ターン延長系
            // トレーニング再開時に毎回演出再生されるのを防ぐため初回は強制スキップ
            if (!MainArguments.ArgumentsKeeps.IsLockOpenExtraTurnLotteryPage) 
            {
                // ターン延長
                if (MainArguments.IsStartFirstExtraTurnThisTurn() && !MainArguments.ArgumentsKeeps.IsShownFirstExtraTurnEffect)
                {
                    OpenPage(TrainingMainPageType.ExtraTurnLottery, MainArguments);
                    return;
                }
                
                // 延長継続
                if (MainArguments.IsStartContinueExtraTurnThisTurn() && !MainArguments.ArgumentsKeeps.IsShownContinueExtraTurnEffect)
                {
                    OpenPage(TrainingMainPageType.ExtraTurnLottery, MainArguments);
                    return;
                }
            }
            else
            {
                MainArguments.UnlockOpenExtraTurnLotteryPage();
            }
            
            // 目標達成 or 失敗
            if(MainArguments.IsCompleteTarget())
            {
                // ヘッダーを非表示
                Header.Hide();
                
                TrainingGoal target = MainArguments.GetTarget(MainArguments.Reward.mTrainingEventId);
                switch( (TrainingUtility.TargetResult)MainArguments.Reward.hasAchievedGoal)
                {
                    case TrainingUtility.TargetResult.Success:
                    {
                        MainPageManager.TargetResult.PlayOpenTargetAchieved(MainArguments.Pending.goalList, target, IsTargetEffectSkip, OnEndTargetResultAnimation);
                        
                        // 次の目標がある場合は目標を表示
                        if(MainArguments.CurrentTarget != null)
                        {
                            MainArguments.OptionFlags |= TrainingMainArguments.Options.OpenTargetPage;
                        }
                        
                        break;
                    }
                    
                    case TrainingUtility.TargetResult.Failed:
                    {
                        MainPageManager.TargetResult.PlayOpenTargetUnachieved(MainArguments.Pending.goalList, target, IsTargetEffectSkip, OnEndTargetResultAnimation);
                        break;
                    }
                }
                MainArguments.Reward.hasAchievedGoal = 0;
                return;
            }
        
            // トレーニング終わり
            if(MainArguments.IsEndTraining)
            {
                // トレーニング結果へ
                OpenPage(TrainingMainPageType.TrainingResult, MainArguments);
                return;
            }
            

            // ビューの初期化
            InitializeView();
            // ターン表示更新
            Header.UpdateTurn(MainArguments);
            // デッキ情報
            TrainingDeckUtility.SetCurrentTrainingDeck( new TrainingDeckUtility.DeckMember(MainArguments.TrainingCharacter, MainArguments.SupportAndFriendCharacterDatas, MainArguments.SpecialSupportCharacterDatas) );
            
            // 目標の表示
            if(MainArguments.IsOpenTargetPage())
            {
                OpenPage(TrainingMainPageType.Target, MainArguments);
                return;
            }

            var eventType = (TrainingEventType)MainArguments.TrainingEvent.eventType;
            switch(eventType)
            {
                // シナリオの再生
                case TrainingEventType.Scenario:
                {
                    OpenPage(TrainingMainPageType.Adv, MainArguments);
                    break;
                }
                
                // 休憩
                case TrainingEventType.Rest:
                {
                    if(IsRestSkip)
                    {
                        OpenPage(TrainingMainPageType.Rest, MainArguments);
                    }
                    else
                    {
                        OpenPage(TrainingMainPageType.Adv, MainArguments);
                    }
                    
                    break;
                }
                
                // ユーザーの行動
                case TrainingEventType.Action:
                {
                    // キャラを表示
                    MainPageManager.Character.gameObject.SetActive(true);
                    OpenPage(TrainingMainPageType.Action, MainArguments);
                    break;
                }
                
                // 試合（練習試合・本番試合）
                case TrainingEventType.Battle:
                {
                    OpenPage(TrainingMainPageType.PracticeGamePreparation, MainArguments);
                    break;
                }
            }            
            
#if !PJFB_REL
            
            // 新機能のログ
            if(TrainingUtility.IsEnableType(TrainingScenarioType.Concentration, MainArguments.Pending.mTrainingScenarioId))
            {
                
                System.Text.StringBuilder log1 = new System.Text.StringBuilder();
                System.Text.StringBuilder log2 = new System.Text.StringBuilder();
                System.Text.StringBuilder log3 = new System.Text.StringBuilder();
                System.Text.StringBuilder log4 = new System.Text.StringBuilder();

                TrainingPending pending = MainArguments.Pending;
                TrainingEventReward reward = MainArguments.Reward;
                
                log1.AppendLine("pending.turn : " + pending.turn);
                log1.AppendLine("pending.mTrainingConcentrationId : " + pending.mTrainingConcentrationId);
                log1.AppendLine("pending.isFinishedConcentration : " + pending.isFinishedConcentration);
                log2.AppendLine("pending.inspireExp : " + pending.inspireExp);
                log2.AppendLine("pending.inspireList : " + pending.inspireList.Length);
                log2.AppendLine("コンセントレーション状態変化 : " + MainArguments.IsChangeConcentration());
                
                // Advのログに入れる
                Adv.AddMessageLog($"<color=#f00>[デバッグ:コンセントレーション]</color> {MainArguments.ActionName}", $"{log1}", 0);
                Adv.AddMessageLog($"<color=#f00>[デバッグ:コンセントレーション]</color> {MainArguments.ActionName}", $"{log2}", 0);
                
                if(reward != null)
                {
                    log3.AppendLine("reward.mTrainingConcentrationId : " + reward.mTrainingConcentrationId );
                    log3.AppendLine("reward.isConcentrationExtended : " + reward.isConcentrationExtended );
                    log3.AppendLine("reward.isConcentrationGradeUp : " + reward.isConcentrationGradeUp );
                    log4.AppendLine("reward.inspireEnhanceRate : " + reward.inspireEnhanceRate );
                    log4.AppendLine("reward.inspireList : " + (reward.inspireList == null ? "Null" : reward.inspireList.Length) );
                    
                    Adv.AddMessageLog($"<color=#f00>[デバッグ:コンセントレーション]</color> {MainArguments.ActionName}", $"{log3}", 0);
                    Adv.AddMessageLog($"<color=#f00>[デバッグ:コンセントレーション]</color> {MainArguments.ActionName}", $"{log4}", 0);
                }
            }
#endif
            
        }
        
        private void InitializeView()
        {
            // ヘッダーの表示
            Header.Show();
            // フッターの表示
            Footer.Show();
            // イベントの非表示
            Header.ShowEventView(false);
            // ヘッダーの更新
            Header.SetView(MainArguments);
            // 現在の情報を更新
            MainPageManager.UpdateData(MainArguments);
            // キャラを表示
            MainPageManager.Character.gameObject.SetActive(false);
        }
        
        protected override void OnEnablePage(object args)
        {
            InitializePage();
            base.OnEnablePage(args);
        }
        
        private void OnEndTargetResultAnimation()
        {
            InitializePage();
        }
    }
}
