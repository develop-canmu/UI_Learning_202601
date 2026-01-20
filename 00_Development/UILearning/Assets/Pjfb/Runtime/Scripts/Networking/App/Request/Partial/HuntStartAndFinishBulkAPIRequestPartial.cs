using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntStartAndFinishBulkAPIRequest : AppAPIRequestBase<HuntStartAndFinishBulkAPIPost, HuntStartAndFinishBulkAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntStartAndFinishBulkAPIResponse response ) {
        }
    }
}
