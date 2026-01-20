using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;

namespace Pjfb.InGame
{
    public class BattleBluelockManModel
    {
        public long Level { get; private set; }
        public string VisualKey { get; private set; }
        public long RequestValueMin { get; private set; }
        public long RequestValueMax { get; private set; }

        public BattleBluelockManModel(BattleV2BattleFbKeeper bluelockManData)
        {
            Level = bluelockManData.level;
            VisualKey = bluelockManData.visualKey;
            RequestValueMin = bluelockManData.requestValueMin;
            RequestValueMax = bluelockManData.requestValueMax;
        }
    }
}
