using System;
using Pjfb.Networking.API;

namespace Pjfb.Editor.API {
   
   [Serializable]
   public class SpecificationGetApiSchemaAPIPost : IPostData {
      public string apiName = null;
   }

   [Serializable]
   public class SpecificationGetApiSchemaAPIResponse : IResponseData {
      
   }
      
   public partial class SpecificationGetApiSchemaAPIRequest : JsonAPIRequest<SpecificationGetApiSchemaAPIPost, SpecificationGetApiSchemaAPIResponse> {
      public override string apiName{get{ return "specification/getApiSchema"; } }
   }
}