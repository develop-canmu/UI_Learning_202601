using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using UnityEngine;
using UnityEngine.UI;

using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.Adv
{
    public class AdvMessageLog : ScrollGridItem
    {
        [SerializeField]
        private IconImage icon = null;
        [SerializeField]
        private RubyTextMeshProUGUI messageText = null;
        
        [SerializeField]
        private RubyTextMeshProUGUI speakerText = null;
        
        protected override void OnSetView(object value)
        {
            AdvMessageLogData m = (AdvMessageLogData)value;
            messageText.UnditedText = m.Message;
            speakerText.UnditedText = m.Speaker;
            
            
            // アイコン
            if(m.Manager.Config.CharacterDatas.HasValue(m.SpeakerId))
            {
                AdvCharacterDataId characterId = m.Manager.Config.CharacterDatas[m.SpeakerId];
                if(string.IsNullOrEmpty(characterId.IconId) == false)
                {                
                    SetTextureAsync(characterId.IconId).Forget();
                }
                else
                {
                    icon.Cancel();
                    icon.gameObject.SetActive(false);
                }
            }
            else
            {
                icon.Cancel();
                icon.gameObject.SetActive(false);
            }
        }

        private async UniTask SetTextureAsync(string iconId)
        {
            await icon.SetTextureAsync( PageResourceLoadUtility.GetCharacterAdvIconImagePath(iconId));
            icon.gameObject.SetActive(true);
        }
        
    }
}
