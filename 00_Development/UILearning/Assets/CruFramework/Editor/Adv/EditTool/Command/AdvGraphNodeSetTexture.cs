using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/SetTexture")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "SetTexture", "テクスチャを変更する。")]
    public class AdvGraphSetTexture : AdvGraphNodeDefault<AdvCommandSetTexture>
    {

    }
}