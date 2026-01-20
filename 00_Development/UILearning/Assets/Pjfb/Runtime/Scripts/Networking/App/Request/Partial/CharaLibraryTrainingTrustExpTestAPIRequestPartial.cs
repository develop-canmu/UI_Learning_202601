using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaLibraryTrainingTrustExpTestAPIRequest : AppAPIRequestBase<CharaLibraryTrainingTrustExpTestAPIPost, CharaLibraryTrainingTrustExpTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaLibraryTrainingTrustExpTestAPIResponse response ) {
        }
    }
}
