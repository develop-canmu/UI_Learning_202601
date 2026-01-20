using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/ShakeCamera")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "ShakeCamera", "カメラを揺らす。")]
    public class AdvGraphNodeShakeCamera : AdvGraphNodeDefault<AdvCommandShakeCamera>
    {

    }
}