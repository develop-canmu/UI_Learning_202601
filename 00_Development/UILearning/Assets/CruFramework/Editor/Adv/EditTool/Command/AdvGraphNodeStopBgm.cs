using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{

    [AdvCommandNode("Framework/StopBgm")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "StopBgm", "BGMの停止。")]
    public class AdvGraphNodeStopBgm : AdvGraphNodeDefault<AdvCommandStopBgm>
    {

    }
}
