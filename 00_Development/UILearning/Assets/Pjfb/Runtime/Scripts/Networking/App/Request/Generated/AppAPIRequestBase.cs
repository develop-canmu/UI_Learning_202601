//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public partial class AppAPIPostBase : IPostData {
      public long[] tutorialNumberList = null;
   }

   [Serializable]
   public partial class AppAPIResponseBase : IResponseData {
      public NativeApiItemContainer itemContainer = new NativeApiItemContainer(); // 各種ユーザーデータをまとめたもの
      public PrizeJsonWrap[] prizeJsonList = new PrizeJsonWrap[]{}; // アイテムの付与内容
      public NativeApiAutoSell autoSell = new NativeApiAutoSell(); // 自動売却したアイテムの内容
      public long[] finishedTutorialNumberList = new long[]{}; // 完了した初回チュートリアル一覧
   }


   public  abstract partial class AppAPIRequestBase<PostType, ResponseType> : JsonAPIRequest<PostType, ResponseType>
   where PostType : AppAPIPostBase 
   where ResponseType : AppAPIResponseBase, new() {
      
      protected abstract void OnAPIReceivedWithResponseType( ResponseType response );

      /// <summary>
      /// APIを受信した
      /// </summary>
      protected sealed override void OnAPIReceived( ResponseType response ) {
         UpdateTime(this);
         OnAPIReceivedWithResponseBase(response);
         OnAPIReceivedWithResponseType(response);
      }

      /// <summary>
      /// ローカルの時間更新
      /// </summary>
      protected void UpdateTime( IAPIRequest request ) {
         var response = request.GetRootResponse();
         AppTime.Reset(response.dateTime);
      }

   }
}