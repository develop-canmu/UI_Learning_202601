using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{

    public class AdvObject : MonoBehaviour
    {
        
        private int id = 0;
        /// <summary>Id</summary>
        public int Id{get{return id;}}
        
        private ulong createId = 0;
        /// <summary>Id</summary>
        public ulong CreateId{get{return createId;}}
        
        private string dataName = string.Empty;
        /// <summary>名前</summary>
        public string DataName{get{return dataName;}}
        
        /// <summary>揺らす対象</summary>
        public virtual Transform ShakeTarget{get{return transform;}}
        
        private AdvObject parentObject = null;
        
        private List<AdvObject> children = new List<AdvObject>();
        /// <summary>子オブジェクト</summary>
        public IReadOnlyList<AdvObject> Children{get{return children;}}
        
        [SerializeField]
        private RectTransform childRoot = null;
        /// <summary>子オブジェクト配置ルート</summary>
        public RectTransform ChildRoot{get{return childRoot;}}

        public void SetTransformParent(Transform parent)
        {
            SetParent(null);
            transform.SetParent(parent);
        }
        
        public void SetParent(AdvObject parent)
        {
            if(parentObject != null)
            {
                parentObject.children.Remove(this);
            }
            parentObject = parent;         
            if(parentObject != null)
            {
                parentObject.children.Add(this);
                transform.SetParent( parentObject.ChildRoot != null ? parentObject.ChildRoot : parentObject.transform);
            }
        }

        /// <summary>名前のセット</summary>
        public void SetName(string name)
        {
            dataName = name;
        }
        
        /// <summary>Id</summary>
        public void SetId(int id)
        {
            this.id = id;
        }
        
        /// <summary>Id</summary>
        public void SetCreateId(ulong id)
        {
            createId = id;
        }

        private void OnDestroy()
        {
            SetParent(null);
        }
    }
}