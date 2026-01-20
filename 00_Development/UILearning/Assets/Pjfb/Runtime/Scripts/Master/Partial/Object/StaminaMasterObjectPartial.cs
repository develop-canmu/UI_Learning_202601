
namespace Pjfb.Master {
	public partial class StaminaMasterObject : StaminaMasterObjectBase, IMasterObject {
		public bool IsDailyRecovery()
		{
			return cureType == 2;
		}
		
	}

}
