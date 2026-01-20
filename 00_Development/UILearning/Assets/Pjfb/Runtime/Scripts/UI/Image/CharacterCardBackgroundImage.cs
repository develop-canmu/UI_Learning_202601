using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CharacterCardBackgroundImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardBackgroundImagePath(id);
        }

        protected override void OnPreLoadTexture()
        {
            // 背景画像なので非表示にしない
        }
    }
}