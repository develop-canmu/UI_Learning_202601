using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/ActiveMessageWindow")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "ActiveMessageWindow", "指定したメッセージウィンドウの表示、非表示を切り替える。")]
    public class AdvGraphNodeActiveMessageWindow : AdvGraphNodeDefault<AdvCommandActiveMessageWindow>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}