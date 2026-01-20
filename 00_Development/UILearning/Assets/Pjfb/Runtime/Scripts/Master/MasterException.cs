using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.API;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.HTTP;
using Pjfb.Storage;
using CruFramework.Extensions;

namespace Pjfb.Master {
    public class MasterException : System.Exception {
        public string message{get; private set;}
        public MasterException( string message ) : base(message) {
            this.message  = message;
        }
    }
}