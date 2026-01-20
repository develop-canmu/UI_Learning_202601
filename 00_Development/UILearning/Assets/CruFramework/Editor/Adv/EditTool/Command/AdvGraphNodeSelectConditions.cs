using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{

    [AdvCommandNode("Framework/SelectConditions")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "SelectConditions", "Selectコマンドの条件チェックコマンド。")]
    public class AdvGraphNodeSelectConditions : AdvGraphNodeCase
    {
        [SerializeField]
        [AdvDocument]
        private AdvCommandSelectConditions command = new AdvCommandSelectConditions();
        /// <summary>
        /// コマンド
        /// </summary>
        public AdvCommandSelectConditions Command{get{return command;}}
        
        protected override List<IAdvCommandObject> GetCommands()
        {
            return new List<IAdvCommandObject>(){command};
        }
    }
}
