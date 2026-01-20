using System;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    [Serializable]
    public class GachaLocalSaveData {
        public List<GachaPickUpSaveData> lastCheckDate = new List<GachaPickUpSaveData>();
    }

    [Serializable]
    public class GachaPickUpSaveData {
        public long choiceId = 0;
        public long elementId = 0;
        public string lastConfirmedDate = "";
        public List<long> confirmedPrizeIds = new List<long>();
    }
}
