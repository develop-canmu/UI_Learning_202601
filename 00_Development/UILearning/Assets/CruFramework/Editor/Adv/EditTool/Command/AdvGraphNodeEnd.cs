using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/End")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "End", "Adv終了位置に必ず配置する。")]
    public class AdvGraphNodeEnd : AdvCommandNode
    {
        public override bool HasOutputPort{get{return false;}}
        protected override Port.Capacity InputPortCapacity{ get{return Port.Capacity.Multi;}}

        protected override List<IAdvCommandObject> GetCommands()
        {
            return new List<IAdvCommandObject>(){ new AdvCommandEnd() };
        }
    }
}