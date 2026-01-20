using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Logger = CruFramework.Logger;

namespace Pjfb.UserData {
    
    public partial class UserDataSupportEquipment
    {
        public List<PracticeSkillInfo> SubPracticeSkillDataList = new List<PracticeSkillInfo>();
        
        public CharaMasterObject MChara => MasterManager.Instance.charaMaster.FindData(charaId);

        public long Type => MChara.parentMCharaId;
    }
}