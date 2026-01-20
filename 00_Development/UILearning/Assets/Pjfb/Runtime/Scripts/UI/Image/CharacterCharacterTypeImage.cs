using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pjfb
{
    public class CharacterCharacterTypeImage : CancellableImageWithEnum<CharacterType>
    {
        public enum ImageType
        {
            Badge,
            Icon
        }
        
        [SerializeField]
        private ImageType imageType = ImageType.Badge;
        
        protected override string GetAddress(CharacterType type)
        {
            return PageResourceLoadUtility.GetCharacterTypeImagePath(type, imageType);
        }
    }
}
