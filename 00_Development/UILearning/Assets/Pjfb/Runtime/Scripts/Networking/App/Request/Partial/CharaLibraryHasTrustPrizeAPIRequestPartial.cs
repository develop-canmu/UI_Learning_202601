using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaLibraryHasTrustPrizeAPIRequest : AppAPIRequestBase<CharaLibraryHasTrustPrizeAPIPost, CharaLibraryHasTrustPrizeAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaLibraryHasTrustPrizeAPIResponse response ) {
        }
    }
}
