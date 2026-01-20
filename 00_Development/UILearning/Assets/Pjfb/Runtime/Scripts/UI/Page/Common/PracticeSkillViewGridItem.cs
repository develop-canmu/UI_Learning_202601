using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb
{
	public class PracticeSkillViewGridItem : ScrollGridItem
	{
		[SerializeField]
		private PracticeSkillView view = null;
		
		protected override void OnSetView(object value)
		{
			PracticeSkillInfo data = (PracticeSkillInfo)value;
			var mCharaId = MasterManager.Instance.charaTrainingStatusMaster.FindData(data.MasterId).mCharaId;
			view.SetSkillData(data, mCharaId, -1);
		}
	}
}

