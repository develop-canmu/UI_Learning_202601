using Pjfb.Storage;

namespace Pjfb.RecommendChara
{
    public static class RecommendCharaManager
    {
        public static bool showHomeBadge => LocalSaveManager.saveData.RecommendCharaCheckNotificationData.ShouldShowNotificationBadge();
    }
}
