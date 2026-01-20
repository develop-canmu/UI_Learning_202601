using System;
using Pjfb.Networking.API;

namespace Pjfb.Editor.Master {
   
   [Serializable]
   public class SpecificationGetMasterClassSchemaListAPIPost : IPostData {
   }

   [Serializable]
   public class SpecificationGetMasterClassSchemaListAPIResponse : IResponseData {
      public object[] shcemaList = null;
   }
      
   public partial class SpecificationGetMasterClassSchemaListAPIRequest : JsonAPIRequest<SpecificationGetMasterClassSchemaListAPIPost, SpecificationGetMasterClassSchemaListAPIResponse> {
      public override string apiName{get{ return "specification/getMasterClassSchemaList"; } }
   }
}