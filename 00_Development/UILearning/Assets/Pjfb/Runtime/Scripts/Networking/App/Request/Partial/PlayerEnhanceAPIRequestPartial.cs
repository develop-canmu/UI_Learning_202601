using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PlayerEnhanceAPIRequest : AppAPIRequestBase<PlayerEnhanceAPIPost, PlayerEnhanceAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PlayerEnhanceAPIResponse response ) {
        }
    }
}
