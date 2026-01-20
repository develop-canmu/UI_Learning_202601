using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class SupportEquipmentNameView : CharacterNameViewBase
    {
        [SerializeField] private SupportEquipmentIcon supportEquipmentIcon;

        public void InitializeUI(SupportEquipmentDetailData detailData)
        {
            InitializeUIAsync(detailData).Forget();
        }
        
        public void InitializeUI(UserDataSupportEquipment uSupportEquipment)
        {
            var detailData = new SupportEquipmentDetailData(uSupportEquipment);
            InitializeUIAsync(detailData).Forget();
        }
        
        public async UniTask InitializeUIAsync(SupportEquipmentDetailData detailData)
        {
            await supportEquipmentIcon.SetIconAsync(detailData);
            await InitializeUIByMCharaAsync(detailData.MChara);
            supportEquipmentIcon.SwipeableParams = null;
        }
    }
}