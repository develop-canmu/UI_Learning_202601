
namespace Pjfb.Master {
	public partial class UserRankingSettingMasterObject : UserRankingSettingMasterObjectBase, IMasterObject {
		new public RankingClientPreviewMasterObject.TriggerType triggerType{get{return (RankingClientPreviewMasterObject.TriggerType)base.triggerType;}}
		new public RankingClientPreviewMasterObject.TargetType targetType{get{return (RankingClientPreviewMasterObject.TargetType)base.targetType;}}
	}

}
