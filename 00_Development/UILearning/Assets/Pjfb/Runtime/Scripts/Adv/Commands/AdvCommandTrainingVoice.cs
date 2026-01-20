
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Voice;

namespace Pjfb.Adv
{
    [System.Serializable]
    public class AdvCommandTrainingVoice : IAdvCommand
    {
        
        public enum VoiceType
        {
            Rest
        }
        
        [SerializeField]
        [AdvDocument("再生するボイスタイプ。")]
        private VoiceType voiceType = VoiceType.Rest;

        void IAdvCommand.Execute(AdvManager manager)
        {
            TrainingAdvManagerExtension extension = manager.GetComponent<TrainingAdvManagerExtension>();
            if(extension == null)return;
            
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(extension.TrainingMainPage.CurrentTrainingData.TrainingCharacter.MCharId);

            switch(voiceType)
            {
                case VoiceType.Rest:
                    VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(mChar, VoiceResourceSettings.LocationType.SYSTEM_TRAINING_REST).Forget();
                    break;
            }
        }        
    }
}
