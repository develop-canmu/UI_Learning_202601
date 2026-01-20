using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGiftAllReceiveAPIRequest : AppAPIRequestBase<DebugGiftAllReceiveAPIPost, DebugGiftAllReceiveAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGiftAllReceiveAPIResponse response ) {
        }
    }
}
