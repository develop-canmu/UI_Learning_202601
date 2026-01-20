using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Voice;

namespace Pjfb.Character
{
    public class GrowthLiberationEffectView : MonoBehaviour
    {
        [Serializable]
        public enum EffectType
        {
            Growth,
            Liberation
        }
        
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private IconImage iconImage;
        [SerializeField] private TMPro.TMP_Text currentLvText;
        [SerializeField] private TMPro.TMP_Text afterLvText;
        [SerializeField] private Animator animator;
        [SerializeField] private EffectType effectType;

        private Func<UniTask> onCloseAction;
        private bool isOnceAnim;
        
        public void InitializeUi(long mCharaId, long currentLv, long afterLv, Func<UniTask> onClose = null)
        {
            isOnceAnim = false;
            if (characterCardImage != null) characterCardImage.SetTexture(mCharaId);
            if (iconImage != null) iconImage.SetTexture(mCharaId);
            currentLvText.text = currentLv.ToString();
            afterLvText.text = afterLv.ToString();
            onCloseAction = onClose;
            animator.SetTrigger("Open");
            PlaySystemVoice(mCharaId).Forget();
        }

        public void OnClickNextButton()
        {
            // 一度アニメーションが再生されていた場合は抜ける
            if (isOnceAnim) return;
            animator.SetTrigger("Close");
            // アニメーションが再生されたらフラグを立てる
            isOnceAnim = true;
            onCloseAction?.Invoke().Forget();
        }

        private async UniTask PlaySystemVoice(long mCharaId)
        {
            var mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            if(mChara == null) return;
            VoiceResourceSettings.LocationType locationType; 
            switch (effectType)
            {
                case EffectType.Growth:
                    locationType = VoiceResourceSettings.LocationType.SYSTEM_LV_UP;
                    break;
                case EffectType.Liberation:
                    locationType = VoiceResourceSettings.LocationType.SYSTEM_REARITY_UP;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(mChara, locationType);
        }
    }
}