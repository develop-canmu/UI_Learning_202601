using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
	
	public interface IAdvCommandLocationIndex
	{
		void SetCommandIndex(int index);
		ulong LocationId{get;}
	}
	
	public class AdvCommandLocation : IAdvCommandObject
    {
	    [SerializeField][HideInInspector]
	    private ulong id = 0;
	    /// <summary>Id</summary>
	    public ulong Id{get{return id;}}
	    
	    
	    public AdvCommandLocation(ulong id)
	    {
		    this.id = id;
	    }
	}
}