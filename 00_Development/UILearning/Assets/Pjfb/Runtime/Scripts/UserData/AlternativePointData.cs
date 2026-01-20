using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.UserData
{
    public class AlternativePointData
    {
        private long mPointId = 0;
        // 仮想ポイントのユーザーデータ情報
        public UserDataPoint UserData { get => UserDataManager.Instance.point.Find(mPointId); }

        // 仮想ポイントのマスタ情報
        private PointAlternativeMasterObject alternativeMaster;
        public PointAlternativeMasterObject AlternativeMasterObject{get => alternativeMaster;}

        public AlternativePointData(long mPointId, PointAlternativeMasterObject alternativeMaster)
        {
            this.mPointId = mPointId;
            this.alternativeMaster = alternativeMaster;
        }
    }
}