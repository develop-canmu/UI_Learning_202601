using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;

namespace Pjfb.InGame
{
    public class InGameSelectGoalSceneScrollItem : ScrollGridItem
    {
        [SerializeField] private InGameSelectGoalSceneScrollItemCharacterUI characterUIOne;
        [SerializeField] private InGameSelectGoalSceneScrollItemCharacterUI characterUITwo;
        [SerializeField] private TextMeshProUGUI goalNumText;
        [SerializeField] private TextMeshProUGUI goalMethodText;
        [SerializeField] private GameObject versusIcon;
        [SerializeField] private GameObject assistIcon;
        [SerializeField] private UIButton button;
        
        private int logIndex = -1;
        
        private void Awake()
        {
            button.OnClickEx.AddListener(OnClickReplayDigest);
        }

        protected override void OnSetView(object value)
        {
            logIndex = -1;
            var digestLog = value as DigestLog;
            if (digestLog == null)
            {
                return;
            }
            
            gameObject.SetActive(true);

            logIndex = digestLog.Index;
            characterUIOne.SetData(digestLog.PrimaryCharacter, true);
            characterUITwo.SetData(digestLog.SecondCharacter, digestLog.IsCross);
            goalNumText.text = $"{digestLog.ScoreIndex}得点目";
            
            if (digestLog.AbilityId > 0)
            {
                goalMethodText.text = MasterManager.Instance.abilityMaster.FindData(digestLog.AbilityId)?.name;
            }
            else
            {
                goalMethodText.text = digestLog.IsCross ? "クロス" : "シュート";
            }
            
            versusIcon.SetActive(!digestLog.IsCross);
            assistIcon.SetActive(digestLog.IsCross);
        }

        private void OnClickReplayDigest()
        {
            BattleEventDispatcher.Instance.OnClickReplayDigestCallback(logIndex);
            // インゲームダイジェストに移行するのでモーダルは全部閉じる
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
        }
    }
}