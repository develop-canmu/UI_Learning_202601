using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class VersionGetVariousVerAPIRequest : AppAPIRequestBase<VersionGetVariousVerAPIPost, VersionGetVariousVerAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( VersionGetVariousVerAPIResponse response ) {
            APIManager.Instance.setting.latestAssetVersion = response.assetVer;
        }
    }
}
