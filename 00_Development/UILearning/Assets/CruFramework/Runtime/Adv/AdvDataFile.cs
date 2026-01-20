using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    [CreateAssetMenu]
    public class AdvDataFile : ScriptableObject
    {
        [SerializeReference]
        private IAdvCommandObject[] commands = null;
        /// <summary>コマンド</summary>
        public IAdvCommandObject[] Commands{get{return commands;}}

        public void SetCommands(IAdvCommandObject[] commands)
        {
            this.commands = commands;
        }
    }
}
