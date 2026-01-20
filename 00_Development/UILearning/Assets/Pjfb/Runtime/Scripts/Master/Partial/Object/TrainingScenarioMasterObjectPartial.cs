
using System;

namespace Pjfb.Master {
	
	/// <summary> トレーニングシナリオタイプ </summary>
	public enum TrainingScenarioType
	{
		// 通常
		Normal = 1,
		// コンセントレーション
		Concentration = 2,
		// Flow
		Flow = 11,
	}
	
	public partial class TrainingScenarioMasterObject : TrainingScenarioMasterObjectBase, IMasterObject {
		public bool IsTutorial()
		{
			// チュートリアルシナリオチェック(通常時の表示から除外)
			return isTutorial == 1;
		}
		/// <summary> シナリオタイプの取得 </summary>
		public TrainingScenarioType ScenarioType => (TrainingScenarioType)type;
		
		public bool IsEnabledTrainingAuto()
		{
			// 自動トレーニング機能が有効かどうか
			return enabledTrainingAuto == 1;
		}
	}

}
