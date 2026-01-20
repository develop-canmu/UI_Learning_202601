using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;

namespace Pjfb.InGame
{
    public enum StatsDetailTabType
    {
        ActionResult, SkillResult
    }

    public class InGameStatsDetailModal : ModalWindow
    {
        [SerializeField] private UIButton selectReplaySceneButton;
        [SerializeField] private CharacterVariableIcon characterIcon;
        [SerializeField] private BaseCharacterNameView characterNameView;
        [SerializeField] private CharacterStatusValuesView characterStatusValuesView;
        [SerializeField] private InGameStatsDetailViewsUI statsDetailsViewsUI;
        [SerializeField] private InGameStatsDetailSkillCountsUI skillCountsUI;

        private long characterId = -1;
        private bool openSelectGoalModal = false;

        protected override void OnAwake()
        {
            selectReplaySceneButton.OnClickEx.AddListener(OpenSelectReplaySceneModal);
            
            base.OnAwake();
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            openSelectGoalModal = false;
            return base.OnOpen(token);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var characterModel = args as BattleCharacterModel;
            if (characterModel != null)
            {
                SetData(characterModel);
            }
            return base.OnPreOpen(args, token);
        }

        private void SetData(BattleCharacterModel characterModel)
        {
            characterId = characterModel.id;
            
            characterIcon.SetIconTextureWithEffectAsync(characterModel.MCharaId).Forget();
            characterIcon.SetIcon(characterModel.CombatPower, characterModel.Rank);
            characterIcon.SetRoleNumberIcon((RoleNumber)characterModel.Position);

            // レベル関連の表示が無いので適当でOK
            characterNameView.InitializeUi(characterModel.CharaMaster, 0, 0, 0);
            characterStatusValuesView.SetStatus(StatusUtility.ToCharacterStatus(characterModel));
            statsDetailsViewsUI.SetData(characterModel);

            selectReplaySceneButton.interactable = characterModel.Stats.GoalCount > 0 || characterModel.Stats.GoalAssistCount > 0;
            skillCountsUI.SetData(characterModel);
        }

        private async void OpenSelectReplaySceneModal()
        {
            openSelectGoalModal = true;
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SelectPlaybackGoalScene, characterId);
        }

        protected override void OnClosed()
        {
            // リプレイ後の戻ってきたときだけ直でゴール選択モーダル開くので...
            if (!openSelectGoalModal && !AppManager.Instance.UIManager.ModalManager.IsRunOpen &&
                !AppManager.Instance.UIManager.ModalManager.IsModalOpened<InGameStatsListModal>())
            {
                AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SelectPlayerDisplayStats, BattleDataMediator.Instance.Decks);
            }
            
            base.OnClosed();
        }
    }
}