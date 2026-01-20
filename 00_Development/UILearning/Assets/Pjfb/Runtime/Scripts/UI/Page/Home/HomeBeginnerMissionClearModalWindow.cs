using Cysharp.Threading.Tasks;

namespace Pjfb.Home
{
    public class HomeBeginnerMissionClearModalWindow : ModalWindow
    {
        #region InnerClass
        #endregion
        
        #region SerializeFields
        #endregion

        #region PrivateFields
        #endregion

        #region StaticMethods
        public static async UniTask<CruFramework.Page.ModalWindow> OpenAsync()
        {
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.HomeBeginnerMissionClear, args: null);
        }
        #endregion
        
        #region OverrideMethods
        #endregion

        #region PrivateMethods
        #endregion
        
        #region EventListeners
        public void OnClickIdleCollider()
        {
            Close();
        }
        #endregion
    }
}
