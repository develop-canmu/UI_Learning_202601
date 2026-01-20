//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
新規タイトル立ち上げ時、ページ同期の動作確認をする際に使うAPI

Crz7環境構築手順 - 本番環境：ページ同期の動作確認

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugPageDeployTestAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugPageDeployTestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugPageDeployTestAPIRequest : AppAPIRequestBase<DebugPageDeployTestAPIPost, DebugPageDeployTestAPIResponse> {
      public override string apiName{get{ return "debug/pageDeployTest"; } }
   }
}