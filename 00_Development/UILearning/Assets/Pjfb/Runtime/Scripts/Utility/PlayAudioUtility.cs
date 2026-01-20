using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Runtime.Scripts.Utility
{
    public class PlayAudioUtility : MonoBehaviour
    {
        [SerializeField, Header("重複再生可能か")] private bool canDuplicatePlayback = true;
        [SerializeField, Header("重複の許容閾値(この秒数以上であれば再生される)")] private float duplicateAllowableTime = 0.1f;
        [SerializeField] private List<AudioClip> audioClip;

        public void PlayAudio(int idx = 0)
        {
            var isValid = audioClip.Count != 0 && audioClip.Count > idx && audioClip[idx] != null;
            if (!isValid) return;
            SEManager.PlaySE(audioClip[idx], canDuplicatePlayback, duplicateAllowableTime);
        }
    }
}