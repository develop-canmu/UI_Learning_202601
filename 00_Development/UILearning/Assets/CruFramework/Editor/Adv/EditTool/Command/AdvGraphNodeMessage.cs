using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{

    [AdvCommandNode("Framework/Message")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Message", "メッセージウィンドウにテキストの表示")]
    public class AdvGraphNodeMessage : AdvGraphNodeDefault<AdvCommandMessage>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
        
        protected override bool IsHitSearchParameters(string text)
        {
            return Command.Message.ToLower().Contains(text);
        }
    }
}
