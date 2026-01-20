
namespace Pjfb.Master {
	public partial class HuntEnemyMasterObject : HuntEnemyMasterObjectBase, IMasterObject {
		public bool IsBattle => mCharaNpcIdList.Length > 0;
	}

}
