using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DeckNameChangeAPIRequest : AppAPIRequestBase<DeckNameChangeAPIPost, DeckNameChangeAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DeckNameChangeAPIResponse response ) {
        }
    }
}
