using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    [Serializable]
    public class AddressableCatalogSettings
    {
        [SerializeField]
        private string catalogName = string.Empty;
        public string CatalogName
        {
            get { return catalogName; }
            set { catalogName = value; }
        }
        
        private bool optimizeCatalogSize = false;
        public bool OptimizeCatalogSize
        {
            get { return optimizeCatalogSize; }
            set { optimizeCatalogSize = value; }
        }
    }
}
