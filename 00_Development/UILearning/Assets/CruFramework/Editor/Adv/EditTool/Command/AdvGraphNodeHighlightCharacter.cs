using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/HighlightCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "HighlightCharacter", "キャラクタをハイライトする。")]
    public class AdvGraphNodeHighlightCharacter : AdvGraphNodeDefault<AdvCommandHighlightCharacter>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}