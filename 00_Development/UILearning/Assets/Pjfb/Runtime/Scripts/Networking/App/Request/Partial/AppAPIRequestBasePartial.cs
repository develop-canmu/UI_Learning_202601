using System;
using Pjfb.Networking.API;
using Pjfb.UserData;

namespace Pjfb.Networking.App.Request {

   
   public partial class AppAPIPostBase : IPostData {
   }

   public partial class AppAPIResponseBase : IResponseData {
      
   }


   public  abstract partial class AppAPIRequestBase<PostType, ResponseType> : JsonAPIRequest<PostType, ResponseType>
   where PostType : AppAPIPostBase 
   where ResponseType : AppAPIResponseBase, new() {
      
      /// <summary>
      /// APIを受信した際の共通処理
      /// </summary>
      protected void OnAPIReceivedWithResponseBase( AppAPIResponseBase response ) {
         if( response.itemContainer != null ) {
            // UserGetDataAPI の場合はフル更新として扱う
            UserDataManager.Instance.UpdateByResponseData(response.itemContainer, response is UserGetDataAPIResponse);
            if(response.itemContainer.title != null) AppManager.Instance.UIManager.Header.UpdateMenuBadge();
         }
         if( response.finishedTutorialNumberList != null) {
             UserDataManager.Instance.UpdateFinishTutorialNumber(response.finishedTutorialNumberList);
         }
      }

   }
}