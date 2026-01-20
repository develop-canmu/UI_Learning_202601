using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaSubPointData
    {
        public long id{ get; private set; }
        public long subPointId{ get; private set; }
        public long value{ get; private set; }
        public long priority { get; private set; }

        public GachaSubPointData(TopSubPoint subPoint)
        {
            id = subPoint.id;
            subPointId = subPoint.mPointId;
            value = subPoint.value;
            priority = subPoint.priority;
        }
        
        public GachaSubPointData(long id, long subPointId, long value, long priority)
        {
            this.id = id;
            this.subPointId = subPointId;
            this.value = value;
            this.priority = priority;
        }
    }
}
