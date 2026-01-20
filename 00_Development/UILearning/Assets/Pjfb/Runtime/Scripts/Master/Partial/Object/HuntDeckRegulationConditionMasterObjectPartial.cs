using Pjfb.Utility;

namespace Pjfb.Master {

	/// <summary> 編成条件の比較対象タイプ </summary>
	public enum HuntDeckRegulationLogicType
	{
		// 特定キャラクターが編成されているか
		CharacterParentRequire = 1,
	}
	
	public partial class HuntDeckRegulationConditionMasterObject : HuntDeckRegulationConditionMasterObjectBase, IMasterObject
	{
		public HuntDeckRegulationLogicType RegulationLogicType => (HuntDeckRegulationLogicType)conditionLogicType;
			
		public CompareOperationType CompareOperationType => CompareOperationUtility.OperationType(operatorType);
	}

}
