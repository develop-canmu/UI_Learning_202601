using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/Camera ")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Camera ", "カメラを操作する。")]
    public class AdvGraphNodeCamera : AdvGraphNodeDefault<AdvCommandCamera>
    {

    }
}