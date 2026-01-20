using System;
using Pjfb.Networking.API;

namespace Pjfb.Editor.API {
   
   [Serializable]
   public class SpecificationGetApiNameListAPIPost : IPostData {

   }

   [Serializable]
   public class SpecificationGetApiNameListAPIResponse : IResponseData {
      public string[] apiNameList = null;
   }
      
   public partial class SpecificationGetApiNameListAPIRequest : JsonAPIRequest<SpecificationGetApiNameListAPIPost, SpecificationGetApiNameListAPIResponse> {
      public override string apiName{get{ return "specification/getApiNameList"; } }
   }
}