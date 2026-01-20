using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/CaseEnd")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "CaseEnd", "分岐の最後にかならず配置する。複数のルートを一つのCaseEndに接続可能。")]
    public class AdvGraphNodeCaseEnd : AdvCommandNode
    {
        protected override Port.Capacity InputPortCapacity { get{return Port.Capacity.Multi; }}

        protected sealed override List<IAdvCommandObject> GetCommands()
        {
            // システム側で調整するのでここでコマンドは返さなくていい
            return null;
        }
    }
}