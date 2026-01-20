using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.UI;

namespace Pjfb
{
	public class PracticeSkillViewMiniGridItem : ScrollGridItem
	{
		public class Info
		{
			private PracticeSkillInfo practiceSkillInfo;
			private bool isLevelUp;
			private bool isLock;

			public PracticeSkillInfo PracticeSkillInfo {get => practiceSkillInfo;} 
			public bool IsLevelUp  {get => isLevelUp;} 
			public bool IsLock  {get => isLock;} 
			
			public Info(PracticeSkillInfo practiceSkillData, bool isLevelUp, bool isLock)
			{
				this.practiceSkillInfo = practiceSkillData;
				this.isLevelUp = isLevelUp;
				this.isLock = isLock;
			}
		}
		
		[SerializeField]
		private PracticeSkillViewMini view = null;
		
		protected override void OnSetView(object value)
		{
		
			switch(value)
			{
				case Info v:
				{
					view.SetSkillData(v.PracticeSkillInfo, v.IsLevelUp, v.IsLock);
					view.gameObject.SetActive(true);
					
					break;
				}
				
				case PracticeSkillInfo v:
				{
					view.SetSkillData(v, false, false);
					break;
				}

				case PracticeSkillLotteryInfo v:
				{
					view.SetSkillData(v);
					break;
				}
				
				case null:
				{
					view.gameObject.SetActive(false);
					break;
				}
			}
			
		}
	}
}

