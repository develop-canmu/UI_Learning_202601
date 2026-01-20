using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/SetText")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "SetText", "テキストの表示。")]
    public class AdvGraphNodeSetText : AdvGraphNodeDefault<AdvCommandSetText>
    {

    }
}