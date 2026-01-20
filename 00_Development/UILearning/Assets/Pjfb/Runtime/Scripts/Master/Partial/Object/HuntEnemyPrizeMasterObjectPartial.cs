
namespace Pjfb.Master {
	public partial class HuntEnemyPrizeMasterObject : HuntEnemyPrizeMasterObjectBase, IMasterObject {  
		// $type 区分（1 ⇒ 指名、2 ⇒ 初回、 11～ ⇒ 汎用、 12 ⇒ 非表示汎用
		public enum Type
		{
			Choice = 1,
			FirstTime = 2,
			General = 11,
			HiddenGeneral = 12,
		}

		public Type typeEnum => (Type) base.type;
	}

}
