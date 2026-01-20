using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MasterDownloadCheckAPIRequest : AppAPIRequestBase<MasterDownloadCheckAPIPost, MasterDownloadCheckAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MasterDownloadCheckAPIResponse response ) {
            
        }
    }
}
