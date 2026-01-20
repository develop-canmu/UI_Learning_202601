
using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/OpenModal")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "OpenModal", "モーダルを開く。")]
    public class AdvGraphNodeOpenModal : AdvGraphNodeDefault<AdvCommandOpenModal>
    {

    }
}

