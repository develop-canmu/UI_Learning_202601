using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;

namespace Pjfb.InGame
{
    public class BattlePlayerModel
    {
        public long UserId
        {
            get;
            private set;
        }
        
        public long IconId
        {
            get;
            private set;
        }
        
        public string Name
        {
            get;
            private set;
        }

        public BattleConst.TeamSide Side
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public bool IsBot
        {
            get;
            private set;
        }

        public int Tactics
        {
            get;
            private set;
        }

        public BattleV2GvgItem[] GvgActiveItems
        {
            get;
            private set;
        }

        public BattlePlayerModel(BattleV2Player playerData)
        {
            UserId = playerData.playerId;
            IconId = playerData.mIconId;
            Name = playerData.name;
            Index = (int)playerData.playerIndex;
            Side = playerData.groupIndex == 1 ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right;
            // 一旦.
            IsBot = playerData.playerType != 1;
            Tactics = (int)playerData.optionValue;
            GvgActiveItems = playerData.gvgItemList;
        }

        public void SetIndex(int index)
        {
            Index = index;
        }

        public void SetTactics(int tactics)
        {
            Tactics = tactics;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}