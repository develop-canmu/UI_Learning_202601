using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
	[System.Serializable]
    public sealed class AdvCommandHighlightCharacter : IAdvCommand
    {
	    
	    [SerializeField]
	    [AdvObjectId(nameof(AdvConfig.CharacterDatas))]
	    [AdvDocument("ハイライトする対象キャラクタ。")]
	    private int[] ids = null;
	    
	    void IAdvCommand.Execute(AdvManager manager)
	    {
		    manager.GrayoutCharacters();
		    foreach(int id in ids)
		    {
			    AdvCharacter c = manager.GetAdvCharacter<AdvCharacter>(id);
			    if(c != null)
				{
					c.Highlight();
			    }
		    }

	    }
    }
}