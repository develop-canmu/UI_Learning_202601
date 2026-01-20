using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/ColorFade")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "ColorFade", "色を指定してフェードする。")]
    public class AdvGraphNodeColorFade : AdvGraphNodeDefault<AdvCommandColorFade>
    {

    }
}