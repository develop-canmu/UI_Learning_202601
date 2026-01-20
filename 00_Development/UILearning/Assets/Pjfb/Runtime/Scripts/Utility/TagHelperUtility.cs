using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Utility
{
    public class TagHelperUtility : MonoBehaviour
    {
        public enum GroupEnum
        {
            Default,
            Model3D,
            VFX
        }

        public GroupEnum group;
        public string id;
        public bool disableOnLoad = false;

        private static List<TagHelperUtility> _tagHelperUtilities = new List<TagHelperUtility>();

        public static TagHelperUtility GetHelper(GroupEnum group, string id)
        {
            var helper = _tagHelperUtilities.Find(x => x.group.Equals(group) && x.id.Equals(id));
            if (helper != null)
            {
                return helper;
            }

            return default;
        }
        
        public static List<TagHelperUtility> GetHelpersInGroup(GroupEnum group)
        {
            return _tagHelperUtilities.FindAll(x => x.group.Equals(group));
        }

        public static void SetEnable(GroupEnum group, string id, bool isEnable)
        {
            var h = GetHelper(group, id);
            if (h != default)
            {
                h.gameObject.SetActive(isEnable);
            }
        }

        public void SetInfo(GroupEnum group, string id, bool isDisableOnLoad = false)
        {
            this.group = group;
            this.id = id;
            this.disableOnLoad = isDisableOnLoad;
        }
        protected virtual void Awake()
        {
            gameObject.SetActive(!disableOnLoad);
            _tagHelperUtilities.Add(this);
            _tagHelperUtilities.RemoveAll(x => x == null);
        }

        protected virtual void OnDestroy()
        {
            _tagHelperUtilities.Remove(this);
            _tagHelperUtilities.RemoveAll(x => x == null);
        }

        protected void Test()
        {
            
        }
    }
}