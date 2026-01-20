using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;
using UnityEditor;

namespace CruFramework.Editor.Adv.Convert
{
    public abstract class ConvertDataId<T> where T : new()
    {
        [System.Serializable]
        public class ValueData
        {
            [SerializeField]
            public int id = 0;
            [SerializeField]
            public T value = new T();
        }
        
        public int allocateId = 0;
        public List<ValueData> values = new List<ValueData>();
        
        public ValueData Get(int id)
        {
            foreach(ValueData value in values)
            {
                if(value.id == id)return value;
            }
            return null;
        }
        
        public void Remove(int id)
        {
            foreach(ValueData value in values)
            {
                if(value.id == id)
                {
                    values.Remove(value);
                    return;
                }
            }
        }
        
        public ValueData Add(T value)
        {
            ValueData v = new ValueData();
            v.id = ++allocateId;
            v.value = value;
            values.Add(v);
            return v;
        }
    }
    
    [System.Serializable]
    public class CharacterData
    {
        [SerializeField]
        private string name = string.Empty;
        [SerializeField]
        private string internalName = string.Empty; 
        [SerializeField]
        public string nameRuby = string.Empty;
        [SerializeField]
        public string resourceId = string.Empty;
        [SerializeField]
        public string iconId = string.Empty;
    }
    
    [System.Serializable]
    public class ObjectData
    {
        [SerializeField]
        public int category = 0;
        [SerializeField]
        public string name = string.Empty;
        [SerializeField]
        public string resourceId = string.Empty;
    }
    
    [System.Serializable]
    public class ObjectCategoryData
    {
        [SerializeField]
        public string name = string.Empty;
        [SerializeField]
        public int resourceKey = 0;
    }
    
    [System.Serializable]
    public class ResourcePathData
    {
        [SerializeField]
        public string name = string.Empty;
        [SerializeField]
        public string path = string.Empty;
    }
    
    [System.Serializable]
    public class StringData
    {
        [SerializeField]
        public string name = string.Empty;
    }
    
    [System.Serializable]
    public class SoundResourceData
    {
        [SerializeField]
        public string sheet = string.Empty;
        [SerializeField]
        public string cue = string.Empty;
        [SerializeField]
        public string extension = ".ogg";
    }
    
    [System.Serializable]
    public class SoundData
    {
        [SerializeField]
        private string name = string.Empty;
        [SerializeField]
        private SoundResourceData resourceData = new SoundResourceData();
        
        public SoundData()
        {
            
        }
        
        public SoundData(string name, string sheet, string cue)
        {
            this.name = name;
            resourceData.sheet = sheet;
            resourceData.cue = cue;
        }
    }
    
    [System.Serializable]
    public class TextureData
    {
        [SerializeField]
        private string name = string.Empty;
        [SerializeField]
        private int resourceKey = 0;
    }
    
    [System.Serializable]
    public class TextData
    {
        [SerializeField]
        public string name = string.Empty;
    }
    
    [System.Serializable]
    public class FadeData
    {
        [SerializeField]
        public string name = string.Empty;

        public FadeData()
        {
        }
        
        public FadeData(string name)
        {
            this.name = name;
        }
    }
    
    [System.Serializable]
    public class PositionData
    {
        [SerializeField]
        private string name = string.Empty;
 
        [SerializeField]
        private Vector3 position = Vector3.zero;
    }
    
    [System.Serializable]
    public class CruAdvSettingsAnimationLayerData
    {
        [SerializeField]
        private string name = string.Empty;
    }
    
    [System.Serializable]
    public class CruAdvSettingsAnimationData
    {
        [SerializeField]
        private string name = string.Empty;
        
        [SerializeField]
        private int layer = 0;
    }
    
    [System.Serializable]
    public class AdvSpineAnimationNameId
    {
        [SerializeField]
        private string name = string.Empty;
        [SerializeField]
        private bool isEyeBlink = true;
        [SerializeField]
        private bool isLipSync = true;
        [SerializeField]
        private int bodyAnimationId = 0;
        [SerializeField]
        private int faceAnimationId = 0;
        [SerializeField]
        private int eyeAnimationId = 0;
        [SerializeField]
        private int eyeBlinkAnimationId = 0;
        [SerializeField]
        private int mouthAnimationId = 0;
    }
    
    [System.Serializable]
    public class CruAdvSettingsAnimationLayerDataIds : ConvertDataId<CruAdvSettingsAnimationLayerData>{}
    
    [System.Serializable]
    public class CruAdvSettingsAnimationDataIds : ConvertDataId<CruAdvSettingsAnimationData>{ }
    
    [System.Serializable]
    public class CruAdvSettingsAnimationPlayDataIds : ConvertDataId<AdvSpineAnimationNameId>{ }

        

    
    [System.Serializable]
    public abstract class CruAdvSettingsExtension
    {
        
    }
    
    public class CruAdvSpineSettings : CruAdvSettingsExtension
    {
        
        [SerializeField]
        public int defaultAnimation = 0;
        
        [SerializeField]
        public CruAdvSettingsAnimationLayerDataIds animationLayerDatas = new CruAdvSettingsAnimationLayerDataIds();
        
        [SerializeField]
        public CruAdvSettingsAnimationDataIds animationDatas = new CruAdvSettingsAnimationDataIds();

        [SerializeField]
        public CruAdvSettingsAnimationPlayDataIds animationPlayDatas = new CruAdvSettingsAnimationPlayDataIds();

    }


    
    [System.Serializable]
    public class CharacterDataIds : ConvertDataId<CharacterData>{ }
    [System.Serializable]
    public class ObjectDataIds : ConvertDataId<ObjectData>{ }
    [System.Serializable]
    public class ObjectCategoryDataIds : ConvertDataId<ObjectCategoryData>{ }
    
    [System.Serializable]
    public class ResourcePathDataIds : ConvertDataId<ResourcePathData>{ }
    
    [System.Serializable]
    public class SoundDataIds : ConvertDataId<SoundData>{ }
    
    [System.Serializable]
    public class TextureDataIds : ConvertDataId<TextureData>{ }
    
    [System.Serializable]
    public class TextDataIds : ConvertDataId<TextData>{ }
    
    [System.Serializable]
    public class FadeDataIds : ConvertDataId<FadeData>{ }
    
    [System.Serializable]
    public class CommonDataIds : ConvertDataId<StringData>{ }
    
    [System.Serializable]
    public class PositionDataIds : ConvertDataId<PositionData>{ }
    
    [System.Serializable]
    public class StringDataIds : ConvertDataId<StringData>{ }
    
    [System.Serializable]
    public class ConverData : ScriptableObject
    {
        [SerializeField]
        public int maxLogCount = 300;
 	    
        [SerializeField]
        public int characterPrefabResourceKey = 0;
	    
        [SerializeField]
        public int characterImageResourceKey = 0;
	    
        [SerializeField]
        public int backgroundResourceKey = 0;
	    
        [SerializeField]
        public int characterLayerKey = 0;
	    
        [SerializeField]
        public int voiceSoundKey = 0;
	    
        [SerializeField]
        public int seSoundKey = 0;
	    
        [SerializeField]
        public int bgmSoundKey = 0;
        
        [SerializeField]
        public CharacterDataIds characterDatas = new CharacterDataIds();
        [SerializeField]
        public ObjectDataIds objectDatas = new ObjectDataIds();
        [SerializeField]
        public ObjectCategoryDataIds objectCategoryDatas = new ObjectCategoryDataIds();
        [SerializeField]
        public CommonDataIds objectLayerDatas = new CommonDataIds();
        [SerializeField]
        public ResourcePathDataIds resourceDatas = new ResourcePathDataIds();
        [SerializeField]
        public SoundDataIds soundDatas = new SoundDataIds();
        [SerializeField]
        public TextureDataIds textureDatas = new TextureDataIds();
        [SerializeField]
        public TextDataIds textDatas = new TextDataIds();
        [SerializeField]
        public FadeDataIds fadeDatas = new FadeDataIds();
        [SerializeField]
        public PositionDataIds positionDatas = new PositionDataIds();
        [SerializeField]
        public StringDataIds stringDatas = new StringDataIds();
        [SerializeField]
        public CommonDataIds messageWindowDatas = new CommonDataIds();
        
        [SerializeField]
        public CommonDataIds animationLayerDatas = new CommonDataIds();
        
        [SerializeReference]
        public List<CruAdvSettingsExtension> extensions = new List<CruAdvSettingsExtension>();
    }
    
    
    public class AdvEngineConvertEditor
    {
        // 誤爆しない様に一旦メニューに表示されない様に
        // [MenuItem("CruFramework/Adv/EngineConvert/Config")]
        public static void Convert()
        {
            // 設定ファイルの読み込み
            AdvConfig config = AssetDatabase.LoadAssetAtPath<AdvConfig>("Assets/Pjfb/Runtime/AssetBundles/Remote/Adv/AdvConfig.asset");
            
            // 変換用クラス
            ConverData c = ScriptableObject.CreateInstance<ConverData>();
            
            // ログ数
            c.maxLogCount = config.MaxLogCount;
            // キャラフレハブ
            c.characterPrefabResourceKey = config.CharacterPrefabResourcePathId;
            // キャライメージ
            c.characterImageResourceKey = config.SpineResourcePathId;
            // 背景
            c.backgroundResourceKey = config.BackgroundResourcePathId;
            // キャラレイヤー
            c.characterLayerKey = config.CharacterLayerId;
            
            // オブジェクトカテゴリ
            c.resourceDatas = ConvertValue<ResourcePathDataIds>(config.ResourcePaths);

            // サウンド関連はSoundに移動
            c.resourceDatas.Remove(3);
            c.resourceDatas.Remove(6);
            c.resourceDatas.Remove(10);
            // 背景のパス変更
            c.resourceDatas.Get(8).value.path = "Images/Background/img_background_{0}.png";
            
            // キャラ
            c.characterDatas = ConvertValue<CharacterDataIds>(config.CharacterDatas);
            // オブジェクト
            c.objectDatas = ConvertValue<ObjectDataIds>(config.ObjectDatas);
            // オブジェクトカテゴリ
            c.objectCategoryDatas = ConvertValue<ObjectCategoryDataIds>(config.ObjectCategories);
            // オブジェクトレイヤー
            c.objectLayerDatas = ConvertCommonValue<CommonDataIds>(config.ObjectLayers);
            // テクスチャ
            c.textureDatas = ConvertValue<TextureDataIds>(config.Textures); 
            // テキスト
            c.textDatas = ConvertValue<TextDataIds>(config.Texts); 
            // フェード
            c.fadeDatas = ConvertValue<FadeDataIds>(config.Fades); 
            
            c.fadeDatas.Get(1).value.name = "ColorFadeIn";
            c.fadeDatas.Add( new FadeData("ColorFadeOut"));
            c.fadeDatas.Add( new FadeData("FadeIn"));
            c.fadeDatas.Add( new FadeData("FadeOut"));
            c.fadeDatas.Add( new FadeData("PassingOfTime"));
            // メッセージウィンドウ
            c.messageWindowDatas = ConvertCommonValue<CommonDataIds>(config.MessageWindows);
            // 座標
            c.positionDatas = ConvertValue<PositionDataIds>(config.Positions); 
            // 文字列
            c.stringDatas = ConvertValue<StringDataIds>(config.StringDatas); 
            
            // サウンド
            c.soundDatas = new SoundDataIds();
            c.soundDatas.Add(new SoundData("SE", "Sound/Se", "{0}"));
            c.soundDatas.Add(new SoundData("BGM", "Sound/Bgm", "{0}"));
            c.soundDatas.Add(new SoundData("Voice", "Sound/Voice/Scenario", "{0}"));
            
            c.seSoundKey = 1;
            c.bgmSoundKey = 2;
            c.voiceSoundKey = 3;
            
            // スパイン
            CruAdvSpineSettings　spineSettings = new CruAdvSpineSettings();
            c.extensions.Add(spineSettings);

            
            // デフォルトId
            spineSettings.defaultAnimation = config.DefaultSpineFaceId;
            // レイヤー
            spineSettings.animationLayerDatas = ConvertExtensionValue<CruAdvSettingsAnimationLayerDataIds>(config.SpineAnimationLayers); 
            // アニメーション
            spineSettings.animationDatas = ConvertExtensionValue<CruAdvSettingsAnimationDataIds>(config.SpineAnimationNames); 
            // 再生用データ
            spineSettings.animationPlayDatas = ConvertExtensionValue<CruAdvSettingsAnimationPlayDataIds>(config.SpineFaces); 

            
            // Json
            string json = JsonUtility.ToJson(c);
            // 参照の名前を修正
            json = json.Replace("\"CruFramework.Editor.Adv.Convert\"", "\"CruEngine\"");
            json = json.Replace("\"Assembly-CSharp-Editor\"", "\"Cru.AdvSpine.Runtime\"");
            
            
            // ファイル出力
            System.IO.File.WriteAllText("adv_convert.json", json );
        }
        
        private static T ConvertCommonValue<T>(AdvObjectIds<AdvStringId> value)
        {
            string json = JsonUtility.ToJson(value);
            json = json.Replace("{\"value\"" ,"{\"name\"");
            return JsonUtility.FromJson<T>(json);
        }
        
        private static T ConvertExtensionValue<T>(object value)
        {
            string json = JsonUtility.ToJson(value);
            json = json.Replace("{\"value\"" ,"{\"name\"");
            return JsonUtility.FromJson<T>(json);
        }
        
        private static T ConvertValue<T>(object value)
        {
            string json = JsonUtility.ToJson(value);
            json = json.Replace("{id}", "{0}");
            return JsonUtility.FromJson<T>(json);
        }
    }
}