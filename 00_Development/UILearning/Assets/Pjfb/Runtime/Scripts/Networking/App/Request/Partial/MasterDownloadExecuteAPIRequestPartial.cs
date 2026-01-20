using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MasterDownloadExecuteAPIRequest : AppAPIRequestBase<MasterDownloadExecuteAPIPost, MasterDownloadExecuteAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MasterDownloadExecuteAPIResponse response ) {
        }
    }
}
