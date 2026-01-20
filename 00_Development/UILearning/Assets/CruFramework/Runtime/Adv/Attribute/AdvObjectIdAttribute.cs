using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
	public class AdvObjectIdAttribute : PropertyAttribute
	{
		
		public enum WindowType
		{
			Generic, SearchWindow
		}
		
		private string name = string.Empty;
		/// <summary>名前</summary>
		public string Name{get{return name;}}
		
		private WindowType winType = WindowType.Generic;
		/// <summary>ウィンドウの種類</summary>
		public WindowType WinType{get{return winType;}}

		public AdvObjectIdAttribute(string name, WindowType windowType = WindowType.Generic)
		{
			this.name = name;
			winType = windowType;
		}
	}
}