using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    public abstract class AdvConfig : ScriptableObject
    {
	    
	    [SerializeField]
	    private int maxLogCount = 300;
	    /// <summary>ログを保存する最大数</summary>
	    public int MaxLogCount{get{return maxLogCount;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int characterPrefabResourcePathId = 0;
	    /// <summary>キャラクタのプレハブ読み込み先</summary>
	    public int CharacterPrefabResourcePathId{get{return characterPrefabResourcePathId;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int characterUIResourcePathId = 0;
	    /// <summary>UIキャラクタ用のリソースパス</summary>
	    public int CharacterUIResourcePathId{get{return characterUIResourcePathId;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int backgroundResourcePathId = 0;
	    /// <summary>背景のリソースパス</summary>
	    public int BackgroundResourcePathId{get{return backgroundResourcePathId;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ObjectLayers))]
	    private int characterLayerId = 0;
	    /// <summary>キャラクタ配置レイヤー</summary>
	    public int CharacterLayerId{get{return characterLayerId;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int voiceResourcePathId = 0;
	    /// <summary>ボイス読み込み先</summary>
	    public int VoiceResourcePathId{get{return voiceResourcePathId;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int seResourcePathId = 0;
	    /// <summary>SE読み込み先</summary>
	    public int SeResourcePathId{get{return seResourcePathId;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int bgmResourcePathId = 0;
	    /// <summary>Bgm読み込み先</summary>
	    public int BgmResourcePathId{get{return bgmResourcePathId;}}
	    
#if CRUFRAMEWORK_SPINE_SUPPORT
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int spineResourcePathId = 0;
	    /// <summary>スパインの読み込み先</summary>
	    public int SpineResourcePathId{get{return spineResourcePathId;}}
#endif    
	    
	    [SerializeField]
	    [AdvObjectId(nameof(ResourcePaths))]
	    private int speakerIconResourcePathId = 0;
	    /// <summary>話者用のアイコンパス</summary>
	    public int SpeakerIconResourcePathId{get{return speakerIconResourcePathId;}}
	    
	    [SerializeField]
	    private AdvCharacterDataIds characterDatas = new AdvCharacterDataIds();
	    /// <summary>キャラクタのデータ</summary>
	    public AdvCharacterDataIds CharacterDatas{get{return characterDatas;}}
	    
	    [SerializeField]
        private AdvObjectDataIds objectDatas = new AdvObjectDataIds();
        /// <summary>オブジェクトデータ</summary>
        public AdvObjectDataIds ObjectDatas{get{return objectDatas;}}
        
        [SerializeField]
        private AdvObjectCategoryIds objectCategories = new AdvObjectCategoryIds();
        /// <summary>オブジェクトカテゴリデータ</summary>
        public AdvObjectCategoryIds ObjectCategories{get{return objectCategories;}}
        
        [SerializeField]
        private AdvSoundIds sounds = new AdvSoundIds();
        /// <summary>サウンドデータ</summary>
        public AdvSoundIds Sounds{get{return sounds;}}
        
        [SerializeField]
        private AdvTextureIds textures = new AdvTextureIds();
        /// <summary>テクスチャデータ</summary>
        public AdvTextureIds Textures{get{return textures;}}
        
        [SerializeField]
        private AdvTextIds texts = new AdvTextIds();
        /// <summary>テキストデータ</summary>
        public AdvTextIds Texts{get{return texts;}}
        
        [SerializeField]
        private AdvFadeIds fades = new AdvFadeIds();
        /// <summary>フェード</summary>
        public AdvFadeIds Fades{get{return fades;}}
        
        [SerializeField]
        private AdvObjectLayerIds objectLayers = new AdvObjectLayerIds();
        /// <summary>オブジェクトレイヤーデータ</summary>
        public AdvObjectLayerIds ObjectLayers{get{return objectLayers;}}
        
        [SerializeField]
        private AdvResourcePaths resourcePaths = new AdvResourcePaths();
        /// <summary>リソースパス</summary>
        public AdvResourcePaths ResourcePaths{get{return resourcePaths;}}
        
        [SerializeField]
        private AdvMessageWindowIds messageWindows = new AdvMessageWindowIds();
        /// <summary>メッセージウィンドウ</summary>
        public AdvMessageWindowIds MessageWindows{get{return messageWindows;}}
        
        [SerializeField]
        private AdvPositionIds positions = new AdvPositionIds();
        /// <summary>座標データ</summary>
        public AdvPositionIds Positions{get{return positions;}}
        
        [SerializeField]
        private AdvStringDataIds stringDatas = new AdvStringDataIds();
        /// <summary>文字列データ</summary>
        public AdvStringDataIds StringDatas{get{return stringDatas;}}
        
#if CRUFRAMEWORK_SPINE_SUPPORT

	    [SerializeField]
	    [AdvObjectId(nameof(SpineFaces))]
	    private int defaultSpineFaceId = 0;
	    /// <summary>デフォルトの表情</summary>
	    public int DefaultSpineFaceId{get{return defaultSpineFaceId;}}
	    
	    [SerializeField]
	    private AdvSpineFaceIds spineFaces = new AdvSpineFaceIds();
	    /// <summary>表情データ</summary>
	    public AdvSpineFaceIds SpineFaces{get{return spineFaces;}}
	    
        [SerializeField]
        private AdvSpineAnimationLayerIds spineAnimationLayers = new AdvSpineAnimationLayerIds();
        /// <summary>オブジェクト名データ</summary>
        public AdvSpineAnimationLayerIds SpineAnimationLayers{get{return spineAnimationLayers;}}
        
        [SerializeField]
        private AdvSpineAnimationNameIds spineAnimationNames = new AdvSpineAnimationNameIds();
        /// <summary>オブジェクト名データ</summary>
        public AdvSpineAnimationNameIds SpineAnimationNames{get{return spineAnimationNames;}}
#endif
    }
}