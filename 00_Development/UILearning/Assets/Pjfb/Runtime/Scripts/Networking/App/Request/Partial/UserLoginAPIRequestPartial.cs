using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserLoginAPIRequest : AppAPIRequestBase<UserLoginAPIPost, UserLoginAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserLoginAPIResponse response ) {
            APIManager.Instance.setting.sessionId = response.sessionId;
            APIManager.Instance.setting.loginId = response.loginId;
            AdjustManager.TrackLoginEvent(response.createdAt,response.lastLoginAt);
        }
    }
}
