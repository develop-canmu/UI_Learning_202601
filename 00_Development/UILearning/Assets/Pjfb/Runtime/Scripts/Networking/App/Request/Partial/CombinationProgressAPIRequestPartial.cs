using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CombinationProgressAPIRequest : AppAPIRequestBase<CombinationProgressAPIPost, CombinationProgressAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CombinationProgressAPIResponse response ) {
        }
    }
}
