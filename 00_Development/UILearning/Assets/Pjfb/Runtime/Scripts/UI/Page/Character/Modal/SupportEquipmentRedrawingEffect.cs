
using System;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class SupportEquipmentRedrawingEffect : MonoBehaviour
    {
        [SerializeField] 
        private SupportEquipmentIcon prevSupportEquipmentIcon;
        
        [SerializeField]
        private SupportEquipmentIcon currentSupportEquipmentIcon;
        
        [SerializeField] 
        private Animator animator;
        
        [SerializeField]
        private CallAnimationEventAction callAnimationEventAction;
        
        public void Init(SupportEquipmentRedrawingEffectModal.Data modalData, Action closeAnimationEventAction)
        {
            prevSupportEquipmentIcon.SetIconAsync(modalData.PrevUserDataSupportEquipment).Forget();
            currentSupportEquipmentIcon.SetIconAsync(modalData.CurrentUserDataSupportEquipment).Forget();
            callAnimationEventAction.AnimationEventAction = () => closeAnimationEventAction?.Invoke();
        }
        
        public void Open()
        {
            animator.Play("Open", 0, 0);
        }
        
        public async UniTask Close()
        {
            await AnimatorUtility.WaitStateAsync(animator, "Close");
        }
    }
}