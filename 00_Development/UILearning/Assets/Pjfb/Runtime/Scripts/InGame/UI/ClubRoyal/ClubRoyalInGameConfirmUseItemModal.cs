using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameConfirmUseItemModal : ModalWindow
    {
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private UIButton okButton;
        [SerializeField] private PossessionItemUi possessionItemUi;

        private GuildBattleCommonItemData itemData;
        
        public class Arguments
        {
            public GuildBattleCommonItemData ItemData;
            
            public Arguments(GuildBattleCommonItemData itemData)
            {
                ItemData = itemData;
            }
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var openArgs = (Arguments)args;
            itemData = openArgs.ItemData;

            var pointMaster = MasterManager.Instance.pointMaster.FindData(itemData.ItemId);
            bodyText.text = StringValueAssetLoader.Instance["clubroyalingame.confirm_use_item_body"].Format(pointMaster?.name, PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleRecoveryValueOnUseItem);
            possessionItemUi.SetAfterCount(itemData.ItemId, itemData.RemainCount, itemData.RemainCount - 1);
            
            return base.OnPreOpen(args, token);
        }

        public void OnClickOKButton()
        {
            okButton.interactable = false;
            RequestDissolveParty().Forget();
        }

        private async UniTask RequestDissolveParty()
        {
            var playerData = await PjfbGameHubClient.Instance.RequestUseItem(itemData.ItemId);
            if (playerData == null)
            {
                AppManager.Instance.UIManager.System.UINotification.ShowNotification(StringValueAssetLoader.Instance["clubroyalingame.failed_to_use_item"]);
            }
            
            ClubRoyalInGameUIMediator.Instance.ActivateItemUI.PlayAnimation(itemData.ItemId);
            Close();
        }
    }
}