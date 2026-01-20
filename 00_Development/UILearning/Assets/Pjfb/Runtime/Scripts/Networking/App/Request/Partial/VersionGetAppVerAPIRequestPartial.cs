using Pjfb.Networking.API;
using UnityEngine;
using UnityEngine.AddressableAssets.Initialization;

namespace Pjfb.Networking.App.Request {
    
    public partial class VersionGetAppVerAPIRequest : AppAPIRequestBase<VersionGetAppVerAPIPost, VersionGetAppVerAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( VersionGetAppVerAPIResponse response ) {
            if( response.preview == Application.version ) {
                APIManager.Instance.setting.baseURL = APIManager.Instance.setting.reviewURL;
                AppEnvironment.IsReview = true;
            } else {
                APIManager.Instance.setting.baseURL = APIManager.Instance.setting.apiURL;
                AppEnvironment.IsReview = false;
            }
            // 動的に変換する変数がキャッシュされているのでクリアする(AppEnvironment.AssetURL用)
            AddressablesRuntimeProperties.ClearCachedPropertyValues();
        }
    }
}
