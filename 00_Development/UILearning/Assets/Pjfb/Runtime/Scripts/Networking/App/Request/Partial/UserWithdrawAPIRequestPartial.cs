using Pjfb.Networking.API;
using UnityEngine;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserWithdrawAPIRequest : AppAPIRequestBase<UserWithdrawAPIPost, UserWithdrawAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserWithdrawAPIResponse response ) {
            //アカウント削除
            AppManager.Instance.DeleteUserAccount();
        }
    }
}
