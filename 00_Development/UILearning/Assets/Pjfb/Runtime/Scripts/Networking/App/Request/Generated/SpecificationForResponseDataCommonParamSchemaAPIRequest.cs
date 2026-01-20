//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
レスポンスのデータ部の汎用パラメータである itemContainer, prizeJsonList の構造をクライアントに共有するためのダミーAPI

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class SpecificationForResponseDataCommonParamSchemaAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class SpecificationForResponseDataCommonParamSchemaAPIResponse : AppAPIResponseBase {

   }
      
   public partial class SpecificationForResponseDataCommonParamSchemaAPIRequest : AppAPIRequestBase<SpecificationForResponseDataCommonParamSchemaAPIPost, SpecificationForResponseDataCommonParamSchemaAPIResponse> {
      public override string apiName{get{ return "specification/forResponseDataCommonParamSchema"; } }
   }
}