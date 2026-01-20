using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DeckEditAPIRequest : AppAPIRequestBase<DeckEditAPIPost, DeckEditAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DeckEditAPIResponse response ) {
        }
    }
}
