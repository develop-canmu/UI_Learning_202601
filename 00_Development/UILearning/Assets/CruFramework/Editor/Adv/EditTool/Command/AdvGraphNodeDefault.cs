using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    public abstract class AdvGraphNodeDefault<T> : AdvCommandNode where T : IAdvCommandObject, new()
    {
        [SerializeField]
        [AdvDocument]
        private T command = new T();
        /// <summary>
        /// コマンド
        /// </summary>
        public T Command{get{return command;}}
        
        protected override List<IAdvCommandObject> GetCommands()
        {
            return new List<IAdvCommandObject>(){command};
        }
    }
}