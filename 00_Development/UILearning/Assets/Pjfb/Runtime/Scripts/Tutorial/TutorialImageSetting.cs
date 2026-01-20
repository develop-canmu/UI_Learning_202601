using System;
using System.Linq;
using UnityEngine;

namespace Pjfb
{
    [CreateAssetMenu]
    public class TutorialImageSetting : ScriptableObject
    {
        [Serializable]
        public class ImageSetting
        {
            public string key;
            public string[] imagePathList;
        }
        [SerializeField]
        private ImageSetting[] settings;

        public string[] GetImagePath(string key)
        {
            return settings.FirstOrDefault(data => data.key == key)?.imagePathList;
        }
    }
}