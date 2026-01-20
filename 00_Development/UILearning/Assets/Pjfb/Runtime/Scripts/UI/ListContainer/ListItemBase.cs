using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(LayoutElement))]
    public class ListItemBase : MonoBehaviour
    {
        #region ItemParams
        public class ItemParamsBase{}
        #endregion
        
        #region SerializeFields
        #endregion

        #region PublicProperties
        #endregion

        #region OverrideMethods
        public virtual void Init(ItemParamsBase itemParamsBase) { }
        #endregion
        
        #region PrivateMethods
        #endregion
    }
}