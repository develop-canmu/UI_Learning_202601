using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using Pjfb.Networking.App.Request;
using Pjfb.Master;
using Pjfb.Utility;
using Logger = UnityEngine.Logger;

namespace Pjfb
{
    public class TutorialTrainingActionPage : TrainingActionPage
    {
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            //AppManager.Instance.TutorialManager.ExecuteTutorialAction();
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.TutorialManager.ShowTouchGuard();
            base.OnEnablePage(args);
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }
        
        protected override async UniTask DecisionCardAsync(int index)
        {
            // マスタ取得
            TrainingCard card =　pending.handList[cardViews[index].Index];
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(card.mTrainingCardId);

            // 練習に参加しているキャラ
            var listSupported3DModel = new  List<long>(MainArguments.Pending.handSupportMCharaIdList[selectedCardIndex].l);
            // 練習参加キャラはサポートキャラ以外のキャラも含まれるのでサポートキャラとフレンドのみに絞る
            listSupported3DModel.RemoveAll(x =>
            {
                // サポートキャラとフレンド以外のデータは除く
                return MainArguments.SupportAndFriendCharacterDatas.Any(s => s.MCharId.Equals(x)) == false;
            });
            
            // TrainingProgress のレスポンスを捏造
            var progressData = AppManager.Instance.TutorialManager.GetTrainingProgressData();
            var args = new TrainingMainArguments(progressData, mCard.practiceName, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = card.mTrainingCardId;
            args.SelectedTrainingCardIndex = index;
            args.JoinSupportCharacters = listSupported3DModel.ToArray();
            
            OpenPage(TrainingMainPageType.EventResult,args);
            await new UniTask();
        }

        protected override async UniTask RestButtonAsync()
        {
            var progressData = AppManager.Instance.TutorialManager.GetTrainingProgressData();
            OpenPage(TrainingMainPageType.Top, new TrainingMainArguments(progressData, MainArguments.ArgumentsKeeps));
            await UniTask.NextFrame();
        }
        
        protected override void SetCombinationUi()
        {
            combinationLockRoot.SetActive(true);
            combinationSkillCountRoot.SetActive(false);
        }

    }
}
