using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/DeleteObject")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "DeleteObject", "指定したオブジェクトの削除。")]
    public class AdvGraphNodeDeleteObject : AdvGraphNodeDefault<AdvCommandDeleteObject>
    {

    }
}