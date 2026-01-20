using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/CreateCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "CreateCharacter", "キャラクタを生成する。これ以降のコマンドでIdを指定してこのキャラクタを操作可能。重複して同じIdは生成できない。")]
    public class AdvGraphNodeCreateCharacter : AdvGraphNodeDefault<AdvCommandCreateCharacter>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}