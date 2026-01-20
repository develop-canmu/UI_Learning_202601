using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DeckGetListAPIRequest : AppAPIRequestBase<DeckGetListAPIPost, DeckGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DeckGetListAPIResponse response ) {
        }
    }
}
