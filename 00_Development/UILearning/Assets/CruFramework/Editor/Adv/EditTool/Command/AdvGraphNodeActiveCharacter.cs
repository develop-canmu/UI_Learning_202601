using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/ActiveCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "ActiveCharacter", "指定したキャラクタの表示、非表示を切り替える。")]
    public class AdvGraphNodeActiveCharacter : AdvGraphNodeDefault<AdvCommandActiveCharacter>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}