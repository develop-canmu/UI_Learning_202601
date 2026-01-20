using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;


namespace CruFramework.Editor.Adv
{

    public abstract class AdvGraphNodeSelectBase<T> : AdvGraphNodeDefault<T>, IAdvCommandSelect where T : IAdvCommandSelect, new()
    {
        int IAdvCommandSelect.GetSelectCount()
        {
            return Command.GetSelectCount();
        }
    }
    
    [AdvCommandNode("Framework/Select")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Select", "選択肢を表示させる。")]
    public class AdvGraphNodeSelect　: AdvGraphNodeSelectBase<AdvCommandSelect>
    {

    }
}
