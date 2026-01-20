using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CruFramework.Editor.Adv
{

    public class AdvCommandNodeAttribute : Attribute
    {
        private string path = string.Empty;
        /// <summary>path</summary>
        public string Path{get{return path;}}
        
        public AdvCommandNodeAttribute(string path)
        {
            this.path = path;
        }
    }

}