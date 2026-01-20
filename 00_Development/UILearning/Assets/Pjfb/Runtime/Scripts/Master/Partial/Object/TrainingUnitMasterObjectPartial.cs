
namespace Pjfb.Master {
	
	public enum TrainingUnitComboType
	{
		None = 0,
		Common = 1,
		Individual = 2
	}
	
	public partial class TrainingUnitMasterObject : TrainingUnitMasterObjectBase, IMasterObject {  
		
		public new TrainingUnitComboType type{get{return (TrainingUnitComboType)base.type;}}
		
	}

}
