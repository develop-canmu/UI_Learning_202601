using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Document;

namespace CruFramework.Adv
{

    public class AdvDocumentAttribute : DocumentAttribute
    {
        
        public AdvDocumentAttribute() : base()
        {
        }
        
        public AdvDocumentAttribute(string text) : base(text)
        {
        }
        
        public AdvDocumentAttribute(string name, string text) : base(name, text)
        {
        }
        
        public AdvDocumentAttribute(string category, string name, string text) : base(category, name, text)
        {
        }
    }
}