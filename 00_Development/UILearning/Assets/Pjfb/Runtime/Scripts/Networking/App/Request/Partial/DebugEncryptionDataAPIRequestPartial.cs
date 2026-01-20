using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugEncryptionDataAPIRequest : AppAPIRequestBase<DebugEncryptionDataAPIPost, DebugEncryptionDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugEncryptionDataAPIResponse response ) {
        }
    }
}
