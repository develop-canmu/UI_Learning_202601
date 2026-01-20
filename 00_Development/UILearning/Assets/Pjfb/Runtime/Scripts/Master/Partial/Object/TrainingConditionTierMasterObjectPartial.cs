
using System;

namespace Pjfb.Master {
	
	public enum TrainingConditionTier
	{
		AWFUL = 0,
		NOTBAD = 1,
		GOOD = 2,
		BEST = 3,
		EXTREME = 4,
		AWAKENING = 5,
	}

	public enum TrainingConditionTierType
	{
		Normal = 1,
		Flow = 2,
	}

	public partial class TrainingConditionTierMasterObject : TrainingConditionTierMasterObjectBase, IMasterObject
	{
		public TrainingConditionTier ConditionTier => (TrainingConditionTier)tier;
	}

}
