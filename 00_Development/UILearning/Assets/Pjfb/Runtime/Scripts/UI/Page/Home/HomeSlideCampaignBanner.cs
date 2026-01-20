using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.Utility;
using UnityEngine;

namespace Pjfb.Home
{
    public class HomeSlideCampaignBanner : ScrollGridItem
    {
        #region Parameters
        public class Parameters
        {
            public NewsBanner bannerData;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private CancellableWebTexture image;
        #endregion

        #region PrivateFields
        private Parameters parameters;
        #endregion

        #region OverrideMethods
        protected override void OnSetView(object value)
        {
            parameters = (Parameters) value;
            image.SetTexture($"{AppEnvironment.AssetBrowserURL}/{parameters.bannerData.imagePath}");
        }
        #endregion

        #region PublicMethods
        public void OnClickBanner()
        {
            ServerActionCommandUtility.ProceedAction(parameters.bannerData.onClick);
        }
        #endregion
    }
}