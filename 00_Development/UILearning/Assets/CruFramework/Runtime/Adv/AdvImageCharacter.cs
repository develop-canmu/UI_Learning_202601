using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Adv
{

    public class AdvImageCharacter : AdvUICharacter
    {
        
        [SerializeField]
        private RawImage image = null;

        public override void Grayout()
        {
            image.color = Color.gray;
        }

        public override void Highlight()
        {
            image.color = Color.white;
        }

        public override async UniTask PreLoadResource(AdvManager manager, string id)
        {
            await manager.PreLoadResourceAsync(manager.Config.CharacterUIResourcePathId, id);
        }

        public override void LoadResource(AdvManager manager, string id)
        {
            Texture2D texture = manager.LoadResource<Texture2D>(manager.Config.CharacterUIResourcePathId, id);
            image.texture = texture;
            RectTransform rt = (RectTransform)image.transform;
            rt.sizeDelta = new Vector2(texture.width, texture.height);
        }
    }
}
