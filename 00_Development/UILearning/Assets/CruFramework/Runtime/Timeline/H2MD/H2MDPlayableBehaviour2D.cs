using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class H2MDPlayableBehaviour2D : H2MDPlayableBehaviour
    {
        internal SpriteRenderer target = null;
        private Sprite sprite = null; 
        
        protected override void OnLoadAsset(Texture2D texture)
        {
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
            target.sprite = sprite;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);
            if(sprite != null)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(sprite);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(sprite);
                }

                if (target != null)
                {
                    target.sprite = null;
                }
                sprite = null;
            }
        }
    }
}
