using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CombinationProgressBulkAPIRequest : AppAPIRequestBase<CombinationProgressBulkAPIPost, CombinationProgressBulkAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CombinationProgressBulkAPIResponse response ) {
        }
    }
}
