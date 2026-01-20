using Logger = CruFramework.Logger;

namespace Pjfb.Beginner
{
    public static class BeginnerManager
    {
        #region PublicProperties
        // TODO: ホーム画面のバッジ
        public static bool showHomeBadge = true;
        #endregion

        #region StaticMethod
        /// <summary>
        /// ホーム画面にある初心者がクリックされた時に実行される
        /// </summary>
        public static void OnClickBeginnerButton()
        {
            Logger.Log("BeginnerManager.OnClickBeginnerButton");
            // TODO: 該当画面画面へ飛ばすなど
        }
        #endregion
    }
}