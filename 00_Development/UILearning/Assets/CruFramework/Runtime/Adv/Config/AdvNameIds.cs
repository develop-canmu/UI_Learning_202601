using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CruFramework.Adv
{
    
    [System.Serializable]
    public class AdvObjectCategoryId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.ResourcePaths))]
        private int resourceKey = 0;
        /// <summary>リソースのキー</summary>
        public int ResourceKey{get{return resourceKey;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvObjectDataId : IAdvObjectId, IAdvObjectCategory
    {
        [SerializeField][HideInInspector]
        [AdvObjectId(nameof(AdvConfig.ObjectCategories))]
        private int category = 0;
        /// <summary>カテゴリ</summary>
        public int Category{get{return category;}}
        
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        private string resourceId = string.Empty;
        /// <summary>リソースのId</summary>
        public string ResourceId{get{return resourceId;}}
        
        string IAdvObjectId.GetString(){return name;}
        void IAdvObjectCategory.SetCategory(int category){this.category = category;}
    }
    
    [System.Serializable]
    public class AdvCharacterDataId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        private string internalName = string.Empty;
        /// <summary>内部で使う名前</summary>
        public string InternalName{get{return internalName;}}
        
        [SerializeField]
        private string nameRuby = string.Empty;
        /// <summary>名前のルビ</summary>
        public string NameRuby{get{return nameRuby;}}
        
        [SerializeField]
        private string resourceId = string.Empty;
        /// <summary>リソースのId</summary>
        public string ResourceId{get{return resourceId;}}
        
        [SerializeField]
        private string iconId = string.Empty;
        /// <summary>アイコンのId</summary>
        public string IconId{get{return iconId;}}
        
        string IAdvObjectId.GetString()
        {
            if(string.IsNullOrEmpty(internalName))return $"[{resourceId}] {name}";
            return $"[{resourceId}] {internalName}";
        }
    }
    
    [System.Serializable]
    public class AdvResourcePath : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        private string path = string.Empty;
        /// <summary>リソースのパス</summary>
        public string Path{get{return path;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvSoundId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.ResourcePaths))]
        private int resourceKey = 0;
        /// <summary>リソースのキー</summary>
        public int ResourceKey{get{return resourceKey;}}
        
        [SerializeField]
        private string resourceId = string.Empty;
        /// <summary>リソースのパス</summary>
        public string ResourceId{get{return resourceId;}}
        
        [SerializeField]
        private bool isBgm = false;
        /// <summary>BGM</summary>
        public bool IsBgm{get{return isBgm;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvTextureId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.ResourcePaths))]
        private int resourceKey = 0;
        /// <summary>リソースのキー</summary>
        public int ResourceKey{get{return resourceKey;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvTextId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvFadeId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvStringDataId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvObjectDataIds : AdvObjectIds<AdvObjectDataId>, IHasAdvObjectCategory
    {

    }
    
    [System.Serializable]
    public class AdvCharacterDataIds : AdvObjectIds<AdvCharacterDataId>
    {

    }
    
    [System.Serializable]
    public class AdvObjectCategoryIds : AdvObjectIds<AdvObjectCategoryId>
    {

    }
    
    [System.Serializable]
    public class AdvMessageWindowIds : AdvObjectIds<AdvStringId>
    {

    }
    
    [System.Serializable]
    public class AdvResourcePaths : AdvObjectIds<AdvResourcePath>
    {

    }
    
    [System.Serializable]
    public class AdvObjectLayerIds : AdvObjectIds<AdvStringId>
    {

    }
    
    [System.Serializable]
    public class AdvSoundIds : AdvObjectIds<AdvSoundId>
    {

    }
    
    [System.Serializable]
    public class AdvTextureIds : AdvObjectIds<AdvTextureId>
    {

    }
    
    [System.Serializable]
    public class AdvTextIds : AdvObjectIds<AdvTextId>
    {

    }
    
    [System.Serializable]
    public class AdvFadeIds : AdvObjectIds<AdvFadeId>
    {

    }
    
    [System.Serializable]
    public class AdvStringDataIds : AdvObjectIds<AdvStringDataId>
    {

    }
}