
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvSpineAnimationNameId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.SpineAnimationLayers))]
        private int layer = 0;
        /// <summary>レイヤー</summary>
        public int Layer{get{return layer;}}
        
        string IAdvObjectId.GetString(){return name;}
    }
    
    [System.Serializable]
    public class AdvSpineFaceId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        private bool isEyeBlink = true;
        /// <summary>目パチする</summary>
        public bool IsEyeBlink{get{return isEyeBlink;}}
        
        [SerializeField]
        private bool isLipSync = true;
        /// <summary>口パクする</summary>
        public bool IsLipSync{get{return isLipSync;}}
        
        [SerializeField][AdvObjectId(nameof(AdvConfig.SpineAnimationNames))]
        private int bodyAnimationId = 0;
        /// <summary>体</summary>
        public int BodyAnimationId{get{return bodyAnimationId;}}
        
        [SerializeField][AdvObjectId(nameof(AdvConfig.SpineAnimationNames))]
        private int faceAnimationId = 0;
        /// <summary>顔</summary>
        public int FaceAnimationId{get{return faceAnimationId;}}
        
        [SerializeField][AdvObjectId(nameof(AdvConfig.SpineAnimationNames))]
        private int eyeAnimationId = 0;
        /// <summary>目</summary>
        public int EyeAnimationId{get{return eyeAnimationId;}}
        
        [SerializeField][AdvObjectId(nameof(AdvConfig.SpineAnimationNames))]
        private int eyeBlinkAnimationId = 0;
        /// <summary>瞬き</summary>
        public int EyeBlinkAnimationId{get{return eyeBlinkAnimationId;}}
        
        [SerializeField][AdvObjectId(nameof(AdvConfig.SpineAnimationNames))]
        private int mouthAnimationId = 0;
        /// <summary>口</summary>
        public int MouthAnimationId{get{return mouthAnimationId;}}

        string IAdvObjectId.GetString(){return name;}
    }

    [System.Serializable]
    public class AdvSpineAnimationNameIds : AdvObjectIds<AdvSpineAnimationNameId>
    {

    }
    
    [System.Serializable]
    public class AdvSpineAnimationLayerIds : AdvObjectIds<AdvStringId>
    {

    }
    
    [System.Serializable]
    public class AdvSpineFaceIds : AdvObjectIds<AdvSpineFaceId>
    {

    }
}

#endif // CRUFRAMEWORK_SPINE_SUPPORT