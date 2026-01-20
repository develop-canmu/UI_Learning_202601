using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Document;

namespace CruFramework
{

    public class FrameworkDocumentAttribute : DocumentAttribute
    {
        
        public FrameworkDocumentAttribute() : base()
        {
        }
        
        public FrameworkDocumentAttribute(string text) : base(text)
        {
        }
        
        public FrameworkDocumentAttribute(string name, string text) : base(name, text)
        {
        }
        
        public FrameworkDocumentAttribute(string category, string name, string text) : base(category, name, text)
        {
        }
    }
}