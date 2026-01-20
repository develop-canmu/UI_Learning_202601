
namespace Pjfb.Master {
	
	public enum TrainingScenarioStatusBonusType
	{
        Card = 1,
        Event = 2,
	}
	
	public partial class TrainingScenarioStatusBonusMasterObject : TrainingScenarioStatusBonusMasterObjectBase, IMasterObject {  
		
		
		new public TrainingScenarioStatusBonusType type{get{return (TrainingScenarioStatusBonusType)base.type;}}
	}

}
