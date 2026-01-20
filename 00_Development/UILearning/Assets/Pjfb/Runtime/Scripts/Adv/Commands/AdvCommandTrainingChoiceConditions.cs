using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.Adv;

namespace Pjfb.Adv
{

	[System.Serializable]
    public class AdvCommandTrainingChoiceConditions : IAdvCommandConditions
    {
	    [SerializeField]
	    [AdvDocument("選択したId。")]
	    private int id = 0;
	    
	    bool IAdvCommandConditions.GetConditions(AdvManager manager)
        {
	        return manager.GetValue<int>(AdvConstants.SelectDataKey) == id;
        }
	}
}