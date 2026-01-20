using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaGetListAPIRequest : AppAPIRequestBase<CharaGetListAPIPost, CharaGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaGetListAPIResponse response ) {
        }
    }
}
