using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class SupportEquipmentDetailData
    {
        private long mCharaId = 0;
        public long MCharaId { get { return mCharaId; } }

        private long lv = 0;
        public long Lv { get { return lv; } }
        
        private long[] statusIdList;
        public long[] StatusIdList { get { return statusIdList; } }
        
        private long uSupportEquipmentId = -1;
        public  long USupportEquipmentId{get{return uSupportEquipmentId;}}

        public CharaMasterObject MChara => MasterManager.Instance.charaMaster.FindData(mCharaId);
        
        public SupportEquipmentDetailData(UserDataSupportEquipment uSupportEquipment)
        {
            this.uSupportEquipmentId = uSupportEquipment.id;
            this.mCharaId = uSupportEquipment.charaId;
            this.lv = uSupportEquipment.level;
            this.statusIdList = uSupportEquipment.lotteryProcessJson.statusList;
        }
        
        public SupportEquipmentDetailData(long mCharaId, long lv, long[] statusIdList)
        {
            this.mCharaId = mCharaId;
            this.lv = lv;
            this.statusIdList = statusIdList;
        }
        
        public SupportEquipmentDetailData(long uSupportEquipmentId, long mCharaId, long lv,  long[] statusIdList)
        {
            this.uSupportEquipmentId = uSupportEquipmentId;
            this.mCharaId = mCharaId;
            this.lv = lv;
            this.statusIdList = statusIdList;
        }
    }
}