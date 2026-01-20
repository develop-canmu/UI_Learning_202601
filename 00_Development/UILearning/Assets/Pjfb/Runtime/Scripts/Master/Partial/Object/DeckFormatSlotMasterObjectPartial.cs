
using System;

namespace Pjfb.Master {

	public enum RoleNumber
	{
		None = 0,
		FW = 1,
		MF = 2,
		DF = 3,
		Adviser = 11,
	}
	
	public partial class DeckFormatSlotMasterObject : DeckFormatSlotMasterObjectBase, IMasterObject {
	
		public new RoleNumber roleNumber
		{
			get { return (RoleNumber)base.roleNumber; }
		}
	}

}
