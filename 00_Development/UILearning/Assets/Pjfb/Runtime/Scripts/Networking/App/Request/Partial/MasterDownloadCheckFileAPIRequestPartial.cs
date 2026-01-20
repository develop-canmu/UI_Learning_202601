using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MasterDownloadCheckFileAPIRequest : AppAPIRequestBase<MasterDownloadCheckFileAPIPost, MasterDownloadCheckFileAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MasterDownloadCheckFileAPIResponse response ) {
        }
    }
}
