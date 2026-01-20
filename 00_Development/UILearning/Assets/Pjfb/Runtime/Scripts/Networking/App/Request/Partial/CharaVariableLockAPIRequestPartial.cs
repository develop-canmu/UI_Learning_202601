using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableLockAPIRequest : AppAPIRequestBase<CharaVariableLockAPIPost, CharaVariableLockAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableLockAPIResponse response ) {
        }
    }
}
