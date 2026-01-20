using System.Collections;
using System.Collections.Generic;
using CruFramework.Audio;
using UnityEngine;

namespace CruFramework.Adv
{
    public interface IAdvSpeaker
    {
        void OnStopVoice();
        void OnPlayVoice(AudioPlayer audioPlayer);
    }
}
