using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CruFramework.Audio
{
    [FrameworkDocument("Audio", nameof(AudioManager), "Audio管理用クラス")]
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        // Local cache for audio group volume settings.
        private readonly Dictionary<string, float> _volumeSettings = new ();
        
        // BGM Time stamp
        private Dictionary<AudioPlayer, float> _bgmTimeStamps = new ();

        private AudioPlayer _activeBGMPlayer = default;

        [SerializeField]
        private AudioMixer audioMixer = null;
        
        private AudioClip stashBGMClip = null;
        private AudioPlayer[] bgmPlayer = new AudioPlayer[2];
        // キャッシュ
        private Dictionary<AudioGroup, List<AudioPlayer>> cache = new Dictionary<AudioGroup, List<AudioPlayer>>();

        /// <summary>BGM再生</summary>
        [FrameworkDocument("BGMを再生する")]
        public void PlayBGM(AudioClip clip, float volume = 1.0f, bool forcePlay = false)
        {
            PlayBGM(bgmPlayer[0], clip, volume, forcePlay);
        }

        public void PlayBGM(AudioPlayer player, AudioClip clip, float volume = 1.0f, bool forcePlay = false)
        {
            if(player.AudioSource.clip != clip || forcePlay)
            {
                player.audioSource.clip = clip;
                player.audioSource.volume = volume;
                _bgmTimeStamps[player] = Time.time;
                player.audioSource.Play();
                _activeBGMPlayer = player;
            }
        }

        public void StopBGM()
        {
            StopBGM(bgmPlayer[0]);
        }

        public void StopBGM(AudioPlayer player)
        {
            if (!_bgmTimeStamps.ContainsKey(player)) return;
            _bgmTimeStamps[player] = 0f;
            player.audioSource.Stop();
            _activeBGMPlayer = default;
        }
        
        /// <summary>BGM再生</summary>
        [FrameworkDocument("BGMを一時停止する")]
        public void PauseBGM()
        {
            AudioPlayer player = bgmPlayer[0];
            player.AudioSource.Pause();
        }
        
        /// <summary>BGM再生</summary>
        [FrameworkDocument("BGMを再生する")]
        public void PlayBGM()
        {
            AudioPlayer player = bgmPlayer[0];
            if(player.AudioSource.isPlaying == false)
            {
                player.AudioSource.Play();
                _activeBGMPlayer = player;
            }
        }

        public void ResumeBGM()
        {
            if (_activeBGMPlayer == default) return;
            var player = _activeBGMPlayer;
            
            if (player != default)
            {
                if (player.audioSource.isPlaying) return;
                var totalSeconds = Time.time - _bgmTimeStamps[player];
                var time = (float)totalSeconds % player.audioSource.clip.length;
                Logger.Log($"[AudioManager]@ResumeBGM diff:{totalSeconds} t:{time}");
                player.audioSource.time = time;
                player.audioSource.Play();
            }
        }

        /// <summary>BGMのフェードアウト</summary>
        [FrameworkDocument("BGMをフェードアウトする")]
        public async UniTask FadeOutBGM(float duration, CancellationToken token = default)
        {
            // フェードアウト
            await bgmPlayer[0].audioSource.DOFade(0, duration).ToUniTask(cancellationToken: token);
            // 停止
            _bgmTimeStamps[bgmPlayer[0]] = 0f;
            bgmPlayer[0].audioSource.Stop();
            
        }
        
        /// <summary>BGMのクロスフェード</summary>
        [FrameworkDocument("BGMをクロスフェードで再生する")]
        public async UniTask CrossFadeBGM(AudioClip clip, float duration, float volume = 1.0f, CancellationToken token = default)
        {
            // index0番目を再生するbgmにする
            AudioPlayer temp = bgmPlayer[0];
            bgmPlayer[0] = bgmPlayer[1];
            bgmPlayer[1] = temp;
            
            // 再生
            bgmPlayer[0].audioSource.volume = 0;
            bgmPlayer[0].audioSource.clip = clip;
            _bgmTimeStamps[bgmPlayer[0]] = Time.time;
            bgmPlayer[0].audioSource.Play();
            _activeBGMPlayer = bgmPlayer[0];
            
            // フェードイン
            UniTask fadeIn = bgmPlayer[0].audioSource.DOFade(volume, duration).ToUniTask(cancellationToken: token);
            // フェードアウト
            UniTask fadeOut = bgmPlayer[1].audioSource.DOFade(0, duration).ToUniTask(cancellationToken: token);
            // 待機
            await UniTask.WhenAll(fadeOut, fadeIn);

            // 停止
            _bgmTimeStamps[bgmPlayer[1]] = 0f;
            bgmPlayer[1].audioSource.Stop();
        }
        
        /// <summary>BGMを一時退避</summary>
        [FrameworkDocument("BGMを一時退避する")]
        public void StashBGM()
        {
            stashBGMClip = bgmPlayer[0].AudioSource.clip;
        }
        
        /// <summary>退避したBGMを再生する</summary>
        [FrameworkDocument("退避したBGMを再生する")]
        public void PlayStashBGM()
        {
            // スタッシュされてない or Nullがスタッシュされてる場合はBGMを止める
            if(stashBGMClip == null)
            {
                Stop(AudioGroup.BGM);
                return;
            }
            // スタッシュされたBGMを再生
            PlayBGM(stashBGMClip);
            stashBGMClip = null;
        }

        /// <summary>停止</summary>
        [FrameworkDocument("グループ単位でAudioを停止")]
        public void Stop(AudioGroup audioGroup)
        {
            if(cache.TryGetValue(audioGroup, out List<AudioPlayer> list))
            {
                foreach (AudioPlayer audioPlayer in list)
                {
                    audioPlayer.audioSource.Stop();
                    if (_bgmTimeStamps.ContainsKey(audioPlayer))
                    {
                        _bgmTimeStamps[audioPlayer] = 0f;
                    }
                }
            }
            _activeBGMPlayer = default;
        }
        
        /// <summary>一時停止</summary>
        [FrameworkDocument("グループ単位でAudioを一時停止")]
        public void Pause(AudioGroup audioGroup)
        {
            if(cache.TryGetValue(audioGroup, out List<AudioPlayer> list))
            {
                foreach (AudioPlayer audioPlayer in list)
                {
                    audioPlayer.audioSource.Pause();
                }
            }
        }
        
        /// <summary>一時停止解除</summary>
        [FrameworkDocument("グループ単位でAudioを一時停止解除")]
        public void UnPause(AudioGroup audioGroup)
        {
            if(cache.TryGetValue(audioGroup, out List<AudioPlayer> list))
            {
                foreach (AudioPlayer audioPlayer in list)
                {
                    audioPlayer.audioSource.UnPause();
                }
            }
        }
        
        /// <summary>ボリュームを0-1で設定</summary>
        [FrameworkDocument("グループ単位でボリュームを変更")]
        public bool SetVolume(AudioGroup audioGroup, float volume)
        {
            var groupName = $"{audioGroup.Name}Volume";
            
            // Store group volume setting in cache. 
            _volumeSettings[groupName] = volume;
            
            // dB値に変換
            float dB = Mathf.Clamp(Mathf.Log10(volume) * 20f,-80f,0f);
            var ret = audioMixer.SetFloat(groupName, dB);
            
            Logger.Log($"[AudioManager]@SetVolume {audioGroup.Name}Volume db:{dB} vol:{_volumeSettings[groupName]}");
            return ret;
        }
        
        /// <summary>SEプレイヤー生成</summary>
        [FrameworkDocument("SE再生プレイヤーを生成")]
        public AudioPlayer CreateSEPlayer(AudioClip clip)
        {
            return CreateAudioPlayer(AudioGroup.SE, clip);
        }
        
        /// <summary>Voiceプレイヤー生成</summary>
        [FrameworkDocument("Voice再生プレイヤーを生成")]
        public AudioPlayer CreateVoicePlayer(AudioClip clip)
        {
            return CreateAudioPlayer(AudioGroup.Voice, clip);
        }
        
        /// <summary>Bgmプレイヤー生成</summary>
        [FrameworkDocument("Bgm再生プレイヤーを生成")]
        public AudioPlayer CreateBGMPlayer(AudioClip clip)
        {
            AudioPlayer player = CreateAudioPlayer(AudioGroup.BGM, clip);
            player.AudioSource.loop = true;
            return player;
        }

        /// <summary>プレイヤーの生成</summary>
        public AudioPlayer CreateAudioPlayer(AudioGroup audioGroup, AudioClip clip)
        {
            // ミキサーグループ取得
            AudioMixerGroup mixerGroup = GetAudioMixerGroup(audioGroup);

            // プレイヤー生成
            AudioPlayer audioPlayer = new GameObject($"{audioGroup.Name}Player").AddComponent<AudioPlayer>();
            // グループ
            audioPlayer.audioGroup = audioGroup;
            // オーディオソース
            audioPlayer.audioSource = audioPlayer.gameObject.AddComponent<AudioSource>();
            audioPlayer.audioSource.playOnAwake = false;
            // 出力ミキサー
            audioPlayer.audioSource.outputAudioMixerGroup = mixerGroup;
            // クリップ設定
            audioPlayer.audioSource.clip = clip;
            // マネージャの子供にする
            audioPlayer.transform.SetParent(transform);
            
            // キーが存在しない
            if(!cache.ContainsKey(audioGroup))
            {
                cache.Add(audioGroup, new List<AudioPlayer>());
            }
            // キャッシュに追加
            cache[audioGroup].Add(audioPlayer);
            
            return audioPlayer;
        }
        
        [FrameworkDocument("プレイヤーを返却する")]
        public void ReleaseAudioPlayer(AudioPlayer audioPlayer)
        {
            // キャッシュから削除
            cache[audioPlayer.audioGroup].Remove(audioPlayer);
            // ゲームオブジェクト削除
            Destroy(audioPlayer.gameObject);
        }

        /// <summary>ミキサーグループを取得</summary>
        private AudioMixerGroup GetAudioMixerGroup(AudioGroup audioGroup)
        {
            AudioMixerGroup[] mixerGroups = audioMixer.FindMatchingGroups(audioGroup.Name);
            return mixerGroups.Length == 0 ? null : mixerGroups[0];
        }

        protected override void OnAwake()
        {
            // Re-Set volume based on volume cache.
            // When audio device changed, audioMixer seems tobe reset on some mobile phone.
            AudioSettings.OnAudioConfigurationChanged += changed =>
            {
                //Resume BGM
                ResumeBGM();
                
                var bgm = _volumeSettings[$"{AudioGroup.BGM.Name}Volume"];
                var se = _volumeSettings[$"{AudioGroup.SE.Name}Volume"];
                var voice = _volumeSettings[$"{AudioGroup.Voice.Name}Volume"];
                SetVolume(AudioGroup.BGM, bgm);
                SetVolume(AudioGroup.SE, se);
                SetVolume(AudioGroup.Voice, voice);
            };
            DontDestroyOnLoad(gameObject);
            
            // BGM再生用
            for(int i = 0; i < bgmPlayer.Length; i++)
            {
                bgmPlayer[i] = CreateAudioPlayer(AudioGroup.BGM, null);
                bgmPlayer[i].audioSource.loop = true;
            }
        }
    }
}
