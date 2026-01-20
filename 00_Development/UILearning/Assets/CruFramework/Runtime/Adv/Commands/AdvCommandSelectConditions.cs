using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{

	[System.Serializable]
    public class AdvCommandSelectConditions : IAdvCommandConditions
    {
	    [SerializeField]
	    [AdvDocument("選択した番号。")]
	    private int no = 0;
	    
	    bool IAdvCommandConditions.GetConditions(AdvManager manager)
        {
	        return manager.GetValue<int>(AdvConstants.SelectDataKey) == no;
        }
	}
}