using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PlayerGetEnhanceListAPIRequest : AppAPIRequestBase<PlayerGetEnhanceListAPIPost, PlayerGetEnhanceListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PlayerGetEnhanceListAPIResponse response ) {
        }
    }
}
