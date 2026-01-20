using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaGetChoiceDetailAPIRequest : AppAPIRequestBase<GachaGetChoiceDetailAPIPost, GachaGetChoiceDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaGetChoiceDetailAPIResponse response ) {
        }
    }
}
