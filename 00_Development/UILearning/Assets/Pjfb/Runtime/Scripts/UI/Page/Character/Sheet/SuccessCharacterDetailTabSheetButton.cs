using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
	public class SuccessCharacterDetailTabSheetButton : SheetSwitchButton<SuccessCharacterDetailTabSheetManager, SuccessCharacterDetailTabSheetType>
    {
	    [SerializeField] private Image activeImage;
	    [SerializeField] private Sprite singleTabSprite;
	    [SerializeField] private Sprite firstTabSprite;
	    [SerializeField] private Sprite centerTabSprite;
	    [SerializeField] private Sprite lastTabSprite;

	    public void UpdateSprite()
	    {
		    var childCount = 0;
		    foreach (Transform item in transform.parent)
		    {
			    if (item.gameObject.activeSelf)
			    {
				    childCount++;
			    }
		    }
		    
		    if (childCount == 1)
		    {
			    activeImage.sprite = singleTabSprite;
			    return;
		    }
		    
		    var index = transform.GetSiblingIndex();
		    var isFirst = index == 0;
		    if (isFirst)
		    {
			    activeImage.sprite = firstTabSprite;
			    return;
		    }
		    
		    var isLast = index == childCount - 1;
		    if (isLast)
		    {
			    activeImage.sprite = lastTabSprite;
			    return;
		    }
		    
		    activeImage.sprite = centerTabSprite;
	    }
    }
}
