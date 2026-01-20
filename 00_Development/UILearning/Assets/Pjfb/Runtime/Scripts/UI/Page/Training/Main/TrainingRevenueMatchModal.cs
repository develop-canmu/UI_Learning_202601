using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    
    public class TrainingRevenueMatchModal : ModalWindow
    {
        
        [SerializeField]
        private DeckRankImage deckRankImage = null;
        [SerializeField]
        private TMP_Text deckTotalValue = null;
        [SerializeField]
        private OmissionTextSetter deckTotalValueOmissionTextSetter = null;
        
        
        [SerializeField]
        private DeckRankImage opponentRankImage = null;
        [SerializeField]
        private TMP_Text opponentTotalValue = null;
        [SerializeField]
        private OmissionTextSetter opponentTotalValueOmissionTextSetter = null;
        
        [SerializeField]
        private TMP_Text rewardTipText = null;
        [SerializeField]
        private OmissionTextSetter rewardTipOmissionTextSetter = null;
        
        [SerializeField]
        private CharacterVariableIcon[] characterIcons = null;
        
        private TrainingMainArguments mainArguments = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
        
            SetCloseParameter(null);
        
            mainArguments = (TrainingMainArguments)args;

            TrainingIntentionalEvent e = mainArguments.Pending.intentionalEventList[0];
            
            List<BattleV2Chara> playerList = TrainingUtility.GetBattleCharacterList(e.clientData, TrainingUtility.PlayerType.Player);
            List<BattleV2Chara> npcList = TrainingUtility.GetBattleCharacterList(e.clientData, TrainingUtility.PlayerType.Npc);
            
            // 総戦力
            BigValue playerTotal = BigValue.Zero;
            foreach(BattleV2Chara c in playerList)
            {
                playerTotal += new BigValue(c.combatPower);
            }
            // 総戦力
            BigValue opponentTotal = BigValue.Zero;
            foreach(BattleV2Chara c in npcList)
            {
                opponentTotal += new BigValue(c.combatPower);
            }
            
            // 自分の総合力
            deckTotalValue.text = playerTotal.ToDisplayString(deckTotalValueOmissionTextSetter.GetOmissionData());
            // ランクアイコン
            deckRankImage.SetTextureAsync( StatusUtility.GetPartyRank( playerTotal) ).Forget();
            // 相手の総合力
            opponentTotalValue.text = opponentTotal.ToDisplayString(opponentTotalValueOmissionTextSetter.GetOmissionData());
            // ランクアイコン
            opponentRankImage.SetTextureAsync( StatusUtility.GetPartyRank( opponentTotal) ).Forget();
            // 報酬
            TrainingEventRewardMasterObject mReward = MasterManager.Instance.trainingEventRewardMaster.FindData(e.expectedMTrainingEventRewardId);
            rewardTipText.text = new BigValue(mReward.hp).ToDisplayString(rewardTipOmissionTextSetter.GetOmissionData());
            // 対戦相手アイコン表示
            for(int i=0;i<npcList.Count;i++)
            {
                BattleV2Chara c = npcList[i];
                CharacterVariableIcon icon = characterIcons[i];
                CharacterVariableDetailData data = new CharacterVariableDetailData(c);
                icon.SetIcon(data);
                icon.SetIconTextureWithEffectAsync(c.mCharaId).Forget();
            }

            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoChoice();
#endif
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private void DebugAutoChoice()
        {
            if (TrainingChoiceDebugMenu.EnabledAutoChoiceAction)
            {
                OnStartButton();
            }
        }
#endif

        /// <summary>UGUI</summary>
        public void OnStartButton()
        {
            BattleStartAsync().Forget();
        }
        
        private async UniTask BattleStartAsync()
        {
            TrainingProgressArgs postArgs = new TrainingProgressArgs();
            // イベントId
            postArgs.mTrainingIntentionalEventId = mainArguments.Pending.intentionalEventList[0].mTrainingIntentionalEventId;
            // API
            TrainingProgressAPIResponse response = await TrainingUtility.ProgressAPI(TrainingUtility.RevenueMatch, postArgs);
            // 試合準備画面
            SetCloseParameter(response);
            // モーダルを閉じる
            await CloseAsync();
        }
    }
}
