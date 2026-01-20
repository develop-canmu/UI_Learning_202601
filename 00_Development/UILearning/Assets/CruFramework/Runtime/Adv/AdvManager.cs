using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using CruFramework.Addressables;
using CruFramework.Audio;
using CruFramework.ResourceManagement;
using UnityEngine;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.UI;
using CruFramework.Page;

namespace CruFramework.Adv
{
    
    public enum AdvSkipMode
    {
        Skip,
        AllForceSkip,
    }
    
    public enum AdvAutoMode
    {
        None = 0, 
        Auto = 1,
        Fast = 2,
    }

    
    public enum AdvBgmUsage
    {
        UseAudioManagerAndStash, UseAudioManager, UseAdvManager
    }

    
    public abstract class AdvManager : MonoBehaviour
    {
        [SerializeField]
        private AdvConfig config = null;
        /// <summary>COnfig</summary>
        public AdvConfig Config{get{return config;}}
        
        [SerializeField]
        private bool isKeepLog = false;
        /// <summary>ログを保持する</summary>
        public bool IsKeepLog{get{return isKeepLog;}set{isKeepLog = value;}}
        
        [SerializeField]
        private AdvBgmUsage bgmUsage = AdvBgmUsage.UseAudioManagerAndStash;
        /// <summary>BGM</summary>
        public AdvBgmUsage BgmUsage{get{return bgmUsage;}set{bgmUsage = value;}}
        
        [SerializeField]
        private AdvSelectRoot selectRoot = null;
        /// <summary>選択肢</summary>
        public AdvSelectRoot SelectRoot{get{return selectRoot;}}
        
        [SerializeField]
        private GameObject world = null;
        /// <summaryワールド</summary>
        public GameObject World{get{return world;}}
        
        private bool isPlaying = false;
        /// <summary>再生中</summary>
        public bool IsPlaying{get{return isPlaying;}}
        
        // メッセージウィンドウ
        private Dictionary<int, AdvMessageWindow> messageWindows = new Dictionary<int, AdvMessageWindow>();
        // レイヤー
        private Dictionary<int, AdvObjectLayer> objectLayers = new Dictionary<int, AdvObjectLayer>();
        // テクスチャ表示オブジェクト
        private Dictionary<int, AdvTextureObject> textureObjects = new Dictionary<int, AdvTextureObject>();
        // フェード
        private Dictionary<int, AdvFade> fades = new Dictionary<int, AdvFade>();
        // テキスト
        private Dictionary<int, AdvTextObject> texts = new Dictionary<int, AdvTextObject>();
        
        private Dictionary<string, string> messageReplaceList = new Dictionary<string, string>();
        /// <summary>文字列置換リスト</summary>
        public IReadOnlyDictionary<string, string> MessageReplaceList{get{return messageReplaceList;}}

        private bool isStopCommand = false;
        /// <summary>コマンドの停止</summary>
        public bool IsStopCommand{get{return isStopCommand;}set{isStopCommand = value;}}
        
#if CRUFRAMEWORK_DEBUG || UNITY_EDITOR
        private bool isDebugMode = false;
        /// <summary>デバッグモード/summary>
        public bool IsDebugMode{get{return isDebugMode;}set{isDebugMode = value;}}
#endif
        
        // クリック待ち
        private bool isWaitClick = false;
        // 時間待機
        private float waitTime = 0;
        
        private bool isEnded = false;
        /// <summary>終了している</summary>
        public bool IsEnded{get{return isEnded;}}
        
        
        private float autoMessage = 0;
        /// <summary>
        /// メッセージの自動送り　0以下指定で自動なし 待機時間（秒）
        /// </summary>
        public float AutoMessage{get{return autoMessage;}set{autoMessage = value;}}
        
        
        private AdvAutoMode autoMode = AdvAutoMode.None;
        /// <summary>自動送り</summary>
        public AdvAutoMode AutoMode{get{return autoMode;}set{autoMode = value;}}
        
        /// <summary>早送りモード？</summary>
        public bool IsFastMode{get{return autoMode == AdvAutoMode.Fast;}}
        
        /// <summary>スキップ中か早送り中</summary>
        public bool IsSkipOrFastMode{get{return IsFastMode || IsSkip;}}
        
        // 自動送り用のタイマー
        private float autoMessageTimer = 0;
        
        private List<AdvMessageLogData> messageLogs = new List<AdvMessageLogData>();
        /// <summary>メッセージログ</summary>
        public IReadOnlyList<AdvMessageLogData> MessageLogs{get{return messageLogs;}}
        
        private bool isPause = false;
        /// <summary>一時停止</summary>
        public  bool IsPause{get{return isPause;}set{isPause = value;}}
        
        private bool isSkip = false;
        /// <summary>スキップ中</summary>
        public bool IsSkip{get{return isSkip;}}

        /// <summary>コマンド実行中</summary>
        public bool IsExecuteCommand
        {
            get
            {
                return
                    isPause == false &&
                    isEnded == false && 
                    isWaitClick == false &&
                    isStopCommand == false &&
                    waitTime <= 0 &&
                    CanExecuteCommand() && 
                    (executeCommands != null && executeCommands.Length > executeCommandIndex);
            }
        }
        
        /// <summary>コマンド実行可能？</summary>
        protected virtual  bool CanExecuteCommand(){return true;}
        
        private IAdvCommandObject[] executeCommands = null;
        /// <summary>実行中のコマンドリスト</summary>
        public IAdvCommandObject[] ExecuteCommands{get{return executeCommands;}}
        
        private int executeCommandIndex = 0;
        /// <summary>実行位置</summary>
        public int ExecuteCommandIndex{get{return executeCommandIndex;}set{executeCommandIndex = value;}}
        
        private ResourcesLoader resourceLoader = new ResourcesLoader();

        // 条件Id
        private AdvCommandNextCase nextCase = null;
        
        // キャラクタの管理
        private Dictionary<int, AdvObject> advCharacters = new Dictionary<int, AdvObject>();
        // オブジェクトの管理
        private Dictionary<int, List<AdvObject>> advObjects = new Dictionary<int, List<AdvObject>>();
        private Dictionary<ulong, AdvObject> advCreateObjects = new Dictionary<ulong, AdvObject>();
        // 保持している値
        private Dictionary<string, object> values = new Dictionary<string, object>();
        
        // Seのキャッシュ
        private Dictionary<AudioClip, AudioPlayer> soundCache = new Dictionary<AudioClip, AudioPlayer>();

        private AudioPlayer voicePlayer = null;
        /// <summary>ボイス再生用</summary>
        public AudioPlayer VoicePlayer{get{return voicePlayer;}}
        
        private AudioPlayer bgmPlayer = null;

        //　現在喋っているオブジェクト    
        private AdvCharacter currentSpeaker = null;
        
        private IAdvCommandObject currentCommand = null;

        public event Action<IAdvCommandObject> OnExecuteCommand = null;
        public event Action<string> OnError = null;

        protected virtual void OnAwake(){}
        protected virtual void OnStart(){}
        protected virtual void OnUpdate(){}
        
        /// <summary>スキップボタンの有効化</summary>
        protected virtual void OnSkipButtonEnable(bool isEnable){}
        
        protected virtual void OnLoadFile(string path){}

        public event Action OnEnded = null;
        
        /// <summary>リソースのパスを取得</summary>
        public string GetResourcePath(int pathId, int dataId)
        {
            return Config.ResourcePaths[pathId].Path.Replace("{id}", dataId.ToString());
        }
        
        /// <summary>リソースのパスを取得</summary>
        public string GetResourcePath(int pathId, string dataId)
        {
            return Config.ResourcePaths[pathId].Path.Replace("{id}", dataId);
        }
        
        /// <summary>Configファイルのパス</summary>
        protected abstract string GetConfigPath();

        /// <summary>リソースのパスを取得</summary>
        public string GetResourcePath(int pathId)
        {
            return Config.ResourcePaths[pathId].Path;
        }
        
        /// <summary>リソースの読み込み</summary>
        public UniTask PreLoadResourceAsync(string path)
        {
            return AddressablesManager.DownloadDependenciesAsync(path);
        }
        
        /// <summary>リソースの読み込み</summary>
        public UniTask PreLoadResourceAsync(int pathId)
        {
            return AddressablesManager.DownloadDependenciesAsync( GetResourcePath(pathId) );
        }
        
        /// <summary>リソースの読み込み</summary>
        public UniTask PreLoadResourceAsync(int pathId, int dataId)
        {
            return AddressablesManager.DownloadDependenciesAsync( GetResourcePath(pathId, dataId) );
        }
        
        /// <summary>リソースの読み込み</summary>
        public UniTask PreLoadResourceAsync(int pathId, string dataId)
        {
            return AddressablesManager.DownloadDependenciesAsync( GetResourcePath(pathId, dataId) );
        }

        /// <summary>リソースの読み込み</summary>
        public UniTask<T> LoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAssetAsync<T>(path);
        }
        
        /// <summary>リソースの読み込み</summary>
        public UniTask<T> LoadResourceAsync<T>(int pathId) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAssetAsync<T>( GetResourcePath(pathId) );
        }
        
        /// <summary>リソースの読み込み</summary>
        public UniTask<T> LoadResourceAsync<T>(int pathId, int dataId) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAssetAsync<T>( GetResourcePath(pathId, dataId) );
        }
        
        /// <summary>リソースの読み込み</summary>
        public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAsset<T>(path);
        }
        
        /// <summary>リソースの読み込み</summary>
        public T LoadResource<T>(int pathId) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAsset<T>( GetResourcePath(pathId) );
        }
        
        /// <summary>リソースの読み込み</summary>
        public T LoadResource<T>(int pathId, int dataId) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAsset<T>( GetResourcePath(pathId, dataId) );
        }
        
        /// <summary>リソースの読み込み</summary>
        public T LoadResource<T>(int pathId, string dataId) where T : UnityEngine.Object
        {
            return resourceLoader.LoadAsset<T>( GetResourcePath(pathId, dataId) );
        }

        /// <summary>クリックまで待機</summary>
        public void WaitClick()
        {
            isWaitClick = true;
        }
        
        /// <summary>指定時間停止</summary>
        public void SetWaitTime(float time)
        {
            waitTime = time;
        }
        
        public void ForceNextCommand()
        {
            isWaitClick = false;
            isStopCommand = false;
            waitTime = 0;
        }
        
        /// <summary>置換リストを削除</summary>
        public void ClearMessageReplaceString()
        {
            messageReplaceList.Clear();
        }
        
        /// <summary>置換リストを追加</summary>
        public void AddMessageReplaceString(string key, string value)
        {
            if(messageReplaceList.ContainsKey(key))
            {
                messageReplaceList[key] = value;
            }
            else
            {
                messageReplaceList.Add(key, value);
            }
        }
        
        /// <summary>値を設定</summary>
        public void SetValue(string key, object value)
        {
            if(values.ContainsKey(key) == false)
            {
                values.Add(key, value);
            }
            else
            {
                values[key] = value;
            }
        }
        
        /// <summary>値を取得</summary>
        public object GetValue(string key)
        {
            if( values.TryGetValue(key, out object value))
            {
                return value;
            }
            
            return null;
        }
        
        /// <summary>値を取得</summary>
        public T GetValue<T>(string key)
        {
            object value = GetValue(key);
            return value != null ? (T)value : default;
        }

        /// <summary>オブジェクトの取得</summary>
        public T GetAdvCharacter<T>(int id) where T : AdvObject
        {
            if(advCharacters.TryGetValue(id, out AdvObject advObject) == false)
            {
                ErrorLog($"存在しないIdにアクセスしました [{id} : {Config.CharacterDatas[id].Name}]");
                return null;
            }
            
            return (T)advObject;
        }
        
        /// <summary>BGMの再生</summary>
        public void PlayBgm(int key, int id, float volume)
        {
            PlayBgm(key, id.ToString(), volume);
        }
        
        /// <summary>BGMの再生</summary>
        public void PlayBgm(int key, string id, float volume)
        {
            AudioClip clip = LoadResource<AudioClip>(key, id);
            
            switch(BgmUsage)
            {
                case AdvBgmUsage.UseAudioManager:
                case AdvBgmUsage.UseAudioManagerAndStash:
                    AudioManager.Instance.PlayBGM(clip, volume, true);
                    break;
                
                case AdvBgmUsage.UseAdvManager:
                    AudioManager.Instance.PauseBGM();
                    AudioManager.Instance.PlayBGM(bgmPlayer, clip);
                    break;
            }
        }
        
        /// <summary>BGMの再生</summary>
        public void StopBgm()
        {
            switch(BgmUsage)
            {
                case AdvBgmUsage.UseAudioManager:
                case AdvBgmUsage.UseAudioManagerAndStash:
                    AudioManager.Instance.Stop(AudioGroup.BGM);
                    break;
                
                case AdvBgmUsage.UseAdvManager:
                    AudioManager.Instance.PauseBGM();
                    AudioManager.Instance.StopBGM(bgmPlayer);
                    break;
            }
        }
        
        /// <summary>Seの再生</summary>
        public void PlaySe(int key, int id, float volume)
        {
            PlaySe(key, id.ToString(), volume);
        }
        
        /// <summary>Seの再生</summary>
        public void PlaySe(int key, string id, float volume)
        {
            // Clipを取得
            AudioClip clip = LoadResource<AudioClip>(key, id);
            // キャッシュをチェック
            if(soundCache.TryGetValue(clip, out AudioPlayer p) == false) 
            {
                p = AudioManager.Instance.CreateSEPlayer(clip);
                p.AudioSource.volume = volume;
                soundCache.Add(clip, p);
            }
            
            p.PlayOneShot();
        }
        
        /// <summary>Seの再生</summary>
        public void StopSe(int key, string id)
        {
            // Clipを取得
            AudioClip clip = LoadResource<AudioClip>(key, id);
            // キャッシュをチェック
            if(soundCache.TryGetValue(clip, out AudioPlayer p)) 
            {
                p.AudioSource.Stop();
            }
        }
        
        /// <summary>ボイスの再生</summary>
        public void PlayVoice(AdvCharacter speaker, int key, string id)
        {
            if(currentSpeaker != speaker)
            {
                if(currentSpeaker is IAdvSpeaker s)
                {
                    s.OnStopVoice();
                }
                currentSpeaker = speaker;
            }
            // クリップの読み込み
            AudioClip clip = LoadResource<AudioClip>(key, id);
            // クリップの登録
            voicePlayer.AudioSource.clip = clip;
            // 再生
            voicePlayer.AudioSource.Play();
            
            // オブジェクトに通知
            {
                if(speaker is IAdvSpeaker s)
                {
                    s.OnPlayVoice(VoicePlayer);
                }
            }
        }
        
        /// <summary>文字列値観葉</summary>
        public string ReplaceText(string text)
        {
            // 文字列の置換
            foreach(KeyValuePair<string, string> replace in MessageReplaceList)
            {
                text = text.Replace(replace.Key, replace.Value);
            }
            return text;
        }
        
        /// <summary>ログデータ</summary>
        public void AddMessageLog(string speaker, string message, int speakerId)
        {
            // 最大保存数をチェック
            if(messageLogs.Count >= Config.MaxLogCount)
            {
                messageLogs.RemoveAt(0);
            }
            // リストに追加
            messageLogs.Add(new AdvMessageLogData(this, speaker, message, speakerId));
        }

        // PositionIdのワールド座標
        public Vector3 GetWorldPosition(int positionId, Vector3 currentPosition, Vector3 offset)
        {
            bool hasPositionValue = Config.Positions.HasValue(positionId);
            // 設定値を取得
            Vector3 p = (hasPositionValue ? Config.Positions[positionId].Position : Vector3.zero) + offset;
            // Transform
            RectTransform worldTransform = (RectTransform)world.transform;
            p = (Vector3)(worldTransform.localToWorldMatrix * p);
            
            if(hasPositionValue == false) 
            {
                p += currentPosition;
            }
            else
            {
                p += worldTransform.position;
            }
            return p;
        }
        
        /// <summary>生成</summary>
        public void CreateCharacter(int id, int positionId, bool isActive)
        {
            // 重複したオブジェクトを生しようとしている
            if(advCharacters.ContainsKey(id))
            {
                ErrorLog($"重複したIdをCreateしようとしました [{id} : {Config.CharacterDatas[id].Name}]");
                return;
            }
            
            // パスを取得
            AdvCharacterDataId characterId = Config.CharacterDatas[id];
            

            // リソース読み込み
            AdvCharacter prefab = LoadResource<AdvCharacter>(Config.CharacterPrefabResourcePathId);
            // オブジェクトを生成
            AdvCharacter advObject = GameObject.Instantiate<AdvCharacter>(prefab, GetObjectLayer(Config.CharacterLayerId).transform);
            // Id
            advObject.SetId(id);
            // リソースの読み込み
            if(string.IsNullOrEmpty(characterId.ResourceId) == false)
            {
                advObject.LoadResource(this, characterId.ResourceId);
            }

            // リストに追加
            if(advObject != null)
            {
                advCharacters.Add(id, advObject );
                if(Config.Positions.HasValue(positionId))
                {
                    advObject.transform.localPosition = Config.Positions[positionId].Position;
                }
                advObject.SetName( config.CharacterDatas[id].Name );
                advObject.Data = config.CharacterDatas[id];
                advObject.gameObject.SetActive(isActive);
            }
            else
            {
                ErrorLog($"CreateCharacterに失敗 [{id} : {Config.CharacterDatas[id].Name}]");
            }
        }
        
        public void GrayoutCharacters()
        {
            foreach(AdvCharacter c in advCharacters.Values)
            {
                c.Grayout();
            }
        }
        
        public void HighlightCharacters()
        {
            foreach(AdvCharacter c in advCharacters.Values)
            {
                c.Highlight();
            }
        }
        
        /// <summary>生成</summary>
        public AdvObject CreateObject(ulong createId, int objectDataId, int layerId, int positionId, Vector3 offset)
        {
            
            AdvObject advObject = null;
            
            // キャッシュから取得
            if(advObjects.TryGetValue(objectDataId, out List<AdvObject> cacheList) == false)
            {
                cacheList = new List<AdvObject>();
                advObjects.Add(objectDataId, cacheList);
            }
            
            foreach(AdvObject obj in cacheList)
            {
                if(obj.gameObject.activeSelf == false)
                {
                    obj.gameObject.SetActive(true);
                    advObject = obj;
                    break;
                }
            }
            
            // パスを取得
            AdvObjectDataId objectId = Config.ObjectDatas[objectDataId];
            AdvObjectCategoryId categoryId = Config.ObjectCategories[objectId.Category];
            
            // オブジェクトを生成
            if(advObject == null)
            {
                // リソース読み込み
                AdvObject prefab = LoadResource<AdvObject>(categoryId.ResourceKey, objectId.ResourceId);
                advObject = GameObject.Instantiate<AdvObject>(prefab, GetObjectLayer(layerId).transform);
            }
            // Id
            advObject.SetId(objectDataId);
            advObject.SetCreateId(createId);
            // リストに追加
            if(advObject != null)
            {
                cacheList.Add(advObject);
                advObject.gameObject.SetActive(true);
                advObject.SetName( config.ObjectDatas[objectDataId].Name );
                
                // 座標
                advObject.transform.localPosition = config.Positions[positionId].Position + offset;
                // 生成Idをキーに保存
                advCreateObjects.Add(createId, advObject);
            }
            else
            {
                ErrorLog($"CreateObjectに失敗 [{objectDataId} : {Config.ObjectDatas[objectDataId].Name}]");
            }
            
            return advObject;
        }
        
        /// <summary>メッセージウィンドウを取得</summary>
        public AdvMessageWindow GetMessageWindow(int id)
        {
            if(messageWindows.TryGetValue(id, out AdvMessageWindow w) == false)
            {
                ErrorLog($"存在しないMessageWubdiwを指定しました。 [{id} : {Config.MessageWindows[id].Value}]");
                return null;
            }
            return w;
        }
        
        /// <summary>メッセージウィンドウを取得</summary>
        public AdvObjectLayer GetObjectLayer(int id)
        {
            if(objectLayers.TryGetValue(id, out AdvObjectLayer layer) == false)
            {
                ErrorLog($"存在しないLayerを指定しました。 [{id} : {Config.ObjectLayers[id].Value}]");
                return null;
            }
            return layer;
        }
        
        /// <summary>テクスチャオブジェクトを取得</summary>
        public AdvTextureObject GetTextureObject(int id)
        {
            if(textureObjects.TryGetValue(id, out AdvTextureObject texture) == false)
            {
                ErrorLog($"存在しないTextureIdを指定しました。 [{id} : {Config.Textures[id].Name}]");
                return null;
            }
            return texture;
        }
        
        public AdvObject GetObject(ulong id)
        {
            if(advCreateObjects.TryGetValue(id, out AdvObject advObject) == false)
            {
                ErrorLog($"存在しないObjectIdを指定しました。 " + id);
                return null;
            }
            return advObject;
        }
        
        /// <summary>フェードを取得</summary>
        public AdvFade GetFade(int id)
        {
            if(fades.TryGetValue(id, out AdvFade fade) == false)
            {
                ErrorLog($"存在しないFadeIdを指定しました。 [{id} : {Config.Fades[id].Name}]");
                return null;
            }
            return fade;
        }
        
        /// <summary>テキストを取得</summary>
        public AdvTextObject GetText(int id)
        {
            if(texts.TryGetValue(id, out AdvTextObject text) == false)
            {
                ErrorLog($"存在しないTextIdを指定しました。 [{id} : {Config.Texts[id].Name}]");
                return null;
            }
            return text;
        }
        
        /// <summary>キャラクタの削除</summary>
        public void DeleteCharacter(int id)
        {
            // 存在しないオブジェクトを消そうとしている
            if(advCharacters.TryGetValue(id, out AdvObject advObject) == false)
            {
                ErrorLog($"存在しないIdをDeleteしようとしました [{id} : {Config.CharacterDatas[id].Name}]");
                return;
            }

            // GameObjectの削除
            GameObject.Destroy(advObject.gameObject);
            // リストから削除
            advCharacters.Remove(id);
        }
        
        /// <summary>オブジェクトの削除</summary>
        public void DeleteObject(int id)
        {
            // 存在しないオブジェクトを消そうとしている
            if(advObjects.TryGetValue(id, out List<AdvObject> advObject) == false)
            {
                ErrorLog($"存在しないIdをDeleteしようとしました [{id} : {Config.ObjectDatas[id].Name}]");
                return;
            }

            // オブジェクトの削除
            for(int i=advObject.Count-1;i>=0;i--)
            {
                if(advObject[i].CreateId != 0)
                {
                    DeleteObject(advObject[i].CreateId);
                }
                GameObject.Destroy(advObject[i].gameObject);
            }

            // リストから削除
            advObjects.Remove(id);
        }
        
        /// <summary>オブジェクトの削除</summary>
        public void DeleteObject(ulong createId)
        {
            // 存在しないオブジェクトを消そうとしている
            if(advCreateObjects.TryGetValue(createId, out AdvObject advObject) == false)
            {
                ErrorLog($"存在しないIdをDeleteしようとしました [{createId}]");
                return;
            }

            for(int i=advObject.Children.Count-1;i>=0;i--)
            {            
                AdvObject child = advObject.Children[i];
            
                switch(child)
                {
                    case AdvCharacter v:
                    {
                        v.SetTransformParent( GetObjectLayer(Config.CharacterLayerId).transform );
                        v.gameObject.SetActive(false);
                        break;
                    }
                    
                    case AdvEffect v:
                    {
                        DeleteObject(v.CreateId);
                        break;
                    }
                    
                    default:
                        ErrorLog("定義されてない : " + child.GetType());
                        break;
                }
            }
            
            // キャッシュしているの悪意ティブを切る
            advObject.gameObject.SetActive(false);
            advObject.SetCreateId(0);
            // 生成済みリストから削除
            advCreateObjects.Remove(createId);
        }
        
        /// <summary>全てのオブジェクトを破棄する</summary>
        public void DeleteAllObjects()
        {
            List<int> characterIds = new List<int>(advCharacters.Keys);
            for(int i=0;i<characterIds.Count;i++)
            {
                DeleteCharacter(characterIds[i]);
            }
            
            List<int> objectIds = new List<int>(advObjects.Keys);
            for(int i=0;i<objectIds.Count;i++)
            {
                DeleteObject(objectIds[i]);
            }
            
            advCreateObjects.Clear();
        }
        
        /// <summary>全てのサウンドを破棄する</summary>
        public void ReleaseAllSounds()
        {
            // ボイス破棄
            if(voicePlayer != null)
            {
                AudioManager.Instance.ReleaseAudioPlayer(voicePlayer);
                voicePlayer = null;
            }
            // Se破棄
            foreach(AudioPlayer p in soundCache.Values)
            {
                AudioManager.Instance.ReleaseAudioPlayer(p);
            }
            
            if(bgmPlayer != null)
            {
                AudioManager.Instance.ReleaseAudioPlayer(bgmPlayer);
            }

            soundCache.Clear();
        }
        
        public void HideAllMessageWindows()
        {
            foreach(AdvMessageWindow messageWindow in messageWindows.Values)
            {
                messageWindow.gameObject.SetActive(false);
            }
        }
        
        public void HideAllTexts()
        {
            foreach(AdvTextObject text in texts.Values)
            {
                text.gameObject.SetActive(false);
            }
        }
        
        private void HideAllFade()
        {
            foreach(AdvFade fade in fades.Values)
            {
                fade.gameObject.SetActive(false);
            }
        }
        
        private void HideAllTexture()
        {
            foreach(AdvTextureObject t in textureObjects.Values)
            {
                t.gameObject.SetActive(false);
            }
        }
        
        /// <summary>表示切り替え</summary>
        public void ShowCharacter(int id, bool show)
        {
            AdvObject advObject = GetAdvCharacter<AdvObject>(id);
            if(advObject != null)
            {
                advObject.gameObject.SetActive(show);
            }
        }
        
        /// <summary>背景の読み込み</summary>
        public void SetTexture(int textureId, int resourceId, int id)
        {
            // テクスチャオブジェクトを取得
            AdvTextureObject textureObject = GetTextureObject(textureId);
            // テクスチャのセット
            Texture texture = LoadResource<Texture>(resourceId, id);
            textureObject.SetTexture(texture);
            // アクティブの設定
            if(texture != null)
            {
                textureObject.gameObject.SetActive(true);
            }
            
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            bool isNext = true;
            // 表示中のメッセージがあるかチェック
            foreach(AdvMessageWindow messageWindow in messageWindows.Values)
            {
                if(messageWindow.IsEndMesssageAnimation == false)
                {
                    isNext = false;
                    messageWindow.ForceEndMessageAnimation();
                }
            }
            
            if(isNext)
            {
                
                if(currentCommand is IAdvCommandOnNext c)
                {
                    c.OnNext(this);
                }
                
                isWaitClick = false;
            }
        }

        /// <summary>ログの削除</summary>
        public void ClearLog()
        {
            messageLogs.Clear();
        }
        
        /// <summary>スキップ実行</summary>
        public void Skip(AdvSkipMode mode)
        {
            SkipAsync(mode).Forget();
        }
        
        /// <summary>スキップ実行</summary>
        public async UniTask SkipAsync(AdvSkipMode mode)
        {
            isSkip = true;
            // 各モードごとに処理
            switch(mode)
            { 
                case AdvSkipMode.AllForceSkip:
                    await EndAsync();
                    break;
                
                case AdvSkipMode.Skip:
                    // 終わるまでコマンドを実行し続ける
                    while(true)
                    {
                        // 強制的に次のコマンドへ
                        ForceNextCommand();
                        // スキップ中断チェック
                        if(GetNextExecuteCommand() is IAdvCommandSkipBreak skip)
                        {
                            if(skip.IsSkipBreak(this))
                            {
                                isSkip = false;
                                break;
                            }
                        }
                        
                        // コマンドの実行
                        ExecuteCommand();
                        // 終了している場合
                        if(IsEnded)break;
                    }
                    break;
            }
            
            isSkip = false;
        }
        
        /// <summary>ログのセット</summary>
        public void SetLogs(AdvMessageLogData[] logs)
        {
            ClearLog();

            int start = Math.Max(0, logs.Length - Config.MaxLogCount);
            
            for(int i=start;i<logs.Length;i++)
            {
                AdvMessageLogData log = logs[i];
                messageLogs.Add( new AdvMessageLogData(this, log.Speaker, log.Message, log.SpeakerId));
                if(messageLogs.Count >= Config.MaxLogCount)break;
            }
        }

        private void Awake()
        {
            Initialize();
            // 通知
            OnAwake();
        }
        
        private void Update()
        {            
            // 一時停止中は処理しない
            if(isPause)return;
            
            OnUpdate();
            
            // 時間待機がある場合
            if(waitTime > 0)
            {
                waitTime -= Time.deltaTime;
            }
            
            // 早送りモード
            if(AutoMode == AdvAutoMode.Fast && IsExecuteCommand == false)
            {
                if(currentCommand is IAdvFastForward c)
                {
                    if(c.OnNext(this))
                    {
                        OnNextButton();
                    }
                }
                else
                {
                    OnNextButton();
                }
            }
            
            // 自動送り
            if(autoMessage > 0)
            {
                bool isEndMessage = true;
                foreach(AdvMessageWindow messageWindow in messageWindows.Values)
                {
                    if(messageWindow.IsNextAutoMessage == false)
                    {
                        isEndMessage = false;
                        break;
                    }
                }
                // メッセージウィンドウの表示が終わっている
                if(isEndMessage)
                {
                    autoMessageTimer += Time.deltaTime;
                    // 自動で次へ
                    if(autoMessageTimer >= autoMessage)
                    {
                        OnNextButton();
                        autoMessageTimer = 0;
                    }
                }
            }
            
            // コマンドの実行
            while(true)
            {
                if(IsExecuteCommand == false)
                {
                    break;
                }
                ExecuteCommand();
            }
        }
        
        private void Initialize()
        {
            if(selectRoot != null)
            {
                selectRoot.ParentManager = this;
            }
            
            
            // メッセージウィンドウの取得
            messageWindows.Clear();
            AdvMessageWindow[] advMessageWindows = gameObject.GetComponentsInChildren<AdvMessageWindow>(true);
            // 全て非表示にする
            foreach(AdvMessageWindow m in advMessageWindows)
            {
                messageWindows.Add(m.WindowId, m);
                m.gameObject.SetActive(false);
            }
            
            
            // レイヤーの取得
            objectLayers.Clear();
            AdvObjectLayer[] advObjectLayers = gameObject.GetComponentsInChildren<AdvObjectLayer>(true);
            foreach(AdvObjectLayer layer in advObjectLayers)
            {
                objectLayers.Add(layer.Id, layer);
            }
            
            // テクスチャオブジェクトの取得
            textureObjects.Clear();
            AdvTextureObject[] advTextureObjects = gameObject.GetComponentsInChildren<AdvTextureObject>(true);
            foreach(AdvTextureObject texture in advTextureObjects)
            {
                textureObjects.Add(texture.Id, texture);
            }
            
            // フェードの取得
            fades.Clear();
            AdvFade[] advFades = gameObject.GetComponentsInChildren<AdvFade>(true);
            foreach(AdvFade fade in advFades)
            {
                fade.gameObject.SetActive(false);
                fades.Add(fade.Id, fade);
            }
            
            // テキストの取得
            texts.Clear();
            AdvTextObject[] advTexts = gameObject.GetComponentsInChildren<AdvTextObject>(true);
            foreach(AdvTextObject text in advTexts)
            {
                text.gameObject.SetActive(false);
                texts.Add(text.Id, text);
            }
        }
        
        /// <summary>指定の位置まで移動</summary>
        public void Goto(ulong id)
        {          
            executeCommandIndex = 0;
            while(true)
            {
                if(executeCommandIndex >= executeCommands.Length)break;
                
                IAdvCommandObject advCommandObject = executeCommands[ executeCommandIndex++];
                
                if(advCommandObject is AdvCommandLocation location && location.Id == id)
                {
                    break;
                }
            }
        }
        
        
        /// <summary>コマンドの実行位置の変更</summary>
        public void GotoIndex(int index)
        {          
            executeCommandIndex = index;
        }
        
        private IAdvCommandObject GetNextExecuteCommand()
        {
            // 実行できない
            if(executeCommands == null || executeCommands.Length <= executeCommandIndex)return null;
            return executeCommands[executeCommandIndex];
        }
        
        private void ExecuteCommand()
        {
            // 実行できない
            if(executeCommands == null || executeCommands.Length <= executeCommandIndex)return;
            // 実行
            currentCommand = executeCommands[executeCommandIndex++];
            
            // スキップ有効
            OnSkipButtonEnable( currentCommand is IAdvEnableSkipButton enableSkipButton && enableSkipButton.EnableSkipButton() );
            
            try
            {
                switch(currentCommand)
                {
                    case IAdvCommand c:
                    {
                        c.Execute(this);
                        break;
                    }
                    
                    case IAdvCommandConditions c:
                    {
                        if(nextCase != null && nextCase.LocationId > 0 && c.GetConditions(this) == false)
                        {
                            if(nextCase.CommandIndex >= 0)
                            {
                                GotoIndex(nextCase.CommandIndex);
                            }
                            else
                            {
                                Goto(nextCase.LocationId);
                            }
                            
                            nextCase = null;
                        }
                        break;
                    }
                    
                    case AdvCommandNextCase c:
                        nextCase = c;
                        break;
                }
                
                if( (AutoMode == AdvAutoMode.Fast || IsSkip) && currentCommand is IAdvCommandSkip skip)
                {
                    skip.Skip(this);
                }
            }
            catch(Exception e)
            {
                ErrorLog(e);
            }
            
            // コールバック
            if(OnExecuteCommand != null)
            {
                OnExecuteCommand(currentCommand);
            }
        }
        
        /// <summary>終了時処理</summary>
        protected virtual UniTask OnEndAsync(){return default;}
        
        /// <summary>終了させる</summary>
        public async UniTask EndAsync()
        {
            // 既に終了済み
            if(isEnded)return;
            // スキップ無効化
            OnSkipButtonEnable(false);
            // 終了済みにする
            isEnded = true;
            // ログの削除
            if(isKeepLog == false)ClearLog();

            // 終了処理
            await OnEndAsync();
            
            // Bgmを戻す
            switch(BgmUsage)
            {
                case AdvBgmUsage.UseAudioManagerAndStash:
                    AudioManager.Instance.PlayStashBGM();
                    break;
                case AdvBgmUsage.UseAdvManager:
                    AudioManager.Instance.PlayBGM();
                    break;
            }
            
            // リソースの破棄
            Release();
            // 終了通知
            if(OnEnded != null)OnEnded();
            // 再生終了
            isPlaying = false;
            
        }
        
        public void End()
        {
            EndAsync().Forget();
        }
        
        public void Release()
        {
            // オブジェクトを破棄
            DeleteAllObjects();
            // メッセージウィンドウを非表示に
            HideAllMessageWindows();
            // テキストをすべて非表示
            HideAllTexts();
            // サウンドを破棄
            ReleaseAllSounds();
            // リソースの破棄
            resourceLoader.Release();
        }
        
        /// <summary>リスタート時の初期化</summary>
        protected virtual UniTask OnRestartAsync(){return default;}
        
        
        private async UniTask RestartAsync()
        {
            // オブジェクトを破棄
            DeleteAllObjects();
            // メッセージウィンドウを非表示に
            HideAllMessageWindows();
            // フェードを非表示に
            HideAllFade();
            // テクスチャを非表示に
            HideAllTexture();
            // テキストをすべて非表示
            HideAllTexts();
            
            // ボイス再生用
            if(voicePlayer == null)
            {
                voicePlayer = AudioManager.Instance.CreateVoicePlayer(null);
            }
            
            
            // スキップ無効化
            OnSkipButtonEnable(false);
            
            // 変数初期化
            isSkip = false;
            isEnded = false;
            executeCommandIndex = 0;
            currentSpeaker = null;
            isWaitClick = false;
            IsStopCommand = true;
            
            // 初期化処理
            await OnRestartAsync();
            IsStopCommand = false;
        }
        
        public void Restart()
        {
            RestartAsync().Forget();
        }
        
        /// <summary>ファイル読み込み</summary>
        public async UniTask LoadAdvFile(string path)
        {
            // スキップボタン無効化
            OnSkipButtonEnable(false);
            AdvDataFile file = await LoadResourceAsync<AdvDataFile>(path);
            await LoadAdvFile(file);
            OnLoadFile(path);
        }
        
        public async UniTask PreLoadAdvFile(string path)
        {
            AdvDataFile file = await LoadResourceAsync<AdvDataFile>(path);
            await PreLoadCommandsAsync(file.Commands);
        }
        
        /// <summary>ファイル読み込み</summary>
        private async UniTask LoadAdvFile(AdvDataFile file)
        {
            await LoadCommandsAsync(file.Commands);
        }

        private async UniTask PreLoadCommandsAsync(IAdvCommandObject[] commands)
        {
            List<UniTask> taskList = new List<UniTask>();
            
            // キャラクタプレハブ
            AdvCharacter characterPrefab = await LoadResourceAsync<AdvCharacter>(config.CharacterPrefabResourcePathId);
            
            // 必要なリソースをダウンロード
            foreach(IAdvCommandObject command in commands)
            {
                if(command is IAdvResource r)
                {
                    taskList.Add( r.PreLoadResources(this) );
                }
                if(command is IAdvCharacterResource c)
                {
                    taskList.Add( c.PreLoadResources(this, characterPrefab) );
                }
            }
            // すべてのタスクの完了を待つ
            await UniTask.WhenAll(taskList);
        }

        public async UniTask LoadCommandsAsync(IAdvCommandObject[] commands)
        {
            // 既に再生中
            if(isPlaying)
            {
                ErrorLog("再生中に新しく再生しようとした");
                return;
            }
            // 再生中に
            isPlaying = true;
            
            // Bgm
            switch(BgmUsage)
            {
                case AdvBgmUsage.UseAudioManagerAndStash:
                    AudioManager.Instance.StashBGM();
                    break;
                case AdvBgmUsage.UseAdvManager:
                    bgmPlayer = AudioManager.Instance.CreateBGMPlayer(null);
                    break;
            }
            
            IsStopCommand = true;
            // コマンド
            executeCommands = commands;
            await PreLoadCommandsAsync(commands);
            await RestartAsync();
        }
        
        public virtual void OpenDebugModal(IAdvCommandObject command, string message)
        {

        }
        
        [System.Diagnostics.Conditional("CRUFRAMEWORK_DEBUG")]
        public void ErrorLog(object log)
        {
            if(OnError != null)
            {
                OnError(log.ToString());
            }
            
            Logger.LogError(log);
        }
    }
}