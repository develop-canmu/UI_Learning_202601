using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class LogRegisterLogAPIRequest : AppAPIRequestBase<LogRegisterLogAPIPost, LogRegisterLogAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( LogRegisterLogAPIResponse response ) {
        }
    }
}
