using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableSellAPIRequest : AppAPIRequestBase<CharaVariableSellAPIPost, CharaVariableSellAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableSellAPIResponse response ) {
        }
    }
}
