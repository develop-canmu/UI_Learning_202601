using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/WaitClick")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "WaitClick", "クリックを待機する。")]
    public class AdvGraphNodeWaitClick : AdvGraphNodeDefault<AdvCommandWaitClick>
    {

    }
}