using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Audio;
using UnityEngine;
using CruFramework.Utils;
using CruFramework.ResourceManagement;
using CruFramework.Addressables;
using Cysharp.Threading.Tasks;
using Pjfb.InGame;
using Pjfb.Master;
using Logger = CruFramework.Logger;
using Random = UnityEngine.Random;

namespace Pjfb.Voice
{
    public class VoiceManager : Singleton<VoiceManager>
    {
        
        #region const
        private const string VOICE_EXTENSION = ".ogg";
        private const string VOICE_SETTING_KEY = "Voice/VoiceResourceSettings.asset";
        private const int AUDIO_PLAYER_COUNT = 2;
        #endregion

        private List<AudioPlayer> voicePlayerList = new List<AudioPlayer>();
        private readonly ResourcesLoader resourcesLoader = new ResourcesLoader();
        private readonly Dictionary<string, AudioClip> cacheAudioClips = new Dictionary<string, AudioClip>();
        private StringBuilder pathBuilder;
        private CancellationTokenSource source;
        private VoiceResourceSettings voiceSettings;
        private int lastPlayIndex = 0;

        protected override void Init()
        {
            for (var i = 0; i < AUDIO_PLAYER_COUNT; i++)
            {
                var voicePlayer = AudioManager.Instance.CreateVoicePlayer(null);
                voicePlayer.AudioSource.clip = null;
                voicePlayerList.Add(voicePlayer);
            }
            pathBuilder = new StringBuilder();
            base.Init();
        }

        private async UniTask InitVoiceSetting()
        {
            var handle = AddressablesManager.LoadAssetAsync<VoiceResourceSettings>(VOICE_SETTING_KEY);
            await AddressablesManager.WaitHandle(handle);
            voiceSettings = handle.Result;
        }

        #region ボイス再生 ボイスの種別と通し番号が判明している場合
        // 例) ★1潔のレア度アップ1を再生したい sys_0001_0018.ogg
        //   var chara = MasterManager.Instance.charaMaster.FindData(1);
        //   VoiceManager.Instance.PlaySystemVoiceForUsageType(chara,18);
        public void PlayPartVoiceForIndex(CharaMasterObject mChara, int useType)
        {
            PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType.Part, mChara, useType).Forget();
        }
        
        public void PlayInVoiceForIndex(CharaMasterObject mChara, int useType)
        {
            PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType.In, mChara, useType).Forget();
        }

        public void PlaySystemVoiceForIndex(CharaMasterObject mChara, int useType)
        {
            PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType.System, mChara, useType).Forget();
        }
        
        public void PlayInSpVoiceForIndex(CharaMasterObject mChara, int useType)
        {
            PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType.InSp, mChara, useType).Forget();
        }
        
        public async UniTask PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType type, CharaMasterObject mChara, int useType)
        {
            if (voiceSettings == null)
            {
                await InitVoiceSetting();
            }
            var personalId = mChara.GetPersonalId();
            var personalUniqueId = mChara.GetPersonalUniqueId();
            var path = voiceSettings.GetVoicePathForIndex(type, personalId, useType, personalUniqueId);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            await PlayVoiceAsync(path);
        }
        
        #endregion
        
        #region ボイス再生 ボイスの種別だけわかっており、同じ用途のボイスをどれか1つ再生したい場合
        // 例) ★1潔のホーム画面タップボイス10種の中からどれか1つランダムで再生したい
        //   var chara = MasterManager.Instance.charaMaster.FindData(1);
        //   VoiceManager.Instance.PlaySystemVoiceForUsageType(chara,VoiceResourceSettings.UsageType.SYSTEM_TAP);
        public async UniTask PlayPartVoiceForLocationTypeAsync(CharaMasterObject mChara, VoiceResourceSettings.LocationType locationType)
        {
            await PlayVoiceForLocationTypeAsync(VoiceResourceSettings.VoiceType.Part, mChara, locationType);
        }
        
        public async UniTask PlayInVoiceForLocationTypeAsync(CharaMasterObject mChara, VoiceResourceSettings.LocationType locationType)
        {
            await PlayVoiceForLocationTypeAsync(VoiceResourceSettings.VoiceType.In, mChara, locationType);
        }

        public async UniTask PlaySystemVoiceForLocationTypeAsync(CharaMasterObject mChara, VoiceResourceSettings.LocationType locationType)
        {
            await PlayVoiceForLocationTypeAsync(VoiceResourceSettings.VoiceType.System, mChara, locationType);
        }
        
        public async UniTask PlayInSpVoiceForLocationTypeAsync(CharaMasterObject mChara, VoiceResourceSettings.LocationType locationType)
        {
            await PlayVoiceForLocationTypeAsync(VoiceResourceSettings.VoiceType.InSp, mChara, locationType);
        }
        
        public async UniTask PlayVoiceForLocationTypeAsync(VoiceResourceSettings.VoiceType type, CharaMasterObject mChara, VoiceResourceSettings.LocationType locationType)
        {
            if (voiceSettings == null)
            {
                await InitVoiceSetting();
            }
            var path = voiceSettings.GetVoicePathForUsageType(type, mChara, locationType);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            PlayVoiceAsync(path).Forget();
        }
        
        #endregion
        
        #region ボイス再生 ボイスのファイル名が判明している場合
        // 例) シナリオやイベントデータからファイル名が直指定されている
        //   VoiceManager.Instance.PlayVoiceAsync("original_tutorial_0001_0001").Forget();
        public async UniTask PlayVoiceAsync(string voiceName)
        {
            if (cacheAudioClips.ContainsKey(voiceName))
            {
                PlayVoice(cacheAudioClips[voiceName]);
                return;
            }

            var assetPath = GetVoicePath(voiceName);

            if (source != null)
            {
                source.Cancel();
            }

            source = new CancellationTokenSource();
            try
            {
                var clip = await resourcesLoader.LoadAssetAsync<AudioClip>(assetPath, source.Token);
                if (clip != null)
                {
                    cacheAudioClips[voiceName] = clip;
                    PlayVoice(clip);   
                }
                source = null;
            }
            catch (Exception e)
            {
                CruFramework.Logger.Log($" cancel playing voice : {assetPath} " + e.Message);
                throw;
            }
        }
        
        #endregion

        #region ボイス再生 キャラ名鑑のマスタから再生を行う場合

        private async UniTask<string> GetVoicePathByCharaLibraryVoiceAsync(CharaLibraryVoiceMasterObject mCharaLibraryVoice)
        {
            if (mCharaLibraryVoice == null)
            {
                return string.Empty;
            }

            var path = "";
            var type = (VoiceResourceSettings.VoiceType)mCharaLibraryVoice.voiceType;
            if (type == VoiceResourceSettings.VoiceType.Scenario)
            {
                var id = mCharaLibraryVoice.locationType;
                var useType = mCharaLibraryVoice.useType;
                path = voiceSettings.GetVoicePathForScenarioId(id, useType);
            }
            else
            {
                var mChara = MasterManager.Instance.charaMaster.FindData(mCharaLibraryVoice.masterId);
                if (mChara == null)
                {
                    return string.Empty;
                }
                
                var masterType = mCharaLibraryVoice.masterType;
                var personalId = mChara.GetPersonalId();
                var personalUniqueId = masterType == 1 ? 0 : mChara.GetPersonalUniqueId();
                var useType = mCharaLibraryVoice.useType;
                
                if (voiceSettings == null)
                {
                    await InitVoiceSetting();
                }
                path = voiceSettings.GetVoicePathForIndex(type, personalId, useType, personalUniqueId);
            }
            return path;
        }
        public async UniTask PlayVoiceForCharaLibraryVoiceAsync(CharaLibraryVoiceMasterObject mCharaLibraryVoice)
        {
            if (voiceSettings == null)
            {
                await InitVoiceSetting();
            }

            var path = await GetVoicePathByCharaLibraryVoiceAsync(mCharaLibraryVoice);
            
            if (string.IsNullOrEmpty(path)) return;

            await PlayVoiceAsync(path);
            await WaitPlayAsync();
        }
        #endregion
 
        private string GetVoicePath(string voiceName)
        {
            pathBuilder.Clear();
            pathBuilder.Append(voiceName);
            pathBuilder.Append(VOICE_EXTENSION);
            return pathBuilder.ToString();
        }

        private AudioPlayer GetVoicePlayer()
        {
            foreach (var player in voicePlayerList)
            {
                if (!player.AudioSource.isPlaying) return player;
            }
            // 万が一埋まってたら最後に使用したplayerとは別のものを適当に
            lastPlayIndex++;
            if (lastPlayIndex >= voicePlayerList.Count) lastPlayIndex = 0;
            return voicePlayerList[lastPlayIndex];
        }

        public void PlayVoice(AudioClip clip)
        {
            if (clip == null) return;
            
            var voicePlayer = GetVoicePlayer();
            
            if (voicePlayer == null) return;
            
            voicePlayer.AudioSource.Stop();
            voicePlayer.AudioSource.clip = clip;
            voicePlayer.PlayOneShot();
        }

        public void StopVoice()
        {
            foreach (var player in voicePlayerList)
            {
                player.AudioSource.Stop();
                player.AudioSource.clip = null;
            }
        }

        public bool IsPlaying()
        {
            return voicePlayerList.Any(player => player.AudioSource.isPlaying);
        }
        
        public async UniTask WaitPlayAsync()
        {
            await UniTask.WaitWhile(IsPlaying);
        }

        public void SetVolume(float volume)
        {
            foreach (var player in voicePlayerList)
            {
                player.AudioSource.volume = volume;
            }
        }

        public void ReleaseAllAsset()
        {
            StopVoice();
            resourcesLoader.Release();
            cacheAudioClips.Clear();
            pathBuilder.Clear();
        }

        public async UniTask<List<CharaLibraryVoiceMasterObject>> GetCharaLibraryVoiceListAsync(CharaMasterObject mChara)
        {
            if (voiceSettings == null)
            {
                await InitVoiceSetting();
            }
            
            var id = mChara.id;
            var parentMCharaId = mChara.parentMCharaId;
            var voiceDictionary = new Dictionary<long, CharaLibraryVoiceMasterObject>();

            var mCharaLibraryVoiceList = MasterManager.Instance.charaLibraryVoiceMaster?.values;

            if (mCharaLibraryVoiceList == null)
            {
                return new List<CharaLibraryVoiceMasterObject>();
            }

            foreach (var voice in mCharaLibraryVoiceList)
            {
             
                if (voice.useType == 0) continue;
                
                if (voice.releaseTrustLevel < 0) continue;

                if (voice.masterType == 1 && voice.masterId != parentMCharaId ||
                    voice.masterType == 2 && voice.masterId != id)
                {   
                    continue;
                }
                
                var key = voice.voiceType * 1000 + voice.useType;
                
                if (voiceDictionary.ContainsKey(key))
                {
                    if (voice.masterType > voiceDictionary[key].masterType)
                    {
                        voiceDictionary[key] = voice;
                    }
                }
                else
                {
                    voiceDictionary[key] = voice;
                }
            }
            return voiceDictionary.Values.ToList();
        }
        
        public async UniTask DebugPlayVoice(long charaLibraryVoiceId)
        {
#if !PJFB_REL         
            if (voiceSettings == null)
            {
                await InitVoiceSetting();
            }

            var master = MasterManager.Instance.charaLibraryVoiceMaster.FindData(charaLibraryVoiceId);

            var body = "";
            var path = "";
            
            path = await GetVoicePathByCharaLibraryVoiceAsync(master);
            
            if (master == null)
            {
                body = $"id:{charaLibraryVoiceId}がマスタに登録されていません";
            }
            else if (string.IsNullOrEmpty(path))
            {
                body = $"VoiceSettingにマスタに対応するボイスが登録されていません";
            }
            else
            {
                body = $"{path}<br>{master.name}";
                PlayVoiceAsync(path).Forget();
            }
            
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            await ConfirmModalWindow.OpenAsync(new ConfirmModalData(
                "ボイスデバッグ", body,"",
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    window =>
                    {
                        window.Close();
                    })),new CancellationToken());
#endif
        }

    }   
}

