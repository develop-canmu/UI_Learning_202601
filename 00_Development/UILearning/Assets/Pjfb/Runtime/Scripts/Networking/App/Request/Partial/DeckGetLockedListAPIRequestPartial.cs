using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DeckGetLockedListAPIRequest : AppAPIRequestBase<DeckGetLockedListAPIPost, DeckGetLockedListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DeckGetLockedListAPIResponse response ) {
        }
    }
}
