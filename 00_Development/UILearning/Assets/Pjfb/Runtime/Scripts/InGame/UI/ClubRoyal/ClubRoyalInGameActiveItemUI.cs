using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameActiveItemUI : MonoBehaviour
    {
        [SerializeField] private ItemIcon itemIcon;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private GameObject blackCoverRoot;
        [SerializeField] private GameObject onCoolTimeRoot;
        [SerializeField] private GameObject reachLimitRoot;
        [SerializeField] private TMP_Text remainCoolTimeText;
        [SerializeField] private UIButton button;

        private GuildBattleCommonItemData itemData;

        public void SetTexture(long itemId)
        {
            itemIcon.SetTextureAsync(itemId).Forget();
        }
        
        public void Initialize(GuildBattleCommonItemData itemData)
        {
            itemIcon.SetTextureAsync(itemData.ItemId).Forget();
            UpdateUI(itemData);
        }

        public void UpdateUI(GuildBattleCommonItemData itemData)
        {
            this.itemData = itemData;
            var isUsedMaxCount = itemData.IsUsedMaxCount();
            var isRemainCountZero = itemData.RemainCount == 0;
            var countFormat = isUsedMaxCount || isRemainCountZero ? "clubroyalingame.use_max_count_active_item_count_format" : "clubroyalingame.active_item_count_format";
            countText.text = StringValueAssetLoader.Instance[countFormat].Format(itemData.RemainCount, itemData.MaxUseCount);
            var canUse = itemData.CanUse();
            button.interactable = canUse;
            blackCoverRoot.SetActive(!canUse);
            if (!canUse)
            {
                reachLimitRoot.SetActive(false);
                onCoolTimeRoot.SetActive(false);
                if (itemData.IsUsedMaxCount())
                {
                    reachLimitRoot.SetActive(true);
                }
                else if(itemData.RemainCount > 0)
                {
                    if (itemData.CoolTime > 0)
                    {
                        onCoolTimeRoot.SetActive(true);
                        remainCoolTimeText.text = itemData.CoolTime.ToString();
                    }
                }
            }
        }
        
        public void OnClickIcon()
        {
            if (GuildBattleCommonDataMediator.Instance.GameState != GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                AppManager.Instance.UIManager.System.UINotification.ShowNotification(StringValueAssetLoader.Instance["clubroyalingame.cant_use_item_before_fight"]);
                return;
            }
            
            var args = new ClubRoyalInGameConfirmUseItemModal.Arguments(itemData);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameConfirmUseItem, args);
        }
    }
}