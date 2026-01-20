using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/EntryPoint")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "EntryPoint", "開始位置。必ず必要")]
    public class AdvGraphNodeEntryPoint : AdvCommandNode
    {
        public override bool HasInputPort{get{return false;}}
        
        protected override List<IAdvCommandObject> GetCommands()
        {
            return null;
        }
    }
}