using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Voice;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameAdviserSkillActivationEffect : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private AbilityNameImage abilityNameImage;
        [SerializeField] private TMP_Text useMessageText;
        
        private const string OpenTrigger = "Open";

        public async UniTask PlayAnimation(long mCharaId , long abilityId , string useMessage)
        {
            await UniTask.WhenAll(
                characterCardImage.SetTextureAsync(mCharaId),
                abilityNameImage.SetTextureAsync(abilityId)
            );
            
            // アニメーション時に音声を再生
            CharaLibraryVoiceMasterObject charaLibraryVoice = MasterManager.Instance.charaLibraryVoiceMaster.GetDataByAbilityId(abilityId);
            if (charaLibraryVoice != null)
            {
                VoiceManager.Instance.PlayVoiceForCharaLibraryVoiceAsync(charaLibraryVoice).Forget();
            }
            else 
            {
                CruFramework.Logger.LogError($"CharaLibraryVoiceMasterObject not found for abilityId: {abilityId}");
            }
            
            gameObject.SetActive(true);
            animator.SetTrigger(OpenTrigger);
            useMessageText.text = useMessage;
        }

    }
}