
namespace Pjfb.Master {
	public partial class TrainingAutoCostMasterObject : TrainingAutoCostMasterObjectBase, IMasterObject {  
		
		public enum CostType
		{
			/// <summary>即時完了</summary>
			CompleteImmediately = 1,
			/// <summary>時短</summary>
			Shortening = 2,
			/// <summary>残り回数増加</summary>
			AutoTrainingRemainingTimesAdd = 3,
			/// <summary>同時実行枠解放</summary>
			FrameRelease = 4,
		}

		public new CostType type
		{
			get
			{
				return (CostType)base.type;
			}
		}
		
	}

}
