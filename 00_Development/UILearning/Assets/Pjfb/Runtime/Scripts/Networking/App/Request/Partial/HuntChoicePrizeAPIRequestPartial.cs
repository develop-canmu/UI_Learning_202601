using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntChoicePrizeAPIRequest : AppAPIRequestBase<HuntChoicePrizeAPIPost, HuntChoicePrizeAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntChoicePrizeAPIResponse response ) {
        }
    }
}
