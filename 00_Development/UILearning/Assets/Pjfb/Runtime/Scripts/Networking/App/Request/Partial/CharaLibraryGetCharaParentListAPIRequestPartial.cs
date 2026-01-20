using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaLibraryGetCharaParentListAPIRequest : AppAPIRequestBase<CharaLibraryGetCharaParentListAPIPost, CharaLibraryGetCharaParentListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaLibraryGetCharaParentListAPIResponse response ) {
        }
    }
}
