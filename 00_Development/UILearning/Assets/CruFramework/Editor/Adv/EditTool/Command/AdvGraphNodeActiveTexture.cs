using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/ActiveTexture")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "ActiveTexture", "テクスチャの表示切り替え。")]
    public class AdvGraphNodeActiveTexture : AdvGraphNodeDefault<AdvCommandActiveTexture>
    {

    }
}