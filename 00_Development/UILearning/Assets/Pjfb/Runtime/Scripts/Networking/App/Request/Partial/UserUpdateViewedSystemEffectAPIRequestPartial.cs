using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserUpdateViewedSystemEffectAPIRequest : AppAPIRequestBase<UserUpdateViewedSystemEffectAPIPost, UserUpdateViewedSystemEffectAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserUpdateViewedSystemEffectAPIResponse response ) {
        }
    }
}
