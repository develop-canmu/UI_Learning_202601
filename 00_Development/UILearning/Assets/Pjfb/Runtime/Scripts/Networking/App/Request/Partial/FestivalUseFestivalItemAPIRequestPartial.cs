using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class FestivalUseFestivalItemAPIRequest : AppAPIRequestBase<FestivalUseFestivalItemAPIPost, FestivalUseFestivalItemAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( FestivalUseFestivalItemAPIResponse response ) {
        }
    }
}
