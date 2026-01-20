using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using CruFramework;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Pjfb
{
    public class SortingLayerSetter : MonoBehaviour
    {
        [SerializeField][SortingLayerDrawer]
        private int sortingLayer = 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetLayer(sortingLayer);
        }
#endif

        /// <summary>レイヤー設定</summary>
        public void SetLayer(int layer)
        {
            sortingLayer = layer;
            // UIの場合レイヤーを調整
            Component[] components = gameObject.GetComponentsInChildren<Component>(true);
            foreach(Component c in components)
            {
                switch(c)
                {
                    case Canvas v:
                    { 
                        v.sortingLayerID = sortingLayer;
                        break;
                    }
                    case SortingGroup v:
                    {
                        v.sortingLayerID = sortingLayer;
                        break;
                    }
                }
            }
        }
        
    }
}