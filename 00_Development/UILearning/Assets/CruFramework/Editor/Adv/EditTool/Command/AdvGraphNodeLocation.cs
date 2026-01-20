using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/Location")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Location", "コマンド位置指定用。")]
    public class AdvGraphNodeLocation : AdvCommandNode
    {
        protected override List<IAdvCommandObject> GetCommands()
        {
            return new List<IAdvCommandObject>(){ new AdvCommandLocation(NodeId) };
        }
    }
}