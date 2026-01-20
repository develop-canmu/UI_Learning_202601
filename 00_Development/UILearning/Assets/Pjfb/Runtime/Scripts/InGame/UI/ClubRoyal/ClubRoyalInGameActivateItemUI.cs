using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameActivateItemUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ClubRoyalInGameActiveItemUI itemUI;
        [SerializeField] private TMP_Text body;
        
        private const string OpenTrigger = "Open";

        public void PlayAnimation(long itemId)
        {
            gameObject.SetActive(true);
            animator.SetTrigger(OpenTrigger);
            itemUI.SetTexture(itemId);
            body.text = StringValueAssetLoader.Instance["clubroyalingame.activate_item_body"].Format(PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleRecoveryValueOnUseItem);
        }
    }
}