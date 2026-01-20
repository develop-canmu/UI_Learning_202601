using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.UI;

namespace Pjfb
{
	public class PracticeSkillTypeViewGridItem : ScrollGridItem
	{
		[SerializeField]
		private PracticeSkillTypeView view = null;
		
		protected override void OnSetView(object value)
		{
			PracticeSkillInfo data = (PracticeSkillInfo)value;
			view.SetSkillData(data);
		}

		public void SetSkillData(object value)
		{
			OnSetView(value);
		}
	}
}

