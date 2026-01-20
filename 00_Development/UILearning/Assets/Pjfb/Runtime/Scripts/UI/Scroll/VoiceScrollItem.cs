using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Voice;
using TMPro;
using UnityEngine;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb.Encyclopedia
{
    public class VoiceScrollData
    {
        public VoiceScrollData(CharaLibraryVoiceMasterObject voiceData, bool isLocked)
        {
            VoiceData = voiceData;
            IsLocked = isLocked;
        }

        public readonly bool IsLocked;
        public readonly CharaLibraryVoiceMasterObject VoiceData;
    }
    
    public class VoiceScrollItem : ScrollDynamicItem
    {
        [SerializeField] private TextMeshProUGUI voiceNameText;
        [SerializeField] private GameObject lockRoot;
        [SerializeField] private TextMeshProUGUI lockText;
        private VoiceScrollData scrollData;

        protected override void OnSetView(object value)
        {
            scrollData = (VoiceScrollData)value;
            voiceNameText.text = scrollData.VoiceData.name;
            lockRoot.SetActive(scrollData.IsLocked);
            if (scrollData.IsLocked)
                lockText.text = string.Format(StringValueAssetLoader.Instance["character.voice_lock"], scrollData.VoiceData.releaseTrustLevel);
            
            // 自動レイアウトの強制計算
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
        }

        public void PlayVoice()
        {
            VoiceManager.Instance.StopVoice();
            VoiceManager.Instance.PlayVoiceForCharaLibraryVoiceAsync(scrollData.VoiceData).Forget();
        }
        
    }
}