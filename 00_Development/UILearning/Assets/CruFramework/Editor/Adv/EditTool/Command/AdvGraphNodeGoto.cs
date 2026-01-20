using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/Goto")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Goto", "Locationコマンドまでコマンド実行位置を移動する。")]
    public class AdvGraphNodeGoto : AdvGraphNodeDefault<AdvCommandGoto>
    {

    }
}