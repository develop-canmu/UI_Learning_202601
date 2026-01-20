using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.InGame
{
    public class InGameMatchUpPhraseUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ParseTextMeshPro[] commandTexts;
        [SerializeField] private ParseTextMeshPro[] commandTextShadows;
        [SerializeField] private ParseTextMeshPro[] flavorTexts;
        [SerializeField] private ParseTextMeshPro[] flavorTextShadows;

        private const string OpenTrigger = "Open";
        private const string CloseTrigger = "Close";
        
        private const string TargetCharacterReplaceArg = "{CharacterName}";

        public void Open(bool hideSomePhrase)
        {
            SetPhraseTexts(hideSomePhrase);
            animator.SetTrigger(OpenTrigger);
        }

        public void Close()
        {
            animator.SetTrigger(CloseTrigger);
        }

        private int GetWiseRank(BattleCharacterModel character)
        {
            var deckA = BattleDataMediator.Instance.GetTeamDeck(character.Side);
            var deckB = BattleDataMediator.Instance.GetTeamDeck(BattleGameLogic.GetOtherSide(character.Side));

            var wiseRank = BattleAbilityLogic.GetInListStatusRank(character, deckA, deckB, BattleConst.StatusParamType.Wise);
            return wiseRank;
        }

        private void SetPhraseTexts(bool hideSomePhrase)
        {
            var matchUpResult = BattleDataMediator.Instance.NextMatchUpResult;
            var character = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            var wise = character.GetCurrentWise();
            var wiseRank = GetWiseRank(character);
            var commandData = BattleDataMediator.Instance.CommandData;
            SetText(wise, wiseRank, commandData, character, hideSomePhrase);
        }

        private void SetText(BigValue wise, int wiseRank, int commandData, BattleCharacterModel character, bool hideSomePhrase)
        {
            var matchUpResult = BattleDataMediator.Instance.NextMatchUpResult;

            var randPhraseIndex = new int[] { 0, 1, 2 }.OrderBy(_ => Guid.NewGuid()).ToArray();
            
            var hasPass = (commandData & (int)BattleConst.LogActionBits.HasPass) == (int)BattleConst.LogActionBits.HasPass;
            var hasLongPass = (commandData & (int)BattleConst.LogActionBits.HasLongPass) == (int)BattleConst.LogActionBits.HasLongPass;
            var hasBackPass = (commandData & (int)BattleConst.LogActionBits.HasBackPass) == (int)BattleConst.LogActionBits.HasBackPass;
            var hasShoot = (commandData & (int)BattleConst.LogActionBits.HasShoot) == (int)BattleConst.LogActionBits.HasShoot;
            var hasCross = (commandData & (int)BattleConst.LogActionBits.HasCross) == (int)BattleConst.LogActionBits.HasCross;
            var distanceToShootRange = matchUpResult.RemainDistanceToShoot;
            var shootPhrase = BattleGameLogic.GetCommandPhraseType(BattleConst.MatchUpActionType.Shoot, wise, wiseRank, hasShoot, distanceToShootRange);
            var throughPhrase = BattleGameLogic.GetCommandPhraseType(BattleConst.MatchUpActionType.Through, wise, wiseRank);
            CharaVoiceLocationType otherPhrase = CharaVoiceLocationType.None;
            if (hasCross)
            {
                otherPhrase = BattleGameLogic.GetCommandPhraseType(BattleConst.MatchUpActionType.Cross, wise, wiseRank);
            }

            if (hasPass)
            {
                otherPhrase = BattleGameLogic.GetCommandPhraseType(BattleConst.MatchUpActionType.Pass, wise, wiseRank);
            }

            if (hasLongPass)
            {
                otherPhrase = CharaVoiceLocationType.CommandPhraseLongPass;
            }

            if (hasBackPass)
            {
                otherPhrase = BattleGameLogic.GetCommandPhraseType(BattleConst.MatchUpActionType.Pass, wise, wiseRank, isBackPass: true);
            }

            foreach (var text in commandTexts)
            {
                text.enabled = false;
            }

            foreach (var text in commandTextShadows)
            {
                if (text != null)
                {
                    text.enabled = false;
                }
            }

            foreach (var text in flavorTexts)
            {
                text.enabled = false;
            }

            foreach (var text in flavorTextShadows)
            {
                if (text != null)
                {
                    text.enabled = false;
                }
            }

            if (shootPhrase != CharaVoiceLocationType.None)
            {
                var mes = MasterManager.Instance.charaLibraryVoiceMaster.GetDataByLocationType(shootPhrase)?.message;
                var index = randPhraseIndex[0];
                commandTexts[index].text = mes;
                commandTexts[index].enabled = true;
                if (commandTextShadows[index] != null)
                {
                    commandTextShadows[index].text = mes;
                    commandTextShadows[index].enabled = true;
                }
            }

            if (throughPhrase != CharaVoiceLocationType.None)
            {
                var mes = MasterManager.Instance.charaLibraryVoiceMaster.GetDataByLocationType(throughPhrase)?.message;
                var index = randPhraseIndex[1];
                commandTexts[index].text = mes;
                commandTexts[index].enabled = true;
                if (commandTextShadows[index] != null)
                {
                    commandTextShadows[index].text = mes;
                    commandTextShadows[index].enabled = true;
                }
            }

            if (otherPhrase != CharaVoiceLocationType.None)
            {
                var mes = MasterManager.Instance.charaLibraryVoiceMaster.GetDataByLocationType(otherPhrase)?.message;
                var index = randPhraseIndex[2];
                var targetCharacterId = matchUpResult.TargetCharacterId;
                if (targetCharacterId > 0)
                {
                    var targetChara = BattleDataMediator.Instance.GetBattleCharacter(targetCharacterId);
                    // コマンドの選択肢としてはパスorクロスも入ってても, 結果としてはシュートが選ばれてTargetCharaに敵が入ってくることがあるため.
                    // オフラインバトルでは関係ないが, Lambdaログからの再生だと一連のログを処理するのではなく単一のログを順次処理して言っている都合上しょうがない.
                    if (targetChara.Side == character.Side)
                    {
                        var targetCharaName = targetChara.GetNameWithTeamSideColorCode();
                        mes = mes?.Replace(TargetCharacterReplaceArg, targetCharaName);
                        commandTexts[index].text = mes;
                        commandTexts[index].enabled = true;
                        if (commandTextShadows[index] != null)
                        {
                            commandTextShadows[index].text = mes;
                            commandTextShadows[index].enabled = true;
                        }
                    }
                }
            }

            var flavorMessages = MasterManager.Instance.charaLibraryVoiceMaster.GetDataArrayByLocationType(CharaVoiceLocationType.CommandPhraseCharaFlavor, character?.CharaMaster?.parentMCharaId ?? -1);
            var max = Math.Min(flavorMessages.Length, flavorTexts.Length);
            if (hideSomePhrase)
            {
                max = 1;
            }
            
            for (var i = 0; i < max; i++)
            {
                var mes = flavorMessages[i];
                flavorTexts[i].text = mes.message;
                flavorTexts[i].enabled = true;
                if (flavorTextShadows[i] != null)
                {
                    flavorTextShadows[i].text = mes.message;
                    flavorTextShadows[i].enabled = true;
                }
            }
        }

#if !PJFB_REL
        public void DebugOpen()
        {
            animator.SetTrigger(OpenTrigger);
            
            var matchUpResult = BattleDataMediator.Instance.NextMatchUpResult;
            var character = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            var commandData = (int)BattleConst.LogActionBits.HasPass;
            SetText(new BigValue(1), 1, commandData, character, false);
        }
        
        public void DebugClose()
        {
            animator.SetTrigger(CloseTrigger);
        }
#endif
    }
}