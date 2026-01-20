using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/FrontCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "FrontCharacter", "キャラクタの表示順を設定する。上から順番に手前に表示。")]
    public class AdvGraphNodeFrontCharacter : AdvGraphNodeDefault<AdvCommandFrontCharacter>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}