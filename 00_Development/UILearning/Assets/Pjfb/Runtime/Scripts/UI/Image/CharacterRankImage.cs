using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class CharacterRankImage : CancellableRawImageWithId
    {
        public enum Size
        {
            Large,
            Small
        }
        
        [SerializeField]
        private Size size = Size.Small;
        
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterRankIconImagePath(id, size);
        }
    }
}

