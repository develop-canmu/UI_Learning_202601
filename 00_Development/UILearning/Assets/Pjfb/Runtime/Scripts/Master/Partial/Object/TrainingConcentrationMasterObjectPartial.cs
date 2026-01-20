
using System;

namespace Pjfb.Master {
	public partial class TrainingConcentrationMasterObject : TrainingConcentrationMasterObjectBase, IMasterObject {  
		
		public enum TrainingConcentrationType
		{
			None = -1,
			CZone= 1,
			Flow = 2,
		}

		public TrainingConcentrationType GetConcentrationType()
		{
			if (Enum.IsDefined(typeof(TrainingConcentrationType), (int)type))
			{
				return (TrainingConcentrationType)type;
			}

			CruFramework.Logger.LogError($"Not Find TrainingConcentrationType : {type} in TrainingConcentrationMasterObject id:{id}");
			return TrainingConcentrationType.None;
		}
	}

}
