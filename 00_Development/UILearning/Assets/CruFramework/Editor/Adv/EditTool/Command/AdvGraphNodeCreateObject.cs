using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/CreateObject")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "CreateObject", "オブジェクトを生成する。")]
    public class AdvGraphNodeCreateObject : AdvGraphNodeDefault<AdvCommandCreateObject>
    {

    }
}