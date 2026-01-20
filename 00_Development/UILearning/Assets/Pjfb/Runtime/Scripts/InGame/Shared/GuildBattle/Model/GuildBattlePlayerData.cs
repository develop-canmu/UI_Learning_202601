using System;
using System.Collections.Generic;
using MagicOnion;
using MessagePack;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    [MessagePackObject]
    public class GuildBattlePlayerData : GuildBattleCommonPlayerData
    {
        [Key(11)]
        public int RemainTurnToRecoveryBallCount;
        
        public void SetData(BattlePlayerModel player)
        {
            PlayerId = player.UserId;
            PlayerIndex = player.Index;
            Side = (GuildBattleCommonConst.GuildBattleTeamSide)player.Side;
            RemainTurnToRecoveryBallCount = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleRequiredTurnToRecoveryMilitaryStrength;
            AvailableMilitaryStrength = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleInitialMilitaryStrength;
            MaxMilitaryStrength = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleInitialMilitaryStrength;
            GuildBattleItemList = new List<GuildBattleCommonItemData>();
            foreach (var item in player.GvgActiveItems)
            {
                var itemData = new GuildBattleCommonItemData();
                itemData.SetData(item);
                GuildBattleItemList.Add(itemData);
            }
            GuildBattleActivatedAbilityList = new List<GuildBattleAbilityData>();
            /*
            var dummy = new GuildBattleCommonItemData();
            dummy.ItemId = 1;
            dummy.RemainCount = 5;
            dummy.MaxUseCount = 3;
            dummy.CoolTime = 0;
            dummy.BringInCount = dummy.RemainCount;
            GuildBattleItemList.Add(dummy);
            */
        }

        public override void AddAvailableMilitaryStrength(int value, bool isAllowOverheal)
        {
            if (isAllowOverheal)
            {
                AvailableMilitaryStrength += value;
            }
            else
            {
                AvailableMilitaryStrength = Math.Clamp(AvailableMilitaryStrength + value, 0, MaxMilitaryStrength);
            }

            if (AvailableMilitaryStrength >= MaxMilitaryStrength)
            {
                RemainTurnToRecoveryBallCount = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleRequiredTurnToRecoveryMilitaryStrength;
            }
        }
    }
}