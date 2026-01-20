
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxHighlightScrollDynamicItemSelector : ScrollDynamicItemSelector
    {

        [SerializeField] 
        ScrollDynamicItem _label;

        [SerializeField] 
        ScrollDynamicItem _listItem;
        
        public override ScrollDynamicItem GetItem(object item){
            var param = (GachaBoxHighlightScrollDynamicItemData)item;
            if( param.labelParam != null ){
                return _label;
            } else {
                return _listItem;
            }
            
            
        }

    }
}
