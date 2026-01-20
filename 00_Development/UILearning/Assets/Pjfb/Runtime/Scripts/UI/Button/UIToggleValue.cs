using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
	
	public abstract class UIToggleMember : UIToggle
	{
		/// <summary>値</summary>
		public abstract object Value{get;}
	}
	
    public abstract class UIToggle<T> : UIToggleMember
    {
        [SerializeField]
        private T value = default;
        /// <summary>値</summary>
        public override object Value{get{return value;}}
    }
    
    public abstract class UIToggleGroup<T> : ToggleGroup
    {
	    public T GetSelectedValue()
	    {
		    Toggle toggle = GetFirstActiveToggle();
		    if(toggle is UIToggleMember m)
			{
				return (T)m.Value;
		    }
		    return default;
	    }
	}
}
