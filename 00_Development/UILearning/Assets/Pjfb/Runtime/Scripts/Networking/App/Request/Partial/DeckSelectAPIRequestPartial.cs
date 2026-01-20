using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DeckSelectAPIRequest : AppAPIRequestBase<DeckSelectAPIPost, DeckSelectAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DeckSelectAPIResponse response ) {
        }
    }
}
