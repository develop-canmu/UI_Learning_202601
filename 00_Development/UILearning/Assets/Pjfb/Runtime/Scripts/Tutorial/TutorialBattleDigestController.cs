using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Pjfb.InGame;
using Pjfb.Master;
using Logger = CruFramework.Logger;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TutorialBattleDigestController : BattleDigestController
    {

        private bool scenarioInGameMode;
        private bool tutorialNormalSpeed;

        public void SetScenarioInGameMode()
        {
            scenarioInGameMode = true;
        }

        public void SetTutorialPlaySpeed(bool isNormalSpeed)
        {
            tutorialNormalSpeed = isNormalSpeed;
        }

        protected override string GetAddress(BattleConst.DigestType type, BattleDigestCharacterData mainCharacterData)
        {
            if (!scenarioInGameMode) return base.GetAddress(type, mainCharacterData);
            var inGameMessageContainer = AppManager.Instance.TutorialManager.ExecuteTutorialMessageAction(type, true);
            // TODO 4人でドリブル会話するための例外対応
            if (inGameMessageContainer != null && inGameMessageContainer.messageList.Count >= 4)
            {
                return "Prefabs/InGame/Dribble_Tutorial.prefab";
            }

            return base.GetAddress(type, mainCharacterData);
        }

        protected override List<BattleDialogueData> CreateBattleDialogueDataList(BattleConst.DigestType type, BattleConst.TeamSide offenceSide, BattleDigestCharacterData mainCharacterData, List<BattleDigestCharacterData> otherCharacterDataList, bool isShowDialogPhrase)
        {
            if (!scenarioInGameMode) return base.CreateBattleDialogueDataList(type, offenceSide, mainCharacterData, otherCharacterDataList, isShowDialogPhrase);
            var dialogueDataList = new List<BattleDialogueData>();
            var inGameMessageContainer = AppManager.Instance.TutorialManager.ExecuteTutorialMessageAction(type);

            if (inGameMessageContainer == null || inGameMessageContainer.messageList.Count == 0)
            {
                return dialogueDataList;
            }

            // TODO この辺はボイス実装後に共通化できると思うんで整理
            for (var i = 0; i < inGameMessageContainer.messageList.Count; i++)
            {
                var message = inGameMessageContainer.messageList[i];
                var master = MasterManager.Instance.charaLibraryVoiceMaster.FindData(message.charaVoiceLibraryId);
                if (master == null)
                {
                    continue;
                }

                var dialogueData = new BattleDialogueData();
                dialogueData.master = master;
                dialogueData.charaId = message.charaId;
                dialogueData.isPlayer = message.isPlayer;
                dialogueDataList.Add(dialogueData);
            }

            return dialogueDataList;
        }

        protected override float GetDigestPlaySpeed(BattleConst.DigestType digestType,
            BattleDigestCharacterData mainCharacter, List<BattleDigestCharacterData> otherCharacters,
            BattleConst.TeamSide offenceSide)
        {
            if (tutorialNormalSpeed) return 1.0f;
            return base.GetDigestPlaySpeed(digestType, mainCharacter, otherCharacters, offenceSide);
        }
        
        protected override void OnLoadedAction(BattleConst.DigestType type)
        {
            if (!AppManager.Instance.TutorialManager.ValidateDigestTypeCondition(TutorialSettings.DigestTriggerType.In,type))
            {
                return;
            }
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }

        public override async UniTask PlayAsync(BattleDigestLog digestLog, CancellationToken token, Action finishAction, Action _middleAction = null)
        {
            // チュートリアルでは敵側マッチアップも強制的に映す

            token.ThrowIfCancellationRequested();
            middleAction = _middleAction;
            if (digestLog.Type != BattleConst.DigestType.None)
            {
                await PlayAsync(digestLog.Type, digestLog.OffenceSide, digestLog.MainCharacterData, digestLog.OtherCharacterDataList, digestLog.Score, digestLog.DistanceToGoal, digestLog.IsLastScoreToEnd);
            }
            // TODO オブジェクト破棄で演出停止されてfinishActionが呼ばれるとコールバック先でエラー吐くので必要に応じてキャンセル

            if (digestLog.Type is  BattleConst.DigestType.Goal or BattleConst.DigestType.ThrowIn or BattleConst.DigestType.ThrowInKeeper &&
                BattleDataMediator.Instance.IsReleaseAsset)
            {
                Release();
                await Resources.UnloadUnusedAssets().ToUniTask();
                GC.Collect();
            }
            
            finishAction?.Invoke();
        }
    }
}