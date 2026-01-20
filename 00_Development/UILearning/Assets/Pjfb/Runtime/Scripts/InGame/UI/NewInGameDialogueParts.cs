using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Voice;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    public class NewInGameDialogueParts : MonoBehaviour
    {
        [SerializeField] private  GameObject parent;
        [SerializeField] private  CharacterInGameIconImage icon;
        [SerializeField] private  GameObject[] frames;
        [SerializeField] private  TextMeshProUGUI message;
        
        private BattleDialogueData data = null;

        public bool IsPlayedVoice { get; private set; }

        public void CancelPlayVoice()
        {
            IsPlayedVoice = false;
            VoiceManager.Instance.StopVoice();
        }

        public void PlayDialog(BattleDialogueData _data)
        {
            IsPlayedVoice = false;
            data = _data;
            if (data == null || string.IsNullOrEmpty(data.master.message))
            {
                parent.SetActive(false);
                return;
            }
            
            parent.SetActive(true);

            message.text =  data.master.message;
            
            var mChara = MasterManager.Instance.charaMaster.FindData(data.charaId);
            icon.SetTextureAsync(mChara.id).Forget();
        }

        public async void PlayVoice(Action onEndPlayCallback)
        {
            if (data?.master == null)
            {
                onEndPlayCallback?.Invoke();
                return;
            }

            IsPlayedVoice = true;
            await VoiceManager.Instance.PlayVoiceForCharaLibraryVoiceAsync(data.master);
            onEndPlayCallback?.Invoke();
        }
    }
}