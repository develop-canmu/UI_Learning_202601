using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaLibraryReceivePrizeAPIRequest : AppAPIRequestBase<CharaLibraryReceivePrizeAPIPost, CharaLibraryReceivePrizeAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaLibraryReceivePrizeAPIResponse response ) {
        }
    }
}
