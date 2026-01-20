using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/Switch")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Switch", "分岐点に配置する。条件コマンドにのみ接続可能。")]
    public class AdvGraphNodeSwitch : AdvCommandNode
    {
        protected override Port.Capacity OutputPortCapacity { get{return Port.Capacity.Multi;}}

        protected override List<IAdvCommandObject> GetCommands()
        {
            return null;
        }
    }
}