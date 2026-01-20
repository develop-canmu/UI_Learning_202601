using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.InGame
{
    public class InGameSelectGoalSceneModal : ModalWindow
    {
        [SerializeField] private ScrollGrid goalScenesScroll;

        protected override void OnAwake()
        {
            goalScenesScroll.ItemPrefab.gameObject.SetActive(false);

            base.OnAwake();
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var targetCharacterId = (long)args;
            var list = BattleLogMediator.Instance.DigestLogLists.Where(digestLog => digestLog.ReferencedCharacterIds.Contains(targetCharacterId)).ToList();
            goalScenesScroll.SetItems(list);

            BattleDataMediator.Instance.ReplayTargetCharacterId = targetCharacterId;

            return base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            // リプレイ後の戻ってきたときだけ直でこのモーダル開くので...
            if (!AppManager.Instance.UIManager.ModalManager.IsModalOpened<InGameStatsDetailModal>())
            {
                var characterModel = BattleDataMediator.Instance.GetBattleCharacter(BattleDataMediator.Instance.ReplayTargetCharacterId);
                AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.StatDetail, characterModel);
            }
            
            base.OnClosed();
        }
    }
}