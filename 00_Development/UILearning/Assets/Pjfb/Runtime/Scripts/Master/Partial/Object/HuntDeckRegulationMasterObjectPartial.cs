
using System;

namespace Pjfb.Master {
	
	// 条件タイプ
	public enum HuntDeckRegulationType
	{
		// 条件なし
		None = 0,
		// 弱体化条件
		Weaken = 1,
		// 試合開始条件
		BattleStart = 2,
	}
	
	public partial class HuntDeckRegulationMasterObject : HuntDeckRegulationMasterObjectBase, IMasterObject {  
		
		/// <summary> 編成条件タイプの取得 </summary>
		public HuntDeckRegulationType HuntConditionType => (HuntDeckRegulationType)condtionCompleteBonusType;
	}

}
