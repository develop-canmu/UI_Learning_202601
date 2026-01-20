using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using CruFramework.Extensions;

namespace Pjfb.Networking.API {
    public abstract class JsonAPIRequest<PostType, ResponseType> : APIRequest<PostType, ResponseType>
    where PostType : class, IPostData 
    where ResponseType : class, IResponseData, new()  {
        protected override IAPISerializer<PostType,ResponseType> CreateSerializer(){
            return new JsonSerializer<PostType, ResponseType>();
        }       
    }
}