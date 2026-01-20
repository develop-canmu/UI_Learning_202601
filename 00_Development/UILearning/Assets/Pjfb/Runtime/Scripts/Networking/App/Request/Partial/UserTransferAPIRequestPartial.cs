using Pjfb.Networking.API;
using UnityEngine;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserTransferAPIRequest : AppAPIRequestBase<UserTransferAPIPost, UserTransferAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserTransferAPIResponse response ) {
            AppManager.Instance.DeleteUserAccount();
            LocalSaveUtility.CreateImmutableSaveData(response.appToken);
        }
    }
}
