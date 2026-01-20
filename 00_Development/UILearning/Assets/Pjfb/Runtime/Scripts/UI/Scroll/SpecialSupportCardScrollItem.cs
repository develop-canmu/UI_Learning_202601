using CruFramework.UI;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{
    public class SpecialSupportCardScrollItem : ScrollGridItem
    {
        [SerializeField]
        private SpecialSupportCardIcon icon = null;
        
        private CharacterScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (CharacterScrollData)value;
            icon.SetIcon(scrollData.CharacterId, scrollData.CharacterLv, scrollData.LiberationLv);
            icon.SwipeableParams = scrollData.SwipeableParams;
        }
    }
}