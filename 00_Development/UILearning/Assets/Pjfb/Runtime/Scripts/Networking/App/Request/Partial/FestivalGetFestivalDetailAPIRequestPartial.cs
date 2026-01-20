using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class FestivalGetFestivalDetailAPIRequest : AppAPIRequestBase<FestivalGetFestivalDetailAPIPost, FestivalGetFestivalDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( FestivalGetFestivalDetailAPIResponse response ) {
        }
    }
}
