using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class VersionGetAssetVerAPIRequest : AppAPIRequestBase<VersionGetAssetVerAPIPost, VersionGetAssetVerAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( VersionGetAssetVerAPIResponse response ) {
            APIManager.Instance.setting.latestAssetVersion = response.assetVer;
        }
    }
}
