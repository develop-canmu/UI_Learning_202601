using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugRequirementLevelAppVerAPIRequest : AppAPIRequestBase<DebugRequirementLevelAppVerAPIPost, DebugRequirementLevelAppVerAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugRequirementLevelAppVerAPIResponse response ) {
        }
    }
}
