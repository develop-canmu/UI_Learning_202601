
using System;

namespace Pjfb.Master
{
	
	public enum MissionTabType
	{
		Daily = 1,
		Total = 2,
		Event = 3,
	}

	public enum MissionChallengeConditionType
	{
		TagId = 101
	}
	
	public partial class DailyMissionCategoryMasterObject : DailyMissionCategoryMasterObjectBase, IMasterObject
	{
		public new MissionTabType tabType => (MissionTabType)base.tabType;
		public MissionChallengeConditionType challengeConditionTypeEnum => (MissionChallengeConditionType)base.challengeConditionType;
	}

}
