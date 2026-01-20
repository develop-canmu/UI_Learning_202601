using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugRequirementLevelMasterVerAPIRequest : AppAPIRequestBase<DebugRequirementLevelMasterVerAPIPost, DebugRequirementLevelMasterVerAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugRequirementLevelMasterVerAPIResponse response ) {
        }
    }
}
