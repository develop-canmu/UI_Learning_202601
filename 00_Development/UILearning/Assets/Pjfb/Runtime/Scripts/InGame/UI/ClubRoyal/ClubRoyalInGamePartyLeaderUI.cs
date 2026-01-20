using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGamePartyLeaderUI : MonoBehaviour
    {
        [SerializeField] private CharacterIcon characterIcon;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject partyNumberRoot;
        [SerializeField] private TMP_Text partyNumberText;
        [SerializeField] private ClubRoyalInGameBallCountUI ballCountUI;
        [SerializeField] private TMP_Text ballCountText;
        [SerializeField] private TMP_Text revivalTurnCountText;
        [SerializeField] private Animator phraseAnimator;
        [SerializeField] private TMP_Text phraseText;
        [SerializeField] private Image[] sideBasedOutlineColorImages;
        [SerializeField] private ClubRoyalInGameStatusBadgeContainer statusBadgeContainer;
        // クールダウン中のバフがかかったときのエフェクト用。インスペクターセット済み。ロジック追加時に表示処理を追加する予定です
        [SerializeField] private GameObject growEffectRoot;

        private Action<GuildBattlePartyModel> onClickIconCallback = null;
        private GuildBattlePartyModel partyModel;
        private DateTime lastEffectTime = DateTime.MinValue;

        private int lastSyncedRevivalTurn = -1;
        private int countUpBallCountFinal;
        private const float EffectMinDisplayTimeSeconds = 1f; // エフェクトの最小表示時間
        
        private GuildBattleCommonConst.GuildBattleTeamSide viewSide;

        public enum AnimationType
        {
            Normal,
            MapNormal,
            StandbyNormal,
            Lose,
            CoolTime,
            ReDeployable,
            BallCountUp,
            AttackL,
            AttackR,
            Damaged,
        }

        public int GetPartyIdentifier()
        {
            return partyModel?.Identifier ?? -1;
        }

        public void InitializeOnMap(GuildBattlePartyModel partyModel, bool showLastMilitaryStrength)
        {
            var leaderUCharaId = partyModel.UCharaIds.FirstOrDefault();
            if (!PjfbGuildBattleDataMediator.Instance.BattleCharaData.TryGetValue(leaderUCharaId, out var leaderChara))
            {
                // failsafe.
                return;
            }
            
            gameObject.SetActive(true);
            partyNumberRoot.SetActive(partyModel.PlayerIndex == PjfbGuildBattleDataMediator.Instance.PlayerIndex);
            countUpBallCountFinal = 0;

            viewSide = partyModel.Side == PjfbGuildBattleDataMediator.Instance.PlayerSide
                ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right;
            characterIcon.SetIcon(leaderChara.mCharaId);
            characterIcon.SetActiveRarity(false);
            characterIcon.SetActiveCharacterTypeIcon(false);
            characterIcon.SetActiveLv(false);
            partyNumberText.text = partyModel.PlayerPartyId.ToString();
            SetOutlineColor();

            this.partyModel = partyModel;

            PlayAnimation(AnimationType.MapNormal);
            var ballCount = showLastMilitaryStrength ? this.partyModel.GetLastBallCount() : this.partyModel.GetBallCount();
            //ballCountText.text = $"x{ballCount.ToString()}";
            ballCountUI.SetActiveBallCount(ballCount);
            
            List<BattleV2AbilityEffect> activatedAbilityEffectList = GuildBattleAbilityLogic.GetActivatedAbilityEffectList(
                partyModel,
                PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData[PjfbGuildBattleDataMediator.Instance.PlayerIndex]);

            statusBadgeContainer.Initialize(partyModel.PlayerIndex == PjfbGuildBattleDataMediator.Instance.PlayerIndex ? activatedAbilityEffectList : null);
        }
        
        public void InitializePartyList(GuildBattlePartyModel partyModel, bool showLastMilitaryStrength)
        {
            var leaderUCharaId = partyModel.UCharaIds.FirstOrDefault();
            if (!PjfbGuildBattleDataMediator.Instance.BattleCharaData.TryGetValue(leaderUCharaId, out var leaderChara))
            {
                // failsafe.
                return;
            }

            gameObject.SetActive(true);
            characterIcon.SetIcon(leaderChara.mCharaId);
            characterIcon.SetActiveRarity(false);
            characterIcon.SetActiveCharacterTypeIcon(false);
            characterIcon.SetActiveLv(false);
            partyNumberText.text = partyModel.PlayerPartyId.ToString();

            this.partyModel = partyModel;

            var isOnMap = partyModel.IsOnMap();
            if (isOnMap)
            {
                var ballCount = showLastMilitaryStrength ? this.partyModel.GetLastBallCount() : this.partyModel.GetBallCount();
                //ballCountText.text = $"x{ballCount.ToString()}";
                ballCountUI.SetActiveBallCount(ballCount);
                PlayAnimation(AnimationType.MapNormal);
            }
            else
            {
                if (partyModel.RevivalTurn > 0)
                {
                    revivalTurnCountText.text = StringValueAssetLoader.Instance["clubroyalingame.revival_turn_format"].Format(partyModel.RevivalTurn);
                    if (lastSyncedRevivalTurn <= 0)
                    {
                        PlayAnimation(AnimationType.CoolTime);
                    }
                }
                else
                {
                    if (lastSyncedRevivalTurn > 0)
                    {
                        PlayAnimation(AnimationType.ReDeployable);
                    }
                    else
                    {
                        PlayAnimation(AnimationType.StandbyNormal);
                    }
                }
            }

            List<BattleV2AbilityEffect> activatedAbilityEffectList = GuildBattleAbilityLogic.GetActivatedAbilityEffectList(
                partyModel ,
                PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData[PjfbGuildBattleDataMediator.Instance.PlayerIndex]);

            statusBadgeContainer.Initialize(activatedAbilityEffectList);

            lastSyncedRevivalTurn = (int)partyModel.RevivalTurn;
        }

        private void SetOutlineColor()
        {
            var key = viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? "clubroyalingame.ally_leaderui_outline" : "clubroyalingame.enemy_leaderui_outline";
            foreach (var image in sideBasedOutlineColorImages)
            {
                image.color = ColorValueAssetLoader.Instance[key];
            }
            
            ballCountUI.SetOutlineColor(viewSide);
        }

        public void PlayAnimation(AnimationType animationType)
        {
            animator.SetTrigger(animationType.ToString());
        }

        public void SetBallCountText(int ballCount)
        {
            ballCountUI.SetActiveBallCount(ballCount);
        }
        
        public void PlayCountUpBallCountAnimation(int finalCount)
        {
            countUpBallCountFinal = finalCount;
            animator.SetTrigger(AnimationType.BallCountUp.ToString());
        }

        public void SetOnClickCallback(Action<GuildBattlePartyModel> callback)
        {
            onClickIconCallback = callback;
        }

        public void OnClickIcon()
        {
            onClickIconCallback?.Invoke(partyModel);
        }

        /// <summary>
        /// Called by animation event.
        /// </summary>
        public void OnChangeBallCountTiming()
        {
            if (countUpBallCountFinal > 0)
            {
                //ballCountText.text = $"x{countUpBallCountFinal}";
                ballCountUI.SetActiveBallCount(countUpBallCountFinal);
            }
        }

        private const string openLeftPhraseTrigger = "OpenL";
        private const string openRightPhraseTrigger = "OpenR";

        public enum WordPhraseType
        {
            OnStartMove,
            OnMatchUp,
            OnBeatEnemy,
        }
        
        public void PlayWordPhraseBalloon(WordPhraseType phraseType)
        {
            var trigger = viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left
                ? openLeftPhraseTrigger : openRightPhraseTrigger;
            var phrase = string.Empty;
            switch (phraseType)
            {
                case WordPhraseType.OnStartMove:
                    phrase = StringValueAssetLoader.Instance[$"clubroyalingame.word_move_pattern_{Random.Range(1, 4)}"];
                    break;
                case WordPhraseType.OnMatchUp:
                    phrase = StringValueAssetLoader.Instance[$"clubroyalingame.word_matchup_pattern_{Random.Range(1, 4)}"];
                    break;
                case WordPhraseType.OnBeatEnemy:
                    phrase = StringValueAssetLoader.Instance[$"clubroyalingame.word_win_pattern_{Random.Range(1, 4)}"];
                    break;
            }
            
            phraseText.text = phrase;
            phraseAnimator.SetTrigger(trigger);
        }

        public void GrowEffectAtCoolTimeParty()
        {
            if (growEffectRoot != null && partyModel != null)
            {
                // クールタイム中のパーティーにエフェクトを表示する
                // クールタイム中でない場合は非表示にする
                lastEffectTime = AppTime.Now;
                growEffectRoot.SetActive(partyModel.IsCoolTime());
            }
        }

        public async UniTask ClearGrowEffectRoot()
        {
            if (growEffectRoot != null)
            {
                float elapsed = (float)(AppTime.Now - lastEffectTime).TotalSeconds;
                if (elapsed < EffectMinDisplayTimeSeconds)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(EffectMinDisplayTimeSeconds - elapsed));
                }

                growEffectRoot.SetActive(false);
            }
        }
    }
}